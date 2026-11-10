"""Touch controls in the tapestry style: a fixed joystick, a jump button (with the knight being played)
and a run/shoot button (crossbow). Each disc is centred in its square and fills 92% of it."""
from embroider import fill, stem, dot, WOOL
import scenery, knights, items

def disc(cx, cy, r, color, rim="dark", rimw=2.4):
    return fill(f"M{cx - r},{cy} a{r},{r} 0 1,0 {2 * r},0 a{r},{r} 0 1,0 {-2 * r},0 Z", color, 0, rim, rimw)

def joy_base():
    c, r = 90, 82
    s = disc(c, c, r, "cream") + stem(f"M{c - r},{c} a{r},{r} 0 1,0 {2 * r},0 a{r},{r} 0 1,0 {-2 * r},0", "terra", 3)
    s += f'<circle cx="{c}" cy="{c}" r="68" fill="none" stroke="{WOOL["gold"]}" stroke-width="1.6" stroke-dasharray="3 4"/>'
    for dx, dy, rot in ((0, -62, 0), (62, 0, 90), (0, 62, 180), (-62, 0, 270)):
        s += f'<g transform="translate({c + dx} {c + dy}) rotate({rot})">' + fill("M-8,5 L0,-7 L8,5 Z", "terra", 0, "dark", 1) + "</g>"
    return s

def joy_knob():
    c = 40
    s = disc(c, c, 36, "terra", "dark", 2.6) + f'<circle cx="{c}" cy="{c}" r="28" fill="none" stroke="{WOOL["gold"]}" stroke-width="2"/>'
    s += fill("M36,24 L44,24 L44,36 L56,36 L56,44 L44,44 L44,56 L36,56 L36,44 L24,44 L24,36 L36,36 Z", "gold", 0, "dark", 0.9)
    return s

def jump_button(key):
    def f():
        c, r = 70, 64
        s = disc(c, c, r, "gold", "dark", 2.8)
        s += f'<g transform="translate({c - 26} {c - 50}) scale(0.92)">' + knights.knight_body(knights.KNIGHTS[key], "jump") + "</g>"
        s += fill(f"M{c + 34},{c - 6} L{c + 44},{c - 22} L{c + 54},{c - 6} L{c + 48},{c - 6} L{c + 48},{c + 12} L{c + 40},{c + 12} L{c + 40},{c - 6} Z", "terra", 0, "dark", 1.1)
        return s
    return f

def run_button():
    c, r = 56, 50
    s = disc(c, c, r, "blue", "dark", 2.8)
    s += f'<circle cx="{c}" cy="{c}" r="{r - 7}" fill="none" stroke="{WOOL["gold"]}" stroke-width="2.2" stroke-dasharray="4 3"/>'
    s += f'<g transform="translate({c - 38} {c - 30}) scale(2.1)">' + items.crossbow(0) + "</g>"
    return s

def pause_button():
    c = 26
    return disc(c, c, 23, "cream") + fill("M17,16 L23,16 L23,36 L17,36 Z M29,16 L35,16 L35,36 L29,36 Z", "dark", 0, None)

scenery.S["ui_joy_base"] = (180, 180, joy_base, False)
scenery.S["ui_joy_knob"] = (80, 80, joy_knob, False)
scenery.S["ui_run"] = (112, 112, run_button, False)
scenery.S["ui_pause"] = (52, 52, pause_button, False)
for k in knights.KNIGHTS:
    scenery.S["ui_jump_" + k] = (140, 140, jump_button(k), False)
