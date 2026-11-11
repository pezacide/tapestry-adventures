using System.Collections.Generic;
using UnityEngine;

namespace Tapestry
{
    /// <summary>Ye Beaste of Seven Heddes (World 7-4), from the Apocalypse Tapestry. Its body stands at the far side of the hall;
    /// seven crowned heads sway on long necks. Every little while one head strikes: it spits fire, lunges low across the floor,
    /// then droops there, dazed - stomp it (or two bolts) and that head is felled. Fell all seven. The fewer heads, the faster they strike.</summary>
    public class Hydra : BossBase
    {
        public override float MaxHealth { get { return 7f; } }
        public override string DefeatedText { get { return "Huzzah! Ye Beaste of Seven Heddes is slayne!"; } }

        class Head
        {
            public Vector2 Home, Pos, From, To;
            public Body Body; public SpriteView View;
            public readonly List<SpriteRenderer> Neck = new List<SpriteRenderer>();
            public int State;   // 0 swaying, 1 spitting, 2 lunging, 3 drooped (vulnerable), 4 returning
            public float T, Phase, Hp = 1f; public bool Gone;
        }
        readonly List<Head> heads = new List<Head>();
        readonly SpriteView body;
        readonly Vector2 shoulder;
        readonly Body bodyBox;
        float strikeT = 2f, hurtGrace;
        bool toldHow;
        const int NeckSegs = 11;

        public Hydra(Vector2 pos, Transform parent)
        {
            Title = "Ye Beaste of Seven Heddes";
            Health = MaxHealth;
            Body = new Body(pos, 1f, 1f);
            ArenaMin = pos.x - 11f; ArenaMax = pos.x + 11f;
            var bodyAt = new Vector2(pos.x + 6f, pos.y);
            body = new SpriteView("bosses/hydra_body", parent, 9);
            body.Face(false); body.SetPosition(bodyAt);
            bodyBox = new Body(bodyAt, 4.6f, 2.4f);
            shoulder = bodyAt + new Vector2(-1.9f, 2.3f);
            for (int i = 0; i < 7; i++)
            {
                float a = Mathf.Lerp(55f, 165f, i / 6f) * Mathf.Deg2Rad;
                var h = new Head { Phase = i * 0.9f };
                h.Home = shoulder + new Vector2(Mathf.Cos(a) * 4.2f - 1.2f, Mathf.Sin(a) * 3.2f + 0.6f);
                h.Pos = h.Home;
                h.Body = new Body(h.Pos, 1.0f, 0.8f);
                for (int k = 0; k < NeckSegs; k++) h.Neck.Add(Art.Make("enemies/hydra_neck", parent, Vector2.zero, 10));
                h.View = new SpriteView("bosses/hydra_head", parent, 12);
                h.View.Face(false);
                heads.Add(h);
            }
            Place();
        }

        void Place()
        {
            foreach (var h in heads)
            {
                if (h.Gone) continue;
                h.Body.Pos = h.Pos - new Vector2(0f, 0.4f);
                h.View.SetPosition(h.Pos - new Vector2(0f, 0.5f));
                var ctrl = shoulder + new Vector2(-0.5f, 2.2f);
                for (int k = 0; k < NeckSegs; k++)   // a curving neck from the shoulders to the head
                {
                    float u = (k + 0.5f) / NeckSegs;
                    var p = (1 - u) * (1 - u) * shoulder + 2 * (1 - u) * u * ctrl + u * u * (h.Pos + new Vector2(0.5f, 0f));
                    h.Neck[k].transform.localPosition = new Vector3(p.x, p.y - 0.2f, 0f);
                }
            }
        }

        int Alive { get { int n = 0; foreach (var h in heads) if (!h.Gone) n++; return n; } }

        public override void Tick(float dt, Level lv, Knight k)
        {
            if (Removed) return;
            if (Dead)
            {
                bodyBox.Pos.y -= 4f * dt; body.SetPosition(bodyBox.Pos);
                if (bodyBox.Pos.y < -4f) { Removed = true; body.Destroy(); }
                return;
            }
            if (!toldHow) { toldHow = true; GameManager.I.Toast("When a hedde droopeth low and dazed, stompe it! Seven heddes to fell."); }
            hurtGrace -= dt;
            float fury = 1f + (7 - Alive) * 0.1f;
            strikeT -= dt;
            bool anyStriking = false;
            foreach (var h in heads) if (!h.Gone && h.State != 0) anyStriking = true;
            if (strikeT <= 0f && (!anyStriking || Alive <= 3))   // with three or fewer left, they strike more than one at a time
            {
                var idle = heads.FindAll(h => !h.Gone && h.State == 0);
                if (idle.Count > 0)
                {
                    var h = idle[Random.Range(0, idle.Count)];
                    h.State = 1; h.T = 0f;
                    strikeT = 2.4f / fury;
                }
            }
            foreach (var h in heads)
            {
                if (h.Gone) continue;
                h.T += dt;
                switch (h.State)
                {
                    case 0:   // swaying
                        h.Pos = h.Home + new Vector2(Mathf.Sin(Time.time * 1.3f + h.Phase) * 0.3f, Mathf.Sin(Time.time * 1.9f + h.Phase) * 0.25f);
                        h.View.SetFrame(0);
                        break;
                    case 1:   // jaws open: spits fire at the knight
                        h.View.SetFrame(1);
                        if (h.T > 0.5f)
                        {
                            var aim = (k.Body.Pos + new Vector2(0f, 0.8f) - h.Pos).normalized * 6f;
                            GameManager.I.SpawnShot("fireball", h.Pos + new Vector2(-0.6f, 0f), aim);
                            Audio.Play("fireball", 0.7f);
                            h.State = 2; h.T = 0f; h.From = h.Pos;
                            h.To = new Vector2(Mathf.Clamp(k.Body.Pos.x, ArenaMin + 1.5f, shoulder.x - 2f), 2.5f);
                        }
                        break;
                    case 2:   // lunging low across the floor
                        h.View.SetFrame(1);
                        h.Pos = Vector2.Lerp(h.From, h.To, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(h.T / (0.45f / fury))));
                        if (h.T > 0.45f / fury) { h.State = 3; h.T = 0f; Audio.Play("bump", 0.7f); }
                        break;
                    case 3:   // drooping, dazed: strike it now
                        h.View.SetFrame(2);
                        h.Pos = h.To + new Vector2(0f, Mathf.Sin(h.T * 6f) * 0.05f);
                        if (h.T > 1.8f) { h.State = 4; h.T = 0f; h.From = h.Pos; }
                        break;
                    case 4:   // back up to sway with the others
                        h.View.SetFrame(0);
                        h.Pos = Vector2.Lerp(h.From, h.Home, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(h.T / 0.7f)));
                        if (h.T > 0.7f) { h.State = 0; h.T = 0f; }
                        break;
                }
            }
            Place();
        }

        public override void Contact(Knight k)
        {
            if (!Touchable) return;
            foreach (var h in heads)
            {
                if (h.Gone || !k.Body.Overlaps(h.Body)) continue;
                bool onTop = k.Body.Vel.y < 0f && k.Body.PrevBottom >= h.Body.Pos.y + h.Body.H * 0.4f;
                if (k.Blessed || (onTop && h.State == 3)) { if (onTop) k.Body.Vel.y = 12f; Fell(h); continue; }
                if (onTop) { k.Body.Vel.y = 12f; continue; }
                if (h.State == 2 && hurtGrace <= 0f) { k.Hurt(); hurtGrace = 1f; }   // a lunging head bites
            }
            if (k.Body.Overlaps(bodyBox) && hurtGrace <= 0f && !k.Blessed) { k.Hurt(); hurtGrace = 1f; }
        }

        public override void OnKnight(Knight k, bool fromAbove) { }

        public override bool BoltHits(Body bolt)
        {
            if (!Touchable) return false;
            foreach (var h in heads)
            {
                if (h.Gone || !bolt.Overlaps(h.Body)) continue;
                if (h.State == 3) { h.Hp -= 0.5f; if (h.Hp <= 0.01f) Fell(h); else Audio.Play("bump", 0.5f); }
                return true;
            }
            return bolt.Overlaps(bodyBox);
        }

        public override void Hit(float amount) { }

        void Fell(Head h)
        {
            if (h.Gone) return;
            h.Gone = true; h.View.Destroy();
            foreach (var n in h.Neck) if (n != null) Object.Destroy(n.gameObject);
            Health = Alive;
            Audio.Play("stomp"); Audio.Play("roar", 0.8f);
            GameManager.I.AddScore(1000, h.Pos + new Vector2(0f, 1f));
            if (Alive == 0)
            {
                Dead = true; Health = 0f;
                body.Renderer.flipY = false;
                GameManager.I.AddScore(5000, bodyBox.Pos + new Vector2(0f, 3f));
                GameManager.I.BossDefeated();
            }
            else GameManager.I.Toast(Alive + (Alive == 1 ? " hedde remayneth!" : " heddes remayne!"));
        }
    }
}
