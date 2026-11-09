"""Bayeux-style embroidery in SVG: laid-and-couched wool fills, stem-stitch outlines, linen ground."""
import random

# Wool palette, after the Bayeux Tapestry's eight-ish dyed wools + marginalia extras
WOOL = {
    "linen": "#ece2c9", "cream": "#f2e8cf", "buff": "#d1b58a", "ochre": "#c9993f", "gold": "#d7ad4f",
    "terra": "#a8432f", "rose": "#c77a62", "sage": "#8f9d5f", "green": "#56693f", "olive": "#6f6a35",
    "indigo": "#2f3b5c", "blue": "#4f6f96", "sky": "#90a9bd", "grey": "#8b8f8f", "brown": "#6b4a33",
    "dark": "#33251d", "white": "#f6efdc", "violet": "#6a4d6f",
}
OUTLINE = "dark"
ANGLES = (0, 30, 60, 90, 120, 150)

def _mix(hexc, f):
    h = hexc.lstrip("#"); r, g, b = (int(h[i:i + 2], 16) for i in (0, 2, 4))
    if f >= 1: r, g, b = (int(v + (255 - v) * (f - 1)) for v in (r, g, b))
    else: r, g, b = (int(v * f) for v in (r, g, b))
    return "#%02x%02x%02x" % (r, g, b)

def defs(seed=3):
    out = ["<defs>"]
    for name, c in WOOL.items():
        for a in ANGLES:   # laid threads (3 strands of 2 units) + one couching thread every 9 units
            out.append(
                f'<pattern id="w-{name}-{a}" width="9" height="6" patternUnits="userSpaceOnUse" patternTransform="rotate({a})">'
                f'<rect width="9" height="6" fill="{c}"/>'
                f'<rect y="2" width="9" height="2" fill="{_mix(c, 0.93)}"/>'
                f'<rect y="4" width="9" height="2" fill="{_mix(c, 1.05)}"/>'
                + "".join(f'<rect y="{y - 0.25}" width="9" height="0.5" fill="{_mix(c, 0.72)}"/>' for y in (0, 2, 4))
                + f'<rect x="4" width="0.9" height="6" fill="{_mix(c, 0.8)}"/>'
                f'<rect x="4.1" y="1.6" width="0.7" height="0.8" fill="{_mix(c, 1.15)}"/>'
                f'<rect x="4.1" y="4.6" width="0.7" height="0.8" fill="{_mix(c, 1.15)}"/>'
                "</pattern>")
        # mail: rings of thread on the base colour
        out.append(
            f'<pattern id="m-{name}" width="5" height="4.5" patternUnits="userSpaceOnUse">'
            f'<rect width="5" height="4.5" fill="{c}"/>'
            f'<circle cx="1.25" cy="1.1" r="1" fill="none" stroke="{_mix(c, 0.62)}" stroke-width="0.6"/>'
            f'<circle cx="3.75" cy="3.35" r="1" fill="none" stroke="{_mix(c, 0.62)}" stroke-width="0.6"/>'
            "</pattern>")
    # linen ground: plain weave
    out.append('<pattern id="linen" width="3" height="3" patternUnits="userSpaceOnUse">'
               f'<rect width="3" height="3" fill="{WOOL["linen"]}"/>'
               f'<rect width="3" height="0.6" fill="{_mix(WOOL["linen"], 0.95)}"/>'
               f'<rect width="0.6" height="3" fill="{_mix(WOOL["linen"], 0.96)}"/>'
               f'<rect x="1.5" y="1.5" width="0.6" height="0.6" fill="{_mix(WOOL["linen"], 1.04)}"/></pattern>')
    # wool filter: wobble the edges, add fibre shading
    out.append(f'''<filter id="wool" x="-5%" y="-5%" width="110%" height="110%" color-interpolation-filters="sRGB">
 <feTurbulence type="fractalNoise" baseFrequency="0.55" numOctaves="2" seed="{seed}" result="n"/>
 <feDisplacementMap in="SourceGraphic" in2="n" scale="1.6" xChannelSelector="R" yChannelSelector="G" result="d"/>
 <feTurbulence type="fractalNoise" baseFrequency="1.4 0.9" numOctaves="1" seed="{seed + 7}" result="f"/>
 <feColorMatrix in="f" type="saturate" values="0" result="fg"/>
 <feComponentTransfer in="fg" result="fs"><feFuncR type="linear" slope="0.45" intercept="0.72"/><feFuncG type="linear" slope="0.45" intercept="0.72"/><feFuncB type="linear" slope="0.45" intercept="0.72"/><feFuncA type="linear" slope="0" intercept="1"/></feComponentTransfer>
 <feBlend in="d" in2="fs" mode="multiply" result="m"/>
 <feComposite in="m" in2="d" operator="in"/>
</filter>''')
    out.append("</defs>")
    return "\n".join(out)

def fill(d, color, angle=0, outline=OUTLINE, ow=1.6, extra=""):
    """A laid-and-couched area outlined in stem stitch."""
    a = min(ANGLES, key=lambda x: abs(x - (angle % 180)))
    s = f'<path d="{d}" fill="url(#w-{color}-{a})" {extra}/>'
    if outline:
        s += stem(d, outline, ow)
    return s

def mail(d, color, outline=OUTLINE, ow=1.6):
    return f'<path d="{d}" fill="url(#m-{color})"/>' + (stem(d, outline, ow) if outline else "")

def stem(d, color=OUTLINE, w=1.6, closed=True):
    """Stem stitch: a dark thread with lighter overlapping stitches twisting along it."""
    c = WOOL.get(color, color)
    return (f'<path d="{d}" fill="none" stroke="{c}" stroke-width="{w}" stroke-linejoin="round" stroke-linecap="round"/>'
            f'<path d="{d}" fill="none" stroke="{_mix(c, 1.35)}" stroke-width="{w * 0.35:.2f}" stroke-dasharray="{w * 1.4:.2f} {w * 1.1:.2f}" stroke-linecap="round" opacity="0.8"/>')

def dot(x, y, r, color, outline=None):
    return fill(f"M{x - r},{y} a{r},{r} 0 1,0 {2 * r},0 a{r},{r} 0 1,0 {-2 * r},0 Z", color, 0, outline, 1)

def doc(w, h, body, bg=False, seed=3):
    """Wrap parts in an SVG document with the wool filter. bg=True paints linen behind."""
    ground = f'<rect width="{w}" height="{h}" fill="url(#linen)"/>' if bg else ""
    return (f'<svg xmlns="http://www.w3.org/2000/svg" width="{w}" height="{h}" viewBox="0 0 {w} {h}">'
            f"{defs(seed)}{ground}<g filter=\"url(#wool)\">{body}</g></svg>")

def g(body, tf=""):
    return f'<g transform="{tf}">{body}</g>' if tf else body

def rot(deg, cx, cy):
    return f"rotate({deg} {cx} {cy})"
