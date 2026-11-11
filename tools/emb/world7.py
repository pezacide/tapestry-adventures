"""World 7: the Apocalypse Tapestry (Angers). Scenes framed against alternating red and blue grounds, with clouds
along the top; angels sounding the seven trumpets, falling stars, crowned locusts with men's faces, the four horsemen,
the seven-headed beast - and at the last, the Cat King's Great Hall."""
import math, random
from embroider import fill, stem, dot, woven, g, rot, WOOL
from creatures import clip, limb, C, dots, scales, cat_head
import scenery, allies
from scenery import BW, BH, _wrap
from world4 import leaf

T = 48
S = scenery.S

# ---------------------------------------------------------------- tiles
def apoc_top():
    s = woven("M0,6 L48,6 L48,48 L0,48 Z", "buff")
    for x, y in ((8, 26), (30, 34), (40, 18), (18, 40)):
        s += f'<circle cx="{x}" cy="{y}" r="2.4" fill="{WOOL["ochre"]}" stroke="{WOOL["brown"]}" stroke-width="0.8"/>'
    s += woven("M0,2 q4,-3 8,0 q4,3 8,0 q4,-3 8,0 q4,3 8,0 q4,-3 8,0 q4,3 8,0 L48,10 L0,10 Z", "green", "dark", 1.2)
    for x in (6, 22, 38):
        s += stem(f"M{x},4 l-2,-5 M{x},4 l2,-6 M{x},4 l4,-4", "green", 1.2, False)
    return s
S["apoc_top"] = (T, T, apoc_top, False)

def apoc_fill():
    s = woven("M0,0 L48,0 L48,48 L0,48 Z", "ochre")
    for x, y in ((10, 10), (34, 18), (20, 34), (42, 40)):
        s += f'<ellipse cx="{x}" cy="{y}" rx="4" ry="2.6" fill="{WOOL["buff"]}" stroke="{WOOL["brown"]}" stroke-width="0.8"/>'
    return s
S["apoc_fill"] = (T, T, apoc_fill, False)

def gothic_block():
    """A carved stone block with a quatrefoil (breaks like a brick)."""
    s = woven("M1,1 L47,1 L47,47 L1,47 Z", "grey", "dark", 1.6)
    for a in range(4):
        cx, cy = 24 + 7 * math.cos(a * math.pi / 2), 24 + 7 * math.sin(a * math.pi / 2)
        s += f'<circle cx="{cx:.1f}" cy="{cy:.1f}" r="7" fill="{WOOL["blue"]}" stroke="{WOOL["dark"]}" stroke-width="1"/>'
    s += dot(24, 24, 4, "gold", "dark")
    return s
S["gothic_block"] = (T, T, gothic_block, False)

def cloud_scroll():
    """A band of the Angers clouds: the plank of World 7."""
    s = woven("M0,6 q6,-6 12,0 q6,-6 12,0 q6,-6 12,0 q6,-6 12,0 L48,16 q-6,4 -12,0 q-6,4 -12,0 q-6,4 -12,0 q-6,4 -12,0 Z", "white", "sky", 1.4)
    s += stem("M2,10 q5,-3 10,0 q5,-3 10,0 q5,-3 10,0 q5,-3 10,0", "sky", 1, False)
    return s
S["cloud_scroll"] = (T, 18, cloud_scroll, False)

# ---------------------------------------------------------------- props and hazards
def trumpet_angel():
    """An angel sounding one of the seven trumpets (it blasts the knight backward)."""
    s = fill("M40,40 Q18,10 4,4 Q12,24 10,38 Q22,36 34,52 Z", "gold", 50) + stem("M40,40 L8,8 M38,44 L10,30", "dark", 0.8, False)   # wing
    robe = "M34,40 Q46,34 56,40 L62,96 Q46,100 30,96 Z"
    s += fill(robe, "cream", 90) + stem("M32,90 L60,90", "gold", 2, False)
    s += dot(46, 30, 9, "buff", "dark") + stem("M37,26 Q46,14 55,26", "gold", 3, False)   # head and hair
    s += f'<circle cx="46" cy="28" r="13" fill="none" stroke="{WOOL["gold"]}" stroke-width="2"/>'   # halo
    s += stem("M52,34 L96,24", "gold", 3.4, False) + fill("M92,16 L100,24 L92,32 Q94,24 92,16 Z", "gold", 0, "dark", 1)   # the trumpet
    s += dot(54, 34, 2.8, "buff", "dark")
    return s
S["trumpet_angel"] = (104, 104, trumpet_angel, False)

def star(f):
    pts = []
    for k in range(10):
        r = 14 if k % 2 == 0 else 6
        a = -math.pi / 2 + k * math.pi / 5 + f * 0.3
        pts.append(f"{16 + r * math.cos(a):.1f},{16 + r * math.sin(a):.1f}")
    s = f'<polygon points="{" ".join(pts)}" fill="{WOOL["gold"]}" stroke="{WOOL["terra"]}" stroke-width="1.6"/>'
    s += dot(16, 16, 3, "white")
    return s
C["star"] = dict(title="Falling Starre", size=(32, 32), frames=2, build=star, behaviour="falls from heaven at the sounding of a trumpet")

def throne():
    """The Cat King's throne (he stands upon it to fling his soup)."""
    s = ""
    for x in (24, 132):
        s += fill(f"M{x},4 L{x + 8},4 L{x + 8},130 L{x},130 Z", "brown", 90, "dark", 1.4)
        s += fill(f"M{x - 2},0 Q{x + 4},-8 {x + 12},-4 L{x + 14},2 Q{x + 6},8 {x - 2},2 Z", "gold", 0, "dark", 1.1)
    s += woven("M32,40 L132,40 L132,96 L32,96 Z", "terra", "dark", 1.4)
    s += stem("M36,44 L128,44", "gold", 2, False)
    s += woven("M20,96 L144,96 L144,112 L20,112 Z", "ochre", "dark", 1.4)
    for i, x in enumerate(range(26, 140, 16)):
        s += fill(f"M{x},100 L{x + 8},100 L{x + 8},108 L{x},108 Z", ("terra", "blue")[i % 2], 0, "dark", 0.8)
    s += stem("M30,112 L30,130 M134,112 L134,130", "brown", 5, False)
    return g(s, "translate(0 8)")
S["cat_throne"] = (164, 140, throne, False)

# ---------------------------------------------------------------- backgrounds
def panels(x0, w, seed):
    """The Angers grounds: alternating red and blue panels, each strewn with tiny vine scrolls and gold stars."""
    s = ""
    r = random.Random(seed)
    for i, x in enumerate(range(x0, x0 + w, 240)):
        s += woven(f"M{x},0 L{x + 240},0 L{x + 240},{BH} L{x},{BH} Z", ("terra", "blue")[i % 2])
        for _ in range(40):
            px, py = r.uniform(x + 8, x + 232), r.uniform(70, BH - 20)
            s += stem(f"M{px:.0f},{py:.0f} q4,-6 8,0 q4,6 8,0", "gold" if i % 2 else "sky", 1, False)
        for _ in range(16):
            s += dot(r.uniform(x + 8, x + 232), r.uniform(70, BH - 20), 1.6, "gold")
    return s

def clouds_top():
    s = woven(f"M0,0 L{BW},0 L{BW},50 " + " ".join(f"Q{BW - k * 40 - 20},{80 if k % 2 else 64} {BW - (k + 1) * 40},50" for k in range(24)) + " Z", "white", "sky", 1.6)
    for k in range(24):
        s += stem(f"M{k * 40 + 6},48 q14,-10 28,0", "sky", 1.2, False)
    return s

def apoc_far():
    s = panels(0, BW, 7) + clouds_top()
    s += woven(f"M0,{BH - 120} Q{BW * 0.25},{BH - 170} {BW * 0.5},{BH - 120} T{BW},{BH - 120} L{BW},{BH} L0,{BH} Z", "green")
    for x in (120, 480, 840):   # little stylised trees, as in the Angers scenes
        s += stem(f"M{x},{BH - 120} L{x},{BH - 200}", "brown", 5, False)
        for k in range(7):
            s += leaf(x + math.cos(k) * 22, BH - 200 + math.sin(k * 2) * 10, (k - 3) * 30, 0.9, ("green", "sage")[k % 2])
    return f'<g opacity="0.9">{s}</g>'
S["bg_apoc_far"] = (BW, BH, apoc_far, False)

def apoc_near():
    items = []
    for i, x in enumerate(range(100, BW, 320)):
        t = ""
        for k in range(5):
            t += leaf(math.cos(k) * 18, BH - 10 - k * 8, (k - 2) * 25, 1.1, ("green", "sage")[k % 2])
        items.append((x, t))
    return _wrap(items)
S["bg_apoc_near"] = (BW, BH, apoc_near, False)

def hall_far():
    """The Cat King's Great Hall: red and blue hangings, banners with his arms (a crowned cat), and a cauldron of soup."""
    s = panels(0, BW, 11)
    for x in range(0, BW, 160):   # arcade
        s += woven(f"M{x},0 L{x + 20},0 L{x + 20},{BH} L{x},{BH} Z", "grey", "dark", 1)
        s += stem(f"M{x + 20},120 Q{x + 90},40 {x + 160},120", "grey", 10, False)
    for x in (110, 430, 750):   # banners: a crowned cat
        s += woven(f"M{x},140 L{x + 80},140 L{x + 80},300 L{x + 40},280 L{x},300 Z", "gold", "dark", 1.4)
        s += g(cat_head(), f"translate({x + 14} 170) scale(1.25)")
        s += fill(f"M{x + 26},168 L{x + 28},158 L{x + 34},164 L{x + 40},154 L{x + 46},164 L{x + 52},158 L{x + 54},168 Z", "terra", 0, "dark", 1)
    return f'<g opacity="0.9">{s}</g>'
S["bg_hall_far"] = (BW, BH, hall_far, False)

# ---------------------------------------------------------------- creatures
def locust(f):
    """A locust of the Apocalypse: a horse's body, a man's face under a golden crown, a woman's hair, wings, a scorpion's tail."""
    up = f == 0
    wing = "M28,16 Q20,0 6,0 Q14,8 12,14 Q20,14 26,20 Z" if up else "M28,20 Q18,30 4,34 Q12,26 12,22 Q20,22 26,20 Z"
    s = fill(wing, "sky", 40) + stem("M28,16 L8,2" if up else "M28,20 L6,32", "dark", 0.8, False)
    s += stem("M8,22 Q0,20 2,10 Q4,4 10,6", "olive", 2.6, False) + fill("M9,3 L14,6 L9,9 Z", "dark", 0, None)   # scorpion tail
    body = "M8,22 Q20,14 38,18 L40,28 Q26,32 12,30 Q6,26 8,22 Z"
    s += fill(body, "olive", 170) + clip(f"lcb{f}", body, scales(6, 14, 42, 32, 4, "green", 0.7)) + stem(body, "dark", 1.2)
    s += limb(14, 28, 7, 2.2, "olive", 20 if up else -20, "hoof") + limb(34, 28, 7, 2.2, "olive", -20 if up else 20, "hoof")
    s += stem("M38,14 Q32,24 36,30", "brown", 3, False)                                   # a woman's hair
    s += fill("M38,10 Q38,4 44,4 Q50,4 50,11 Q50,18 44,19 Q38,18 38,10 Z", "buff", 0, "dark", 1)   # a man's face
    s += dot(42, 10, 0.8, "dark") + dot(47, 10, 0.8, "dark") + stem("M42,15 L48,15", "dark", 0.7, False) + stem("M41,13 L49,16", "dark", 0.5, False)
    s += fill("M38,5 L39,-1 L42,3 L44,-2 L46,3 L49,-1 L50,5 Z", "gold", 0, "dark", 0.8)       # crown
    return g(s, "translate(2 4)")
C["locust"] = dict(title="Crowned Locuste", size=(56, 40), frames=2, build=locust,
                   behaviour="swarms: hovers about its place, then drifts after the knight; stompable")

RIDERS = {   # the four horsemen: horse colour, robe, and what each bears
    "white": ("white", "cream", "blue"), "red": ("terra", "terra", "gold"),
    "black": ("dark", "indigo", "grey"), "pale": ("sage", "grey", "buff"),
}
def rider(kind):
    _, robe, trim = RIDERS[kind]
    s = fill("M30,38 Q38,32 46,38 L48,64 L28,64 Z", robe, 90) + stem("M28,60 L48,60", trim, 2, False)
    if kind == "pale":   # Death: a grey corpse with a hollow face
        s += fill("M32,22 Q32,14 38,14 Q44,14 44,22 Q44,30 38,31 Q32,30 32,22 Z", "white", 0, "dark", 1) + dot(36, 21, 1.6, "dark") + dot(41, 21, 1.6, "dark") + stem("M35,27 L41,27", "dark", 0.8, False)
    else:
        s += fill("M32,22 Q32,14 38,14 Q44,14 44,22 Q44,30 38,31 Q32,30 32,22 Z", "buff", 0, "dark", 1) + dot(40, 21, 0.8, "dark")
        s += fill("M32,16 L33,9 L36,13 L38,8 L40,13 L43,9 L44,16 Z", "gold", 0, "dark", 0.8)   # crowned
    if kind == "white":   # a bow
        s += stem("M52,24 Q62,40 52,56", "brown", 2.4, False) + stem("M52,24 L52,56", "cream", 0.8, False) + dot(48, 42, 2.2, "buff", "dark")
    elif kind == "red":   # a great sword raised
        s += stem("M46,42 L60,8", "grey", 3, False) + stem("M44,38 L52,42", "gold", 2.4, False) + dot(46, 42, 2.2, "buff", "dark")
    elif kind == "black":   # a pair of scales
        s += stem("M48,40 L48,22 M38,22 L58,22", "gold", 1.6, False) + stem("M38,22 L36,30 L40,30 Z M58,22 L56,30 L60,30 Z", "gold", 1.2, False)
    else:   # Death's spear
        s += stem("M44,44 L62,14", "brown", 2, False)
    return s

def horseman(kind):
    horse_c = RIDERS[kind][0]
    def build(f):
        pose = ("walk1", "walk2")[f]
        return g(allies.horse(pose, body_c=horse_c, far_c=horse_c, mane="brown" if horse_c != "dark" else "grey",
                              rider=rider(kind), saddle=True), f"translate(0 {allies.MOUNT_PAD})")
    return build
for k, title in (("white", "Ye Ryder on ye White Horse"), ("red", "Ye Ryder on ye Red Horse"),
                 ("black", "Ye Ryder on ye Blacke Horse"), ("pale", "Death on ye Pale Horse")):
    C[f"horseman_{k}"] = dict(title=title, size=(allies.HW, allies.HH + allies.MOUNT_PAD), frames=2, build=horseman(k),
                              behaviour="one of the four horsemen: gallops in from behind and across the realm, leaping gaps; leap him, or stomp him")

# ---------------------------------------------------------------- bosses
import bosses
def hydra_body(f):
    """Ye Beaste of Seven Heddes: a leopard's body, a bear's feet, ten horns; red. The heads are separate sprites."""
    s = ""
    for x, a in ((60, 10), (200, -10)):
        s += bosses.big_limb(x, 110, 34, 16, "brown", a)
    s += stem("M30,100 Q-4,96 6,60 Q14,40 30,50", "terra", 10, False) + stem("M30,100 Q-4,96 6,60 Q14,40 30,50", "dark", 1, False)
    body = "M28,70 Q60,30 160,36 Q240,40 250,90 Q230,130 140,128 Q50,130 28,100 Z"
    s += fill(body, "terra", 160) + clip("hyb", body, dots(20, 30, 260, 130, 14, 4, "dark") + scales(20, 30, 260, 130, 16, "rose", 0.8))
    s += stem(body, "dark", 2)
    for x, a in ((80, -10), (220, 10)):
        s += bosses.big_limb(x, 112, 34, 16, "terra", a)
    s += woven("M180,44 Q210,20 248,40 L250,70 Q220,60 190,68 Z", "terra", "dark", 1.4)   # the shoulders the necks rise from
    if f == 1:   # wounded: flash
        s += f'<path d="{body}" fill="{WOOL["white"]}" opacity="0.35"/>'
    return s
bosses.B["hydra_body"] = dict(title="Ye Beaste of Seven Heddes", size=(270, 150), frames=2, build=hydra_body,
                              behaviour="boss body (the heads and necks are drawn separately)")

def hydra_head(f):
    """One of the seven crowned heads: 0 watching, 1 jaws open (spitting fire), 2 drooping, dazed (strike it now)."""
    s = fill("M6,22 Q8,8 26,8 Q44,8 50,20 L56,26 Q50,34 40,34 Q20,40 8,32 Z", "terra", 30, "dark", 1.4)
    if f == 1:
        s = fill("M6,22 Q8,8 26,8 Q44,8 50,18 L58,14 L52,26 L58,38 Q46,34 40,34 Q20,40 8,32 Z", "terra", 30, "dark", 1.4)
        s += fill("M44,24 L58,16 L54,26 L58,36 Z", "dark", 0, None) + stem("M48,20 L54,24 M48,32 L54,30", "white", 1.2, False)
    s += dot(30, 17, 3, "gold" if f != 2 else "white", "dark") + dot(31, 17, 1.2, "dark")
    s += fill("M14,10 L16,0 L20,7 L24,-2 L28,7 L32,0 L34,10 Z", "gold", 0, "dark", 1)   # a crown upon each head
    if f == 2:
        for k in range(4):
            a = k * 1.57; s += dot(26 + math.cos(a) * 14, 0 + math.sin(a) * 4, 2, "gold", "dark")
    return g(s, "translate(0 8)")
bosses.B["hydra_head"] = dict(title="A Crowned Hedde", size=(60, 48), frames=3, build=hydra_head,
                              behaviour="one of seven heads: rises, spits fire, then droops dazed - strike it then")

def hydra_neck(f):
    return f'<circle cx="10" cy="10" r="8.5" fill="{WOOL["terra"]}" stroke="{WOOL["dark"]}" stroke-width="1.4"/>' + dot(10, 10, 3, "rose")
C["hydra_neck"] = dict(title="neck", size=(20, 20), frames=1, build=hydra_neck, behaviour="a segment of a neck of the seven-headed beast")

def cat_king_afoot(f):
    """The Cat King on his feet: 0 standing (ladle low), 1 flinging soup (ladle high), 2/3 running, 4 dazed."""
    run = f in (2, 3)
    s = ""
    legs = ((-15, 15), (15, -15))[f - 2] if run else (0, 0)
    s += limb(42, 96, 22, 7, "grey", legs[0], "paw", "grey") + limb(58, 96, 22, 7, "grey", legs[1], "paw", "grey")
    cloak = "M30,50 Q18,90 26,110 L74,110 Q82,90 70,50 Q50,40 30,50 Z"
    s += fill(cloak, "terra", 90) + clip(f"ckf{f}", cloak, stem("M22,106 L80,106", "gold", 5, False)) + stem(cloak, "dark", 1.6)
    s += fill("M38,52 Q50,46 62,52 Q64,86 50,100 Q36,86 38,52 Z", "cream", 90, None)
    s += stem("M30,104 Q10,108 6,90 Q4,80 12,78", "grey", 4, False)   # tail
    lad = stem("M0,0 L0,-46", "gold", 3, False) + fill("M-7,-46 Q-7,-56 0,-56 Q7,-56 7,-46 Q0,-41 -7,-46 Z", "gold", 0, "dark", 1) + dot(0, -50, 1.8, "sage")
    s += g(lad, f"translate(70 {56 if f == 1 else 70}) rotate({-20 if f == 1 else 30})")
    s += dot(70, 60 if f == 1 else 72, 3.6, "grey", "dark")
    s += g(cat_head(), "translate(23 -4) scale(1.3)")
    s += fill("M38,8 L40,-2 L45,4 L50,-4 L55,4 L60,-2 L62,8 Q50,4 38,8 Z", "gold", 0, "dark", 1.2)
    if f == 4:
        for k in range(5):
            a = k * 1.2566; s += dot(50 + math.cos(a) * 18, -8 + math.sin(a) * 5, 2.4, "gold", "dark")
    return g(s, "translate(4 14)")
bosses.B["cat_king_afoot"] = dict(title="The Cat King", size=(104, 132), frames=5, build=cat_king_afoot,
                                  behaviour="final boss afoot: flings soup from his throne, summons Soup Cats, then leaps down and prowls - stomp him then")

# ---------------------------------------------------------------- end buildings
import castles
from castles import door, window, pennant
def heaven_gate():
    """A gate of heaven, among the clouds (end of 7-1)."""
    s = woven("M30,252 L30,90 Q120,0 210,90 L210,252 Z", "gold", "dark", 1.8)
    s += woven("M70,252 L70,130 Q120,70 170,130 L170,252 Z", "sky", "dark", 1.4)
    for k in range(8):
        a = math.pi * (k + 0.5) / 8
        s += dot(120 - math.cos(a) * 82, 96 - math.sin(a) * 70, 6, ("terra", "blue")[k % 2], "dark")
    s += door(120, 252, 40, 80)
    return s
castles.C["castle_heaven_gate"] = dict(size=(240, 252), title="Ye Gate of Heaven", fn=heaven_gate)

def city_gate():
    """A gate of the Holy City, twelve-gated and jewelled (end of 7-2)."""
    s = castles.wall(20, 80, 220, 256, "cream", "gold") + castles.crenels(16, 224, 80, "gold")
    for i, x in enumerate((50, 120, 190)):
        s += woven(f"M{x - 18},256 L{x - 18},160 Q{x},132 {x + 18},160 L{x + 18},256 Z", ("blue", "terra", "green")[i], "dark", 1.2)
    for x in range(30, 214, 16):
        s += dot(x, 100, 3, ("terra", "blue", "green", "gold")[(x // 16) % 4], "dark")
    s += door(120, 256, 36, 70)
    return s
castles.C["castle_city_gate"] = dict(size=(240, 256), title="Ye Gate of ye Holy Citie", fn=city_gate)

def cat_keep():
    """The outer gate of the Cat King's keep: grey stone, cat-eared towers (end of 7-3)."""
    s = castles.wall(40, 90, 200, 260, "grey", "dark") + castles.crenels(36, 204, 90, "terra")
    for x in (40, 200):
        s += castles.wall(x - 24, 50, x + 24, 260, "grey", "dark")
        s += fill(f"M{x - 24},52 L{x - 18},24 L{x - 6},50 Z", "grey", 0, "dark", 1.4) + fill(f"M{x + 24},52 L{x + 18},24 L{x + 6},50 Z", "grey", 0, "dark", 1.4)
        s += dot(x - 8, 70, 3, "gold", "dark") + dot(x + 8, 70, 3, "gold", "dark")   # cat's eyes for windows
    s += pennant(120, 40, "terra", 26)
    s += door(120, 260, 40, 80, portcullis=True)
    return s
castles.C["castle_cat_keep"] = dict(size=(240, 264), title="Ye Keep of ye Cat King", fn=cat_keep)

for k in ("castle_heaven_gate", "castle_city_gate", "castle_cat_keep"):
    v = castles.C[k]; S[k] = (v["size"][0], v["size"][1], v["fn"], False)
