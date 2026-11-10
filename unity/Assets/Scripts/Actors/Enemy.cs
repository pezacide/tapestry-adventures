using UnityEngine;

namespace Tapestry
{
    /// <summary>Every bestiary beast, one class with a behaviour per kind (see the cases in Think).</summary>
    public class Enemy
    {
        public readonly string Kind;
        public readonly Body Body;
        public bool Alive = true, Active, Removed;
        public bool IsShell { get { return shell; } }
        public bool ShellMoving { get { return shell && shellVx != 0f && !carried; } }
        public bool Carried { get { return carried; } }
        bool IsSnail { get { return Kind == "snail_hound" || Kind == "snail_moss"; } }
        bool carried;      // a green (mossy) shell held by a knight
        public int ShellCombo;   // beasts this shell has felled in a row (the score doubles each time)
        Knight carrier;

        readonly SpriteView view;
        readonly Vector2 home;
        float dir = -1f, timer, anim, kickGrace, deadTimer, shellVx;
        bool shell, squashed, flipped, charging, swooping, gravity = true;
        float speed;

        public Enemy(string kind, Vector2 pos, Transform parent)
        {
            Kind = kind;
            home = pos;
            var sheet = Art.Sheet("enemies/" + (kind == "leaper" ? "fish" : kind));
            float w = (sheet.MaxX - sheet.MinX) / Art.DesignPerTile * 0.62f;
            float h = (sheet.MaxY - sheet.MinY) / Art.DesignPerTile * 0.78f;
            Body = new Body(pos, w, h);
            view = new SpriteView("enemies/" + (kind == "leaper" ? "fish" : kind), parent, 10);
            view.Face(false);
            switch (kind)
            {
                case "snail_hound": case "snail_moss": speed = 1.1f; break;
                case "killer_hare": speed = 0f; break;
                case "fiery_lion": speed = 1.4f; timer = 1f; break;
                case "bayeux_hound": speed = 4.2f; break;
                case "camel_dragon": speed = 0.9f; break;
                case "unicorn": speed = 1.2f; break;
                case "wyvern": gravity = false; break;
                case "owl_face": gravity = false; break;
                case "gull": gravity = false; break;
                case "bird": gravity = false; break;
                case "fish": case "sea_serpent": case "sea_monk": case "leaper": case "hippocamp": gravity = false; break;
                case "crab": speed = 2f; break;
                case "soup_cat": speed = 0f; timer = 1.2f; break;
            }
            view.SetPosition(pos);
        }

        public void Tick(float dt, Level lv, Knight player)
        {
            if (Removed) return;
            if (!Alive) { TickDead(dt); return; }
            timer += dt; anim += dt; kickGrace -= dt;
            if (carried) { TickCarried(); return; }
            Think(dt, lv, player);
            if (gravity)
            {
                Body.Vel.y = Mathf.Max(Body.Vel.y - 40f * dt, -16f);
                Body.Step(lv, dt);
            }
            else Body.Pos += Body.Vel * dt;
            if (Body.Pos.y < -3f) Remove();
            view.SetPosition(Body.Pos);
        }

        void Walk(Level lv, bool turnAtLedges)
        {
            if (Body.HitWall) dir = -dir;
            if (turnAtLedges && Body.Grounded && !Body.FloorAhead(lv, dir)) dir = -dir;
            Body.Vel.x = dir * speed;
            view.Face(dir > 0f);
        }

        void WalkFrames(int a = 0, int b = 1, float rate = 5f) { view.SetFrame(((int)(anim * rate) % 2) == 0 ? a : b); }

        float ToPlayer(Knight p) { return p.Body.Pos.x - Body.Pos.x; }

        void Think(float dt, Level lv, Knight p)
        {
            float dx = ToPlayer(p);
            switch (Kind)
            {
                case "snail_hound":
                case "snail_moss":
                    if (shell)
                    {
                        view.SetFrame(2);
                        if (Body.HitWall && shellVx != 0f) shellVx = -shellVx;
                        Body.Vel.x = shellVx;
                    }
                    else { Walk(lv, true); WalkFrames(); }
                    break;

                case "bayeux_hound":
                    Walk(lv, false);
                    if (Body.Grounded && !Body.FloorAhead(lv, dir)) Body.Vel.y = 9f;   // leaps small gaps
                    WalkFrames(0, 1, 8f);
                    break;

                case "camel_dragon":
                    Walk(lv, true); WalkFrames(0, 1, 3f);
                    break;

                case "killer_hare":
                    if (Body.Grounded)
                    {
                        Body.Vel.x = 0f;
                        view.Face(dx > 0f);
                        if (timer > 0.75f) { timer = 0f; Body.Vel.y = 10f; Body.Vel.x = Mathf.Sign(dx) * 3.5f; }
                        view.SetFrame(0);
                    }
                    else view.SetFrame(1);
                    break;

                case "fiery_lion":
                    if (timer > 2.6f && Mathf.Abs(dx) < 10f)
                    {
                        Body.Vel.x = 0f; view.Face(dx > 0f); view.SetFrame(2);
                        if (timer > 3.1f)
                        {
                            timer = 0f;
                            Audio.Play("roar", 0.7f); Audio.Play("fireball", 0.6f);
                            GameManager.I.SpawnShot("fireball", Body.Pos + new Vector2(Mathf.Sign(dx) * Body.W * 0.7f, Body.H * 0.65f), new Vector2(Mathf.Sign(dx) * 6.5f, 0f));
                        }
                    }
                    else { Walk(lv, true); WalkFrames(); }
                    break;

                case "unicorn":
                    if (charging)
                    {
                        Body.Vel.x = dir * 8.5f;
                        WalkFrames(0, 1, 10f);
                        if (Body.HitWall || timer > 2f || (Body.Grounded && !Body.FloorAhead(lv, dir))) { charging = false; timer = 0f; dir = -dir; }
                    }
                    else
                    {
                        Walk(lv, true); WalkFrames(0, 1, 3f);
                        bool ahead = Mathf.Sign(dx) == dir;
                        if (ahead && Mathf.Abs(dx) < 8f && Mathf.Abs(p.Body.Pos.y - Body.Pos.y) < 1.5f && timer > 1f) { charging = true; timer = 0f; }
                    }
                    break;

                case "wyvern":
                    Body.Pos = new Vector2(home.x + Mathf.Sin(timer * 0.8f) * 3f, home.y + Mathf.Sin(timer * 2.1f) * 0.6f);
                    Body.Vel = Vector2.zero;
                    view.Face(Mathf.Cos(timer * 0.8f) > 0f);
                    WalkFrames(0, 1, 6f);
                    break;

                case "owl_face":
                    if (!swooping)
                    {
                        view.SetFrame(0);
                        view.Face(dx > 0f);
                        if (Mathf.Abs(dx) < 4f && p.Body.Pos.y < Body.Pos.y) { swooping = true; timer = 0f; Body.Vel = (p.Body.Pos - Body.Pos).normalized * 6f; }
                    }
                    else
                    {
                        WalkFrames(1, 1);
                        if (timer > 1.1f) Body.Vel = new Vector2(Mathf.Sign(Body.Vel.x == 0f ? 1f : Body.Vel.x) * 3f, 4f);
                        if (Body.Pos.y > 14f) Remove();
                    }
                    break;

                case "gull":
                    // wheels to and fro over its patch of shore, then dives at a knight beneath it
                    if (!swooping)
                    {
                        Body.Pos.y = Mathf.MoveTowards(Body.Pos.y, home.y + Mathf.Sin(timer * 2f) * 0.4f, 3f * dt);
                        Body.Vel = new Vector2(dir * 2.4f, 0f);
                        if (Body.Pos.x < home.x - 4f) dir = 1f; else if (Body.Pos.x > home.x + 4f) dir = -1f;
                        view.Face(dir > 0f);
                        WalkFrames(0, 1, 5f);
                        if (Mathf.Abs(dx) < 2f && p.Body.Pos.y < Body.Pos.y - 1f && timer > 1.5f)
                        { swooping = true; timer = 0f; Body.Vel = (p.Body.Pos + new Vector2(0f, 0.8f) - Body.Pos).normalized * 7f; Audio.Play("jump", 0.4f); }
                    }
                    else
                    {
                        view.SetFrame(1);
                        view.Face(Body.Vel.x > 0f);
                        if (timer > 0.7f || Body.Pos.y < p.Body.Pos.y + 0.6f) { swooping = false; timer = 0f; }   // pull up and climb back
                    }
                    break;

                case "fish":   // swims to and fro in a gentle wave, turning at rocks
                    if (Body.Pos.x < home.x - 7f) dir = 1f; else if (Body.Pos.x > home.x + 7f) dir = -1f;
                    Body.Vel = new Vector2(dir * 2.2f, Mathf.Cos(timer * 2f) * 0.9f);
                    Body.Step(lv, dt); if (Body.HitWall) dir = -dir;
                    Body.Vel = Vector2.zero;
                    view.Face(dir > 0f); WalkFrames(0, 1, 4f);
                    break;

                case "sea_serpent":   // long patrols in coils
                    if (Body.Pos.x < home.x - 5f) dir = 1f; else if (Body.Pos.x > home.x + 5f) dir = -1f;
                    Body.Pos = new Vector2(Body.Pos.x + dir * 2.6f * dt, home.y + Mathf.Sin(timer * 1.6f) * 1.1f);
                    view.Face(dir > 0f); WalkFrames(0, 1, 3f);
                    break;

                case "hippocamp":   // paddles in place, then gallops through the water at the knight
                    if (charging)
                    {
                        Body.Pos.x += dir * 6f * dt;
                        Body.Pos.y = Mathf.MoveTowards(Body.Pos.y, p.Body.Pos.y + 0.3f, 1.5f * dt);
                        WalkFrames(0, 1, 10f);
                        if (timer > 1.6f) { charging = false; timer = 0f; }
                    }
                    else
                    {
                        Body.Pos.y = home.y + Mathf.Sin(anim * 2f) * 0.3f;
                        Body.Pos.x = Mathf.MoveTowards(Body.Pos.x, home.x, 1.5f * dt);
                        dir = dx > 0f ? 1f : -1f;
                        WalkFrames(0, 1, 3f);
                        if (Mathf.Abs(dx) < 8f && Mathf.Abs(p.Body.Pos.y - Body.Pos.y) < 3f && timer > 1.5f) { charging = true; timer = 0f; Audio.Play("mount", 0.4f); }
                    }
                    view.Face(dir > 0f);
                    break;

                case "sea_monk":   // sinks slowly, then pulses toward the knight (like Mario's Blooper)
                    if (timer > 1.1f && Mathf.Abs(dx) < 9f)
                    {
                        timer = 0f;
                        var to = (p.Body.Pos + new Vector2(0f, 1f) - Body.Pos);
                        Body.Vel = new Vector2(Mathf.Clamp(to.x, -1f, 1f) * 3.5f, to.y > -0.5f ? 4.5f : 1.5f);
                    }
                    Body.Vel = Vector2.MoveTowards(Body.Vel, new Vector2(0f, -0.7f), 4f * dt);
                    if (Body.Pos.y < 2f && Body.Vel.y < 0f) Body.Vel.y = 0f;
                    if (Body.Pos.y + Body.H > 9.5f && Body.Vel.y > 0f) Body.Vel.y = 0f;
                    view.SetFrame(Body.Vel.y > 1f ? 1 : 0);
                    break;

                case "bird":   // flies straight across the sky toward the knight, in a gentle wave
                    Body.Vel = new Vector2(-2.8f, Mathf.Cos(timer * 3f) * 1.2f);
                    view.Face(false); WalkFrames(0, 1, 6f);
                    if (Body.Pos.x < home.x - 40f) Remove();
                    break;

                case "leaper":   // a fish leaping from the sea in an arc, then back in
                    Body.Vel.y -= 20f * dt;
                    view.Face(Body.Vel.x > 0f); WalkFrames(0, 1, 8f);
                    view.Renderer.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(Body.Vel.y, Mathf.Abs(Body.Vel.x)) * Mathf.Rad2Deg * (Body.Vel.x > 0f ? 1f : -1f) * 0.6f);
                    if (Body.Pos.y < -2f && Body.Vel.y < 0f) Remove();
                    break;

                case "crab":
                    Walk(lv, true); WalkFrames(0, 1, 7f);
                    if (Body.Grounded && timer > 1.8f) { timer = 0f; Body.Vel.y = 7f; }
                    break;

                case "soup_cat":
                    view.Face(dx > 0f);
                    if (timer > 2.4f && Mathf.Abs(dx) < 11f)
                    {
                        view.SetFrame(2);
                        if (timer > 2.8f)
                        {
                            timer = 0f;
                            Audio.Play("soup", 0.8f);
                            GameManager.I.SpawnShot("soup_glob", Body.Pos + new Vector2(0f, Body.H), new Vector2(Mathf.Clamp(dx * 0.55f, -5f, 5f), 8f));
                        }
                    }
                    else WalkFrames(0, 1, 3f);
                    break;
            }
        }

        /// <summary>The knight touched this beast. fromAbove = it was a stomp.</summary>
        public void OnKnight(Knight k, bool fromAbove)
        {
            if (!Alive || k.Dead) return;
            if (IsSnail && shell)
            {
                if (ShellMoving)
                {
                    if (fromAbove) { shellVx = 0f; ShellCombo = 0; k.Bounce(); }
                    else if (kickGrace <= 0f) k.Hurt();
                }
                else if (Kind == "snail_moss" && TapestryInput.RunHeld && !k.Mounted && k.Carrying == null)
                {
                    // hold RUN against a green shell to pick it up
                    carried = true; carrier = k; k.Carrying = this; shellVx = 0f; ShellCombo = 0;
                    Audio.Play("select");
                }
                else
                {
                    float d = k.Body.Pos.x < Body.Pos.x ? 1f : -1f;
                    shellVx = d * 10f; kickGrace = 0.3f; ShellCombo = 0;
                    Audio.Play("kick");
                    if (fromAbove) k.Bounce();
                }
                return;
            }
            if (!fromAbove) { k.Hurt(); return; }
            if (k.Mounted) { k.Bounce(); Squash(); return; }   // Bayard's hooves crush every beast, burning or not
            if (Kind == "fiery_lion") { k.Hurt(); k.Bounce(); return; }   // burning mane
            k.Bounce();
            if (IsSnail)
            {
                shell = true; Body.Vel.x = 0f; Body.H *= 0.8f; view.SetFrame(2);
                Audio.Play("stomp");
                GameManager.I.AddScore(100, Body.Pos + new Vector2(0f, Body.H));
                return;
            }
            Squash();
        }

        /// <summary>Held in front of the knight; letting go of RUN hurls it.</summary>
        void TickCarried()
        {
            var k = carrier;
            float d = k.FacingRight ? 1f : -1f;
            Body.Pos = k.Body.Pos + new Vector2(d * (k.Body.W * 0.5f + Body.W * 0.5f - 0.05f), k.Mounted ? 0.9f : 0.45f);
            Body.Vel = Vector2.zero;
            view.SetFrame(2);
            view.SetPosition(Body.Pos);
            if (k.Dead) Release(0f);
            else if (!TapestryInput.RunHeld) { Release(d * 11f); Audio.Play("kick"); }
        }

        void Release(float vx)
        {
            carried = false;
            if (carrier != null && carrier.Carrying == this) carrier.Carrying = null;
            carrier = null;
            shellVx = vx; kickGrace = 0.4f; ShellCombo = 0;
            if (vx != 0f) Body.Vel.y = 2f;
        }

        public void Squash()
        {
            Alive = false; squashed = true; deadTimer = 0f;
            Audio.Play("stomp");
            view.Root.transform.localScale = new Vector3(1f, 0.45f, 1f);
            GameManager.I.AddScore(100, Body.Pos + new Vector2(0f, Body.H));
        }

        /// <summary>Knocked out by a shell or a bumped block: flips over and falls off the screen.</summary>
        public void Flip(bool award = true)
        {
            if (!Alive) return;
            if (carried) Release(0f);
            Alive = false; flipped = true;
            Audio.Play("kick", 0.8f);
            view.Renderer.flipY = true;
            Body.Vel = new Vector2(Body.Vel.x * 0.3f, 8f);
            if (award) GameManager.I.AddScore(200, Body.Pos + new Vector2(0f, Body.H));
        }

        void TickDead(float dt)
        {
            deadTimer += dt;
            if (squashed && deadTimer > 0.45f) Remove();
            if (flipped)
            {
                Body.Vel.y -= 40f * dt;
                Body.Pos += Body.Vel * dt;
                view.SetPosition(Body.Pos);
                if (Body.Pos.y < -4f) Remove();
            }
        }

        void Remove() { if (Removed) return; Removed = true; Alive = false; view.Destroy(); }
        public void Destroy() { Remove(); }
    }

    /// <summary>Fireballs and soup.</summary>
    public class Shot
    {
        public readonly Body Body;
        public bool Removed;
        public readonly bool Friendly;   // a knight's crossbowe bolt (hurts beasts, not knights)
        public readonly string Kind;
        readonly SpriteView view;
        readonly bool gravity;
        readonly float life;
        float t;

        public Shot(string kind, Vector2 pos, Vector2 vel, Transform parent)
        {
            Body = new Body(pos, 0.35f, 0.3f) { Vel = vel, UsePlatforms = false };
            gravity = kind == "soup_glob";
            Friendly = kind == "bolt";
            Kind = kind;
            life = Friendly ? 1.1f : 4f;
            view = new SpriteView("projectiles/" + kind, parent, 15);
            view.Face(vel.x >= 0f);
            view.SetPosition(pos);
        }

        public void Tick(float dt, Level lv)
        {
            if (Removed) return;
            t += dt;
            if (gravity) Body.Vel.y -= 22f * dt;
            Body.Step(lv, dt);
            view.SetFrame(((int)(t * 10f)) % 2);
            view.SetPosition(Body.Pos);
            if (Body.HitWall || Body.Grounded || Body.HitCeiling || t > life || Body.Pos.y < -2f) Destroy();
        }

        public void Destroy() { if (Removed) return; Removed = true; view.Destroy(); }
    }
}
