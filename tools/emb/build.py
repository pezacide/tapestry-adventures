"""Render every embroidered asset into ../../assets.  Run: python3 tools/emb/build.py  (needs Pillow + Node Playwright)."""
import json, os, subprocess, sys, tempfile
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
from PIL import Image
import knights, creatures, scenery, bosses, allies, items, castles, ui, world4, world5  # castles, ui, world4 and world5 add themselves
from embroider import doc, g

ROOT = os.path.normpath(os.path.join(HERE, "..", "..", "assets"))
SCALE = 2   # every PNG is rendered at 2x its design size; draw at 0.5 in a 960x540 game
TMP = tempfile.mkdtemp()
jobs, sheets, atlas = [], {}, {"scale": SCALE, "note": "sizes are design units; PNG pixels = size x scale",
                               "heroes": {}, "mounts": {}, "princess": {}, "enemies": {}, "bosses": {}, "projectiles": {}, "items": {}, "scenery": {}, "backgrounds": {}}

def job(svg, key, w, h):
    p = os.path.join(TMP, key.replace("/", "_"))
    open(p + ".svg", "w").write(svg)
    jobs.append(dict(svg=p + ".svg", png=p + ".png", w=w, h=h, scale=SCALE))
    return p + ".png"

for k, cfg in knights.KNIGHTS.items():
    sheets[f"heroes/{k}"] = [job(knights.knight(cfg, p), f"{k}_{p}", knights.W, knights.H) for p in knights.POSES]
    atlas["heroes"][k] = dict(title=cfg["title"], trait=cfg["trait"], weapon=cfg["weapon"], frame=[knights.W, knights.H], frames=knights.POSES, file=f"heroes/{k}.png")
    mt = allies.mounted(knights.knight_body(cfg, "ride"))
    sheets[f"heroes/{k}_mounted"] = [job(mt[p], f"{k}_m_{p}", allies.HW, allies.HH + allies.MOUNT_PAD) for p in allies.POSES]
    atlas["heroes"][k]["mounted"] = dict(file=f"heroes/{k}_mounted.png", frame=[allies.HW, allies.HH + allies.MOUNT_PAD], frames=allies.POSES)
    um = allies.mounted(knights.knight_body(cfg, "ride"), lambda p, rider: world5.unicorn_steed(p, rider))
    sheets[f"heroes/{k}_unicorn"] = [job(um[p], f"{k}_u_{p}", allies.HW, allies.HH + allies.MOUNT_PAD) for p in allies.POSES]
    atlas["heroes"][k]["unicorn"] = dict(file=f"heroes/{k}_unicorn.png", frame=[allies.HW, allies.HH + allies.MOUNT_PAD], frames=allies.POSES)

sheets["mounts/bayard"] = [job(doc(allies.HW, allies.HH, allies.horse(p)), f"bayard_{p}", allies.HW, allies.HH) for p in allies.POSES]
atlas["mounts"]["bayard"] = dict(title="Bayard, the magic horse", frame=[allies.HW, allies.HH], frames=allies.POSES, file="mounts/bayard.png",
    saddle=list(allies.SEAT), rider_offset=[allies.SEAT[0] - allies.RIDER_HIP[0], allies.SEAT[1] - allies.RIDER_HIP[1]],
    behaviour="Yoshi-style mount: found in roundel blocks; ride to run faster, jump higher and trample fiery beasts. Take a hit while riding and you are "
              "thrown off - Bayard rears and bolts, and you can catch him again. Press down to dismount. Use a knight's _mounted sheet, "
              "or draw the knight 'ride' frame at rider_offset.")
sheets["mounts/unicorn"] = [job(doc(allies.HW, allies.HH + world5.UPAD, g(world5.unicorn_steed(p), f"translate(0 {world5.UPAD})")), f"unicorn_m_{p}", allies.HW, allies.HH + world5.UPAD) for p in allies.POSES]
atlas["mounts"]["unicorn"] = dict(title="Ye Spotted Unicorne, spared", frame=[allies.HW, allies.HH + world5.UPAD], frames=allies.POSES, file="mounts/unicorn.png",
    behaviour="the second mount (won in 5-4): faster than Bayard, and its horn fells beasts it runs into at a gallop; no flutter-leap")
sheets["projectiles/spear"] = [job(items.build_svg(items.SPEAR, 0), f"spear_{i}", 48, 10) for i in range(2)]
atlas["projectiles"]["spear"] = dict(title="Huntsman's Speare", behaviour="hurled by huntsmen in a long arc", frame=[48, 10], frame_count=2, file="projectiles/spear.png")
sheets["princess/aelfgyva"] = [job(doc(allies.PW, allies.PH, allies.princess(p)), f"aelf_{p}", allies.PW, allies.PH) for p in allies.P_POSES]
atlas["princess"]["aelfgyva"] = dict(title="Princess Aelfgyva", frame=[allies.PW, allies.PH], frames=allies.P_POSES, file="princess/aelfgyva.png",
    note="Named after the mysterious lady in the Bayeux Tapestry. Held captive behind the Cat King's throne; idle, wave, joy (rescued).")
for n, it in items.I.items():
    w, h = it["size"]
    sheets[f"items/{n}"] = [job(items.build_svg(it, f), f"item_{n}_{f}", w, h) for f in range(it["frames"])]
    atlas["items"][n] = dict(title=it["title"], behaviour=it["effect"], frame=[w, h], frame_count=it["frames"], file=f"items/{n}.png")
sheets["projectiles/gaze"] = [job(items.build_svg(items.GAZE, f), f"gaze_{f}", 40, 12) for f in range(2)]
atlas["projectiles"]["gaze"] = dict(title="Petrifying Gaze", behaviour="the Cockatrice's glare: turns a knight to stone for a moment", frame=[40, 12], frame_count=2, file="projectiles/gaze.png")
sheets["projectiles/bolt"] = [job(items.build_svg(items.BOLT, 0), "bolt", 24, 8)]
atlas["projectiles"]["bolt"] = dict(title="Crossbowe Bolt", behaviour="shot by a knight who carries the crossbowe", frame=[24, 8], frame_count=1, file="projectiles/bolt.png")
sheets["allies/monk"] = [job(doc(allies.MW, allies.MH, allies.monk(p)), f"monk_{p}", allies.MW, allies.MH) for p in allies.M_POSES]
atlas["princess"]["monk"] = dict(title="Brother Odo, the rescued monk", frame=[allies.MW, allies.MH], frames=allies.M_POSES, file="allies/monk.png",
    note="waits at the end of each castle to tell ye knyghte that ye Princesse is in another castel")
for n, bb in bosses.B.items():
    w, h = bb["size"]
    sheets[f"bosses/{n}"] = [job(bosses.build_svg(n, f), f"{n}_{f}", w, h) for f in range(bb["frames"])]
    atlas["bosses"][n] = dict(title=bb["title"], behaviour=bb["behaviour"], frame=[w, h], frame_count=bb["frames"], file=f"bosses/{n}.png")

for n, c in creatures.C.items():
    w, h = c["size"]
    cat = "bosses" if n.endswith("_boss") else "projectiles" if n in ("fireball", "soup_glob") else "enemies"
    sheets[f"{cat}/{n}"] = [job(creatures.build_svg(n, f), f"{n}_{f}", w, h) for f in range(c["frames"])]
    atlas[cat][n] = dict(title=c["title"], behaviour=c["behaviour"], frame=[w, h], frame_count=c["frames"], file=f"{cat}/{n}.png")

for n, (w, h, fn, bg) in scenery.S.items():
    if n.startswith("bezant_") or n.startswith("checkpoint_") or n.startswith("fire_"):
        continue
    folder = "backgrounds" if n.startswith("bg_") else "scenery"
    name = n[3:] if n.startswith("bg_") else n
    sheets[f"{folder}/{name}"] = [job(scenery.build_svg(n), n, w, h)]
    atlas[folder][name] = dict(size=[w, h], file=f"{folder}/{name}.png")
sheets["scenery/bezant"] = [job(scenery.build_svg(f"bezant_{i}"), f"bezant_{i}", 24, 24) for i in range(4)]
sheets["scenery/fire"] = [job(scenery.build_svg(f"fire_{i}"), f"fire_{i}", 48, 48) for i in range(2)]
atlas["scenery"]["fire"] = dict(size=[48, 48], frame_count=2, file="scenery/fire.png", note="burning pit (kills), flickers between two frames")
sheets["scenery/checkpoint"] = [job(scenery.build_svg(f"checkpoint_{i}"), f"checkpoint_{i}", 44, 96) for i in range(2)]
atlas["scenery"]["checkpoint"] = dict(size=[44, 96], frame_count=2, file="scenery/checkpoint.png", note="halfway banner: unclaimed, claimed")
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
