"""Power-ups hidden in bricks: Mario's items with a medieval twist."""
from embroider import fill, stem, dot, doc, g, WOOL

I = {}

def chalice(f):   # 1-up: the golden chalice
    s = fill("M7,4 L25,4 Q25,15 16,17 Q7,15 7,4 Z", "gold", 90)
    s += stem("M8.5,6.5 L23.5,6.5", "ochre", 1.2, False)
    s += dot(12, 10, 1.4, "terra", "dark") + dot(16, 11.5, 1.4, "blue", "dark") + dot(20, 10, 1.4, "terra", "dark")
    s += fill("M14.5,17 L17.5,17 L17.5,24 L14.5,24 Z", "gold", 90, "dark", 1) + dot(16, 20.5, 1.6, "gold", "dark")
    s += fill("M9,24 L23,24 Q24,29 16,29.5 Q8,29 9,24 Z", "gold", 0, "dark", 1.2)
    if f: s += stem("M5,2 l2,2 M27,2 l-2,2 M16,0.5 l0,2", "cream", 1, False)
    return s
I["chalice"] = dict(title="Golden Chalice", size=(32, 32), frames=2, build=chalice, effect="one more lyfe (the 1-up)")

def capon(f):   # super mushroom: a roast capon leg - mends a heart
    s = fill("M5,13 Q4,4 13,4 Q22,5 22,13 Q22,21 13,22 Q5,21 5,13 Z", "brown", 30)
    s += stem("M8,10 Q11,7 15,8", "ochre", 1.4, False) + stem("M9,15 Q13,13 17,15", "ochre", 1, False)
    s += fill("M19,18 L25,24 L23.5,25.5 L17.5,19.5 Z", "cream", 45, "dark", 1)
    s += dot(25.5, 23.5, 2, "cream", "dark") + dot(23.5, 26.5, 2, "cream", "dark")
    return s
I["capon"] = dict(title="Roast Capon", size=(32, 32), frames=1, build=capon, effect="mends one herte (the mushroom)")

def crossbow(f):   # fire flower: the crossbow - shoot bolts with RUN
    s = stem("M16,3 Q24,14 16,25", "brown", 2.6, False)              # the bow (prod)
    s += stem("M16,3 L13,14 L16,25", "cream", 0.8, False)           # string drawn back
    s += fill("M2,12.5 L28,12.5 L28,15.5 L2,15.5 Z", "ochre", 0, "dark", 1.1)   # stock
    s += fill("M4,15 L9,15 L8,21 L5,21 Z", "brown", 90, "dark", 1)              # grip
    s += stem("M13,12.5 L31,12.5", "grey", 1.3, False) + fill("M30,10.5 L34,12.5 L30,14.5 Z", "grey", 0, "dark", 0.7)
    return s
I["crossbow"] = dict(title="Crossbowe", size=(36, 28), frames=1, build=crossbow, effect="shoote bolts with RUN (the fire flower)")

def relic(f):   # star: the holy relic - a jewelled reliquary; nothing can harm you for a while
    rays = "".join(stem(f"M16,15 L{16 + 15 * c:.1f},{15 + 15 * s_:.1f}", "gold", 1, False)
                   for c, s_ in ((1, 0), (-1, 0), (0, -1), (0.7, -0.7), (-0.7, -0.7), (0.7, 0.7), (-0.7, 0.7))) if f else ""
    s = rays
    s += fill("M6,14 L16,5 L26,14 L26,27 L6,27 Z", "gold", 90)
    s += fill("M14.5,8.5 L17.5,8.5 L17.5,11.5 L20.5,11.5 L20.5,14 L17.5,14 L17.5,19 L14.5,19 L14.5,14 L11.5,14 L11.5,11.5 L14.5,11.5 Z", "terra", 0, "dark", 0.8)
    s += dot(10, 22, 1.5, "blue", "dark") + dot(16, 23, 1.5, "terra", "dark") + dot(22, 22, 1.5, "blue", "dark")
    s += stem("M6,27 L26,27", "dark", 1.4, False)
    return s
I["relic"] = dict(title="Holy Relique", size=(32, 32), frames=2, build=relic, effect="no beaste may harme thee for a whyle (the star)")

def bolt(f):
    s = stem("M3,4 L20,4", "brown", 1.6, False) + fill("M19,1.8 L24,4 L19,6.2 Z", "grey", 0, "dark", 0.8)
    s += fill("M1,1 L6,3.6 L1,4 Z", "terra", 0, None) + fill("M1,7 L6,4.4 L1,4 Z", "terra", 0, None)
    return s
BOLT = dict(title="Crossbowe Bolt", size=(24, 8), frames=1, build=bolt)

def build_svg(spec, f):
    w, h = spec["size"]
    return doc(w, h, spec["build"](f), seed=41 + f)

def gaze(f):
    s = stem("M2,6 Q8,1 14,6 T26,6 T38,6" if f == 0 else "M2,6 Q8,11 14,6 T26,6 T38,6", "gold", 2.4, False)
    s += stem("M2,6 L38,6", "cream", 0.8, False) + dot(36, 6, 3, "gold", "dark")
    return s
GAZE = dict(title="Petrifying Gaze", size=(40, 12), frames=2, build=gaze)

def horn(f):   # World 5: the unicorn's horn-touch, carried to the poisoned fountain to make its waters pure
    s = ""
    if f:
        s += stem("M18,2 L18,8 M18,30 L18,34 M3,18 L8,18 M28,18 L33,18 M7,7 l3,3 M29,7 l-3,3", "gold", 1, False)
    s += fill("M10,30 L26,6 L16,32 Z", "cream", 60, "dark", 1.2)
    s += stem("M12,27 L16,27.5 M14,23 L18.5,23.5 M16.5,19 L21,19.5 M19,15 L23,15.5 M21.5,11 L24.5,11.2", "gold", 1, False)
    return s
I["horn"] = dict(title="Ye Unicorne's Horne", size=(36, 36), frames=2, build=horn, effect="carry it to ye fountaine and ye poysoned waters runne pure")

def spear(f):
    s = stem("M2,5 L40,5", "brown", 1.8, False) + fill("M39,2 L48,5 L39,8 Z", "grey", 0, "dark", 0.8)
    s += stem("M4,3 L8,5 L4,7", "terra", 1, False)
    return s
SPEAR = dict(title="Huntsman's Speare", size=(48, 10), frames=1, build=spear)
