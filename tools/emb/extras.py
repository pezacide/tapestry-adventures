"""Extras: the title banner, a pole bell, the Smiths (and their hammers), urns with biting plants, and wells."""
import math
from embroider import fill, stem, dot, woven, g, rot, WOOL
from creatures import C, limb, clip
import scenery

S = scenery.S
T = 48

# ---------------------------------------------------------------- the title banner
def logo():
    """A hanging banner: a gilded rod, a red field with a gold border and a swallow-tail, and the name in cream wool."""
    s = ""
    s += stem("M60,10 L60,26 M420,10 L420,26", "grey", 1.6, False)                                   # the cords
    s += woven("M20,12 L460,12 L460,24 L20,24 Z", "brown", "dark", 1.4)                              # the rod
    for x in (16, 464): s += dot(x, 18, 9, "gold", "dark")
    body = "M34,26 L446,26 L446,206 L240,236 L34,206 Z"
    s += woven(body, "terra", "dark", 2)
    s += stem("M44,34 L436,34 L436,198 L240,224 L44,198 Z", "gold", 3.2)                              # the gilt border
    s += stem("M52,42 L428,42 L428,192 L240,214 L52,192 Z", "gold", 1.2)
    for x, y in ((52, 42), (428, 42), (52, 192), (428, 192)): s += dot(x, y, 4, "gold", "dark")
    def word(txt, y, size, w):
        shadow = f'<text x="241.5" y="{y + 2.5}" font-family="Georgia, \'DejaVu Serif\', serif" font-weight="bold" font-size="{size}" text-anchor="middle" textLength="{w}" lengthAdjust="spacingAndGlyphs" fill="{WOOL["dark"]}">{txt}</text>'
        face = f'<text x="240" y="{y}" font-family="Georgia, \'DejaVu Serif\', serif" font-weight="bold" font-size="{size}" text-anchor="middle" textLength="{w}" lengthAdjust="spacingAndGlyphs" fill="{WOOL["cream"]}" stroke="{WOOL["gold"]}" stroke-width="0.8">{txt}</text>'
        return shadow + face
    s += word("YE", 78, 30, 54)
    s += word("TAPESTRIE", 138, 62, 372)
    s += word("ADVENTURES", 184, 40, 340)
    for x in (60, 120, 360, 420):   # tassels along the fringe
        s += stem(f"M{x},204 L{x},216", "gold", 1.4, False) + dot(x, 219, 3.6, "gold", "dark")
    return s
S["logo"] = (480, 240, logo, False)

# ---------------------------------------------------------------- a bell, rung at the pole
def bell():
    s = stem("M16,3 L16,8", "dark", 1.6, False) + dot(16, 3, 2.4, "gold", "dark")
    s += fill("M7,28 Q8,16 10,12 Q16,6 22,12 Q24,16 25,28 Q16,31 7,28 Z", "gold", 90)
    s += stem("M7,28 Q16,31 25,28", "dark", 1.4, False) + stem("M10,18 Q16,12 22,18", "cream", 1, False)
    s += dot(16, 31, 3, "ochre", "dark")
    return s
S["bell"] = (32, 36, bell, False)

# ---------------------------------------------------------------- the Smith (and his hammer)
def smith_body(f):
    """A village smith in a leather apron, with a great hammer: 0 standing, 1 hopping, 2 hurling a hammer overhead."""
    s = ""
    hop = f == 1
    legs = (-14, 14) if hop else (-3, 3)
    s += limb(24, 52, 24, 6, "brown", legs[0], "hoof") + limb(36, 52, 24, 6, "brown", legs[1], "hoof")
    tunic = "M16,24 Q30,18 44,24 L46,54 Q30,58 14,54 Z"
    s += fill(tunic, "grey", 90) + stem(tunic, "dark", 1.4)
    apron = "M20,30 L40,30 L42,58 Q30,62 18,58 Z"
    s += fill(apron, "brown", 160, "dark", 1.4) + stem("M20,30 L40,30", "buff", 1.6, False)
    s += fill("M20,6 Q30,-2 40,6 Q42,16 30,18 Q18,16 20,6 Z", "buff", 0)                          # head
    s += fill("M20,8 Q30,-4 40,8 L40,5 Q30,-6 20,5 Z", "dark", 0, "dark", 1)                      # a leather cap
    s += fill("M22,13 Q30,26 38,13 Q34,22 30,23 Q26,22 22,13 Z", "ochre", 0, "dark", 1)           # a thick beard
    s += dot(26, 10, 0.9, "dark") + dot(34, 10, 0.9, "dark")
    if f == 2:   # arm and hammer thrown up
        s += fill("M38,28 L50,6 L55,9 L44,34 Z", "grey", 60, "dark", 1.1) + dot(52, 6, 3.2, "buff", "dark")
        s += stem("M52,8 L52,-10", "brown", 3, False) + fill("M44,-16 L60,-16 L60,-6 L44,-6 Z", "grey", 60, "dark", 1.4)
    else:        # the hammer held low, head on the ground
        s += fill("M38,28 L50,42 L46,47 L35,34 Z", "grey", 60, "dark", 1.1) + dot(48, 44, 3.2, "buff", "dark")
        s += stem("M49,44 L58,70", "brown", 3, False) + fill("M52,66 L66,72 L63,80 L50,74 Z", "grey", 60, "dark", 1.4)
    s += fill("M16,26 L8,44 L14,48 L22,32 Z", "grey", 60, "dark", 1.1) + dot(11, 46, 3.2, "buff", "dark")   # the other arm
    return s
def smith(f): return g(smith_body(f), "translate(2 22)")
C["smith"] = dict(title="Smythe", size=(76, 108), frames=3, build=smith,
                  behaviour="walks a short beat, hops now and then, and hurls hammers in arcs at the knight; always in pairs; stompable")

def hammer(f):
    s = stem("M3,18 L21,6", "brown", 2.6, False) + fill("M16,0 L26,6 L22,14 L12,8 Z", "grey", 60, "dark", 1.2)
    return g(s, "translate(-1 2)")
C["hammer"] = dict(title="Smith's Hammer", size=(28, 24), frames=1, build=hammer, behaviour="hurled by a smith in a long arc")

# ---------------------------------------------------------------- the urn and its biting plant
def urn():
    """A clay urn, solid, with a flat rim to stand on."""
    s = woven("M4,0 L44,0 L44,8 L4,8 Z", "terra", "dark", 1.4)                                          # the rim
    body = "M8,8 L40,8 Q46,28 38,44 Q34,48 24,48 Q14,48 10,44 Q2,28 8,8 Z"
    s += woven(body, "ochre", "dark", 1.4)
    s += clip("urnc", body, woven("M0,18 L48,18 L48,26 L0,26 Z", "blue"))
    for x in (12, 20, 28, 36): s += dot(x, 22, 1.4, "gold")
    s += stem("M12,36 Q24,40 36,36", "brown", 1.4, False)
    return s
S["urn"] = (T, T, urn, False)

def urn_plant(f):
    """A biting plant that rises from an urn: 0 a closed bud, 1 jaws wide open (the danger)."""
    s = stem("M22,52 L22,30", "green", 6, False) + stem("M22,52 L22,30", "dark", 0.8, False)
    s += fill("M22,44 Q8,44 4,34 Q16,32 22,40 Z", "sage", 70, "dark", 1.1) + fill("M22,44 Q36,44 40,34 Q28,32 22,40 Z", "sage", 70, "dark", 1.1)
    if f == 0:
        s += fill("M10,30 Q22,2 34,30 Q22,38 10,30 Z", "terra", 40, "dark", 1.4) + stem("M16,24 Q22,10 28,24", "rose", 1.2, False)
    else:
        s += fill("M6,22 Q10,4 22,10 Q20,20 22,26 Q12,32 6,22 Z", "terra", 40, "dark", 1.4)
        s += fill("M38,22 Q34,4 22,10 Q24,20 22,26 Q32,32 38,22 Z", "terra", 40, "dark", 1.4)
        for x, y in ((12, 22), (17, 24), (27, 24), (32, 22)):
            s += fill(f"M{x},{y} l2,-7 l2,7 Z", "white", 0, "dark", 0.6)
        s += dot(15, 12, 1.6, "gold") + dot(29, 12, 1.6, "gold")
    return s
C["urn_plant"] = dict(title="Urn-Plante", size=(44, 54), frames=2, build=urn_plant,
                      behaviour="rises out of its urn and snaps, then sinks again; stays down while the knight is near or upon the urn; stomping it hurts; bolts fell it")

# ---------------------------------------------------------------- the well
def well():
    """A stone well-head: stand upon it and press DOWN to climb in."""
    s = woven("M2,8 L46,8 L46,48 L2,48 Z", "grey", "dark", 1.6)
    for y in (18, 28, 38): s += stem(f"M2,{y} L46,{y}", "dark", 0.7, False)
    for x, y in ((12, 13), (28, 13), (20, 23), (36, 23), (12, 33), (28, 33), (20, 43), (36, 43)):
        s += stem(f"M{x},{y} l0,5", "dark", 0.6, False)
    s += woven("M0,0 L48,0 L48,10 L0,10 Z", "cream", "dark", 1.4)                                       # the rim
    s += woven("M10,2 L38,2 L38,8 L10,8 Z", "dark")                                                     # the dark mouth
    s += stem("M24,10 L24,22", "brown", 1.6, False) + woven("M19,22 L29,22 L28,30 L20,30 Z", "brown", "dark", 1)   # a bucket on its rope
    return s
S["well"] = (T, T, well, False)

# ---------------------------------------------------------------- the leaping ember
def ember(f):
    """A spark leaping from a fire pit: 0 rising (flame trailing below), 1 falling (flame streaming above)."""
    up = f == 0
    s = fill("M18,40 Q4,28 12,14 Q14,24 20,22 Q18,10 24,2 Q34,14 32,24 Q40,20 36,32 Q34,42 18,40 Z" if up else
             "M18,4 Q4,16 12,30 Q14,20 20,22 Q18,34 24,42 Q34,30 32,20 Q40,24 36,12 Q34,2 18,4 Z", "terra", 40, "dark", 1.4)
    s += fill("M20,36 Q12,28 18,20 Q22,26 26,22 Q30,30 26,36 Q22,38 20,36 Z" if up else
              "M20,8 Q12,16 18,24 Q22,18 26,22 Q30,14 26,8 Q22,6 20,8 Z", "gold", 0)
    s += dot(21, 29 if up else 15, 2.2, "cream") + dot(27, 29 if up else 15, 1.6, "cream")
    return s
C["ember"] = dict(title="Leaping Ember", size=(44, 46), frames=2, build=ember,
                  behaviour="leaps out of castle fire pits at intervals and falls back in; cannot be stomped or shot, only avoided")
