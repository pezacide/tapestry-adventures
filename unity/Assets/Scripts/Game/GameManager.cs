using System.Collections.Generic;
using UnityEngine;

namespace Tapestry
{
    /// <summary>Runs the whole game: title & knight select, the level, camera, parallax, HUD and touch buttons.
    /// It builds everything from code, so it works in any scene (Bootstrap adds it automatically).</summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager I { get; private set; }

        enum State { Title, Playing, Paused, Dying, Won, GameOver }
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
            if (Application.isMobilePlatform) Application.targetFrameRate = 60;
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
            enemies.Clear(); shots.Clear(); bezants.Clear(); debris.Clear(); items.Clear(); lifts.Clear(); riding = null; popups.Clear(); bayard = null; winPhase = 0; toast = null;
            SetupBackdrop();
            world = new GameObject("World").transform;
            level = new Level(L.Map, world);

            foreach (var d in L.Props)
                Art.Make(d.Key, world, new Vector2(d.X, level.GroundAt(d.X)), -20);
            // the castle at the end, centred on its doorway; drawn behind the knight, who walks in and vanishes
            Art.Make(L.Castle, world, new Vector2(L.CastleX, level.GroundAt(L.CastleX)), -5);

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
                    case 'L': case 'E': lifts.Add(new Lift(new Vector2(p.x - 0.5f, p.y + 1f), s.Key == 'E', world, lifts.Count * 1.3f)); break;
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
            player = new Knight(Knights.All[knightIndex], level.Start, world);
            camX = ClampCam(level.Start.x + 4f);
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
                    if (TapestryInput.StartPressed || TapestryInput.JumpPressed) SetState(State.Playing);
                    else if (TapestryInput.DownPressed) { levelIndex = 0; LoadLevel(); SetState(State.Title); }
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
                        if (levelIndex + 1 < Levels.All.Length) { levelIndex++; LoadLevel(); SetState(State.Playing); }
                        else { levelIndex = 0; LoadLevel(); SetState(State.Title); }
                    }
                    break;
                case State.GameOver:
                    if (stateTime > 1.5f && (TapestryInput.JumpPressed || TapestryInput.StartPressed || TapestryInput.TapPosition.HasValue))
                    { lives = 3; bezantCount = 0; score = 0; levelIndex = 0; LoadLevel(); SetState(State.Title); }
                    break;
            }
        }

        string LevelMusic { get { return L.Music; } }
        bool relicMusic;

        void SetState(State s)
        {
            var was = state;
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
                case State.GameOver: Audio.Music("game_over", false); break;
            }
        }

        /// <summary>The holy relique has its own tune while it lasts.</summary>
        void UpdateRelicMusic()
        {
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
                if (levelIndex != 0) { levelIndex = 0; LoadLevel(); }
                player.SetKnight(Knights.All[knightIndex]);
                Audio.Play("confirm");
                SetState(State.Playing);
            }
        }

        void TickPlay(float dt)
        {
            if (TapestryInput.StartPressed) { SetState(State.Paused); return; }
            if (TapestryInput.NextKnightPressed) SwitchKnight(1);
            if (TapestryInput.PrevKnightPressed) SwitchKnight(-1);

            level.Tick(dt);
            foreach (var l in lifts) l.Tick(dt);
            player.Tick(dt, level);
            TickEntities(dt);
            if (player.Dead) return;
            UpdateRelicMusic();

            var pb = player.Body;
            foreach (var e in enemies)
            {
                if (!e.Active || !e.Alive || e.Carried || !pb.Overlaps(e.Body)) continue;
                // a stomp if we were falling and our feet were in the top half of the beast last frame
                if (player.Blessed) { e.Flip(); continue; }   // the holy relique smites any beast it touches
                bool fromAbove = pb.Vel.y < 0f && pb.PrevBottom >= e.Body.Pos.y + e.Body.H * (player.Mounted ? 0.35f : 0.5f);
                e.OnKnight(player, fromAbove);
                if (player.Dead) return;
            }
            foreach (var s in enemies)
            {
                if (!s.Alive || !(s.ShellMoving || s.Carried)) continue;
                foreach (var o in enemies)
                    if (o != s && o.Alive && o.Active && !o.Carried && s.Body.Overlaps(o.Body))
                    {
                        o.Flip();
                        if (s.Carried) { s.Flip(); break; }   // a held shell is spent on the first beast it strikes
                    }
            }
            foreach (var s in shots)
            {
                if (s.Removed) continue;
                if (!s.Friendly)
                {
                    if (s.Body.Overlaps(pb)) { player.Hurt(); s.Destroy(); }
                    continue;
                }
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

            if (pb.Pos.y < -0.6f || level.Water(Mathf.FloorToInt(pb.Pos.x), Mathf.FloorToInt(pb.Pos.y + 0.3f))) player.Die(true);

            if (level.GoalX > 0f && pb.Right >= level.GoalX - 0.1f && !player.Dead)
            {
                player.Freeze();
                player.FaceRight();
                PoleScore(pb.Pos.y - level.GroundAt(level.GoalX), new Vector2(level.GoalX, pb.Pos.y + pb.H));
                pb.Pos.x = level.GoalX - pb.W * 0.5f - 0.05f;
                winPhase = 0; winTime = 0f;
                SetState(State.Won);
            }
        }

        void TickEntities(float dt)
        {
            Cleanup();
            float wake = camX + HalfWidth + 2f;
            foreach (var e in enemies)
            {
                if (!e.Active && e.Body.Pos.x < wake) e.Active = true;
                if (e.Active) e.Tick(dt, level, player);
            }
            foreach (var s in shots) s.Tick(dt, level);
            foreach (var b in bezants) b.Tick(dt);
            foreach (var d in debris) d.Tick(dt);
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
                    Hud(W, H);
                    if (state == State.Paused)
                    {
                        Panel(new Rect(W * 0.25f, H * 0.3f, W * 0.5f, H * 0.36f));
                        GUI.Label(new Rect(0, H * 0.33f, W, H * 0.12f), "THOU ART AT REST", big);
                        GUI.Label(new Rect(0, H * 0.46f, W, H * 0.16f), "JUMPE or Starte to continewe\nDOWNE to returne unto ye beginnynge\nM (or Selecte) to silence ye musick", center);
                    }
                    if (state == State.Won && winPhase == 2)
                    {
                        Panel(new Rect(W * 0.15f, H * 0.24f, W * 0.7f, H * 0.5f));
                        GUI.Label(new Rect(0, H * 0.27f, W, H * 0.1f), "HERE YE KNYGHTE IS VICTORIOUS", big);
                        GUI.Label(new Rect(W * 0.15f, H * 0.37f, W * 0.7f, H * 0.17f), levelIndex + 1 < Levels.All.Length
                            ? "Huzzah! Ye beastes are put to flyghte.\nYet ye Princesse Aelfgyva lyeth in another castel...\nPresse JUMPE to ryde on."
                            : "Huzzah! Thou hast passed every realme yet stitched.\nMore of ye tapestrie is yet to be sewn...", center);
                        DrawFrame("princess/aelfgyva", 1, new Rect(W * 0.5f - H * 0.05f, H * 0.54f, H * 0.1f, H * 0.19f));
                    }
                    if (state == State.Dying)
                    {
                        Panel(new Rect(W * 0.25f, H * 0.32f, W * 0.5f, H * 0.22f));
                        GUI.Label(new Rect(0, H * 0.34f, W, H * 0.1f), deathFell ? "ALAS, HE IS FALLEN!" : "ALAS, HE IS SLAYNE!", big);
                        GUI.Label(new Rect(0, H * 0.44f, W, H * 0.08f), (deathFell ? "Ye knyghte hath fallen into ye pitte. " : "Ye knyghte is struck downe. ") + (lives > 1 ? (lives - 1) + " lyves yet remayne." : "No lyves remayne..."), center);
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

        void Hud(float W, float H)
        {
            Popups();
            float y = H * 0.125f, s = U * 0.05f;
            GUI.Label(new Rect(W * 0.02f, y, W * 0.3f, s * 1.2f), player.Def.Title + (player.Mounted ? " upon Bayard" : ""), small);
            for (int i = 0; i < player.Hearts; i++)
                DrawFrame("scenery/roundel_block", 0, new Rect(W * 0.02f + i * s * 1.1f, y + s * 1.2f, s, s));
            DrawFrame("scenery/bezant", 0, new Rect(W * 0.40f, y + s * 0.1f, s, s));
            GUI.Label(new Rect(W * 0.40f + s * 1.2f, y, W * 0.15f, s * 1.2f), "× " + bezantCount, small);
            GUI.Label(new Rect(W * 0.58f, y, W * 0.2f, s * 1.2f), "Lyves × " + lives, small);
            GUI.Label(new Rect(W * 0.76f, y, W * 0.22f, s * 1.2f), "Glorie " + score.ToString("000000"), small);
            float ix = W * 0.02f + Mathf.Max(player.Hearts, 3) * s * 1.1f + s * 0.4f;
            if (player.HasCrossbow) { DrawFrame("items/crossbow", 0, new Rect(ix, y + s * 1.15f, s * 1.3f, s)); ix += s * 1.5f; }
            if (player.Blessed) DrawFrame("items/relic", 1, new Rect(ix, y + s * 1.1f, s * 1.1f, s * 1.1f));
            if (toast != null && toastTime > 0f)
            {
                var c = GUI.color; GUI.color = new Color(1f, 1f, 1f, Mathf.Clamp01(toastTime));
                Panel(new Rect(W * 0.2f, y + s * 2.6f, W * 0.6f, s * 1.4f));
                GUI.Label(new Rect(W * 0.2f, y + s * 2.6f, W * 0.6f, s * 1.4f), toast, center);
                GUI.color = c;
            }
        }

        void TouchButtons()
        {
            Button(TapestryInput.LeftButton, "<");
            Button(TapestryInput.RightButton, ">");
            Button(TapestryInput.DownButton, "v");
            Button(TapestryInput.RunButton, player.HasCrossbow ? "SHOOTE" : "RUNNE");
            Button(TapestryInput.JumpButton, "JUMPE");
            Button(TapestryInput.PauseButton, "II");
        }

        void Button(Rect frac, string label)
        {
            var r = TapestryInput.ToScreen(frac);
            var c = GUI.color;
            GUI.color = new Color(0.2f, 0.145f, 0.114f, 0.28f);
            GUI.DrawTexture(r, Texture2D.whiteTexture);
            GUI.color = c;
            GUI.Label(r, label, big);
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
