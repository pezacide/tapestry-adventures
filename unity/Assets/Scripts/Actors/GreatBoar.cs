using UnityEngine;

namespace Tapestry
{
    /// <summary>Ye Greate Boar, lord of World 4: the quarry of the hunt grown enormous.
    /// It paws the ground (a warning), then charges across the lodge. If it rams the wall it is dazed for a moment -
    /// stomp it then (or shoot it: three bolts to a stomp). Stomping it while it is not dazed only bounces you off.
    /// Three blows and it falls.</summary>
    public class GreatBoar : BossBase
    {
        public override float MaxHealth { get { return 3f; } }
        enum State { Paw, Charge, Dazed }
        State state = State.Paw;
        readonly SpriteView view;
        float dir = -1f, t, hurtGrace;

        public GreatBoar(Vector2 pos, Transform parent)
        {
            Title = "Ye Greate Boar";
            Health = MaxHealth;
            Body = new Body(pos, 3.8f, 2.1f);
            ArenaMin = pos.x - 11f; ArenaMax = pos.x + 11f;
            view = new SpriteView("bosses/great_boar", parent, 11);
            view.Face(false);
            view.SetPosition(pos);
        }

        public override void Tick(float dt, Level lv, Knight player)
        {
            if (Removed) return;
            t += dt; hurtGrace -= dt;
            if (Dead)
            {
                Body.Vel.y -= 30f * dt; Body.Pos += Body.Vel * dt;
                view.SetPosition(Body.Pos);
                if (Body.Pos.y < -5f) { Removed = true; view.Destroy(); }
                return;
            }
            float fury = Health < 2f ? 1.35f : 1f;   // angrier when wounded
            switch (state)
            {
                case State.Paw:
                    dir = player.Body.Pos.x > Body.Pos.x ? 1f : -1f;
                    view.Face(dir > 0f);
                    view.SetFrame(1);
                    Body.Vel.x = 0f;
                    if (t > 0.9f / fury) { state = State.Charge; t = 0f; Audio.Play("roar"); }
                    break;
                case State.Charge:
                    view.SetFrame(((int)(t * 10f)) % 2 == 0 ? 0 : 1);
                    Body.Vel.x = dir * 8.5f * fury;
                    if (Body.HitWall)
                    {
                        state = State.Dazed; t = 0f; Body.Vel.x = 0f;
                        Audio.Play("brick"); Audio.Play("bump");
                        GameManager.I.Toast("It is dazed! Stomp it now!");
                    }
                    if (t > 4f) { state = State.Paw; t = 0f; }
                    break;
                case State.Dazed:
                    view.SetFrame(2);
                    Body.Vel.x = 0f;
                    if (t > 2.2f) { state = State.Paw; t = 0f; }
                    break;
            }
            Body.Vel.y = Mathf.Max(Body.Vel.y - 40f * dt, -16f);
            Body.Step(lv, dt);
            view.Renderer.enabled = !(state == State.Dazed && t < 0.3f) || Mathf.Repeat(t, 0.1f) > 0.05f;
            view.SetPosition(Body.Pos);
        }

        public override void OnKnight(Knight k, bool fromAbove)
        {
            if (Dead || k.Dead) return;
            if (k.Blessed) { Hit(1f); return; }
            if (fromAbove)
            {
                k.Body.Vel.y = 13f;
                if (state == State.Dazed) Hit(1f);   // only a dazed boar can be wounded
                return;
            }
            if (hurtGrace <= 0f) { k.Hurt(); hurtGrace = 1f; }
        }

        public override bool BoltHits(Body bolt)
        {
            if (!Touchable || !bolt.Overlaps(Body)) return false;
            if (state == State.Dazed) Hit(0.34f);
            return true;
        }

        public override void Hit(float amount)
        {
            if (Dead) return;
            Health -= amount;
            Audio.Play("stomp"); Audio.Play("roar", 0.8f);
            GameManager.I.AddScore(amount >= 1f ? 1000 : 300, Body.Pos + new Vector2(0f, Body.H));
            if (amount >= 1f) { state = State.Paw; t = -0.4f; }   // it shakes itself, then paws again
            if (Health <= 0.01f)
            {
                Health = 0f; Dead = true;
                view.Renderer.enabled = true; view.Renderer.flipY = true;
                Body.Vel = new Vector2(0f, 8f);
                GameManager.I.AddScore(5000, Body.Pos + new Vector2(0f, Body.H + 1f));
                GameManager.I.BossDefeated();
            }
        }
    }
}
