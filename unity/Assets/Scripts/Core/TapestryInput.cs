using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Tapestry
{
    /// <summary>One place that reads keyboard, gamepad (Xbox / PlayStation / Switch pads) and touch,
    /// with either Unity input backend. Call Poll() once per frame.</summary>
    public static class TapestryInput
    {
        public static float Horizontal;
        public static bool JumpHeld, JumpPressed, RunHeld, RunPressed, DownHeld, DownPressed, StartPressed, NextKnightPressed, PrevKnightPressed, MutePressed;
        public static bool LeftPressed, RightPressed;
        public static bool TouchSeen;
        public static Vector2? TapPosition;    // GUI coordinates (top-left origin) of a new touch/click this frame

        static bool prevMute, prevJump, prevRun, prevDown, prevStart, prevNext, prevPrev, prevLeft, prevRight;

        // ---- on-screen touch controls (GUI coordinates, top-left origin), laid out inside the safe area ----
        // a fixed joystick bottom-left; round JUMPE and RUNNE/SHOOTE buttons bottom-right; pause top-right
        static float Unit { get { return Mathf.Min(Screen.height, Screen.width * 9f / 16f); } }
        static Vector2 At(float fx, float fy) { var a = Safe; return new Vector2(a.x + fx * a.width, a.y + fy * a.height); }
        public static Vector2 JoyCentre { get { return At(0.092f, 0.825f); } }   // tucked into the bottom-left corner, just inside the screen edge
        public static float JoyRadius { get { return Unit * 0.15f; } }
        public static Vector2 JumpCentre { get { return At(0.915f, 0.70f); } }
        public static float JumpRadius { get { return Unit * 0.135f; } }
        public static Vector2 RunCentre { get { return At(0.80f, 0.845f); } }
        public static float RunRadius { get { return Unit * 0.105f; } }
        public static Vector2 PauseCentre { get { return At(0.955f, 0.19f); } }
        public static float PauseRadius { get { return Unit * 0.05f; } }
        public static Vector2 JoyKnob;             // where the joystick knob is pushed to, relative to its centre
        public static bool TouchJump, TouchRun;    // for drawing the buttons pressed
        static bool joyTaken, stickRun;

        public static bool ShowTouchControls { get { return TouchSeen || Application.isMobilePlatform; } }

        public static void Poll()
        {
            float h = 0f;
            bool jump = false, run = false, down = false, start = false, next = false, prev = false, mute = false;
            TapPosition = null;
            JoyKnob = Vector2.zero; joyTaken = false; stickRun = false; TouchJump = TouchRun = false;

#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.leftArrowKey.isPressed || kb.aKey.isPressed) h -= 1f;
                if (kb.rightArrowKey.isPressed || kb.dKey.isPressed) h += 1f;
                jump |= kb.spaceKey.isPressed || kb.zKey.isPressed || kb.kKey.isPressed || kb.upArrowKey.isPressed || kb.wKey.isPressed;
                run |= kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed || kb.xKey.isPressed || kb.jKey.isPressed;
                down |= kb.downArrowKey.isPressed || kb.sKey.isPressed;
                start |= kb.enterKey.isPressed || kb.escapeKey.isPressed || kb.pKey.isPressed;
                next |= kb.eKey.isPressed || kb.tabKey.isPressed;
                prev |= kb.qKey.isPressed;
                mute |= kb.mKey.isPressed;
            }
            var pad = Gamepad.current;
            if (pad != null)
            {
                Vector2 s = pad.leftStick.ReadValue();
                if (pad.dpad.left.isPressed || s.x < -0.35f) h -= 1f;
                if (pad.dpad.right.isPressed || s.x > 0.35f) h += 1f;
                jump |= pad.buttonSouth.isPressed;
                run |= pad.buttonWest.isPressed || pad.rightTrigger.ReadValue() > 0.4f;
                down |= pad.dpad.down.isPressed || s.y < -0.6f;
                start |= pad.startButton.isPressed;
                next |= pad.rightShoulder.isPressed;
                prev |= pad.leftShoulder.isPressed;
                mute |= pad.selectButton.isPressed;
            }
            var ts = Touchscreen.current;
            int touchCount = 0;
            if (ts != null)
            {
                foreach (var t in ts.touches)
                {
                    if (!t.press.isPressed) continue;
                    touchCount++;
                    TouchSeen = true;
                    Vector2 p = t.position.ReadValue();
                    ApplyTouch(new Vector2(p.x, Screen.height - p.y), ref h, ref jump, ref run, ref down, ref start);
                    if (t.press.wasPressedThisFrame) TapPosition = new Vector2(p.x, Screen.height - p.y);
                }
            }
            var mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            {
                Vector2 p = mouse.position.ReadValue();
                TapPosition = new Vector2(p.x, Screen.height - p.y);
            }
#else
            int touchCount = 0;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) h -= 1f;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) h += 1f;
            float ax = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(ax) > 0.35f) h += Mathf.Sign(ax);
            jump |= Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.UpArrow)
                    || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.JoystickButton0);
            run |= Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.J)
                   || Input.GetKey(KeyCode.JoystickButton2);
            down |= Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || Input.GetAxisRaw("Vertical") < -0.6f;
            start |= Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.P) || Input.GetKey(KeyCode.JoystickButton7);
            next |= Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Tab) || Input.GetKey(KeyCode.JoystickButton5);
            prev |= Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.JoystickButton4);
            mute |= Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.JoystickButton6);
            for (int i = 0; i < Input.touchCount; i++)
            {
                var t = Input.GetTouch(i);
                if (t.phase == UnityEngine.TouchPhase.Ended || t.phase == UnityEngine.TouchPhase.Canceled) continue;
                touchCount++;
                TouchSeen = true;
                var gp = new Vector2(t.position.x, Screen.height - t.position.y);
                ApplyTouch(gp, ref h, ref jump, ref run, ref down, ref start);
                if (t.phase == UnityEngine.TouchPhase.Began) TapPosition = gp;
            }
            if (Input.GetMouseButtonDown(0) && Input.touchCount == 0)
                TapPosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
#endif

            Horizontal = Mathf.Clamp(h, -1f, 1f);
            JumpPressed = jump && !prevJump; JumpHeld = jump; prevJump = jump;
            // pushing the stick all the way runs, but only the button fires the crossbowe
            RunHeld = run || stickRun; RunPressed = run && !prevRun; prevRun = run;
            DownHeld = down; DownPressed = down && !prevDown; prevDown = down;
            StartPressed = start && !prevStart; prevStart = start;
            NextKnightPressed = next && !prevNext; prevNext = next;
            PrevKnightPressed = prev && !prevPrev; prevPrev = prev;
            MutePressed = mute && !prevMute; prevMute = mute;
            bool left = Horizontal < -0.5f, right = Horizontal > 0.5f;
            LeftPressed = left && !prevLeft; prevLeft = left;
            RightPressed = right && !prevRight; prevRight = right;
        }

        static void ApplyTouch(Vector2 gui, ref float h, ref bool jump, ref bool run, ref bool down, ref bool start)
        {
            var d = gui - JoyCentre;
            if (!joyTaken && d.magnitude < JoyRadius * 1.9f)
            {
                // the fixed joystick: left/right walks, pushed to the rim runs, pulled down dismounts
                joyTaken = true;
                var k = Vector2.ClampMagnitude(d, JoyRadius);
                JoyKnob = k;
                float nx = k.x / JoyRadius, ny = k.y / JoyRadius;
                if (Mathf.Abs(nx) > 0.25f) h += Mathf.Sign(nx);
                if (Mathf.Abs(nx) > 0.85f) stickRun = true;
                if (ny > 0.6f && Mathf.Abs(nx) < 0.7f) down = true;
                return;
            }
            if ((gui - JumpCentre).magnitude < JumpRadius * 1.15f) { jump = true; TouchJump = true; }
            if ((gui - RunCentre).magnitude < RunRadius * 1.15f) { run = true; TouchRun = true; }
            if ((gui - PauseCentre).magnitude < PauseRadius * 1.4f) start = true;
        }

        /// <summary>The part of the screen clear of camera notches and rounded corners, in GUI coordinates (top-left origin).</summary>
        public static Rect Safe
        {
            get
            {
                var s = Screen.safeArea;
                if (s.width <= 0f || s.height <= 0f) return new Rect(0, 0, Screen.width, Screen.height);
                return new Rect(s.x, Screen.height - s.y - s.height, s.width, s.height);
            }
        }

        public static Rect ToScreen(Rect frac)
        {
            var a = Safe;
            return new Rect(a.x + frac.x * a.width, a.y + frac.y * a.height, frac.width * a.width, frac.height * a.height);
        }


    }
}
