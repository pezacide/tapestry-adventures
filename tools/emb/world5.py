"""World 5: the Unicorn Tapestries (The Hunt of the Unicorn). Woven millefleur grounds - a dark green field
strewn with countless little flowers - with oak, holly and pomegranate trees, and a château on the hill.
The unicorn itself is white, spotted red with pomegranate juice, and once spared becomes a second mount."""
import math, random
from embroider import fill, stem, dot, woven, g, rot, WOOL, doc
from creatures import clip, limb, C, dots
import scenery, allies
from scenery import BW, BH, _wrap
from world4 import leaf

T = 48
S = scenery.S

def millefleurs(x0, y0, x1, y1, n, seed, cols=("cream", "terra", "gold", "rose", "sky", "white")):
    """The thousand flowers: tiny plants, each a stalk, two leaves and a bloom."""
    r = random.Random(seed); s = ""
    for _ in range(n):
        x, y = r.uniform(x0, x1), r.uniform(y0, y1); c = r.choice(cols); k = r.uniform(0.7, 1.3)
        s += f'<path d="M{x:.1f},{y + 6 * k:.1f} L{x:.1f},{y:.1f}" stroke="{WOOL["sage"]}" stroke-width="{0.9 * k:.2f}"/>'
        s += f'<path d="M{x:.1f},{y + 4 * k:.1f} q{-3 * k:.1f},{-1 * k:.1f} {-4 * k:.1f},{-3 * k:.1f} M{x:.1f},{y + 4 * k:.1f} q{3 * k:.1f},{-1 * k:.1f} {4 * k:.1f},{-3 * k:.1f}" stroke="{WOOL["sage"]}" stroke-width="{0.8 * k:.2f}" fill="none"/>'
        for j in range(5):
            a = j * 1.2566 + x
            s += f'<circle cx="{x + math.cos(a) * 1.7 * k:.1f}" cy="{y + math.sin(a) * 1.7 * k:.1f}" r="{1.2 * k:.2f}" fill="{WOOL[c]}"/>'
        s += f'<circle cx="{x:.1f}" cy="{y:.1f}" r="{0.7 * k:.2f}" fill="{WOOL["gold"]}"/>'
    return s

# ---------------------------------------------------------------- tiles
def millefleur_top():
    s = woven("M0,6 L48,6 L48,48 L0,48 Z", "green")
    s += millefleurs(2, 12, 46, 42, 6, 31)
    s += woven("M0,2 q4,-3 8,0 q4,3 8,0 q4,-3 8,0 q4,3 8,0 q4,-3 8,0 q4,3 8,0 L48,9 L0,9 Z", "sage", "dark", 1.2)
    return s
S["millefleur_top"] = (T, T, millefleur_top, False)

DEEP = '<rect x="{x}" y="{y}" width="{w}" height="{h}" fill="#1f2f3a" opacity="0.5"/>'   # darkens green toward the deep blue-green ground
def deep(x, y, w, h): return DEEP.format(x=x, y=y, w=w, h=h)

def millefleur_fill():
    return woven("M0,0 L48,0 L48,48 L0,48 Z", "green") + deep(0, 0, 48, 48) + millefleurs(2, 4, 46, 44, 4, 37, ("cream", "terra", "gold", "rose"))
S["millefleur_fill"] = (T, T, millefleur_fill, False)

def paling():
    """A block of the unicorn's fence: split oak palings bound with withies (breaks like a brick)."""
    s = woven("M1,1 L47,1 L47,47 L1,47 Z", "buff", "dark", 1.6)
    for x in range(1, 47, 9):
        s += woven(f"M{x},1 L{x + 8},1 L{x + 8},47 L{x},47 Z", ("ochre", "buff")[(x // 9) % 2], "brown", 0.8)
    s += stem("M1,14 L47,14 M1,34 L47,34", "brown", 2.4, False)
    return s
S["paling"] = (T, T, paling, False)

def gate_paling():
    """The bolted gate of the unicorn's fence: darker oak with iron straps (crumbles when the boss is calmed)."""
    s = woven("M1,1 L47,1 L47,47 L1,47 Z", "brown", "dark", 1.6)
    for x in range(1, 47, 9):
        s += stem(f"M{x + 4},1 L{x + 4},47", "dark", 0.8, False)
    s += woven("M1,10 L47,10 L47,16 L1,16 Z", "grey", "dark", 1) + woven("M1,32 L47,32 L47,38 L1,38 Z", "grey", "dark", 1)
    s += dot(8, 13, 1.6, "dark") + dot(40, 13, 1.6, "dark") + dot(8, 35, 1.6, "dark") + dot(40, 35, 1.6, "dark")
    return s
S["gate_paling"] = (T, T, gate_paling, False)

def rail():
    """A fence rail laid across: the plank of the unicorn worlds."""
    s = woven("M0,5 L48,5 L48,15 L0,15 Z", "ochre", "dark", 1.2)
    s += stem("M2,10 L46,10", "brown", 1, False) + leaf(12, 6, -70, 0.3, "green") + leaf(36, 6, 60, 0.3, "sage")
    return s
S["rail"] = (T, 18, rail, False)

def stream(color, light, foul):
    s = woven("M0,10 L48,10 L48,48 L0,48 Z", color)
    for y in (16, 28, 40):
        s += stem(f"M0,{y} q6,-3 12,0 t12,0 t12,0 t12,0", light, 1.4, False)
    s += woven("M0,8 q6,-4 12,0 t12,0 t12,0 t12,0 L48,12 L0,12 Z", light)
    if foul:
        for x, y, r in ((10, 22, 2.4), (30, 34, 1.8), (40, 18, 2.8), (20, 42, 1.6)):
            s += f'<circle cx="{x}" cy="{y}" r="{r}" fill="none" stroke="{WOOL["sage"]}" stroke-width="1"/>'
        s += stem("M6,30 l3,-3 l3,3 M34,24 l3,-3 l3,3", "olive", 1, False)
    return s
S["stream_foul"] = (T, T, lambda: stream("violet", "sage", True), False)
S["stream_pure"] = (T, T, lambda: stream("blue", "sky", False), False)

# ---------------------------------------------------------------- props
def fountain():
    """The Gothic fountain of 'The Unicorn at the Fountain': an octagonal basin, a pillar, lion-head spouts."""
    s = ""
    s += woven("M8,192 L14,140 L130,140 L136,192 Z", "grey", "dark", 1.6)
    for x in range(14, 136, 20):
        s += stem(f"M{x + 6},144 L{x + 6},188", "dark", 0.8, False)
    s += woven("M4,140 L140,140 L136,130 L8,130 Z", "cream", "dark", 1.4)
    s += woven("M10,132 Q72,120 134,132 L134,140 L10,140 Z", "sky")                      # the water
    s += woven("M62,130 L66,40 L78,40 L82,130 Z", "grey", "dark", 1.4)                   # pillar
    s += woven("M46,70 L98,70 L92,82 L52,82 Z", "cream", "dark", 1.2)                    # upper basin
    s += woven("M58,40 L86,40 L80,30 L64,30 Z", "gold", "dark", 1.2)
    s += woven("M72,30 L68,10 L72,2 L76,10 Z", "gold", "dark", 1.2)                       # finial
    for sx in (-1, 1):                                                                     # lion-head spouts
        hx = 72 + sx * 22
        s += dot(hx, 88, 5, "gold", "dark") + dot(hx - 1.5, 87, 0.8, "dark") + dot(hx + 1.5, 87, 0.8, "dark")
        s += stem(f"M{hx + sx * 4},90 Q{hx + sx * 22},96 {hx + sx * 26},128", "sky", 2.4, False)
    return s
S["fountain"] = (144, 192, fountain, False)

def hunt_line():
    """The hunt closing in (5-3): huntsmen with levelled spears and their greyhounds, all running."""
    s = ""
    for i, (x, y) in enumerate(((70, 0), (30, 12), (110, 18))):
        s += g(hunter_body(i % 2, (("terra", "blue"), ("green", "rose"), ("blue", "ochre"))[i]), f"translate({x} {y + 112}) scale(1.6)")
    for i, x in enumerate((10, 120)):
        s += g(hound(i, "grey", "white", "terra"), f"translate({x} {240}) scale(1.4)")
    return s
S["hunt_line"] = (230, 310, hunt_line, False)

# ---------------------------------------------------------------- backgrounds
def tree(x, base, kind, h=300):
    """A woven tree of the unicorn tapestries: oak, holly, orange or pomegranate."""
    s = woven(f"M{x - 9},{base} L{x - 6},{base - h} L{x + 6},{base - h} L{x + 9},{base} Z", "brown", "dark", 1.2)
    r = random.Random(x)
    cy = base - h
    for k in range(60):
        a = r.uniform(0, 6.28); d = r.uniform(0, 95)
        lx, ly = x + math.cos(a) * d * 1.1, cy + math.sin(a) * d * 0.7
        if kind == "holly":
            s += woven(f"M{lx:.0f},{ly:.0f} l6,-4 l2,5 l6,-2 l-3,6 l5,3 l-8,1 l-2,5 l-4,-5 l-6,1 l3,-5 Z", "green", "dark", 0.8)
        else:
            s += leaf(lx, ly + 12, r.uniform(-50, 50), 1.2, ("green", "sage", "olive")[k % 3])
    if kind in ("orange", "pomegranate"):
        for k in range(14):
            a = r.uniform(0, 6.28); d = r.uniform(10, 80)
            s += dot(x + math.cos(a) * d, cy + math.sin(a) * d * 0.6, 5.5, "ochre" if kind == "orange" else "terra", "dark")
    return s

def millefleur_far():
    s = woven(f"M0,0 L{BW},0 L{BW},{BH} L0,{BH} Z", "green") + deep(0, 0, BW, BH)
    s += woven(f"M0,0 L{BW},0 L{BW},150 Q{BW * 0.7},120 {BW * 0.5},150 T0,140 Z", "sky")     # a strip of sky
    s += woven("M560,150 L560,80 L640,80 L640,150 Z", "cream", "dark", 1.2)                      # the château on the hill
    for x in (548, 612):
        s += woven(f"M{x},150 L{x},60 L{x + 40},60 L{x + 40},150 Z", "cream", "dark", 1.2) + woven(f"M{x - 4},62 L{x + 20},20 L{x + 44},62 Z", "blue", "dark", 1.2)
    for x, kind in ((90, "oak"), (300, "holly"), (470, "orange"), (760, "oak"), (900, "holly")):
        s += tree(x, BH, kind, 300)
    s += millefleurs(0, 160, BW, BH, 600, 41)
    return f'<g opacity="0.9">{s}</g>'
S["bg_millefleur_far"] = (BW, BH, millefleur_far, False)

def millefleur_near():
    items = []
    for i, x in enumerate(range(60, BW, 300)):
        t = ""
        for k in range(6):
            t += leaf(math.cos(k) * 22, BH - 10 - k * 9, (k - 3) * 25, 1.2, ("green", "sage")[k % 2])
        t += millefleurs(-40, BH - 70, 40, BH - 10, 12, 50 + i)
        items.append((x, t))
    return _wrap(items)
S["bg_millefleur_near"] = (BW, BH, millefleur_near, False)

def fountain_far():
    """The grove of the fountain: millefleur woods, and the brook running from it."""
    s = millefleur_far().replace('opacity="0.9"', 'opacity="0.85"')
    s += woven(f"M0,250 Q{BW * 0.25},230 {BW * 0.5},250 T{BW},250 L{BW},280 Q{BW * 0.75},300 {BW * 0.5},280 T0,280 Z", "sky")   # the brook below the fountain
    s += stem(f"M40,264 q30,-6 60,0 t60,0 M400,262 q30,-6 60,0 t60,0 M700,266 q30,-6 60,0 t60,0", "white", 1.6, False)
    return s
S["bg_fountain_far"] = (BW, BH, fountain_far, False)

def garden_far():
    """The unicorn's enclosure (5-4): a dark millefleur garden, a round fence, the pomegranate tree."""
    s = woven(f"M0,0 L{BW},0 L{BW},{BH} L0,{BH} Z", "green") + deep(0, 0, BW, BH)
    s += millefleurs(0, 0, BW, BH, 650, 61)
    s += tree(480, BH - 200, "pomegranate", 220)
    cx, cy, rx, ry = 480, BH - 200, 170, 30      # the round paling fence, seen from a little above
    for k in range(37):
        a = math.pi * k / 36
        x, y = cx - math.cos(a) * rx, cy + math.sin(a) * ry * (1 if k % 2 else 1)
        s += woven(f"M{x - 6:.0f},{y:.0f} L{x - 6:.0f},{y - 90:.0f} L{x:.0f},{y - 98:.0f} L{x + 6:.0f},{y - 90:.0f} L{x + 6:.0f},{y:.0f} Z", ("buff", "ochre")[k % 2], "dark", 1)
    s += stem(f"M{cx - rx},{cy - 30} Q{cx},{cy + ry * 2 - 30} {cx + rx},{cy - 30} M{cx - rx},{cy - 70} Q{cx},{cy + ry * 2 - 70} {cx + rx},{cy - 70}", "brown", 3, False)
    return f'<g opacity="0.9">{s}</g>'
S["bg_garden_far"] = (BW, BH, garden_far, False)

# ---------------------------------------------------------------- creatures (stitched)
def hunter_body(f, cols=("terra", "blue"), throw=False):
    """A huntsman of the unicorn hunt: hat, short tunic, hose, and a long spear. 52 x 76 design units."""
    tunic, hose = cols
    st = (-12, 12) if f == 0 else (10, -8)
    s = limb(22, 46, 26, 5, hose, st[0], "hoof") + limb(30, 46, 26, 5, hose, st[1], "hoof")
    body = "M16,22 Q26,16 36,22 L38,48 Q26,52 14,48 Z"
    s += fill(body, tunic, 90) + stem("M15,40 L37,40", "gold", 1.4, False)
    s += fill("M18,10 Q18,2 26,2 Q34,2 34,10 Q34,18 26,19 Q18,18 18,10 Z", "buff", 0)
    s += fill("M16,6 Q26,-3 37,5 L36,8 Q26,4 16,9 Z", "dark" if f else "green", 0, "dark", 1)       # hat
    s += dot(29, 9, 0.9, "dark") + stem("M27,14 L31,14", "terra", 0.8, False)
    if throw:
        s += fill("M32,24 L44,6 L48,9 L36,28 Z", tunic, 60, "dark", 1.1) + dot(46, 7, 2.2, "buff", "dark")
        s += stem("M30,22 L52,-6", "brown", 1.8, False)
    else:
        s += fill("M32,24 L44,32 L42,36 L30,30 Z", tunic, 60, "dark", 1.1) + dot(44, 34, 2.2, "buff", "dark")
        s += stem("M8,40 L52,26", "brown", 1.8, False) + fill("M50,24.5 L58,23.5 L52,29 Z", "grey", 0, "dark", 0.8)
    return s

def hunter(f):
    return g(hunter_body(f % 2, throw=f == 2), "translate(0 4)")
C["hunter"] = dict(title="Huntsman", size=(60, 84), frames=3, build=hunter,
                   behaviour="walks its beat; when the knight is in front it hurls a spear in a long arc (frame 2); stompable")

def hound(f, base="grey", belly="white", collar="terra"):
    """A greyhound of the unicorn hunt: slender, deep-chested, collared."""
    ext = f == 0
    s = stem("M12,18 Q2,16 4,6" if ext else "M12,18 Q0,22 2,12", base, 2, False)
    s += limb(16, 22, 18, 3, base, 40 if ext else -15, "paw", base) + limb(48, 20, 18, 3, base, -45 if ext else 25, "paw", base)
    body = "M10,18 Q30,10 54,14 L56,24 Q44,24 34,22 Q20,30 12,26 Q8,22 10,18 Z"
    s += fill(body, base, 170) + clip(f"gh{base}", body, fill("M30,22 Q42,26 54,22 L54,26 L30,26 Z", belly, 0, None)) + stem(body, "dark", 1.3)
    s += limb(20, 22, 18, 3, belly, 55 if ext else -30, "paw", belly) + limb(52, 21, 18, 3, belly, -60 if ext else 35, "paw", belly)
    s += fill("M50,14 Q56,6 62,7 L70,12 Q70,15 67,15.5 L59,17 Q53,18 50,14 Z", base, 0)
    s += fill("M56,8 L52,4 L58,9 Z", base, 0, "dark", 0.8) + dot(62, 10.5, 0.9, "dark")
    s += stem("M51,11 Q52,16 54,18", collar, 2.4, False) + dot(53, 18.5, 1.1, "gold")
    return s
C["greyhound"] = dict(title="Greyhounde", size=(72, 40), frames=2, build=hound,
                      behaviour="lurks still; when the knight comes near it races at him, leaping gaps; stompable")

def pheasant(f):
    """A cock pheasant from the millefleur grass: crouched (0) or bursting up into the air (1)."""
    s = ""
    if f:
        s += fill("M24,18 Q14,0 2,2 Q10,10 8,16 Q16,16 22,22 Z", "ochre", 40) + stem("M24,18 L6,4", "dark", 0.8, False)
    s += stem("M14,22 Q4,24 0,30" if f else "M12,26 Q2,24 0,18", "brown", 2.6, False) + stem("M14,22 Q4,24 0,30" if f else "M12,26 Q2,24 0,18", "ochre", 1, False)
    body = "M10,24 Q18,14 34,16 Q42,20 40,28 Q30,34 18,32 Q10,30 10,24 Z"
    s += fill(body, "terra", 0) + clip(f"phb{f}", body, dots(8, 14, 44, 34, 4.5, 0.9, "gold")) + stem(body, "dark", 1.2)
    s += fill("M32,16 Q34,6 40,6 Q46,8 44,14 L40,18 Z", "green", 0, "dark", 1)
    s += fill("M39,9 Q44,10 42,14 Q39,13 39,9 Z", "terra", 0, None) + dot(40.5, 9, 0.8, "dark")
    s += fill("M44,9 L49,11 L44,12 Z", "ochre", 0, "dark", 0.7) + stem("M32,17 Q36,20 40,18", "white", 1.4, False)
    if not f:
        s += stem("M22,32 L21,38 M30,32 L31,38", "dark", 1.2, False)
    return s
C["pheasant"] = dict(title="Pheasaunt", size=(52, 40), frames=2, build=pheasant,
                     behaviour="crouches in the flowers; bursts up and whirs across the sky when the knight comes near; stompable")

def genet(f):
    """A genet, the spotted beast by the fountain: low, long, with a ringed tail. It pounces."""
    up = f == 1
    tail = "M10,22 Q0,20 2,8 Q4,2 10,4"
    s = stem(tail, "buff", 3.2, False) + f'<path d="{tail}" fill="none" stroke="{WOOL["dark"]}" stroke-width="3.2" stroke-dasharray="2 3"/>'
    s += limb(16, 24, 9, 3, "buff", 30 if up else -10, "paw", "buff") + limb(44, 24, 9, 3, "buff", -40 if up else 10, "paw", "buff")
    body = "M10,18 Q28,10 48,14 L50,24 Q32,28 14,26 Q8,24 10,18 Z"
    s += fill(body, "buff", 170) + clip(f"gnb{f}", body, dots(8, 10, 52, 30, 5, 1.3, "dark")) + stem(body, "dark", 1.3)
    s += fill("M46,14 Q52,8 58,10 L60,16 Q56,20 50,20 Z", "buff", 0, "dark", 1.1)
    s += fill("M50,11 L50,5 L54,10 Z", "buff", 0, "dark", 0.8) + dot(55, 13, 0.9, "dark")
    return g(s, rot(-14 if up else 0, 30, 24))
C["genet"] = dict(title="Genette", size=(62, 34), frames=2, build=genet,
                  behaviour="paces by the fountain and pounces at the knight in long low leaps; stompable")

# ---------------------------------------------------------------- the unicorn (mount and boss)
POM = [(22, 30), (30, 36), (40, 30), (50, 38), (58, 30), (66, 36), (28, 44), (46, 44), (62, 44), (36, 38), (74, 32)]
def unicorn_deco(collar=False):
    s = ""
    for x, y in POM:   # spotted red with pomegranate juice, as in 'The Unicorn in Captivity'
        s += dot(x, y, 1.6, "terra")
    s += fill("M86,6 L103,-18 L91,8 Z", "cream", 60, "dark", 1)                                   # the spiral horn
    s += stem("M88,3 L92,2 M91,-2 L95,-3 M94,-7 L98,-8 M97,-12 L100,-13", "gold", 0.9, False)
    s += fill("M93,23 Q94,32 90,34 Q90,27 89,23 Z", "white", 90, "dark", 0.8)                   # goat's beard
    if collar:
        s += stem("M70,18 Q76,26 82,24", "gold", 3, False) + dot(78, 25, 1.6, "terra", "dark")
    return s

def unicorn_steed(pose, rider="", saddle=True, collar=False):
    return allies.horse(pose, body_c="white", far_c="cream", mane="cream", rider=rider, saddle=saddle, deco=unicorn_deco(collar))

UPAD = 22   # headroom above the frame for the horn
def spotted_unicorn(f):
    """Ye Spotted Unicorne (boss of World 5): 0/1 galloping charge, 2 rearing (warning), 3 dazed, 4 calmed (spared)."""
    pose = ("walk1", "walk2", "rear", "idle", "idle")[f]
    s = unicorn_steed(pose, saddle=False, collar=f == 4)
    s = g(s, f"translate(0 {UPAD}) scale(1)")
    if f == 3:
        for k in range(5):
            a = k * 1.2566; s += dot(88 + math.cos(a) * 14, 12 + math.sin(a) * 4, 2, "gold", "dark")
    if f == 4:   # calmed: a little garland of flowers about it
        s += millefleurs(10, 90, 100, 104, 8, 77)
    return g(s, "scale(1.9)")
import bosses
bosses.B["spotted_unicorn"] = dict(title="Ye Spotted Unicorne", size=(200, 202), frames=5, build=spotted_unicorn,
                                   behaviour="boss: rears, then charges; leap aside so it rams the fence gate. Three rams break the gate and it is calmed - spared, it becomes a mount")

# ---------------------------------------------------------------- end buildings
import castles
from castles import door, window, pennant
def chateau():
    """The château of the unicorn tapestries: pale walls, round towers with blue pointed roofs."""
    s = castles.wall(50, 120, 190, 260, "cream", "buff")
    for x in (34, 186):
        s += castles.wall(x - 22, 70, x + 22, 260, "cream", "buff") + woven(f"M{x - 28},72 L{x},10 L{x + 28},72 Z", "blue", "dark", 1.6) + pennant(x, 6, "terra", 18)
    s += woven("M44,122 L120,70 L196,122 Z", "blue", "dark", 1.6)
    s += window(90, 160) + window(150, 160) + window(34, 120) + window(186, 120)
    s += door(120, 260, 40, 70)
    return s
castles.C["castle_chateau"] = dict(size=(240, 260), title="Ye Castel of ye Hunte", fn=chateau)

def chapel():
    """A little Gothic chapel by the fountain's stream."""
    s = castles.wall(50, 110, 190, 252, "buff", "brown")
    s += woven("M40,112 L120,40 L200,112 Z", "terra", "dark", 1.6)
    s += woven("M108,40 L108,16 L132,16 L132,40 Z", "cream", "dark", 1.2) + stem("M120,16 L120,0 M113,6 L127,6", "gold", 2.4, False)
    s += woven("M104,130 Q120,108 136,130 L136,170 L104,170 Z", "sky", "dark", 1.4) + stem("M120,112 L120,170 M104,146 L136,146", "dark", 1, False)
    s += door(120, 252, 36, 62)
    return s
castles.C["castle_chapel"] = dict(size=(240, 252), title="Ye Chapel by ye Fountaine", fn=chapel)

def gatehouse():
    """The gatehouse of the park, where the chase ends."""
    s = castles.wall(30, 80, 210, 260, "grey", "dark") + castles.crenels(26, 214, 80, "cream")
    s += woven("M80,260 L80,150 Q120,110 160,150 L160,260 Z", "dark")
    s += stem(" ".join(f"M{x},150 L{x},260" for x in range(88, 160, 10)) + " M80,190 L160,190 M80,225 L160,225", "grey", 1.6, False)
    s += window(50, 130) + window(190, 130) + pennant(40, 40, "blue", 24) + pennant(200, 40, "terra", 24)
    s += door(120, 260, 36, 64)
    return s
castles.C["castle_gatehouse"] = dict(size=(240, 260), title="Ye Gatehouse of ye Parke", fn=gatehouse)

for k in ("castle_chateau", "castle_chapel", "castle_gatehouse"):
    v = castles.C[k]; S[k] = (v["size"][0], v["size"][1], v["fn"], False)
