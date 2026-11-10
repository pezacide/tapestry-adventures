"""Render the whole of World 1-1 from the real art -> assets/level1_preview.png (for checking the layout)."""
import os, re
from PIL import Image
ROOT = os.path.normpath(os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", ".."))
A = os.path.join(ROOT, "assets")
src = open(os.path.join(ROOT, "unity/Assets/Scripts/Levels/Level1.cs")).read()
rows = re.findall(r'^\s+"(.*)",$', src, re.M)
props = re.findall(r'new Decor\("([^"]+)", (\d+)\)', src)
H = len(rows); W = max(len(r) for r in rows); T = 48
def load(key, frame=0, frames=1):
    im = Image.open(os.path.join(A, key + ".png")).convert("RGBA")
    fw = im.width // frames
    im = im.crop((frame * fw, 0, (frame + 1) * fw, im.height))
    return im.resize((im.width // 2, im.height // 2), Image.LANCZOS)
def get(x, y):
    r = rows[H - 1 - y]; return r[x] if 0 <= x < len(r) else " "
out = Image.new("RGBA", (W * T, H * T), (236, 226, 201, 255))
def put(im, x, y):   # bottom-centre at tile coords
    out.alpha_composite(im, (int(x * T - im.width / 2), int((H - y) * T - im.height)))
for x0 in range(0, W * T, 960): out.alpha_composite(load("backgrounds/far"), (x0, H * T - 540))
for key, x in props:
    x = float(x); gy = next((y + 1 for y in range(H - 1, -1, -1) if get(int(x), y) in "#BXHT?t"), 0)
    put(load(key), x, gy)
tiles = {"B": "scenery/masonry", "?": "scenery/masonry", "H": "scenery/masonry", "~": "scenery/sea"}
done = set()
for y in range(H):
    for x in range(W):
        c = get(x, y)
        if c == "#": put(load("scenery/ground_fill" if get(x, y + 1) == "#" else "scenery/ground_top"), x + 0.5, y)
        elif c in tiles: put(load(tiles[c]), x + 0.5, y)
        elif c == "=": put(load("scenery/plank"), x + 0.5, y + 1 - 18 / 48)
        elif c in "Tt" and (x, y) not in done:
            done.add((x, y)); done.add((x + 1, y)); put(load("scenery/tower_top" if c == "T" else "scenery/tower_body"), x + 1, y)
spr = {"s": ("enemies/snail_hound", 3), "h": ("enemies/killer_hare", 2), "l": ("enemies/fiery_lion", 3), "d": ("enemies/bayeux_hound", 2),
       "c": ("enemies/camel_dragon", 2), "w": ("enemies/wyvern", 2), "u": ("enemies/unicorn", 2), "O": ("enemies/owl_face", 2), "k": ("enemies/soup_cat", 3)}
for y in range(H):
    for x in range(W):
        c = get(x, y)
        if c in spr: put(load(spr[c][0], 0, spr[c][1]), x + 0.5, y)
        elif c == "o": put(load("scenery/bezant", 0, 4), x + 0.5, y + 0.25)
        elif c == "G": put(load("scenery/goal_pole"), x + 0.5, y)
        elif c == "P": put(load("heroes/rubeus", 0, 5), x + 0.5, y)
out.convert("RGB").save(os.path.join(A, "level1_preview.png"), optimize=True)
print(out.size)
