using UnityEngine;

namespace Tapestry
{
    /// <summary>A hanging board, three tiles wide, that glides to and fro (L) or up and down (E).
    /// You stand on its top and can jump up through it, like a plank; it carries whoever rides it.</summary>
    public class Lift
    {
        public const float Width = 3f;
        public Vector2 Pos;          // left end of the top surface
        public Vector2 Delta;        // how far it moved this frame
        public float X0 { get { return Pos.x; } }
        public float X1 { get { return Pos.x + Width; } }
        public float Top { get { return Pos.y; } }

        readonly Vector2 home;
        readonly bool vertical;
        readonly SpriteRenderer view;
        float t;

        public Lift(Vector2 tileLeftTop, bool vertical, Transform parent, float phase)
        {
            home = tileLeftTop; this.vertical = vertical; t = phase;
            Pos = home;
            view = Art.Make("scenery/lift", parent, Vector2.zero, 2);
            Place();
        }

        public void Tick(float dt)
        {
            var before = Pos;
            t += dt * 0.9f;
            float s = Mathf.Sin(t);
            Pos = vertical ? home + new Vector2(0f, s * 2.5f) : home + new Vector2(s * 3f, 0f);
            Delta = Pos - before;
            Place();
        }

        void Place()
        {
            // the sprite's pivot is bottom-centre; its top board edge sits 8 design units below the rope tops
            float h = 22f / Art.DesignPerTile, boardTop = 8f / Art.DesignPerTile;
            view.transform.localPosition = new Vector3(Pos.x + Width * 0.5f, Pos.y - (h - boardTop), 0f);
        }
    }
}
