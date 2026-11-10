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
        public static bool JumpHeld, JumpPressed, RunHeld, RunPressed, DownPressed, StartPressed, NextKnightPressed, PrevKnightPressed;
        public static bool LeftPressed, RightPressed;
        public static bool TouchSeen;
        public static Vector2? TapPosition;    // GUI coordinates (top-left origin) of a new touch/click this frame

        static bool prevJump, prevRun, prevDown, prevStart, prevNext, prevPrev, prevLeft, prevRight;

        // On-screen touch buttons, as fractions of the screen (x, y from top, w, h).
        public static readonly Rect LeftButton = new Rect(0.02f, 0.74f, 0.11f, 0.22f);
        public static readonly Rect RightButton = new Rect(0.15f, 0.74f, 0.11f, 0.22f);
        public static readonly Rect DownButton = new Rect(0.085f, 0.52f, 0.11f, 0.18f);
        public static readonly Rect RunButton = new Rect(0.72f, 0.74f, 0.12f, 0.22f);
        public static readonly Rect JumpButton = new Rect(0.86f, 0.70f, 0.12f, 0.26f);
        public static readonly Rect PauseButton = new Rect(0.90f, 0.14f, 0.08f, 0.10f);

        public static bool ShowTouchControls { get { return TouchSeen || Application.isMobilePlatform; } }

        public static void Poll()
        {
            float h = 0f;
            bool jump = false, run = false, down = false, start = false, next = false, prev = false;
            TapPosition = null;

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
            RunHeld = run; RunPressed = run && !prevRun; prevRun = run;
            DownPressed = down && !prevDown; prevDown = down;
            StartPressed = start && !prevStart; prevStart = start;
            NextKnightPressed = next && !prevNext; prevNext = next;
            PrevKnightPressed = prev && !prevPrev; prevPrev = prev;
            bool left = Horizontal < -0.5f, right = Horizontal > 0.5f;
            LeftPressed = left && !prevLeft; prevLeft = left;
            RightPressed = right && !prevRight; prevRight = right;
        }

        static void ApplyTouch(Vector2 gui, ref float h, ref bool jump, ref bool run, ref bool down, ref bool start)
        {
            if (Hit(LeftButton, gui)) h -= 1f;
            if (Hit(RightButton, gui)) h += 1f;
            if (Hit(DownButton, gui)) down = true;
            if (Hit(RunButton, gui)) run = true;
            if (Hit(JumpButton, gui)) jump = true;
            if (Hit(PauseButton, gui)) start = true;
        }

        public static Rect ToScreen(Rect frac)
        {
            return new Rect(frac.x * Screen.width, frac.y * Screen.height, frac.width * Screen.width, frac.height * Screen.height);
        }

        static bool Hit(Rect frac, Vector2 gui) { return ToScreen(frac).Contains(gui); }
    }
}
