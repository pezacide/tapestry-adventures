"""The app icon: Sir Rubeus's helm and shield on linen inside a Bayeux border. Run: python3 tools/emb/icon.py"""
import json, os, subprocess, sys, tempfile
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
from embroider import fill, stem, dot, doc, g, WOOL

def icon():
    s = f'<rect width="256" height="256" fill="url(#linen)"/>'
    # border band with diagonal bars, as on the Tapestry
    s += fill("M6,6 L250,6 L250,250 L6,250 Z M26,26 L26,230 L230,230 L230,26 Z", "terra", 0, "dark", 2)
    for i in range(0, 256, 32):
        s += stem(f"M{i},6 L{i + 18},26 M{i},230 L{i + 18},250 M6,{i} L26,{i + 18} M230,{i} L250,{i + 18}", "gold", 3, False)
    # Sir Rubeus himself, standing in the frame
    import knights
    s += g(knights.knight_body(knights.KNIGHTS["rubeus"], "idle"), "translate(51 33) scale(2.0)")
    return s

if __name__ == "__main__":
    root = os.path.normpath(os.path.join(HERE, "..", ".."))
    out = os.path.join(root, "unity", "Assets", "Icons", "icon.png")
    os.makedirs(os.path.dirname(out), exist_ok=True)
    tmp = tempfile.mkdtemp()
    open(os.path.join(tmp, "icon.svg"), "w").write(doc(256, 256, icon(), seed=5))
    json.dump([dict(svg=os.path.join(tmp, "icon.svg"), png=out, w=256, h=256, scale=4)], open(os.path.join(tmp, "m.json"), "w"))
    subprocess.run(["node", os.path.join(HERE, "render.js"), os.path.join(tmp, "m.json")], check=True)
    import shutil; shutil.copyfile(out, os.path.join(root, "assets", "icon.png"))
    print("icon ->", out)
