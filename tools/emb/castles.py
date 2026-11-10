"""End-of-level buildings, one per world, in the Bayeux manner. Each has a ground-level doorway at its centre-x."""
from embroider import fill, stem, dot, g, WOOL
from creatures import clip, scales
import scenery

C = {}

def wall(x0, y0, x1, y1, color, joint="brown", course=10):
    s = fill(f"M{x0},{y0} L{x1},{y0} L{x1},{y1} L{x0},{y1} Z", color, 0)
    lines, row = [], 0
    y = y0 + course
    while y < y1 - 1:
        lines.append(f"M{x0 + 1},{y} L{x1 - 1},{y}")
        off = 9 if row % 2 else 0
        x = x0 + 6 + off
        while x < x1 - 2:
            lines.append(f"M{x},{y - course + 1} L{x},{y - 1}"); x += 18
        y += course; row += 1
    return s + clip(f"w{x0}_{y0}", f"M{x0},{y0} L{x1},{y0} L{x1},{y1} L{x0},{y1} Z", stem(" ".join(lines), joint, 0.8, False)) + stem(f"M{x0},{y0} L{x1},{y0} L{x1},{y1} L{x0},{y1} Z", "dark", 1.6)

def crenels(x0, x1, y, color, w=9, gap=7, h=9):
    s, x = "", x0
    while x + w <= x1 + 0.5:
        s += fill(f"M{x},{y - h} L{x + w},{y - h} L{x + w},{y + 1} L{x},{y + 1} Z", color, 0, "dark", 1.3); x += w + gap
    return s

def roof(cx, base, w, h, color, cid):
    d = f"M{cx - w / 2},{base} L{cx},{base - h} L{cx + w / 2},{base} Z"
    return fill(d, color, 0, None) + clip(cid, d, scales(cx - w / 2 - 6, base - h, cx + w / 2 + 6, base + 4, 7, "dark", 0.8)) + stem(d, "dark", 1.6)

def dome(cx, base, w, h, color, cid):
    d = f"M{cx - w / 2},{base} Q{cx - w / 2},{base - h} {cx},{base - h} Q{cx + w / 2},{base - h} {cx + w / 2},{base} Z"
    return fill(d, color, 0, None) + clip(cid, d, scales(cx - w / 2 - 6, base - h, cx + w / 2 + 6, base + 4, 7, "dark", 0.8)) + stem(d, "dark", 1.6)

def door(cx, bottom, w, h, portcullis=False):
    d = f"M{cx - w / 2},{bottom} L{cx - w / 2},{bottom - h + w / 2} Q{cx - w / 2},{bottom - h} {cx},{bottom - h} Q{cx + w / 2},{bottom - h} {cx + w / 2},{bottom - h + w / 2} L{cx + w / 2},{bottom} Z"
    s = fill(d, "dark", 0, "dark", 1.6)
    s += stem(f"M{cx - w / 2 - 3},{bottom} L{cx - w / 2 - 3},{bottom - h + w / 2} Q{cx - w / 2 - 3},{bottom - h - 3} {cx},{bottom - h - 3} Q{cx + w / 2 + 3},{bottom - h - 3} {cx + w / 2 + 3},{bottom - h + w / 2} L{cx + w / 2 + 3},{bottom}", "ochre", 1.6, False)
    if portcullis:
        bars = " ".join(f"M{x},{bottom - h + 4} L{x},{bottom - h * 0.35}" for x in range(int(cx - w / 2 + 4), int(cx + w / 2 - 2), 5))
        bars += " " + " ".join(f"M{cx - w / 2 + 1},{y} L{cx + w / 2 - 1},{y}" for y in range(int(bottom - h + 10), int(bottom - h * 0.35), 6))
        s += clip(f"pc{cx}", d, stem(bars, "grey", 1.1, False))
    return s

def window(cx, cy, w=7, h=12):
    return fill(f"M{cx - w / 2},{cy + h / 2} L{cx - w / 2},{cy - h / 2 + w / 2} Q{cx},{cy - h / 2 - 1} {cx + w / 2},{cy - h / 2 + w / 2} L{cx + w / 2},{cy + h / 2} Z", "indigo", 90, "dark", 1)

def pennant(x, top, color, h=26, flip=False):
    s = stem(f"M{x},{top + h} L{x},{top}", "brown", 1.8, False)
    d = f"M{x},{top + 2} L{x + 18},{top + 6} L{x},{top + 11} Z" if not flip else f"M{x},{top + 2} L{x - 18},{top + 6} L{x},{top + 11} Z"
    return s + fill(d, color, 0, "dark", 1)

def stripes_mound(d, cid, cols=("green", "ochre", "sage", "terra"), y0=0, y1=300, w=400, step=9):
    body = "".join(stem(f"M-10,{y} Q{w / 2},{y - 50} {w + 10},{y}", cols[i % len(cols)], 2.4, False) for i, y in enumerate(range(y0, y1, step)))
    return fill(d, "green", 0, None) + clip(cid, d, body) + stem(d, "dark", 1.8)

# ---------- World 1: Ye Keepe upon ye Motte ----------
def keep():
    W, H = 288, 264
    s = stripes_mound("M0,264 Q30,180 104,156 L184,156 Q258,180 288,264 Z", "km", y0=150, y1=300, w=W)
    s += stem("M20,262 L100,166", "brown", 3.2, False) + stem("M30,252 L40,254 M46,234 L56,236 M62,216 L72,218 M78,198 L88,200", "brown", 1.4, False)  # flying bridge
    for x in range(100, 190, 8):   # palisade
        s += fill(f"M{x},{158} L{x},{146} L{x + 3},{141} L{x + 6},{146} L{x + 6},{158} Z", "brown", 90, "dark", 0.9)
    s += wall(110, 70, 178, 152, "buff")
    s += fill("M106,70 L114,70 L114,152 L106,152 Z", "terra", 90, "dark", 1.1) + fill("M174,70 L182,70 L182,152 L174,152 Z", "terra", 90, "dark", 1.1)
    s += crenels(106, 182, 70, "ochre")
    s += window(130, 96) + window(158, 96) + window(144, 124)
    s += roof(144, 60, 46, 36, "terra", "kr") + pennant(144, 2, "blue")
    s += wall(116, 206, 172, 264, "sage", "dark")   # gatehouse at the foot of the mound
    s += crenels(114, 174, 206, "terra", 8, 6, 8)
    s += door(144, 264, 24, 44, True)
    return s
C["castle_keep"] = dict(size=(288, 264), title="Ye Keepe upon ye Motte", fn=keep)

# ---------- World 2: Ye Tower by ye Sea ----------
def sea_tower():
    W, H = 240, 288
    rock = "M8,288 Q14,240 54,232 Q90,214 120,226 Q160,212 196,232 Q232,240 234,288 Z"
    s = fill(rock, "grey", 30) + clip("rk", rock, stem("M10,262 Q60,250 120,258 T232,256 M30,244 Q80,236 120,240 T214,240", "brown", 1.4, False)) + stem(rock, "dark", 1.6)
    s += wall(150, 150, 196, 236, "sky") + crenels(146, 200, 150, "blue", 8, 6, 8) + window(173, 186) + roof(173, 140, 40, 34, "terra", "sr2") + pennant(173, 84, "terra")
    t = "M70,236 L70,92 L150,92 L150,236 Z"
    bands = "".join(fill(f"M70,{y} L150,{y} L150,{y + 12} L70,{y + 12} Z", ("buff", "ochre", "terra", "buff", "sage")[i % 5], 0, None) for i, y in enumerate(range(92, 236, 12)))
    s += clip("tw", t, bands + stem(" ".join(f"M70,{y} L150,{y}" for y in range(104, 236, 12)), "dark", 0.7, False)) + stem(t, "dark", 1.8)
    s += stem("M78,92 L78,236 M142,92 L142,236", "cream", 0.8, False)   # roundness of the drum
    s += crenels(66, 154, 92, "blue")
    s += window(96, 120) + window(124, 120) + window(110, 156) + window(110, 190, 8, 10)
    s += dome(110, 82, 74, 44, "blue", "sd") + stem("M110,38 L110,14 M103,22 L117,22", "gold", 2, False)
    s += door(110, 236, 26, 40)
    s += stem("M100,236 L120,236", "ochre", 1, False)
    for i, y in enumerate((262, 276)):   # waves lapping the rock
        s += stem(f"M0,{y} " + " ".join(f"q6,{-5 if (j + i) % 2 else -3} 12,0" for j in range(20)), ("blue", "sky")[i], 2.2, False)
    return s
C["castle_sea_tower"] = dict(size=(240, 288), title="Ye Tower by ye Sea", fn=sea_tower)

# ---------- World 3: Ye Castel of Stone ----------
def stone_castle():
    W, H = 336, 264
    s = wall(50, 120, 286, 264, "grey", "dark") + crenels(50, 286, 120, "grey")
    for cx, c in ((36, "blue"), (300, "blue")):
        s += wall(cx - 34, 70, cx + 34, 264, "buff") + crenels(cx - 36, cx + 36, 70, "terra", 9, 6, 9)
        s += window(cx, 104) + window(cx, 150) + window(cx, 196)
        s += roof(cx, 60, 76, 46, c, f"st{cx}") + pennant(cx, 0, "terra", 16, cx > 200)
    s += wall(124, 74, 212, 264, "ochre") + crenels(120, 216, 74, "terra")
    s += window(148, 104) + window(188, 104) + window(168, 136, 8, 14)
    s += fill("M160,30 L176,30 L176,74 L160,74 Z", "terra", 90, "dark", 1.2) + crenels(158, 178, 30, "terra", 6, 4, 6)
    s += pennant(168, 0, "gold", 20)
    s += door(168, 264, 40, 72, True)
    s += stem("M60,232 L118,232 M218,232 L276,232", "terra", 2.6, False)   # a coloured course, as on Bayeux buildings
    return s
C["castle_stone"] = dict(size=(336, 264), title="Ye Castel of Stone", fn=stone_castle)

# ---------- World 4: Ye Greate Halle of ye Cat King ----------
def great_hall():
    W, H = 336, 288
    s = wall(40, 150, 296, 288, "buff")
    for i, x in enumerate(range(52, 290, 40)):   # arcade
        if x == 172: continue
        s += fill(f"M{x - 5},{288} L{x - 5},{170} L{x + 1},{170} L{x + 1},{288} Z", ("terra", "blue", "green")[i % 3], 90, "dark", 1)
        s += fill(f"M{x + 6},288 L{x + 6},206 Q{x + 18},186 {x + 30},206 L{x + 30},288 Z", "indigo", 90, "dark", 1.2)
    roofd = "M30,152 Q168,104 306,152 Z"
    s += fill(roofd, "terra", 0, None) + clip("ghr", roofd, scales(24, 104, 312, 156, 8, "dark", 0.8)) + stem(roofd, "dark", 1.8)
    for x, f in ((26, False), (310, True)):   # beast-head gable finials, as in the Tapestry
        hd = f"M{x},{150} Q{x - 10 if not f else x + 10},{140} {x - 16 if not f else x + 16},{146} L{x - 18 if not f else x + 18},{152} Q{x - 8 if not f else x + 8},{156} {x},{154} Z"
        s += fill(hd, "gold", 0, "dark", 1.1) + dot(x - 10 if not f else x + 10, 146, 1.2, "dark")
    for cx in (40, 296):
        s += wall(cx - 22, 96, cx + 22, 288, "sage") + window(cx, 130) + window(cx, 180)
        s += dome(cx, 96, 52, 36, "blue", f"gd{cx}") + stem(f"M{cx},60 L{cx},44 M{cx - 5},50 L{cx + 5},50", "gold", 1.8, False)
    s += wall(138, 76, 198, 150, "ochre") + window(156, 104) + window(180, 104)
    s += dome(168, 76, 72, 50, "terra", "gdc") + stem("M168,26 L168,4 M161,12 L175,12", "gold", 2.2, False)
    s += door(168, 288, 44, 76, False)
    s += fill("M146,212 L190,212 L190,216 L146,216 Z", "gold", 0, None)
    return s
C["castle_great_hall"] = dict(size=(336, 288), title="Ye Greate Halle of ye Cat King", fn=great_hall)

# ---------- World 1-2: Ye Abbey in ye Wood ----------
def abbey():
    W, H = 288, 288
    s = wall(20, 150, 268, 288, "buff")
    s += fill("M12,152 L144,104 L276,152 Z", "terra", 0, None) + clip("abr", "M12,152 L144,104 L276,152 Z", scales(8, 100, 280, 156, 7, "dark", 0.8)) + stem("M12,152 L144,104 L276,152 Z", "dark", 1.8)
    for x in (44, 80, 208, 244):
        s += window(x, 200, 10, 22)
        s += fill(f"M{x - 12},288 L{x - 12},170 L{x - 8},170 L{x - 8},288 Z", ("blue", "green")[(x // 40) % 2], 90, "dark", 1)
    s += wall(110, 60, 178, 288, "sage")
    s += crenels(106, 182, 60, "ochre")
    s += window(144, 92, 10, 20) + window(128, 134) + window(160, 134)
    s += fill("M136,100 a8,8 0 1,0 16,0 a8,8 0 1,0 -16,0 Z", "gold", 0, "dark", 1.1)   # the bell
    s += roof(144, 50, 56, 48, "blue", "abs") + stem("M144,4 L144,-10 M137,-4 L151,-4".replace("-10", "0").replace("-4", "2"), "gold", 2.2, False)
    s += door(144, 288, 34, 62)
    s += fill("M232,150 L262,150 L262,288 L232,288 Z", "ochre", 90, "dark", 1.4) + roof(247, 150, 40, 36, "terra", "abt")
    return s
C["castle_abbey"] = dict(size=(288, 288), title="Ye Abbey in ye Wood", fn=abbey)

# ---------- World 1-3: Ye Rounde Tower ----------
def round_tower():
    W, H = 240, 300
    s = wall(10, 200, 230, 300, "grey", "dark") + crenels(10, 230, 200, "grey")   # curtain wall
    t = "M64,300 L64,80 L176,80 L176,300 Z"
    bands = "".join(fill(f"M64,{y} L176,{y} L176,{y + 16} L64,{y + 16} Z", ("buff", "ochre", "buff", "terra")[i % 4], 0, None) for i, y in enumerate(range(80, 300, 16)))
    s += clip("rt", t, bands + stem(" ".join(f"M64,{y} L176,{y}" for y in range(96, 300, 16)), "dark", 0.7, False)) + stem(t, "dark", 1.8)
    s += stem("M74,80 L74,300 M166,80 L166,300", "cream", 0.9, False)
    s += crenels(60, 180, 80, "terra")
    s += window(96, 120) + window(144, 120) + window(120, 164, 9, 16) + window(96, 210) + window(144, 210)
    s += roof(120, 70, 128, 66, "green", "rtr") + pennant(120, 0, "terra", 22)
    s += door(120, 300, 34, 58, True)
    return s
C["castle_round_tower"] = dict(size=(240, 300), title="Ye Rounde Tower", fn=round_tower)

# ---------- World 1-3 (above ground): ye way down into ye undercroft ----------
def crypt_entrance():
    W, H = 192, 216
    s = wall(20, 96, 172, 216, "grey", "dark")
    s += fill("M12,98 L96,40 L180,98 Z", "blue", 0, None) + clip("cer", "M12,98 L96,40 L180,98 Z", scales(8, 36, 184, 100, 7, "dark", 0.8)) + stem("M12,98 L96,40 L180,98 Z", "dark", 1.8)
    s += stem("M96,40 L96,14 M88,22 L104,22", "gold", 2.4, False)
    s += fill("M68,216 L68,140 Q96,112 124,140 L124,216 Z", "dark", 0, "dark", 1.6)
    for i, y in enumerate(range(150, 216, 11)):   # steps going down into the dark
        s += stem(f"M{72 + i * 2},{y} L{120 - i * 2},{y}", "grey", 2, False)
    s += stem("M64,216 L64,138 Q96,106 128,138 L128,216", "ochre", 2, False)
    s += window(44, 140) + window(148, 140)
    s += fill("M30,104 L50,104 L50,216 L30,216 Z", "terra", 90, "dark", 1.1) + fill("M142,104 L162,104 L162,216 L142,216 Z", "terra", 90, "dark", 1.1)
    return s
C["crypt_entrance"] = dict(size=(192, 216), title="Ye Way Downe", fn=crypt_entrance)

# ---------- end of the undercroft: ye stair back up to ye light ----------
def crypt_exit():
    W, H = 144, 288
    s = wall(4, 0, 140, 288, "grey", "dark")
    arch = "M30,288 L30,90 Q72,40 114,90 L114,288 Z"
    s += fill(arch, "cream", 90, "dark", 1.6)                          # daylight at the top of the stair
    s += clip("cx", arch, fill("M30,170 L114,170 L114,288 L30,288 Z", "dark", 0, None)
              + "".join(stem(f"M{30 + i * 6},{288 - i * 16} L{114 - i * 6},{288 - i * 16}", "grey", 2.2, False) for i in range(8))
              + "".join(stem(f"M{50 + i * 12},70 L{44 + i * 14},150", "gold", 1.2, False) for i in range(4)))   # shafts of light
    s += stem("M26,288 L26,88 Q72,34 118,88 L118,288", "ochre", 2.2, False)
    return s
C["crypt_exit"] = dict(size=(144, 288), title="Ye Stair to ye Lyght", fn=crypt_exit)

# ---------- World 2-1: Ye Harbour Gate ----------
def harbour_gate():
    W, H = 264, 264
    s = wall(14, 120, 250, 264, "grey", "dark") + crenels(14, 250, 120, "grey")
    s += wall(90, 50, 174, 264, "buff") + crenels(86, 178, 50, "terra")
    s += fill("M80,64 L184,64 L184,92 L80,92 Z", "brown", 0, "dark", 1.4)   # timber hoarding
    s += stem("M80,78 L184,78 M96,64 L96,92 M112,64 L112,92 M128,64 L128,92 M144,64 L144,92 M160,64 L160,92", "ochre", 1, False)
    s += window(110, 116) + window(154, 116)
    s += pennant(132, 2, "blue", 30) + pennant(40, 76, "terra", 18) + pennant(224, 76, "terra", 18, True)
    s += door(132, 264, 44, 78, True)
    return s
C["castle_harbour_gate"] = dict(size=(264, 264), title="Ye Harbour Gate", fn=harbour_gate)

for k, v in C.items():
    scenery.S[k] = (v["size"][0], v["size"][1], v["fn"], False)
