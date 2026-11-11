"""Bayard the magic horse (rideable mount) and Princess Aelfgyva."""
from embroider import fill, mail, stem, dot, doc, g, rot, WOOL
from creatures import limb, clip

# ---------------- Bayard ----------------
HW, HH = 104, 84
SEAT = (48, 22)          # where a rider's hip sits, in horse design units
RIDER_HIP = (35, 63)     # hip in the knight frame
POSES = ["idle", "walk1", "walk2", "leap", "rear"]

def horse(pose, body_c="ochre", far_c="blue", mane="brown", rider="", saddle=True, deco=""):
    L = dict(idle=(0, 0, 0, 0), walk1=(-20, 16, 16, -20), walk2=(16, -20, -20, 16),
             leap=(-55, -40, 50, 35), rear=(-40, -18, 12, -4))[pose]   # near-front, far-front, near-back, far-back
    s = ""
    s += limb(72, 44, 30, 5.5, far_c, L[1], "hoof") + limb(30, 44, 30, 6, far_c, L[3], "hoof")
    s += stem("M17,30 Q5,34 6,50 Q7,60 3,70", mane, 4.5, False) + stem("M17,30 Q7,36 9,52 Q10,60 7,68", "dark", 0.7, False)
    body = "M16,30 Q40,22 70,25 Q81,30 79,42 Q62,52 30,50 Q14,46 16,30 Z"
    s += fill(body, body_c, 170)
    s += fill("M64,28 Q68,14 78,6 Q86,2 92,8 L99,18 Q100,22 96,23.5 L89,22.5 Q83,24 79,42 Z", body_c, 120)
    s += fill("M80,7 L79,0.5 L85,5 Z", body_c, 90, "dark", 0.9)
    s += stem("M66,27 Q70,14 79,7", mane, 3.6, False) + stem("M67,24 l-3,-1 M69,19 l-3,-1.5 M72,14 l-3,-2 M75,10 l-2.5,-2", mane, 1.2, False)
    s += dot(87.5, 10, 1, "dark") + dot(96.5, 19, 0.7, "dark")
    s += stem("M84,6 L91,21 M81,14 L97,16.5", "terra", 1.1, False) + stem("M95,21 Q76,30 56,22", "terra", 0.9, False)
    # saddle cloth, saddle, stirrup
    if saddle: s += fill("M38,25 L59,24 L61,40 L37,41 Z", "terra", 90, "dark", 1.1) + stem("M39,38 L60,37", "gold", 1, False)
    if saddle:
        s += fill("M38,19 Q37,26 42,26 L56,26 Q61,26 60,17 L56,22 L42,22 Z", "brown", 0, "dark", 1.1)
        s += stem("M48,26 L48,46", "dark", 0.8, False) + stem("M45.5,46 L50.5,46", "grey", 1.6, False)
    s += limb(68, 44, 30, 6, body_c, L[0], "hoof") + limb(26, 44, 30, 6.5, body_c, L[2], "hoof")
    s += deco
    if rider:
        s += g(rider, f"translate({SEAT[0] - RIDER_HIP[0]} {SEAT[1] - RIDER_HIP[1]})")
    if pose == "rear":
        return g(s, rot(-22, 30, 70))
    if pose == "leap":
        return g(s, "translate(0 -6)")
    return s

# ---------------- Princess Aelfgyva ----------------
PW, PH = 52, 100
P_POSES = ["idle", "wave", "joy"]
def princess(pose):
    s = ""
    gown = "M15,40 Q26,34 37,40 L45,96 Q26,99 7,96 Z"
    s += fill(gown, "terra", 90)
    s += clip("pg", gown, fill("M0,88 L52,88 L52,100 L0,100 Z", "ochre", 0, "dark", 1) + stem("M26,40 L26,96", "rose", 1, False))
    s += stem(gown, "dark", 1.6)
    s += fill("M15,58 L37,58 L37.6,61.5 L14.4,61.5 Z", "gold", 0, "dark", 0.9)
    def sleeve(side, up):
        if side > 0:   # her left (viewer's right)
            d = ("M35,42 L46,22 L51,26 L39,50 Z" if up else "M35,42 L45,66 L37,70 L33,50 Z")
            hand = (48.5, 22) if up else (40, 70)
        else:
            d = ("M17,42 L6,22 L1,26 L13,50 Z" if up else "M17,42 L7,66 L15,70 L19,50 Z")
            hand = (3.5, 22) if up else (12, 70)
        return fill(d, "rose", 70 if up else 100, "dark", 1.2) + dot(hand[0], hand[1] - (1 if up else 0), 2.3, "buff", "dark")
    s += sleeve(-1, pose == "joy") + sleeve(1, pose in ("wave", "joy"))
    veil = "M14,32 Q13,12 26,9 Q39,12 38,32 L40,46 Q26,49 12,46 Z"
    s += fill(veil, "cream", 90)
    s += stem("M16,40 Q26,44 36,40", "blue", 1, False)
    s += fill("M19,22 Q19,14 26,14 Q33,14 33,22 Q33,30 26,31 Q19,30 19,22 Z", "buff", 0)
    s += dot(23, 21.5, 0.9, "dark") + dot(29, 21.5, 0.9, "dark")
    s += stem("M23.5,26.5 Q26,28 28.5,26.5" if pose != "idle" else "M24,27 L28,27", "terra", 0.9, False)
    s += fill("M17,11 L19,3.5 L22.5,8.5 L26,2 L29.5,8.5 L33,3.5 L35,11 Q26,8.5 17,11 Z", "gold", 0, "dark", 1)
    s += dot(26, 6.5, 1, "terra")
    return s

MOUNT_PAD = 44   # extra headroom above the horse in mounted frames
def mounted(knight_body, steed=None):
    """Horse + rider in one frame (HW x HH+MOUNT_PAD), for each horse pose. steed(pose, rider) draws another mount (the unicorn)."""
    steed = steed or (lambda p, rider: horse(p, rider=rider))
    return {p: doc(HW, HH + MOUNT_PAD, g(steed(p, knight_body), f"translate(0 {MOUNT_PAD})")) for p in POSES}

# ---------------- the rescued monk (end of each castle) ----------------
MW, MH = 44, 76
M_POSES = ["idle", "wave"]
def monk(pose):
    s = fill("M10,28 Q22,22 34,28 L40,74 L4,74 Z", "brown", 90)
    s += stem("M12,46 L32,46", "cream", 1.6, False)   # rope girdle
    if pose == "wave":
        s += fill("M30,32 L42,12 L46,16 L35,38 Z", "brown", 70, "dark", 1.1) + dot(43.5, 11, 2.4, "buff", "dark")
    else:
        s += fill("M30,32 L36,50 L30,52 L27,38 Z", "brown", 100, "dark", 1.1) + dot(32.5, 52, 2.2, "buff", "dark")
    s += fill("M12,18 Q12,6 22,6 Q32,6 32,18 Q32,28 22,29 Q12,28 12,18 Z", "buff", 0)
    s += fill("M12,14 Q14,4 22,4 Q30,4 32,14 Q22,9 12,14 Z", "brown", 0, "dark", 1)   # tonsure ring
    s += dot(18.5, 17, 0.9, "dark") + dot(25.5, 17, 0.9, "dark") + stem("M19,23 Q22,25 25,23", "terra", 0.9, False)
    return s
