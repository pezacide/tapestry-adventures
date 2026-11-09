"""Bayeux knights on foot. Frame 64x96 (rendered at 2x). Faces right."""
from embroider import fill, mail, stem, dot, doc, g, rot

W, H = 64, 96
SHIELD = "M30,37 Q41,31 53,37 Q53,58 42,80 Q30,58 30,37 Z"

def leg(color, shade=False):
    c = color
    s = fill("M31.5,62 L39,62 L38.6,86 L32.4,86 Z", c, 90)
    for y in (68, 74, 80):   # cross-gartering
        s += stem(f"M32.5,{y} L38.2,{y + 4} M38.4,{y} L32.6,{y + 4}", "dark", 0.7)
    s += fill("M31.5,85.5 L39,85.5 Q45.5,86.5 46.5,91.5 L31.5,91.5 Z", "dark" if not shade else "brown", 0, "dark", 1.2)
    if shade:
        s += '<path d="M31.5,62 L39,62 L39,92 L31.5,92 Z" fill="#33251d" opacity="0.18"/>'
    return s

def shield_design(kind, a, b):
    clip = f'<clipPath id="sc"><path d="{SHIELD}"/></clipPath>'
    base = fill(SHIELD, a, 90, None)
    if kind == "cross":
        art = fill("M39,30 L44.5,30 L44.5,82 L39,82 Z", b, 90, "dark", 1) + fill("M28,46 L56,46 L56,51 L28,51 Z", b, 0, "dark", 1)
    elif kind == "bands":
        art = "".join(fill(f"M26,{y} L58,{y - 6} L58,{y - 1} L26,{y + 5} Z", b, 160, "dark", 1) for y in (44, 56, 68))
    elif kind == "bezants":
        art = "".join(dot(x, y, 1.9, b, "dark") for x, y in ((36, 41), (47, 41), (41.5, 52), (36, 60), (47, 60), (41.5, 69)))
    elif kind == "wyrm":   # a little curling dragon
        art = stem("M35,62 Q34,50 42,48 Q50,46 48,40 M42,48 Q46,58 40,66 Q36,70 39,73", b, 2.4, False)
        art += dot(48, 39.5, 2, b, "dark")
    else:
        art = ""
    rim = stem(SHIELD, "dark", 1.7) + stem("M32.5,38.5 Q41,33.5 51,38.5", "cream", 0.8, False)
    return clip + base + f'<g clip-path="url(#sc)">{art}</g>' + dot(41.5, 46 if kind != "cross" else 48.5, 2.2, "gold", "dark") + rim

def weapon(kind, wood="brown"):
    if kind == "spear":
        s = stem("M20.5,10 L19.5,90", "brown", 2.2, False)
        s += fill("M20.6,0.5 Q23.5,6 21,11.5 L19.8,11.5 Q17.5,6 20.6,0.5 Z", "grey", 90, "dark", 1)
        s += fill("M21.2,13 L33,16 L24,19 L21.1,20 Z", "terra", 0, "dark", 1)   # pennon
        return s
    if kind == "sword":
        s = fill("M19.6,12 L22.6,12 L22.6,41 L19.6,41 Z", "sky", 90, "dark", 1)
        s += fill("M21.1,8.5 L22.6,12 L19.6,12 Z", "sky", 90, "dark", 0.8)
        s += fill("M15.5,41 L26.5,41 L26.5,43.5 L15.5,43.5 Z", "gold", 0, "dark", 1)
        return s
    if kind == "axe":
        s = stem("M21,12 L20,78", "brown", 2.3, False)
        s += fill("M20,12 Q11,10 8,19 Q12,25 20.5,23 Z", "grey", 60, "dark", 1.1)
        return s
    if kind == "mace":   # Odo's knobbly club
        s = stem("M21,20 L20,60", "brown", 2.6, False)
        s += fill("M21.5,6 Q27.5,9 26,17 Q24,22 20,21 Q15.5,19 16,12 Q17,7 21.5,6 Z", "ochre", 30, "dark", 1.1)
        s += dot(18.5, 12, 1.2, "brown") + dot(23.5, 11, 1.2, "brown") + dot(21, 17, 1.2, "brown")
        return s
    return ""

def arm(color):
    s = mail("M24,33 Q20,38 19.5,45 L24.5,47 Q26,41 29,37 Z", color, "dark", 1.3)
    s += dot(21.5, 47.5, 2.6, "buff", "dark")   # fist
    return s

def knight(cfg, pose):
    hz, sh = cfg["mail"], cfg["shield"]
    fl, bl = {"idle": (0, 0), "walk1": (-24, 20), "walk2": (20, -24), "jump": (-38, 26)}[pose]
    lift = -3 if pose == "jump" else (-1 if pose.startswith("walk") else 0)
    s = g(leg(cfg["hose"], True), rot(bl, 33, 63) + " translate(-4 0)")
    s += weapon(cfg["weapon"])
    s += arm(hz)
    s += g(leg(cfg["hose"]), rot(fl, 35, 63))
    # hauberk with split skirt
    s += mail("M24,31 Q34,27.5 45,31.5 L48.5,48 L51.5,68 L36,68 L34.5,61 L33,68 L16.5,68 L20.5,48 Z", hz)
    s += stem("M23,66 L33,66 M36,66 L50.5,66", cfg["trim"], 1.6, False)   # coloured hem
    if cfg.get("braid"):
        s += fill("M26,17 Q20,24 22,33 Q23.5,36 25,33 Q24,25 29,19 Z", cfg["braid"], 70, "dark", 1)
    # face + conical nasal helmet
    s += fill("M27,17 L43.5,17 Q46.5,21 45.2,24.5 L44.8,26.5 Q43,30.5 35,31 Q28,30.4 27,25 Z", "buff", 0)
    s += dot(40.2, 21.5, 0.9, "dark")
    s += stem("M41.5,27.8 Q43,28.4 44,27.6", "terra", 0.8, False)
    if cfg.get("beard"):
        s += fill("M33,27 Q38,30 44.6,27.5 Q43,32.5 36,32.5 Q32,31.5 33,27 Z", cfg["beard"], 0, "dark", 1)
    s += fill("M24.5,19.5 Q25.5,8 33.5,1.5 Q41.5,8 45,19.5 Z", cfg["helm"], 75)
    s += fill("M24,18.3 L45.5,18.3 L45.5,21 L24,21 Z", cfg["trim"], 0, "dark", 1)
    s += fill("M42,19 L45,19 L45,27 L42.4,27 Z", cfg["helm"], 90, "dark", 1)
    s += shield_design(cfg["device"], sh[0], sh[1])
    return doc(W, H, g(s, f"translate(0 {lift})"), seed=cfg["seed"])

KNIGHTS = {
    "rubeus": dict(title="Sir Rubeus", mail="terra", hose="blue", helm="grey", trim="gold", shield=("gold", "terra"), device="cross", weapon="spear", beard="brown", seed=3,
                   trait="Balanced all-rounder; the spear-bearer"),
    "azura": dict(title="Dame Azura", mail="blue", hose="terra", helm="sky", trim="cream", shield=("cream", "blue"), device="wyrm", weapon="sword", braid="gold", seed=5,
                  trait="Floaty jumper; glides a moment at the top of a leap"),
    "verdain": dict(title="Sir Verdain", mail="green", hose="ochre", helm="grey", trim="terra", shield=("terra", "gold"), device="bands", weapon="axe", beard="ochre", seed=7,
                    trait="Heavy and strong; his axe breaks masonry"),
    "aurelia": dict(title="Lady Aurelia", mail="ochre", hose="green", helm="grey", trim="indigo", shield=("indigo", "gold"), device="bezants", weapon="mace", braid="terra", seed=9,
                    trait="Swift; fastest on foot, lighter jumps"),
}
POSES = ["idle", "walk1", "walk2", "jump"]
