using UnityEngine;

namespace Tapestry
{
    /// <summary>The player: Mario-style running and jumping, plus riding Bayard.</summary>
    public class Knight
    {
        const float FootW = 0.62f, FootH = 1.72f, RideW = 1.25f, RideH = 1.9f;
        const float RiseGravity = 24f, FallGravity = 60f, MaxFall = 17f;

        public readonly Body Body;
        public KnightDef Def { get; private set; }
        public bool Mounted { get; private set; }
        public bool Dead { get; private set; }
        public int Hearts = 3;
        public bool FacingRight { get { return view.FacingRight; } }
        public bool HasCrossbow { get; private set; }
        public float RelicTime;     // > 0: the holy relique protects the knight and smites beasts
        public bool Blessed { get { return RelicTime > 0f; } }
        SpriteRenderer crossbowView;

        readonly SpriteView view;
        float invuln, coyote, jumpBuffer, flutterTime, anim;
        bool flutterUsed, frozen;

        public Knight(KnightDef def, Vector2 pos, Transform parent)
        {
            Def = def;
            Body = new Body(pos, FootW, FootH);
            Hearts = def.Hearts;
            view = new SpriteView(def.Sheet, parent, 20);
            view.SetPosition(pos);
        }

        public void SetKnight(KnightDef def)
        {
            Def = def;
            view.SetSheet(Mounted ? def.MountedSheet : def.Sheet);
        }

        public void Freeze() { frozen = true; Body.Vel = Vector2.zero; }
        public void Hide() { view.Renderer.enabled = false; }
        public void FaceRight() { view.Face(true); }

        public void Tick(float dt, Level lv)
        {
            if (Dead)
            {
                Body.Vel.y -= FallGravity * 0.6f * dt;
                Body.Pos += Body.Vel * dt;
                view.SetPosition(Body.Pos);
                return;
            }
            if (frozen) { Body.Grounded = true; Animate(dt); view.SetPosition(Body.Pos); return; }

            float h = TapestryInput.Horizontal;
            float speedMul = Mounted ? 1.2f : 1f;
            float target = h * (TapestryInput.RunHeld ? Def.Run : Def.Walk) * speedMul;
            float accel;
            if (Body.Grounded || coyote > 0f)
                accel = h != 0f && Mathf.Sign(h) != Mathf.Sign(Body.Vel.x) && Mathf.Abs(Body.Vel.x) > 0.5f ? 55f : (h != 0f ? 28f : 34f);
            else
                accel = 18f;
            Body.Vel.x = Mathf.MoveTowards(Body.Vel.x, target, accel * dt);
            if (h > 0.1f) view.Face(true); else if (h < -0.1f) view.Face(false);

            // jump with coyote time and a jump buffer, so it feels forgiving
            if (Body.Grounded) { coyote = 0.1f; flutterUsed = false; }
            else coyote -= dt;
            if (TapestryInput.JumpPressed) jumpBuffer = 0.12f; else jumpBuffer -= dt;
            if (jumpBuffer > 0f && coyote > 0f)
            {
                Body.Vel.y = Def.Jump + Mathf.Abs(Body.Vel.x) * 0.12f + (Mounted ? 1.4f : 0f);
                jumpBuffer = coyote = 0f;
                Audio.Play("jump", 0.55f);
            }

            // dismount
            if (Mounted && TapestryInput.DownPressed && Body.Grounded) Dismount(false);

            float g = Body.Vel.y > 0f && TapestryInput.JumpHeld ? RiseGravity : FallGravity;
            float maxFall = MaxFall;
            if (Def.Glide && !Mounted && Body.Vel.y < 0f && TapestryInput.JumpHeld) maxFall = 2.6f;

            // Bayard's flutter-leap: once per jump, hold JUMP while falling for a short boost (like Yoshi)
            if (Mounted && !Body.Grounded && Body.Vel.y < 0f && TapestryInput.JumpHeld && !flutterUsed) { flutterUsed = true; flutterTime = 0.45f; }
            if (flutterTime > 0f)
            {
                flutterTime -= dt;
                if (TapestryInput.JumpHeld) { g = -28f; if (Body.Vel.y > 3.5f) Body.Vel.y = 3.5f; }
                else flutterTime = 0f;
            }

            Body.Vel.y = Mathf.Max(Body.Vel.y - g * dt, -maxFall);
            Body.Step(lv, dt);

            if (Body.HitCeiling) GameManager.I.KnightHitBlock(Body.CeilX, Body.CeilY);

            // the crossbowe: RUN shoots (as Mario throws fireballs with the run button)
            if (HasCrossbow && TapestryInput.RunPressed && GameManager.I.BoltCount < 2)
            {
                GameManager.I.SpawnBolt(Body.Pos + new Vector2(FacingRight ? 0.6f : -0.6f, Mounted ? 1.45f : 0.95f), FacingRight);
                Audio.Play("crossbow", 0.8f);
            }

            if (invuln > 0f) invuln -= dt;
            view.Renderer.enabled = invuln <= 0f || Mathf.Repeat(invuln, 0.16f) > 0.08f;
            if (RelicTime > 0f)
            {
                RelicTime -= dt;
                float k = Mathf.Repeat(Time.time * 6f, 1f);
                view.Renderer.color = Color.Lerp(new Color(1f, 0.85f, 0.35f), Color.white, k);
                if (RelicTime <= 0f) view.Renderer.color = Color.white;
            }
            PlaceCrossbow();
            Animate(dt);
            view.SetPosition(Body.Pos);
        }

        void Animate(float dt)
        {
            int frame;
            if (!Body.Grounded && !frozen) frame = 3;
            else if (Mathf.Abs(Body.Vel.x) < 0.3f) frame = 0;
            else
            {
                anim += dt * (2f + Mathf.Abs(Body.Vel.x) * 0.9f);
                frame = 1 + ((int)anim % 2);
            }
            view.SetFrame(frame);
        }

        /// <summary>Spring off a stomped beast.</summary>
        public void Bounce()
        {
            Body.Vel.y = TapestryInput.JumpHeld ? 13f : 9f;
            flutterUsed = false;
        }

        public bool Invulnerable { get { return invuln > 0f || Dead || Blessed; } }

        public void Hurt()
        {
            if (Invulnerable) return;
            if (Mounted)
            {
                Dismount(true);
                invuln = 1.4f;
                return;
            }
            Hearts--;
            invuln = 1.6f;
            Audio.Play("hurt");
            if (HasCrossbow) { HasCrossbow = false; PlaceCrossbow(); }   // like losing the fire flower
            if (Hearts <= 0) Die();
        }

        /// <summary>fell = dropped into a pit or the sea: pop back up into view so the death is seen.</summary>
        public void Die(bool fell = false)
        {
            if (Dead) return;
            Dead = true;
            if (Mounted) { Mounted = false; view.SetSheet(Def.Sheet); }
            view.Renderer.enabled = true;
            RelicTime = 0f; view.Renderer.color = Color.white; HasCrossbow = false; PlaceCrossbow();
            view.SetFrame(3);
            view.Renderer.flipY = true;
            Body.Vel = new Vector2(0f, fell ? 17f : 11f);
            if (fell) Body.Pos.y = Mathf.Min(Body.Pos.y, -1.6f);
            GameManager.I.PlayerDied(fell);
        }

        public void Mount()
        {
            Mounted = true;
            Body.W = RideW; Body.H = RideH;
            Audio.Play("mount", 0.9f);
            view.SetSheet(Def.MountedSheet);
        }

        void Dismount(bool thrown)
        {
            Mounted = false;
            Body.W = FootW; Body.H = FootH;
            view.SetSheet(Def.Sheet);
            GameManager.I.ReleaseBayard(Body.Pos, FacingRight, thrown);
            Body.Vel.y = thrown ? 10f : 7f;
            if (thrown) Audio.Play("thrown");
            Body.Vel.x = thrown ? (FacingRight ? -3f : 3f) : 0f;
        }

        public void GiveCrossbow()
        {
            HasCrossbow = true;
            if (crossbowView == null)
            {
                crossbowView = Art.Make("items/crossbow", view.Root.transform, Vector2.zero, 21);
                crossbowView.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
            }
            PlaceCrossbow();
        }

        public void Heal() { Hearts = Mathf.Min(Hearts + 1, Def.Hearts); }

        void PlaceCrossbow()
        {
            if (crossbowView == null) return;
            crossbowView.enabled = HasCrossbow && view.Renderer.enabled && !Dead;
            crossbowView.flipX = !FacingRight;
            crossbowView.transform.localPosition = new Vector3(FacingRight ? 0.3f : -0.3f, Mounted ? 1.3f : 0.75f, 0f);
        }

        public void Destroy() { view.Destroy(); }
    }
}
