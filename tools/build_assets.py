"""Generate every sprite, tile and background into ../assets (run: python3 tools/build_assets.py)."""
import json, os, sys
sys.path.insert(0, os.path.dirname(__file__))
from PIL import Image
import heroes, enemies, scenery as sc
from tapestry import sheet, stitch, weave

ROOT = os.path.join(os.path.dirname(__file__), "..", "assets")
for d in ("heroes", "enemies", "scenery", "backgrounds"):
    os.makedirs(os.path.join(ROOT, d), exist_ok=True)
S = 4  # stitched preview scale
atlas = {"heroes": {}, "enemies": {}, "scenery": {}, "backgrounds": {}}

def save(img, folder, name, stitched=True):
    p = os.path.join(ROOT, folder, name)
    img.save(p + ".png")
    if stitched:
        stitch(img, S).save(p + "@4x.png")

# heroes
for k, kn in heroes.KNIGHTS.items():
    frames = [heroes.frame(k, p) for p in heroes.POSES]
    save(sheet(frames), "heroes", k)
    atlas["heroes"][k] = dict(title=kn["title"], trait=kn["trait"], frame=[heroes.W, heroes.H], frames=heroes.POSES, file=f"heroes/{k}.png")

# enemies
for n, e in enemies.ENEMIES.items():
    frames = enemies.build(n)
    save(sheet(frames), "enemies", n)
    atlas["enemies"][n] = dict(title=e["title"], behaviour=e["behaviour"], frame=list(e["size"]), frame_count=len(frames), file=f"enemies/{n}.png")

# scenery tiles / props (woven texture baked in)
items = {
    "ground_top": sc.ground_top(), "ground_fill": sc.ground_fill(), "brick": sc.brick(),
    "platform": sc.platform(), "block_shield": sc.block_shield(), "block_used": sc.block_used(),
    "tower_top": sc.tower_top(), "tower_body": sc.tower_body(), "cloud": sc.cloud(),
    "tree": sc.tree(), "flag": sc.flag(), "bush": sc.bush(), "flowers": sc.flowers(),
    "border_strip": sc.border_strip(),
}
for n, img in items.items():
    save(weave(img, 0.05), "scenery", n)
    atlas["scenery"][n] = dict(size=list(img.size), file=f"scenery/{n}.png")
coin = sheet(sc.coin_frames())
save(coin, "scenery", "coin")
atlas["scenery"]["coin"] = dict(size=[16, 16], frame_count=4, file="scenery/coin.png", note="spinning florin")

# parallax backgrounds (480x270, weave baked in)
for n, fn in {"sky": sc.bg_sky, "far": sc.bg_far, "near": sc.bg_near}.items():
    img = weave(fn(), 0.06)
    save(img, "backgrounds", n, stitched=False)
    atlas["backgrounds"][n] = dict(size=[sc.BW, sc.BH], file=f"backgrounds/{n}.png", tileable_x=(n != "sky"))

with open(os.path.join(ROOT, "atlas.json"), "w") as f:
    json.dump(atlas, f, indent=2)
print("done")
