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
        readonly List<Layer> layers = new List<Layer>();

        int knightIndex, bezantCount, lives = 3, score;
        float stateTime, camX;

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
            enemies.Clear(); shots.Clear(); bezants.Clear(); bayard = null;
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
                    case 'G': Art.Make("scenery/gonfanon", world, p, 5); break;
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
                    if (stateTime > 2.2f)
                    {
                        lives--;
                        if (lives > 0) { LoadLevel(); SetState(State.Playing); }
                        else SetState(State.GameOver);
                    }
                    break;
                case State.Won:
                    player.Tick(dt, level);
                    TickEntities(dt);
                    if (stateTime > 1.5f && (TapestryInput.JumpPressed || TapestryInput.StartPressed || TapestryInput.TapPosition.HasValue))
                    { LoadLevel(); SetState(State.Title); }
                    break;
                case State.GameOver:
                    if (stateTime > 1.5f && (TapestryInput.JumpPressed || TapestryInput.StartPressed || TapestryInput.TapPosition.HasValue))
                    { lives = 3; bezantCount = 0; score = 0; LoadLevel(); SetState(State.Title); }
                    break;
            }
        }

        void SetState(State s) { state = s; stateTime = 0f; }

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
                bool fromAbove = pb.Vel.y < 0f && pb.PrevBottom >= e.Body.Top - 0.35f;
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
                if (!s.Removed && s.Body.Overlaps(pb)) { player.Hurt(); s.Destroy(); }
            foreach (var b in bezants)
                if (!b.Removed && b.Touches(pb)) { b.Destroy(); AddBezant(); }
            if (bayard != null && !bayard.Removed && !player.Mounted && bayard.CanMount && pb.Overlaps(bayard.Body))
            {
                player.Mount();
                bayard.Destroy();
                bayard = null;
            }

            if (pb.Pos.y < -1.5f || level.Water(Mathf.FloorToInt(pb.Pos.x), Mathf.FloorToInt(pb.Pos.y + 0.3f))) player.Die();

            if (level.GoalX > 0f && pb.Pos.x >= level.GoalX - 0.2f && !player.Dead)
            {
                player.Freeze();
                score += 1000;
                SetState(State.Won);
            }
        }

        void TickEntities(float dt)
        {
            float wake = camX + HalfWidth + 2f;
            foreach (var e in enemies)
            {
                if (!e.Active && e.Body.Pos.x < wake) e.Active = true;
                if (e.Active) e.Tick(dt, level, player);
            }
            foreach (var s in shots) s.Tick(dt, level);
            foreach (var b in bezants) b.Tick(dt);
            if (bayard != null) { bayard.Tick(dt, level); if (bayard.Removed) bayard = null; }
            enemies.RemoveAll(e => e.Removed);
            shots.RemoveAll(s => s.Removed);
            bezants.RemoveAll(b => b.Removed);
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
            var r = level.HitFromBelow(x, y, player.Def.Breaker);
            var top = new Vector2(x + 0.5f, y + 1f);
            switch (r)
            {
                case Level.BumpResult.Bezant:
                    bezants.Add(new Bezant(top, world, true)); AddBezant(); break;
                case Level.BumpResult.Bayard:
                    if (!player.Mounted && bayard == null) { bayard = new Bayard(top, world); bayard.Hop(); }
                    else { bezants.Add(new Bezant(top, world, true)); AddBezant(); }
                    break;
                case Level.BumpResult.Broke: score += 50; break;
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
        public void AddScore(int n) { score += n; }
        public void PlayerDied() { SetState(State.Dying); }

        void AddBezant()
        {
            bezantCount++; score += 10;
            if (bezantCount >= 100) { bezantCount -= 100; lives++; }
        }

        // ------------------------------------------------------------------ GUI
        GUIStyle big, small, center;

        void Styles()
        {
            if (big != null && big.fontSize == Mathf.RoundToInt(Screen.height / 16f)) return;
            big = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(Screen.height / 16f), fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            big.normal.textColor = Thread;
            small = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(Screen.height / 30f) };
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
            float h = Screen.height * 0.34f, w = h * 64f / 96f, gap = w * 0.35f;
            float total = Knights.All.Length * w + (Knights.All.Length - 1) * gap;
            return new Rect((Screen.width - total) * 0.5f + i * (w + gap), Screen.height * 0.38f, w, h);
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
                    var t = Art.Texture("scenery/title");
                    if (t != null) { float tw = Mathf.Min(W * 0.8f, H * 0.95f), th = tw * t.height / t.width; GUI.DrawTexture(new Rect((W - tw) * 0.5f, H * 0.15f, tw, th), t); }
                    GUI.Label(new Rect(0, H * 0.26f, W, H * 0.07f), "World 1-1 · " + Level1.Name, center);
                    for (int i = 0; i < Knights.All.Length; i++)
                    {
                        var r = PortraitRect(i);
                        if (i == knightIndex) { var c = GUI.color; GUI.color = new Color(0.66f, 0.26f, 0.18f, 0.35f); GUI.DrawTexture(new Rect(r.x - 6, r.y - 6, r.width + 12, r.height + 12), Texture2D.whiteTexture); GUI.color = c; }
                        else GUI.color = new Color(1f, 1f, 1f, 0.55f);
                        DrawFrame(Knights.All[i].Sheet, i == knightIndex ? (int)(Time.time * 3f) % 3 : 0, r);
                        GUI.color = Color.white;
                    }
                    var def = Knights.All[knightIndex];
                    GUI.Label(new Rect(0, H * 0.74f, W, H * 0.07f), def.Title, big);
                    GUI.Label(new Rect(W * 0.1f, H * 0.81f, W * 0.8f, H * 0.06f), def.Trait, center);
                    GUI.Label(new Rect(W * 0.1f, H * 0.88f, W * 0.8f, H * 0.08f),
                        TapestryInput.ShowTouchControls ? "Tap a knight to choose, tap again to ride out" : "LEFT / RIGHT to choose your knight  -  JUMP or Enter to begin", center);
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
                        GUI.Label(new Rect(0, H * 0.33f, W, H * 0.12f), "PAUSA", big);
                        GUI.Label(new Rect(0, H * 0.46f, W, H * 0.16f), "JUMP or Start to continue\nDOWN to return to the title", center);
                    }
                    if (state == State.Won)
                    {
                        Panel(new Rect(W * 0.15f, H * 0.24f, W * 0.7f, H * 0.5f));
                        GUI.Label(new Rect(0, H * 0.27f, W, H * 0.1f), "HIC MILES VICIT", big);
                        GUI.Label(new Rect(W * 0.2f, H * 0.38f, W * 0.6f, H * 0.12f), "Here the knight is victorious!\nBut Princess Aelfgyva is held in another castle...", center);
                        DrawFrame("princess/aelfgyva", 1, new Rect(W * 0.5f - H * 0.06f, H * 0.5f, H * 0.12f, H * 0.23f));
                    }
                    if (state == State.GameOver)
                    {
                        Panel(new Rect(W * 0.25f, H * 0.3f, W * 0.5f, H * 0.3f));
                        GUI.Label(new Rect(0, H * 0.34f, W, H * 0.12f), "FINIS", big);
                        GUI.Label(new Rect(0, H * 0.46f, W, H * 0.1f), "The beasts have won this day. Press JUMP.", center);
                    }
                    if (state == State.Playing && TapestryInput.ShowTouchControls) TouchButtons();
                    break;
            }
        }

        void Hud(float W, float H)
        {
            float y = H * 0.125f, s = H * 0.05f;
            GUI.Label(new Rect(W * 0.02f, y, W * 0.3f, s * 1.2f), player.Def.Title + (player.Mounted ? " on Bayard" : ""), small);
            for (int i = 0; i < player.Hearts; i++)
                DrawFrame("scenery/roundel_block", 0, new Rect(W * 0.02f + i * s * 1.1f, y + s * 1.2f, s, s));
            DrawFrame("scenery/bezant", 0, new Rect(W * 0.40f, y + s * 0.1f, s, s));
            GUI.Label(new Rect(W * 0.40f + s * 1.2f, y, W * 0.15f, s * 1.2f), "× " + bezantCount, small);
            GUI.Label(new Rect(W * 0.58f, y, W * 0.2f, s * 1.2f), "Vitae × " + lives, small);
            GUI.Label(new Rect(W * 0.76f, y, W * 0.22f, s * 1.2f), "Gloria " + score.ToString("000000"), small);
        }

        void TouchButtons()
        {
            Button(TapestryInput.LeftButton, "<");
            Button(TapestryInput.RightButton, ">");
            Button(TapestryInput.DownButton, "v");
            Button(TapestryInput.RunButton, "RUN");
            Button(TapestryInput.JumpButton, "JUMP");
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
