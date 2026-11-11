"""World 6: The Lady and the Unicorn. Red millefleur grounds strewn with flowers and little beasts, with blue oval
islands of flowers on which the Lady stands; the Le Viste arms (a blue bend with three silver crescents) on banners.
Six tapestries: Sight, Hearing, Taste, Smell, Touch, and 'A mon seul desir'."""
import math, random
from embroider import fill, stem, dot, woven, g, rot, WOOL
from creatures import clip, limb, C, dots, scales
import scenery
from scenery import BW, BH, _wrap
from world4 import leaf
from world5 import millefleurs, tree, unicorn_steed, UPAD

T = 48
S = scenery.S
RED_FLOWERS = ("cream", "gold", "sky", "white", "rose", "green")

# ---------------------------------------------------------------- tiles
def island_top():
    """The blue flower island the Lady stands on: dark blue, flowered, with a green turf edge."""
    s = woven("M0,6 L48,6 L48,48 L0,48 Z", "indigo")
    s += millefleurs(2, 14, 46, 44, 5, 81, ("cream", "terra", "gold", "sky"))
    s += woven("M0,2 q4,-3 8,0 q4,3 8,0 q4,-3 8,0 q4,3 8,0 q4,-3 8,0 q4,3 8,0 L48,9 L0,9 Z", "green", "dark", 1.2)
    return s
S["island_top"] = (T, T, island_top, False)

def island_fill():
    return woven("M0,0 L48,0 L48,48 L0,48 Z", "indigo") + millefleurs(2, 4, 46, 44, 3, 83, ("cream", "terra", "gold"))
S["island_fill"] = (T, T, island_fill, False)

def coffer():
    """A gilded jewel coffer, from the Lady's casket (breaks like a brick)."""
    s = woven("M1,1 L47,1 L47,47 L1,47 Z", "ochre", "dark", 1.6)
    s += woven("M1,1 L47,1 L47,12 L1,12 Z", "gold", "dark", 1)
    s += stem("M1,24 L47,24 M24,12 L24,47", "brown", 1.4, False)
    for x, y in ((12, 18), (36, 18), (12, 36), (36, 36)):
        s += dot(x, y, 3, ("terra", "blue")[(x + y) % 2], "dark")
    return s
S["coffer"] = (T, T, coffer, False)

def garland():
    """A garland of flowers strung across: the plank of World 6."""
    s = stem("M0,9 Q24,15 48,9", "green", 3.2, False)
    for k in range(7):
        x = 3 + k * 7; y = 9 + math.sin(k / 6 * math.pi) * 4
        s += leaf(x, y + 2, (k % 2) * 60 - 30, 0.28, ("green", "sage")[k % 2])
        s += dot(x + 2, y, 2.6, ("terra", "cream", "gold", "rose")[k % 4], "dark")
    return s
S["garland"] = (T, 18, garland, False)

def mirror_pane(frame_c, glass_c):
    s = woven("M2,2 L46,2 L46,46 L2,46 Z", frame_c, "dark", 1.6)
    s += woven("M8,8 L40,8 L40,40 L8,40 Z", glass_c, "dark", 1)
    s += stem("M12,30 L28,12 M16,36 L36,14", "white", 1.6, False)   # a glint
    return s
S["mirror_gold"] = (T, T, lambda: mirror_pane("gold", "sky"), False)
S["mirror_silver"] = (T, T, lambda: mirror_pane("grey", "blue"), False)

def mirror_block():
    """The Lady's hand-mirror on a block: bump it from below and the gold panes and the silver panes change places."""
    s = woven("M1,1 L47,1 L47,47 L1,47 Z", "terra", "dark", 1.6)
    s += stem("M24,32 L24,44", "gold", 3.4, False)
    s += f'<circle cx="24" cy="20" r="13" fill="{WOOL["gold"]}" stroke="{WOOL["dark"]}" stroke-width="1.4"/>'
    s += f'<circle cx="24" cy="20" r="9" fill="{WOOL["sky"]}"/>' + stem("M19,24 L28,15", "white", 1.6, False)
    return s
S["mirror_block"] = (T, T, mirror_block, False)

def organ_pipe():
    """A great organ pipe (6-2): its flat top is the platform; it rises and falls with the musick."""
    s = woven("M10,8 L86,8 L86,300 L10,300 Z", "gold", "dark", 1.6)
    s += woven("M4,0 L92,0 L92,12 L4,12 Z", "ochre", "dark", 1.4)
    for x in range(22, 86, 16):
        s += stem(f"M{x},14 L{x},300", "ochre", 1.2, False)
    s += woven("M30,60 L66,60 L58,80 L38,80 Z", "dark")           # the pipe's mouth
    s += stem("M30,60 L66,60", "cream", 1.6, False)
    return s
S["organ_pipe"] = (96, 300, organ_pipe, False)

def basket():
    """A basket of flowers (6-3): land on it and it springs thee high."""
    s = woven("M4,20 L44,20 L40,47 L8,47 Z", "ochre", "dark", 1.6)
    for y in (27, 34, 41):
        s += stem(f"M6,{y} L42,{y}", "brown", 1.2, False)
    for k in range(6):
        x = 8 + k * 6.5
        s += leaf(x, 22, (k - 2.5) * 18, 0.35, ("green", "sage")[k % 2])
        s += dot(x, 12 + (k % 2) * 4, 3.4, ("terra", "rose", "gold", "cream")[k % 4], "dark")
    return s
S["basket"] = (T, T, basket, False)

def crescent_banner(x, y, h=90):
    """A Le Viste banner: red, with a blue bend bearing three silver crescents."""
    s = woven(f"M{x},{y} L{x + 50},{y} L{x + 50},{y + h} L{x + 25},{y + h - 12} L{x},{y + h} Z", "terra", "dark", 1.2)
    s += clip(f"bn{x}{y}", f"M{x},{y} L{x + 50},{y} L{x + 50},{y + h} L{x},{y + h} Z",
              woven(f"M{x - 4},{y + 10} L{x + 8},{y} L{x + 56},{y + h - 20} L{x + 44},{y + h - 6} Z", "blue"))
    for k in range(3):
        cx, cy = x + 12 + k * 13, y + 18 + k * 22
        s += f'<path d="M{cx - 5},{cy} a5,5 0 1,0 10,0 a4,3 0 1,1 -10,0 Z" fill="{WOOL["white"]}"/>'
    s += stem(f"M{x - 4},{y} L{x + 54},{y}", "brown", 3, False)
    return s

def rabbit(x, y, k=1.0):
    return g(fill("M0,10 Q4,0 14,2 Q22,4 20,12 Z", "buff", 0, "dark", 0.8) + fill("M16,4 L15,-6 L19,3 Z", "buff", 0, "dark", 0.6)
             + dot(17, 6, 0.6, "dark"), f"translate({x} {y}) scale({k})")

# ---------------------------------------------------------------- backgrounds
def red_ground(seed):
    s = woven(f"M0,0 L{BW},0 L{BW},{BH} L0,{BH} Z", "terra")
    s += millefleurs(0, 0, BW, BH, 700, seed, RED_FLOWERS)
    r = random.Random(seed)
    for _ in range(9):
        s += rabbit(r.uniform(20, BW - 40), r.uniform(60, BH - 40), r.uniform(0.9, 1.4))
    return s

def red_far():
    """The red millefleur ground, and the blue flower island with its trees."""
    s = red_ground(91)
    s += woven(f"M120,{BH - 30} Q480,{BH - 230} 840,{BH - 30} Z", "indigo")
    s += millefleurs(200, BH - 170, 760, BH - 40, 120, 92, ("cream", "terra", "gold", "sky"))
    for x, kind in ((260, "holly"), (700, "orange")):
        s += tree(x, BH - 80, kind, 240)
    s += crescent_banner(440, 80)
    return f'<g opacity="0.88">{s}</g>'
S["bg_red_far"] = (BW, BH, red_far, False)

def red_near():
    items = []
    for i, x in enumerate(range(80, BW, 320)):
        t = ""
        for k in range(5):
            t += leaf(math.cos(k) * 18, BH - 10 - k * 8, (k - 2) * 25, 1.1, ("green", "sage")[k % 2])
        t += millefleurs(-40, BH - 60, 40, BH - 8, 10, 100 + i, ("cream", "gold", "sky"))
        items.append((x, t))
    return _wrap(items)
S["bg_red_near"] = (BW, BH, red_near, False)

def organ_far():
    """Hearing: the Lady's portative organ upon a table on the blue island; the maid works the bellows."""
    s = red_ground(95)
    s += woven(f"M80,{BH - 30} Q480,{BH - 250} 880,{BH - 30} Z", "indigo")
    s += woven(f"M330,{BH - 120} L630,{BH - 120} L620,{BH - 100} L340,{BH - 100} Z", "brown", "dark", 1.4)    # table
    s += stem(f"M360,{BH - 100} L360,{BH - 40} M600,{BH - 100} L600,{BH - 40}", "brown", 5, False)
    for k in range(9):
        x = 400 + k * 18; h = 70 + abs(4 - k) * -8 + 60
        s += woven(f"M{x},{BH - 120} L{x},{BH - 120 - h} L{x + 12},{BH - 120 - h} L{x + 12},{BH - 120} Z", "gold", "dark", 1)
    s += woven(f"M390,{BH - 120} L580,{BH - 120} L580,{BH - 140} L390,{BH - 140} Z", "ochre", "dark", 1.2)
    s += crescent_banner(120, 70) + crescent_banner(790, 70)
    return f'<g opacity="0.88">{s}</g>'
S["bg_organ_far"] = (BW, BH, organ_far, False)

def pavilion_far():
    """'A mon seul desir': the Lady's blue tent strewn with golden tears, its flaps held open; the motto above."""
    s = red_ground(97)
    tent = f"M300,{BH - 20} L340,140 Q480,40 620,140 L660,{BH - 20} Z"
    s += woven(tent, "blue") + clip("tnt", tent, "".join(f'<path d="M{x},{y} q3,6 0,9 q-3,-3 0,-9 Z" fill="{WOOL["gold"]}"/>'
                                                          for x in range(310, 660, 26) for y in range(120, BH, 30)))
    s += stem(tent, "dark", 1.8)
    s += woven(f"M400,{BH - 20} Q480,260 560,{BH - 20} Z", "dark")
    s += woven("M330,110 L630,110 L630,150 L330,150 Z", "cream", "dark", 1.2)
    s += f'<text x="480" y="138" font-family="serif" font-size="26" text-anchor="middle" fill="{WOOL["dark"]}">A MON SEUL DESIR</text>'
    s += stem("M480,60 L480,18", "brown", 3, False) + crescent_banner(110, 90) + crescent_banner(800, 90)
    return f'<g opacity="0.9">{s}</g>'
S["bg_pavilion_far"] = (BW, BH, pavilion_far, False)

# ---------------------------------------------------------------- creatures
def lapdog(f):
    """The Lady's little white lapdog, with a belled collar: it yaps and scampers."""
    ext = f == 0
    s = stem("M8,12 Q2,6 6,2", "white", 3, False)
    s += limb(12, 18, 9, 3, "white", 35 if ext else -20, "paw", "white") + limb(30, 18, 9, 3, "white", -35 if ext else 20, "paw", "white")
    body = "M6,12 Q20,6 34,10 L36,20 Q22,24 10,22 Q4,18 6,12 Z"
    s += fill(body, "white", 170) + stem(body, "dark", 1.2)
    s += fill("M32,10 Q36,2 42,4 Q46,8 44,14 L36,16 Z", "white", 0, "dark", 1.1) + fill("M34,6 L32,14 L36,13 Z", "buff", 0, "dark", 0.7)
    s += dot(40, 8, 0.9, "dark") + stem("M33,14 Q36,17 39,15", "terra", 2.2, False) + dot(36, 17, 1.2, "gold", "dark")
    return s
C["lapdog"] = dict(title="Lappe-Dogge", size=(48, 30), frames=2, build=lapdog,
                   behaviour="scampers to and fro, yapping, and hops at the knight; stompable")

def fox(f):
    ext = f == 0
    s = stem("M10,16 Q0,14 2,6" if ext else "M10,16 Q-2,18 2,10", "terra", 5, False) + dot(2, 7 if ext else 10, 2.4, "white")
    s += limb(16, 22, 11, 3, "terra", 35 if ext else -20, "paw") + limb(40, 22, 11, 3, "terra", -35 if ext else 20, "paw")
    body = "M10,14 Q26,8 44,12 L46,22 Q30,26 14,24 Q8,20 10,14 Z"
    s += fill(body, "terra", 170) + clip(f"fxb{f}", body, fill("M24,22 Q34,26 46,20 L46,26 L24,26 Z", "cream", 0, None)) + stem(body, "dark", 1.2)
    s += fill("M42,12 Q48,4 54,8 L60,14 Q56,18 48,18 Z", "terra", 0, "dark", 1.1)
    s += fill("M46,8 L45,0 L50,6 Z", "terra", 0, "dark", 0.8) + dot(52, 10, 0.9, "dark") + dot(59.5, 14, 0.9, "dark")
    return s
C["fox"] = dict(title="Foxe", size=(62, 34), frames=2, build=fox,
                behaviour="slinks along, then darts at the knight when he turns his back; stompable")

def monkey(f):
    """A monkey from 'Taste', sitting and nibbling - or flinging a comfit (frame 1)."""
    s = stem("M10,34 Q2,30 4,20 Q6,12 12,14", "brown", 2.4, False)
    body = "M12,24 Q22,16 32,22 L34,38 Q22,42 12,38 Z"
    s += fill(body, "brown", 90) + fill("M18,28 Q22,24 28,28 L28,38 L18,38 Z", "buff", 0, None)
    s += fill("M16,12 Q16,2 24,2 Q32,2 32,12 Q32,20 24,21 Q16,20 16,12 Z", "brown", 0) + fill("M19,10 Q24,6 29,10 Q29,17 24,18 Q19,17 19,10 Z", "buff", 0, None)
    s += dot(22, 11, 0.9, "dark") + dot(26, 11, 0.9, "dark") + stem("M22,15 Q24,16.5 26,15", "dark", 0.8, False)
    s += dot(15, 9, 2.4, "buff", "dark") + dot(33, 9, 2.4, "buff", "dark")
    if f:
        s += fill("M30,24 L42,6 L45,9 L34,28 Z", "brown", 60, "dark", 1) + dot(44, 5, 2.8, "rose", "dark")
    else:
        s += fill("M30,26 L36,32 L32,35 L28,30 Z", "brown", 60, "dark", 1) + dot(33, 31, 2.2, "rose", "dark")
    s += limb(16, 38, 6, 3, "brown", 30) + limb(30, 38, 6, 3, "brown", -30)
    return s
C["monkey"] = dict(title="Monkeye", size=(48, 46), frames=2, build=monkey,
                   behaviour="sits and nibbles sweets; when the knight is in range it flings comfits in arcs; stompable")

def parakeet(f):
    up = f == 0
    wing = "M20,16 Q14,0 2,0 Q10,8 8,14 Q14,14 18,20 Z" if up else "M20,18 Q12,30 0,34 Q8,26 8,22 Q14,22 18,20 Z"
    s = fill(wing, "green", 40) + stem("M2,22 L-2,30 M4,22 L2,32", "green", 1.6, False)
    body = "M6,20 Q16,12 30,14 Q38,18 36,24 Q26,28 14,26 Q6,25 6,20 Z"
    s += fill(body, "sage", 0) + stem(body, "dark", 1.2)
    s += fill("M28,12 Q32,4 38,6 Q42,10 38,16 Z", "green", 0, "dark", 1) + stem("M30,16 Q34,18 38,15", "terra", 1.8, False)
    s += fill("M38,8 Q44,10 40,15 Z", "terra", 0, "dark", 0.8) + dot(34, 9, 0.9, "dark")
    return s
C["parakeet"] = dict(title="Popinjaye", size=(46, 36), frames=2, build=parakeet,
                     behaviour="a parakeet: flutters in a wavering line across the sky toward the knight; stompable")

def comfit(f):
    return f'<ellipse cx="8" cy="8" rx="6" ry="4.5" fill="{WOOL["rose" if f == 0 else "cream"]}" stroke="{WOOL["dark"]}" stroke-width="1"/>' + dot(6, 6.5, 1.2, "white")
C["comfit"] = dict(title="Comfit", size=(16, 16), frames=2, build=comfit, behaviour="a sugared sweet flung by monkeys")

# ---------------------------------------------------------------- the twin boss: Ye Lyon and ye Unicorne
import bosses
def heraldic_lion(f):
    """The Lady's lion, a heraldic supporter come to life: 0 standing, 1 crouched to spring (stomp him now), 2 leaping, 3 stunned."""
    s = ""
    leap = f == 2
    fl = ((10, -10), (-25, 30), (-50, 45), (0, 0))[f]
    s += bosses.big_limb(44, 90, 32, 11, "ochre", fl[1]) + bosses.big_limb(116, 86, 32, 11, "ochre", fl[0])
    s += stem("M30,84 Q4,80 10,50 Q14,34 26,40", "gold", 6, False) + dot(26, 40, 7, "brown", "dark")        # tail with tuft
    body = "M28,60 Q50,40 100,46 Q128,52 130,74 Q120,96 80,96 Q40,98 28,84 Z"
    s += fill(body, "gold", 170) + stem(body, "dark", 1.8)
    s += bosses.big_limb(56, 90, 32, 11, "gold", -fl[1]) + bosses.big_limb(124, 86, 32, 11, "gold", -fl[0])
    for k in range(9):   # the curled mane
        a = -1.6 + k * 0.42
        s += dot(128 + math.cos(a) * 22, 46 + math.sin(a) * 22, 9, ("brown", "ochre")[k % 2], "dark")
    s += fill("M116,34 Q130,24 146,32 Q156,44 150,58 Q138,66 124,60 Q114,50 116,34 Z", "gold", 0, "dark", 1.6)   # face
    s += dot(138, 40, 2.2, "white" if f != 3 else "gold", "dark") + dot(139, 40, 1, "dark")
    s += fill("M148,52 L160,56 L148,58 Z", "terra", 0, "dark", 0.9)                                        # tongue
    s += stem("M136,52 Q142,56 148,52", "dark", 1.4, False)
    if f == 3:
        for k in range(5):
            a = k * 1.2566; s += dot(136 + math.cos(a) * 20, 14 + math.sin(a) * 6, 3, "gold", "dark")
    return g(s, "translate(0 -10) rotate(-8 80 80)" if leap else ("translate(0 14)" if f == 1 else ""))
bosses.B["heraldic_lion"] = dict(title="Ye Lyon", size=(170, 140), frames=4, build=heraldic_lion,
                                 behaviour="twin boss with the unicorn: crouches, then springs at the knight; stomp him while he crouches")

def lady_unicorn(f):
    """The Lady's unicorn, white and unspotted: 0/1 charging, 2 rearing (stomp it then), 3 stunned."""
    pose = ("walk1", "walk2", "rear", "idle")[f]
    from world5 import POM
    # no pomegranate spots on the Lady's unicorn: draw it again without them
    import allies, world5
    deco = world5.unicorn_deco(True)
    for x, y in POM:
        deco = deco.replace(dot(x, y, 1.6, "terra"), "")
    s = allies.horse(pose, body_c="white", far_c="cream", mane="cream", saddle=False, deco=deco)
    s = g(s, f"translate(0 {UPAD})")
    if f == 3:
        for k in range(5):
            a = k * 1.2566; s += dot(88 + math.cos(a) * 14, 12 + math.sin(a) * 4, 2, "gold", "dark")
    return g(s, "scale(1.6)")
bosses.B["lady_unicorn"] = dict(title="Ye Unicorne", size=(170, 172), frames=4, build=lady_unicorn,
                                behaviour="twin boss with the lion: rears, then charges across the tent; stomp it while it rears")

# ---------------------------------------------------------------- end buildings
import castles
from castles import door, window, pennant
def lady_tent():
    """The Lady's small blue tent, strewn with golden tears."""
    tent = "M30,252 L60,90 Q120,40 180,90 L210,252 Z"
    s = woven(tent, "blue") + clip("ltn", tent, "".join(f'<path d="M{x},{y} q3,6 0,9 q-3,-3 0,-9 Z" fill="{WOOL["gold"]}"/>'
                                                         for x in range(40, 210, 20) for y in range(80, 252, 24))) + stem(tent, "dark", 1.8)
    s += stem("M120,58 L120,10", "brown", 3, False) + pennant(120, 6, "terra", 20)
    s += door(120, 252, 40, 80)
    return s
castles.C["castle_lady_tent"] = dict(size=(240, 252), title="Ye Ladye's Tente", fn=lady_tent)

def music_hall():
    """A hall of musick: its great window shows the organ pipes within."""
    s = castles.wall(40, 100, 200, 256, "cream", "buff")
    s += woven("M30,102 L120,40 L210,102 Z", "blue", "dark", 1.6)
    s += woven("M80,120 Q120,92 160,120 L160,186 L80,186 Z", "dark")
    for k in range(6):
        s += woven(f"M{88 + k * 12},186 L{88 + k * 12},{140 + abs(k - 2.5) * 8:.0f} L{96 + k * 12},{140 + abs(k - 2.5) * 8:.0f} L{96 + k * 12},186 Z", "gold", None)
    s += door(120, 256, 36, 56)
    return s
castles.C["castle_music_hall"] = dict(size=(240, 256), title="Ye Halle of Musick", fn=music_hall)

def bower():
    """A rose bower in the Lady's garden, a trellis arch twined with flowers."""
    s = woven("M30,252 L30,110 Q120,10 210,110 L210,252 L180,252 L180,116 Q120,44 60,116 L60,252 Z", "brown", "dark", 1.4)
    for k in range(18):
        a = math.pi * k / 17
        x, y = 120 - math.cos(a) * 75, 120 - math.sin(a) * 70
        s += leaf(x, y + 6, k * 20, 0.6, ("green", "sage")[k % 2]) + dot(x, y, 4, ("terra", "rose", "cream")[k % 3], "dark")
    s += door(120, 252, 46, 90)
    return s
castles.C["castle_bower"] = dict(size=(240, 252), title="Ye Rose Bower", fn=bower)

for k in ("castle_lady_tent", "castle_music_hall", "castle_bower"):
    v = castles.C[k]; S[k] = (v["size"][0], v["size"][1], v["fn"], False)
