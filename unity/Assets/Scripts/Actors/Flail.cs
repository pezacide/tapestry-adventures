using UnityEngine;

namespace Tapestry
{
    /// <summary>An iron flail on a chain, swinging round its block (the castle's answer to Mario's fire bar).</summary>
    public class Flail
    {
        readonly Vector2 centre;
        readonly float speed;
        readonly Transform[] parts;
        readonly float[] dist;
        float angle;

        public Flail(Vector2 blockCentre, Transform parent, bool clockwise, float startAngle)
        {
            centre = blockCentre;
            speed = clockwise ? -1.7f : 1.7f;
            angle = startAngle;
            dist = new[] { 0.45f, 0.8f, 1.15f, 1.5f, 1.95f };
            parts = new Transform[dist.Length];
            for (int i = 0; i < dist.Length; i++)
            {
                bool ball = i == dist.Length - 1;
                var key = ball ? "scenery/flail_ball" : "scenery/chain_link";
                float h = (ball ? 24f : 12f) / Art.DesignPerTile;
                var holder = new GameObject("flail").transform;
                holder.SetParent(parent, false);
                Art.Make(key, holder, new Vector2(0f, -h * 0.5f), 7);   // sprite centred on the holder
                parts[i] = holder;
            }
            Place();
        }

        public void Tick(float dt) { angle += speed * dt; Place(); }

        void Place()
        {
            var d = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            for (int i = 0; i < parts.Length; i++)
            {
                var p = centre + d * dist[i];
                parts[i].localPosition = new Vector3(p.x, p.y, 0f);
            }
        }

        /// <summary>Does the swinging iron touch this body?</summary>
        public bool Hits(Body b)
        {
            var d = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            for (int i = 0; i < dist.Length; i++)
            {
                var p = centre + d * dist[i];
                float r = i == dist.Length - 1 ? 0.3f : 0.12f;
                if (p.x + r > b.Left && p.x - r < b.Right && p.y + r > b.Pos.y && p.y - r < b.Top) return true;
            }
            return false;
        }
    }
}
