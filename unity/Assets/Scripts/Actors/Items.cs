using UnityEngine;

namespace Tapestry
{
    /// <summary>A power-up rising out of a brick, Mario-fashion:
    /// chalice = 1-up (slides along), capon = mushroom (slides along), crossbow = fire flower (stays put),
    /// relic = star (bounds about).</summary>
    public class Item
    {
        public readonly string Kind;
        public readonly Body Body;
        public bool Removed;
        public bool Ready { get { return emerge >= 1f; } }

        readonly SpriteView view;
        readonly Vector2 top;
        float emerge, dir;

        public Item(string kind, Vector2 blockTop, Transform parent, bool goRight)
        {
            Kind = kind;
            top = blockTop;
            dir = goRight ? 1f : -1f;
            var sheet = Art.Sheet("items/" + kind);
            Body = new Body(blockTop - new Vector2(0f, 0.6f), (sheet.MaxX - sheet.MinX) / Art.DesignPerTile * 0.9f, (sheet.MaxY - sheet.MinY) / Art.DesignPerTile * 0.9f);
            view = new SpriteView("items/" + kind, parent, -1);   // starts behind the brick and rises out of it
            view.SetPosition(Body.Pos);
        }

        public void Tick(float dt, Level lv)
        {
            if (Removed) return;
            view.SetFrame(((int)(Time.time * 4f)) % 2);
            if (emerge < 1f)
            {
                emerge = Mathf.Min(1f, emerge + dt / 0.6f);
                Body.Pos = top - new Vector2(0f, 0.6f * (1f - emerge));
                if (emerge >= 1f) view.Renderer.sortingOrder = 12;
                view.SetPosition(Body.Pos);
                return;
            }
            switch (Kind)
            {
                case "crossbow": break;   // waits on top of its block
                case "relic":
                    if (Body.HitWall) dir = -dir;
                    Body.Vel.x = dir * 3.5f;
                    if (Body.Grounded) Body.Vel.y = 9f;
                    break;
                default:              // chalice and capon slide along and drop off ledges
                    if (Body.HitWall) dir = -dir;
                    Body.Vel.x = dir * 2.6f;
                    break;
            }
            if (Kind != "crossbow")
            {
                Body.Vel.y = Mathf.Max(Body.Vel.y - 30f * dt, -14f);
                Body.Step(lv, dt);
            }
            if (Body.Pos.y < -3f) { Destroy(); return; }
            view.SetPosition(Body.Pos);
        }

        public void Destroy() { if (Removed) return; Removed = true; view.Destroy(); }
    }
}
