# Working title: *Dream Palette*

*A Super Mario Bros. 2 (USA) style platformer set inside the paintings of Monet, Cézanne, Van Gogh and friends.*

Status: **planning only**. Nothing here is built. This is a separate game from the tapestry game in the rest of this repo; it lives in `docs/painters/` until we decide where it goes (see Open questions).

Title options: *Dream Palette* · *Pull the Paint* · *Gallery of Dreams*.

---

## 1. Pitch

You fall asleep on a bench in a museum at closing time. In the dream, **Baron Grisaille**, a restorer who hates colour, is stripping every painting back to grey. You and three other painters (each a famous hand with a distinct technique) walk **into the paintings**, pull pigment out of the canvas, throw it at the Baron's grey army, and put the colour back.

It keeps SMB2's odd, brilliant core: **you don't stomp, you pick things up and throw them**, four characters play differently, there is a secret "sub-space", and the whole thing was a dream.

## 2. Pillars

1. **Pull, carry, throw.** Everything is a verb on objects: pull paint from the canvas, lift enemies, throw blocks. No stomp kills (you can ride and carry enemies).
2. **Each world is a painter's *technique*, not just a skin.** Monet = light and fog. Cézanne = planes and weight. Van Gogh = flowing brushstroke currents. If the mechanic wouldn't exist without that painter, it's right.
3. **Style never costs readability.** Backgrounds can be blurry, dotted or swirling. Anything you can stand on, grab or get hurt by keeps a firm contour and strong value contrast.

## 3. What we keep from SMB2, and what we change

| SMB2 | Ours |
|---|---|
| Mario, Luigi, Toad, Peach | Four painters: Cézanne, Monet, Van Gogh, Seurat (section 4) |
| Pull vegetables from the ground | Pull **daubs** (blobs of pigment) from the canvas; in each world they look like that painter's still-life objects (water-lily buds, apples, sunflowers, dots) |
| Pick up and throw enemies, blocks, bombs | Same. Bombs are **oily rags** (they really do self-combust). Mushroom blocks are **stretcher blocks** (canvas on a wooden frame) |
| Magic potion opens Subspace | **Turpentine flask** thrown on the ground dissolves the paint to the **Underpainting** (section 6) |
| Hearts (2 to 4) | **Palette wells** (2 to 4 blobs of paint on a palette) |
| 5 cherries = Starman | 5 **gold leaf flakes** = **Gilded** (invincible; Klimt-style shimmer) |
| Pots/jars | **Paint pots** (also lead to sub-areas) |
| Keys, locked doors, Phanto | **Gallery keys**, gilded doors, and the **Phantom Frame**: an empty gilt picture-frame with a stare that chases you after you take the key |
| Slot machine bonus | **Mixing-bowl** reels: match three tubes for extra lives |
| Birdo/Mouser style bosses (catch the projectile, throw it back) | Same loop; each world's boss is a figure from that painter's famous work, gone grey |
| Wart (hates vegetables) | **Baron Grisaille** (hates colour); hit him by feeding pigment into his open mouth |
| "It was all a dream" | Same ending, with one small difference in a painting (section 10) |
| 20 levels, 7 worlds | 20 levels, 7 worlds (6 painters, then the Baron) |

## 4. The four painters (playable)

Mapped one-to-one onto SMB2's four, so we inherit its proven asymmetry. Choose before each level; swap on death.

| Painter | SMB2 analogue | Feel | Pull speed | Run | Jump | Special |
|---|---|---|---|---|---|---|
| **Cézanne** | Mario | Solid, constructive. The "default". | medium | medium | medium | **Straight throws**: flat, accurate arc; thrown blocks land square |
| **Monet** | Peach | Soft, dreamy, drifting | slow | medium | medium | **Haze float**: hold jump to drift for ~1.5s |
| **Van Gogh** | Luigi | Intense, wobbly, high | slow | slippery | **highest** | **Flutter-kick** with a spiral brushstroke trail that eases your fall; strongest crouch-charge jump |
| **Seurat** | Toad | Small, precise, quick | **fastest** | fastest, even while carrying | lowest | **Quick-pull**: pulls and throws before enemies react; carries heavy items at full speed |

Rules of the cast:
- Secrets and routes are tuned so *each* painter reaches something others can't (Monet crosses a wide gap, Van Gogh reaches a high rope, Seurat outruns a collapsing bridge while carrying a key, Cézanne is the only one who can land a thrown block exactly on a switch).
- **Style clash is deliberate.** A painter standing in someone else's world is drawn in their own style (Cézanne in Hokusai's flat woodblock world looks faceted and out of place). The tapestry game already uses this idea (embroidered knights in woven worlds). Their outline stays strong so they always read.
- Respectful portrayal: stylised, friendly, no references to illness or injury (no ear jokes).

## 5. Core moves and rules

- **Pull** a daub from the ground (hold button, brief struggle). Sizes: small, large (slower), plus rare **big sprouts** (a whole haystack, a giant apple) that are heavy and break blocks.
- **Throw** with an arc; **crouch+throw** drops it gently. Thrown daubs splat on contact, leaving a coloured stain for a moment (decorative, and usable as a clue in puzzles).
- **Carry anything**: enemies (stunned by being lifted), stretcher blocks, rags, keys. Carrying slows you except for Seurat.
- **Stand on enemies** and ride them; jumping on them doesn't hurt them (SMB2 rule). Exception: fixed hazards (spiked wire-brush, flames).
- **Crouch-charge jump**: crouch until you flash, then jump higher.
- **Dig** by crouching on sandy/ochre earth to tunnel down.
- **Ladders** (painters' studio ladders) and **chains** for climbing.
- No timer. Lives are **canvases**; continues return you to the last world start.

## 6. The Underpainting (our Subspace)

Throw a turpentine flask on open ground: the paint **runs and dissolves** and a door appears. Step in and you're in the **Underpainting**, the artist's original sketch under the picture:

- Rendered as graphite and sepia line drawing with one spot colour. It's literally the "far, outline-only underdrawing" idea.
- Contains coins (**pigment drops**), extra palette wells (health upgrades), and sometimes the hidden gold-leaf flakes.
- Limited time: the solvent evaporates (a visible meter) and you're pushed back out.
- **Pentimenti**: ghosts of earlier compositions revealed in the sketch (a real art-history thing: painters painting over earlier versions). Each world has one hidden pentimento to find, which unlocks a Gallery extra (concept art, a painter bio card).
- Hidden-level "mushrooms" are replaced by the extra wells; there are ~3 per world.

## 7. Enemies and items

Generic cast (appear throughout, recoloured per world), mapped to SMB2 archetypes:

| Ours | SMB2 analogue | Behaviour |
|---|---|---|
| **Lay Figure** (wooden artist's mannequin, blank face) | Shy Guy | Walks; can be lifted/thrown. Colour variants: faster, sometimes turns at ledges |
| **Fixative Sprayer** (a mannequin with a mouth-atomiser) | Snifit | Stops and sprays a short arc of fixative (sticky slow patch) |
| **Oily Rag** | Bob-omb | Walks, then flashes and bursts; thrown, it breaks blocks |
| **Wire Brush** | Porcupo | Spiky, can't be lifted from the top; can be thrown from the side if you pull it by the handle |
| **Hog-hair Bristle-Bug** | Ninji | Hops randomly |
| **Phantom Frame** | Phanto | Appears when you take a key; chases; vanishes when the key is placed |
| **Flying Canvas** | Pidgit on carpet | A mannequin on a flying canvas; knock it off and ride the canvas |
| **Sunflower / Cadmium Bloom** | Panser | Stationary, fires pollen/petals |
| **Crow** | Albatoss | Flies overhead, drops oily rags |
| **Koi / Carp** | Trouter | Leaps from water |

Items: daubs, stretcher blocks, oily rags, turpentine flasks, gallery keys, gold leaf (x5 = Gilded), palette wells (+1 health), mixing-bowl reels, "Gesso Burst" block (clears all small enemies on screen, like POW).

## 8. Worlds and levels (20 levels: W1-W6 x3, W7 x2)

Order loosely mirrors SMB2's biomes: grass, desert, water and ice, night, circus/wacky, dream-horror, then the villain's lair.

| W | Painter | Biome | Signature mechanic | Levels | Boss (SMB2-style) |
|---|---|---|---|---|---|
| 1 | **Monet** | Giverny gardens | **Atmosphere**: distant things are blurry and resolve as you get close; bobbing lily pads; light that changes during a level | 1-1 *The Lily Pond* (footbridge, sinking pads, koi; teaches pull/throw); 1-2 *Haystacks at All Hours* (the Haystack series: dawn to night as you advance; giant haystack daubs); 1-3 *Rouen Cathedral* (vertical climb up a façade whose colours shift) | **Woman with a Parasol** spits droplets; catch and throw back |
| 2 | **Cézanne** | Provence, sun-baked | **Planes and weight**: faceted terraces, stacked still-life, digging | 2-1 *Mont Sainte-Victoire* (terraced hills, apple/pear daubs); 2-2 *Bibémus Quarry* (dig down through ochre layers); 2-3 *The Still-Life Table* (interior; the "table" tilts so jugs slide; platforms drawn from two viewpoints at once) | **The Card Player** tosses oily rags; throw back |
| 3 | **Hokusai** (ukiyo-e painter and printmaker) | Sea and snow | **Surf and slip**: ride wave crests as moving platforms; ice slopes | 3-1 *Beneath the Wave* (surf the swell); 3-2 *Fine Wind, Clear Morning* (red Fuji; snow, cranes); 3-3 *Smoke Dragon* | **Smoke Dragon of Fuji**, three heads that breathe fire; throw stretcher blocks (Tryclyde-style) |
| 4 | **Van Gogh** | Arles and Saint-Rémy, night | **Brushstroke currents**: swirls carry you and thrown items; impasto thick-paint platforms | 4-1 *Wheat Field with Crows* (crows drop rags); 4-2 *The Yellow Bedroom* (key + Phantom Frame level; deliberately skewed perspective; framed pictures on the wall); 4-3 *Starry Night* (climb the swirling sky) | **The Cypress** (a living green flame-tree) splits into flames when hit (Fryguy-style) |
| 5 | **Seurat** | Park, river, circus | **Optical mixing**: **mixing wells** where you throw two colours to make a third (blue+yellow = green bridge, red+yellow = orange fire) | 5-1 *A Sunday on La Grande Jatte* (park, dotted strollers); 5-2 *Bathers at Asnières* (rafts); 5-3 *Le Cirque* (carousels and conveyor belts) | **The Ringmaster** juggles huge dots (rock-thrower style) |
| 6 | **Bosch** | Dream and nightmare | **Bubbles and bestiary**: glass globes you float in, impossible creatures | 6-1 *The Garden* (pastel Eden panel); 6-2 *Earthly Delights* (giant fruit, bubbles as platforms); 6-3 *Music Hell* (bridges made of giant lutes and harps) | **The Tree-Man** (hollow torso, a tavern inside) |
| 7 | **Baron Grisaille** | The Restoration Studio | **Repaint**: you restore colour to grey replays of earlier worlds | 7-1 *The Grey Gallery* (greyscale callbacks; throw pigment to recolour and open the path); 7-2 *The Solvent Machine* | **Baron Grisaille** (section 9) |

Level sketch, **1-1 The Lily Pond** (side view, `~` water, `o` lily pad, `^` daub, `L` Lay Figure, `K` koi):

```
                          ___
                  o       |  |   o    o
 P    ^ ^     o   ~~~     |  |  ~~~~  ~~   ^  L        [door]
=====      ===========  ~~~~~~~ ===== ~~~~ =========  ===========
              ^ L          K                     K
```
Beats: pull, throw at a Lay Figure; ride one across a gap; first lily pad that sinks; a turpentine flask halfway; a hidden door to the Underpainting under the footbridge.

## 9. Final boss and ending

**Baron Grisaille's Solvent Machine** (the Dream Machine): it spits grey bubbles. Each bubble you pop releases a tube of pigment; **grab it and throw it into the Baron's open mouth**. Three hits, with the phases getting more colourful as he suffers. (He recoils from colour, like Wart and vegetables.) The machine keeps all four painters' toolsets relevant by what's near.

**Ending:** the Baron dissolves into colour, the grey gallery floods back to full colour. Cut to the museum: you wake on the bench, the guard says closing time. Credits roll over a slideshow of the paintings, and in each one there is a tiny new detail: one of the four painters, small, in a corner. Last frame: one painting with a coin-sized pentimento you remember finding.

## 10. Art direction

Each painter has a **style card**. The game is 2D side-scrolling; style is applied in layers.

| World | Palette (hex) | Rendering |
|---|---|---|
| Monet | lilac `#B7A5D6`, water green `#7FB5A4`, pond blue `#6C8FC8`, sunrise orange `#F08A4B`, rose `#E8A0B4`, cream `#F4EAD5` | Broken colour, soft edges and bloom on background layers; reflections |
| Cézanne | ochre `#C9A24B`, viridian `#3F7F6B`, ultramarine `#3B4F9A`, terracotta `#B5593C`, apple red `#B83A2E`, chalk `#E6DFC8` | Constructive parallel hatching; faceted planes; geometric tree canopies |
| Hokusai | Prussian blue `#1B3B6F`, indigo ink `#14213D`, wave white `#F2EFE4`, sand `#D9C79A`, vermilion `#D96B5A` | Flat colour, black key lines, woodblock grain |
| Van Gogh | cobalt `#2A4C9E`, chrome yellow `#F2C230`, cypress green `#2F5D3A`, night `#101B3D`, swirl pale `#A9CBE8`, orange `#E07B20` | Directional brushstroke flow, thick impasto highlights |
| Seurat | red `#E9724C`, yellow `#F2D24B`, blue `#5BA8C9`, green `#7AB367`, violet `#8E5AA8` on paper `#F5EFE0` | Dot stipple (dot size tied to zoom); figures simplified, frozen |
| Bosch | pink `#E7A8A8`, sky `#BFD8D2`, hell fire `#D4552B`, black `#1C1613`, flesh `#EBC9A8`, leaf `#6F8F5A` | Miniature detail, pale Eden vs. dark Hell |
| Grisaille | greys `#2B2B2E` to `#D8D8D8` plus *one* spot colour | Monochrome; colour returns as you restore |

**Readability rules** (so style never wins over play):
- Style effects (blur, dots, swirls) live on background and surface layers. The gameplay layer is always drawn with a **dark ink contour** and a clear value difference from its background.
- Solid platform tops get a consistent "lit edge" in every world.
- Never use colour alone to mark a mechanic: dangerous, grabbable and solid things also differ in **shape and pattern** (accessibility for colour-blind players).
- Monet's fog hides only *far* scenery, never hazards within about 6 tiles.

**Audio:** each world gets music in its painter's era and idiom (our own compositions, not recordings): Monet in Debussy-like harmonies, Cézanne in Satie-style spare piano, Hokusai in koto and shamisen pentatonics, Van Gogh in swirling strings, Seurat as a cabaret-circus waltz, Bosch as hurdy-gurdy and dissonant medieval modes. **Grisaille's world starts as a thin drone and gains instruments as you restore colour.** The repo already generates its audio from code (`tools/audio/make_audio.py`), so we can reuse that approach.

## 11. Tech notes

- The tapestry game already has a Unity 6 project (`unity/`) and an art pipeline where SVG drawings are run through a style filter and rendered by Chromium (`tools/emb/`). The same idea fits here: a **per-painter filter** (dots, hatching, swirl flow, woodblock key lines) over simple vector sprites. Hand-painted-feel at low cost, consistent across worlds.
- In-engine: one tilemap per world plus a **per-world post-process** (bloom/blur for Monet, flow-map swirl for Van Gogh, stipple for Seurat) applied to background layers only.
- Reusable systems from this repo if it stays in the same engine: input and touch controls, level briefings, the gallery screen, audio generation, level generators (`tools/unity/make_levels.py`).
- New systems: pull/carry/throw physics, daub spawning, the Underpainting (separate scene with a timer), mixing wells, the Phantom Frame, four character controllers.

## 12. Legal and sourcing

- Stick to painters who died **before 1956** so their works are in the public domain in life+70 countries (Monet 1926, Cézanne 1906, Van Gogh 1890, Seurat 1891, Hokusai 1849, Bosch 1516).
- **Dalí (d. 1989) and Magritte (d. 1967) are not clear**: a Dalí world would need an original surreal design, not reproductions of his specific works. That's why Bosch fills the dream slot.
- Redraw everything in our own style rather than tracing museum photographs, which can carry their own rights.
- "Style" is free; *specific compositions* are only free if the work is public domain. Do a quick check before using any 20th-century work.

## 13. Roadmap (order, not dates)

1. **Greybox**: one character, pull/carry/throw, a flat test level.
2. **Vertical slice**: level 1-1 in Monet's style, with Lay Figures and koi, plus one Underpainting door.
3. **All four painters** playable, with a level that needs each one.
4. **World 1 complete** (three levels and the Parasol boss).
5. **Second style**: World 2 (Cézanne), to prove the filter pipeline generalises, plus digging.
6. Remaining worlds in order; Mixing wells with W5; Phantom Frame with W4.
7. World 7 and the dream ending; polish, music, gallery extras.

## 14. Stretch ideas

- **Two-player co-op** where you can pick up and throw your partner.
- Colour-mixing across the whole game (any two thrown daubs that meet blend), not just in W5.
- Klimt gold-leaf palace as a bonus world; Munch, Rousseau, Hopper, Vermeer as alternates.
- Daily "gallery challenge" levels.

## 15. Decisions so far

1. **Heroes are the real painters, drawn from their self-portraits.** Cézanne, Van Gogh and Monet all have usable self-portraits. **Seurat has no well-known painted self-portrait**, so his character will be drawn from period photographs/portraits of him, redrawn in our own style (check rights on any photograph). Hokusai has a self-portrait drawing; Bosch has no authenticated one (the best-known likeness is by someone else), so he stays a world, not a playable.
2. **A new repo and project.** This doc moves there when it exists.
3. **Art:** vector sprites run through a per-painter filter.
4. **Hokusai stays.**
5. **Tone:** gentle and whimsical.
6. **Platforms: phone first, then PC, then possibly consoles** (section 16).

## 16. Platforms

**Order:** phone (iOS and Android) → PC on Steam (Windows, plus Mac/Linux/Steam Deck) → consoles if it earns it.

**Is Steam a good fit?** Yes, as the second step. A pick-and-throw platformer with a strong art identity suits it, and Steam Deck compatibility is nearly free if we support controllers. The cost is a one-off fee per game (about US$100 at the time of writing; check Steamworks), a store page with trailer, and the fact that Steam's audience expects more content and polish than a mobile game does. Plan on the Steam version being a fuller edition (extra levels, gallery extras, achievements, controller support), not just a port.

**Consoles:** Nintendo Switch, PlayStation and Xbox each need a developer-programme application (Xbox's ID@Xbox is the easiest to enter), certification, and ratings. Unity has official console support; Godot needs a third-party porting company. A platformer like this is a natural Switch game. Treat consoles as "decide after Steam".

**Engine:** stay with **Unity** (the tapestry game already uses it, we know its touch and audio setup, and every target platform above is supported).

**Design consequences of phone-first:**
- Controls must be **a virtual stick plus two buttons**: *Jump* and *Action* (tap to pull, hold to carry, tap again to throw; hold Action while crouching for a gentle drop). Crouch is down on the stick. No input needs more than two thumbs.
- Everything must work with **see-through touch controls** that don't hide the action (the tapestry game already has these).
- Short levels (about 3 to 5 minutes) with checkpoints so a phone session fits; the Underpainting timer must be forgiving.
- Gamepad and keyboard layouts are designed from day one so the PC port is not a rewrite.
- Cloud save and a "free first world, one unlock purchase" model are the tapestry game's mobile approach and could carry over.

## 17. Open questions

1. Name: **Dream Palette** is confirmed.
2. Business model: **first world free, one unlock purchase** (same as the tapestry game). Confirmed.
3. Where does the new repo live, and under which account? (Setup steps are in the chat; once it exists, this doc moves across.)
