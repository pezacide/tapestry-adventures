# Tapestry Adventures

A Mario-Bros-style platformer where the world is woven wool: heraldic knights vs. weird medieval-marginalia beasts.

## Art assets (done)
Open `assets/gallery.html` for a full preview, or `assets/showcase.png`.

| Folder | Contents |
|---|---|
| `assets/heroes/` | 4 knights, 20x24 frames: idle, walk1, walk2, jump (Rubeus/Lion, Azura/Swan, Verdain/Stag, Aurelia/Sun) |
| `assets/enemies/` | Snail Knight, Bishop Fox, Hare Jouster, Winged Boar, Thistle Hedgehog, Wyrm boss |
| `assets/scenery/` | 16px tiles (turf, earth, brick, platform, heraldic `?`-block, used block), spinning florin, tower "pipe", tree, bush, flowers, cloud, goal banner, tapestry border |
| `assets/backgrounds/` | 480x270 parallax layers: sky, far (hills + castles), near (mille-fleur meadow); far/near tile horizontally |
| `assets/atlas.json` | frame sizes, frame counts and intended enemy behaviours |

`*.png` are 1x game sprites (draw with `image-rendering: pixelated`); `*@4x.png` are cross-stitch-rendered previews.
Sprites face right; flip for left. Regenerate everything with `python3 tools/build_assets.py && python3 tools/showcase.py` (needs Pillow).
