"""Render whole levels from the real art -> assets/levelN_preview.png (for checking the layout).
Usage: python3 tools/unity/preview_level.py [N ...]   (default: every level)"""
import os, re, sys, glob
from PIL import Image
ROOT = os.path.normpath(os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", ".."))
A = os.path.join(ROOT, "assets")
def render(n):
  src = open(os.path.join(ROOT, f"unity/Assets/Scripts/Levels/Level{n}.cs")).read()
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
  near = re.search(r'NearLayer = "([^"]+)"', src)
  far = (re.search(r'FarLayer = "([^"]+)"', src) or re.search(r"(backgrounds/far)", "backgrounds/far")).group(1)
  for x0 in range(0, W * T, 960): out.alpha_composite(load(far), (x0, H * T - 540))
  for y in range(H):
      for x in range(W):
          if get(x, y) == "|": put(load("scenery/trunk"), x + 0.5, y)
  for key, x in props:
      x = float(x); gy = next((y + 1 for y in range(H - 1, -1, -1) if get(int(x), y) in "#BXHT?t1MWRV" and not (get(int(x), y) == "V" and y >= 9)), 0)
      put(load(key), x, gy)
  sk = lambda k, d: (re.search(k + r' = "([^"]+)"', src) or re.search("(" + d + ")", d)).group(1)
  brick, gtop, gfill = sk("Brick", "scenery/masonry"), sk("GroundTop", "scenery/ground_top"), sk("GroundFill", "scenery/ground_fill")
  tiles = {c: brick for c in "B?H1MWR"}; tiles["~"] = "scenery/sea"; tiles["V"] = sk("Vault", "scenery/vault"); tiles["F"] = tiles["V"]
  plank = sk("Plank", "scenery/plank"); liftk = sk("LiftSkin", "scenery/lift")
  done = set()
  for y in range(H):
      for x in range(W):
          c = get(x, y)
          if c == "#": put(load(gfill if get(x, y + 1) == "#" else gtop), x + 0.5, y)
          elif c in tiles: put(load(tiles[c]), x + 0.5, y)
          elif c == "=": pl = load(plank); put(pl, x + 0.5, y + 1 - pl.height / 48)
          elif c == "^": put(load("scenery/canopy"), x + 0.5, y + 1 - 26 / 48)
          elif c == "*": put(load("scenery/fire", (x + y) % 2, 2), x + 0.5, y)
          elif c == "_": put(load("scenery/drawbridge"), x + 0.5, y + 1 - 26 / 48)
          elif c in "LE": put(load(liftk), x + 1.5, y + 1 - 14 / 48)
          elif c in "YU": put(load("scenery/longship"), x + 2.7, y + 1 - 52 / 48)
          elif c in "Tt" and (x, y) not in done:
              done.add((x, y)); done.add((x + 1, y)); put(load("scenery/tower_top" if c == "T" else "scenery/tower_body"), x + 1, y)
  spr = {"s": ("enemies/snail_hound", 3), "S": ("enemies/snail_moss", 3), "g": ("enemies/gull", 2), "x": ("enemies/crab", 2), "f": ("enemies/fish", 2), "z": ("enemies/sea_serpent", 2), "m": ("enemies/sea_monk", 2), "Q": ("scenery/log_pile", 1), "b": ("enemies/bird", 2), "<": ("scenery/wind_head", 1), ">": ("scenery/wind_head", 1), "h": ("enemies/killer_hare", 2), "l": ("enemies/fiery_lion", 3), "d": ("enemies/bayeux_hound", 2),
         "c": ("enemies/camel_dragon", 2), "w": ("enemies/wyvern", 2), "u": ("enemies/unicorn", 2), "O": ("enemies/owl_face", 2), "k": ("enemies/soup_cat", 3)}
  for y in range(H):
      for x in range(W):
          c = get(x, y)
          if c in spr: put(load(spr[c][0], 0, spr[c][1]), x + 0.5, y)
          elif c == "o": put(load("scenery/bezant", 0, 4), x + 0.5, y + 0.25)
          elif c == "G": put(load("scenery/goal_pole"), x + 0.5, y)
          elif c == "C": put(load("scenery/checkpoint", 0, 2), x + 0.5, y)
          elif c == "K": put(load("bosses/cockatrice", 0, 2), x + 0.5, y)
          elif c == "J": put(load("bosses/whale_boss", 0, 2), x + 0.5, 0.55)
          elif c == "A": put(load("scenery/bridge_axe"), x + 0.5, y)
          elif c == "N": put(load("allies/monk", 0, 2), x + 0.5, y)
          elif c == "F":
              b = load("scenery/flail_ball")
              for k in range(1, 5): put(b, x + 0.5 + k * 0.45, y + 0.3)
          elif c == "P": put(load("heroes/rubeus", 0, 5), x + 0.5, y)
  cm = re.search(r'Castle = "([^"]+)"', src)
  if cm: put(load(cm.group(1)), float(re.search(r"CastleX = ([0-9.]+)f", src).group(1)), 2)
  out.convert("RGB").save(os.path.join(A, f"level{n}_preview.png"), optimize=True)
  print(out.size)

nums = sys.argv[1:] or sorted(re.findall(r"Level(\d+[a-z]?)\.cs", " ".join(os.listdir(os.path.join(ROOT, "unity/Assets/Scripts/Levels")))))
for n in nums: render(n)
