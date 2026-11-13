using UnityEngine;

namespace Tapestry
{
    /// <summary>The player: Mario-style running and jumping, plus riding Bayard.</summary>
    public class Knight
    {
        const float FootW = 0.62f, FootH = 1.72f, RideW = 1.25f, RideH = 1.9f, CrouchH = 0.95f;   // crouched, he fits a one-tile gap
        const float RiseGravity = 24f, FallGravity = 60f, MaxFall = 17f;

        public readonly Body Body;
        public KnightDef Def { get; private set; }
        public bool Mounted { get; private set; }
        public string MountKind { get; private set; }   // "bayard" or "unicorn" while mounted
        public bool OnUnicorn { get { return Mounted && MountKind == "unicorn"; } }
        public string MountName { get { return OnUnicorn ? "ye Unicorne" : "Bayard"; } }
        public bool Dead { get; private set; }
        public int Hearts = 3;
        public bool FacingRight { get { return view.FacingRight; } }
        public bool HasCrossbow { get; private set; }
        public float RelicTime;     // > 0: the holy relique protects the knight and smites beasts
        public float Petrified;     // > 0: turned to stone by the Cockatrice's gaze
        public void Petrify(float t) { if (!Blessed && !Dead) { Petrified = Mathf.Max(Petrified, t); Audio.Play("bump"); } }
        public void Unfreeze() { frozen = false; }
        public bool Blessed { get { return RelicTime > 0f; } }
        SpriteRenderer crossbowView;
        Transform auraPivot; SpriteRenderer auraView;
        public Enemy Carrying;      // a green shell held in front of the knight

        readonly SpriteView view;
        float invuln, coyote, jumpBuffer, flutterTime, anim;
        bool flutterUsed, frozen;

        public Knight(KnightDef def, Vector2 pos, Transform parent)
        {
            Def = def;
            Body = new Body(pos, FootW, FootH);
            Body.HitsHidden = true;
            Hearts = def.Hearts;
            view = new SpriteView(def.Sheet, parent, 20);
            view.SetPosition(pos);
        }

        public void SetKnight(KnightDef def)
        {
            Def = def;
            view.SetSheet(Mounted ? def.RideSheet(MountKind) : def.Sheet);
        }

        public void Freeze() { frozen = true; Body.Vel = Vector2.zero; }
        public void Hide() { view.Renderer.enabled = false; if (crossbowView != null) crossbowView.enabled = false; }
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
            if (frozen) { Body.Grounded = true; Animate(dt); if (lying) { SetLying(false); view.SetTilt(0f, 0f); } view.SetPosition(Body.Pos); return; }

            if (Petrified > 0f) Petrified -= dt;
            bool stone = Petrified > 0f;
            if (GameManager.I.Underwater) Swim(dt, lv, stone);
            else
            {
            float h = stone ? 0f : TapestryInput.Horizontal;
            // crouch: on foot, on the ground, holding DOWN. He ducks low (under spears, flung comfits and the like) and creeps; he stands
            // again as soon as there is room above him (so he stays crouched in a low passage).
            bool wantCrouch = !Mounted && !stone && Body.Grounded && TapestryInput.DownHeld;
            if (wantCrouch && !crouching) { crouching = true; Body.H = CrouchH; }
            else if (crouching && !wantCrouch && RoomToStand(lv)) { crouching = false; Body.H = FootH; }
            float speedMul = OnUnicorn ? 1.45f : Mounted ? 1.2f : 1f;   // the unicorn is the swifter steed
            float target = h * (TapestryInput.RunHeld && !crouching ? Def.Run : Def.Walk) * speedMul * (crouching ? 0.4f : 1f);
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
            if (TapestryInput.JumpPressed && !stone) jumpBuffer = 0.12f; else jumpBuffer -= dt;
            if (jumpBuffer > 0f && coyote > 0f)
            {
                Body.Vel.y = Def.Jump + Mathf.Abs(Body.Vel.x) * 0.12f + (OnUnicorn ? 1.0f : Mounted ? 1.4f : 0f);
                jumpBuffer = coyote = 0f;
                Audio.Play("jump", 0.55f);
            }

            // dismount
            if (Mounted && TapestryInput.DownPressed && Body.Grounded) Dismount(false);

            float g = Body.Vel.y > 0f && TapestryInput.JumpHeld ? RiseGravity : FallGravity;
            float maxFall = MaxFall;
            if (Def.Glide && !Mounted && Body.Vel.y < 0f && TapestryInput.JumpHeld) maxFall = 2.6f;

            // Bayard's flutter-leap: once per jump, hold JUMP while falling for a short boost (like Yoshi)
            if (Mounted && !OnUnicorn && !Body.Grounded && Body.Vel.y < 0f && TapestryInput.JumpHeld && !flutterUsed) { flutterUsed = true; flutterTime = 0.45f; }
            if (flutterTime > 0f)
            {
                flutterTime -= dt;
                if (TapestryInput.JumpHeld) { g = -28f; if (Body.Vel.y > 3.5f) Body.Vel.y = 3.5f; }
                else flutterTime = 0f;
            }

            Body.Vel.y = Mathf.Max(Body.Vel.y - g * dt, -maxFall);
            GameManager.I.CarryOnLift(Body);
            Body.Step(lv, dt);
            GameManager.I.LandOnLifts(Body);
            }

            if (Body.HitCeiling) GameManager.I.KnightHitBlock(Body.CeilX, Body.CeilY);

            // the crossbowe: RUN shoots (as Mario throws fireballs with the run button)
            if (!stone && HasCrossbow && Carrying == null && TapestryInput.RunPressed && GameManager.I.BoltCount < 2)
            {
                GameManager.I.SpawnBolt(Body.Pos + new Vector2(FacingRight ? 0.6f : -0.6f, lying ? 0.35f : Mounted ? 1.45f : crouching ? 0.55f : 0.95f), FacingRight);
                Audio.Play("crossbow", 0.8f);
            }

            if (invuln > 0f) invuln -= dt;
            view.Renderer.enabled = invuln <= 0f || Mathf.Repeat(invuln, 0.16f) > 0.08f;
            if (RelicTime > 0f)
            {
                RelicTime -= dt;
                ShowGlory(RelicTime > 0f);
            }
            else view.Renderer.color = stone ? new Color(0.55f, 0.56f, 0.6f) : Color.white;   // grey as stone while petrified
            PlaceCrossbow();
            Animate(dt);
            view.SetTilt(lying ? (FacingRight ? -80f : 80f) : 0f, Body.H * 0.5f);
            view.SetPosition(Body.Pos);
        }

        float stroke;   // time since the last swimming stroke
        bool crouching; // ducking low (DOWNE held, or the stick pulled down)
        public bool Crouching { get { return crouching; } }
        bool lying;     // swimming flat, head first (the body is low and long while so)
        const float SwimW = 1.2f, SwimH = 0.75f;

        /// <summary>Is there room above the knight's feet to stand up straight?</summary>
        bool RoomToStand(Level lv)
        {
            int x0 = Mathf.FloorToInt(Body.Pos.x - FootW * 0.5f), x1 = Mathf.FloorToInt(Body.Pos.x + FootW * 0.5f);
            int y0 = Mathf.FloorToInt(Body.Pos.y + 0.05f), y1 = Mathf.FloorToInt(Body.Pos.y + FootH - 0.05f);
            for (int x = x0; x <= x1; x++)
                for (int y = y0; y <= y1; y++)
                    if (lv.Solid(x, y)) return false;
            return Body.Pos.y + FootH <= 9.6f;
        }

        void SetLying(bool on)
        {
            if (on == lying) return;
            lying = on;
            if (on) { Body.W = SwimW; Body.H = SwimH; }
            else { Body.W = FootW; Body.H = FootH; }
        }

        /// <summary>Under the waves: tap JUMPE to stroke upward; the knight sinks slowly and cannot rise past the surface.</summary>
        void Swim(float dt, Level lv, bool stone)
        {
            float h = stone ? 0f : TapestryInput.Horizontal;
            float max = TapestryInput.RunHeld ? 4.2f : 3.0f;
            Body.Vel.x = Mathf.MoveTowards(Body.Vel.x, h * max, 9f * dt);
            if (h > 0.1f) view.Face(true); else if (h < -0.1f) view.Face(false);
            stroke += dt;
            if (TapestryInput.JumpPressed && !stone)
            {
                Body.Vel.y = Mathf.Min(Mathf.Max(Body.Vel.y, 0f) + 4.2f, 5f);
                stroke = 0f;
                Audio.Play("jump", 0.25f);
            }
            Body.Vel.y = Mathf.Max(Body.Vel.y - 7f * dt, -2.4f);
            Body.Step(lv, dt);
            const float surface = 9.6f;   // the water's surface: no swimming out of the level
            if (Body.Top > surface) { Body.Pos.y = surface - Body.H; if (Body.Vel.y > 0f) Body.Vel.y = 0f; }
            // flat and head first while swimming; upright to walk the sea-bed (if there is headroom)
            SetLying(!(Body.Grounded && RoomToStand(lv)));
        }

        void Animate(float dt)
        {
            int frame;
            if (crouching && !GameManager.I.Underwater) frame = 5;
            else if (GameManager.I.Underwater && !Body.Grounded && !frozen) frame = stroke < 0.25f ? 1 : 3;   // a kick, then a glide
            else if (!Body.Grounded && !frozen) frame = 3;
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
            if (HasCrossbow)
            {
                // like losing the fire flower: the crossbowe is knocked away, but no health is lost
                HasCrossbow = false; PlaceCrossbow();
                invuln = 1.4f;
                Audio.Play("hurt", 0.6f);
                GameManager.I.Toast("Thy crossbowe is knocked from thy hande!");
                return;
            }
            Hearts--;
            invuln = 1.6f;
            Audio.Play("hurt");
            if (Hearts <= 0) Die();
        }

        /// <summary>fell = dropped into a pit or the sea: pop back up into view so the death is seen.</summary>
        public void Die(bool fell = false)
        {
            if (Dead) return;
            Dead = true; if (crouching) { crouching = false; Body.H = FootH; }
            if (Mounted) { Mounted = false; view.SetSheet(Def.Sheet); }
            view.Renderer.enabled = true;
            RelicTime = 0f; ShowGlory(false); HasCrossbow = false; PlaceCrossbow();
            if (lying) { SetLying(false); view.SetTilt(0f, 0f); }
            view.SetFrame(3);
            view.Renderer.flipY = true;
            Body.Vel = new Vector2(0f, fell ? 17f : 11f);
            if (fell) Body.Pos.y = Mathf.Min(Body.Pos.y, -1.6f);
            GameManager.I.PlayerDied(fell);
        }

        public void Mount(string kind = "bayard", bool silent = false)
        {
            Mounted = true; MountKind = kind; crouching = false;
            Body.W = RideW; Body.H = RideH;
            if (!silent) Audio.Play("mount", 0.9f);
            view.SetSheet(Def.RideSheet(kind));
        }

        void Dismount(bool thrown)
        {
            Mounted = false;
            Body.W = FootW; Body.H = FootH;
            view.SetSheet(Def.Sheet);
            GameManager.I.ReleaseBayard(Body.Pos, FacingRight, thrown, MountKind);
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

        static readonly Color[] gloryColours =
        {
            new Color(1f, 0.82f, 0.15f), Color.white, new Color(1f, 0.55f, 0.2f), new Color(1f, 1f, 0.6f),
        };

        /// <summary>The holy relique's glory: a pulsing golden aura of rays, and the knight flickers gold and white.
        /// It flickers more slowly in the last two seconds, as a warning that it is ending.</summary>
        void ShowGlory(bool on)
        {
            if (auraPivot == null)
            {
                auraPivot = new GameObject("aura").transform;
                auraPivot.SetParent(view.Root.transform, false);
                auraView = Art.Make("scenery/aura", auraPivot, new Vector2(0f, -76f / Art.DesignPerTile), 19);
            }
            auraPivot.gameObject.SetActive(on);
            if (!on) { view.Renderer.color = Color.white; return; }
            float rate = RelicTime > 2f ? 14f : 6f;
            int i = (int)(Time.time * rate) % gloryColours.Length;
            view.Renderer.color = gloryColours[i];
            float pulse = 1f + 0.12f * Mathf.Sin(Time.time * 12f);
            float size = Mounted ? 1.45f : 1f;
            auraPivot.localPosition = new Vector3(0f, Body.H * 0.5f, 0f);
            auraPivot.localScale = new Vector3(pulse * size, pulse * size, 1f);
            auraPivot.localEulerAngles = new Vector3(0f, 0f, Time.time * 40f);
            auraView.color = new Color(1f, 1f, 1f, i % 2 == 0 ? 1f : 0.65f);
        }

        /// <summary>Carry the knight's state on into the next level or part.</summary>
        public void ApplyCarry(int hearts, bool crossbow, string mount)
        {
            Hearts = Mathf.Clamp(hearts, 1, Def.Hearts);
            if (crossbow) GiveCrossbow();
            if (!string.IsNullOrEmpty(mount)) Mount(mount, true);
        }

        void PlaceCrossbow()
        {
            if (crossbowView == null) return;
            crossbowView.enabled = HasCrossbow && view.Renderer.enabled && !Dead;
            crossbowView.flipX = !FacingRight;
            crossbowView.transform.localPosition = lying ? new Vector3(FacingRight ? 0.75f : -0.75f, 0.2f, 0f)
                : new Vector3(FacingRight ? 0.3f : -0.3f, Mounted ? 1.3f : crouching ? 0.42f : 0.75f, 0f);
        }

        public void Destroy() { view.Destroy(); }
    }
}
