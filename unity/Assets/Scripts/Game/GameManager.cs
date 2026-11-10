using System.Collections.Generic;
using UnityEngine;

namespace Tapestry
{
    /// <summary>Runs the whole game: title & knight select, the level, camera, parallax, HUD and touch buttons.
    /// It builds everything from code, so it works in any scene (Bootstrap adds it automatically).</summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager I { get; private set; }

        enum State { Title, Playing, Paused, Dying, Won, GameOver, Entering }
        State state = State.Title;

        const float ViewHalfHeight = 5.625f;           // 540 design px tall = 11.25 tiles
        static readonly Color Linen = new Color(0.925f, 0.886f, 0.788f);
        static readonly Color Thread = new Color(0.2f, 0.145f, 0.114f);

        Camera cam;
        Transform world, backdrop;
        Level level;
        Knight player;
        Bayard bayard;
        readonly List<Enemy> enemies = new List<Enemy>();
        readonly List<Shot> shots = new List<Shot>();
        readonly List<Bezant> bezants = new List<Bezant>();
        readonly List<Debris> debris = new List<Debris>();
        readonly List<Item> items = new List<Item>();
        readonly List<Lift> lifts = new List<Lift>();
        Lift riding;   // the lift the knight stands on
        // halfway banner
        SpriteRenderer checkpointView;
        Vector2 checkpointPos;
        bool hasCheckpoint, checkpointClaimed;
        // what the knight carries on into the next level
        bool carry; int carryHearts; bool carryCrossbow, carryMounted;
        // castles: flails, the boss, the axe that drops the bridge, and the rescued monk
        readonly List<Flail> flails = new List<Flail>();
        readonly List<Log> logs = new List<Log>();
        readonly List<Vector2> logPiles = new List<Vector2>(); readonly List<float> logTimers = new List<float>();
        BossBase boss;
        bool toldOfBoss;
        bool bossMusic, deathFire;
        int relicCombo;   // beasts smitten in a row by the holy relique
        // the crossing: gusts of wind, and fish leaping from the sea
        float windT, wind, leapT; bool windToldOf;
        public bool Underwater { get { return L.Underwater; } }
        // the level's tyme: counts down Mario-fashion; at 100 the musick quickens; at 0 the knight is lost
        const float TymeTick = 0.4f;
        float tymeLeft; bool hurried, deathTyme; int tymeBonus;
        SpriteRenderer axeView; Vector2 axePos; bool hasAxe, collapsing;
        List<Vector2Int> collapseTiles; float collapseT, collapseDoneT;
        SpriteRenderer monkView; Vector2 monkPos; bool hasMonk, rescuedMonk;
        // title screen: 0 = choose a knight, 1 = choose a realme (only realmes already passed)
        int titleStep, realmChoice;
        readonly List<int> realmStarts = new List<int>();

        static string PassedKey(string title) { return "passed:" + title; }
        static bool Passed(string title) { return PlayerPrefs.GetInt(PassedKey(title), 0) == 1; }

        /// <summary>The first level of each realme (e.g. 1-3 starts at Ye Crypte Gate) that may be chosen:
        /// 1-1 always, and any realme already passed.</summary>
        void FindRealms()
        {
            realmStarts.Clear();
            string seen = null;
            for (int i = 0; i < Levels.All.Length; i++)
            {
                var t = Levels.All[i].Title;
                if (t == seen) continue;
                seen = t;
                if (i == 0 || Passed(t)) realmStarts.Add(i);
            }
        }

        void StartAt(int index)
        {
            levelIndex = index; checkpointClaimed = false;
            LoadLevel();
            player.SetKnight(Knights.All[knightIndex]);
            Audio.Play("confirm");
            SetState(State.Playing);
        }
        string toast; float toastTime;
        class Popup { public Vector2 Pos; public string Text; public float T; }
        readonly List<Popup> popups = new List<Popup>();
        readonly List<Layer> layers = new List<Layer>();

        int knightIndex, bezantCount, lives = 3, score, levelIndex;
        LevelDef L { get { return Levels.All[levelIndex]; } }
        string farKey, nearKey;
        float stateTime, camX, winTime;
        bool deathFell;
        int winPhase;   // 0 slide down the pole, 1 walk to the castle, 2 inside

        class Layer { public Transform[] Copies; public float Period, Speed; }

        // ------------------------------------------------------------------ setup
        void Awake()
        {
            if (I != null && I != this) { Destroy(gameObject); return; }
            I = this;
            if (Application.isMobilePlatform) { Application.targetFrameRate = 60; Screen.sleepTimeout = SleepTimeout.NeverSleep; }
            SetupCamera();
            Audio.Init(gameObject);
            SetupBackdrop();
            LoadLevel();
            SetState(State.Title);
        }

        void SetupCamera()
        {
            cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera");
                go.tag = "MainCamera";
                cam = go.AddComponent<Camera>();
            }
            if (Object.FindAnyObjectByType<AudioListener>() == null) cam.gameObject.AddComponent<AudioListener>();
            cam.orthographic = true;
            cam.orthographicSize = ViewHalfHeight;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Linen;
            cam.transform.position = new Vector3(0f, ViewHalfHeight, -10f);
        }

        float HalfWidth { get { return ViewHalfHeight * cam.aspect; } }

        void SetupBackdrop()
        {
            // rebuilt when a level asks for different background layers
            if (backdrop != null && farKey == L.FarLayer && nearKey == L.NearLayer) return;
            if (backdrop != null) Destroy(backdrop.gameObject);
            layers.Clear();
            farKey = L.FarLayer; nearKey = L.NearLayer;
            backdrop = new GameObject("Backdrop").transform;
            AddLayer("backgrounds/linen", 0f, 0f, -100, 3);
            AddLayer(L.FarLayer, 0.25f, 0f, -90, 3);
            AddLayer(L.NearLayer, 0.5f, -0.6f, -80, 3);
            AddLayer("scenery/border", 1f, ViewHalfHeight * 2f - 64f / Art.DesignPerTile, 90, 7);
        }

        void AddLayer(string key, float speed, float y, int order, int copies)
        {
            var sheet = Art.Sheet(key);
            var l = new Layer { Speed = speed, Period = sheet.FrameW / Art.DesignPerTile, Copies = new Transform[copies] };
            for (int i = 0; i < copies; i++)
                l.Copies[i] = Art.Make(key, backdrop, new Vector2(0f, y), order).transform;
            layers.Add(l);
        }

        void UpdateLayers()
        {
            foreach (var l in layers)
            {
                float offset = camX * (1f - l.Speed);
                int k = Mathf.FloorToInt((camX - offset) / l.Period);
                int m = l.Copies.Length / 2;
                for (int i = 0; i < l.Copies.Length; i++)
                {
                    var p = l.Copies[i].localPosition;
                    p.x = offset + (k + i - m) * l.Period + l.Period * 0.5f;
                    l.Copies[i].localPosition = p;
                }
            }
        }

        void LoadLevel()
        {
            if (world != null) Destroy(world.gameObject);
            enemies.Clear(); shots.Clear(); bezants.Clear(); debris.Clear(); items.Clear(); lifts.Clear(); riding = null; popups.Clear(); hasCheckpoint = false; checkpointView = null;
            windT = 3f; wind = 0f; leapT = 2f; windToldOf = false;
            tymeLeft = L.Time; hurried = false; deathTyme = false; Audio.MusicPitch(1f);
            flails.Clear(); logs.Clear(); logPiles.Clear(); logTimers.Clear(); boss = null; toldOfBoss = false; bossMusic = false; hasAxe = collapsing = false; hasMonk = rescuedMonk = false; bayard = null; winPhase = 0; toast = null;
            SetupBackdrop();
            world = new GameObject("World").transform;
            level = new Level(L.Map, world, L);

            foreach (var d in L.Props)
                Art.Make(d.Key, world, new Vector2(d.X, level.GroundAt(d.X)), -20);
            // the castle at the end, centred on its doorway; drawn behind the knight, who walks in and vanishes
            if (!string.IsNullOrEmpty(L.Castle)) Art.Make(L.Castle, world, new Vector2(L.CastleX, level.GroundAt(L.CastleX)), -5);

            foreach (var s in level.Spawns)
            {
                Vector2 p = s.Value;
                string kind = null;
                switch (s.Key)
                {
                    case 'o': bezants.Add(new Bezant(p + new Vector2(0f, 0.25f), world, false)); break;
                    case 'G': Art.Make("scenery/goal_pole", world, p, 5); break;
                    case 's': kind = "snail_hound"; break;
                    case 'S': kind = "snail_moss"; break;
                    case 'g': kind = "gull"; break;
                    case 'x': kind = "crab"; break;
                    case 'f': kind = "fish"; break;
                    case 'z': kind = "sea_serpent"; break;
                    case 'm': kind = "sea_monk"; break;
                    case 'Q': logPiles.Add(p); logTimers.Add(1.5f); Art.Make("scenery/log_pile", world, p, 3); break;
                    case 'F': flails.Add(new Flail(p, world, flails.Count % 2 == 0, flails.Count * 1.7f)); break;
                    case 'K': boss = new Boss(p, world); break;
                    case 'J': boss = new Whale(p, world); break;
                    case 'A': hasAxe = true; axePos = p; axeView = Art.Make("scenery/bridge_axe", world, p, 5); break;
                    case 'N': hasMonk = true; monkPos = p; monkView = Art.Make("allies/monk", world, p, 9); monkView.flipX = true; break;
                    case 'C':
                        hasCheckpoint = true; checkpointPos = p;
                        checkpointView = Art.Make("scenery/checkpoint", world, p, 4, checkpointClaimed ? 1 : 0);
                        break;
                    case 'L': case 'E': case 'Y': case 'U':
                        var kindL = s.Key == 'E' ? Lift.Kind.BoardUpDown : s.Key == 'Y' ? Lift.Kind.ShipRocking : s.Key == 'U' ? Lift.Kind.ShipSailing : Lift.Kind.BoardSideways;
                        lifts.Add(new Lift(new Vector2(p.x - 0.5f, p.y + 1f), kindL, world, lifts.Count * 1.3f));
                        break;
                    case 'h': kind = "killer_hare"; break;
                    case 'l': kind = "fiery_lion"; break;
                    case 'd': kind = "bayeux_hound"; break;
                    case 'c': kind = "camel_dragon"; break;
                    case 'w': kind = "wyvern"; break;
                    case 'u': kind = "unicorn"; break;
                    case 'O': kind = "owl_face"; break;
                    case 'k': kind = "soup_cat"; break;
                }
                if (kind != null) enemies.Add(new Enemy(kind, p, world));
            }
            var start = hasCheckpoint && checkpointClaimed ? checkpointPos : level.Start;
            player = new Knight(Knights.All[knightIndex], start, world);
            if (carry)
            {
                if (carryMounted && L.Underwater) Toast("Bayard waiteth on ye shore. Swimme on!");
                player.ApplyCarry(carryHearts, carryCrossbow, carryMounted && !L.Underwater);   // no horse beneath the waves
                carry = false;
            }
            camX = ClampCam(start.x + 4f);
            PlaceCamera();
        }

        float ClampCam(float x) { return Mathf.Clamp(x, HalfWidth, Mathf.Max(HalfWidth, level.Width - HalfWidth)); }

        void PlaceCamera()
        {
            cam.transform.position = new Vector3(camX, ViewHalfHeight, -10f);
            UpdateLayers();
        }

        // ------------------------------------------------------------------ loop
        void Update()
        {
            TapestryInput.Poll();
            if (TapestryInput.MutePressed) Audio.ToggleMute();
            float dt = Mathf.Min(Time.deltaTime, 1f / 30f);
            stateTime += dt;
            switch (state)
            {
                case State.Title: TickTitle(); break;
                case State.Playing: TickPlay(dt); break;
                case State.Paused:
                    if (TapestryInput.TapPosition.HasValue)
                    {
                        if (PauseButtonRect(0).Contains(TapestryInput.TapPosition.Value)) { SetState(State.Playing); break; }
                        if (PauseButtonRect(1).Contains(TapestryInput.TapPosition.Value)) { levelIndex = 0; checkpointClaimed = false; LoadLevel(); SetState(State.Title); break; }
                        if (PauseButtonRect(2).Contains(TapestryInput.TapPosition.Value)) { Audio.ToggleMute(); break; }
                    }
                    if (TapestryInput.StartPressed || TapestryInput.JumpPressed) SetState(State.Playing);
                    else if (TapestryInput.DownPressed) { levelIndex = 0; checkpointClaimed = false; LoadLevel(); SetState(State.Title); }
                    break;
                case State.Dying:
                    player.Tick(dt, level);
                    if (stateTime > 2.6f)
                    {
                        lives--;
                        if (lives > 0) { LoadLevel(); SetState(State.Playing); }
                        else SetState(State.GameOver);
                    }
                    break;
                case State.Won:
                    TickWin(dt);
                    player.Tick(dt, level);
                    TickEntities(dt);
                    if (winPhase == 2 && winTime > 1.2f && (TapestryInput.JumpPressed || TapestryInput.StartPressed || TapestryInput.TapPosition.HasValue))
                    {
                        if (levelIndex + 1 < Levels.All.Length) NextLevel();
                        else { levelIndex = 0; checkpointClaimed = false; LoadLevel(); SetState(State.Title); }
                    }
                    break;
                case State.Entering:   // walked into the crypt door: fade, then on to the next part
                    if (stateTime > 1.1f) NextLevel();
                    break;
                case State.GameOver:
                    if (stateTime > 1.5f && (TapestryInput.JumpPressed || TapestryInput.StartPressed || TapestryInput.TapPosition.HasValue))
                    { lives = 3; bezantCount = 0; score = 0; levelIndex = 0; checkpointClaimed = false; LoadLevel(); SetState(State.Title); }
                    break;
            }
        }

        string LevelMusic { get { return bossMusic ? "boss" : L.Music; } }
        bool relicMusic;

        void NextLevel()
        {
            carry = true; carryHearts = player.Hearts; carryCrossbow = player.HasCrossbow; carryMounted = player.Mounted;
            levelIndex++;
            checkpointClaimed = false;
            LoadLevel();
            SetState(State.Playing);
        }

        void SetState(State s)
        {
            var was = state;
            if (s == State.Title) titleStep = 0;
            state = s; stateTime = 0f;
            switch (s)
            {
                case State.Title: Audio.Music("title"); break;
                case State.Playing:
                    if (was == State.Paused) Audio.ResumeMusic();
                    else { relicMusic = false; Audio.Music(LevelMusic); Toast(L.Title + "  ·  " + L.Name); }
                    break;
                case State.Paused: Audio.PauseMusic(); Audio.Play("pause"); break;
                case State.Dying: Audio.Music("death", false); break;
                case State.Won: Audio.Play("pole"); Audio.Music("victory", false); break;
                case State.Entering: Audio.Play("pole", 0.7f); break;
                case State.GameOver: Audio.Music("game_over", false); break;
            }
        }

        /// <summary>The holy relique has its own tune while it lasts.</summary>
        /// <summary>Mario-fashion chains: 100, 200, 400, 800 for the first four beasts in a row; the 5th and every one after is a free lyfe.</summary>
        void ChainKill(int n, Vector2 at)
        {
            if (n <= 4) { AddScore(100 << (n - 1), at); return; }
            lives++;
            popups.Add(new Popup { Pos = at, Text = "1 LYFE!" });
            Audio.Play("oneup");
        }

        /// <summary>Gusts: calm for a while, then the wind blows (alternately from ahead and behind) for a couple of seconds.
        /// Leaping fish: every so often a fish bursts from the sea near the knight.</summary>
        void TickWindAndSea(float dt)
        {
            if (L.Wind && !player.Dead)
            {
                windT -= dt;
                if (windT <= 0f)
                {
                    if (wind == 0f)
                    {
                        wind = Random.value < 0.6f ? -2.4f : 2.0f;   // more often against you
                        windT = 2.2f;
                        Audio.Play("fireball", 0.5f);
                        if (!windToldOf) { windToldOf = true; Toast("Ye wynde bloweth! Brace thyself."); }
                    }
                    else { wind = 0f; windT = 3.5f + Random.value * 2f; }
                }
                if (wind != 0f) player.Body.Nudge(level, wind * dt);
            }
            if (L.LeapingFish && !player.Dead)
            {
                leapT -= dt;
                if (leapT <= 0f)
                {
                    leapT = 1.4f + Random.value * 1.2f;
                    float x = player.Body.Pos.x + (Random.value * 2f - 1f) * HalfWidth * 0.8f;
                    float d = Random.value < 0.5f ? -1f : 1f;
                    var e = new Enemy("leaper", new Vector2(x, -1.5f), world);
                    e.Active = true;
                    e.Body.Vel = new Vector2(d * 2.6f, 13.5f);
                    enemies.Add(e);
                    Audio.Play("soup", 0.4f);
                }
            }
        }

        void UpdateRelicMusic()
        {
            if (!player.Blessed) relicCombo = 0;
            if (player.Blessed && !relicMusic) { relicMusic = true; Audio.Music("relic"); }
            else if (!player.Blessed && relicMusic) { relicMusic = false; Audio.Music(LevelMusic); }
        }

        /// <summary>After touching the pole: slide down it, walk into the castle, then show the verse.</summary>
        void TickWin(float dt)
        {
            var pb = player.Body;
            winTime += dt;
            float ground = level.GroundAt(pb.Pos.x);
            if (winPhase == 0)
            {
                pb.Vel = Vector2.zero;
                pb.Pos.y = Mathf.Max(ground, pb.Pos.y - 7f * dt);
                if (pb.Pos.y <= ground && winTime > 0.4f) { winPhase = 1; winTime = 0f; pb.Pos.x = level.GoalX + pb.W * 0.5f + 0.1f; }
            }
            else if (winPhase == 1)
            {
                pb.Vel = new Vector2(3.2f, 0f);
                pb.Pos.x += 3.2f * dt;
                pb.Pos.y = level.GroundAt(pb.Pos.x);
                if (pb.Pos.x >= L.CastleX || winTime > 6f) { winPhase = 2; winTime = 0f; pb.Vel = Vector2.zero; player.Hide(); }
            }
        }

        void OnApplicationPause(bool paused) { if (paused && state == State.Playing) SetState(State.Paused); }

        void TickTitle()
        {
            if (titleStep == 1) { TickRealmSelect(); return; }
            int n = Knights.All.Length;
            if (TapestryInput.LeftPressed) { knightIndex = (knightIndex + n - 1) % n; Audio.Play("select"); }
            if (TapestryInput.RightPressed) { knightIndex = (knightIndex + 1) % n; Audio.Play("select"); }
            bool go = stateTime > 0.3f && (TapestryInput.JumpPressed || TapestryInput.StartPressed);
            if (TapestryInput.TapPosition.HasValue)
            {
                for (int i = 0; i < n; i++)
                    if (PortraitRect(i).Contains(TapestryInput.TapPosition.Value))
                    {
                        if (i == knightIndex) go = true;
                        else Audio.Play("select");
                        knightIndex = i;
                    }
            }
            if (go)
            {
                FindRealms();
                if (realmStarts.Count <= 1) { StartAt(0); return; }
                titleStep = 1; realmChoice = realmStarts.Count - 1; stateTime = 0f;   // offer the furthest realme first
                Audio.Play("select");
            }
        }

        void TickRealmSelect()
        {
            int n = realmStarts.Count;
            if (TapestryInput.LeftPressed) { realmChoice = (realmChoice + n - 1) % n; Audio.Play("select"); }
            if (TapestryInput.RightPressed) { realmChoice = (realmChoice + 1) % n; Audio.Play("select"); }
            if (TapestryInput.RunPressed) { titleStep = 0; Audio.Play("select"); return; }   // back to the knights
            bool go = stateTime > 0.25f && (TapestryInput.JumpPressed || TapestryInput.StartPressed);
            if (TapestryInput.TapPosition.HasValue)
                for (int i = 0; i < n; i++)
                    if (RealmRect(i).Contains(TapestryInput.TapPosition.Value))
                    {
                        if (i == realmChoice) go = true; else Audio.Play("select");
                        realmChoice = i;
                    }
            if (go) { titleStep = 0; StartAt(realmStarts[realmChoice]); }
        }

        Rect RealmRect(int i)
        {
            int n = realmStarts.Count;
            float w = Mathf.Min(Screen.width * 0.9f / n - 10f, U * 0.42f), h = U * 0.16f;
            float total = n * w + (n - 1) * 10f;
            return new Rect((Screen.width - total) * 0.5f + i * (w + 10f), Screen.height * 0.42f, w, h);
        }

        void TickPlay(float dt)
        {
            if (TapestryInput.StartPressed) { SetState(State.Paused); return; }
            if (!collapsing && !rescuedMonk)
            {
                tymeLeft -= dt / TymeTick;
                if (!hurried && tymeLeft <= 100f)
                {
                    hurried = true;
                    Audio.Play("pause"); Audio.MusicPitch(1.15f);
                    Toast("Make haste! Ye tyme groweth shorte!");
                }
                if (tymeLeft <= 0f && !player.Dead) { tymeLeft = 0f; deathTyme = true; player.Die(); return; }
            }
            if (TapestryInput.NextKnightPressed) SwitchKnight(1);
            if (TapestryInput.PrevKnightPressed) SwitchKnight(-1);

            level.Tick(dt);
            foreach (var l in lifts) l.Tick(dt);
            foreach (var f in flails) f.Tick(dt);
            if (collapsing) TickCollapse(dt);
            player.Tick(dt, level);
            TickWindAndSea(dt);
            TickEntities(dt);
            if (player.Dead) return;
            UpdateRelicMusic();

            var pb = player.Body;
            foreach (var e in enemies)
            {
                if (!e.Active || !e.Alive || e.Carried || !pb.Overlaps(e.Body)) continue;
                // a stomp if we were falling and our feet were in the top half of the beast last frame
                if (player.Blessed)   // the holy relique smites any beast it touches - and the glory doubles each time
                {
                    var at = e.Body.Pos + new Vector2(0f, e.Body.H);
                    e.Flip(false);
                    ChainKill(++relicCombo, at);
                    continue;
                }
                bool fromAbove = !Underwater && pb.Vel.y < 0f && pb.PrevBottom >= e.Body.Pos.y + e.Body.H * (player.Mounted ? 0.35f : 0.5f);   // under water nothing can be stomped
                e.OnKnight(player, fromAbove);
                if (player.Dead) return;
            }
            foreach (var s in enemies)
            {
                if (!s.Alive || !(s.ShellMoving || s.Carried)) continue;
                foreach (var o in enemies)
                    if (o != s && o.Alive && o.Active && !o.Carried && s.Body.Overlaps(o.Body))
                    {
                        var at = o.Body.Pos + new Vector2(0f, o.Body.H);
                        o.Flip(false);
                        ChainKill(++s.ShellCombo, at);     // a sliding shell's glory doubles with every beast in a row
                        if (s.Carried) { s.Flip(); break; }   // a held shell is spent on the first beast it strikes
                    }
            }
            foreach (var f in flails)
                if (f.Hits(pb)) player.Hurt();
            foreach (var lg in logs)
            {
                if (lg.Removed || lg.Knocked || !pb.Overlaps(lg.Body)) continue;
                if (player.Blessed) { lg.Destroy(); AddScore(200, lg.Body.Pos + new Vector2(0f, 1f)); continue; }
                bool onTop = pb.Vel.y < 0f && pb.PrevBottom >= lg.Body.Pos.y + lg.Body.H * 0.5f;
                if (onTop) { player.Bounce(); lg.Knock(); Audio.Play("kick"); AddScore(100, lg.Body.Pos + new Vector2(0f, 1f)); }   // stomped: it tumbles away
                else player.Hurt();
            }
            if (boss != null && boss.Touchable && pb.Overlaps(boss.Body))
            {
                bool onCrown = pb.Vel.y < 0f && pb.PrevBottom >= boss.Body.Pos.y + boss.Body.H * 0.6f;
                boss.OnKnight(player, onCrown);
            }
            foreach (var s in shots)
            {
                if (s.Removed) continue;
                if (!s.Friendly)
                {
                    if (s.Body.Overlaps(pb))
                    {
                        if (s.Kind == "gaze") player.Petrify(1.3f); else player.Hurt();
                        s.Destroy();
                    }
                    continue;
                }
                if (boss != null && boss.Touchable && s.Body.Overlaps(boss.Body)) { boss.Hit(0.34f); s.Destroy(); continue; }
                foreach (var lg in logs)   // a bolt splits a log
                    if (!lg.Removed && s.Body.Overlaps(lg.Body)) { lg.Destroy(); s.Destroy(); AddScore(200, lg.Body.Pos + new Vector2(0f, 1f)); break; }
                if (s.Removed) continue;
                foreach (var e in enemies)   // a crossbowe bolt fells any beast, burning lions too
                    if (e.Active && e.Alive && !e.Carried && s.Body.Overlaps(e.Body)) { e.Flip(); s.Destroy(); break; }
            }
            foreach (var it in items)
                if (!it.Removed && it.Ready && it.Body.Overlaps(pb)) { Collect(it); it.Destroy(); }
            foreach (var b in bezants)
                if (!b.Removed && b.Touches(pb)) { b.Destroy(); AddBezant(); }
            if (bayard != null && !bayard.Removed && !player.Mounted && bayard.CanMount && pb.Overlaps(bayard.Body))
            {
                player.Mount();
                bayard.Destroy();
                bayard = null;
            }

            deathFire = level.Fire(Mathf.FloorToInt(pb.Pos.x), Mathf.FloorToInt(pb.Pos.y + 0.3f));
            if (pb.Pos.y < -0.6f || deathFire || level.Water(Mathf.FloorToInt(pb.Pos.x), Mathf.FloorToInt(pb.Pos.y + 0.3f))) { player.Die(true); return; }

            // strike the axe: the drawbridge falls, and whatever stands on it
            if (hasAxe && !collapsing && pb.Right > axePos.x - 0.4f && pb.Left < axePos.x + 0.4f && pb.Pos.y < axePos.y + 2f)
            {
                collapsing = true; collapseT = collapseDoneT = 0f;
                collapseTiles = level.BridgeTiles();
                player.Freeze();
                if (axeView != null) Destroy(axeView.gameObject);
                Audio.Play("kick");
            }

            // the rescued monk ends a castle
            if (hasMonk && !rescuedMonk && boss != null && !boss.Dead && pb.Pos.x >= monkPos.x - 1.2f)
            {
                if (!toldOfBoss) { toldOfBoss = true; Toast("Brother Odo crieth: first vanquish " + boss.Title + "!"); }
            }
            else if (hasMonk && !rescuedMonk && pb.Pos.x >= monkPos.x - 1.2f)
            {
                rescuedMonk = true;
                if (monkView != null) monkView.sprite = Art.Frames("allies/monk")[1];
                player.Freeze(); player.FaceRight();
                PlayerPrefs.SetInt(PassedKey(L.Title), 1); PlayerPrefs.Save();
                score += 1000;
                TymeBonus();
                winPhase = 2; winTime = 0f;
                SetState(State.Won);
                return;
            }

            if (hasCheckpoint && !checkpointClaimed && pb.Pos.x >= checkpointPos.x - 0.3f)
            {
                checkpointClaimed = true;
                if (checkpointView != null) checkpointView.sprite = Art.Frames("scenery/checkpoint")[1];
                Audio.Play("item_appear");
                Toast("Ye halfway banner is raysed! Here shalt thou ryse agayne.");
            }

            if (L.EndAtDoor && !rescuedMonk && pb.Pos.x >= L.CastleX - 0.3f && pb.Pos.y < 4f)
            {
                rescuedMonk = true;   // (reused as 'level finished')
                player.Freeze(); player.Hide();
                PlayerPrefs.SetInt(PassedKey(L.Title), 1); PlayerPrefs.Save();
                TymeBonus();
                winPhase = 2; winTime = 0f;
                SetState(State.Won);
                return;
            }

            if (L.Entrance && pb.Pos.x >= L.CastleX - 0.2f && pb.Grounded)
            {
                player.Freeze(); player.Hide();
                SetState(State.Entering);
                return;
            }

            if (level.GoalX > 0f && pb.Right >= level.GoalX - 0.1f && !player.Dead)
            {
                player.Freeze();
                player.FaceRight();
                TymeBonus();
                PoleScore(pb.Pos.y - level.GroundAt(level.GoalX), new Vector2(level.GoalX, pb.Pos.y + pb.H));
                pb.Pos.x = level.GoalX - pb.W * 0.5f - 0.05f;
                winPhase = 0; winTime = 0f;
                PlayerPrefs.SetInt(PassedKey(L.Title), 1); PlayerPrefs.Save();   // unlocks it in the level select
                SetState(State.Won);
            }
        }

        void TickEntities(float dt)
        {
            Cleanup();
            float wake = camX + HalfWidth + 2f;
            if (boss != null)
            {
                if (!boss.Active && boss.Body.Pos.x < wake - 2f)
                {
                    boss.Active = true; bossMusic = true; Audio.Music("boss");
                    Toast(boss.Title + " barreth thy way!");
                }
                if (boss.Active) boss.Tick(dt, level, player);
            }
            foreach (var e in enemies)
            {
                if (!e.Active && e.Body.Pos.x < wake) e.Active = true;
                if (e.Active) e.Tick(dt, level, player);
            }
            foreach (var s in shots) s.Tick(dt, level);
            foreach (var b in bezants) b.Tick(dt);
            foreach (var d in debris) d.Tick(dt);
            // log piles roll a log toward the knight every few seconds while he is near
            for (int i = 0; i < logPiles.Count; i++)
            {
                var lp = logPiles[i];
                if (Mathf.Abs(lp.x - player.Body.Pos.x) > HalfWidth + 3f) continue;
                logTimers[i] -= dt;
                if (logTimers[i] <= 0f)
                {
                    logTimers[i] = 3.2f;
                    float d = player.Body.Pos.x < lp.x ? -1f : 1f;
                    logs.Add(new Log(lp + new Vector2(d * 1.2f, 0.05f), d, world));
                    Audio.Play("bump", 0.5f);
                }
            }
            foreach (var lg in logs)
            {
                lg.Tick(dt, level);
                if (!lg.Removed && Mathf.Abs(lg.Body.Pos.x - camX) > HalfWidth + 1f) lg.Destroy();   // rolled off screen: gone, never to rebound
            }
            foreach (var it in items) it.Tick(dt, level);
            if (bayard != null)
            {
                bayard.Tick(dt, level);
                if (bayard.Removed)
                {
                    if (bayard.Lost) { Toast("Alas! Bayard is loste. Seeke him agayne in ye bryckes."); Audio.Play("bayard_lost"); }
                    bayard = null;
                }
            }
            if (toastTime > 0f) toastTime -= dt;
            foreach (var p in popups) p.T += dt;
            popups.RemoveAll(p => p.T > 1.1f);
            Cleanup();
        }

        void Cleanup()
        {
            enemies.RemoveAll(e => e.Removed);
            shots.RemoveAll(s => s.Removed);
            bezants.RemoveAll(b => b.Removed);
            debris.RemoveAll(d => d.Removed);
            logs.RemoveAll(l => l.Removed);
            items.RemoveAll(i => i.Removed);
        }

        void LateUpdate()
        {
            if (player == null || level == null) return;
            if (state == State.Playing || state == State.Won)
            {
                float target = player.Body.Pos.x + (player.FacingRight ? 1.5f : -1.5f);
                camX = Mathf.Lerp(camX, ClampCam(target), 1f - Mathf.Exp(-6f * Mathf.Min(Time.deltaTime, 0.05f)));
            }
            camX = ClampCam(camX);
            PlaceCamera();
        }

        void SwitchKnight(int d)
        {
            int n = Knights.All.Length;
            knightIndex = (knightIndex + d + n) % n;
            player.SetKnight(Knights.All[knightIndex]);
        }

        // ------------------------------------------------------------------ called by actors
        public void KnightHitBlock(int x, int y)
        {
            var r = level.HitFromBelow(x, y);
            switch (r)
            {
                case Level.BumpResult.Broke: Audio.Play("brick"); break;
                case Level.BumpResult.Thud: Audio.Play("bump", 0.8f); break;
                case Level.BumpResult.Bezant: case Level.BumpResult.None: break;
                default: Audio.Play("item_appear"); break;
            }
            var top = new Vector2(x + 0.5f, y + 1f);
            switch (r)
            {
                case Level.BumpResult.Bezant:
                    bezants.Add(new Bezant(top, world, true)); AddBezant(); break;
                case Level.BumpResult.Bayard:
                    if (!player.Mounted && bayard == null) { bayard = new Bayard(top, world); bayard.Hop(); }
                    else { bezants.Add(new Bezant(top, world, true)); AddBezant(); }
                    break;
                case Level.BumpResult.Chalice: items.Add(new Item("chalice", top, world, player.FacingRight)); break;
                case Level.BumpResult.Capon: items.Add(new Item("capon", top, world, player.FacingRight)); break;
                case Level.BumpResult.Crossbow: items.Add(new Item("crossbow", top, world, player.FacingRight)); break;
                case Level.BumpResult.Relic: items.Add(new Item("relic", top, world, player.FacingRight)); break;
                case Level.BumpResult.Broke:
                    score += 50;
                    var c = new Vector2(x + 0.5f, y + 0.3f);
                    debris.Add(new Debris(c, new Vector2(-2.5f, 9f), world));
                    debris.Add(new Debris(c, new Vector2(2.5f, 9f), world));
                    debris.Add(new Debris(c, new Vector2(-1.8f, 6f), world));
                    debris.Add(new Debris(c, new Vector2(1.8f, 6f), world));
                    break;
            }
            if (r != Level.BumpResult.None)
                foreach (var e in enemies)   // beasts standing on a bumped block are knocked flying
                    if (e.Alive && Mathf.Abs(e.Body.Pos.x - top.x) < 0.8f && Mathf.Abs(e.Body.Pos.y - top.y) < 0.2f) e.Flip();
        }

        public void ReleaseBayard(Vector2 pos, bool knightFacingRight, bool thrown)
        {
            if (bayard != null) bayard.Destroy();
            bayard = new Bayard(pos, world);
            if (thrown) bayard.Bolt(!knightFacingRight); else bayard.Calm();
        }

        public void SpawnShot(string kind, Vector2 pos, Vector2 vel) { shots.Add(new Shot(kind, pos, vel, world)); }
        public void SpawnBolt(Vector2 pos, bool right) { shots.Add(new Shot("bolt", pos, new Vector2(right ? 13f : -13f, 0f), world)); }
        public int BoltCount { get { int n = 0; foreach (var s in shots) if (s.Friendly && !s.Removed) n++; return n; } }

        public void Toast(string text) { toast = text; toastTime = 3f; }

        void Collect(Item it)
        {
            score += 1000;
            switch (it.Kind)
            {
                case "chalice": Audio.Play("oneup"); lives++; Toast("A golden chalice! One more lyfe is thyne."); break;
                case "capon": Audio.Play("powerup"); player.Heal(); Toast("A roaste capon! Thy herte is mended."); break;
                case "crossbow": Audio.Play("powerup"); player.GiveCrossbow(); Toast("A crossbowe! Presse RUNNE to loose a bolte."); break;
                case "relic": Audio.Play("powerup"); player.RelicTime = 9f; Toast("A holy relique! No beaste may harme thee for a whyle."); break;
            }
        }
        public void AddScore(int n) { score += n; }

        /// <summary>Every tick of tyme left over at the goal is worth 50 glorie.</summary>
        /// <summary>Roman numerals, as a scribe of 1066 would have written them (nulla for nought).</summary>
        public static string Roman(int n)
        {
            if (n <= 0) return "N";
            int[] v = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            string[] r = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < v.Length; i++)
                while (n >= v[i]) { sb.Append(r[i]); n -= v[i]; }
            return sb.ToString();
        }

        void TymeBonus()
        {
            tymeBonus = Mathf.CeilToInt(Mathf.Max(0f, tymeLeft)) * 50;
            score += tymeBonus;
            Audio.MusicPitch(1f);
        }

        public void BossDefeated()
        {
            bossMusic = false;
            Audio.Music(L.Music);
            Toast("Huzzah! " + (boss != null ? boss.Title : "Ye beaste") + " is slayne!");
        }

        void TickCollapse(float dt)
        {
            collapseT += dt;
            while (collapseT > 0.07f && collapseTiles.Count > 0)
            {
                collapseT -= 0.07f;
                var t = collapseTiles[0]; collapseTiles.RemoveAt(0);
                level.RemoveTile(t.x, t.y);
                if (collapseTiles.Count % 3 == 0) Audio.Play("brick", 0.6f);
            }
            if (collapseTiles.Count == 0)
            {
                collapseDoneT += dt;
                if (collapseDoneT > 1.2f) { collapsing = false; hasAxe = false; player.Unfreeze(); }
            }
        }

        /// <summary>Move a body standing on a lift along with it (call before the body steps).</summary>
        public void CarryOnLift(Body b)
        {
            if (riding != null) b.Pos += riding.Delta;
        }

        /// <summary>Land a falling body on any lift top it crosses (call after the body steps).</summary>
        public void LandOnLifts(Body b)
        {
            riding = null;
            if (b.Vel.y > 0f) return;
            foreach (var l in lifts)
            {
                if (b.Right <= l.X0 + 0.05f || b.Left >= l.X1 - 0.05f) continue;
                if (b.PrevBottom >= l.Top - 0.25f && b.Pos.y <= l.Top + 0.02f)
                {
                    b.Pos.y = l.Top; b.Vel.y = 0f; b.Grounded = true; riding = l;
                    return;
                }
            }
        }
        public void AddScore(int n, Vector2 at) { score += n; popups.Add(new Popup { Pos = at, Text = n.ToString() }); }

        /// <summary>The higher on the pole ye grasp it, the more glorie (Mario-fashion). The very top gives a lyfe.</summary>
        void PoleScore(float height, Vector2 at)
        {
            if (height >= 7f)
            {
                lives++; score += 5000;
                popups.Add(new Popup { Pos = at, Text = "1 LYFE!" });
                Audio.Play("oneup");
                Toast("Ye toppe of ye pole! A lyfe is thyne.");
                return;
            }
            int pts = height >= 5.5f ? 5000 : height >= 4f ? 2000 : height >= 2.5f ? 800 : height >= 1.2f ? 400 : height >= 0.4f ? 200 : 100;
            AddScore(pts, at);
        }
        public void PlayerDied(bool fell) { deathFell = fell; SetState(State.Dying); }

        void AddBezant()
        {
            Audio.Play("bezant", 0.7f);
            bezantCount++; score += 10;
            if (bezantCount >= 100) { bezantCount -= 100; lives++; }
        }

        // ------------------------------------------------------------------ GUI
        GUIStyle big, small, center, popupStyle;

        /// <summary>GUI unit: screen height, or less when the window is narrower than 16:9.</summary>
        static float U { get { return Mathf.Min(Screen.height, Screen.width * 9f / 16f); } }

        void Styles()
        {
            if (big != null && big.fontSize == Mathf.RoundToInt(U / 16f)) return;
            big = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(U / 16f), fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            big.normal.textColor = Thread;
            small = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(U / 30f) };
            small.normal.textColor = Thread;
            center = new GUIStyle(small) { alignment = TextAnchor.MiddleCenter, wordWrap = true };
            popupStyle = new GUIStyle(small) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            popupStyle.normal.textColor = new Color(0.66f, 0.26f, 0.18f);
        }

        void DrawFrame(string key, int frame, Rect r)
        {
            var tex = Art.Texture(key);
            if (tex == null) return;
            int n = Mathf.Max(1, Art.Sheet(key).Frames);
            GUI.DrawTextureWithTexCoords(r, tex, new Rect(frame / (float)n, 0f, 1f / n, 1f));
        }

        Rect PortraitRect(int i)
        {
            // four portraits + three gaps must fit in 92% of the width, and in a third of the height
            float w = Mathf.Min(Screen.height * 0.32f * 64f / 96f, Screen.width * 0.92f / (Knights.All.Length + (Knights.All.Length - 1) * 0.35f));
            float h = w * 96f / 64f, gap = w * 0.35f;
            float total = Knights.All.Length * w + (Knights.All.Length - 1) * gap;
            return new Rect((Screen.width - total) * 0.5f + i * (w + gap), Screen.height * 0.36f, w, h);
        }

        void Panel(Rect r) { var c = GUI.color; GUI.color = new Color(Linen.r, Linen.g, Linen.b, 0.88f); GUI.DrawTexture(r, Texture2D.whiteTexture); GUI.color = c; }

        void OnGUI()
        {
            Styles();
            float W = Screen.width, H = Screen.height;
            switch (state)
            {
                case State.Title:
                    Panel(new Rect(0, H * 0.12f, W, H * 0.88f));
                    if (titleStep == 1) { DrawRealmSelect(W, H); break; }
                    float y = H * 0.14f;
                    var t = Art.Texture("scenery/title");
                    if (t != null) { float tw = Mathf.Min(W * 0.9f, U * 1.3f), th = tw * t.height / t.width; GUI.DrawTexture(new Rect((W - tw) * 0.5f, y, tw, th), t); y += th; }
                    GUI.Label(new Rect(0, y, W, U * 0.07f), Levels.All[0].Title + " · " + Levels.All[0].Name, center);
                    for (int i = 0; i < Knights.All.Length; i++)
                    {
                        var r = PortraitRect(i);
                        if (i == knightIndex) { var c = GUI.color; GUI.color = new Color(0.66f, 0.26f, 0.18f, 0.35f); GUI.DrawTexture(new Rect(r.x - 6, r.y - 6, r.width + 12, r.height + 12), Texture2D.whiteTexture); GUI.color = c; }
                        else GUI.color = new Color(1f, 1f, 1f, 0.55f);
                        DrawFrame(Knights.All[i].Sheet, i == knightIndex ? (int)(Time.time * 3f) % 3 : 0, r);
                        GUI.color = Color.white;
                    }
                    var def = Knights.All[knightIndex];
                    y = PortraitRect(0).yMax + U * 0.03f;
                    GUI.Label(new Rect(0, y, W, U * 0.08f), def.Title, big); y += U * 0.09f;
                    GUI.Label(new Rect(W * 0.05f, y, W * 0.9f, U * 0.07f), def.Trait, center); y += U * 0.07f;
                    GUI.Label(new Rect(W * 0.05f, y, W * 0.9f, U * 0.1f),
                        TapestryInput.ShowTouchControls ? "Touche a knyghte to chuse, and touche agayne to ryde forth" : "LEFTE or RYGHTE to chuse thy knyghte  -  JUMPE or Entre to ryde forth", center);
                    break;

                case State.Playing:
                case State.Paused:
                case State.Dying:
                case State.Won:
                case State.GameOver:
                case State.Entering:
                    Hud(W, H);
                    if (state == State.Entering)
                    {
                        var fc = GUI.color; GUI.color = new Color(0.12f, 0.09f, 0.07f, Mathf.Clamp01(stateTime / 0.9f));
                        GUI.DrawTexture(new Rect(0, 0, W, H), Texture2D.whiteTexture); GUI.color = fc;
                    }
                    if (state == State.Paused)
                    {
                        Panel(new Rect(W * 0.2f, H * 0.26f, W * 0.6f, H * 0.5f));
                        GUI.Label(new Rect(0, H * 0.28f, W, H * 0.1f), "THOU ART AT REST", big);
                        if (TapestryInput.ShowTouchControls)
                        {
                            string[] labels = { "Continewe", "Returne to ye beginnynge", Audio.Muted ? "Unsilence ye musick" : "Silence ye musick" };
                            for (int i = 0; i < 3; i++)
                            {
                                var r = PauseButtonRect(i);
                                var bc = GUI.color; GUI.color = new Color(0.66f, 0.26f, 0.18f, 0.3f); GUI.DrawTexture(r, Texture2D.whiteTexture); GUI.color = bc;
                                GUI.Label(r, labels[i], center);
                            }
                        }
                        else GUI.Label(new Rect(0, H * 0.42f, W, H * 0.2f), "JUMPE or Starte to continewe\nDOWNE to returne unto ye beginnynge\nM (or Selecte) to silence ye musick", center);
                    }
                    if (state == State.Won && winPhase == 2)
                    {
                        if (tymeBonus > 0) GUI.Label(new Rect(0, H * 0.73f, W, H * 0.06f), "Tyme bonus: " + tymeBonus + " glorie", center);
                        Panel(new Rect(W * 0.15f, H * 0.24f, W * 0.7f, H * 0.57f));
                        GUI.Label(new Rect(0, H * 0.27f, W, H * 0.1f), "HERE YE KNYGHTE IS VICTORIOUS", big);
                        GUI.Label(new Rect(W * 0.15f, H * 0.37f, W * 0.7f, H * 0.17f), hasMonk
                            ? "Grammercy, good knyghte! " + (boss != null ? boss.Title : "Ye beaste") + " is vanquished.\nYet ye Princesse Aelfgyva is held in another castel..." + (levelIndex + 1 < Levels.All.Length ? "\nPresse JUMPE to ryde on." : "\nMore of ye tapestrie is yet to be sewn...")
                            : levelIndex + 1 < Levels.All.Length
                            ? "Huzzah! Ye beastes are put to flyghte.\nYet ye Princesse Aelfgyva lyeth in another castel...\nPresse JUMPE to ryde on."
                            : "Huzzah! Thou hast passed every realme yet stitched.\nMore of ye tapestrie is yet to be sewn...", center);
                        if (hasMonk) DrawFrame("allies/monk", 1, new Rect(W * 0.5f - H * 0.05f, H * 0.56f, H * 0.1f, H * 0.17f)); else
                        DrawFrame("princess/aelfgyva", 1, new Rect(W * 0.5f - H * 0.05f, H * 0.54f, H * 0.1f, H * 0.19f));
                    }
                    if (state == State.Dying)
                    {
                        Panel(new Rect(W * 0.25f, H * 0.32f, W * 0.5f, H * 0.22f));
                        GUI.Label(new Rect(0, H * 0.34f, W, H * 0.1f), deathTyme ? "YE TYME IS UP!" : deathFire ? "ALAS, HE IS BURNT!" : deathFell ? "ALAS, HE IS FALLEN!" : "ALAS, HE IS SLAYNE!", big);
                        GUI.Label(new Rect(0, H * 0.44f, W, H * 0.08f), (deathTyme ? "Ye knyghte tarried too long. " : deathFire ? "Ye knyghte hath fallen into ye fyre. " : deathFell ? "Ye knyghte hath fallen into ye pitte. " : "Ye knyghte is struck downe. ") + (lives > 1 ? (lives - 1) + " lyves yet remayne." : "No lyves remayne..."), center);
                    }
                    if (state == State.GameOver)
                    {
                        Panel(new Rect(W * 0.25f, H * 0.3f, W * 0.5f, H * 0.3f));
                        GUI.Label(new Rect(0, H * 0.34f, W, H * 0.12f), "HERE ENDETH YE TALE", big);
                        GUI.Label(new Rect(0, H * 0.46f, W, H * 0.1f), "Ye beastes have wonne this daye. Presse JUMPE.", center);
                    }
                    if (state == State.Playing && TapestryInput.ShowTouchControls) TouchButtons();
                    break;
            }
        }

        /// <summary>A long strip of tapestry: the embroidered part is the knight's health, stitched in segments.</summary>
        void HealthBar(Rect r)
        {
            var empty = Art.Texture("scenery/health_empty");
            var full = Art.Texture("scenery/health_full");
            if (empty == null || full == null) return;
            int max = Mathf.Max(1, player.Def.Hearts);
            float frac = Mathf.Clamp01(player.Hearts / (float)max);
            GUI.DrawTexture(r, empty);
            // the fringe takes the first and last 8/240 of the strip; fill between them
            const float edge = 8f / 240f;
            float k = edge + (1f - 2f * edge) * frac;
            if (frac > 0f) GUI.DrawTextureWithTexCoords(new Rect(r.x, r.y, r.width * k, r.height), full, new Rect(0f, 0f, k, 1f));
            var c = GUI.color; GUI.color = Thread;
            for (int i = 1; i < max; i++)
            {
                float x = r.x + r.width * (edge + (1f - 2f * edge) * i / max);
                GUI.DrawTexture(new Rect(x - 1f, r.y + r.height * 0.12f, 2f, r.height * 0.76f), Texture2D.whiteTexture);
            }
            GUI.color = c;
        }

        Rect PauseButtonRect(int i)
        {
            float w = Screen.width * 0.44f, h = Screen.height * 0.085f;
            return new Rect((Screen.width - w) * 0.5f, Screen.height * 0.4f + i * h * 1.2f, w, h);
        }

        void Gusts()
        {
            if (wind == 0f || state != State.Playing) return;
            var tex = Art.Texture("scenery/gust");
            if (tex == null) return;
            float w = U * 0.4f, h = w * tex.height / tex.width;
            for (int i = 0; i < 4; i++)
            {
                float speed = Screen.width * 0.9f;
                float x = Mathf.Repeat(Time.time * speed * Mathf.Sign(wind) + i * Screen.width * 0.3f, Screen.width + w) - w;
                float y = Screen.height * (0.28f + 0.14f * i);
                var r = wind < 0f ? new Rect(x + w, y, -w, h) : new Rect(x, y, w, h);   // mirrored when blowing from the right
                var c = GUI.color; GUI.color = new Color(1f, 1f, 1f, 0.8f); GUI.DrawTexture(r, tex); GUI.color = c;
            }
        }

        void Popups()
        {
            foreach (var p in popups)
            {
                var sp = cam.WorldToScreenPoint(new Vector3(p.Pos.x, p.Pos.y + p.T * 1.4f, 0f));
                var c = GUI.color; GUI.color = new Color(1f, 1f, 1f, Mathf.Clamp01(1.6f - p.T * 1.4f));
                GUI.Label(new Rect(sp.x - U * 0.15f, Screen.height - sp.y - U * 0.03f, U * 0.3f, U * 0.06f), p.Text, popupStyle);
                GUI.color = c;
            }
        }

        void DrawRealmSelect(float W, float H)
        {
            var t = Art.Texture("scenery/title");
            float y = H * 0.14f;
            if (t != null) { float tw = Mathf.Min(W * 0.9f, U * 1.3f), th = tw * t.height / t.width; GUI.DrawTexture(new Rect((W - tw) * 0.5f, y, tw, th), t); }
            GUI.Label(new Rect(0, H * 0.3f, W, U * 0.08f), "Chuse thy realme", big);
            for (int i = 0; i < realmStarts.Count; i++)
            {
                var r = RealmRect(i);
                var def = Levels.All[realmStarts[i]];
                var c = GUI.color;
                GUI.color = i == realmChoice ? new Color(0.66f, 0.26f, 0.18f, 0.4f) : new Color(0.2f, 0.145f, 0.114f, 0.12f);
                GUI.DrawTexture(r, Texture2D.whiteTexture);
                GUI.color = c;
                GUI.Label(new Rect(r.x, r.y + r.height * 0.08f, r.width, r.height * 0.45f), def.Title, big);
                GUI.Label(new Rect(r.x, r.y + r.height * 0.55f, r.width, r.height * 0.4f), def.Name, center);
            }
            GUI.Label(new Rect(0, H * 0.62f, W, U * 0.08f), Knights.All[knightIndex].Title + " shall ryde", center);
            GUI.Label(new Rect(W * 0.05f, H * 0.78f, W * 0.9f, U * 0.1f),
                TapestryInput.ShowTouchControls ? "Touche a realme, and touche agayne to ryde" : "LEFTE or RYGHTE to chuse  -  JUMPE to ryde  -  RUNNE to chuse another knyghte", center);
        }

        void Hud(float W, float H)
        {
            Gusts();
            Popups();
            if (boss != null && boss.Active && !boss.Dead)
            {
                var r = new Rect(W * 0.5f - U * 0.3f, H * 0.2f, U * 0.6f, U * 0.05f);
                GUI.Label(new Rect(r.x, r.y - U * 0.055f, r.width, U * 0.05f), boss.Title, center);
                var e = Art.Texture("scenery/health_empty"); var f = Art.Texture("scenery/health_full");
                if (e != null && f != null)
                {
                    GUI.DrawTexture(r, e);
                    float k = Mathf.Clamp01(boss.Health / boss.MaxHealth);
                    const float edge = 8f / 240f;
                    float u = edge + (1f - 2f * edge) * k;
                    if (k > 0f) GUI.DrawTextureWithTexCoords(new Rect(r.x, r.y, r.width * u, r.height), f, new Rect(0f, 0f, u, 1f));
                }
            }
            float y = H * 0.125f, s = U * 0.05f;
            var safe = TapestryInput.Safe;   // keep the HUD clear of camera notches and rounded corners
            GUI.BeginGroup(new Rect(safe.x, 0, safe.width, H));
            W = safe.width;
            GUI.Label(new Rect(W * 0.02f, y, W * 0.3f, s * 1.2f), player.Def.Title + (player.Mounted ? " upon Bayard" : ""), small);
            HealthBar(new Rect(W * 0.02f, y + s * 1.25f, s * 6f, s * 0.75f));
            DrawFrame("scenery/bezant", 0, new Rect(W * 0.34f, y + s * 0.1f, s, s));
            GUI.Label(new Rect(W * 0.34f + s * 1.2f, y, W * 0.12f, s * 1.2f), "× " + bezantCount, small);
            GUI.Label(new Rect(W * 0.47f, y, W * 0.15f, s * 1.2f), "Lyves × " + lives, small);
            GUI.Label(new Rect(W * 0.62f, y, W * 0.2f, s * 1.2f), "Glorie " + score.ToString("000000"), small);
            var tc = small.normal.textColor;
            if (hurried) small.normal.textColor = new Color(0.66f, 0.26f, 0.18f);   // red when tyme is short
            GUI.Label(new Rect(W * 0.80f, y, W * 0.2f, s * 1.2f), "Tyme " + Roman(Mathf.CeilToInt(Mathf.Max(0f, tymeLeft))), small);
            small.normal.textColor = tc;
            float ix = W * 0.02f + s * 6.4f;
            if (player.HasCrossbow) { DrawFrame("items/crossbow", 0, new Rect(ix, y + s * 1.15f, s * 1.3f, s)); ix += s * 1.5f; }
            if (player.Blessed) DrawFrame("items/relic", 1, new Rect(ix, y + s * 1.1f, s * 1.1f, s * 1.1f));
            if (toast != null && toastTime > 0f)
            {
                var c = GUI.color; GUI.color = new Color(1f, 1f, 1f, Mathf.Clamp01(toastTime));
                Panel(new Rect(W * 0.2f, y + s * 2.6f, W * 0.6f, s * 1.4f));
                GUI.Label(new Rect(W * 0.2f, y + s * 2.6f, W * 0.6f, s * 1.4f), toast, center);
                GUI.color = c;
            }
            GUI.EndGroup();
        }

        void TouchButtons()
        {
            // the fixed joystick: base, then the knob where the thumb has pushed it
            DrawDisc("scenery/ui_joy_base", TapestryInput.JoyCentre, TapestryInput.JoyRadius, 82f / 90f, 1f);
            DrawDisc("scenery/ui_joy_knob", TapestryInput.JoyCentre + TapestryInput.JoyKnob, TapestryInput.JoyRadius * 0.42f, 36f / 40f, 1f);
            // JUMPE shows the knight being played; RUNNE/SHOOTE shows the crossbowe
            DrawDisc("scenery/ui_jump_" + player.Def.Key, TapestryInput.JumpCentre, TapestryInput.JumpRadius, 64f / 70f, TapestryInput.TouchJump ? 0.7f : 1f);
            DrawDisc("scenery/ui_run", TapestryInput.RunCentre, TapestryInput.RunRadius, 50f / 56f, TapestryInput.TouchRun ? 0.7f : 1f);
            DrawDisc("scenery/ui_pause", TapestryInput.PauseCentre, TapestryInput.PauseRadius, 23f / 26f, 1f);
        }

        /// <summary>Draw a round control: the disc fills discFrac of its square texture; brightness < 1 shows it pressed.</summary>
        void DrawDisc(string key, Vector2 centre, float radius, float discFrac, float brightness)
        {
            var tex = Art.Texture(key);
            if (tex == null) return;
            float size = 2f * radius / discFrac;
            var c = GUI.color;
            GUI.color = new Color(brightness, brightness, brightness, 0.95f);
            GUI.DrawTexture(new Rect(centre.x - size * 0.5f, centre.y - size * 0.5f, size, size), tex);
            GUI.color = c;
        }
    }

    /// <summary>Starts the game in whatever scene is open, so pressing Play always works.</summary>
    public static class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            if (Object.FindAnyObjectByType<GameManager>() == null)
                new GameObject("Tapestry Game").AddComponent<GameManager>();
        }
    }
}
