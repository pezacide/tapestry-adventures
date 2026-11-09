"""Write assets/gallery.html from assets/atlas.json."""
import json, os
R = os.path.normpath(os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..", "assets"))
a = json.load(open(os.path.join(R, "atlas.json")))
h = ['<!doctype html><meta charset=utf-8><meta name=viewport content="width=device-width,initial-scale=1"><title>Tapestry Adventures - Asset Gallery</title>',
     '<style>body{background:#ece2c9;font-family:Georgia,serif;color:#33251d;margin:0;padding:16px}h1,h2{border-bottom:3px double #a8432f;font-variant:small-caps;letter-spacing:.05em}'
     '.g{display:flex;flex-wrap:wrap;gap:14px}.c{background:#f2e8cf;border:2px solid #33251d;padding:10px;max-width:100%;box-sizing:border-box}.c img{display:block;max-width:100%;height:auto}'
     'small{display:block;max-width:520px;margin-top:6px}img.hero{width:100%;max-width:1440px;border:3px solid #33251d;margin-bottom:10px}</style>',
     '<h1>Tapestry Adventures &middot; Asset Gallery</h1><img class=hero src=showcase.png alt="Level scene"><img class=hero src=showcase_cast.png alt="Cast scene">']
def card(t, f, s, w): return f'<div class=c><b>{t}</b><img src="{f}" style="width:{int(min(w, 1000))}px"><small>{s}</small></div>'
def frames_w(v): return v["frame"][0] * len(v.get("frames", [None] * v.get("frame_count", 1))) * 1.5
for cat, label in (("heroes", "Heroes"), ("mounts", "Mount"), ("princess", "Princess"), ("enemies", "Antagonists"), ("bosses", "Bosses"), ("projectiles", "Projectiles")):
    h.append(f"<h2>{label}</h2><div class=g>")
    for v in a[cat].values():
        desc = v.get("trait") or v.get("behaviour") or v.get("note", "")
        if "frames" in v: desc += f" &middot; frames: {', '.join(v['frames'])}"
        h.append(card(v["title"], v["file"], desc, frames_w(v)))
        if "mounted" in v:
            m = v["mounted"]; h.append(card(v["title"] + " on Bayard", m["file"], "frames: " + ", ".join(m["frames"]), m["frame"][0] * 5 * 1.2))
    h.append("</div>")
h.append("<h2>Scenery</h2><div class=g>")
for k, v in a["scenery"].items():
    h.append(card(k, v["file"], f"{v['size'][0]}x{v['size'][1]}" + (" &middot; " + v["note"] if "note" in v else ""), v["size"][0] * (4 if k == "bezant" else 1) * 1.5))
h.append("</div><h2>Background layers</h2><div class=g>")
for k, v in a["backgrounds"].items():
    h.append(card(k, v["file"], "960x540" + (", tiles horizontally" if v.get("tileable_x") else ""), 480))
h.append("</div>")
open(os.path.join(R, "gallery.html"), "w").write("\n".join(h))
print("gallery ok")
