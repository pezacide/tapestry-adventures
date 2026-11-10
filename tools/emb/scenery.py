"""Bayeux-style scenery: tiles (48px), props, borders and parallax layers."""
import math, random
from embroider import fill, mail, stem, dot, doc, g, rot, WOOL
from creatures import clip, scales

T = 48
S = {}   # name -> (w, h, body_fn, bg)

def wave(y0, amp, period, w):
    return f"M0,{y0} " + " ".join(f"q{period / 4},{-amp} {period / 2},0 q{period / 4},{amp} {period / 2},0" for _ in range(int(w / period) + 1))

# ---------------- tiles ----------------
def ground_top():
    s = fill(f"M0,10 L48,10 L48,48 L0,48 Z", "brown", 0, None)
    for i, y in enumerate((18, 26, 34, 42)):
        s += stem(f"M0,{y} q6,-2 12,0 q6,2 12,0 q6,-2 12,0 q6,2 12,0", ("ochre", "buff", "olive", "ochre")[i], 1.4, False)
    s += fill("M0,4 q6,-3 12,0 q6,3 12,0 q6,-3 12,0 q6,3 12,0 L48,14 q-6,3 -12,0 q-6,-3 -12,0 q-6,3 -12,0 q-6,-3 -12,0 Z", "green", 0, "dark", 1.4)
    s += stem("M0,9 q6,-3 12,0 q6,3 12,0 q6,-3 12,0 q6,3 12,0", "sage", 1, False)
    for x in (8, 30):   # little sprigs
        s += stem(f"M{x},4 q-2,-3 -4,-3 M{x},4 q0,-3 1,-4 M{x},4 q2,-2 4,-2", "green", 1.2, False)
    return s
S["ground_top"] = (T, T, ground_top, False)

def ground_fill():
    s = fill("M0,0 L48,0 L48,48 L0,48 Z", "brown", 0, None)
    for i, y in enumerate((4, 12, 20, 28, 36, 44)):
        s += stem(f"M0,{y} q6,-2 12,0 q6,2 12,0 q6,-2 12,0 q6,2 12,0", ("ochre", "buff", "olive")[i % 3], 1.4, False)
    return s
S["ground_fill"] = (T, T, ground_fill, False)

def masonry():
    cols = ["terra", "ochre", "sage", "blue", "buff"]
    s, k = "", 0
    for r, y in enumerate((0, 16, 32)):
        off = 12 if r % 2 else 0
        for x in range(-off, 48, 24):
            s += fill(f"M{x + 0.8},{y + 0.8} L{x + 23.2},{y + 0.8} L{x + 23.2},{y + 15.2} L{x + 0.8},{y + 15.2} Z", cols[(r * 2 + k) % 5], 0, "dark", 1.4)
            k += 1
    return clip("mc", "M0,0 L48,0 L48,48 L0,48 Z", s) + stem("M0.8,0.8 L47.2,0.8 L47.2,47.2 L0.8,47.2 Z", "dark", 1.6)
S["masonry"] = (T, T, masonry, False)

def roundel(used=False):
    def f():
        s = fill("M1,1 L47,1 L47,47 L1,47 Z", "buff" if used else "ochre", 0, "dark", 1.8)
        if used:
            return s + stem("M8,8 L40,8 L40,40 L8,40 Z", "brown", 1, True)
        s += fill("M24,5 a19,19 0 1,0 0.1,0 Z", "terra", 0, "dark", 1.6)
        s += fill("M24,10 a14,14 0 1,0 0.1,0 Z", "indigo", 0, "dark", 1.2)
        s += f'<path d="M24,7.5 a16.5,16.5 0 1,0 0.1,0 Z" fill="none" stroke="{WOOL["cream"]}" stroke-width="1.2" stroke-dasharray="0.1 3" stroke-linecap="round"/>'
        s += fill("M21.5,13 L26.5,13 L26.5,21.5 L35,21.5 L35,26.5 L26.5,26.5 L26.5,35 L21.5,35 L21.5,26.5 L13,26.5 L13,21.5 L21.5,21.5 Z", "gold", 0, "dark", 1)
        for x, y in ((4, 4), (44, 4), (4, 44), (44, 44)):
            s += dot(x, y, 1.5, "terra")
        return s
    return f
S["roundel_block"] = (T, T, roundel(False), False)
S["roundel_used"] = (T, T, roundel(True), False)

def bezant(i):
    def f():
        sx = (1, 0.6, 0.15, 0.6)[i]
        s = fill("M12,2.5 a9.5,9.5 0 1,0 0.1,0 Z", "gold", 90 if sx < 0.3 else 0, "dark", 1.4)
        if sx > 0.3:
            s += stem("M12,6 L12,18 M6,12 L18,12", "ochre", 1.6, False) + stem("M12,4.5 a7.5,7.5 0 1,0 0.1,0", "cream", 0.6, False)
        return g(s, f"translate(12 0) scale({sx} 1) translate(-12 0)")
    return f
for i in range(4):
    S[f"bezant_{i}"] = (24, 24, bezant(i), False)

def plank():
    s = fill("M1,2 L47,2 L47,16 L1,16 Z", "ochre", 0, "dark", 1.4)
    s += stem("M1,6 L47,6 M1,12 L47,12", "brown", 0.9, False)
    s += stem("M4,22 L10,16 M20,22 L26,16 M36,22 L42,16".replace("22", "16").replace("16", "9", 0), "terra", 0, False) if False else ""
    for x in (4, 16, 28, 40):
        s += stem(f"M{x},8 l3,2 l3,-2", "terra", 1, False)
    return s
S["plank"] = (T, 18, plank, False)

def tower_top():
    s = ""
    for x in (2, 26, 50, 74):
        s += fill(f"M{x},2 L{x + 18},2 L{x + 18},16 L{x},16 Z", ("terra", "ochre", "terra", "ochre")[x // 24 % 4], 0, "dark", 1.4)
    s += fill("M0,16 L96,16 L96,48 L0,48 Z", "sage", 0, "dark", 1.6)
    for x in (10, 38, 66):   # arcade of windows
        s += fill(f"M{x},44 L{x},30 Q{x + 10},20 {x + 20},30 L{x + 20},44 Z", "indigo", 90, "dark", 1.2)
    s += stem("M0,20 L96,20", "gold", 1.2, False)
    return s
S["tower_top"] = (96, 48, tower_top, False)

def tower_body():
    s = fill("M4,0 L92,0 L92,48 L4,48 Z", "buff", 90, "dark", 1.6)
    for x, c in ((4, "terra"), (36, "blue"), (60, "blue"), (82, "terra")):
        s += fill(f"M{x},0 L{x + 10},0 L{x + 10},48 L{x},48 Z", c, 90, "dark", 1.2)
    s += stem("M14,12 L36,12 M14,36 L36,36 M70,12 L82,12 M70,36 L82,36 M46,24 L60,24", "brown", 0.8, False)
    for y in (6, 18, 30, 42):
        s += dot(9, y, 1.1, "cream") + dot(87, y, 1.1, "cream")
    return s
S["tower_body"] = (96, 48, tower_body, False)

def sea():
    s = ""
    for i, y in enumerate((8, 20, 32, 44)):
        c = ("sky", "blue", "sky", "indigo")[i]
        s += fill(wave(y, 4, 24, 48).replace("M0,", "M-12," if i % 2 else "M0,") + " L60,60 L-12,60 Z", c, 0, "dark", 1.2)
    return clip("sea", "M0,0 L48,0 L48,48 L0,48 Z", s)
S["sea"] = (T, T, sea, False)

def palmette(cx, cy, ang, cols=("green", "sage", "terra"), size=1.0):
    s = ""
    for i, a in enumerate((-60, -30, 0, 30, 60)):
        L = (10, 13, 15, 13, 10)[i] * size
        d = f"M0,0 Q{-3 * size},{-L * 0.6} 0,{-L} Q{3 * size},{-L * 0.6} 0,0 Z"
        s += g(fill(d, cols[i % len(cols)], 90, "dark", 0.9), f"translate({cx} {cy}) rotate({ang + a})")
    return s

def tree():
    s = ""
    br = [("M46,96 Q28,86 24,66 Q22,50 34,44", "brown"), ("M50,96 Q68,84 72,64 Q74,50 62,44", "brown"),
          ("M48,92 Q36,70 52,52 Q62,40 50,26", "olive"), ("M48,92 Q60,70 44,52 Q34,40 46,22", "olive")]
    for d, c in br:
        s += stem(d, c, 5, False) + stem(d, "dark", 0.7, False)
    s += fill("M42,160 Q46,128 44,92 L53,92 Q51,128 57,160 Z", "brown", 90)
    s += stem("M44,100 Q38,94 34,96 M54,104 Q62,100 66,104", "brown", 3, False)
    s += palmette(34, 44, -40) + palmette(62, 44, 40) + palmette(50, 26, 10, ("sage", "green", "gold")) + palmette(46, 22, -10, ("green", "terra", "sage"))
    s += palmette(24, 66, -80, ("sage", "green")) + palmette(72, 64, 80, ("sage", "green"))
    s += palmette(32, 96, -100, ("terra", "gold"), 0.7) + palmette(66, 104, 100, ("terra", "gold"), 0.7)
    return s
S["tree"] = (96, 160, tree, False)

def shrub():
    s = stem("M32,48 Q30,36 20,30 Q12,26 14,18 M32,48 Q34,34 46,28 Q54,24 52,16 M32,48 L32,22", "green", 3, False)
    s += palmette(14, 18, -30, ("sage", "green"), 0.8) + palmette(52, 16, 30, ("sage", "green"), 0.8) + palmette(32, 22, 0, ("terra", "gold", "green"), 0.9)
    return s
S["shrub"] = (64, 48, shrub, False)

def gonfanon():
    s = stem("M8,10 L8,144", "brown", 3, False)
    s += fill("M6,2 L10,2 L10,12 L6,12 Z M3,5 L13,5 L13,8 L3,8 Z", "gold", 0, "dark", 0.9)
    flag = "M9.5,16 L46,16 L46,42 L40,36 L40,52 L33,44 L33,58 L26,48 L26,58 L9.5,58 Z"
    s += fill(flag, "white", 0)
    s += clip("gf", flag, stem("M9.5,16 L46,16 L46,58 L9.5,58", "blue", 6, False) + fill("M20,22 L25,22 L25,52 L20,52 Z M12,32 L34,32 L34,37 L12,37 Z", "terra", 0, "dark", 0.9))
    s += stem(flag, "dark", 1.4)
    return s
S["gonfanon"] = (48, 144, gonfanon, False)

def goal_pole():
    """The end-of-level pole, 8 tiles tall: stitched staff, gilt cross, gonfanon near the top, masonry plinth."""
    s = stem("M24,22 L24,358", "brown", 4, False) + stem("M25.5,24 L25.5,356", "ochre", 1, False)
    s += fill("M21,4 L27,4 L27,22 L21,22 Z M15,9 L33,9 L33,14 L15,14 Z", "gold", 0, "dark", 1)
    flag = "M26,28 L62,28 L62,56 L56,50 L56,68 L49,58 L49,74 L42,64 L42,74 L26,74 Z"
    s += fill(flag, "white", 0)
    s += clip("gp", flag, stem("M26,28 L62,28 L62,74 L26,74", "blue", 6, False) + fill("M36,34 L41,34 L41,68 L36,68 Z M28,46 L52,46 L52,51 L28,51 Z", "terra", 0, "dark", 0.9))
    s += stem(flag, "dark", 1.4)
    s += fill("M4,358 L44,358 L44,384 L4,384 Z", "terra", 0, "dark", 1.4) + stem("M4,371 L44,371 M24,358 L24,371 M14,371 L14,384 M34,371 L34,384", "dark", 1, False)
    return s
S["goal_pole"] = (64, 384, goal_pole, False)

def ship():
    s = stem("M76,64 L76,4", "brown", 3, False)
    sail = "M48,10 L104,10 Q108,34 102,56 L50,56 Q44,34 48,10 Z"
    s += fill(sail, "white", 90)
    s += clip("sail", sail, "".join(fill(f"M{x},0 L{x + 8},0 L{x + 8},60 L{x},60 Z", "terra", 90, None) for x in range(48, 108, 16)))
    s += stem(sail, "dark", 1.4) + stem("M44,10 L108,10", "brown", 2.4, False)
    hull = "M8,62 L152,62 Q146,84 120,88 L40,88 Q14,84 8,62 Z"
    s += fill(hull, "ochre", 0)
    s += clip("hull", hull, stem("M0,70 L160,70", "terra", 4, False) + stem("M0,78 L160,78", "blue", 4, False) + stem("M0,85 L160,85", "green", 3, False))
    s += stem(hull, "dark", 1.6)
    s += fill("M8,62 Q0,48 6,30 Q14,26 16,32 Q10,34 12,40 Q12,52 16,62 Z", "ochre", 80)   # stern
    s += fill("M152,62 Q160,46 154,28 Q148,22 142,26 L146,30 Q150,30 150,36 Q148,50 144,62 Z", "ochre", 100)  # dragon prow
    s += dot(147, 28, 1, "dark") + stem("M142,26 L136,24", "terra", 1, False)
    for x in (30, 50, 98, 118, 134):   # shields on the gunwale
        s += fill(f"M{x - 5},62 a5,5 0 1,1 10,0 Z", ("terra", "gold", "blue", "terra", "gold")[(x // 20) % 5], 0, "dark", 1)
    return s
S["ship"] = (160, 96, ship, False)

def hill():
    s = fill("M0,64 Q20,20 56,14 Q84,10 104,26 Q126,10 160,30 L160,64 Z", "sage", 0)
    s += clip("hl", "M0,64 Q20,20 56,14 Q84,10 104,26 Q126,10 160,30 L160,64 Z",
              "".join(stem(f"M0,{y} Q40,{y - 10} 80,{y} T160,{y}", ("green", "olive", "ochre")[i % 3], 1.4, False) for i, y in enumerate(range(24, 70, 7))))
    return s
S["hill"] = (160, 64, hill, False)

def hall():
    s = fill("M10,128 L10,60 L150,60 L150,128 Z", "buff", 90)
    roof = "M4,62 Q80,30 156,62 Z"
    s += fill(roof, "terra", 0) + clip("rf", roof, scales(0, 36, 160, 64, 7, "dark", 0.8))
    for x in (18, 52, 86, 120):
        s += fill(f"M{x},128 L{x},86 Q{x + 11},70 {x + 22},86 L{x + 22},128 Z", "indigo", 90, "dark", 1.2)
        s += fill(f"M{x - 4},128 L{x - 4},70 L{x},70 L{x},128 Z", ("blue", "green", "terra", "blue")[x // 34 % 4], 90, "dark", 1)
    s += fill("M146,128 L146,70 L150,70 L150,128 Z", "blue", 90, "dark", 1)
    s += fill("M60,36 L100,36 L100,62 L60,62 Z", "sage", 90) + fill("M56,38 Q80,10 104,38 Z", "terra", 0) + clip("rt", "M56,38 Q80,10 104,38 Z", scales(50, 14, 110, 40, 6, "dark", 0.8))
    s += stem("M80,22 L80,4 M75,9 L85,9", "gold", 1.6, False)
    return s
S["hall"] = (160, 128, hall, False)

def motte():   # castle on a mound, as at Dol, Rennes and Dinan
    s = fill("M0,128 Q30,70 64,66 Q98,70 128,128 Z", "green", 0)
    s += clip("mt", "M0,128 Q30,70 64,66 Q98,70 128,128 Z", "".join(stem(f"M0,{y} Q64,{y - 40} 128,{y}", ("ochre", "sage", "terra")[i % 3], 2, False) for i, y in enumerate(range(80, 160, 9))))
    s += fill("M40,68 L40,30 L88,30 L88,68 Z", "blue", 90)
    s += fill("M34,32 Q64,0 94,32 Z", "terra", 0) + clip("mr", "M34,32 Q64,0 94,32 Z", scales(30, 4, 100, 34, 6, "dark", 0.8))
    s += fill("M58,68 L58,50 Q64,42 70,50 L70,68 Z", "dark", 0, "dark", 1)
    s += stem("M8,124 L52,70", "brown", 3, False) + stem("M14,116 L22,118 M22,106 L30,108 M30,96 L38,98 M38,86 L46,88", "brown", 1.4, False)
    return s
S["motte"] = (128, 128, motte, False)

# ---------------- forest (World 1-2) ----------------
def canopy():
    """A treetop you can stand on (and jump up through), 48 wide, tiles horizontally."""
    s = ""
    for i, cx in enumerate((-6, 12, 30, 48)):
        d = f"M{cx - 12},20 Q{cx - 13},6 {cx},4 Q{cx + 13},6 {cx + 12},20 Z"
        s += fill(d, ("green", "sage", "green", "sage")[i], 60 + i * 30, "dark", 1.2)
    s += fill("M0,14 L48,14 L48,24 Q24,27 0,24 Z", "green", 0, None)
    s += stem("M0,23 Q24,27 48,23", "dark", 1.4, False)
    for x, y in ((6, 10), (20, 8), (34, 11), (42, 7), (14, 16), (28, 17)):
        s += dot(x, y, 1.3, ("terra", "gold")[(x // 7) % 2])
    return clip("cn", "M0,0 L48,0 L48,30 L0,30 Z", s)
S["canopy"] = (48, 26, canopy, False)

def trunk():
    s = fill("M16,0 L32,0 L32,48 L16,48 Z", "brown", 90, None)
    s += stem("M16,0 L16,48 M32,0 L32,48", "dark", 1.4, False)
    s += stem("M21,4 Q19,14 22,24 M27,22 Q29,32 26,44 M23,30 L23,38", "olive", 1, False)
    return s
S["trunk"] = (48, 48, trunk, False)

def forest_tree(cols, scale=1.0, trunk_c="brown"):
    """A Bayeux tree in other colours, for the forest backdrops."""
    s = ""
    for d, c in (("M46,96 Q28,86 24,66 Q22,50 34,44", trunk_c), ("M50,96 Q68,84 72,64 Q74,50 62,44", trunk_c),
                 ("M48,92 Q36,70 52,52 Q62,40 50,26", "olive"), ("M48,92 Q60,70 44,52 Q34,40 46,22", "olive")):
        s += stem(d, c, 5, False) + stem(d, "dark", 0.7, False)
    s += fill("M42,160 Q46,128 44,92 L53,92 Q51,128 57,160 Z", trunk_c, 90)
    s += palmette(34, 44, -40, cols) + palmette(62, 44, 40, cols) + palmette(50, 26, 10, cols[::-1]) + palmette(46, 22, -10, cols)
    s += palmette(24, 66, -80, cols) + palmette(72, 64, 80, cols)
    return g(s, f"scale({scale})")

def forest_far():
    items = []
    for i, x in enumerate(range(0, 960, 80)):
        items.append((x, g(underdraw(forest_tree(("green", "sage"), 1.0)), f"translate(0 {250 + (i % 3) * 25}) scale(1.6)")))
    return _wrap(items)

def forest_near():
    items = []
    palettes = (("green", "sage", "terra"), ("sage", "green", "gold"), ("green", "olive", "sage"))
    for i, x in enumerate(range(10, 960, 150)):
        items.append((x, g(forest_tree(palettes[i % 3], 1.0), f"translate(0 {330 + (i % 2) * 20}) scale(1.35)")))
    for x in range(70, 960, 150):
        items.append((x, g(shrub(), "translate(0 440) scale(1.2)")))
    return _wrap(items)

# ---------------- halfway banner + health bar ----------------
def checkpoint(claimed):
    def f():
        s = stem("M10,10 L10,92", "brown", 2.4, False) + dot(10, 7, 3, "gold", "dark")
        flag = "M11,12 L40,16 L34,24 L40,32 L11,36 Z"
        s += fill(flag, "terra" if claimed else "buff", 0)
        if claimed:
            s += fill("M19,19 L23,19 L23,23 L27,23 L27,27 L23,27 L23,31 L19,31 L19,27 L15,27 L15,23 L19,23 Z", "gold", 0, "dark", 0.7)
        s += stem(flag, "dark", 1.3)
        s += fill("M2,90 L18,90 L18,96 L2,96 Z", "grey", 0, "dark", 1)
        return s
    return f
S["checkpoint_0"] = (44, 96, checkpoint(False), False)
S["checkpoint_1"] = (44, 96, checkpoint(True), False)

def health(full):
    """The health bar: a long strip of tapestry with fringed ends; the full one is embroidered."""
    def f():
        w, h = 240, 30
        band = f"M8,4 L{w - 8},4 L{w - 8},{h - 4} L8,{h - 4} Z"
        s = ""
        for x in list(range(2, 9, 3)) + list(range(w - 8, w - 1, 3)):   # fringe
            s += stem(f"M{x},6 L{x},{h - 6}", "buff", 1.2, False)
        if full:
            s += fill(band, "terra", 0, None)
            s += clip("hb", band, stem(f"M8,7 L{w - 8},7 M8,{h - 7} L{w - 8},{h - 7}", "gold", 1.4, False)
                      + "".join(fill(f"M{x},{h / 2} L{x + 7},8.5 L{x + 14},{h / 2} L{x + 7},{h - 8.5} Z", ("gold", "blue")[(x // 20) % 2], 0, "dark", 0.8) for x in range(12, w - 16, 20)))
        else:
            s += fill(band, "cream", 0, None)
            s += clip("hbe", band, "".join(stem(f"M{x},6 L{x + 10},{h - 6}", "buff", 0.7, False) for x in range(8, w, 8)))
        s += stem(band, "dark", 1.6)
        return s
    return f
S["health_empty"] = (240, 30, health(False), False)
S["health_full"] = (240, 30, health(True), False)

def aura():
    """The holy relique's glory: a ring of golden rays (drawn behind the knight)."""
    import math as _m
    cx, cy = 64, 76
    s = ""
    for i in range(16):
        a = i * 2 * _m.pi / 16
        L = 62 if i % 2 == 0 else 46
        x1, y1 = cx + _m.cos(a) * L, cy + _m.sin(a) * L
        xa, ya = cx + _m.cos(a + 0.22) * 18, cy + _m.sin(a + 0.22) * 18
        xb, yb = cx + _m.cos(a - 0.22) * 18, cy + _m.sin(a - 0.22) * 18
        s += fill(f"M{xa:.1f},{ya:.1f} L{x1:.1f},{y1:.1f} L{xb:.1f},{yb:.1f} Z", ("gold", "cream")[i % 2], 0, "ochre", 0.8)
    s += f'<ellipse cx="{cx}" cy="{cy}" rx="34" ry="44" fill="{WOOL["cream"]}" opacity="0.55"/>'
    s += f'<ellipse cx="{cx}" cy="{cy}" rx="38" ry="48" fill="none" stroke="{WOOL["gold"]}" stroke-width="3"/>'
    return s
S["aura"] = (128, 152, aura, False)

# ---------------- the shore (World 2) ----------------
def sand_top():
    s = fill("M0,8 L48,8 L48,48 L0,48 Z", "buff", 0, None)
    for i, y in enumerate((18, 28, 38)):
        s += stem(f"M0,{y} q6,-2 12,0 q6,2 12,0 q6,-2 12,0 q6,2 12,0", ("ochre", "cream", "ochre")[i], 1.2, False)
    s += fill("M0,4 q6,-3 12,0 q6,3 12,0 q6,-3 12,0 q6,3 12,0 L48,11 q-6,2 -12,0 q-6,-2 -12,0 q-6,2 -12,0 q-6,-2 -12,0 Z", "cream", 0, "dark", 1.2)
    s += dot(9, 14, 1.4, "rose") + dot(33, 22, 1.2, "white") + stem("M40,13 l3,-2 l3,2", "terra", 0.9, False)   # shells
    return s
S["sand_top"] = (T, T, sand_top, False)

def sand_fill():
    s = fill("M0,0 L48,0 L48,48 L0,48 Z", "buff", 0, None)
    for i, y in enumerate((6, 16, 26, 36, 46)):
        s += stem(f"M0,{y} q6,-2 12,0 q6,2 12,0 q6,-2 12,0 q6,2 12,0", ("ochre", "cream")[i % 2], 1.2, False)
    return s
S["sand_fill"] = (T, T, sand_fill, False)

def timber():
    """A timber crate from the shipwrights' yard: breaks like a brick."""
    s = fill("M1,1 L47,1 L47,47 L1,47 Z", "ochre", 0, "dark", 1.6)
    s += stem("M1,12.5 L47,12.5 M1,24 L47,24 M1,35.5 L47,35.5", "brown", 1, False)
    s += stem("M4,4 L44,44 M44,4 L4,44", "brown", 3, False) + stem("M4,4 L44,44 M44,4 L4,44", "dark", 0.6, False)
    for x, y in ((5, 5), (43, 5), (5, 43), (43, 43)): s += dot(x, y, 1.2, "grey", "dark")
    return s
S["timber"] = (T, T, timber, False)

def log_roll():
    """A rolling log, seen end-on (the game spins it as it rolls)."""
    s = fill("M24,2 a22,22 0 1,0 0.1,0 Z", "brown", 30, "dark", 1.8)
    s += fill("M24,7 a17,17 0 1,0 0.1,0 Z", "buff", 0, "dark", 1)
    for r in (12, 7, 3): s += stem(f"M{24 - r},24 a{r},{r} 0 1,0 {2 * r},0 a{r},{r} 0 1,0 {-2 * r},0", "ochre", 1, False)
    s += stem("M24,10 L24,20", "brown", 1.2, False)   # a crack, so the spin shows
    return s
S["log"] = (T, T, log_roll, False)

def log_pile():
    s = ""
    for i, (cx, cy) in enumerate(((14, 70), (40, 70), (66, 70), (27, 48), (53, 48), (40, 26))):
        s += fill(f"M{cx},{cy - 12} a12,12 0 1,0 0.1,0 Z", "brown", 30, "dark", 1.3) + fill(f"M{cx},{cy - 8} a8,8 0 1,0 0.1,0 Z", "buff", 0, "dark", 0.8)
        s += stem(f"M{cx - 4},{cy} a4,4 0 1,0 8,0", "ochre", 0.8, False)
    s += stem("M2,82 L78,82", "brown", 3, False) + stem("M6,82 L2,94 M74,82 L78,94", "brown", 2.4, False)
    return s
S["log_pile"] = (80, 96, log_pile, False)

def shore_far():
    """The sea, with the fleet of longships sailing on it."""
    s = ""
    for i, y in enumerate(range(350, BH + 20, 22)):
        c = ("sky", "blue", "sky", "indigo")[i % 4]
        s += fill(f"M0,{y} " + " ".join(f"q12,{-6 if (j + i) % 2 else -3} 24,0" for j in range(42)) + f" L{BW + 20},{BH} L0,{BH} Z", c, 0, "dark", 1)
    for x, y in ((80, 290), (380, 300), (680, 286)):
        s += g(underdraw(ship()) .replace('opacity="0.33"', 'opacity="0.55"'), f"translate({x} {y}) scale(1.0)")
    return s

def shore_near():
    items = []
    for x in range(40, BW, 320):
        items.append((x, g(log_pile(), "translate(0 430) scale(1.0)")))
    for x in range(200, BW, 320):   # stumps of the felled trees
        st = fill("M0,500 L0,476 Q14,468 28,476 L28,500 Z", "brown", 90, "dark", 1.2) + fill("M0,476 Q14,468 28,476 Q14,482 0,476 Z", "buff", 0, "dark", 1)
        items.append((x, st))
    return _wrap(items)

def spout():
    """The whale's spout: a column of water that hurls a knight skyward."""
    s = ""
    for i, (w, c) in enumerate(((18, "sky"), (12, "white"), (6, "cream"))):
        d = f"M{24 - w},192 Q{24 - w * 1.2},100 {24 - w * 0.7},30 Q24,{14 + i * 4} {24 + w * 0.7},30 Q{24 + w * 1.2},100 {24 + w},192 Z"
        s += fill(d, c, 90, "blue" if i == 0 else None, 1.2)
    for x, y in ((6, 24), (40, 30), (14, 10), (34, 8), (24, 2)):
        s += dot(x, y, 3, "white", "blue")
    return s
S["spout"] = (48, 192, spout, False)

def manicule():
    """A manicule: the pointing hand scribes drew in the margins to say 'look here' (points right)."""
    s = fill("M4,22 L28,22 L30,40 L4,40 Z", "terra", 90, "dark", 1.4)                 # the sleeve
    s += stem("M4,26 L28,26 M4,36 L28,36", "gold", 1.4, False)
    s += fill("M28,20 Q36,14 44,18 L82,18 Q88,22 82,26 L52,26 L54,30 Q56,36 50,38 L54,42 Q54,48 46,48 L36,48 Q28,46 28,40 Z", "buff", 0, "dark", 1.6)
    s += stem("M52,26 L44,28 M50,38 L42,38 M46,48 L40,46", "brown", 1, False)          # the curled fingers
    s += stem("M60,18 L60,26 M72,18 L72,26", "brown", 0.8, False)                     # the knuckles of the pointing finger
    return s
S["manicule"] = (90, 56, manicule, False)

# ---------------- the Tapestry of Creation (World 3) ----------------
def cloud_top():
    """Ground made of Romanesque cloud: scalloped bands of blue and white."""
    s = fill("M0,12 L48,12 L48,48 L0,48 Z", "sky", 0, None)
    for i, y in enumerate((22, 32, 42)):
        s += stem(f"M0,{y} q6,-5 12,0 q6,-5 12,0 q6,-5 12,0 q6,-5 12,0", ("white", "blue", "white")[i], 1.6, False)
    s += fill("M0,14 q6,-12 12,-2 q6,-12 12,0 q6,-12 12,-2 q6,-12 12,0 L48,16 L0,16 Z", "white", 0, "dark", 1.3)
    return s
S["cloud_top"] = (T, T, cloud_top, False)

def cloud_fill():
    s = fill("M0,0 L48,0 L48,48 L0,48 Z", "sky", 0, None)
    for i, y in enumerate((8, 18, 28, 38, 48)):
        s += stem(f"M0,{y} q6,-5 12,0 q6,-5 12,0 q6,-5 12,0 q6,-5 12,0", ("white", "blue")[i % 2], 1.6, False)
    return s
S["cloud_fill"] = (T, T, cloud_fill, False)

def cloud_band():
    """A thin cloud you can stand on and jump up through (the sky's plank)."""
    s = fill("M0,14 q6,-12 12,-2 q6,-12 12,0 q6,-12 12,-2 q6,-12 12,0 L48,20 Q24,24 0,20 Z", "white", 0, "dark", 1.3)
    s += stem("M2,15 q6,-4 11,0 q6,-4 11,0 q6,-4 11,0 q6,-4 11,0", "sky", 1.4, False)
    return s
S["cloud_band"] = (T, 24, cloud_band, False)

def star_board():
    """A gliding board of gold stars, 3 tiles wide (the sky's lift)."""
    s = fill("M2,6 L142,6 L142,18 L2,18 Z", "indigo", 0, "dark", 1.4)
    import math as _m
    for cx in range(14, 140, 24):
        pts = " ".join(f"{cx + _m.cos(a) * (8 if i % 2 == 0 else 3.5):.1f},{12 + _m.sin(a) * (8 if i % 2 == 0 else 3.5):.1f}" for i, a in enumerate([_m.pi / 2 + k * _m.pi / 5 for k in range(10)]))
        s += f'<polygon points="{pts}" fill="{WOOL["gold"]}" stroke="{WOOL["dark"]}" stroke-width="0.8"/>'
    return s
S["star_board"] = (144, 22, star_board, False)

def wind_head():
    """One of the Winds, as on the Girona tapestry: a puffed-cheeked face blowing, in a green roundel. Blows to the right."""
    s = fill("M48,4 a44,44 0 1,0 0.1,0 Z", "green", 0, "dark", 2)
    s += f'<circle cx="48" cy="48" r="38" fill="none" stroke="{WOOL["gold"]}" stroke-width="2" stroke-dasharray="4 3"/>'
    for i, a in enumerate(range(-150, -20, 20)):   # wild hair
        import math as _m
        r1, r2 = 26, 38
        s += stem(f"M{48 + _m.cos(_m.radians(a)) * r1:.1f},{46 + _m.sin(_m.radians(a)) * r1:.1f} Q{48 + _m.cos(_m.radians(a + 10)) * 34:.1f},{46 + _m.sin(_m.radians(a + 10)) * 34:.1f} {48 + _m.cos(_m.radians(a)) * r2:.1f},{46 + _m.sin(_m.radians(a)) * r2:.1f}", ("ochre", "blue")[i % 2], 3, False)
    s += fill("M28,48 Q26,26 48,24 Q70,26 68,48 Q68,70 48,72 Q28,70 28,48 Z", "buff", 0)
    s += fill("M58,52 a8,8 0 1,0 0.1,0 Z", "rose", 0, None) + fill("M30,52 a7,7 0 1,0 0.1,0 Z", "rose", 0, None)   # puffed cheeks
    s += dot(40, 42, 1.4, "dark") + dot(56, 42, 1.4, "dark") + stem("M36,38 L44,40 M52,40 L60,38", "brown", 1.2, False)
    s += fill("M64,58 Q72,54 76,58 Q72,62 64,60 Z", "terra", 0, "dark", 1)                      # pursed lips
    s += stem("M78,52 Q86,50 92,54 M78,60 Q88,62 94,58 M78,56 L95,56", "white", 1.8, False)      # his breath
    return s
S["wind_head"] = (96, 96, wind_head, False)

def creation_far():
    """The great wheel of creation: concentric bands and roundels of sun, moon and stars."""
    import math as _m
    s = ""
    cx, cy = BW / 2, BH + 260
    for i, r in enumerate((720, 620, 520)):
        s += stem(f"M{cx - r},{cy} A{r},{r} 0 0,1 {cx + r},{cy}", ("ochre", "green", "terra")[i], 10, False)
        s += stem(f"M{cx - r},{cy} A{r},{r} 0 0,1 {cx + r},{cy}", "dark", 0.8, False)
    for k in range(9):   # spokes
        a = _m.pi * (0.1 + k * 0.1)
        s += stem(f"M{cx + _m.cos(a) * 520:.0f},{cy - _m.sin(a) * 520:.0f} L{cx + _m.cos(a) * 720:.0f},{cy - _m.sin(a) * 720:.0f}", "brown", 3, False)
    for k in range(8):   # roundels between the spokes: sun, moon and stars in turn
        a = _m.pi * (0.15 + k * 0.1)
        x, y = cx + _m.cos(a) * 670, cy - _m.sin(a) * 670
        s += fill(f"M{x - 36:.0f},{y:.0f} a36,36 0 1,0 72,0 a36,36 0 1,0 -72,0 Z", ("blue", "indigo")[k % 2], 0, "dark", 1.4)
        if k % 3 == 0: s += fill(f"M{x - 18:.0f},{y:.0f} a18,18 0 1,0 36,0 a18,18 0 1,0 -36,0 Z", "gold", 0, "dark", 1)
        elif k % 3 == 1: s += fill(f"M{x - 16:.0f},{y - 8:.0f} a18,18 0 1,0 30,22 a14,14 0 1,1 -30,-22 Z", "cream", 0, "dark", 1)
        else:
            for dx, dy in ((-14, -10), (12, -12), (0, 10), (-10, 14), (16, 8)): s += dot(x + dx, y + dy, 3, "gold")
    return f'<g opacity="0.55">{s}</g>'

def creation_near():
    items = []
    for x in range(60, BW, 200):
        st = "".join(dot(x2, y2, 2.4, "gold") for x2, y2 in ((0, 120), (30, 80), (60, 140), (90, 60), (120, 110)))
        items.append((x, st))
    return _wrap(items)

# ---------------- the crossing (World 2-3) ----------------
def longship():
    """A Norman longship to stand on: 6 tiles wide, its deck 58 units below the top of the drawing."""
    s = stem("M150,58 L150,6", "brown", 3.4, False)
    sail = "M112,10 L188,10 Q194,32 186,52 L114,52 Q106,32 112,10 Z"
    s += fill(sail, "white", 90) + clip("lss", sail, "".join(fill(f"M{x},0 L{x + 9},0 L{x + 9},60 L{x},60 Z", "terra", 90, None) for x in range(112, 192, 18)))
    s += stem(sail, "dark", 1.3) + stem("M106,10 L194,10", "brown", 2.4, False)
    hull = "M14,58 L274,58 Q266,92 228,104 L60,104 Q22,92 14,58 Z"
    s += fill(hull, "ochre", 0)
    s += clip("lsh", hull, stem("M0,70 L288,70", "terra", 5, False) + stem("M0,82 L288,82", "blue", 5, False) + stem("M0,94 L288,94", "green", 4, False))
    s += stem(hull, "dark", 1.8) + stem("M14,58 L274,58", "brown", 2.6, False)
    for i, x in enumerate(range(40, 260, 26)):   # shields along the gunwale
        s += fill(f"M{x - 7},58 a7,7 0 1,1 14,0 Z", ("terra", "gold", "blue", "green")[i % 4], 0, "dark", 1)
    s += fill("M14,58 Q2,40 10,18 Q20,14 22,22 Q14,26 16,34 Q16,48 22,58 Z", "ochre", 80)      # stern
    s += fill("M274,58 Q288,36 280,14 Q272,8 264,12 L268,18 Q274,18 274,24 Q272,44 266,58 Z", "ochre", 100)   # dragon prow
    s += dot(272, 15, 1.2, "dark") + stem("M264,12 L256,9", "terra", 1.2, False)
    s += stem("M0,108 " + " ".join(f"q12,-5 24,0" for _ in range(12)), "white", 1.6, False)   # bow wave at the waterline
    return s
S["longship"] = (288, 110, longship, False)

def gust():
    """A gust of wind: stitched curls, like the breath of the winds in old maps."""
    s = stem("M4,28 Q40,14 80,26 T156,22 Q176,20 182,10 Q184,2 176,4 Q170,8 176,14", "blue", 3.4, False)
    s += stem("M30,40 Q70,32 110,40 T180,36", "sky", 2.4, False) + stem("M60,14 Q90,6 120,12", "blue", 2, False)
    return s
S["gust"] = (192, 48, gust, False)

def sea_waves_near():
    """Foreground wave crests along the bottom of the screen."""
    items = []
    for x in range(0, BW, 96):
        w = stem("M0,520 q24,-14 48,0 q24,14 48,0", "white", 2.4, False) + stem("M10,530 q20,-8 40,0", "sky", 2, False)
        items.append((x, w))
    return _wrap(items)

# ---------------- beneath the waves (World 2-2) ----------------
def searock():
    """Unbreakable sea rock, grown over with weed."""
    s = fill("M0,0 L48,0 L48,48 L0,48 Z", "grey", 40, None)
    for i, (x, y, r) in enumerate(((10, 12, 9), (34, 10, 10), (22, 32, 12), (42, 38, 7), (6, 40, 7))):
        s += fill(f"M{x - r},{y} a{r},{r} 0 1,0 {2 * r},0 a{r},{r} 0 1,0 {-2 * r},0 Z", ("grey", "brown", "grey", "olive", "brown")[i], 30 * i, "dark", 1)
    s += stem("M4,2 q4,6 0,12 M28,0 q-4,6 0,10 M44,4 q4,6 0,12", "green", 1.6, False)
    return clip("sr", "M0,0 L48,0 L48,48 L0,48 Z", s) + stem("M0.8,0.8 L47.2,0.8 L47.2,47.2 L0.8,47.2 Z", "dark", 1.4)
S["searock"] = (T, T, searock, False)

def seaweed():
    s = ""
    for i, x in enumerate((12, 24, 36)):
        h = (80, 92, 72)[i]
        d = f"M{x},96 q-8,-{h * 0.25} 0,-{h * 0.5} q8,-{h * 0.25} 0,-{h * 0.5}"
        s += stem(d, ("green", "sage", "olive")[i], 4, False) + stem(d, "dark", 0.6, False)
        for k in range(3):
            yy = 96 - h * (0.25 + k * 0.25)
            s += fill(f"M{x},{yy} q{(-1) ** k * 10},-4 {(-1) ** k * 12},-12 q{(-1) ** k * -6},6 {(-1) ** k * -12},12 Z", ("green", "sage")[k % 2], 30, "dark", 0.8)
    return s
S["seaweed"] = (48, 96, seaweed, False)

def deep_far():
    """Deep water: bands of blue, shafts of light from the surface, far rocks."""
    s = ""
    for i, y in enumerate(range(0, BH + 40, 40)):
        s += fill(f"M0,{y} L{BW},{y} L{BW},{y + 40} L0,{y + 40} Z", ("sky", "blue", "blue", "indigo")[min(3, i // 4)], 0, None)
    s += stem(f"M0,40 " + " ".join(f"q12,-6 24,0" for _ in range(41)), "white", 2, False)   # the surface, seen from below
    for x in range(40, BW, 180):
        s += f'<path d="M{x},40 L{x + 60},40 L{x + 140},{BH} L{x + 70},{BH} Z" fill="{WOOL["cream"]}" opacity="0.10"/>'
    for x in range(0, BW, 240):
        s += fill(f"M{x},{BH} Q{x + 40},{BH - 120} {x + 110},{BH - 90} Q{x + 160},{BH - 160} {x + 220},{BH} Z", "indigo", 20, "dark", 1)
    return s

def deep_near():
    items = []
    for x in range(30, BW, 160):
        items.append((x, g(seaweed(), "translate(0 420) scale(1.2)")))
    return _wrap(items)

# ---------------- castle (World 1-4) ----------------
def fire_pit(f):
    def fn():
        s = fill("M0,30 L48,30 L48,48 L0,48 Z", "dark", 0, None)
        for i, x in enumerate((4, 16, 28, 40)):   # glowing coals
            s += fill(f"M{x - 5},48 Q{x - 6},36 {x},34 Q{x + 6},36 {x + 5},48 Z", ("terra", "ochre")[(i + f) % 2], 0, "dark", 0.9)
        for i, x in enumerate((6, 18, 30, 42)):   # flames licking up
            h = (22, 30, 18, 28)[(i + f * 2) % 4]
            s += fill(f"M{x - 6},34 Q{x - 7},{34 - h * 0.6} {x + (2 if f else -2)},{34 - h} Q{x + 7},{34 - h * 0.6} {x + 6},34 Z", ("terra", "ochre", "terra", "gold")[(i + f) % 4], 90, "dark", 0.9)
            s += fill(f"M{x - 2.5},34 Q{x - 3},{34 - h * 0.4} {x},{34 - h * 0.6} Q{x + 3},{34 - h * 0.4} {x + 2.5},34 Z", "gold", 90, None)
        return s
    return fn
S["fire_0"] = (T, T, fire_pit(0), False)
S["fire_1"] = (T, T, fire_pit(1), False)

def drawbridge():
    s = fill("M0,2 L48,2 L48,22 L0,22 Z", "brown", 0, "dark", 1.4)
    s += stem("M0,8 L48,8 M0,15 L48,15", "ochre", 1, False) + stem("M16,2 L16,22 M32,2 L32,22", "dark", 0.9, False)
    s += dot(8, 5, 1.2, "grey", "dark") + dot(40, 18, 1.2, "grey", "dark")
    s += stem("M0,24 L48,24", "grey", 2.4, False)   # the chain along its edge
    return s
S["drawbridge"] = (T, 26, drawbridge, False)

def bridge_axe():
    s = stem("M24,24 L24,92", "brown", 4, False) + stem("M25.5,26 L25.5,90", "ochre", 1, False)
    s += fill("M24,10 Q44,6 46,28 Q44,46 24,40 Z", "grey", 60, "dark", 1.4)
    s += fill("M24,10 Q4,6 2,28 Q4,46 24,40 Z", "grey", 120, "dark", 1.4)
    s += stem("M24,10 Q44,6 46,28 Q44,46 24,40 M24,10 Q4,6 2,28 Q4,46 24,40", "white", 0.8, False)
    s += fill("M20,4 L28,4 L28,12 L20,12 Z", "gold", 0, "dark", 1)
    return s
S["bridge_axe"] = (T, 96, bridge_axe, False)

def flail_ball():
    s = ""
    import math as _m
    for i in range(8):
        a = i * _m.pi / 4
        s += fill(f"M{12 + _m.cos(a - 0.3) * 7:.1f},{12 + _m.sin(a - 0.3) * 7:.1f} L{12 + _m.cos(a) * 12:.1f},{12 + _m.sin(a) * 12:.1f} L{12 + _m.cos(a + 0.3) * 7:.1f},{12 + _m.sin(a + 0.3) * 7:.1f} Z", "grey", 0, "dark", 0.8)
    s += fill("M12,4 a8,8 0 1,0 0.1,0 Z", "grey", 45, "dark", 1.3) + stem("M8,9 Q11,6 14,7", "white", 0.9, False)
    return s
S["flail_ball"] = (24, 24, flail_ball, False)

def chain_link():
    return fill("M6,2 a4,4 0 1,0 0.1,0 Z", "grey", 0, "dark", 1.1) + dot(6, 6, 1.4, "dark")
S["chain_link"] = (12, 12, chain_link, False)

def castle_far():
    """The great hall's walls, hung with tapestries - a tapestry within the tapestry."""
    s = fill(f"M0,0 L{BW},0 L{BW},{BH} L0,{BH} Z", "grey", 0, None)
    lines = []
    for r, y in enumerate(range(20, BH, 28)):
        lines.append(f"M0,{y} L{BW},{y}")
        for x in range(14 * (r % 2), BW, 56): lines.append(f"M{x},{y - 27} L{x},{y - 1}")
    s += stem(" ".join(lines), "dark", 0.7, False)
    for i, x in enumerate(range(60, BW, 240)):
        hang = f"M{x},120 L{x + 110},120 L{x + 110},330 L{x + 55},350 L{x},330 Z"
        s += fill(hang, ("terra", "blue", "green", "terra")[i % 4], 90)
        s += clip(f"tp{x}", hang, stem(f"M{x},132 L{x + 110},132 M{x},318 L{x + 110},318", "gold", 2, False))
        s += beast(x + 34, 220, "ochre", "dark") if i % 2 == 0 else bird(x + 40, 225, "gold", "cream")
        s += stem(hang, "dark", 1.6) + stem(f"M{x - 8},118 L{x + 118},118", "brown", 4, False)
        s += fill(f"M{x + 150},180 L{x + 166},180 L{x + 166},250 L{x + 150},250 Z", "dark", 0, "dark", 1)   # arrow slit
    return s

def castle_near():
    items = []
    for x in range(120, BW, 240):   # wall torches
        t = fill("M-5,330 L5,330 L3,360 L-3,360 Z", "brown", 90, "dark", 1)
        t += fill("M-8,330 Q-9,312 0,300 Q9,312 8,330 Z", "terra", 90, "dark", 1) + fill("M-4,330 Q-4,318 0,310 Q4,318 4,330 Z", "gold", 90, None)
        items.append((x, t))
    return _wrap(items)

# ---------------- undercroft (World 1-3) ----------------
def vault():
    """Unbreakable vault stone: dark dressed blocks."""
    s = ""
    for r, y in enumerate((0, 24)):
        off = 12 if r else 0
        for x in range(-off, 48, 24):
            s += fill(f"M{x + 0.8},{y + 0.8} L{x + 23.2},{y + 0.8} L{x + 23.2},{y + 23.2} L{x + 0.8},{y + 23.2} Z", ("grey", "blue")[(x // 24 + r) % 2], 0, "dark", 1.4)
    return clip("vc", "M0,0 L48,0 L48,48 L0,48 Z", s) + stem("M0.8,0.8 L47.2,0.8 L47.2,47.2 L0.8,47.2 Z", "dark", 1.6)
S["vault"] = (T, T, vault, False)

def lift():
    """A hanging board that moves: 3 tiles wide, slung on ropes."""
    s = stem("M12,0 L12,8 M132,0 L132,8", "brown", 1.6, False)
    s += fill("M2,8 L142,8 L142,20 L2,20 Z", "ochre", 0, "dark", 1.5)
    s += stem("M2,13 L142,13", "brown", 0.9, False)
    for x in range(10, 140, 24):
        s += stem(f"M{x},10 l4,2 l4,-2", "terra", 1, False)
    return s
S["lift"] = (144, 22, lift, False)

def crypt_far():
    """An opaque wall of vaulted arcades: it hides the linen, as a crypt would."""
    s = fill(f"M0,0 L{BW},0 L{BW},{BH} L0,{BH} Z", "grey", 0, None)
    for x in range(0, BW, 120):
        s += fill(f"M{x + 14},{BH} L{x + 14},250 Q{x + 60},150 {x + 106},250 L{x + 106},{BH} Z", "indigo", 90, "dark", 1.4)
        s += fill(f"M{x - 8},{BH} L{x - 8},240 L{x + 14},240 L{x + 14},{BH} Z", ("sage", "terra")[(x // 120) % 2], 90, "dark", 1.2)
        s += stem(f"M{x + 10},252 Q{x + 60},144 {x + 110},252", "buff", 2, False)
    for y in range(30, 230, 26):
        s += stem(f"M0,{y} L{BW},{y}", "dark", 0.6, False)
    return s

def crypt_near():
    """Hanging lamps between the arches."""
    items = []
    for x in range(60, BW, 240):
        lamp = stem("M0,0 L0,150", "brown", 1.4, False) + fill("M-9,150 L9,150 L6,168 L-6,168 Z", "gold", 0, "dark", 1.1)
        lamp += fill("M-3,150 Q0,136 3,150 Z", "terra", 0, None)
        items.append((x, lamp))
    return _wrap(items)

# ---------------- border & inscription ----------------
def bird(x, y, c1, c2, flip=False):
    b = fill("M0,0 Q6,-6 14,-2 Q18,0 16,4 Q8,8 0,4 Z", c1, 0, "dark", 1)
    b += fill("M2,0 Q8,-10 14,-4 Q8,-2 2,0 Z", c2, 30, "dark", 0.9)
    b += fill("M14,-2 Q16,-8 20,-6 Q22,-4 20,-2 Z", c1, 0, "dark", 0.9) + stem("M21,-5 L24,-4.4", "ochre", 1, False)
    b += fill("M0,2 L-7,-2 L-6,6 Z", c2, 0, "dark", 0.9) + stem("M8,7 L7,12 M11,7 L12,12", "dark", 0.8, False) + dot(18.5, -5, 0.6, "dark")
    tf = f"translate({x} {y})" + (" scale(-1 1)" if flip else "")
    return g(b, tf)

def beast(x, y, c1, c2):
    b = fill("M0,0 Q12,-4 24,0 L26,6 Q12,9 1,6 Z", c1, 0, "dark", 1)
    b += fill("M23,0 Q27,-7 31,-5 L34,-2 Q33,1 30,1 L26,3 Z", c1, 0, "dark", 0.9) + dot(30, -3, 0.6, "dark")
    b += stem("M3,6 L1,13 M8,7 L9,13 M20,7 L18,13 M25,6 L28,13", c2, 1.6, False)
    b += stem("M0,2 Q-6,-2 -4,-9 Q-2,-11 0,-9", c2, 1.4, False)
    return g(b, f"translate({x} {y})")

def border():
    w = 384
    s = fill(f"M0,0 L{w},0 L{w},64 L0,64 Z", "cream", 0, None, 0, 'opacity="0.0"')
    s += stem(f"M0,3 L{w},3 M0,61 L{w},61", "terra", 1.6, False) + stem(f"M0,6 L{w},6 M0,58 L{w},58", "indigo", 1, False)
    cols = ["terra", "green", "ochre", "blue"]
    for i, x in enumerate(range(0, w, 64)):
        s += fill(f"M{x - 4},58 L{x + 4},58 L{x + 12},6 L{x + 4},6 Z", cols[i % 4], 70, "dark", 1.1)
        cx = x + 22
        k = i % 3
        if k == 0: s += bird(cx, 34, "terra", "ochre")
        elif k == 1: s += beast(cx - 4, 28, "blue", "terra")
        else: s += bird(cx + 22, 34, "green", "terra", True)
    return s
S["border"] = (384, 64, border, False)

def inscription(text="HERE YE KNYGHTES DOE BATTEL WYTH YE BEASTES"):
    def f():
        cols = ("indigo", "terra", "green", "indigo", "terra", "green", "indigo")
        parts = []
        for i, wd in enumerate(text.split()):
            if i: parts.append(f'<tspan fill="{WOOL["dark"]}" stroke="{WOOL["dark"]}"> · </tspan>')
            c = WOOL[cols[i % 5]]
            parts.append(f'<tspan fill="{c}" stroke="{c}">{wd}</tspan>')
        return (f'<text x="6" y="34" font-family="DejaVu Serif, serif" font-size="28" letter-spacing="1.5" stroke-width="0.6">'
                + "".join(parts) + "</text>")
    return f
S["inscription"] = (900, 44, inscription(), False)
S["title"] = (500, 44, inscription("TAPESTRY ADVENTURES"), False)

# ---------------- parallax layers (960x540) ----------------
BW, BH = 960, 540
def linen_bg():
    s = ""
    r = random.Random(4)
    for _ in range(14):   # age stains on the linen
        x, y, rr = r.randint(0, BW), r.randint(0, BH), r.randint(40, 140)
        s += f'<ellipse cx="{x}" cy="{y}" rx="{rr}" ry="{rr * 0.6}" fill="#b89a64" opacity="0.06"/>'
    return s
S["bg_linen"] = (BW, BH, linen_bg, True)

def _wrap(items):
    s = ""
    for x, body in items:
        for dx in (0, -BW, BW):
            if -400 < x + dx < BW + 50:
                s += g(body, f"translate({x + dx} 0)")
    return s

def underdraw(body):   # pale thread, unfilled: as if only the outlines were stitched
    import re
    body = re.sub(r'fill="url\(#w-[^"]+\)"', 'fill="none"', body)
    body = re.sub(r'fill="url\(#m-[^"]+\)"', 'fill="none"', body)
    return f'<g opacity="0.33">{body}</g>'

def far_layer():
    items = [(40, g(underdraw(motte()), "translate(0 300) scale(1.6)")),
             (330, g(underdraw(hill()), "translate(0 360) scale(2)")),
             (560, g(underdraw(hall()), "translate(0 260) scale(1.6)")),
             (820, g(underdraw(tree()), "translate(0 210) scale(1.5)"))]
    return _wrap(items)
S["bg_far"] = (BW, BH, far_layer, False)

def near_layer():
    items = []
    for x, k in ((10, "t"), (210, "s"), (300, "h"), (520, "t"), (700, "s"), (780, "t")):
        if k == "t": items.append((x, g(tree(), "translate(0 300) scale(1.3)")))
        elif k == "s": items.append((x, g(shrub(), "translate(0 440) scale(1.2)")))
        else: items.append((x, g(hill(), "translate(0 430) scale(1.3)")))
    return _wrap(items)
S["bg_near"] = (BW, BH, near_layer, False)

def build_svg(name):
    w, h, fn, bg = S[name]
    return doc(w, h, fn(), bg=bg, seed=21)

S["bg_forest_far"] = (BW, BH, forest_far, False)
S["bg_forest_near"] = (BW, BH, forest_near, False)

S["bg_crypt_far"] = (BW, BH, crypt_far, False)
S["bg_crypt_near"] = (BW, BH, crypt_near, False)

S["bg_castle_far"] = (BW, BH, castle_far, False)
S["bg_castle_near"] = (BW, BH, castle_near, False)

S["bg_shore_far"] = (BW, BH, shore_far, False)
S["bg_shore_near"] = (BW, BH, shore_near, False)

S["bg_deep_far"] = (BW, BH, deep_far, False)
S["bg_deep_near"] = (BW, BH, deep_near, False)

S["bg_sea_near"] = (BW, BH, sea_waves_near, False)

S["bg_creation_far"] = (BW, BH, creation_far, False)
S["bg_creation_near"] = (BW, BH, creation_near, False)
