using UnityEngine;

namespace Tapestry
{
    /// <summary>Ye Spotted Unicorne, the quarry of World 5, frantic inside its fence.
    /// It cannot be harmed: stomping it only bounces you off. It rears (a warning), then charges across the enclosure.
    /// Leap aside so that it rams the fence GATE (the right-hand side): each ram splinters the gate, and the third breaks it.
    /// Then the unicorn is calmed and spared - and it stays, to be ridden as a second mount.
    /// Its "health" bar is the strength of the gate.</summary>
    public class SpottedUnicorn : BossBase
    {
        public override float MaxHealth { get { return 3f; } }
        enum State { Rear, Charge, Dazed, Calm }
        State state = State.Rear;
        readonly SpriteView view;
        float dir = -1f, t, hurtGrace;
        bool toldHow, toldWrongWall;

        public SpottedUnicorn(Vector2 pos, Transform parent)
        {
            Title = "Ye Spotted Unicorne";
            Health = MaxHealth;
            Body = new Body(pos, 3.0f, 2.6f);
            ArenaMin = pos.x - 11f; ArenaMax = pos.x + 11f;
            view = new SpriteView("bosses/spotted_unicorn", parent, 11);
            view.Face(false);
            view.SetPosition(pos);
        }

        public override string DefeatedText { get { return "Ye unicorne is calmed! It is spared, and shall bear thee."; } }

        public override void Tick(float dt, Level lv, Knight player)
        {
            if (Removed) return;
            t += dt; hurtGrace -= dt;
            if (!toldHow) { toldHow = true; GameManager.I.Toast("It may not be harmed! Leape aside, that it breake ye gate."); }
            float fury = Health < 2f ? 1.25f : 1f;
            switch (state)
            {
                case State.Rear:
                    dir = player.Body.Pos.x > Body.Pos.x ? 1f : -1f;
                    view.Face(dir > 0f);
                    view.SetFrame(2);
                    Body.Vel.x = 0f;
                    if (t > 1.0f / fury) { state = State.Charge; t = 0f; Audio.Play("mount"); }
                    break;
                case State.Charge:
                    view.SetFrame(((int)(t * 9f)) % 2);
                    Body.Vel.x = dir * 9f * fury;
                    if (Body.HitWall)
                    {
                        Body.Vel.x = 0f; t = 0f;
                        Audio.Play("brick"); Audio.Play("bump");
                        if (dir > 0f)   // the gate side: it splinters
                        {
                            Health -= 1f;
                            GameManager.I.CrackGate(Health);
                            GameManager.I.AddScore(1000, Body.Pos + new Vector2(0f, Body.H));
                            if (Health <= 0.01f) { Calm(); return; }
                            GameManager.I.Toast(Health > 1.5f ? "Ye gate splintereth!" : "Once more, and ye gate shall breake!");
                        }
                        else if (!toldWrongWall) { toldWrongWall = true; GameManager.I.Toast("Ye stone wall holdeth. Leade it to ye gate!"); }
                        state = State.Dazed;
                    }
                    if (t > 4f) { state = State.Rear; t = 0f; }
                    break;
                case State.Dazed:
                    view.SetFrame(3);
                    Body.Vel.x = 0f;
                    if (t > 1.4f) { state = State.Rear; t = 0f; }
                    break;
                case State.Calm:
                    view.SetFrame(4);
                    Body.Vel.x = 0f;
                    if (t > 1.6f)   // it lowers its head; now it is a steed like Bayard
                    {
                        GameManager.I.SpareUnicorn(Body.Pos, dir > 0f);
                        Removed = true; view.Destroy();
                        return;
                    }
                    break;
            }
            Body.Vel.y = Mathf.Max(Body.Vel.y - 40f * dt, -16f);
            Body.Step(lv, dt);
            view.SetPosition(Body.Pos);
        }

        void Calm()
        {
            Health = 0f; Dead = true; state = State.Calm; t = 0f;
            Audio.Play("purify");
            GameManager.I.AddScore(5000, Body.Pos + new Vector2(0f, Body.H + 1f));
            GameManager.I.BossDefeated();
        }

        public override void OnKnight(Knight k, bool fromAbove)
        {
            if (Dead || k.Dead) return;
            if (fromAbove) { k.Body.Vel.y = 13f; Audio.Play("bump", 0.6f); return; }   // its back is no place to stand
            if (state == State.Charge && hurtGrace <= 0f && !k.Blessed) { k.Hurt(); hurtGrace = 1f; }
        }

        public override bool BoltHits(Body bolt)
        {
            if (!Touchable || !bolt.Overlaps(Body)) return false;
            return true;   // bolts glance off its hide
        }

        public override void Hit(float amount) { }   // it cannot be harmed
    }
}
