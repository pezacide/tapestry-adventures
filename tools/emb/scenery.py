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

def inscription(text="HIC MILITES PUGNANT CUM BESTIIS"):
    def f():
        cols = ("indigo", "terra", "green", "indigo", "terra")
        parts = []
        for i, wd in enumerate(text.split()):
            if i: parts.append(f'<tspan fill="{WOOL["dark"]}" stroke="{WOOL["dark"]}"> · </tspan>')
            c = WOOL[cols[i % 5]]
            parts.append(f'<tspan fill="{c}" stroke="{c}">{wd}</tspan>')
        return (f'<text x="6" y="34" font-family="DejaVu Serif, serif" font-size="28" letter-spacing="1.5" stroke-width="0.6">'
                + "".join(parts) + "</text>")
    return f
S["inscription"] = (760, 44, inscription(), False)
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
