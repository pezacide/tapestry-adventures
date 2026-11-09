"""Bestiary & Bayeux antagonists. Each creature: size + frame builders (faces right)."""
import math
from embroider import fill, mail, stem, dot, doc, g, rot, WOOL

def clip(cid, d, body):
    return f'<clipPath id="{cid}"><path d="{d}"/></clipPath><g clip-path="url(#{cid})">{body}</g>'

def scales(x0, y0, x1, y1, s, color, w=0.9):
    out, row = [], 0
    y = y0
    while y < y1:
        off = (s / 2) if row % 2 else 0
        x = x0 - off
        while x < x1:
            out.append(f"M{x:.1f},{y:.1f} q{s / 2:.1f},{s * 0.7:.1f} {s:.1f},0")
            x += s
        y += s * 0.55; row += 1
    return stem(" ".join(out), color, w, False)

def dots(x0, y0, x1, y1, step, r, color):
    s, row, y = "", 0, y0
    while y < y1:
        x = x0 + (step / 2 if row % 2 else 0)
        while x < x1:
            s += dot(x, y, r, color, None); x += step
        y += step * 0.8; row += 1
    return s

def spiral(cx, cy, r0, r1, turns, start=0):
    pts, n = [], int(60 * turns)
    for i in range(n + 1):
        t = i / n; a = start + t * turns * 2 * math.pi; r = r0 + (r1 - r0) * t
        pts.append(f"{cx + r * math.cos(a):.1f},{cy + r * math.sin(a):.1f}")
    return "M" + " L".join(pts)

def limb(x, y, length, w, color, angle=0, foot=None, footc="dark"):
    """A straight stiff Bayeux leg hanging from (x,y), rotated about the hip."""
    d = f"M{x - w / 2},{y} L{x + w / 2},{y} L{x + w * 0.35},{y + length} L{x - w * 0.35},{y + length} Z"
    s = fill(d, color, 90, "dark", 1.1)
    if foot == "hoof":
        s += fill(f"M{x - w * 0.45},{y + length - 1} L{x + w * 0.6},{y + length - 1} L{x + w * 0.7},{y + length + 3} L{x - w * 0.5},{y + length + 3} Z", footc, 0, "dark", 0.9)
    elif foot == "paw":
        s += fill(f"M{x - w * 0.45},{y + length - 1} Q{x + w * 1.6},{y + length - 1} {x + w * 1.4},{y + length + 2.5} L{x - w * 0.45},{y + length + 2.5} Z", footc, 0, "dark", 0.9)
    elif foot == "claw":
        s += stem(f"M{x},{y + length} l3,2 M{x},{y + length} l-1,3 M{x},{y + length} l4,-0.5", "dark", 1, False)
    return g(s, rot(angle, x, y))

C = {}

# ---------------- Snail-Hound: dog's head on a coiled snail body ----------------
def snail_hound(f):
    shell_only = f == 2
    bob = (0, -1.2, 0)[f]
    s = fill("M6,60 Q30,57 58,60 L58,62.5 L6,62.5 Z", "buff", 0, "dark", 1) if not shell_only else ""
    if not shell_only:
        s += g(fill("M30,34 Q35,18 44,13 L52,19 Q44,26 42,44 Z", "rose", 100), f"translate(0 {bob})")
    shell = "M24,20 a20,20 0 1,0 0.1,0 Z"
    s += fill("M24,20 a20,20 0 1,0 0.1,0 Z".replace("M24,20", "M24,20"), "terra", 20)
    s += stem(spiral(24, 40, 2, 19, 2.6, math.pi), "dark", 1.4, False)
    s += stem(spiral(24, 40, 3.6, 20.6, 2.6, math.pi), "cream", 0.9, False).replace('stroke-width="0.90"', 'stroke-width="0.9"').replace("stroke-dasharray", "data-x") \
        if False else f'<path d="{spiral(24, 40, 4.5, 21.5, 2.45, math.pi)}" fill="none" stroke="{WOOL["cream"]}" stroke-width="1.1" stroke-dasharray="0.1 2.6" stroke-linecap="round"/>'
    s += stem(shell, "dark", 1.6)
    if shell_only:
        s += dot(40, 48, 1.2, "dark") + stem("M37,45.5 L42,46.5", "dark", 0.9, False)
        return s
    head = fill("M42,10 Q45,4.5 51,5.5 L59.5,9 Q63.5,11 62.5,14.5 L57,17.5 Q50,20 44.5,18 Q40,15 42,10 Z", "sky", 0)
    head += fill("M45.5,7.5 L45,0.5 L50.5,5.5 Z", "sky", 90, "dark", 1)
    head += dot(52.5, 10, 1.1, "dark")
    head += stem("M53,14.8 L62,13.6", "dark", 1, False)
    head += f'<path d="M54,14.6 l1,1.6 l1,-1.7 l1,1.6 l1,-1.7 l1,1.6 l1,-1.7" fill="none" stroke="{WOOL["white"]}" stroke-width="0.9"/>'
    return s + g(head, f"translate(0 {bob})")
C["snail_hound"] = dict(title="Snail-Hound", size=(64, 64), frames=3, build=snail_hound,
                        behaviour="walker; stomp it and it hides in its shell - kick the shell to bowl over other beasts")

# ---------------- Owl with a man's face ----------------
OWL = "M22,22 Q36,13 50,22 Q58,44 47,62 Q36,67 25,62 Q14,44 22,22 Z"
def owl(f):
    s = ""
    if f == 1:   # wings spread
        for sx in (1, -1):
            d = "M44,30 Q58,14 70,10 Q66,22 69,26 Q62,30 64,36 Q56,38 50,44 Z"
            wing = fill(d, "olive", 70) + stem("M48,34 L66,14 M50,38 L64,26 M50,42 L60,36", "dark", 0.8, False)
            s += wing if sx == 1 else g(wing, "translate(72 0) scale(-1 1)")
    s += fill(OWL, "sage", 90)
    s += clip("owlc", OWL, scales(14, 34, 58, 64, 5, "green", 0.8))
    if f == 0:
        s += fill("M47,30 Q55,44 48,60 Q44,48 42,34 Z", "olive", 80, "dark", 1.2)
        s += stem("M46,38 L50,52 M45,44 L48,56", "dark", 0.7, False)
    s += fill("M28,14 L25,5 L33,12 Z", "olive", 90, "dark", 1) + fill("M44,14 L47,5 L39,12 Z", "olive", 90, "dark", 1)
    face = "M36,11 Q47,11 47,23 Q46.5,33 36,35 Q25.5,33 25,23 Q25,11 36,11 Z"
    s += fill(face, "buff", 90)
    s += stem("M29,20 Q31.5,18.5 34,20 M38,20 Q40.5,18.5 43,20", "dark", 1, False)
    s += dot(31.5, 22, 1.1, "dark") + dot(40.5, 22, 1.1, "dark")
    s += stem("M36,21 L35,27 L37,27", "brown", 0.9, False) + stem("M32.5,30 Q36,31.5 39.5,30", "terra", 1, False)
    s += stem("M29,26 Q30,32 36,34.6 Q42,32 43,26", "brown", 0.8, False)
    s += stem("M30,64 l-3,3 M30,64 l0,4 M30,64 l3,3 M42,64 l-3,3 M42,64 l0,4 M42,64 l3,3", "ochre", 1.4, False)
    if f == 0:
        s += fill("M10,68 Q36,64 66,68 L66,70.5 Q36,67 10,70.5 Z", "brown", 0, "dark", 1)
    return s
C["owl_face"] = dict(title="Man-Faced Owl", size=(72, 72), frames=2, build=owl,
                     behaviour="perches in trees, swoops down at a knight when one passes beneath")

# ---------------- Soup Cat ----------------
def soup_cat(f):
    s = ""
    fl = (0, 1, 0)[f]
    # fire
    for x, h, c in ((44, 9 + fl, "terra"), (51, 12 - fl, "gold"), (58, 9 + fl, "terra"), (51, 7, "ochre")):
        s += fill(f"M{x - 4},80 Q{x - 3},{80 - h * 0.6} {x},{80 - h} Q{x + 3},{80 - h * 0.6} {x + 4},80 Z", c, 90, "dark", 0.9)
    s += stem("M38,79.5 L66,79.5", "brown", 2, False)
    # tail
    s += stem("M12,66 Q2,62 4,50 Q6,42 2,36", "grey", 3.4, False) + stem("M12,66 Q2,62 4,50 Q6,42 2,36", "dark", 0.6, False)
    # body
    body = "M13,30 Q7,50 11,72 L31,72 Q35,50 31,30 Z"
    s += fill(body, "grey", 90)
    s += clip("catb", body, stem("M8,40 Q14,42 16,38 M8,50 Q14,52 17,48 M8,60 Q14,62 17,58 M26,44 Q30,46 34,43 M26,56 Q30,58 34,55", "dark", 1.2, False))
    s += fill("M20,40 Q27,50 25,68 Q20,70 18,60 Q16,50 20,40 Z", "cream", 90, None)
    s += fill("M10,70 L20,70 L20,75 L8,75 Z", "grey", 0, "dark", 1) + fill("M22,70 L32,70 L33,75 L21,75 Z", "grey", 0, "dark", 1)
    # cauldron
    pot = "M37,52 L69,52 Q70,72 53,76 Q36,72 37,52 Z"
    s += fill(pot, "dark", 0, "dark", 1.4)
    s += stem("M40,58 Q53,60 66,58", "grey", 0.8, False)
    s += fill("M35,50 Q53,46 71,50 L71,53 Q53,49.5 35,53 Z", "grey", 0, "dark", 1.2)
    s += fill("M38,50.5 Q53,47.5 68,50.5 Q53,52.5 38,50.5 Z", "sage", 0, None)
    for i, x in enumerate((46, 54, 61)):   # steam
        s += stem(f"M{x},46 q-2,-4 0,-7 q2,-3 0,-7", "white", 0.9, False) if (i + f) % 2 == 0 else ""
    # arm + spoon
    if f < 2:
        sp = (-8, 6)[f]
        s += g(stem("M44,26 L52,52", "brown", 2.4, False) + fill("M50,51 Q55,49 56,54 Q53,57 50,55 Z", "brown", 0, "dark", 0.9), rot(sp, 46, 40))
        s += mail("M27,34 Q34,36 42,39 L41,43.5 Q33,42 26,40 Z", "grey", "dark", 1.1) + dot(43.5, 40.5, 2.6, "grey", "dark")
    else:   # flinging soup
        s += g(stem("M30,32 L44,6", "brown", 2.4, False) + fill("M42,3 Q48,1 48,7 Q45,10 42,8 Z", "brown", 0, "dark", 0.9), "")
        s += mail("M27,34 Q30,28 31,24 L35,26 Q34,31 31,37 Z", "grey", "dark", 1.1) + dot(32.5, 25, 2.6, "grey", "dark")
        s += dot(52, 6, 2.4, "sage", "dark") + dot(58, 10, 1.6, "sage", "dark")
    # head
    head = "M10,20 Q10,8 21,8 Q32,8 32,20 Q32,30 21,31 Q10,30 10,20 Z"
    s += fill("M11,13 L12,2 L18,9 Z", "grey", 90, "dark", 1) + fill("M31,13 L30,2 L24,9 Z", "grey", 90, "dark", 1)
    s += fill(head, "grey", 0)
    s += stem("M17,9 L18,13 M21,8.5 L21,12.5 M25,9 L24,13", "dark", 0.9, False)
    s += stem("M13.5,16 L19,18.5 M28.5,16 L23,18.5", "dark", 1.3, False)
    s += fill("M15,19.5 Q17,18 19,20 Q17,21.5 15,19.5 Z", "gold", 0, "dark", 0.7) + fill("M23,20 Q25,18 27,19.5 Q25,21.5 23,20 Z", "gold", 0, "dark", 0.7)
    s += fill("M19.5,23 L22.5,23 L21,24.6 Z", "rose", 0, "dark", 0.6)
    s += stem("M17.5,27.5 Q21,25.5 24.5,27.5", "dark", 0.9, False)
    s += stem("M9,24 L16,24.5 M9,26.5 L16,26 M33,24 L26,24.5 M33,26.5 L26,26", "dark", 0.5, False)
    return s
C["soup_cat"] = dict(title="The Soup Cat", size=(72, 82), frames=3, build=soup_cat,
                     behaviour="stands guard stirring her cauldron; flings arcs of scalding soup ('That's it, you're going in the soup')")

# ---------------- Spotted Unicorn ----------------
def unicorn(f):
    fl, bl = ((-22, 22), (18, -16))[f]
    s = ""
    s += limb(22, 40, 18, 4.5, "blue", bl, "hoof") + limb(52, 38, 18, 4.5, "blue", fl, "hoof")
    s += stem("M14,30 Q6,30 6,22 Q6,16 2,14", "blue", 2.2, False) + fill("M0,12 Q4,8 6,14 Q2,20 0,12 Z", "cream", 90, "dark", 0.8)
    body = "M14,26 Q30,17 52,22 L58,38 Q44,46 22,45 Q9,41 14,26 Z"
    s += fill(body, "blue", 160)
    s += clip("unib", body, dots(12, 22, 60, 46, 5, 1.3, "indigo"))
    s += limb(27, 40, 18, 4.5, "blue", -bl * 0.6, "hoof") + limb(48, 38, 18, 4.5, "blue", -fl * 0.6, "hoof")
    neck = "M47,23 Q53,12 61,9.5 Q69,8.5 74.5,14 Q77,19 73.5,21 L66,21.5 Q62,25 60,38 L53,39.5 Z"
    s += fill(neck, "cream", 120)
    s += fill("M71,11.5 L80,0.5 L73.5,13.5 Z", "gold", 60, "dark", 0.9) + stem("M73,10 L74.6,11 M75.3,7.2 L76.6,8.2 M77.4,4.6 L78.4,5.4", "brown", 0.6, False)
    s += fill("M61.5,10.5 L59.5,3 L65.5,8.5 Z", "cream", 90, "dark", 0.9)
    s += dot(68.5, 14, 1, "dark") + stem("M72.5,19.2 L75.5,18.6", "dark", 0.7, False)
    s += stem("M55,14 Q50,18 49,26 M57,12 Q52,20 51,30", "blue", 1.6, False)   # mane
    return s
C["unicorn"] = dict(title="Spotted Unicorn", size=(80, 64), frames=2, build=unicorn,
                    behaviour="charger; lowers its horn and gallops when it sees a knight, can't turn quickly")

# ---------------- Wyvern ----------------
def wyvern(f):
    s = ""
    up = f == 0
    wing = ("M42,27 Q36,6 24,0.5 Q25,8 21,11 Q13,8 5,10 Q12,16 13,22 Q22,20 30,29 Z" if up else
            "M42,31 Q32,46 18,58 Q20,50 13,47 Q20,44 19,38 Q28,40 32,34 Z")
    ribs = ("M42,27 L24,1.5 M42,27 L6,10.5 M42,27 L14,21.5" if up else "M42,31 L18,57 M42,31 L14,47 M42,31 L20,38")
    s += fill(wing, "terra", 40) + stem(ribs, "dark", 0.9, False)
    s += stem("M28,38 Q14,44 9,34 Q5,24 15,22 Q22,23 19,30 Q17,33 14,31", "green", 3.6, False)
    s += stem("M28,38 Q14,44 9,34 Q5,24 15,22 Q22,23 19,30 Q17,33 14,31", "dark", 0.7, False)
    s += fill("M14,31 Q10,28 12,34 Q16,36 14,31 Z", "gold", 0, "dark", 0.8)
    s += limb(34, 40, 12, 4, "green", 10, "claw") + limb(46, 40, 12, 4, "green", -10 if up else 18, "claw")
    body = "M26,31 Q40,22 54,26 Q60,34 52,42 Q38,46 28,41 Z"
    s += fill(body, "green", 150) + clip("wyb", body, scales(24, 26, 60, 46, 4, "olive", 0.7))
    s += fill("M30,40 Q42,44 52,41 Q42,42 30,40 Z", "gold", 0, None)
    neck = "M50,27 Q57,14 64,12 L73,13.5 Q77.5,15.5 75,19.5 L67,21 Q61,23 57,35 Z"
    s += fill(neck, "green", 120)
    s += fill("M66,20.5 L75,19.5 Q76,23.5 70,24 Q66,23 66,20.5 Z", "green", 0, "dark", 1)
    s += stem("M75,21.5 L80,22 M80,22 l1.5,-1.5 M80,22 l1.5,1.4", "terra", 1, False)
    s += fill("M63,12.5 L60,5 L67,11 Z", "green", 90, "dark", 0.9) + dot(68, 15.5, 1.1, "gold", "dark")
    return s
C["wyvern"] = dict(title="Wyvern", size=(80, 60), frames=2, build=wyvern,
                   behaviour="flyer; loops in a figure-eight above pits; stompable")

# ---------------- Killer Hare (marginalia) ----------------
def hare(f):
    fl, bl = ((-20, 18), (16, -18))[f]
    s = limb(22, 42, 16, 5, "buff", bl, "paw", "buff")
    s += fill("M22,10 Q20,1 23,0 Q26,1 25.5,11 Z", "buff", 90, "dark", 1) + fill("M27,11 Q28,1 31.5,1 Q33,4 29.5,12 Z", "buff", 90, "dark", 1)
    body = "M17,22 Q26,18 32,24 Q36,36 31,46 Q22,49 15,45 Q12,32 17,22 Z"
    s += fill(body, "buff", 100) + fill("M27,26 Q34,34 30,44 Q26,36 26,28 Z", "cream", 90, None)
    s += stem("M14,42 Q9,42 10,37", "cream", 3, False)
    s += limb(27, 42, 16, 5, "buff", fl, "paw", "buff")
    s += fill("M20,12 Q30,8 35,15 Q37,19 33,21 Q26,24 21,20 Q18,16 20,12 Z", "buff", 0)
    s += dot(29.5, 14, 1.1, "dark") + stem("M35.5,18.6 L33,18.8", "rose", 1, False) + stem("M33,19.5 L39,18 M33,20.5 L39,21.5", "dark", 0.45, False)
    # sword raised
    sw = (-30, -10)[f]
    s += g(fill("M33,30 L50,30 L50,32.6 L33,32.6 Z", "sky", 0, "dark", 0.9) + fill("M50,30 L53,31.3 L50,32.6 Z", "sky", 0, "dark", 0.7)
           + fill("M32,27.5 L34,27.5 L34,35 L32,35 Z", "gold", 0, "dark", 0.7), rot(sw, 31, 31))
    s += dot(30.5, 31.3, 2.4, "buff", "dark")
    return s
C["killer_hare"] = dict(title="Killer Hare", size=(56, 64), frames=2, build=hare,
                        behaviour="hopper; bounds toward the knight brandishing a sword - stomp before it lands")

# ---------------- Bayeux Hound ----------------
def hound(f):
    ext = f == 0
    s = ""
    s += stem("M16,20 Q6,14 10,4" if ext else "M16,20 Q4,22 4,10", "ochre", 2.2, False)
    s += limb(19, 24, 18, 3.4, "terra", 40 if ext else -15, "paw", "terra") + limb(52, 22, 18, 3.4, "terra", -45 if ext else 25, "paw", "terra")
    body = "M14,20 Q34,13 56,17 L60,27 Q46,30 34,27 Q22,32 15,28 Q11,24 14,20 Z"
    s += fill(body, "ochre", 170)
    s += limb(23, 24, 18, 3.4, "ochre", 55 if ext else -30, "paw") + limb(56, 23, 18, 3.4, "ochre", -60 if ext else 35, "paw")
    head = "M53,16 Q58,9 64,9.5 L71,14.5 Q72.5,17.5 69.5,18.5 L61,20 Q56,21 53,16 Z"
    s += fill(head, "ochre", 0) + fill("M58,10.5 L54,5 L61,9.5 Z", "terra", 0, "dark", 0.9)
    s += dot(64, 12.5, 0.9, "dark") + stem("M66,17.4 L71,16.6", "dark", 0.8, False)
    s += stem("M55.5,13 Q56,19 58,21", "terra", 2.2, False) + dot(58, 22.5, 1.2, "gold", "dark")
    return s
C["bayeux_hound"] = dict(title="Bayeux Hound", size=(76, 48), frames=2, build=hound,
                         behaviour="fast runner that comes in packs of three; leaps small gaps")

# ---------------- Boss: Aspidochelone, the island-whale ----------------
def whale(f):
    s = ""
    body = "M8,132 Q28,64 110,52 Q180,42 214,70 Q236,90 228,112 Q206,128 168,130 Z"
    s += fill("M96,60 L104,34 L112,52 L122,30 L128,50 L138,34 L140,54 Z", "olive", 90, "dark", 1.4)
    s += fill(body, "green", 160, "dark", 2.2)
    s += clip("whb", body, scales(6, 48, 236, 132, 9, "olive", 1.2) + fill("M8,132 Q70,112 168,118 Q206,116 228,112 L228,140 L8,140 Z", "sage", 0, "dark", 1.2))
    s += fill("M160,84 Q170,94 160,108 Q150,96 160,84 Z", "olive", 90, "dark", 1.2)   # side fin
    s += stem("M180,64 Q172,90 182,118", "dark", 1.4, False)   # gill line
    s += fill("M190,70 a9,9 0 1,0 18,0 a9,9 0 1,0 -18,0 Z", "white", 0, "dark", 1.4) + dot(201, 70, 4.2, "dark")
    if f == 0:
        s += stem("M231,101 Q214,106 192,104", "dark", 1.8, False)
    else:
        s += fill("M231,96 Q212,100 192,103 Q212,124 230,122 Z", "terra", 0, "dark", 1.8)
        s += f'<path d="M196,104 l3,4 l3,-4.4 l3,4.6 l3,-4.8 l3,5 l3,-5 l3,5 l3,-5.2 l3,5" fill="none" stroke="{WOOL["white"]}" stroke-width="1.6"/>'
        s += f'<path d="M200,118 l3,-4 l3,4.3 l3,-4.5 l3,4.6 l3,-4.8 l3,4.8 l3,-4.8" fill="none" stroke="{WOOL["white"]}" stroke-width="1.6"/>'
    # tail rising from the waves at left
    s += fill("M6,130 Q-2,108 8,92 Q12,104 22,112 Q30,96 44,94 Q34,112 30,130 Z", "green", 120, "dark", 1.8)
    # sea
    for i, y in enumerate((124, 133, 142, 151)):
        c = ("blue", "sky", "blue", "indigo")[i]
        d = f"M0,{y} " + " ".join(f"q10,{-6 if (j + i + f) % 2 else -4} 20,0" for j in range(13)) + f" L260,170 L0,170 Z"
        s += fill(d, c, 0, "dark", 1.2)
    return s
C["whale_boss"] = dict(title="Aspidochelone, the Island-Whale", size=(256, 168), frames=2, build=whale,
                       behaviour="boss; surfaces from the sea, spouts water, snaps; knights must jump on its scaly back and strike the eye")

def build_svg(name, f):
    c = C[name]; w, h = c["size"]
    return doc(w, h, c["build"](f), seed=11 + f)
