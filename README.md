# Tapestry Adventures

A Mario-Bros-style platformer stitched like the Bayeux Tapestry: Norman knights against the weird beasts of medieval bestiaries.

## Art assets
Open `assets/gallery.html`, or look at `assets/showcase.png`.

The art imitates Bayeux wool embroidery: laid-and-couched fills, stem-stitch outlines, a plain linen ground, and only the tapestry's dye colours (terracotta, ochre, buff, sage, dark green, indigo, blue-grey).

| Folder | Contents |
|---|---|
| `assets/heroes/` | 4 knights in nasal helmets, mail and kite shields; frames idle, walk1, walk2, jump, ride. **Sir Rubeus** (spear, cross shield), **Dame Azura** (sword, wyrm shield), **Sir Verdain** (axe, banded shield), **Lady Aurelia** (mace, bezant shield). Each also has a `_mounted` sheet riding Bayard |
| `assets/mounts/` | **Bayard, the magic horse**: the Yoshi-style mount. Frames idle, walk1, walk2, leap, rear. A hit throws the rider; Bayard rears and bolts. The atlas holds the saddle point and rider offset |
| `assets/princess/` | **Princess Aelfgyva** (the mysterious lady of the Bayeux Tapestry): idle, wave, joy |
| `assets/enemies/` | **Snail-Hound** (walk x2 + shell), **Man-Faced Owl**, **Soup Cat**, **Spotted Unicorn**, **Wyvern**, **Killer Hare**, **Bayeux Hound**, **Fiery Lion** (walk x2 + roar), **Camel-Dragon** (two-faced, with a head on its tail) |
| `assets/bosses/` | **Aspidochelone the Island-Whale** (sea), **The Cockatrice** (petrifying gaze), **The Great Dragon** (fire breath), **The Cat King** (final boss, on his throne with a ladle-sceptre) |
| `assets/projectiles/` | fireball, soup glob |
| `assets/scenery/` | 48px tiles: turf, earth, masonry, roundel block (the `?` block) and used block, sea. Also bezant coin (4 frames), tower (the "pipe"), plank, longship, gonfanon (goal banner), Bayeux tree, shrub, hill, hall, castle on a motte, a beast-and-bird border strip, and Latin inscriptions |
| `assets/backgrounds/` | 960x540 layers: linen, far (outline-only stitched "underdrawing"), near (trees, shrubs, hills); far and near tile horizontally |
| `assets/atlas.json` | frame sizes, frame counts and intended behaviours |

Each PNG is rendered at **2x** its design size (see `scale` in the atlas). It is meant to be drawn at half size in a 960x540 game. Sprite sheets lay their frames out left to right, and sprites face right.

## The game (Unity)
The playable game is in [`unity/`](unity/README.md): open that folder in Unity 6 and press Play. That README has the setup steps, controls and build notes. `assets/level1_preview.png` and `assets/level2_preview.png` show whole levels.

## Regenerating
All the art is code: `tools/emb/` holds SVG drawings plus an embroidery filter, and Chromium renders them.

```
python3 tools/emb/build.py      # renders all assets (Pillow + Node Playwright)
python3 tools/emb/showcase.py   # composes assets/showcase.png and showcase_cast.png
python3 tools/emb/gallery.py    # writes assets/gallery.html
python3 tools/unity/export_art.py    # copies the art into the Unity project + ArtCatalog.cs
python3 tools/unity/make_levels.py   # regenerates every level's map
python3 tools/unity/preview_level.py # renders assets/levelN_preview.png
python3 tools/audio/make_audio.py    # composes the music + sound effects (numpy, ffmpeg)
```

- To recolour a knight, edit the `KNIGHTS` table in `knights.py`.
- To add a beast, add a function to `creatures.py`.
