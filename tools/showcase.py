"""Compose a showcase scene from the generated assets -> assets/showcase.png"""
import os
from PIL import Image
R = os.path.join(os.path.dirname(__file__), "..", "assets")
def L(p): return Image.open(os.path.join(R, p)).convert("RGBA")
def frame(img, i, w): return img.crop((i * w, 0, (i + 1) * w, img.height))
scene = L("backgrounds/sky.png")
scene.alpha_composite(L("backgrounds/far.png")); scene.alpha_composite(L("backgrounds/near.png"))
gy = 220
top, fill, brick, blk, used = (L(f"scenery/{n}.png") for n in ("ground_top", "ground_fill", "brick", "block_shield", "block_used"))
for x in range(0, 480, 16):
    scene.alpha_composite(top, (x, gy))
    for y in range(gy + 16, 270, 16): scene.alpha_composite(fill, (x, y))
for i, x in enumerate(range(96, 96 + 16 * 5, 16)): scene.alpha_composite([brick, blk, brick, used, brick][i], (x, gy - 64))
coin = L("scenery/coin.png")
for i, x in enumerate(range(100, 164, 16)): scene.alpha_composite(frame(coin, i % 4, 16), (x, gy - 84))
scene.alpha_composite(L("scenery/tower_top.png"), (400, gy - 32)); scene.alpha_composite(L("scenery/tower_body.png"), (400, gy - 16))
scene.alpha_composite(L("scenery/flag.png"), (456, gy - 48))
scene.alpha_composite(L("scenery/bush.png"), (20, gy - 14)); scene.alpha_composite(L("scenery/flowers.png"), (330, gy - 14))
x = 12
for n in ("rubeus", "azura", "verdain", "aurelia"):
    scene.alpha_composite(frame(L(f"heroes/{n}.png"), 0 if n != "azura" else 1, 20), (x, gy - 24)); x += 26
ex = 190
for n, fw, fh in (("snail_knight", 16, 16), ("bishop_fox", 16, 16), ("hare_jouster", 16, 16), ("winged_boar", 16, 16), ("thistle_hedgehog", 16, 16)):
    y = gy - fh - (36 if n == "winged_boar" else 0)
    scene.alpha_composite(frame(L(f"enemies/{n}.png"), 0, fw), (ex, y)); ex += 22
scene.alpha_composite(L("enemies/wyrm_boss.png"), (318, gy - 24))
scene.resize((scene.width * 3, scene.height * 3), Image.NEAREST).save(os.path.join(R, "showcase.png"))
print("showcase ok")
