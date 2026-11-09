# Tapestry Adventures

A Mario-Bros-style platformer stitched like the Bayeux Tapestry: Norman knights against the weird beasts of medieval bestiaries.

## Art assets
Open `assets/gallery.html`, or look at `assets/showcase.png`.

The art imitates Bayeux wool embroidery: laid-and-couched fills, stem-stitch outlines, a plain linen ground, and only the tapestry's dye colours (terracotta, ochre, buff, sage, dark green, indigo, blue-grey).

| Folder | Contents |
|---|---|
| `assets/heroes/` | 4 knights in nasal helmets, mail and kite shields; frames idle, walk1, walk2, jump. **Sir Rubeus** (spear, cross shield), **Dame Azura** (sword, wyrm shield), **Sir Verdain** (axe, banded shield), **Lady Aurelia** (mace, bezant shield) |
| `assets/enemies/` | **Snail-Hound** (walk x2 + shell), **Man-Faced Owl** (perched, flying), **Soup Cat** (stir x2, fling), **Spotted Unicorn**, **Wyvern**, **Killer Hare**, **Bayeux Hound**, boss **Aspidochelone the Island-Whale** (closed / open jaws) |
| `assets/scenery/` | 48px tiles: turf, earth, masonry, roundel block (the `?` block) and used block, sea. Also bezant coin (4 frames), tower (the "pipe"), plank, longship, gonfanon (goal banner), Bayeux tree, shrub, hill, hall, castle on a motte, a beast-and-bird border strip, and Latin inscriptions |
| `assets/backgrounds/` | 960x540 layers: linen, far (outline-only stitched "underdrawing"), near (trees, shrubs, hills); far and near tile horizontally |
| `assets/atlas.json` | frame sizes, frame counts and intended enemy behaviours |

Each PNG is rendered at **2x** its design size (see `scale` in the atlas). It is meant to be drawn at half size in a 960x540 game. Sprite sheets lay their frames out left to right, and sprites face right.

## Regenerating
All the art is code: `tools/emb/` holds SVG drawings plus an embroidery filter, and Chromium renders them.

```
python3 tools/emb/build.py      # renders all assets (Pillow + Node Playwright)
python3 tools/emb/showcase.py   # composes assets/showcase.png
```

- To recolour a knight, edit the `KNIGHTS` table in `knights.py`.
- To add a beast, add a function to `creatures.py`.
