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
        public bool ShellMoving { get { return shell && shellVx != 0f; } }

        readonly SpriteView view;
        readonly Vector2 home;
        float dir = -1f, timer, anim, kickGrace, deadTimer, shellVx;
        bool shell, squashed, flipped, charging, swooping, gravity = true;
        float speed;

        public Enemy(string kind, Vector2 pos, Transform parent)
        {
            Kind = kind;
            home = pos;
            var sheet = Art.Sheet("enemies/" + kind);
            float w = (sheet.MaxX - sheet.MinX) / Art.DesignPerTile * 0.62f;
            float h = (sheet.MaxY - sheet.MinY) / Art.DesignPerTile * 0.78f;
            Body = new Body(pos, w, h);
            view = new SpriteView("enemies/" + kind, parent, 10);
            view.Face(false);
            switch (kind)
            {
                case "snail_hound": speed = 1.1f; break;
                case "killer_hare": speed = 0f; break;
                case "fiery_lion": speed = 1.4f; timer = 1f; break;
                case "bayeux_hound": speed = 4.2f; break;
                case "camel_dragon": speed = 0.9f; break;
                case "unicorn": speed = 1.2f; break;
                case "wyvern": gravity = false; break;
                case "owl_face": gravity = false; break;
                case "soup_cat": speed = 0f; timer = 1.2f; break;
            }
            view.SetPosition(pos);
        }

        public void Tick(float dt, Level lv, Knight player)
        {
            if (Removed) return;
            if (!Alive) { TickDead(dt); return; }
            timer += dt; anim += dt; kickGrace -= dt;
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

                case "soup_cat":
                    view.Face(dx > 0f);
                    if (timer > 2.4f && Mathf.Abs(dx) < 11f)
                    {
                        view.SetFrame(2);
                        if (timer > 2.8f)
                        {
                            timer = 0f;
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
            if (Kind == "snail_hound" && shell)
            {
                if (ShellMoving)
                {
                    if (fromAbove) { shellVx = 0f; k.Bounce(); }
                    else if (kickGrace <= 0f) k.Hurt();
                }
                else
                {
                    float d = k.Body.Pos.x < Body.Pos.x ? 1f : -1f;
                    shellVx = d * 10f; kickGrace = 0.3f;
                    if (fromAbove) k.Bounce();
                }
                return;
            }
            if (!fromAbove) { k.Hurt(); return; }
            if (k.Mounted) { k.Bounce(); Squash(); return; }   // Bayard's hooves crush every beast, burning or not
            if (Kind == "fiery_lion") { k.Hurt(); k.Bounce(); return; }   // burning mane
            k.Bounce();
            if (Kind == "snail_hound")
            {
                shell = true; Body.Vel.x = 0f; Body.H *= 0.8f; view.SetFrame(2);
                GameManager.I.AddScore(100);
                return;
            }
            Squash();
        }

        public void Squash()
        {
            Alive = false; squashed = true; deadTimer = 0f;
            view.Root.transform.localScale = new Vector3(1f, 0.45f, 1f);
            GameManager.I.AddScore(100);
        }

        /// <summary>Knocked out by a shell or a bumped block: flips over and falls off the screen.</summary>
        public void Flip()
        {
            if (!Alive) return;
            Alive = false; flipped = true;
            view.Renderer.flipY = true;
            Body.Vel = new Vector2(Body.Vel.x * 0.3f, 8f);
            GameManager.I.AddScore(200);
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
        readonly SpriteView view;
        readonly bool gravity;
        float t;

        public Shot(string kind, Vector2 pos, Vector2 vel, Transform parent)
        {
            Body = new Body(pos, 0.35f, 0.3f) { Vel = vel, UsePlatforms = false };
            gravity = kind == "soup_glob";
            view = new SpriteView("projectiles/" + kind, parent, 15);
            view.Face(vel.x >= 0f);
            view.SetPosition(pos);
        }

        public void Tick(float dt, Level lv)
        {
            t += dt;
            if (gravity) Body.Vel.y -= 22f * dt;
            Body.Step(lv, dt);
            view.SetFrame(((int)(t * 10f)) % 2);
            view.SetPosition(Body.Pos);
            if (Body.HitWall || Body.Grounded || Body.HitCeiling || t > 4f || Body.Pos.y < -2f) Destroy();
        }

        public void Destroy() { if (Removed) return; Removed = true; view.Destroy(); }
    }
}
