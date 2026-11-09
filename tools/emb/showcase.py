"""Compose assets/showcase.png (1920x1080) from the rendered assets."""
import json, os
from PIL import Image
R = os.path.normpath(os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..", "assets"))
A = json.load(open(os.path.join(R, "atlas.json"))); K = A["scale"]
def L(p): return Image.open(os.path.join(R, p)).convert("RGBA")
def fr(cat, n, i=0):
    im = L(A[cat][n]["file"]); fw = A[cat][n].get("frame", A[cat][n].get("size"))[0] * K
    return im.crop((i * fw, 0, (i + 1) * fw, im.height))
sc = L("backgrounds/linen.png")
def put(im, x, y): sc.alpha_composite(im, (int(x * K), int(y * K)))
put(L("backgrounds/far.png"), 0, 0); put(L("backgrounds/near.png"), 0, -40)
for x in range(0, 960, 384): put(L("scenery/border.png"), x, 0)
put(L("scenery/inscription.png"), 100, 70)
GY = 444
for x in range(0, 672, 48):
    put(L("scenery/ground_top.png"), x, GY); put(L("scenery/ground_fill.png"), x, GY + 48)
for x in range(672, 960, 48):
    for y in (GY + 12, GY + 60): put(L("scenery/sea.png"), x, y)
for i, n in enumerate(("masonry", "roundel_block", "masonry", "roundel_used", "masonry")):
    put(L(f"scenery/{n}.png"), 220 + 48 * i, GY - 150)
for i in range(4): put(fr("scenery", "bezant", i), 232 + 48 * i, GY - 190)
put(L("scenery/tower_top.png"), 540, GY - 96); put(L("scenery/tower_body.png"), 540, GY - 48)
put(L("scenery/gonfanon.png"), 640, GY - 144)
put(fr("enemies", "whale_boss", 1), 704, 540 - 168)
put(L("scenery/ship.png"), 724, 540 - 168 - 30)
put(fr("enemies", "owl_face"), 470, GY - 260)
put(fr("enemies", "wyvern"), 380, GY - 240)
x = 20
for n, p in (("rubeus", 0), ("azura", 1), ("verdain", 2), ("aurelia", 3)):
    put(fr("heroes", n, p), x, GY - 96 - (14 if p == 3 else 0)); x += 58
for n, x in (("snail_hound", 262), ("killer_hare", 330), ("bayeux_hound", 380), ("unicorn", 455)):
    e = fr("enemies", n); put(e, x, GY - e.height / K)
cat = fr("enemies", "soup_cat", 2); put(cat, 540 + 8, GY - 96 - 82)
sc.convert("RGB").save(os.path.join(R, "showcase.png"), optimize=True)
print("ok")
