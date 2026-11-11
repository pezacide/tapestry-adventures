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

        public Lift(Vector2 tileLeftTop, Kind kind, Transform parent, float phase, string boardSkin = "scenery/lift")
        {
            home = tileLeftTop; t = phase; Pos = home;
            string key;
            switch (kind)
            {
                case Kind.BoardUpDown: key = boardSkin; Width = 3f; ampY = 2.5f; speed = 0.9f; spriteH = 22f; deckFromTop = 8f; break;
                case Kind.ShipRocking: key = "scenery/longship"; Width = 5.4f; ampY = 0.35f; speed = 1.6f; rock = 3f; spriteH = 110f; deckFromTop = 58f; break;
                case Kind.ShipSailing: key = "scenery/longship"; Width = 5.4f; ampX = 3f; ampY = 0.25f; speed = 0.7f; rock = 2f; spriteH = 110f; deckFromTop = 58f; break;
                default: key = boardSkin; Width = 3f; ampX = 3f; speed = 0.9f; spriteH = 22f; deckFromTop = 8f; break;
            }
            view = Art.Make(key, parent, Vector2.zero, kind == Kind.ShipRocking || kind == Kind.ShipSailing ? -2 : 2);
            Place();
        }

        // a board riding round the rim of a turning wheel (it stays level, like a fair's big wheel)
        bool onWheel; Vector2 centre; float radius;

        public static Lift OnWheel(Vector2 centre, float radius, float phase, float speed, Transform parent, string skin)
        {
            var l = new Lift(centre, Kind.BoardSideways, parent, phase, skin);
            l.onWheel = true; l.centre = centre; l.radius = radius;
            l.SetSpeed(speed);
            l.Tick(0f);
            return l;
        }

        float wheelSpeed;
        void SetSpeed(float s) { wheelSpeed = s; }

        // a raft drifting downstream on the current, from where it was launched to the end of its run
        bool drifting; float driftEnd;
        public bool Gone;   // drifted past the end of its run and sank
        public static Lift Raft(Vector2 tileLeftTop, float run, Transform parent)
        {
            var l = new Lift(tileLeftTop, Kind.BoardSideways, parent, 0f, "scenery/raft");
            l.drifting = true; l.driftEnd = tileLeftTop.x + run;
            l.Tick(0f);
            return l;
        }

        public void Tick(float dt)
        {
            var before = Pos;
            if (drifting)
            {
                t += dt;
                Pos = new Vector2(Pos.x + 1.4f * dt, home.y + Mathf.Sin(t * 2.2f) * 0.12f);
                if (Pos.x > driftEnd) { Gone = true; view.enabled = false; }
                Delta = Pos - before;
                Place();
                return;
            }
            if (onWheel)
            {
                t += dt * wheelSpeed;
                Pos = centre + new Vector2(Mathf.Cos(t), Mathf.Sin(t)) * radius - new Vector2(Width * 0.5f, 0f);
                Delta = Pos - before;
                Place();
                return;
            }
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
