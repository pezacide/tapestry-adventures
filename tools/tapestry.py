"""Shared helpers: pixel-map parsing, cross-stitch rendering, weave texture."""
from PIL import Image

def parse(rows, pal, w=None, h=None):
    h = h or len(rows)
    w = w or max(len(r) for r in rows)
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    px = img.load()
    for y, row in enumerate(rows[:h]):
        for x, ch in enumerate(row.ljust(w, ".")[:w]):
            if ch != "." and ch != " ":
                if ch not in pal:
                    raise KeyError(f"palette missing {ch!r} (row {y})")
                px[x, y] = pal[ch] + (255,)
    return img

def _shade(c, f):
    return tuple(max(0, min(255, int(v * f))) for v in c[:3]) + (c[3],)

def stitch(img, s=4):
    """Upscale so every pixel becomes a little woven cross-stitch."""
    w, h = img.size
    out = Image.new("RGBA", (w * s, h * s), (0, 0, 0, 0))
    src, dst = img.load(), out.load()
    for y in range(h):
        for x in range(w):
            c = src[x, y]
            if c[3] == 0:
                continue
            for j in range(s):
                for i in range(s):
                    if i == j:
                        f = 1.18          # '\' thread on top
                    elif i + j == s - 1:
                        f = 0.82          # '/' thread underneath
                    else:
                        f = 1.0
                    dst[x * s + i, y * s + j] = _shade(c, f)
    return out

def weave(img, strength=0.07, period=2):
    """Basket-weave brightness texture over an opaque-ish image (in place copy)."""
    img = img.copy()
    px = img.load()
    w, h = img.size
    for y in range(h):
        for x in range(w):
            c = px[x, y]
            if c[3] == 0:
                continue
            warp = ((x // period) + (y // period)) % 2 == 0
            f = 1 + strength if warp else 1 - strength
            if (x % period == 0) != (y % period == 0):
                f *= 0.97
            px[x, y] = _shade(c, f)
    return img

def sheet(frames):
    w = sum(f.width for f in frames)
    h = max(f.height for f in frames)
    out = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    x = 0
    for f in frames:
        out.paste(f, (x, h - f.height))
        x += f.width
    return out

def flip(img):
    return img.transpose(Image.FLIP_LEFT_RIGHT)
