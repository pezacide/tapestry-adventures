using UnityEngine;

namespace Tapestry
{
    /// <summary>A log tumbling from the shipwrights' pile. Jump it or bounce off its top - it can't be stomped flat,
    /// and it hurts if it rolls into you. It rolls on until it drops into a pit or the sea.</summary>
    public class Log
    {
        public readonly Body Body;
        public bool Removed;
        readonly Transform spin;
        float dir, angle;

        public Log(Vector2 pos, float dir, Transform parent)
        {
            this.dir = dir;
            Body = new Body(pos, 0.85f, 0.85f) { UsePlatforms = true };
            spin = new GameObject("log").transform;
            spin.SetParent(parent, false);
            Art.Make("scenery/log", spin, new Vector2(0f, -0.5f), 9);   // centred on the pivot so it can spin
            Place();
        }

        public void Tick(float dt, Level lv)
        {
            if (Removed) return;
            if (Body.HitWall) dir = -dir;
            Body.Vel.x = dir * 3.4f;
            Body.Vel.y = Mathf.Max(Body.Vel.y - 40f * dt, -16f);
            Body.Step(lv, dt);
            angle -= dir * 3.4f * dt / 0.45f * Mathf.Rad2Deg;
            if (Body.Pos.y < -2f || lv.Water(Mathf.FloorToInt(Body.Pos.x), Mathf.FloorToInt(Body.Pos.y))) { Destroy(); return; }
            Place();
        }

        void Place()
        {
            spin.localPosition = new Vector3(Body.Pos.x, Body.Pos.y + 0.5f, 0f);
            spin.localEulerAngles = new Vector3(0f, 0f, angle);
        }

        public void Destroy() { if (Removed) return; Removed = true; Object.Destroy(spin.gameObject); }
    }
}
