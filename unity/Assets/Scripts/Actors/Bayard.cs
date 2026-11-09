using UnityEngine;

namespace Tapestry
{
    /// <summary>Bayard without a rider: waits to be mounted, or bolts after throwing his knight.</summary>
    public class Bayard
    {
        public readonly Body Body;
        public bool Removed;
        readonly SpriteView view;
        float fleeTime, cooldown, anim, rearTime;
        float dir = 1f;

        public Bayard(Vector2 pos, Transform parent)
        {
            Body = new Body(pos, 1.25f, 1.55f);
            view = new SpriteView("mounts/bayard", parent, 18);
            view.SetPosition(pos);
            cooldown = 0.4f;
        }

        public bool CanMount { get { return cooldown <= 0f && rearTime <= 0f; } }

        public void Hop() { Body.Vel.y = 7f; }

        /// <summary>Thrown rider: rear up, then gallop away from the danger.</summary>
        public void Bolt(bool runRight)
        {
            dir = runRight ? 1f : -1f;
            rearTime = 0.5f; fleeTime = 2.6f; cooldown = 1.0f;
        }

        public void Calm() { cooldown = 0.8f; }

        public void Tick(float dt, Level lv)
        {
            cooldown -= dt; anim += dt;
            if (rearTime > 0f)
            {
                rearTime -= dt; Body.Vel.x = 0f; view.SetFrame(4);
            }
            else if (fleeTime > 0f)
            {
                fleeTime -= dt;
                if (Body.HitWall) dir = -dir;
                if (Body.Grounded && !Body.FloorAhead(lv, dir)) dir = -dir;   // a good horse won't run into a pit
                Body.Vel.x = dir * 7f;
                view.SetFrame(1 + ((int)(anim * 9f) % 2));
            }
            else
            {
                Body.Vel.x = Mathf.MoveTowards(Body.Vel.x, 0f, 20f * dt);
                view.SetFrame(Body.Grounded ? 0 : 3);
            }
            view.Face(dir > 0f);
            Body.Vel.y = Mathf.Max(Body.Vel.y - 40f * dt, -16f);
            Body.Step(lv, dt);
            if (Body.Pos.y < -3f) Destroy();
            view.SetPosition(Body.Pos);
        }

        public void Destroy() { if (Removed) return; Removed = true; view.Destroy(); }
    }

    /// <summary>A spinning gold bezant: a coin waiting to be collected, or one popping out of a roundel.</summary>
    public class Bezant
    {
        public readonly Vector2 Pos;
        public bool Removed;
        readonly SpriteView view;
        readonly bool popping;
        float t;

        public Bezant(Vector2 pos, Transform parent, bool pop)
        {
            Pos = pos; popping = pop;
            view = new SpriteView("scenery/bezant", parent, 12);
            view.SetPosition(pos);
        }

        public void Tick(float dt)
        {
            t += dt;
            view.SetFrame(((int)(t * (popping ? 16f : 7f))) % 4);
            if (popping)
            {
                float k = t / 0.5f;
                view.SetPosition(Pos + new Vector2(0f, Mathf.Sin(Mathf.Min(k, 1f) * Mathf.PI * 0.85f) * 1.8f));
                if (k >= 1f) Destroy();
            }
        }

        public bool Touches(Body b)
        {
            return !popping && Pos.x > b.Left - 0.3f && Pos.x < b.Right + 0.3f && Pos.y + 0.25f > b.Pos.y && Pos.y < b.Top;
        }

        public void Destroy() { if (Removed) return; Removed = true; view.Destroy(); }
    }
}
