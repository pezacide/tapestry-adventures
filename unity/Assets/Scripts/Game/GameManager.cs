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
        string toast; float toastTime;
        readonly List<Layer> layers = new List<Layer>();

        int knightIndex, bezantCount, lives = 3, score;
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
            SetupBackdrop();
            LoadLevel();
            state = State.Title;
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
            cam.orthographic = true;
            cam.orthographicSize = ViewHalfHeight;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Linen;
            cam.transform.position = new Vector3(0f, ViewHalfHeight, -10f);
        }

        float HalfWidth { get { return ViewHalfHeight * cam.aspect; } }

        void SetupBackdrop()
        {
            backdrop = new GameObject("Backdrop").transform;
            AddLayer("backgrounds/linen", 0f, 0f, -100, 3);
            AddLayer("backgrounds/far", 0.25f, 0f, -90, 3);
            AddLayer("backgrounds/near", 0.5f, -0.6f, -80, 3);
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
            enemies.Clear(); shots.Clear(); bezants.Clear(); debris.Clear(); items.Clear(); bayard = null; winPhase = 0; toast = null;
            world = new GameObject("World").transform;
            level = new Level(Level1.Map, world);

            foreach (var d in Level1.Props)
                Art.Make(d.Key, world, new Vector2(d.X, level.GroundAt(d.X)), -20);

            foreach (var s in level.Spawns)
            {
                Vector2 p = s.Value;
                string kind = null;
                switch (s.Key)
                {
                    case 'o': bezants.Add(new Bezant(p + new Vector2(0f, 0.25f), world, false)); break;
                    case 'G': Art.Make("scenery/goal_pole", world, p, 5); break;
                    case 's': kind = "snail_hound"; break;
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
            float dt = Mathf.Min(Time.deltaTime, 1f / 30f);
            stateTime += dt;
            switch (state)
            {
                case State.Title: TickTitle(); break;
                case State.Playing: TickPlay(dt); break;
                case State.Paused:
                    if (TapestryInput.StartPressed || TapestryInput.JumpPressed) SetState(State.Playing);
                    else if (TapestryInput.DownPressed) { LoadLevel(); SetState(State.Title); }
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
                    { LoadLevel(); SetState(State.Title); }
                    break;
                case State.GameOver:
                    if (stateTime > 1.5f && (TapestryInput.JumpPressed || TapestryInput.StartPressed || TapestryInput.TapPosition.HasValue))
                    { lives = 3; bezantCount = 0; score = 0; LoadLevel(); SetState(State.Title); }
                    break;
            }
        }

        void SetState(State s) { state = s; stateTime = 0f; }

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
                if (winTime > 3f || pb.Pos.x > level.Width - 3f) { winPhase = 2; winTime = 0f; pb.Vel = Vector2.zero; player.Hide(); }
            }
        }

        void OnApplicationPause(bool paused) { if (paused && state == State.Playing) SetState(State.Paused); }

        void TickTitle()
        {
            int n = Knights.All.Length;
            if (TapestryInput.LeftPressed) knightIndex = (knightIndex + n - 1) % n;
            if (TapestryInput.RightPressed) knightIndex = (knightIndex + 1) % n;
            bool go = stateTime > 0.3f && (TapestryInput.JumpPressed || TapestryInput.StartPressed);
            if (TapestryInput.TapPosition.HasValue)
            {
                for (int i = 0; i < n; i++)
                    if (PortraitRect(i).Contains(TapestryInput.TapPosition.Value))
                    {
                        if (i == knightIndex) go = true;
                        knightIndex = i;
                    }
            }
            if (go)
            {
                player.SetKnight(Knights.All[knightIndex]);
                SetState(State.Playing);
            }
        }

        void TickPlay(float dt)
        {
            if (TapestryInput.StartPressed) { SetState(State.Paused); return; }
            if (TapestryInput.NextKnightPressed) SwitchKnight(1);
            if (TapestryInput.PrevKnightPressed) SwitchKnight(-1);

            level.Tick(dt);
            player.Tick(dt, level);
            TickEntities(dt);
            if (player.Dead) return;

            var pb = player.Body;
            foreach (var e in enemies)
            {
                if (!e.Active || !e.Alive || !pb.Overlaps(e.Body)) continue;
                // a stomp if we were falling and our feet were in the top half of the beast last frame
                if (player.Blessed) { e.Flip(); continue; }   // the holy relique smites any beast it touches
                bool fromAbove = pb.Vel.y < 0f && pb.PrevBottom >= e.Body.Pos.y + e.Body.H * (player.Mounted ? 0.35f : 0.5f);
                e.OnKnight(player, fromAbove);
                if (player.Dead) return;
            }
            foreach (var s in enemies)
            {
                if (!s.Alive || !s.ShellMoving) continue;
                foreach (var o in enemies)
                    if (o != s && o.Alive && o.Active && s.Body.Overlaps(o.Body)) o.Flip();
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
                    if (e.Active && e.Alive && s.Body.Overlaps(e.Body)) { e.Flip(); s.Destroy(); break; }
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
                score += 100 * Mathf.Clamp(Mathf.RoundToInt(pb.Pos.y - 2f), 1, 9) * 5;   // higher grab, more glory
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
                    if (bayard.Lost) Toast("Alas! Bayard is loste. Seeke him agayne in ye bryckes.");
                    bayard = null;
                }
            }
            if (toastTime > 0f) toastTime -= dt;
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
                case "chalice": lives++; Toast("A golden chalice! One more lyfe is thyne."); break;
                case "capon": player.Heal(); Toast("A roaste capon! Thy herte is mended."); break;
                case "crossbow": player.GiveCrossbow(); Toast("A crossbowe! Presse RUNNE to loose a bolte."); break;
                case "relic": player.RelicTime = 9f; Toast("A holy relique! No beaste may harme thee for a whyle."); break;
            }
        }
        public void AddScore(int n) { score += n; }
        public void PlayerDied(bool fell) { deathFell = fell; SetState(State.Dying); }

        void AddBezant()
        {
            bezantCount++; score += 10;
            if (bezantCount >= 100) { bezantCount -= 100; lives++; }
        }

        // ------------------------------------------------------------------ GUI
        GUIStyle big, small, center;

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
                    GUI.Label(new Rect(0, y, W, U * 0.07f), "Ye Firste Realme · " + Level1.Name, center);
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
                        GUI.Label(new Rect(0, H * 0.46f, W, H * 0.16f), "JUMPE or Starte to continewe\nDOWNE to returne unto ye beginnynge", center);
                    }
                    if (state == State.Won && winPhase == 2)
                    {
                        Panel(new Rect(W * 0.15f, H * 0.24f, W * 0.7f, H * 0.5f));
                        GUI.Label(new Rect(0, H * 0.27f, W, H * 0.1f), "HERE YE KNYGHTE IS VICTORIOUS", big);
                        GUI.Label(new Rect(W * 0.2f, H * 0.38f, W * 0.6f, H * 0.12f), "Huzzah! Ye beastes are put to flyghte.\nYet ye Princesse Aelfgyva lyeth in another castel...", center);
                        DrawFrame("princess/aelfgyva", 1, new Rect(W * 0.5f - H * 0.06f, H * 0.5f, H * 0.12f, H * 0.23f));
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

        void Hud(float W, float H)
        {
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
