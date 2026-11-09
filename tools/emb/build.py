"""Render every embroidered asset into ../../assets.  Run: python3 tools/emb/build.py  (needs Pillow + Node Playwright)."""
import json, os, subprocess, sys, tempfile
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
from PIL import Image
import knights, creatures, scenery

ROOT = os.path.normpath(os.path.join(HERE, "..", "..", "assets"))
SCALE = 2   # every PNG is rendered at 2x its design size; draw at 0.5 in a 960x540 game
TMP = tempfile.mkdtemp()
jobs, sheets, atlas = [], {}, {"scale": SCALE, "note": "sizes are design units; PNG pixels = size x scale",
                               "heroes": {}, "enemies": {}, "scenery": {}, "backgrounds": {}}

def job(svg, key, w, h):
    p = os.path.join(TMP, key.replace("/", "_"))
    open(p + ".svg", "w").write(svg)
    jobs.append(dict(svg=p + ".svg", png=p + ".png", w=w, h=h, scale=SCALE))
    return p + ".png"

for k, cfg in knights.KNIGHTS.items():
    sheets[f"heroes/{k}"] = [job(knights.knight(cfg, p), f"{k}_{p}", knights.W, knights.H) for p in knights.POSES]
    atlas["heroes"][k] = dict(title=cfg["title"], trait=cfg["trait"], weapon=cfg["weapon"], frame=[knights.W, knights.H], frames=knights.POSES, file=f"heroes/{k}.png")

for n, c in creatures.C.items():
    w, h = c["size"]
    sheets[f"enemies/{n}"] = [job(creatures.build_svg(n, f), f"{n}_{f}", w, h) for f in range(c["frames"])]
    atlas["enemies"][n] = dict(title=c["title"], behaviour=c["behaviour"], frame=[w, h], frame_count=c["frames"], file=f"enemies/{n}.png")

for n, (w, h, fn, bg) in scenery.S.items():
    if n.startswith("bezant_"):
        continue
    folder = "backgrounds" if n.startswith("bg_") else "scenery"
    name = n[3:] if n.startswith("bg_") else n
    sheets[f"{folder}/{name}"] = [job(scenery.build_svg(n), n, w, h)]
    atlas[folder][name] = dict(size=[w, h], file=f"{folder}/{name}.png")
sheets["scenery/bezant"] = [job(scenery.build_svg(f"bezant_{i}"), f"bezant_{i}", 24, 24) for i in range(4)]
atlas["scenery"]["bezant"] = dict(size=[24, 24], frame_count=4, file="scenery/bezant.png", note="spinning gold bezant (the coin)")
for n in ("far", "near"):
    atlas["backgrounds"][n]["tileable_x"] = True

json.dump(jobs, open(os.path.join(TMP, "m.json"), "w"))
subprocess.run(["node", os.path.join(HERE, "render.js"), os.path.join(TMP, "m.json")], check=True)

for key, files in sheets.items():
    os.makedirs(os.path.join(ROOT, os.path.dirname(key)), exist_ok=True)
    ims = [Image.open(f).convert("RGBA") for f in files]
    out = Image.new("RGBA", (sum(i.width for i in ims), max(i.height for i in ims)))
    x = 0
    for im in ims:
        out.paste(im, (x, 0)); x += im.width
    out.save(os.path.join(ROOT, key + ".png"), optimize=True)
json.dump(atlas, open(os.path.join(ROOT, "atlas.json"), "w"), indent=2)
print("built", len(jobs), "frames into", ROOT)
