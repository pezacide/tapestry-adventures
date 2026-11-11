using UnityEngine;

namespace Tapestry
{
    /// <summary>What every boss shares: a body to touch, health, and being woken, struck and defeated.</summary>
    public abstract class BossBase
    {
        public Body Body;
        public float Health;
        public abstract float MaxHealth { get; }
        public bool Active, Dead, Removed;
        public string Title;
        public float ArenaMin, ArenaMax;   // while the boss lives, the screen and the knight are held within these
        public virtual bool Touchable { get { return Active && !Dead; } }   // false while out of reach (e.g. under the sea)
        public abstract void Tick(float dt, Level lv, Knight player);
        public abstract void OnKnight(Knight k, bool fromAbove);
        public abstract void Hit(float amount);

        /// <summary>The knight may be touching the boss: work out whether it was a stomp, and react.</summary>
        public virtual void Contact(Knight k)
        {
            if (!Touchable || !k.Body.Overlaps(Body)) return;
            bool onTop = k.Body.Vel.y < 0f && k.Body.PrevBottom >= Body.Pos.y + Body.H * 0.6f;
            OnKnight(k, onTop);
        }

        /// <summary>A crossbowe bolt may strike the boss (three bolts to a stomp). Returns true if it did.</summary>
        public virtual bool BoltHits(Body bolt)
        {
            if (!Touchable || !bolt.Overlaps(Body)) return false;
            Hit(0.34f);
            return true;
        }
    }

    /// <summary>The Cockatrice, lord of World 1. Paces its drawbridge, hops, and glares a petrifying gaze.
    /// Three stomps on its crowned head (or bolts: three to a stomp) fell it - or strike the axe and drop the bridge.</summary>
    public class Boss : BossBase
    {
        public override float MaxHealth { get { return 3f; } }

        readonly SpriteView view;
        readonly float minX, maxX;
        float dir = -1f, timer, stun, attackT = -1f;
        readonly System.Random rng = new System.Random(4);

        public Boss(Vector2 pos, Transform parent)
        {
            Body = new Body(pos, 1.9f, 2.5f);
            Health = MaxHealth; Title = "Ye Cockatrice";
            ArenaMin = pos.x - 12f; ArenaMax = pos.x + 9f;   // the bridge and the axe beyond it
            view = new SpriteView("bosses/cockatrice", parent, 11);
            view.Face(false);
            minX = pos.x - 5f; maxX = pos.x + 3f;
            view.SetPosition(pos);
        }

        public override void Tick(float dt, Level lv, Knight player)
        {
            if (Removed) return;
            if (Dead)
            {
                Body.Vel.y -= 30f * dt;
                Body.Pos += Body.Vel * dt;
                view.SetPosition(Body.Pos);
                if (Body.Pos.y < -5f) { Removed = true; view.Destroy(); }
                return;
            }
            timer += dt; stun -= dt;
            float dx = player.Body.Pos.x - Body.Pos.x;
            view.Renderer.enabled = stun <= 0f || Mathf.Repeat(stun, 0.14f) > 0.07f;

            if (attackT >= 0f)
            {
                // the glare: rear up, then loose the petrifying gaze toward the knight
                attackT += dt;
                Body.Vel.x = 0f;
                view.Face(dx > 0f);
                view.SetFrame(1);
                if (attackT > 0.35f && attackT - dt <= 0.35f)
                {
                    float d = dx > 0f ? 1f : -1f;
                    GameManager.I.SpawnShot("gaze", Body.Pos + new Vector2(d * Body.W * 0.7f, Body.H * 0.85f), new Vector2(d * 7.5f, 0f));
                    Audio.Play("roar");
                }
                if (attackT > 0.7f) { attackT = -1f; timer = 0f; }
            }
            else
            {
                view.SetFrame(0);
                if (Body.HitWall || Body.Pos.x < minX || Body.Pos.x > maxX) dir = Body.Pos.x < minX ? 1f : Body.Pos.x > maxX ? -1f : -dir;
                Body.Vel.x = dir * (Health < 2f ? 2.4f : 1.6f);   // quicker when wounded
                view.Face(dir > 0f);
                if (Body.Grounded && rng.NextDouble() < dt * 0.35) Body.Vel.y = 10f;
                if (timer > (Health < 2f ? 1.8f : 2.6f) && Mathf.Abs(dx) < 12f) attackT = 0f;
            }
            Body.Vel.y = Mathf.Max(Body.Vel.y - 32f * dt, -16f);
            Body.Step(lv, dt);
            if (Body.Pos.y < -1f) Defeat();   // the bridge went from under it
            view.SetPosition(Body.Pos);
        }

        public override void OnKnight(Knight k, bool fromAbove)
        {
            if (Dead || k.Dead) return;
            if (k.Blessed) { Hit(1f); return; }
            if (fromAbove)
            {
                k.Body.Vel.y = 14f;   // a big bounce off the crown
                if (stun <= 0f) Hit(1f);
                return;
            }
            if (stun <= 0.6f) k.Hurt();
        }

        public override void Hit(float amount)
        {
            if (Dead || stun > 0f) return;
            Health -= amount;
            stun = amount >= 1f ? 1.1f : 0.35f;
            Audio.Play("stomp"); Audio.Play("roar", 0.8f);
            GameManager.I.AddScore(amount >= 1f ? 1000 : 300, Body.Pos + new Vector2(0f, Body.H));
            if (Health <= 0.01f) Defeat();
        }

        void Defeat()
        {
            if (Dead) return;
            Dead = true; Health = 0f;
            view.Renderer.enabled = true;
            view.Renderer.flipY = true;
            Body.Vel = new Vector2(0f, 8f);
            GameManager.I.AddScore(5000, Body.Pos + new Vector2(0f, Body.H));
            GameManager.I.BossDefeated();
        }
    }
}
