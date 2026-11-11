using UnityEngine;

namespace Tapestry
{
    /// <summary>Ye Lyon and ye Unicorne, lords of World 6: the heraldic supporters of the Lady's arms, come to life in her tent.
    /// Two foes at once, three blows each:
    ///  - the LYON crouches (stomp him then!), then springs at the knight in a great arc;
    ///  - the UNICORNE rears (stomp it then!), then charges across the tent until it meets a wall.
    /// Stomping either at any other time only bounces you off. Each blow to one makes the other angrier (faster).
    /// When one falls, the other fights on alone.</summary>
    public class TwinBoss : BossBase
    {
        public override float MaxHealth { get { return 6f; } }
        public override string DefeatedText { get { return "Huzzah! Ye Lyon and ye Unicorne are overcome!"; } }

        class Part
        {
            public Body Body; public SpriteView View; public bool Lion;
            public int Hp = 3; public bool Down, Gone;
            public int State;          // lion: 0 crouch, 1 leap, 2 stunned / unicorn: 0 rear, 1 charge, 2 stunned
            public float T, Fury = 1f, Dir = -1f, Grace;
            public bool Vulnerable { get { return State == 0 && T > 0.15f; } }
        }
        readonly Part lion, unicorn;
        bool toldHow;

        public TwinBoss(Vector2 pos, Transform parent)
        {
            Title = "Ye Lyon and ye Unicorne";
            Health = MaxHealth;
            Body = new Body(pos, 1f, 1f);   // marks the middle of the tent (where the fight wakes)
            ArenaMin = pos.x - 11f; ArenaMax = pos.x + 11f;
            lion = new Part { Lion = true, Body = new Body(pos + new Vector2(-5f, 0f), 2.6f, 1.9f), View = new SpriteView("bosses/heraldic_lion", parent, 11) };
            unicorn = new Part { Body = new Body(pos + new Vector2(5f, 0f), 2.6f, 2.3f), View = new SpriteView("bosses/lady_unicorn", parent, 11), T = -0.6f };
            lion.View.Face(true); unicorn.View.Face(false);
            lion.View.SetPosition(lion.Body.Pos); unicorn.View.SetPosition(unicorn.Body.Pos);
        }

        public override void Tick(float dt, Level lv, Knight player)
        {
            if (Removed) return;
            if (!toldHow) { toldHow = true; GameManager.I.Toast("Stompe ye Lyon as he croucheth, ye Unicorne as it reareth!"); }
            TickLion(lion, dt, lv, player);
            TickUnicorn(unicorn, dt, lv, player);
            if (lion.Gone && unicorn.Gone) Removed = true;
        }

        void Fall(Part p, float dt)
        {
            p.Body.Vel.y -= 30f * dt; p.Body.Pos += p.Body.Vel * dt;
            p.View.SetPosition(p.Body.Pos);
            if (p.Body.Pos.y < -5f && !p.Gone) { p.Gone = true; p.View.Destroy(); }
        }

        void TickLion(Part p, float dt, Level lv, Knight k)
        {
            if (p.Gone) return;
            if (p.Down) { Fall(p, dt); return; }
            p.T += dt; p.Grace -= dt;
            float dx = k.Body.Pos.x - p.Body.Pos.x;
            switch (p.State)
            {
                case 0:   // crouched, gathering himself to spring
                    p.Dir = dx > 0f ? 1f : -1f; p.View.Face(p.Dir > 0f); p.View.SetFrame(1);
                    p.Body.Vel.x = 0f;
                    if (p.T > 1.3f / p.Fury)
                    {
                        p.State = 1; p.T = 0f; Audio.Play("roar", 0.8f);
                        p.Body.Vel = new Vector2(Mathf.Clamp(dx * 1.1f, -9f, 9f) * p.Fury, 13f);
                    }
                    break;
                case 1:   // springing through the air
                    p.View.SetFrame(2);
                    if (p.Body.Grounded && p.T > 0.2f) { p.State = 0; p.T = 0f; p.Body.Vel.x = 0f; Audio.Play("bump", 0.6f); }
                    break;
                case 2:   // stunned by a blow
                    p.View.SetFrame(3); p.Body.Vel.x = 0f;
                    if (p.T > 0.9f) { p.State = 0; p.T = 0f; }
                    break;
            }
            p.Body.Vel.y = Mathf.Max(p.Body.Vel.y - (p.State == 1 ? 30f : 40f) * dt, -16f);
            p.Body.Step(lv, dt);
            if (p.Body.HitWall && p.State == 1) p.Body.Vel.x = -p.Body.Vel.x * 0.3f;
            p.View.Renderer.enabled = p.State != 2 || Mathf.Repeat(p.T, 0.12f) > 0.05f;
            p.View.SetPosition(p.Body.Pos);
        }

        void TickUnicorn(Part p, float dt, Level lv, Knight k)
        {
            if (p.Gone) return;
            if (p.Down) { Fall(p, dt); return; }
            p.T += dt; p.Grace -= dt;
            float dx = k.Body.Pos.x - p.Body.Pos.x;
            switch (p.State)
            {
                case 0:   // rearing - its belly bared
                    p.Dir = dx > 0f ? 1f : -1f; p.View.Face(p.Dir > 0f); p.View.SetFrame(2);
                    p.Body.Vel.x = 0f;
                    if (p.T > 1.1f / p.Fury) { p.State = 1; p.T = 0f; Audio.Play("mount", 0.8f); }
                    break;
                case 1:   // charging
                    p.View.SetFrame(((int)(p.T * 9f)) % 2);
                    p.Body.Vel.x = p.Dir * 8.5f * p.Fury;
                    if (p.Body.HitWall || p.T > 3f) { p.State = 0; p.T = 0f; p.Body.Vel.x = 0f; Audio.Play("bump"); }
                    break;
                case 2:
                    p.View.SetFrame(3); p.Body.Vel.x = 0f;
                    if (p.T > 0.9f) { p.State = 0; p.T = 0f; }
                    break;
            }
            p.Body.Vel.y = Mathf.Max(p.Body.Vel.y - 40f * dt, -16f);
            p.Body.Step(lv, dt);
            p.View.Renderer.enabled = p.State != 2 || Mathf.Repeat(p.T, 0.12f) > 0.05f;
            p.View.SetPosition(p.Body.Pos);
        }

        public override void Contact(Knight k)
        {
            if (!Touchable) return;
            foreach (var p in new[] { lion, unicorn })
            {
                if (p.Down || p.Gone || !k.Body.Overlaps(p.Body)) continue;
                bool onTop = k.Body.Vel.y < 0f && k.Body.PrevBottom >= p.Body.Pos.y + p.Body.H * 0.55f;
                if (k.Blessed) { Strike(p, 1); continue; }
                if (onTop)
                {
                    k.Body.Vel.y = 13f;
                    if (p.Vulnerable) Strike(p, 1); else Audio.Play("bump", 0.6f);
                    continue;
                }
                if (p.State != 2 && p.Grace <= 0f) { k.Hurt(); p.Grace = 1f; }
            }
        }

        public override void OnKnight(Knight k, bool fromAbove) { }   // handled per beast in Contact

        float boltDamage;
        public override bool BoltHits(Body bolt)
        {
            if (!Touchable) return false;
            foreach (var p in new[] { lion, unicorn })
            {
                if (p.Down || p.Gone || !bolt.Overlaps(p.Body)) continue;
                if (p.Vulnerable) { boltDamage += 0.34f; if (boltDamage >= 1f) { boltDamage = 0f; Strike(p, 1); } }   // three bolts to a blow
                return true;
            }
            return false;
        }

        public override void Hit(float amount) { }

        void Strike(Part p, int n)
        {
            if (p.Down || p.State == 2) return;
            p.Hp -= n; Health = Mathf.Max(0f, Health - n);
            p.State = 2; p.T = 0f;
            Audio.Play("stomp"); Audio.Play("roar", 0.7f);
            GameManager.I.AddScore(1000, p.Body.Pos + new Vector2(0f, p.Body.H));
            var other = p == lion ? unicorn : lion;
            if (!other.Down) { other.Fury += 0.25f; GameManager.I.Toast(other.Lion ? "Ye Lyon roareth in wrath!" : "Ye Unicorne stampeth in wrath!"); }
            if (p.Hp <= 0)
            {
                p.Down = true; p.View.Renderer.enabled = true; p.View.Renderer.flipY = true; p.Body.Vel = new Vector2(0f, 8f);
                if (lion.Down && unicorn.Down)
                {
                    Dead = true; Health = 0f;
                    GameManager.I.AddScore(5000, p.Body.Pos + new Vector2(0f, p.Body.H + 1f));
                    GameManager.I.BossDefeated();
                }
                else GameManager.I.Toast((p.Lion ? "Ye Lyon" : "Ye Unicorne") + " is overcome! Now ye other!");
            }
        }
    }
}
