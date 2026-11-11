using UnityEngine;

namespace Tapestry
{
    /// <summary>The Cat King, last of all foes (World 7-5). He stands upon his throne flinging soup with his ladle-sceptre,
    /// and summons Soup Cats to his aid. Then he leaps down and prowls after the knight - stomp him then! (On his throne he
    /// cannot be harmed.) After a blow he is dazed, then bounds back up to his throne. Four blows, and Princess Aelfgyva is free.</summary>
    public class CatKing : BossBase
    {
        public override float MaxHealth { get { return 4f; } }
        public override string DefeatedText { get { return "Ye Cat King is overthrowne! Go to ye Princesse!"; } }

        enum State { Throne, LeapDown, Prowl, Dazed, LeapUp }
        State state = State.Throne;
        readonly SpriteView view;
        readonly Vector2 seat;
        Vector2 arcFrom, arcTo;
        float t, dir = -1f, hurtGrace, fury = 1f;
        int flings;
        bool toldHow;

        public CatKing(Vector2 pos, Transform parent)
        {
            Title = "The Cat King";
            Health = MaxHealth;
            Art.Make("scenery/cat_throne", parent, pos, 8);
            seat = pos + new Vector2(0f, 0.75f);
            Body = new Body(seat, 1.3f, 2.2f);
            ArenaMin = pos.x - 17f; ArenaMax = pos.x + 3f;
            view = new SpriteView("bosses/cat_king_afoot", parent, 11);
            view.Face(false);
            view.SetPosition(Body.Pos);
        }

        public override void Tick(float dt, Level lv, Knight k)
        {
            if (Removed) return;
            t += dt; hurtGrace -= dt;
            if (Dead)
            {
                Body.Vel.y -= 30f * dt; Body.Pos += Body.Vel * dt; view.SetPosition(Body.Pos);
                if (Body.Pos.y < -5f) { Removed = true; view.Destroy(); }
                return;
            }
            if (!toldHow) { toldHow = true; GameManager.I.Toast("On his throne he may not be harmed. When he leapeth downe, stompe him!"); }
            float dx = k.Body.Pos.x - Body.Pos.x;
            switch (state)
            {
                case State.Throne:
                    view.Face(dx > 0f);
                    view.SetFrame(t > 0.7f / fury && t < 1f / fury ? 1 : 0);
                    if (t > 1f / fury)
                    {
                        t = 0f; flings++;
                        GameManager.I.SpawnShot("soup_glob", Body.Pos + new Vector2(-0.6f, 2f), new Vector2(Mathf.Clamp(dx * 0.6f, -8f, -2f), 9f));
                        Audio.Play("soup", 0.7f);
                        if (flings % 3 == 0)
                        {
                            if (GameManager.I.CountEnemies("soup_cat") < 2)
                            {
                                GameManager.I.SpawnEnemy("soup_cat", new Vector2(seat.x - 7f - Random.value * 4f, 2f));
                                Audio.Play("meow"); GameManager.I.Toast("Ye Cat King summoneth a Soupe Catte!");
                            }
                            Leap(State.LeapDown, new Vector2(Mathf.Clamp(k.Body.Pos.x + 2.5f, ArenaMin + 2f, seat.x - 3f), 2f));
                        }
                    }
                    break;
                case State.LeapDown:
                case State.LeapUp:
                    {
                        float u = Mathf.Clamp01(t / 0.8f);
                        Body.Pos = Vector2.Lerp(arcFrom, arcTo, u) + new Vector2(0f, Mathf.Sin(u * Mathf.PI) * 3.5f);
                        view.SetFrame(2);
                        if (u >= 1f)
                        {
                            if (state == State.LeapDown) { state = State.Prowl; t = 0f; dir = dx > 0f ? 1f : -1f; Audio.Play("bump"); }
                            else { state = State.Throne; t = 0f; }
                        }
                    }
                    break;
                case State.Prowl:   // after the knight - this is the moment to stomp him
                    if (Body.HitWall) dir = -dir;
                    if (t > 0.6f && Mathf.Sign(dx) != dir && Mathf.Abs(dx) > 1.5f) dir = Mathf.Sign(dx);
                    Body.Vel.x = dir * 4.6f * fury;
                    view.Face(dir > 0f); view.SetFrame(2 + ((int)(t * 8f)) % 2);
                    Body.Vel.y = Mathf.Max(Body.Vel.y - 40f * dt, -16f);
                    Body.Step(lv, dt);
                    if (t > 3.2f) Leap(State.LeapUp, seat);
                    break;
                case State.Dazed:
                    view.SetFrame(4); Body.Vel.x = 0f;
                    Body.Vel.y = Mathf.Max(Body.Vel.y - 40f * dt, -16f);
                    Body.Step(lv, dt);
                    if (t > 1.1f) Leap(State.LeapUp, seat);
                    break;
            }
            view.Renderer.enabled = state != State.Dazed || Mathf.Repeat(t, 0.12f) > 0.05f;
            view.SetPosition(Body.Pos);
        }

        void Leap(State s, Vector2 to)
        {
            state = s; t = 0f; arcFrom = Body.Pos; arcTo = to; Body.Vel = Vector2.zero;
            Audio.Play("jump", 0.6f);
        }

        public override void OnKnight(Knight k, bool fromAbove)
        {
            if (Dead || k.Dead) return;
            bool open = state == State.Prowl;
            if (k.Blessed && open) { Hit(1f); return; }
            if (fromAbove)
            {
                k.Body.Vel.y = 13f;
                if (open) Hit(1f); else Audio.Play("bump", 0.6f);
                return;
            }
            if (state != State.Dazed && hurtGrace <= 0f && !k.Blessed) { k.Hurt(); hurtGrace = 1f; }
        }

        float boltDamage;
        public override bool BoltHits(Body bolt)
        {
            if (!Touchable || !bolt.Overlaps(Body)) return false;
            if (state == State.Prowl) { boltDamage += 0.34f; if (boltDamage >= 1f) { boltDamage = 0f; Hit(1f); } }
            return true;
        }

        public override void Hit(float amount)
        {
            if (Dead || state != State.Prowl) return;
            Health -= amount; fury += 0.2f;
            Audio.Play("stomp"); Audio.Play("meow");
            GameManager.I.AddScore(1000, Body.Pos + new Vector2(0f, Body.H));
            if (Health <= 0.01f)
            {
                Health = 0f; Dead = true;
                view.Renderer.enabled = true; view.Renderer.flipY = true;
                Body.Vel = new Vector2(0f, 9f);
                GameManager.I.AddScore(10000, Body.Pos + new Vector2(0f, Body.H + 1f));
                GameManager.I.BossDefeated();
                return;
            }
            state = State.Dazed; t = 0f;
        }
    }
}
