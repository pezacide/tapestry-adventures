using UnityEngine;

namespace Tapestry
{
    /// <summary>One of the Winds of the Tapestry of Creation: a puffed-cheeked face that blows steadily across
    /// a stretch of sky (12 tiles), pushing the knight while he is within it. Stitched gusts stream out of its mouth.</summary>
    public class WindHead
    {
        public const float Reach = 12f, Push = 3.2f;
        readonly Vector2 at;
        readonly float dir;
        readonly Transform[] gusts = new Transform[3];
        float t;

        public WindHead(Vector2 pos, bool blowsRight, Transform parent)
        {
            at = pos + new Vector2(0f, 0.5f); dir = blowsRight ? 1f : -1f;
            var head = Art.Make("scenery/wind_head", parent, new Vector2(pos.x, pos.y - 0.5f), 3);
            head.flipX = !blowsRight;
            for (int i = 0; i < gusts.Length; i++)
            {
                var g = Art.Make("scenery/gust", parent, Vector2.zero, 3);
                g.flipX = !blowsRight;
                var c = g.color; c.a = 0.75f; g.color = c;
                gusts[i] = g.transform;
            }
        }

        bool Inside(Body b)
        {
            float x0 = dir > 0f ? at.x : at.x - Reach, x1 = dir > 0f ? at.x + Reach : at.x;
            return b.Right > x0 && b.Left < x1 && b.Top > at.y - 4f && b.Pos.y < at.y + 4f;
        }

        public void Tick(float dt, Level lv, Knight k)
        {
            t += dt;
            for (int i = 0; i < gusts.Length; i++)   // gusts stream out across the windy stretch
            {
                float along = Mathf.Repeat(t * 6f + i * Reach / gusts.Length, Reach);
                gusts[i].localPosition = new Vector3(at.x + dir * (1.2f + along), at.y - 0.6f + (i - 1) * 1.1f, 0f);
            }
            if (!k.Dead && Inside(k.Body)) k.Body.Nudge(lv, dir * Push * dt);
        }
    }
}
