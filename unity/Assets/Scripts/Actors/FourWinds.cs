using System.Collections.Generic;
using UnityEngine;

namespace Tapestry
{
    /// <summary>Ye Foure Windes, lord of World 3: four great wind-heads round the hall, two on each side.
    /// In turn each one BLOWS (a strong wind shoves the knight across the hall, toward the fire), then DRAWS BREATH:
    /// it flashes, and for that moment a touch or a bolt defeats it. Four heads, four blows.</summary>
    public class FourWinds : BossBase
    {
        public override float MaxHealth { get { return 4f; } }
        public override bool Touchable { get { return Active && !Dead; } }

        class Head
        {
            public Vector2 At; public bool Left, Beaten; public SpriteRenderer View; public Transform[] Gusts;
            public Rect Box { get { return new Rect(At.x - 0.9f, At.y - 0.9f, 1.8f, 1.8f); } }
        }

        readonly List<Head> heads = new List<Head>();
        enum Phase { Rest, Blow, Breathe }
        Phase phase = Phase.Rest;
        int current = -1;
        float t, hurtGrace;

        public FourWinds(Vector2 centre, Transform parent)
        {
            Title = "Ye Foure Windes";
            Health = MaxHealth;
            Body = new Body(centre, 1f, 1f);   // a marker only: the heads are what you strike
            ArenaMin = centre.x - 12f; ArenaMax = centre.x + 12f;
            foreach (var pos in new[] { new Vector2(-9.5f, 3.6f), new Vector2(-9.5f, 7.4f), new Vector2(9.5f, 3.6f), new Vector2(9.5f, 7.4f) })
            {
                var h = new Head { At = centre + new Vector2(pos.x, pos.y - centre.y), Left = pos.x < 0f };
                h.At = new Vector2(centre.x + pos.x, pos.y);
                h.View = Art.Make("scenery/wind_head", parent, new Vector2(h.At.x, h.At.y - 1.25f), 8);
                h.View.transform.localScale = new Vector3(1.25f, 1.25f, 1f);
                h.View.flipX = !h.Left;   // the left heads blow right, the right heads blow left
                h.Gusts = new Transform[3];
                for (int i = 0; i < 3; i++)
                {
                    var g = Art.Make("scenery/gust", parent, Vector2.zero, 8);
                    g.flipX = !h.Left; g.enabled = false;
                    h.Gusts[i] = g.transform;
                }
                heads.Add(h);
            }
        }

        Head Cur { get { return current >= 0 ? heads[current] : null; } }

        void NextHead()
        {
            for (int k = 1; k <= 4; k++)
            {
                int i = (current + k) % 4;
                if (!heads[i].Beaten) { current = i; return; }
            }
        }

        public override void Tick(float dt, Level lv, Knight player)
        {
            if (Removed) return;
            t += dt; hurtGrace -= dt;
            if (Dead) { if (t > 1f) Removed = true; return; }
            float quick = Health <= 2f ? 0.7f : 1f;   // the last two are quicker
            switch (phase)
            {
                case Phase.Rest:
                    if (t > 1f * quick) { NextHead(); phase = Phase.Blow; t = 0f; Audio.Play("fireball", 0.8f); }
                    break;
                case Phase.Blow:
                    if (!player.Dead) player.Body.Nudge(lv, (Cur.Left ? 1f : -1f) * 5f * dt);
                    if (t > 2.4f * quick) { phase = Phase.Breathe; t = 0f; Audio.Play("pause", 0.6f); }
                    break;
                case Phase.Breathe:
                    // drawing breath: a gentle pull toward it - and it can be struck
                    if (!player.Dead) player.Body.Nudge(lv, (Cur.Left ? -1f : 1f) * 1.2f * dt);
                    if (t > 1.8f) { phase = Phase.Rest; t = 0f; }
                    break;
            }
            for (int i = 0; i < heads.Count; i++)
            {
                var h = heads[i];
                bool blowing = i == current && phase == Phase.Blow;
                bool breathing = i == current && phase == Phase.Breathe;
                h.View.color = h.Beaten ? new Color(0.45f, 0.45f, 0.5f) : breathing && Mathf.Repeat(t, 0.2f) > 0.1f ? new Color(1f, 0.85f, 0.4f) : Color.white;
                float puff = blowing ? 1.25f + Mathf.Sin(t * 20f) * 0.05f : breathing ? 1.1f : 1.25f;
                h.View.transform.localScale = new Vector3(puff, puff, 1f);
                for (int g = 0; g < 3; g++)
                {
                    var gs = h.Gusts[g];
                    gs.GetComponent<SpriteRenderer>().enabled = blowing;
                    if (!blowing) continue;
                    float along = Mathf.Repeat(t * 9f + g * 6f, 18f);
                    gs.localPosition = new Vector3(h.At.x + (h.Left ? 1f : -1f) * (1.5f + along), h.At.y - 0.8f + (g - 1) * 1.2f, 0f);
                }
            }
        }

        bool Vulnerable(Head h) { return heads.IndexOf(h) == current && phase == Phase.Breathe && !h.Beaten; }

        static bool Overlaps(Rect r, Body b) { return b.Right > r.xMin && b.Left < r.xMax && b.Top > r.yMin && b.Pos.y < r.yMax; }

        public override void Contact(Knight k)
        {
            if (!Touchable || k.Dead) return;
            foreach (var h in heads)
            {
                if (h.Beaten || !Overlaps(h.Box, k.Body)) continue;
                if (Vulnerable(h) || k.Blessed) { Strike(h); k.Body.Vel.y = Mathf.Max(k.Body.Vel.y, 9f); }
                else if (hurtGrace <= 0f) { k.Hurt(); hurtGrace = 1f; }
                return;
            }
        }

        public override bool BoltHits(Body bolt)
        {
            if (!Touchable) return false;
            foreach (var h in heads)
                if (!h.Beaten && Overlaps(h.Box, bolt))
                {
                    if (Vulnerable(h)) Strike(h);
                    return true;   // bolts glance off a head that is not drawing breath
                }
            return false;
        }

        public override void OnKnight(Knight k, bool fromAbove) { }
        public override void Hit(float amount) { if (Cur != null && Vulnerable(Cur)) Strike(Cur); }

        void Strike(Head h)
        {
            h.Beaten = true;
            Health -= 1f;
            Audio.Play("stomp"); Audio.Play("roar", 0.8f);
            GameManager.I.AddScore(1000, h.At + new Vector2(0f, 1.2f));
            phase = Phase.Rest; t = 0f;
            if (Health <= 0.01f)
            {
                Health = 0f; Dead = true; t = 0f;
                GameManager.I.AddScore(5000, Body.Pos + new Vector2(0f, 4f));
                GameManager.I.BossDefeated();
            }
        }
    }
}
