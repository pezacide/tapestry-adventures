using UnityEngine;

namespace Tapestry
{
    /// <summary>Something you stand on that moves and carries you, and that you can jump up through:
    /// the hanging boards of the undercroft (L sideways, E up and down) and the longships of the crossing
    /// (Y rocking at anchor, U sailing to and fro).</summary>
    public class Lift
    {
        public float Width;
        public Vector2 Pos;          // left end of the top surface
        public Vector2 Delta;        // how far it moved this frame
        public float X0 { get { return Pos.x; } }
        public float X1 { get { return Pos.x + Width; } }
        public float Top { get { return Pos.y; } }

        readonly Vector2 home;
        readonly float ampX, ampY, speed, rock, spriteH, deckFromTop;
        readonly SpriteRenderer view;
        float t;

        public enum Kind { BoardSideways, BoardUpDown, ShipRocking, ShipSailing }

        public Lift(Vector2 tileLeftTop, Kind kind, Transform parent, float phase)
        {
            home = tileLeftTop; t = phase; Pos = home;
            string key;
            switch (kind)
            {
                case Kind.BoardUpDown: key = "scenery/lift"; Width = 3f; ampY = 2.5f; speed = 0.9f; spriteH = 22f; deckFromTop = 8f; break;
                case Kind.ShipRocking: key = "scenery/longship"; Width = 5.4f; ampY = 0.35f; speed = 1.6f; rock = 3f; spriteH = 110f; deckFromTop = 58f; break;
                case Kind.ShipSailing: key = "scenery/longship"; Width = 5.4f; ampX = 3f; ampY = 0.25f; speed = 0.7f; rock = 2f; spriteH = 110f; deckFromTop = 58f; break;
                default: key = "scenery/lift"; Width = 3f; ampX = 3f; speed = 0.9f; spriteH = 22f; deckFromTop = 8f; break;
            }
            view = Art.Make(key, parent, Vector2.zero, kind == Kind.ShipRocking || kind == Kind.ShipSailing ? -2 : 2);
            Place();
        }

        public void Tick(float dt)
        {
            var before = Pos;
            t += dt * speed;
            Pos = home + new Vector2(Mathf.Sin(t) * ampX, Mathf.Sin(t * (ampX > 0f ? 2f : 1f)) * ampY);
            Delta = Pos - before;
            Place();
        }

        void Place()
        {
            // pivot is bottom-centre of the drawing; its deck/board top is deckFromTop design units below the drawing's top
            float below = (spriteH - deckFromTop) / Art.DesignPerTile;
            view.transform.localPosition = new Vector3(Pos.x + Width * 0.5f, Pos.y - below, 0f);
            if (rock != 0f) view.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Sin(t * 1.3f) * rock);   // a gentle roll (looks only)
        }
    }
}
