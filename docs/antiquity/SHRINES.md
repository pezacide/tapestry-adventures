# Muse Shrines and the Capsa

Companion to [`DESIGN.md`](DESIGN.md) (section 8, "The Great Floor", has the nine Muses and what they give) and [`HUD.md`](HUD.md). Planning only: the pictures are mock-ups (SVG with PNG copies in `mockups/`), not game art. The screen size is the same 844 by 390 point landscape phone as the HUD.

A **Muse Shrine** is SMB3's Toad House: a small stop on the Great Floor where a friendly NPC hands you an item. Ours has one change worth noting: **you see what is on offer and choose**, instead of opening unknown chests. A phone player should never have to gamble on a tap.

## 1. The shrine screen

![Muse Shrine states](mockups/muse_shrine_states.png)

**Left: browsing.** **Right: after taking the gift.**

| Part | What it is |
|---|---|
| **Room** | A small marble shrine: a lapis frieze with a meander band, two columns, and a black-and-white mosaic floor, the same stage vocabulary as the title screen |
| **The Muse** | A figure on a plinth with her attribute (Calliope holds a scroll), drawn in red-figure style, with a **speech bubble** |
| **Three pedestals** | Each holds one item, with its name. The Muse's **signature gift** (the one in the Muse table in `DESIGN.md`) has a red **HER GIFT** ribbon, and the other two are common helpers |
| **Tap an item** | It is **highlighted** and a dark card at the top shows its name and what it does. A **TAKE IT** button then confirms, so a stray tap never costs you the choice |
| **After taking** | The other two items fade, the chosen one **arcs up into the Capsa badge** (13/28) with a line of confirmation, and the Muse says a blessing. **CONTINUE** returns to the map |
| **LEAVE** | Always available, bottom left; leaving without taking anything is allowed, and you can come back |

**Rules**
- **One gift per shrine** (as an SMB3 Toad House is used up): afterwards a small **laurel mark** shows on the map, and a revisit gets a **free tip or a verse of the Act's music** from the Muse, no item.
- **Both helpers are always visible.** There are no hidden contents.
- **Full Capsa:** if the Capsa is at 28 of 28 distinct items, the Muse says so and the screen offers to swap an item for the gift.
- **Thespis** is the shrine usher, so the first shrine of the game has him explain the screen in one line. Once an Act's boss is beaten, the transformed Keeper also appears at the shrine's door.

## 2. What each Muse offers

Each shrine has the Muse's **signature gift** plus two helpers (Ambrosia Cake, Oil Lamp, Aegis, a Comedy Mask or a map Chisel). The signature gifts follow the table in `DESIGN.md` section 8; the helper pairs are my first proposal.

| Act | Muse | Signature gift (HER GIFT) | Helpers on offer |
|---|---|---|---|
| I | Calliope | Laurel Wreath | Ambrosia Cake, Oil Lamp |
| II | Euterpe | Orpheus's Lyre | Ambrosia Cake, Comedy Mask |
| III | Polyhymnia | Giant Sandal | Ambrosia Cake, Oil Lamp |
| IV | Melpomene | Marble Statue | Ambrosia Cake, map Chisel |
| V | Terpsichore | Winged Sandals | Ambrosia Cake, Comedy Mask |
| VI | Clio | **Hint** (shows one secret on this Act's map) | Ambrosia Cake, map Chisel |
| VII | Erato | Comedy Mask (an extra life) | Ambrosia Cake, Oil Lamp |
| VIII | Urania | Star Chart (warp) | Ambrosia Cake, Aegis |
| any | Thalia | a round of **Vase Match** (no item; see below) | (prizes) |

Notes:
- The **Laurel Wreath** is on offer in Act I only, so the suit is introduced early; later Acts can drop Laurels as pickups in scenes.
- The **Mallet and Chisel** suit arrives in Act VI as a scene reward (Charun's fight), not from a Muse.
- A **Muse who only gives a Hint** (Clio) never shows an item she has no use for: her helpers are neutral.

## 3. The Capsa

![The Capsa](mockups/capsa.png)

The Capsa is the **inventory on the map screen**: 28 slots in 7 columns and 4 rows (the same size as SMB3's), opened from the map. It is *not* in the level HUD, which only has the one-slot **Niche** (the reserve item).

| Part | What it is |
|---|---|
| **Grid** | 28 slots, each a distinct item with a stack count (shown as a small ×2 badge). A slot may stack up to 9 of one item. Selected slot has a gold ring |
| **Detail panel** | Name, whether it is a suit or a map item, what it does, and which Muse gave it |
| **WEAR IT** | Starts the next scene in that suit (the suit is spent when you are hit, as usual) |
| **PUT IN NICHE** | Carries the item into the next scene as the in-level reserve, usable by tapping the Niche |
| **USE** | For map items instead of "Wear": the **Chisel** (break a rock), the **Star Chart** (warp), the **Lyre** (sleep a Satyr), Nephele's cloud (skip a scene) |
| **BACK** | Returns to the map |

- Items persist across sessions with **cloud save**, as in the other two games.
- Item icons are the same as the ones on the pedestals and in the Niche, so a player learns them once.
- **Empty slots** are drawn but dimmed, so the 28-slot size is visible at a glance.

## 4. Vase Match (next screen to design)

Thalia's shrine runs **Vase Match**, the card-matching game. The rules are in `DESIGN.md` section 8: 18 face-down vases, nine shapes, flip two at a time, matches clear. The screen is still to be drawn.

## 5. Still to draw

- The **Vase Match** screen.
- The **Zodiac Room** (the Star Chart warp map).
- Screens for **pause**, **settings** and the **Gallery**.

## 6. Decisions to confirm

1. **Visible choices, not hidden chests.** The three gifts are shown and you choose one. Recommended: yes.
2. **One gift per shrine**, with a free tip on revisits. Recommended: yes.
3. **Signature gift plus two helpers** per Muse, with the helper pairs in the table above. Recommended: yes.
4. **Stacks up to 9 per item** in the Capsa, on 28 distinct slots. Recommended: yes.
5. **A second tap to confirm** (the TAKE IT button) before the gift is given. Recommended: yes.
