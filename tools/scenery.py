"""Tapestry scenery: 16x16 tiles, props and parallax backgrounds."""
import math, random
from PIL import Image, ImageDraw
from tapestry import parse, weave

K = (42, 29, 20)
PARCH = (232, 217, 168); INDIGO = (43, 58, 103); WOAD = (62, 111, 163)
MADDER = (166, 58, 46); WELD = (224, 176, 64); LEAF = (79, 122, 58); DLEAF = (52, 80, 42)
BROWN = (107, 74, 47); STONE = (150, 150, 158); DSTONE = (105, 108, 120)

def tile(rows, pal):
    return parse(rows, pal, 16, 16)

def _noise_tile(base, dark, light, seed, density=0.22):
    r = random.Random(seed)
    img = Image.new("RGBA", (16, 16), base + (255,))
    px = img.load()
    for y in range(16):
        for x in range(16):
            v = r.random()
            if (x + y) % 2 == 0 and v < density: px[x, y] = dark + (255,)
            elif (x + y) % 2 == 1 and v < density / 2: px[x, y] = light + (255,)
    return img

# ---------------- tiles ----------------
def ground_top():
    img = _noise_tile(BROWN, (80, 54, 34), (130, 94, 62), 1)
    d = ImageDraw.Draw(img)
    d.rectangle([0, 0, 15, 4], fill=LEAF + (255,))
    for x in range(16):                      # woven grass fringe, scalloped
        h = 5 + (1 if x % 4 in (1, 2) else 0)
        d.line([x, 4, x, h], fill=LEAF + (255,))
        if x % 3 == 0: d.point((x, 1), fill=DLEAF + (255,))
        if x % 4 == 2: d.point((x, 3), fill=(120, 160, 70, 255))
    d.line([0, 0, 15, 0], fill=K + (255,))
    for x in range(0, 16, 4): d.point((x + 1, 8), fill=(220, 190, 120, 255))
    return img

def ground_fill():
    img = _noise_tile(BROWN, (80, 54, 34), (130, 94, 62), 2)
    d = ImageDraw.Draw(img)
    for y in (3, 8, 13):                     # stitched furrows
        for x in range(0, 16, 4): d.line([x, y, x + 1, y], fill=(150, 110, 70, 255))
    return img

def brick():
    img = Image.new("RGBA", (16, 16), STONE + (255,)); d = ImageDraw.Draw(img)
    for y in (0, 5, 10, 15): d.line([0, y, 15, y], fill=DSTONE + (255,))
    for y0, off in ((1, 4), (6, 0), (11, 4)):
        for x in (off, off + 8): d.line([x % 16, y0, x % 16, y0 + 3], fill=DSTONE + (255,))
    for y in (1, 6, 11): d.line([0, y, 15, y], fill=(176, 176, 184, 255))
    d.rectangle([0, 0, 15, 15], outline=K + (255,))
    return img

def platform():
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0)); d = ImageDraw.Draw(img)
    d.rectangle([0, 0, 15, 6], fill=MADDER + (255,), outline=K + (255,))
    for x in range(2, 15, 4):
        d.polygon([(x, 3), (x + 1, 1), (x + 2, 3), (x + 1, 5)], fill=WELD + (255,))
    d.line([1, 6, 14, 6], fill=(120, 36, 30, 255))
    return img

BLOCK_PAL = dict(k=K, G=WELD, g=(166, 120, 40), R=MADDER, r=(120, 36, 30), W=(250, 240, 200), B=BROWN)
def block_shield():
    return tile([
        "kkkkkkkkkkkkkkkk",
        "kGGGGGGGGGGGGGgk",
        "kGkGGGGGGGGGkGgk",
        "kGGGkkkkkkkGGGgk",
        "kGGkRRRWWRRRkGgk",
        "kGGkRRRWWRRRkGgk",
        "kGGkRWWWWWWRkGgk",
        "kGGkRRRWWRRRkGgk",
        "kGGkRRRWWRRRkGgk",
        "kGGGkRRWWRRkGGgk",
        "kGGGGkRWWRkGGGgk",
        "kGGGGGkkkkGGGGgk",
        "kGkGGGGGGGGGkGgk",
        "kGGGGGGGGGGGGGgk",
        "kggggggggggggggk",
        "kkkkkkkkkkkkkkkk",
    ], BLOCK_PAL)
def block_used():
    return _used()
def _used():
    img = Image.new("RGBA", (16, 16), (118, 96, 70, 255)); d = ImageDraw.Draw(img)
    d.rectangle([0, 0, 15, 15], outline=K + (255,))
    for p in ((2, 2), (13, 2), (2, 13), (13, 13)): d.point(p, fill=K + (255,))
    for y in range(4, 12, 2): d.line([4, y, 11, y], fill=(96, 76, 54, 255))
    return img

def coin_frames():
    """Gold florin / spool of thread, 4 frames spinning."""
    out = []
    for hw in (5, 3, 1, 3):
        img = Image.new("RGBA", (16, 16), (0, 0, 0, 0)); d = ImageDraw.Draw(img)
        d.ellipse([8 - hw - 1, 2, 8 + hw, 13], fill=K + (255,))
        d.ellipse([8 - hw, 3, 8 + hw - 1, 12], fill=WELD + (255,))
        if hw > 2:
            d.line([8, 5, 8, 10], fill=(166, 120, 40, 255))
            d.line([7, 7, 9, 7], fill=(166, 120, 40, 255))
            d.point((8 - hw + 1, 5), fill=(250, 230, 150, 255))
        out.append(img)
    return out

TOWER_PAL = dict(k=K, S=STONE, s=DSTONE, L=(176, 176, 184), R=MADDER, W=(250, 240, 200))
def tower_top():   # 32x16 crenellated castle tower cap (the 'pipe')
    img = Image.new("RGBA", (32, 16), (0, 0, 0, 0)); d = ImageDraw.Draw(img)
    for x in range(0, 32, 8):
        d.rectangle([x + 1, 0, x + 5, 5], fill=STONE + (255,), outline=K + (255,))
    d.rectangle([0, 5, 31, 15], fill=STONE + (255,), outline=K + (255,))
    for x in range(4, 31, 8): d.line([x, 7, x, 14], fill=DSTONE + (255,))
    d.line([2, 6, 29, 6], fill=(176, 176, 184, 255))
    return img
def tower_body():
    img = Image.new("RGBA", (32, 16), STONE + (255,)); d = ImageDraw.Draw(img)
    d.line([1, 0, 1, 15], fill=K + (255,)); d.line([30, 0, 30, 15], fill=K + (255,))
    for y in (3, 11): d.line([2, y, 29, y], fill=DSTONE + (255,))
    for x, y in ((8, 0), (20, 0), (14, 7), (26, 7), (4, 7)): d.line([x, y, x, y + 2], fill=DSTONE + (255,))
    d.line([3, 0, 3, 15], fill=(176, 176, 184, 255)); d.line([29, 0, 29, 15], fill=DSTONE + (255,))
    return img

def cloud():
    img = Image.new("RGBA", (48, 20), (0, 0, 0, 0)); d = ImageDraw.Draw(img)
    for cx, cy, r in ((10, 12, 6), (20, 8, 8), (32, 9, 7), (40, 13, 5)):
        d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=K + (255,))
    d.rectangle([6, 12, 42, 18], fill=K + (255,))
    for cx, cy, r in ((10, 12, 5), (20, 8, 7), (32, 9, 6), (40, 13, 4)):
        d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=(246, 240, 222, 255))
    d.rectangle([7, 12, 42, 17], fill=(246, 240, 222, 255))
    for cx, cy in ((20, 8), (32, 10)):       # swirl motif
        d.arc([cx - 3, cy - 3, cx + 3, cy + 3], 20, 300, fill=(190, 205, 225, 255))
    d.line([9, 16, 41, 16], fill=(200, 210, 225, 255))
    return img

def tree():   # mille-fleur tree 32x48
    img = Image.new("RGBA", (32, 48), (0, 0, 0, 0)); d = ImageDraw.Draw(img)
    d.rectangle([13, 24, 18, 47], fill=BROWN + (255,), outline=K + (255,))
    d.line([15, 26, 15, 46], fill=(80, 54, 34, 255))
    d.ellipse([1, 1, 30, 30], fill=K + (255,))
    d.ellipse([2, 2, 29, 29], fill=LEAF + (255,))
    r = random.Random(7)
    for _ in range(60):
        x, y = r.randint(4, 27), r.randint(4, 27)
        if (x - 15.5) ** 2 + (y - 15.5) ** 2 < 11.5 ** 2:
            d.point((x, y), fill=DLEAF + (255,) if r.random() < .6 else (120, 160, 70, 255))
    for x, y in ((9, 9), (20, 7), (14, 15), (24, 17), (7, 20), (17, 23), (12, 4)):   # fruit
        d.rectangle([x, y, x + 1, y + 1], fill=MADDER + (255,))
    return img

def flag():   # goal banner 16x48
    img = Image.new("RGBA", (16, 48), (0, 0, 0, 0)); d = ImageDraw.Draw(img)
    d.rectangle([2, 4, 3, 47], fill=(120, 96, 60, 255), outline=K + (255,))
    d.ellipse([0, 0, 5, 5], fill=WELD + (255,), outline=K + (255,))
    d.polygon([(4, 6), (15, 6), (15, 24), (9, 20), (4, 24)], fill=MADDER + (255,), outline=K + (255,))
    d.polygon([(8, 10), (11, 13), (8, 16), (6, 13)], fill=WELD + (255,))
    return img

def bush():
    img = Image.new("RGBA", (32, 16), (0, 0, 0, 0)); d = ImageDraw.Draw(img)
    for cx, cy, r in ((8, 10, 7), (16, 7, 8), (25, 10, 7)): d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=K + (255,))
    d.rectangle([4, 10, 28, 15], fill=K + (255,))
    for cx, cy, r in ((8, 10, 6), (16, 7, 7), (25, 10, 6)): d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=LEAF + (255,))
    d.rectangle([5, 10, 27, 14], fill=LEAF + (255,))
    for x, y in ((6, 8), (13, 5), (20, 6), (26, 9), (16, 11), (10, 12)): d.point((x, y), fill=(120, 160, 70, 255))
    for x, y in ((9, 7), (18, 9), (23, 5)): d.rectangle([x, y, x + 1, y + 1], fill=WELD + (255,))
    return img

def flowers():  # mille-fleur ground cover 16x16
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0)); d = ImageDraw.Draw(img)
    for x, y, c in ((3, 8, MADDER), (9, 5, WELD), (13, 10, (240, 235, 215))):
        d.line([x, y + 1, x, 15], fill=DLEAF + (255,))
        d.rectangle([x - 1, y - 1, x + 1, y + 1], fill=c + (255,)); d.point((x, y), fill=K + (255,))
    return img

# ---------------- backgrounds (480x270) ----------------
BW, BH = 480, 270

def bg_sky():
    img = Image.new("RGBA", (BW, BH)); px = img.load()
    stops = [(0, (30, 44, 90)), (110, (52, 96, 150)), (190, (120, 165, 190)), (270, (232, 217, 168))]
    for y in range(BH):
        for i in range(len(stops) - 1):
            if stops[i][0] <= y <= stops[i + 1][0]:
                t = (y - stops[i][0]) / (stops[i + 1][0] - stops[i][0])
                c = tuple(int(a + (b - a) * t) for a, b in zip(stops[i][1], stops[i + 1][1]))
        band = 0.96 if (y // 3) % 2 else 1.0      # weft bands
        for x in range(BW): px[x, y] = tuple(int(v * band) for v in c) + (255,)
    d = ImageDraw.Draw(img); r = random.Random(3)
    for _ in range(26):                          # tiny woven stars/flowers
        x, y = r.randint(0, BW - 4), r.randint(0, 100)
        c = (240, 225, 160, 255)
        d.rectangle([x + 1, y, x + 1, y + 2], fill=c); d.rectangle([x, y + 1, x + 2, y + 1], fill=c)
    c = cloud()
    for x, y in ((30, 40), (190, 70), (330, 30), (420, 90), (110, 120)): img.alpha_composite(c, (x, y))
    return img

def _castle(w=64, h=80):
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0)); d = ImageDraw.Draw(img)
    def tower(x0, x1, top):
        d.rectangle([x0, top, x1, h], fill=(96, 104, 130, 255), outline=K + (255,))
        for x in range(x0, x1 - 2, 6): d.rectangle([x + 1, top - 5, x + 3, top], fill=(96, 104, 130, 255), outline=K + (255,))
        mid = (x0 + x1) // 2
        d.rectangle([mid - 1, top + 12, mid + 1, top + 20], fill=K + (255,))
        d.line([x0 + 1, top + 1, x1 - 1, top + 1], fill=(130, 140, 170, 255))
    tower(2, 16, 28); tower(48, 62, 34); tower(20, 44, 40); tower(26, 38, 8)
    d.line([32, 8, 32, 0], fill=K + (255,)); d.polygon([(33, 0), (42, 2), (33, 5)], fill=MADDER + (255,))
    d.rectangle([28, 56, 36, h], fill=K + (255,))
    return img

def bg_far():
    img = Image.new("RGBA", (BW, BH), (0, 0, 0, 0)); d = ImageDraw.Draw(img)
    for y in range(BH):                          # blue-grey rolling hills, tileable (periods divide BW)
        pass
    for x in range(BW):
        h = 150 + 22 * math.sin(2 * math.pi * x / BW * 2) + 10 * math.sin(2 * math.pi * x / BW * 5 + 1)
        for y in range(int(h), BH):
            band = ((y - int(h)) // 4) % 2
            c = (92, 120, 150) if band else (84, 111, 140)
            if (x + y) % 6 == 0: c = (104, 132, 162)       # diagonal stitching
            img.putpixel((x, y), c + (255,))
        img.putpixel((x, int(h)), K + (255,))
    cs = _castle()
    for cx in (60, 300): img.alpha_composite(cs, (cx, int(150 + 22 * math.sin(2 * math.pi * (cx + 32) / BW * 2)) - 62))
    return img

def bg_near():
    img = Image.new("RGBA", (BW, BH), (0, 0, 0, 0)); d = ImageDraw.Draw(img)
    base = 190
    for x in range(BW):
        h = base + 8 * math.sin(2 * math.pi * x / BW * 3)
        for y in range(int(h), BH):
            c = LEAF if ((y - int(h)) // 3) % 2 == 0 else (68, 108, 50)
            img.putpixel((x, y), c + (255,))
        img.putpixel((x, int(h)), K + (255,))
    t = tree(); r = random.Random(11)
    for x in (20, 140, 250, 380, 450):
        y = int(base + 8 * math.sin(2 * math.pi * (x + 16) / BW * 3)) - 44
        img.alpha_composite(t, (x - 16 if x - 16 > 0 else 0, y))
    for _ in range(70):                          # mille-fleur scatter
        x, y = r.randint(2, BW - 5), r.randint(base + 14, BH - 4)
        c = r.choice([MADDER, WELD, (240, 235, 215), (150, 110, 190)])
        d.rectangle([x, y, x + 1, y + 1], fill=c + (255,)); d.point((x, y + 2), fill=DLEAF + (255,))
    return img

def border_strip():
    """Tapestry border: 16px tall, tiles horizontally. Madder band, gold lozenges."""
    img = Image.new("RGBA", (32, 16), MADDER + (255,)); d = ImageDraw.Draw(img)
    d.line([0, 1, 31, 1], fill=WELD + (255,)); d.line([0, 14, 31, 14], fill=WELD + (255,))
    d.line([0, 0, 31, 0], fill=K + (255,)); d.line([0, 15, 31, 15], fill=K + (255,))
    for cx in (8, 24):
        d.polygon([(cx, 3), (cx + 5, 8), (cx, 12), (cx - 5, 8)], fill=WELD + (255,), outline=K + (255,))
        d.polygon([(cx, 6), (cx + 2, 8), (cx, 10), (cx - 2, 8)], fill=INDIGO + (255,))
    return img

