using UnityEngine;

namespace Tapestry
{
    /// <summary>Aspidochelone, the Island-Whale, lord of World 2. It lurks below, surfaces beside the knight's ship,
    /// spouts (the spout hurls a knight skyward), snaps its jaws, then dives again.
    /// Stomp its back while it is up - or shoot it (three bolts to a stomp). Three blows and it sinks for good.</summary>
    public class Whale : BossBase
    {
        public override float MaxHealth { get { return 3f; } }
        public override bool Touchable { get { return Active && !Dead && state == State.Up; } }

        enum State { Hidden, Rising, Up, Sinking }
        State state = State.Hidden;
        readonly SpriteView view;
        readonly SpriteRenderer spoutView;
        readonly float minX, maxX;
        float x, sy, t, spoutT = -1f;
        const float UpY = 0.55f, DownY = -3.4f;
        readonly System.Random rng = new System.Random(7);

        public Whale(Vector2 pos, Transform parent)
        {
            Title = "Aspidochelone, ye Island-Whale";
            Health = MaxHealth;
            x = pos.x; sy = DownY;
            minX = pos.x - 13f; maxX = pos.x + 13f;
            Body = new Body(new Vector2(x, -20f), 4.2f, 1.35f);
            view = new SpriteView("bosses/whale_boss", parent, 5);
            spoutView = Art.Make("scenery/spout", parent, Vector2.zero, 4);
            spoutView.enabled = false;
            Place();
        }

        public override void Tick(float dt, Level lv, Knight player)
        {
            if (Removed) return;
            t += dt;
            switch (state)
            {
                case State.Hidden:
                    if (t > (Health < 2f ? 0.8f : 1.3f) && !Dead)
                    {
                        // surface a few strides from the knight, on one side or the other
                        float side = rng.NextDouble() < 0.5 ? -1f : 1f;
                        x = Mathf.Clamp(player.Body.Pos.x + side * (3.5f + (float)rng.NextDouble() * 1.5f), minX, maxX);
                        view.Face(player.Body.Pos.x > x);
                        state = State.Rising; t = 0f;
                        Audio.Play("soup", 0.8f);
                    }
                    break;
                case State.Rising:
                    sy = Mathf.Lerp(DownY, UpY, Mathf.SmoothStep(0f, 1f, t / 0.9f));
                    if (t >= 0.9f) { state = State.Up; t = 0f; sy = UpY; }
                    break;
                case State.Up:
                    if (t > 0.3f && spoutT < 0f) { spoutT = 0f; Audio.Play("fireball", 0.7f); }
                    view.SetFrame(t > 1.6f && t < 2.3f ? 1 : 0);   // the jaws open and snap
                    if (t > 1.6f && t - dt <= 1.6f) Audio.Play("roar");
                    if (t > 2.7f) { state = State.Sinking; t = 0f; }
                    break;
                case State.Sinking:
                    sy = Mathf.Lerp(UpY, DownY, Mathf.SmoothStep(0f, 1f, t / 0.8f));
                    if (t >= 0.8f)
                    {
                        sy = DownY; state = State.Hidden; t = 0f;
                        if (Dead) { Removed = true; view.Destroy(); spoutView.enabled = false; return; }
                    }
                    break;
            }
            TickSpout(dt, player);
            Place();
        }

        void TickSpout(float dt, Knight player)
        {
            if (spoutT < 0f) { spoutView.enabled = false; return; }
            spoutT += dt;
            float grow = Mathf.Clamp01(spoutT / 0.3f);
            float h = 192f / Art.DesignPerTile;
            float bx = x + (view.FacingRight ? -0.6f : 0.6f), by = sy + 2.5f;
            spoutView.enabled = true;
            spoutView.transform.localPosition = new Vector3(bx, by, 0f);
            spoutView.transform.localScale = new Vector3(1f, grow, 1f);
            // a knight caught in the column is flung upward
            var pb = player.Body;
            if (grow > 0.5f && pb.Right > bx - 0.4f && pb.Left < bx + 0.4f && pb.Pos.y < by + h * grow && pb.Top > by)
                pb.Vel.y = Mathf.Max(pb.Vel.y, 13f);
            if (spoutT > 1.5f) spoutT = -1f;
        }

        void Place()
        {
            view.SetPosition(new Vector2(x, sy));
            // the stompable back, or out of reach while below
            Body.Pos = state == State.Up ? new Vector2(x + (view.FacingRight ? -0.2f : 0.2f), sy + 1.3f) : new Vector2(x, -20f);
            view.Renderer.enabled = !(stun > 0f) || Mathf.Repeat(stun, 0.14f) > 0.07f;
            stun -= Time.deltaTime;
        }

        float stun;

        public override void OnKnight(Knight k, bool fromAbove)
        {
            if (!Touchable || k.Dead) return;
            if (k.Blessed) { Hit(1f); return; }
            if (fromAbove) { k.Body.Vel.y = 14f; Hit(1f); return; }
            k.Hurt();   // its scaly side and snapping jaws
        }

        public override void Hit(float amount)
        {
            if (!Touchable || stun > 0f) return;
            Health -= amount;
            stun = 1f;
            Audio.Play("stomp"); Audio.Play("roar", 0.9f);
            GameManager.I.AddScore(amount >= 1f ? 1000 : 300, new Vector2(x, sy + 3f));
            if (Health <= 0.01f)
            {
                Health = 0f; Dead = true;
                GameManager.I.AddScore(5000, new Vector2(x, sy + 3.5f));
                GameManager.I.BossDefeated();
            }
            state = State.Sinking; t = 0f;   // struck: it dives at once
        }
    }
}
