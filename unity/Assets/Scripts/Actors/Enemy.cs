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
        float dir = -1f, timer, anim, kickGrace, deadTimer, shellVx, lifeT;
        bool shell, squashed, flipped, charging, swooping, gravity = true;
        float speed, plantLevel;
        /// <summary>A plant sunk inside its urn: it cannot touch, or be touched.</summary>
        public bool Untouchable { get; private set; }
        /// <summary>1 normally; faster on 'Ye Wronge Syde' (the beasts, their hammers and spears all move and wait that much quicker).</summary>
        public static float TimeScale = 1f;

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
                case "stag": speed = 5.2f; break;
                case "talbot": speed = 6.5f; dir = 1f; break;
                case "boar": speed = 1.3f; break;
                case "hunter": speed = 1.0f; timer = 1f; break;
                case "lapdog": speed = 2.6f; break;
                case "smith": speed = 1.4f; break;
                case "urn_plant": gravity = false; break;
                case "ember": gravity = false; timer = (pos.x * 0.37f) % 2f; break;
                case "fox": speed = 1.4f; break;
                case "monkey": speed = 0f; timer = 1f; break;
                case "parakeet": gravity = false; break;
                case "locust": gravity = false; break;
                case "horseman_white": case "horseman_red": case "horseman_black": case "horseman_pale": speed = 9.5f; dir = 1f; break;
                case "greyhound": speed = 0f; break;
                case "genet": speed = 1.2f; break;
                case "pheasant": gravity = false; break;
                case "otter": case "swan": case "falcon": gravity = false; break;
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
            if (kind == "urn_plant") view.Renderer.sortingOrder = -1;   // it rises from behind the urn
            view.SetPosition(pos);
        }

        public void Tick(float dt, Level lv, Knight player)
        {
            if (Removed) return;
            dt *= TimeScale;
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

                case "stag":   // runs with its herd, leaping gaps, turning back at walls; leaves the field after a while
                    if (Body.HitWall) dir = -dir;
                    Body.Vel.x = dir * speed; view.Face(dir > 0f);
                    if (Body.Grounded && !Body.FloorAhead(lv, dir)) Body.Vel.y = 10f;
                    WalkFrames(0, 1, 9f);
                    lifeT += dt;
                    if (lifeT > 10f || Mathf.Abs(Body.Pos.x - p.Body.Pos.x) > 18f) Remove();
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

                case "talbot":   // a hound of the hunt: runs on past the knight, leaping gaps, then leaves
                    if (Body.HitWall) dir = -dir;
                    Body.Vel.x = dir * speed; view.Face(dir > 0f);
                    if (Body.Grounded && !Body.FloorAhead(lv, dir)) Body.Vel.y = 10f;
                    WalkFrames(0, 1, 10f);
                    lifeT += dt;
                    if (lifeT > 8f || Body.Pos.x > p.Body.Pos.x + 22f) Remove();
                    break;

                case "hunter":   // walks its beat; when the knight is in front, it hurls a spear in a long arc
                    if (charging)
                    {
                        Body.Vel.x = 0f; view.SetFrame(2);
                        if (timer > 0.4f)
                        {
                            charging = false; timer = 0f;
                            Audio.Play("spear", 0.7f);
                            GameManager.I.SpawnShot("spear", Body.Pos + new Vector2(dir * 0.6f, Body.H * 0.85f), new Vector2(dir * 7f, 4.5f));
                        }
                    }
                    else
                    {
                        Walk(lv, true); WalkFrames(0, 1, 4f);
                        if (timer > 2.6f && Mathf.Abs(dx) < 10f && Mathf.Abs(dx) > 1.5f && Mathf.Abs(p.Body.Pos.y - Body.Pos.y) < 3.5f)
                        { charging = true; timer = 0f; dir = Mathf.Sign(dx); view.Face(dir > 0f); }
                    }
                    break;

                case "greyhound":   // lies in wait; when the knight comes near it races at him, leaping gaps, and runs on
                    if (!charging)
                    {
                        Body.Vel.x = 0f; view.SetFrame(1); view.Face(dx > 0f);
                        if (Mathf.Abs(dx) < 8f && Mathf.Abs(p.Body.Pos.y - Body.Pos.y) < 3f) { charging = true; dir = Mathf.Sign(dx); Audio.Play("roar", 0.35f); }
                    }
                    else
                    {
                        if (Body.HitWall) dir = -dir;
                        Body.Vel.x = dir * 7.5f; view.Face(dir > 0f);
                        if (Body.Grounded && !Body.FloorAhead(lv, dir)) Body.Vel.y = 10f;
                        WalkFrames(0, 1, 11f);
                        if (Mathf.Abs(Body.Pos.x - p.Body.Pos.x) > 20f) Remove();
                    }
                    break;

                case "pheasant":   // crouches in the flowers, then bursts up with a whirr and flies off across the sky
                    if (!swooping)
                    {
                        view.SetFrame(0); view.Face(dx > 0f);
                        if (Mathf.Abs(dx) < 4.5f) { swooping = true; timer = 0f; dir = Mathf.Sign(dx); Body.Vel = new Vector2(dir * 4.5f, 7f); Audio.Play("jump", 0.4f); }
                    }
                    else
                    {
                        Body.Vel.y = Mathf.MoveTowards(Body.Vel.y, 0f, 5f * dt);
                        view.Face(dir > 0f); view.SetFrame(1);
                        if (timer > 7f || Body.Pos.y > 14f) Remove();
                    }
                    break;

                case "genet":   // paces, then pounces at the knight in long low leaps
                    if (Body.Grounded)
                    {
                        if (timer > 1.6f && Mathf.Abs(dx) < 7f) { timer = 0f; dir = Mathf.Sign(dx); Body.Vel = new Vector2(dir * 6f, 7.5f); view.Face(dir > 0f); }
                        else { Walk(lv, true); WalkFrames(0, 0, 1f); }
                    }
                    else view.SetFrame(1);
                    break;

                case "locust":   // hovers about its place, then drifts after a knight who comes near, wings whirring
                    {
                        var target = Mathf.Abs(dx) < 9f ? p.Body.Pos + new Vector2(0f, 1.2f) : home;
                        if ((target - home).magnitude > 10f) target = home;
                        var to = target - Body.Pos;
                        Body.Vel = to.normalized * Mathf.Min(2.4f, to.magnitude * 2f) + new Vector2(Mathf.Sin(timer * 5f + home.x) * 1.2f, Mathf.Cos(timer * 4f) * 1.2f);
                        view.Face(Body.Vel.x > 0f); WalkFrames(0, 1, 12f);
                    }
                    break;

                case "horseman_white": case "horseman_red": case "horseman_black": case "horseman_pale":
                    // one of the four horsemen: gallops in from behind and across the realm, leaping gaps
                    if (Body.HitWall) Body.Vel.y = Body.Grounded ? 12f : Body.Vel.y;
                    Body.Vel.x = dir * speed; view.Face(dir > 0f);
                    if (Body.Grounded && !Body.FloorAhead(lv, dir)) Body.Vel.y = 12f;
                    WalkFrames(0, 1, 9f);
                    lifeT += dt;
                    if (lifeT > 9f || Body.Pos.x > p.Body.Pos.x + 24f) Remove();
                    break;

                case "smith":   // paces a short beat, hops now and then, and hurls hammers in arcs at a knight in range (they come in pairs)
                    if (charging)
                    {
                        Body.Vel.x = 0f; view.SetFrame(2); view.Face(dx > 0f);
                        if (timer > 0.35f)
                        {
                            charging = false; timer = 0f;
                            float d = dx > 0f ? 1f : -1f;
                            GameManager.I.SpawnShot("hammer", Body.Pos + new Vector2(d * 0.3f, Body.H + 0.2f), new Vector2(Mathf.Clamp(dx * 0.55f, -5.5f, 5.5f), 9.5f));
                            Audio.Play("spear", 0.5f);
                        }
                        break;
                    }
                    if (Mathf.Abs(Body.Pos.x - home.x) > 2.4f) dir = Body.Pos.x < home.x ? 1f : -1f;
                    else if (Body.HitWall) dir = -dir;
                    Body.Vel.x = dir * speed * 0.8f; view.Face(dir > 0f);
                    if (Body.Grounded) WalkFrames(0, 1, 3f); else view.SetFrame(1);
                    lifeT += dt;
                    if (Body.Grounded && lifeT > 2.6f + (home.x % 1f)) { lifeT = 0f; Body.Vel.y = 11f; }   // a hop
                    else if (Body.Grounded && timer > 1.9f && Mathf.Abs(dx) < 11f && Mathf.Abs(dx) > 1.2f) { charging = true; timer = 0f; }
                    break;

                case "ember":   // a spark of the fire pit: sinks from sight, then leaps high and falls back in; it cannot be stomped or shot
                    if (!swooping)
                    {
                        Body.Pos = new Vector2(home.x, home.y - 0.9f); view.Renderer.enabled = false; Body.Vel = Vector2.zero;
                        if (timer > 2.2f) { swooping = true; timer = 0f; Body.Pos = new Vector2(home.x, home.y); Body.Vel = new Vector2(0f, 12.5f); Audio.Play("soup", 0.3f); }
                    }
                    else
                    {
                        view.Renderer.enabled = true; view.SetFrame(Body.Vel.y > 0f ? 0 : 1);
                        Body.Vel.y -= 24f * dt;
                        if (Body.Pos.y < home.y - 0.6f && Body.Vel.y < 0f) { swooping = false; timer = 0f; }
                    }
                    Untouchable = !swooping;
                    break;

                case "urn_plant":   // rises from its urn, snaps, and sinks again; it stays down while a knight is near or upon the urn
                    {
                        bool near = Mathf.Abs(dx) < 1.5f && p.Body.Pos.y < home.y + 3f;
                        bool shouldBeUp = (timer % 4.4f) < 2.8f && !near;
                        plantLevel = Mathf.MoveTowards(plantLevel, shouldBeUp ? 1f : 0f, dt * 1.6f);
                        Body.Pos = new Vector2(home.x, home.y - 1.1f * (1f - plantLevel));
                        view.Renderer.enabled = plantLevel > 0.05f;
                        view.SetFrame(plantLevel > 0.9f ? 1 : 0);
                        Untouchable = plantLevel < 0.35f;
                    }
                    break;

                case "lapdog":   // scampers to and fro, yapping, and hops at the knight when he is near
                    Walk(lv, true); WalkFrames(0, 1, 9f);
                    if (Body.Grounded && timer > 1.4f && Mathf.Abs(dx) < 5f) { timer = 0f; Body.Vel.y = 8f; dir = Mathf.Sign(dx); Audio.Play("jump", 0.25f); }
                    break;

                case "fox":   // slinks along; darts at the knight when his back is turned
                    if (charging)
                    {
                        Body.Vel.x = dir * 7f; view.Face(dir > 0f); WalkFrames(0, 1, 12f);
                        if (timer > 1.1f || Body.HitWall) { charging = false; timer = 0f; }
                    }
                    else
                    {
                        Walk(lv, true); WalkFrames(0, 1, 4f);
                        bool backTurned = (dx > 0f) == p.FacingRight;   // the knight faces away from the fox
                        if (timer > 1.5f && Mathf.Abs(dx) < 7f && backTurned && Mathf.Abs(p.Body.Pos.y - Body.Pos.y) < 1.5f)
                        { charging = true; timer = 0f; dir = Mathf.Sign(dx); }
                    }
                    break;

                case "monkey":   // sits nibbling; flings comfits in arcs at a knight in range
                    Body.Vel.x = 0f; view.Face(dx > 0f);
                    if (timer > 2.4f && Mathf.Abs(dx) < 9f)
                    {
                        view.SetFrame(1);
                        if (timer > 2.7f)
                        {
                            timer = 0f;
                            GameManager.I.SpawnShot("comfit", Body.Pos + new Vector2(Mathf.Sign(dx) * 0.4f, Body.H), new Vector2(Mathf.Clamp(dx * 0.7f, -5f, 5f), 8f));
                            Audio.Play("jump", 0.3f);
                        }
                    }
                    else view.SetFrame(0);
                    break;

                case "parakeet":   // flutters in a wavering line toward the knight, then on past
                    if (!swooping) { if (Mathf.Abs(dx) < 13f) { swooping = true; dir = Mathf.Sign(dx); timer = 0f; } }
                    else
                    {
                        Body.Vel = new Vector2(dir * 3.2f, Mathf.Cos(timer * 4f) * 1.6f);
                        if (Mathf.Abs(Body.Pos.x - home.x) > 30f) Remove();
                    }
                    view.Face(dir > 0f); WalkFrames(0, 1, 8f);
                    break;

                case "otter":   // hides in the river, then leaps up at the knight and drops back in
                    if (!swooping)
                    {
                        Body.Pos = new Vector2(home.x, home.y - 1.2f); view.Renderer.enabled = false;
                        if (timer > 1.8f && Mathf.Abs(dx) < 5f) { swooping = true; timer = 0f; Body.Pos = new Vector2(home.x, home.y - 0.6f); Body.Vel = new Vector2(Mathf.Sign(dx) * 1.5f, 11f); Audio.Play("soup", 0.5f); }
                    }
                    else
                    {
                        view.Renderer.enabled = true; view.SetFrame(1); view.Face(Body.Vel.x > 0f);
                        Body.Vel.y -= 22f * dt;
                        if (Body.Pos.y < home.y - 0.8f && Body.Vel.y < 0f) { swooping = false; timer = 0f; Body.Vel = Vector2.zero; }
                    }
                    break;

                case "swan":   // glides on the water; charges at a knight on its level, hissing
                    Body.Pos.y = home.y + Mathf.Sin(anim * 2f) * 0.06f;
                    if (charging)
                    {
                        Body.Vel = new Vector2(dir * 5.5f, 0f);
                        if (timer > 1.2f || Mathf.Abs(Body.Pos.x - home.x) > 7f) { charging = false; timer = 0f; }
                    }
                    else
                    {
                        if (Body.Pos.x < home.x - 3f) dir = 1f; else if (Body.Pos.x > home.x + 3f) dir = -1f;
                        Body.Vel = new Vector2(dir * 1.2f, 0f);
                        if (timer > 2f && Mathf.Abs(dx) < 6f && Mathf.Abs(p.Body.Pos.y - Body.Pos.y) < 2f && Mathf.Sign(dx) == dir)
                        { charging = true; timer = 0f; Audio.Play("fireball", 0.5f); }
                    }
                    view.Face(dir > 0f); WalkFrames(0, 1, charging ? 8f : 2f);
                    break;

                case "unicorn":
                case "boar":
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
                case "falcon":
                    // wheels to and fro over its patch of ground, then dives at a knight beneath it
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

                case "hippocamp":
                    // Paddles near its home. When the knight is in front of it, close by and level with it, it rears
                    // (a half-second warning), then makes ONE straight dash of about 4 tiles - no steering - and rests.
                    // It never strays more than 6 tiles from home.
                    if (charging)
                    {
                        if (timer < 0.5f)   // the warning: rearing and flashing in place
                        {
                            view.SetFrame(1);
                            view.Renderer.enabled = Mathf.Repeat(timer, 0.12f) > 0.05f;
                        }
                        else
                        {
                            view.Renderer.enabled = true;
                            Body.Pos.x += dir * 5f * dt;
                            WalkFrames(0, 1, 10f);
                            if (timer > 1.3f || Mathf.Abs(Body.Pos.x - home.x) > 6f) { charging = false; timer = 0f; }
                        }
                    }
                    else
                    {
                        view.Renderer.enabled = true;
                        Body.Pos.x = Mathf.MoveTowards(Body.Pos.x, home.x, 2f * dt);
                        Body.Pos.y = Mathf.MoveTowards(Body.Pos.y, home.y + Mathf.Sin(anim * 2f) * 0.3f, 2f * dt);
                        dir = dx > 0f ? 1f : -1f;
                        WalkFrames(0, 1, 3f);
                        bool inFront = Mathf.Abs(dx) < 6f && Mathf.Abs(p.Body.Pos.y - Body.Pos.y) < 1.5f;
                        bool homeSide = Mathf.Abs(Body.Pos.x - home.x) < 1f;
                        if (inFront && homeSide && timer > 3f) { charging = true; timer = 0f; Audio.Play("mount", 0.4f); }
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

        /// <summary>The knight touched this beast. fromAbove = it was a stomp. Returns true if the stomp felled it
        /// (or shelled it), so the game can score the stomp chain.</summary>
        public bool OnKnight(Knight k, bool fromAbove)
        {
            if (!Alive || k.Dead) return false;
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
                return false;
            }
            if (Kind == "urn_plant" || Kind == "ember") { k.Hurt(); return false; }   // it bites a knight who lands upon it, too
            if (!fromAbove) { k.Hurt(); return false; }
            if (k.Mounted) { k.Bounce(); Squash(false); return true; }   // Bayard's hooves crush every beast, burning or not
            if (Kind == "fiery_lion") { k.Hurt(); k.Bounce(); return false; }   // burning mane
            k.Bounce();
            if (IsSnail)
            {
                shell = true; Body.Vel.x = 0f; Body.H *= 0.8f; view.SetFrame(2);
                Audio.Play("stomp");
                return true;
            }
            Squash(false);
            return true;
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

        public void Squash(bool award = true)
        {
            Alive = false; squashed = true; deadTimer = 0f;
            Audio.Play("stomp");
            view.Root.transform.localScale = new Vector3(1f, 0.45f, 1f);
            if (award) GameManager.I.AddScore(100, Body.Pos + new Vector2(0f, Body.H));
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
            gravity = kind == "soup_glob" || kind == "spear" || kind == "comfit" || kind == "star" || kind == "hammer";
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
            dt *= Enemy.TimeScale;
            t += dt;
            if (gravity) Body.Vel.y -= (Kind == "spear" ? 9f : Kind == "star" ? 5f : 22f) * dt;
            if (Kind == "hammer") view.Renderer.transform.localEulerAngles = new Vector3(0f, 0f, -t * 720f * (Body.Vel.x >= 0f ? 1f : -1f));   // end over end
            if (Kind == "spear") view.Renderer.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(Body.Vel.y, Mathf.Abs(Body.Vel.x)) * Mathf.Rad2Deg * (Body.Vel.x >= 0f ? 1f : -1f));
            Body.Step(lv, dt);
            view.SetFrame(((int)(t * 10f)) % 2);
            view.SetPosition(Body.Pos);
            if (Body.HitWall || Body.Grounded || Body.HitCeiling || t > life || Body.Pos.y < -2f) Destroy();
        }

        public void Destroy() { if (Removed) return; Removed = true; view.Destroy(); }
    }
}
