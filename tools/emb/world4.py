"""World 4: the Devonshire Hunting Tapestries - the first WOVEN world. Backgrounds and tiles are woven
(flat colour, fine weft lines), while the creatures keep the stitched look so they read clearly on top."""
import math, random
from embroider import fill, stem, dot, woven, g, rot, WOOL
from creatures import clip, scales, limb, C, dots
import scenery
from scenery import BW, BH, _wrap, palmette

T = 48
S = scenery.S

def leaf(cx, cy, ang, size, color, vein="olive"):
    """A big woven leaf, like the verdure of the hunting tapestries (oak-ish lobes)."""
    L, W = 26 * size, 10 * size
    d = (f"M0,0 Q{-W},{-L * 0.3} {-W * 0.6},{-L * 0.55} Q{-W * 1.1},{-L * 0.65} {-W * 0.4},{-L * 0.8} "
         f"Q{-W * 0.5},{-L * 0.95} 0,{-L} Q{W * 0.5},{-L * 0.95} {W * 0.4},{-L * 0.8} "
         f"Q{W * 1.1},{-L * 0.65} {W * 0.6},{-L * 0.55} Q{W},{-L * 0.3} 0,0 Z")
    s = woven(d, color, "dark", 0.9) + f'<path d="M0,0 L0,{-L * 0.9}" stroke="{WOOL[vein]}" stroke-width="{1.2 * size}"/>'
    return g(s, f"translate({cx:.1f} {cy:.1f}) rotate({ang:.1f})")

def flowers(x0, y0, x1, y1, n, seed):
    r = random.Random(seed); s = ""
    for _ in range(n):
        x, y = r.uniform(x0, x1), r.uniform(y0, y1)
        c = r.choice(["terra", "gold", "cream", "rose", "sky"])
        for k in range(5):
            a = k * 1.2566
            s += f'<circle cx="{x + math.cos(a) * 2.2:.1f}" cy="{y + math.sin(a) * 2.2:.1f}" r="1.6" fill="{WOOL[c]}"/>'
        s += f'<circle cx="{x:.1f}" cy="{y:.1f}" r="1" fill="{WOOL["gold"]}"/>'
    return s

# ---------------------------------------------------------------- tiles
def verdure_top():
    s = woven("M0,8 L48,8 L48,48 L0,48 Z", "green")
    s += woven("M0,24 L48,24 L48,48 L0,48 Z", "olive")
    s += flowers(2, 12, 46, 22, 4, 3)
    s += woven("M0,2 q6,-4 12,0 q6,4 12,0 q6,-4 12,0 q6,4 12,0 L48,10 L0,10 Z", "sage", "dark", 1.2)
    return s
S["verdure_top"] = (T, T, verdure_top, False)

def verdure_fill():
    return woven("M0,0 L48,0 L48,48 L0,48 Z", "olive") + flowers(2, 4, 46, 44, 3, 5)
S["verdure_fill"] = (T, T, verdure_fill, False)

def wicker():
    """A woven wicker hamper from the hunt's baggage: breaks like a brick."""
    s = woven("M1,1 L47,1 L47,47 L1,47 Z", "ochre", "dark", 1.6)
    for y in range(6, 46, 8):
        s += stem(f"M1,{y} Q12,{y - 3} 24,{y} T47,{y}", "brown", 1.6, False)
    for x in range(8, 46, 10):
        s += stem(f"M{x},1 L{x},47", "buff", 1.2, False)
    return s
S["wicker"] = (T, T, wicker, False)

def branch():
    """A fallen branch laid across: the woods' plank."""
    s = woven("M0,6 Q24,2 48,6 L48,16 Q24,13 0,16 Z", "brown", "dark", 1.2)
    s += stem("M2,10 Q24,7 46,10", "ochre", 1, False) + leaf(10, 6, -60, 0.35, "green") + leaf(38, 6, 50, 0.35, "sage")
    return s
S["branch"] = (T, 18, branch, False)

def raft():
    """A raft of lashed logs drifting on the river current (3 tiles wide)."""
    s = ""
    for i, x in enumerate(range(4, 140, 17)):
        s += woven(f"M{x},6 L{x + 16},6 L{x + 16},20 L{x},20 Z", ("brown", "ochre")[i % 2], "dark", 1)
        s += f'<circle cx="{x + 8}" cy="13" r="3" fill="none" stroke="{WOOL["buff"]}" stroke-width="1"/>'
    s += stem("M4,9 L140,9 M4,17 L140,17", "buff", 1.4, False)
    return s
S["raft"] = (144, 22, raft, False)

def hunting_stand():
    """A tall wooden hunting stand (scenery under the platforms of the heath)."""
    s = stem("M8,0 L4,192 M88,0 L92,192", "brown", 5, False)
    for y in range(20, 192, 34):
        s += stem(f"M6,{y} L90,{y + 30} M90,{y} L6,{y + 30}", "ochre", 2.4, False)
    return s
S["hunting_stand"] = (96, 192, hunting_stand, False)

# ---------------------------------------------------------------- backgrounds (woven, opaque)
def verdure_far():
    """A wall of woven verdure: dark leaves crowded edge to edge, flowers in the gaps, deer glimpsed between."""
    s = woven(f"M0,0 L{BW},0 L{BW},{BH} L0,{BH} Z", "green")
    r = random.Random(11)
    for _ in range(170):
        s += leaf(r.uniform(-20, BW + 20), r.uniform(40, BH + 20), r.uniform(-60, 60), r.uniform(1.4, 2.4), r.choice(["olive", "green", "sage", "olive"]))
    s += flowers(0, 30, BW, BH, 60, 12)
    return f'<g opacity="0.9">{s}</g>'
S["bg_verdure_far"] = (BW, BH, verdure_far, False)

def verdure_near():
    items = []
    for i, x in enumerate(range(40, BW, 210)):
        t = woven(f"M-10,{BH} L-6,250 L6,250 L10,{BH} Z", "brown", "dark", 1.4)
        for k in range(9):
            t += leaf(0 + math.cos(k) * 40, 250 - k * 6 + math.sin(k * 2) * 20, (k - 4) * 22, 1.6, ("green", "sage")[k % 2])
        items.append((x, t))
    return _wrap(items)
S["bg_verdure_near"] = (BW, BH, verdure_near, False)

def river_far():
    """The far bank: woven verdure above, the river below with swans and reeds."""
    s = verdure_far().replace('opacity="0.9"', 'opacity="0.85"')
    for i, y in enumerate(range(360, BH + 20, 18)):
        s += woven(f"M0,{y} " + " ".join(f"q10,{-4 if (j + i) % 2 else 4} 20,0" for j in range(49)) + f" L{BW},{BH} L0,{BH} Z", ("sky", "blue")[i % 2])
    for x in range(30, BW, 70):
        s += stem(f"M{x},360 Q{x - 4},330 {x + 2},300 M{x + 8},362 Q{x + 12},334 {x + 6},310", "olive", 2.4, False)
    return s
S["bg_river_far"] = (BW, BH, river_far, False)

def heath_far():
    """The open heath: a woven sky, low hills of heather, falconers' towers on the skyline."""
    s = woven(f"M0,0 L{BW},0 L{BW},{BH} L0,{BH} Z", "sky")
    s += woven(f"M0,260 Q{BW * 0.25},200 {BW * 0.5},250 T{BW},240 L{BW},{BH} L0,{BH} Z", "sage")
    s += woven(f"M0,320 Q{BW * 0.3},280 {BW * 0.6},330 T{BW},310 L{BW},{BH} L0,{BH} Z", "violet")
    s += flowers(0, 330, BW, BH, 80, 21)
    for x in (160, 520, 820):
        s += woven(f"M{x},{250} L{x + 30},{250} L{x + 26},{150} L{x + 4},{150} Z", "buff", "dark", 1.2) + woven(f"M{x - 4},150 L{x + 15},120 L{x + 34},150 Z", "terra", "dark", 1.2)
    for x, y in ((300, 90), (700, 60), (430, 130)):
        s += stem(f"M{x - 12},{y} Q{x},{y - 8} {x + 12},{y} M{x - 12},{y} Q{x},{y + 4} {x + 12},{y}", "dark", 1.6, False)
    return f'<g opacity="0.92">{s}</g>'
S["bg_heath_far"] = (BW, BH, heath_far, False)

def lodge_far():
    """The hunting lodge's hall: timbered walls hung with woven hunting scenes and antlers."""
    s = woven(f"M0,0 L{BW},0 L{BW},{BH} L0,{BH} Z", "buff")
    for x in range(0, BW, 120):
        s += woven(f"M{x},0 L{x + 14},0 L{x + 14},{BH} L{x},{BH} Z", "brown", "dark", 1)
    for x in range(30, BW, 240):
        s += woven(f"M{x},110 L{x + 170},110 L{x + 170},330 L{x},330 Z", "green", "dark", 1.4)
        s += g(verdure_far(), f"translate({x + 4} 114) scale(0.17 0.42)")
        s += stem(f"M{x - 6},108 L{x + 176},108", "brown", 4, False)
    for x in range(150, BW, 240):   # antlers
        s += stem(f"M{x},90 Q{x - 20},60 {x - 30},40 M{x - 22},62 L{x - 36},58 M{x},90 Q{x + 20},60 {x + 30},40 M{x + 22},62 L{x + 36},58", "cream", 3, False)
        s += woven(f"M{x - 8},88 L{x + 8},88 L{x + 6},102 L{x - 6},102 Z", "brown", "dark", 1)
    return f'<g opacity="0.95">{s}</g>'
S["bg_lodge_far"] = (BW, BH, lodge_far, False)

# ---------------------------------------------------------------- creatures (stitched, so they read on the woven ground)
def talbot(f):
    """A white talbot hunting hound with brown patches, as in the hunting tapestries."""
    ext = f == 0
    s = stem("M14,20 Q4,14 8,4" if ext else "M14,20 Q2,22 4,10", "white", 2.4, False)
    s += limb(17, 24, 18, 3.4, "buff", 40 if ext else -15, "paw", "buff") + limb(50, 22, 18, 3.4, "buff", -45 if ext else 25, "paw", "buff")
    body = "M12,20 Q32,13 54,17 L58,27 Q44,30 32,27 Q20,32 13,28 Q9,24 12,20 Z"
    s += fill(body, "white", 170) + clip("tlb", body, fill("M24,14 Q34,12 38,20 Q32,26 24,22 Z", "brown", 0, None)) + stem(body, "dark", 1.4)
    s += limb(21, 24, 18, 3.4, "white", 55 if ext else -30, "paw", "white") + limb(54, 23, 18, 3.4, "white", -60 if ext else 35, "paw", "white")
    s += fill("M51,16 Q56,9 62,9.5 L70,15 Q71,18 68,19 L59,20 Q54,21 51,16 Z", "white", 0)
    s += fill("M57,10 L52,16 L56,20 Z", "brown", 0, "dark", 0.9) + dot(63, 13, 0.9, "dark") + stem("M53.5,13 Q54,19 56,21", "terra", 2.2, False)
    return s
C["talbot"] = dict(title="Talbot Hound", size=(74, 48), frames=2, build=talbot,
                   behaviour="one of the hunt's hounds: runs in from behind the knight and on past, leaping gaps; stompable")

def boar(f):
    ext = f == 0
    s = limb(20, 30, 12, 4.4, "brown", 25 if ext else -20, "hoof") + limb(46, 30, 12, 4.4, "brown", -25 if ext else 20, "hoof")
    body = "M8,24 Q14,8 36,8 Q54,8 58,22 L60,32 Q40,38 20,36 Q8,34 8,24 Z"
    s += fill(body, "brown", 160) + clip("bob", body, stem("M14,10 L18,4 M20,9 L24,3 M26,8 L30,2 M32,8 L36,2 M38,9 L42,4", "dark", 1.4, False))
    s += stem(body, "dark", 1.4)
    s += limb(24, 30, 12, 4.4, "olive", -25 if ext else 20, "hoof") + limb(50, 30, 12, 4.4, "olive", 25 if ext else -20, "hoof")
    s += fill("M54,14 Q64,14 68,22 L70,28 Q64,30 58,28 Z", "brown", 0, "dark", 1.2) + dot(62, 18, 1, "dark")
    s += stem("M66,26 Q70,22 70,18", "cream", 2.2, False)   # tusk
    s += stem("M8,24 Q2,22 4,16", "brown", 2, False)
    return s
C["boar"] = dict(title="Wilde Boare", size=(72, 46), frames=2, build=boar,
                 behaviour="charges when it sees the knight ahead, can't turn quickly; stompable")

def otter(f):
    up = f == 1
    s = fill("M10,30 Q12,10 24,6 Q36,10 38,30 Z" if up else "M6,30 Q10,18 24,16 Q40,18 42,30 Z", "brown", 90)
    s += fill("M18,10 Q24,4 30,10 Q30,16 24,17 Q18,16 18,10 Z" if up else "M30,16 Q38,12 44,18 Q42,23 36,23 Z", "brown", 0, "dark", 1.1)
    s += fill("M20,16 Q24,20 28,16 Z" if up else "M38,20 Q42,22 44,19 Z", "cream", 0, None)
    s += dot(22, 10, 0.9, "dark") + dot(26, 10, 0.9, "dark") if up else dot(40, 17, 0.9, "dark")
    s += stem("M4,30 L44,30", "sky", 2, False)   # the waterline about its middle
    return s
C["otter"] = dict(title="Otter", size=(48, 34), frames=2, build=otter,
                  behaviour="lurks in the river, then leaps up out of the water at the knight; stompable while out of the water")

def swan(f):
    s = fill("M10,26 Q20,14 40,16 Q52,18 50,28 Q36,34 18,32 Q8,30 10,26 Z", "white", 0)
    s += fill("M14,22 Q26,8 42,14 Q32,22 20,26 Z" if f else "M14,22 Q24,14 40,16 Q30,22 20,24 Z", "cream", 30, "dark", 1)   # folded wing
    s += stem("M44,18 Q48,6 40,2 Q34,0 36,6", "white", 4, False) + stem("M44,18 Q48,6 40,2", "dark", 0.7, False)
    s += fill("M34,4 L28,6 L34,8 Z", "ochre", 0, "dark", 0.8) + dot(38, 4, 0.9, "dark")
    s += stem("M6,32 Q28,36 52,32", "sky", 2, False)
    return s
C["swan"] = dict(title="Swanne", size=(56, 36), frames=2, build=swan,
                 behaviour="glides on the river and charges at the knight with a hiss; stompable")

def falcon(f):
    up = f == 0
    s = ""
    wing = "M24,20 Q16,2 2,2 Q10,10 8,16 Q16,16 22,24 Z" if up else "M24,22 Q14,36 0,40 Q8,32 8,28 Q16,28 22,24 Z"
    s += fill(wing, "brown", 40) + stem("M24,20 L6,4 M24,20 L10,14" if up else "M24,22 L4,38 M24,22 L10,29", "dark", 0.8, False)
    body = "M8,24 Q20,14 38,18 Q46,22 44,28 Q32,32 18,30 Q8,29 8,24 Z"
    s += fill(body, "buff", 0) + clip("flb", body, dots(6, 14, 48, 34, 5, 1.1, "brown")) + stem(body, "dark", 1.3)
    s += fill("M36,16 Q42,8 48,14 Q48,20 42,20 Z", "brown", 0, "dark", 1) + dot(44, 14, 1, "gold", "dark")
    s += fill("M48,15 L53,18 L48,19 Z", "dark", 0, None)
    s += fill("M8,24 L0,20 L2,30 Z", "brown", 0, "dark", 0.9)
    s += stem("M20,30 L22,34 M28,30 L30,34", "gold", 1.4, False) + dot(25, 33, 1.2, "terra")   # jesses and bell
    return s
C["falcon"] = dict(title="Falcon", size=(56, 42), frames=2, build=falcon,
                   behaviour="a hunting falcon loosed from the fist: wheels high, then stoops on the knight; stompable")

# ---------------------------------------------------------------- the boss: Ye Greate Boar
import bosses
def great_boar(f):
    """Ye Greate Boar: the quarry of the hunt grown enormous. frame 0 charging, 1 pawing the ground, 2 dazed."""
    s = ""
    ext = f == 0
    for x, a in ((40, 25 if ext else -10), (70, -25 if ext else 10), (120, 25 if ext else -10), (150, -25 if ext else 10)):
        s += bosses.big_limb(x, 92, 28, 12, "brown", a)
    body = "M14,70 Q30,20 96,18 Q150,16 176,52 L182,94 Q130,110 70,106 Q20,100 14,70 Z"
    s += fill(body, "brown", 160) + clip("gbb", body, stem(" ".join(f"M{x},{24 + (x % 3) * 2} L{x + 6},{10}" for x in range(30, 170, 10)), "dark", 2, False))
    s += clip("gbb2", body, scales(10, 30, 190, 110, 12, "olive", 1))
    s += stem(body, "dark", 2)
    s += fill("M168,40 Q196,36 206,62 L212,84 Q196,92 178,88 Z", "brown", 0, "dark", 1.6)       # head
    s += dot(186, 54, 3, "gold" if f != 2 else "white", "dark")
    if f == 2:   # dazed: little stars round its head
        for k in range(5):
            a = k * 1.2566; s += dot(188 + math.cos(a) * 26, 30 + math.sin(a) * 8, 3, "gold", "dark")
    s += stem("M206,80 Q216,66 210,52", "cream", 5, False) + stem("M206,80 Q216,66 210,52", "dark", 0.8, False)   # great tusk
    s += fill("M200,40 L196,24 L208,34 Z", "brown", 0, "dark", 1.2)                                   # ear
    s += stem("M14,70 Q2,64 6,50", "brown", 4, False)
    if f == 1:   # pawing: dust kicked up behind
        for x, y in ((20, 116), (8, 108), (34, 112)): s += dot(x, y, 4, "buff")
    return s
bosses.B["great_boar"] = dict(title="Ye Greate Boar", size=(220, 124), frames=3, build=great_boar,
                              behaviour="boss: paws the ground, then charges across the lodge; if it rams the wall it is dazed - stomp it then; three blows")

# ---------------------------------------------------------------- end buildings
import castles
from castles import door, window, pennant
def pavilion():
    """A striped hunting pavilion, as pitched in the Devonshire tapestries."""
    s = ""
    tent = "M20,252 L40,110 Q120,60 200,110 L220,252 Z"
    s += woven(tent, "cream") + clip("pvl", tent, "".join(woven(f"M{x},0 L{x + 20},0 L{x + 30},260 L{x + 10},260 Z", "blue") for x in range(10, 230, 40))) + stem(tent, "dark", 1.8)
    s += woven("M30,110 Q120,30 210,110 Q120,90 30,110 Z", "terra", "dark", 1.6)
    s += stem("M120,58 L120,8", "brown", 3, False) + pennant(120, 4, "terra", 20)
    s += stem(" ".join(f"M{x},110 L{x},124" for x in range(40, 204, 16)), "gold", 2, False)   # scalloped valance tassels
    s += door(120, 252, 40, 74)
    return s
castles.C["castle_pavilion"] = dict(size=(240, 252), title="Ye Huntsmen's Pavilion", fn=pavilion)

def mill():
    """A water mill on the river, its wheel turning in the stream."""
    s = castles.wall(40, 110, 200, 252, "buff")
    s += woven("M30,112 L120,50 L210,112 Z", "terra", "dark", 1.8)
    s += window(80, 150) + window(160, 150)
    for k in range(8):
        a = k * math.pi / 4
        s += stem(f"M220,180 L{220 + math.cos(a) * 34:.1f},{180 + math.sin(a) * 34:.1f}", "brown", 4, False)
    s += stem("M186,180 a34,34 0 1,0 68,0 a34,34 0 1,0 -68,0", "brown", 4, False) + dot(220, 180, 6, "ochre", "dark")
    s += door(120, 252, 38, 64)
    return s
castles.C["castle_mill"] = dict(size=(264, 252), title="Ye Mill by ye River", fn=mill)

def mews():
    """The falconers' mews: a stone tower with perches and a hooded falcon."""
    s = castles.wall(70, 70, 170, 264, "grey", "dark") + castles.crenels(66, 174, 70, "terra")
    s += window(120, 110) + window(120, 160)
    s += stem("M60,150 L180,150", "brown", 4, False) + stem("M40,200 L200,200", "brown", 4, False)   # perches
    for x in (70, 170):
        s += woven(f"M{x - 8},148 Q{x},134 {x + 8},148 Z", "brown", "dark", 1) + dot(x + 2, 140, 1.2, "terra")
    s += castles.roof(120, 62, 120, 50, "green", "mwr") + pennant(120, 2, "gold", 20)
    s += door(120, 264, 36, 62)
    return s
castles.C["castle_mews"] = dict(size=(240, 264), title="Ye Falconers' Mews", fn=mews)

for k in ("castle_pavilion", "castle_mill", "castle_mews"):
    v = castles.C[k]; S[k] = (v["size"][0], v["size"][1], v["fn"], False)
