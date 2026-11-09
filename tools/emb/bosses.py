"""Bosses: the Cockatrice, the Great Dragon, and the Cat King. (The Island-Whale lives in creatures.py.)"""
import math
from embroider import fill, mail, stem, dot, doc, g, rot, WOOL
from creatures import limb, clip, scales, cat_head, flame

B = {}

def big_limb(x, y, length, w, color, angle, claws=True):
    d = f"M{x - w / 2},{y} L{x + w / 2},{y} L{x + w * 0.35},{y + length} L{x - w * 0.35},{y + length} Z"
    s = fill(d, color, 90, "dark", 1.4)
    if claws:
        s += stem(f"M{x},{y + length} l6,4 M{x},{y + length} l-2,6 M{x},{y + length} l8,-1 M{x},{y + length} l-6,3", "dark", 1.6, False)
    return g(s, rot(angle, x, y))

# ---------------- Cockatrice ----------------
def cockatrice(f):
    atk = f == 1
    s = ""
    tail = "M62,100 Q20,112 18,80 Q16,50 44,56 Q60,62 50,78 Q42,86 36,78"
    s += stem(tail, "green", 14, False) + f'<path d="{tail}" fill="none" stroke="{WOOL["olive"]}" stroke-width="10" stroke-dasharray="1.2 4"/>' + stem(tail, "dark", 0.8, False)
    s += fill("M36,78 L28,72 L34,86 Z", "terra", 0, "dark", 1)
    wing = ("M82,62 Q58,6 30,0.5 Q40,20 32,30 Q48,32 52,44 Q66,42 76,66 Z" if atk else
            "M82,62 Q62,26 40,14 Q46,30 40,38 Q54,40 56,50 Q68,48 76,66 Z")
    s += fill(wing, "terra", 40) + stem("M82,62 L34,4 M82,62 L36,30 M82,62 L54,46", "dark", 1, False)
    s += big_limb(84, 104, 30, 7, "gold", 6) + big_limb(100, 104, 30, 7, "ochre", -8)
    body = "M56,72 Q68,48 100,50 Q124,56 121,84 Q112,108 84,110 Q60,106 56,72 Z"
    s += fill(body, "ochre", 150) + clip("ckb", body, scales(50, 50, 126, 112, 8, "brown", 1))
    s += fill("M104,56 Q108,36 118,28 Q130,22 138,30 Q142,40 133,46 Q122,50 116,72 Z", "gold", 120)
    s += fill("M116,28 Q115,19 121,20 Q123,12 129,16 Q133,9 138,17 Q141,23 137,28 Z", "terra", 0, "dark", 1.1)
    s += fill("M117,15 L119,5 L123.5,11 L127.5,3 L131.5,11 L136,5 L137,15 Q127,12 117,15 Z", "gold", 0, "dark", 1.1)
    s += dot(127.5, 9.5, 1.2, "blue")
    if atk:
        s += fill("M137,31 L152,25 L139,35 Z", "ochre", 0, "dark", 1) + fill("M137,37 L151,44 L138,40 Z", "ochre", 0, "dark", 1)
        s += stem("M136,31 L158,22 M136,32 L159,32 M136,33 L157,41", "gold", 1, False)   # petrifying gaze
    else:
        s += fill("M137,31 L150,36 L137,40 Z", "ochre", 0, "dark", 1)
    s += fill("M134,41 Q138,50 132,52 Q129,47 134,41 Z", "terra", 0, "dark", 0.9)
    s += dot(131, 31, 2.2, "gold", "dark") + dot(131.5, 31, 1, "dark")
    return s
B["cockatrice"] = dict(title="The Cockatrice", size=(160, 150), frames=2, build=cockatrice,
                       behaviour="boss; struts and pecks, its gaze (frame 2) turns a knight to stone for a moment; stomp its crown three times")

# ---------------- Great Dragon ----------------
def dragon(f):
    wing_up, fire = f != 1, f == 2
    s = ""
    wing = ("M132,74 Q112,18 70,2 Q78,24 70,34 Q50,30 34,38 Q52,50 54,62 Q92,52 112,82 Z" if wing_up else
            "M132,78 Q100,110 60,150 Q66,130 52,124 Q66,116 64,104 Q90,108 104,90 Z")
    ribs = ("M132,74 L72,4 M132,74 L36,38 M132,74 L56,60" if wing_up else "M132,78 L60,148 M132,78 L52,124 M132,78 L66,104")
    s += fill(wing, "olive", 40) + stem(ribs, "dark", 1.2, False)
    tail = "M66,100 Q20,104 12,122 Q8,140 26,140 Q40,138 36,126"
    s += stem(tail, "terra", 11, False) + f'<path d="{tail}" fill="none" stroke="{WOOL["gold"]}" stroke-width="3" stroke-dasharray="3 5"/>' + stem(tail, "dark", 0.8, False)
    s += fill("M36,126 L44,118 L46,132 Z", "gold", 0, "dark", 1.2)
    s += big_limb(86, 112, 28, 10, "olive", 8) + big_limb(160, 106, 28, 10, "olive", -8)
    body = "M60,92 Q100,62 160,70 Q184,80 178,104 Q152,122 96,122 Q64,118 60,92 Z"
    s += fill(body, "terra", 160) + clip("dgb", body, scales(56, 62, 186, 124, 9, "dark", 1) + fill("M60,108 Q120,124 180,100 L180,130 L60,130 Z", "gold", 0, "dark", 1.2))
    s += big_limb(100, 112, 30, 11, "terra", -6) + big_limb(170, 104, 30, 11, "terra", 10)
    neck = "M160,76 Q176,46 196,36 Q212,30 222,38 L232,46 Q236,52 230,55 L218,55 Q204,57 186,98 Z"
    s += fill(neck, "terra", 120) + clip("dgn", neck, scales(150, 30, 240, 100, 7, "dark", 0.9))
    if fire:
        s += fill("M218,55 L236,62 Q226,66 214,60 Z", "terra", 0, "dark", 1.4)
    s += stem("M205,36 Q200,22 190,18 M212,34 Q210,20 202,12", "gold", 3, False)   # horns
    s += dot(214, 42, 2.4, "gold", "dark") + dot(214.5, 42, 1, "dark")
    s += f'<path d="M222,55 l2,3 l2,-3 l2,3 l2,-3" fill="none" stroke="{WOOL["white"]}" stroke-width="1.3"/>'
    if fire:
        for c, c2, k in (("terra", "ochre", 1.0), ("ochre", "gold", 0.75), ("gold", "cream", 0.5)):
            for j, ang in enumerate((70, 80, 90, 100, 110)):
                L = (52, 64, 72, 62, 50)[j] * k
                d = f"M-6,0 Q-9,{-L * 0.5} 0,{-L} Q9,{-L * 0.5} 6,0 Z"
                s += g(fill(d, c, 90, "dark" if k == 1.0 else None, 1) + fill(f"M-2,0 Q-3,{-L * 0.4} 0,{-L * 0.65} Q3,{-L * 0.4} 2,0 Z", c2, 90, None),
                       f"translate(228 57) rotate({ang})")
    return s
B["great_dragon"] = dict(title="The Great Dragon", size=(300, 160), frames=3, build=dragon,
                         behaviour="castle boss; flies between battlements, breathes a gout of fire (frame 3); cut the bridge or stomp its head")

# ---------------- Cat King ----------------
def cat_king(f):
    up = f == 1
    s = ""
    for x in (24, 132):   # throne posts with beast-head finials, as in Edward's throne
        s += fill(f"M{x},64 L{x + 8},64 L{x + 8},170 L{x},170 Z", "brown", 90, "dark", 1.4)
        s += fill(f"M{x - 2},60 Q{x + 4},50 {x + 12},54 L{x + 14},60 Q{x + 6},66 {x - 2},60 Z", "gold", 0, "dark", 1.1)
        s += dot(x + 8, 56, 0.9, "dark")
    s += fill("M20,150 L144,150 L144,160 L20,160 Z", "ochre", 0, "dark", 1.4)
    for i, x in enumerate(range(26, 140, 16)):
        s += fill(f"M{x},152 L{x + 8},152 L{x + 8},158 L{x},158 Z", ("terra", "blue")[i % 2], 0, "dark", 0.8)
    cloak = "M44,80 Q26,124 34,164 L130,164 Q138,124 120,80 Q82,66 44,80 Z"
    s += fill(cloak, "terra", 90) + clip("ckc", cloak, stem("M30,160 L136,160", "gold", 6, False) + stem("M44,80 Q30,124 36,164 M120,80 Q134,124 128,164", "gold", 3, False))
    s += stem(cloak, "dark", 1.8)
    body = "M58,84 Q52,124 60,160 L104,160 Q112,124 106,84 Q82,74 58,84 Z"
    s += fill(body, "grey", 90) + clip("ckb2", body, stem("M54,100 Q62,102 66,96 M54,118 Q62,120 66,114 M98,100 Q104,102 110,96 M98,118 Q104,120 110,114", "dark", 2, False))
    s += fill("M72,96 Q82,90 92,96 Q94,130 82,148 Q70,130 72,96 Z", "cream", 90, None)
    s += fill("M58,158 L74,158 L74,166 L56,166 Z", "grey", 0, "dark", 1.2) + fill("M90,158 L106,158 L108,166 L90,166 Z", "grey", 0, "dark", 1.2)
    # ladle-sceptre
    lad = stem("M0,0 L0,-62", "gold", 3.4, False) + fill("M-8,-62 Q-8,-74 0,-74 Q8,-74 8,-62 Q0,-56 -8,-62 Z", "gold", 0, "dark", 1.1) + dot(0, -66, 2, "sage")
    s += g(lad, f"translate(118 {104 if up else 118}) rotate({38 if up else 8})")
    s += fill("M104,104 Q114,108 120,116 L116,122 Q108,116 100,112 Z", "grey", 0, "dark", 1.2) + dot(118.5, 118, 4.4, "grey", "dark")
    s += g(cat_head(), "translate(34 0) scale(2.3)")
    s += fill("M60,22 L62,4 L70,14 L82,0 L94,14 L102,4 L104,22 Q82,16 60,22 Z", "gold", 0, "dark", 1.4)
    s += dot(82, 10, 2, "terra") + dot(70, 17, 1.4, "blue") + dot(94, 17, 1.4, "blue")
    return s
B["cat_king"] = dict(title="The Cat King", size=(164, 170), frames=2, build=cat_king,
                     behaviour="final boss; enthroned in the Great Hall, flings soup with his ladle-sceptre and summons Soup Cats; rescue Princess Aelfgyva from behind his throne")

def build_svg(name, f):
    b = B[name]; w, h = b["size"]
    return doc(w, h, b["build"](f), seed=31 + f)
