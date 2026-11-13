# *Masks & Mosaics* (series: *Into the Frame*, part 3)

*A Super Mario Bros. 3 style platformer staged as a play in antiquity: Egyptian tomb walls, Minoan frescoes, Greek vases, Roman floors, Etruscan tombs and Byzantine gold, all drawn as the art they come from.*

Status: **planning only**. Nothing here is built. This is part 3 of a series. It lives in `docs/antiquity/` until we decide where it goes (decision 9, section 18), alongside `docs/painters/DESIGN.md` (part 2) and the tapestry game in the rest of this repo (part 1).

**Contents:** [0 Names](#0-names) · [1 Pitch](#1-pitch) · [2 The series](#2-the-series) · [3 Pillars](#3-pillars) · [4 SMB3 mapping](#4-what-we-keep-from-smb3-and-what-we-change) · [5 Artists](#5-the-four-artists-playable) · [6 Suits and items](#6-suits-and-items) · [7 Enemies](#7-enemies) · [8 The Great Floor](#8-the-great-floor-the-map) · [9 Acts and levels](#9-worlds-acts-and-levels) · [10 Vandal and ending](#10-the-great-vandal-and-the-ending) · [11 Art direction](#11-art-direction) · [12 Audio](#12-audio) · [13 Tech](#13-tech-notes) · [14 Legal](#14-legal-and-sourcing) · [15 Platforms](#15-platforms-and-business) · [16 Roadmap](#16-roadmap-order-not-dates) · [17 Stretch](#17-stretch-ideas-and-alternatives) · [18 Decisions](#18-decisions). Research notes: [SMB3_NOTES.md](SMB3_NOTES.md). Mock-ups and Act I maps: [LEVEL_MAPS.md](LEVEL_MAPS.md). Acts II to VIII: [LEVEL_MAPS_ACTS_II-VIII.md](LEVEL_MAPS_ACTS_II-VIII.md). HUD and controls: [HUD.md](HUD.md). Muse Shrines and the Capsa: [SHRINES.md](SHRINES.md). Pause, settings and gallery: [MENUS.md](MENUS.md).

---

## 0. Names

**Series name.** Proposed: **Into the Frame**. Every game steps through a different kind of frame: a tapestry border (part 1), a picture frame (part 2), a stage proscenium and a mosaic border (part 3). Alternatives: *The Grand Tour* (good for a non-chronological series, implies travel), *Museum Dreams* (only really fits part 2), *Masterpiece Leap* (fun, but sounds like a single-game title).

**Part 3 title options.**

| Option | Why | Caution |
|---|---|---|
| **Masks & Mosaics** (working title, recommended) | Says both things the game is: theatre and tesserae. Easy to say, easy to search | Check app-store search for clashes |
| **Tessera Tales** | Cute, and "tessera" is the mosaic tile | Less obviously ancient |
| **Mosaic Odyssey** | Strong, Homeric, pixel-art-friendly | Sits right next to *Super Mario Odyssey*. I would avoid it |
| **Ancient Wonders** | Plain and clear | Sits right next to *Super Mario Bros. Wonder*, and "Wonders" is a generic name. I would avoid it |
| **Curtain Call** | Theatre, and a nice end-of-level name | Very generic |

I have not searched any store for these names. Do a store and trademark search before we commit.

---

## 1. Pitch

The curtain rises on a **Greek theatre** at dusk. A chorus in comedy and tragedy masks shuffles into the orchestra (the round dancing floor), and an old actor, **Thespis**, steps out of the chorus to tell us a story. (Thespis is the legendary first actor: the tradition says he was the first to step out of the chorus and speak as a character.)

The story: a restless figure called **the Great Vandal** has been smashing the world's pictures into loose tesserae (the little tiles of a mosaic), sweeping them into a heap, and whitewashing the walls. He has also stolen **Mnemosyne**, the goddess of Memory and mother of the nine Muses, because you cannot smash a memory but you can lock it up. The **Great Floor**, a vast Roman-style floor mosaic that holds every picture together and doubles as the world map, is cracking.

You and three other artists (real craftspeople of the ancient world) step **into the pictures** on the Great Floor, one panel at a time: a Theban tomb wall, a Minoan palace, a Greek vase, a Pompeian villa, the clouds of Olympus, an Etruscan tomb, a Ravenna church. In each you run, jump and stomp through the painted world, win back that panel's **emblema** (the central picture of a mosaic), and move on to the next. At the end you face the Vandal in his Hoard.

It keeps what makes SMB3 special: **a map you read like a board game, suits that change how you move, flight as a reward, a toy-box of items, and every world a different biome with its own tricks**. It adds one thing SMB3 only hinted at: the game is **staged**. It opens on a stage, and the curtain is the loading screen.

## 2. The series

| Part | Game | Model | Frame | Villain |
|---|---|---|---|---|
| 1 | *Ye Tapestrie Adventures* (Unity, in this repo) | SMB1 | tapestry borders | The Cat King |
| 2 | *Dream Palette* (`docs/painters/`) | SMB2 (USA) | picture frames | Baron Grisaille |
| 3 | *Masks & Mosaics* (this doc) | SMB3 | stage and mosaic border | The Great Vandal |

- **Not chronological.** Part 1 is medieval, part 2 is 19th century, part 3 is antiquity. Each part picks the corner of art history whose *medium* suits the Mario game it borrows from. Part 4 can go anywhere (see section 17).
- **Shared conventions** (so the series feels like one family): gentle and whimsical tone; heroes who are real people from the art; a villain who erases or flattens art; a villain whose last hit sends them back where they came from rather than killing them; a small new detail in the ending; a first world free and one unlock purchase; phone first; code-drawn art.
- **No cameos of parts 1 and 2** in this game; it stands on its own. (A later part could add them once more than one sibling exists.)

## 3. Pillars

1. **The medium is the mechanic.** A mosaic is a grid of tiles, so our tiles *are* tesserae. A tomb wall is stacked horizontal bands, so Egypt's levels have **registers** you hop between. A vase is a cylinder painted in black and orange, so its frieze wraps round and a **kiln switch** flips black-figure to red-figure. If the trick would work in any painting, it's the wrong trick.
2. **Staged like a play.** Worlds are **Acts**, levels are **Scenes**, the map is a floor, the curtain rises and falls, lives are masks. Theatre is the SMB3 connection and the unifying look; it is not decoration.
3. **Style never costs readability.** (Same as part 2.) Gold backgrounds, black figures on orange vases and torch-lit tombs are the hardest cases we could have picked, so they get firm rules (section 12).
4. **SMB3 *feel*, our own expression.** We borrow structure, pacing and the *kinds* of tools (glide, flight, statue, swim, hammers). We do not borrow characters, level layouts, sounds or names (section 15).

## 4. What we keep from SMB3, and what we change

| SMB3 | Ours |
|---|---|
| Mario and Luigi | Four ancient artists (section 5) |
| Opening on a theatre stage with a curtain | A Greek theatre with a chorus, **Thespis**, and masks; each Act starts and ends with the curtain |
| Princess, kings turned into animals, stolen wands | **Mnemosyne** (Memory) is captured; each world's **emblema** is stolen |
| Koopalings (one per world), then Bowser | A **Wonder or monster boss** per world, then **the Great Vandal** |
| World map with nodes, rocks, bridges | **The Great Floor**: a Roman floor mosaic. Nodes are tesserae, paths are meander borders (section 8) |
| Hammer Brothers wandering the map | **Wandering Satyrs** |
| Toad House | **Muse Shrine** (nine Muses hand out items) |
| N-Spade card game | **Vase Match** (flip and match vase shapes) |
| Warp whistle | **Urania's Star Chart** |
| Airship level | **Ship battle**: Nile barque, Minoan galley, Argo, liburnian, cloud galley, Etruscan ferry, Byzantine dromon |
| Mini-fortress and Boom Boom | **Tomb / temple / kiln** fortress with a mini-boss |
| `?` block | **Amphora block** (breaks into sherds when used) |
| Brick block | **Mudbrick** (W1 to W2), **marble block** (later) |
| Coins | **Gold tesserae**; 100 = a life |
| Lives | **Comedy masks** (a life); the Game Over screen is a **tragedy mask** that the Chorus gently turns back into a comedy mask ("Again!") |
| Warp pipe | **Pithos**: a giant storage jar buried to its neck (Diogenes lived in one, so he sits in one as a cameo). In Rome the pipes are lead aqueduct pipes |
| P-meter | **Winged meter**: the little wings on your sandals fill as you run |
| End-of-level card roulette | **Victor's Dais**: a podium with a spinning roulette of three prize icons; you take a bow. Three in a row gives extra lives |
| Inventory (28 slots) | **The Capsa** (a Roman scroll-case), also 28 slots |
| 88 levels | **41 levels** across 8 Acts (section 9). Part 1 is 29, part 2 is 20 |
| Timer | **No timer** (like part 2). Mid-level checkpoint at a **brazier** (Hestia's hearth flame) |

## 5. The four artists (playable)

**Approach.** Parts 1 and 2 use the real painters, drawn from their self-portraits. **Antiquity gives us a problem: almost no ancient artist's face survives.** Most were anonymous; the few we know by name (potters who signed their pots, the odd mosaicist, a court sculptor) left us their *names* but no likenesses. So:

- The heroes are **real, named ancient artists**, but **their faces are invented**, and we say so on their gallery cards.
- Each is drawn **in their own medium**, which also gives us the same "style clash" as parts 1 and 2: Exekias in black-figure walking through a tomb painting looks very out of place, and that is the joke.
- Their outline stays strong so they always read.

| Artist | Who they were (briefly) | Drawn as | Feel | Run | Jump | Special |
|---|---|---|---|---|---|---|
| **Exekias** (Athens, active c. 545 to 530 BC) | Potter *and* painter; signed "Exekias made me". Famous for the *Achilles and Ajax playing dice* amphora and the *Dionysus in a boat* cup | Black-figure silhouette with incised lines and a white-and-purple accent | The all-rounder ("Mario"). Steady, precise | medium | medium | **Steady Hand**: shortest recovery after a hit; best at learning the game |
| **Iaia of Cyzicus** (active in Rome, 1st century BC) | A woman painter whom the Roman writer Pliny the Elder praised for her speed and skill, and says painted a self-portrait in a mirror (it is lost) | Encaustic panel portrait, with big watchful eyes (in the manner of the Fayum mummy portraits, a style clash on purpose) | Light and quick ("Luigi"). Floaty | medium | **highest** | **Mirror Hop**: a tiny second hop after a stomp |
| **Thutmose** (Amarna, active c. 1340 BC) | Court sculptor of the pharaoh Akhenaten. His workshop at Amarna was found with the famous bust of Nefertiti | Raised limestone relief with Egyptian profile and flat colour | Heavy and sturdy | slow acceleration | low | **Hard Head**: breaks blocks from below even when small; **Statue** suit lasts longer |
| **Sosus of Pergamon** (2nd century BC) | Famed mosaicist: *The Unswept Floor* (a floor of food scraps, with a mouse) and *The Doves Drinking*, known now through Roman copies | A little tessera mosaic figure | Small, fast | **fastest** (fills the winged meter quickest) | low | **Tessera Magnet**: collects tesserae in a small radius |

**Why these four.** Two potters/painters (one Greek, one Roman-era), one Egyptian sculptor, one Hellenistic mosaicist, so every medium the game uses has someone at home. Iaia matters because the ancient art world was not all men: Pliny lists several women painters (Timarete, Irene, Kalypso, Aristarete and Iaia), though only a few lines about each survive.

**Why not the famous names?** Praxiteles, Phidias, Apelles and Polykleitos are tempting, but their originals are lost and their portraits are invented anyway. We prefer people whose *works are the game's backgrounds*.

**Cameo characters** (not playable, but fun):
- **Thespis** (host; returns as the Muse Shrine usher).
- **Daedalus** (legendary craftsman of the Labyrinth in W2; later offers the **Wax Wings** joke item in W5, and his son Icarus appears in the sky).
- **Diogenes** in his pithos (W1 to W4 bonus rooms; "Stand out of my sun" is his legendary line to Alexander, so he complains that you are blocking the light).
- **Arachne**, the legendary weaver turned into a spider in Ovid's telling, as a tiny nod to part 1's tapestry. (Optional.)

Rules of the cast:
- Each artist reaches something others can't (Iaia's height, Sosus's winged meter, Thutmose's blocks, Exekias's recovery for a hard room).
- Respectful portrayal. In black-figure the convention is dark skin for men and white for women; in Egyptian art, the same with red-brown and yellow. Those are conventions *of the medium*, not statements about people; the gallery card explains this.

## 6. Suits and items

SMB3's suits are its identity. Ours are objects from ancient life and myth. Each suit has **one new verb**, so the set stays as readable as the original.

| Suit | SMB3 analogue | What it does | Introduced |
|---|---|---|---|
| **Laurel Wreath** | Super Leaf | **Short glide** after a run-up, plus a **wreath spin** (a quick sweep that stuns enemies and bonks blocks) | W1 |
| **Triton's Conch** | Frog suit | **Swimming** (fast and graceful in water, clumsy on land). Blowing it calls a dolphin who carries you | W2 |
| **Giant Sandal** | Kuribo's shoe | A single huge hobnailed sandal you hop in: **stomp spikes, ice and hot things**; hops high; can't be used underwater. Reusable in a level | W3 (Giant Act) |
| **Marble Statue** | Tanooki statue | Hold **Down + Action** to strike a *contrapposto* pose: invulnerable, but frozen, and for a few seconds only | W4 |
| **Winged Sandals of Hermes** | P-wing | **Sustained flight** for one level (tap Jump to flap, SMB3 raccoon style). The Winged meter at full | W5 (also an item from W2) |
| **Sculptor's Mallet and Chisel** | Hammer suit | **Throws chips** (arcing stone shards) and, **crouched, you are a sturdy block** immune to fire. In W7 it **scrapes whitewash** off hidden mosaics | W6 |
| **Orpheus's Lyre** | Music box | Plays a tune that **sleeps wandering enemies** for a stage (or the map's Satyrs for one battle) | Muse Shrine item, from W1 |

**Proposed additions** (SMB3 leans on a few things we should not leave out):

| Item | SMB3 analogue | Notes |
|---|---|---|
| **Ambrosia Cake** | Super Mushroom | Makes you "grown" (a bigger figure, one extra hit). Honey cakes are really ancient |
| **Oil Lamp** | Fire Flower | Throws bouncing drops of burning oil. The **only** way to cauterise the Hydra's heads (section 9, W3) |
| **Aegis** | Starman | Athena's shield with the Gorgon's head: you are **invincible** for a few seconds and enemies you touch **turn to stone statues** (non-lethal, and they crumble into tesserae) |
| **Comedy Mask** | 1-Up | An extra life |
| **Kiln Switch** | P-Switch | Flips black-figure to red-figure in any level (section 9, W3); available in every Act as a pickup |

**Phone controls.** SMB3 needs "hold B to run." On a phone that is a pain. We use **Jump** and **Action** buttons plus a stick; **pushing the stick fully means run**, so the winged meter fills without a third input. Action throws, spins, strikes the statue pose or plays the lyre, depending on the suit. Down on the stick is crouch, enter pithos or hold the statue pose.

## 7. Enemies

Generic cast, recoloured per world, mapped to SMB3's archetypes. Every one gets a **look** from the medium it appears in (a profile figure in the tomb, a black silhouette on the vase).

| Ours | SMB3 analogue | Behaviour |
|---|---|---|
| **Tessera Beetle** | Goomba | Walks. Stomped, it bursts into gold tesserae |
| **Chelone** (a tortoise; Hermes made the first lyre from a tortoise shell) | Koopa | Walks; stomp for a shell; kick it. **Winged Chelone** hops or flies. Kicked shells bring lyre sounds |
| **Acanthus Snapper** | Piranha Plant | Pops out of pithoi and aqueduct pipes (acanthus is the leaf on Corinthian columns) |
| **Discobolus statue** | Bill Blaster | A discus-thrower on a plinth hurls **discs** (the Bullet Bills) |
| **Column Drum** | Thwomp | Waits, then drops |
| **Cave Canem Hound** | Chain Chomp | A mastiff on a chain, from the real "Beware of the dog" Pompeii mosaic |
| **Canopic Stack** | Pokey | Stack of four canopic jars with the heads of the four sons of Horus (human, baboon, jackal, falcon); you stomp the top ones off |
| **Aten** (a sun disc whose rays end in little hands) | Angry Sun | Swoops down in W1 |
| **Flying Fish, Octopus, Urchin** | Cheep Cheep, Blooper, Spiny | Minoan sea creatures. Marine Style vases and the "Flying Fish" fresco from Melos are the sources |
| **Harpy** | Para-Goomba | Flies, swoops |
| **Nephele** (cloud nymph) | Lakitu | Rides a cloud and tosses things (W5). Nephele is the Greek word for cloud |
| **Shade** | Boo | Shy: only moves when you're looking away. W6 |
| **Bandage Scamp** (comic mummy) | Dry Bones | Collapses, then reassembles |
| **Satyr** | Hammer Bro | On the map and in some levels. Hurls wineskins and pine cones (the thyrsus tips) |
| **Whitewasher** | (new) | Vandal's minion with bucket and brush; paints a wall over, making it solid or invisible. W7 to W8 |
| **Oarsman** | Rocky Wrench | Pops out of oar ports on ship levels and tosses pots (our version of cannonballs: gentle) |
| **Pot Bomb** | Bob-omb | Walking amphora that bursts into sherds |

## 8. The Great Floor (the map)

The map is a huge **Roman floor mosaic**, in the manner of the Nile Mosaic of Palestrina and the black-and-white floors of Ostia. It is seen from above, and it cracks as the Great Vandal works.

- **Nodes** are tesserae; **paths** are meander (Greek key) borders.
- **Panels** (one per Act) are islands in the floor, each with its own look.
- The **Vandal's cracks** spread across the floor as the game goes on; each emblema you recover mends a stretch of it.
- **Wandering Satyrs** dance along paths. If one catches you, you get a **one-screen fight** (like SMB3's hammer bros). Win and you get a Capsa item. **Orpheus's Lyre** makes them sleep.
- **Muse Shrines** (SMB3's Toad House): a small shrine where a Muse hands out an item. The nine:

| Muse | Domain | Gives |
|---|---|---|
| Calliope | Epic | Laurel Wreath |
| Clio | History | Hint: reveals a hidden path or shortcut on the map |
| Erato | Love poetry | Comedy Mask (1-Up) |
| Euterpe | Music | Orpheus's Lyre |
| Melpomene | Tragedy | Marble Statue |
| Polyhymnia | Hymns | Giant Sandal |
| Terpsichore | Dance | Winged Sandals |
| Thalia | Comedy | A round of **Vase Match** |
| Urania | Astronomy | **Star Chart** (warps you to a later Act, like SMB3's whistle) |

- **Vase Match** (SMB3's card game): 18 face-down vases, nine shapes (amphora, hydria, kylix, krater, lekythos, oinochoe, pelike, skyphos, rhyton). Flip two at a time; matches clear. Pairs give coins and, at the end, items or lives.
- **Gates** are barred with sphinx and lion statues that you unlock by clearing the Act's fortress.
- Each Act begins with a curtain and a title card, "**Act III: The Heroes' Vases**", and ends with a short *exodos* (exit) card.

## 9. Worlds (Acts) and levels

**41 levels** in 8 Acts: Act I has **five scenes, a fortress with a mini-boss, and a ship battle that leads to a Wonder or monster boss** (seven in all); each of Acts II to VII has **three scenes, a fortress and a ship battle** (five); Act VIII has four. Order mirrors SMB3's biomes (the user's direction, loosely): Desert, Water, Giant, Pipe, Sky, Dark, a polished-gold "ice" world, and the final world.

| Act | Place and art | SMB3 biome | Signature mechanic | Mini-boss (fortress) | Boss (ship, then Wonder or monster) |
|---|---|---|---|---|---|
| I | **The Nile**: tomb paintings and papyrus | Desert | **Registers** (stacked lanes), bending reeds, quicksand, the Aten swoop | **Khepri**, the Great Scarab (rolls the sun) | **The Sphinx** (riddle) |
| II | **Crete**: Minoan frescoes | Water | Swimming, dolphin rides, **bull-leap vaults** | **The Minotaur** (Labyrinth) | **Talos**, the bronze giant |
| III | **Heroes' Vases**: black-figure and red-figure | Giant Land | **Giant scale**, **Kiln flip** (black-figure to red-figure), vase wrap-around | **The Nemean Lion** | **The Lernaean Hydra** |
| IV | **Rome**: Pompeii frescoes, floor mosaics | Pipe Land | **Aqueduct pipes** and **valves**: you steer the water; Vesuvius rumbles | **The Retiarius** (gladiator with a net) | **The Colossus of Nero** |
| V | **Olympus**: sky, clouds, vase-and-relief gods | Sky | Cloud platforms, **Winged Sandals**, the Icarus **Wax Wings** joke | **Atlas** (a tower climb) | **Zeus** |
| VI | **Etruscan Tombs**: Tarquinia wall paintings | Dark Land | **Lamp light**: you see by your own light; false doors | **Charun** (the Etruscan demon with a mallet) | **Aita (Hades)** and Phersipnai |
| VII | **Byzantium**: Ravenna gold | Ice Land | **Polished gold floors** (slippery), **light beams** that lift you, **scraping whitewash** | **The Whitewasher** | **The Throne of Solomon** (golden automaton lions and birds) |
| VIII | **The Vandal's Hoard** | Dark Land / Bowser's Castle | **Re-laying the floor**: broken tesserae, whitewash, a shattered map | (none) | **The Great Vandal** |

### Act I: The Nile (tomb painting)

The Egyptian tomb wall is the clearest SMB3 *tutorial* look there is: flat colour, strong outlines, and a famous six-colour palette.

| Scene | Name | Idea | New things |
|---|---|---|---|
| I-1 | *The Marsh of Nebamun* | The fowling-in-the-marshes scene from the tomb-chapel of Nebamun (c. 1350 BC): papyrus thickets, birds and fish, and **the cat that catches the birds** | **Bending reeds** that spring you up; teaches run, jump, stomp, Tessera Beetles. Laurel Wreath hidden |
| I-2 | *Harvest Registers* | A tomb wall as horizontal bands: ploughmen, oxen, scribes tallying grain | **Registers**: three lanes stacked, with ladders and gaps to hop between. Canopic Stacks |
| I-3 | *The Sands of Giza* | Dunes and pyramids, no tomb | **Quicksand** pits; the **Aten** dives; sloping sand |
| I-4 | *The Collapsing Causeway* | An autoscroll along a causeway of stone slabs and mudbrick on the floodplain | **Autoscroll** pressure; slabs that **crumble** if you linger; mudbrick walls a **kicked shell** breaks; collect every tessera and a **gold shrine** appears on the map |
| I-5 | *The Shaduf Barges* | Rope-and-pulley barges and **shadufs** (the counterweight water-lifts of the Nile) | **See-saw shaduf platforms**; **rope barges**; one barge that **frays** at the end (visible fibres show it); leaping fish |
| I-6 | *The Scribe's Tomb* (fortress) | A dark tomb of brick, hieroglyph-ish walls (invented glyphs, see section 14) | Bandage Scamps; **mini-boss Khepri** rolls the sun-ball: hop it, then stomp him three times |
| I-7 | *Barque of Millions of Years* | A scrolling **solar barque** battle (the sun-boat of Ra) as a ship level | Oarsmen toss pots; then **boss: the Sphinx** (below) |

**The Sphinx (riddle boss).** She poses the Theban riddle ("four legs in the morning, two at noon, three in the evening") as a **picture riddle**: three plinths show an animal, a person and a bird; the correct one (the person) glows when you **stand on it and strike the sun-gong**; wrong ones drop a small hail of sand. Four rounds, each faster. (The riddle is Greek, so she speaks Greek: the play is a Greek play about Egypt, and Thespis points that out.) She is a lion with a human face, not a scary one: she *loves* being right, and yelps when she's out-riddled.

**Level sketch, I-1** (`~` marsh water, `|` papyrus reed, `o` tessera, `?` amphora block, `B` Tessera Beetle, `C` the cat, `P` start):

```
                         ()                ()
              ?  ?                 ?
 P        |  |   o o  o        |  |     o  o         [Victor's Dais]
=====~~~====   =========  ~~~~~~~~  ===========  ~~~~======
            B           C                    B
```
Beats: run (the Winged meter fills); stomp a beetle; a reed that springs you up; a pit with a hidden pithos; the cat, who is an *enemy who changes sides*: pelt her with a stomp and she drops the bird she caught, which is a coin shower.

### Act II: Crete (Minoan frescoes)

| Scene | Name | Idea | New things |
|---|---|---|---|
| II-1 | *The Dolphin Hall* | An underwater level from the Queen's Megaron dolphin fresco: dolphins and fish on a blue floor | **Swimming** (Triton's Conch); dolphins carry you; **Flying Fish** |
| II-2 | *The Bull-Leaping Court* | Based on the Bull-Leaping fresco: acrobats vaulting a charging bull | **Bull vaults**: a charging bull is a moving springboard; grab the horns and flip over |
| II-3 | *The Marine Style Seabed* | Octopus-and-seaweed pottery come to life | Spiralling octopus tentacles; **Octopus, Urchin** |
| II-4 | *The Labyrinth* (fortress) | A palace maze (the real palace at Knossos is the likely origin of the Labyrinth legend) | Ariadne's **thread** is your guide: follow it to the exit. **Mini-boss: the Minotaur**: he charges and rams a wall; when stunned, strike him. He is called **Asterion** ("starry"), his real myth name, and he is lonely, not horrid |
| II-5 | *The Fleet of Thera* | The Miniature Ship Fresco from Akrotiri, with its long galleys and dolphins | Oarsmen; **boss: Talos**, the bronze giant who circles Crete throwing rocks. In the myth his one weak spot is a plug in his ankle: hit it three times |

### Act III: The Heroes' Vases (giant land)

Black-figure vases are black silhouettes on orange clay, with incised detail. Red-figure flips the colours.

| Scene | Name | Idea | New things |
|---|---|---|---|
| III-1 | *The Labours Frieze* | The twelve labours of Heracles as a looping frieze; enemies are **giant** | **Giant scale** (you are tiny): bigger foes with several hit points. **Giant Sandal** introduced |
| III-2 | *Achilles and Ajax Play Dice* | The famous Exekias amphora | Dice as platforms; the heroes' voices pop out as inscriptions ("three", "four"). *Tessera* comes from the Greek for "four", which Thespis will happily tell you |
| III-3 | *Dionysus Sails* | The Exekias cup: Dionysus in a boat with a vine growing up the mast, dolphins all round | **Vase wrap-around** (round the cup, level loops); **Kiln Switch** flips black-figure and red-figure, which swaps what's solid |
| III-4 | *The Kiln* (fortress) | A pottery kiln | **Mini-boss: the Nemean Lion**: his pelt can't be pierced, so wait until he rolls and yawns |
| III-5 | *The Argo* | The ship of the Argonauts | Harpies; then **boss: the Hydra**: cut a head and two grow back unless you **cauterise the stump with the Oil Lamp** (as Heracles' nephew Iolaus did) |

### Act IV: Rome (pipe land)

| Scene | Name | Idea | New things |
|---|---|---|---|
| IV-1 | *The Aqua Claudia* | Aqueduct arches and the water channel on top | **Water flow** you ride; **pipes** (lead aqueduct pipes are the warp pipes); **Acanthus Snappers** |
| IV-2 | *Cave Canem* | A Pompeian street and house, from the "Beware of the dog" threshold mosaic | **Cave Canem Hounds**; atria and peristyles; the Alexander Mosaic as a floor you cross |
| IV-3 | *The Villa of the Mysteries* | The famous red-walled fresco cycle, where figures step out of the wall | Figures that **step out and become platforms**; **valves** that redirect water |
| IV-4 | *The Hypogeum* (fortress) | The Colosseum's underworks of lifts, trapdoors and cages | **Mini-boss: the Retiarius** (net and trident): he throws nets that you hop. **Marble Statue** introduced |
| IV-5 | *The Liburnian* | A Roman galley | **Boss: the Colossus of Nero**: a giant bronze statue (real: it stood near the Colosseum and gave it its name); you fight from the arena while Vesuvius rumbles in the background |

### Act V: Olympus (sky)

| Scene | Name | Idea | New things |
|---|---|---|---|
| V-1 | *Cloud-Cuckoo-Land* | The city in the sky from Aristophanes' comedy *The Birds* | Cloud platforms; **Nephele** throws things; bird-people |
| V-2 | *The Gigantomachy* | The giants' battle, in the manner of the Pergamon Altar frieze | Marble-relief look; giants with snakes for legs; parallax |
| V-3 | *The Chariot of Helios* | A scrolling sky ride | Autoscroll; **Winged Sandals** flight introduced. Icarus cameo and **Wax Wings** (joke item: fly high and they melt) |
| V-4 | *Atlas* (fortress) | A tower climb up the shoulders of the Titan holding the sky | **Mini-boss: Atlas**: he lets the sky slip when you hit his hands |
| V-5 | *The Cloud Galley* | A sky ship | **Boss: Zeus**: throws zig-zag thunderbolts and turns into a swan and then a bull (his stories); he laughs when you dodge. He is a show-off, not a menace |

### Act VI: The Etruscan Tombs (dark land)

| Scene | Name | Idea | New things |
|---|---|---|---|
| VI-1 | *The Tomb of the Leopards* | A Tarquinia banquet chamber with musicians, dancers and leopards on the pediment | **Lamp light**: only your torch lights the wall; paintings appear as you pass |
| VI-2 | *Hunting and Fishing* | The Tarquinia tomb with the sea scene: divers, fishing boats, dolphins, birds | Lamp light plus a **sea section**; divers as moving platforms |
| VI-3 | *The Dancers' Triclinium* | The Tomb of the Triclinium, whose banqueters and dancers move to the music | **Beat platforms**: platforms appear on the beat of the dance; a rhythm level |
| VI-4 | *The Tomb of Orcus* (fortress) | A cave with false doors | **False doors** (painted doors that turn out to be real, or not). **Mini-boss: Charun**, a blue Etruscan death-demon with a hammer. Sculptor's Mallet introduced |
| VI-5 | *The Ferry on the Styx* | A ferry level | Shades; **boss: Aita (Hades)** in his wolf-skin cap, and **Phersipnai** (Persephone), who feeds him pomegranates to keep him calm |

### Act VII: Byzantium (gold)

| Scene | Name | Idea | New things |
|---|---|---|---|
| VII-1 | *The Procession at Ravenna* | The court procession mosaics of San Vitale (6th century) | **Gold ground**; **polished floors** (slippery); drawn from our own original figures, not copies |
| VII-2 | *The Starry Vault* | The mausoleum of Galla Placidia, a deep-blue vault of gold stars | **Star platforms**; the sky is the level |
| VII-3 | *The Great Dome* | Hagia Sophia's dome, with its ring of windows | **Light beams** lift you like an updraft; swinging chandeliers |
| VII-4 | *The Whitewashed Chapel* (fortress) | A chapel whitewashed over | **Scraping** the whitewash with the Mallet and Chisel to reveal the mosaic and the path; **mini-boss: the Whitewasher** |
| VII-5 | *The Dromon* | A Byzantine war-galley with fire siphons | **Boss: the Throne of Solomon**: the golden automaton throne of the Magnaura palace, described in the 10th century by the envoy Liutprand of Cremona, with roaring lions and singing birds; the throne rises as it fights |

### Act VIII: The Vandal's Hoard

| Scene | Name | Idea | New things |
|---|---|---|---|
| VIII-1 | *The Broken Floor* | The map itself is shattered | **Falling tessera floors**; you re-lay as you go |
| VIII-2 | *The Whitewashed Gallery* | Callbacks to Acts I to VII, whitewashed | Throw/spin tesserae to **recolour** and open paths; recall one trick from each Act |
| VIII-3 | *The Fleet of the Great Vandal* | A multi-ship chase, all the ship types | Each ship uses a different Act's enemies; last ship carries the Hoard |
| VIII-4 | *The Unswept Floor* | The Vandal's lair, shaped like Sosus's mosaic of food scraps and a mouse | **Boss: the Great Vandal** (section 10) |

### What we took from SMB3

The full research is in [`SMB3_NOTES.md`](SMB3_NOTES.md): a table per Act that maps SMB3's levels to scene ideas. This section holds what we **decided** to take.

**Rules adopted**
- **Power-up substitution.** A power-up block gives an Ambrosia Cake if you are small, and the next item up (Laurel Wreath, Lamp) if you are already grown.
- **Rewards on a small skill check.** Hidden things are found by side-bumping, crouching on a special panel or a run-up glide, never by guessing.
- **Suits as keys.** Every suit opens at least one secret per Act.
- **Tessera thresholds.** Collecting all of a scene's tesserae makes a shrine appear on the Great Floor (an anchor, Winged Sandals, and so on).
- **Shortcuts cost something** (the hidden Star Chart in a fortress ends it early but leaves the gate shut).
- **Every life-farm is capped:** spawner pots, bouncing enemies, brick resets.
- **Mini-bosses change after the first hit** (SMB3's Boom Boom grows wings), and each boss reuses the previous one's pattern with one twist (below).
- **Difficulty cap.** The wiki's manga ratings run 2/10 to 9/10. We cap main-route scenes at about **7/10** and keep 8 to 9 for optional bonus scenes.
- **No timer.** Scenes that relied on one use a pacing device instead (a rising tide, a closing door).
- **Gentle hit rule** (the international version's): a hit suit becomes the "grown" form, and a hit "grown" artist shrinks.

**Act I is seven scenes.** SMB3's World 1 has six levels, a fortress and an airship, and Act I is the free sample, so it gets two extra scenes (the Act I table above lists all seven).

**The Keeper, transformed.** In SMB3 each world's king has been **turned into an animal** by the Koopaling who stole his wand, and beating the boss turns him back. Our version fits Greek myth perfectly: **each Act's Keeper has been transformed**, as in Ovid's *Metamorphoses*. Defeating the boss returns the **emblema** and restores the Keeper, and the Keeper is the Act's host at its Muse Shrine afterwards.

| Act | Keeper | Turned into |
|---|---|---|
| I | The Scribe of the Nile | a **cat** |
| II | The King of Crete | a **bull** |
| III | A Hero's herald | a **stag** (the myth of Actaeon) |
| IV | The Villa's master | a **goose** (Roman house-guards, the geese that saved the Capitol) |
| V | The Cloud King | a **swan** |
| VI | The Tomb's host | a **wolf** (Aita's wolf-skin cap) |
| VII | The Emperor's steward | a **peacock** (a Byzantine symbol) |

#### Boss progression

The wiki's Larry and Morton share one pattern, and Morton is **slower but fires his wand more often**. That teaches a rule we should copy: **each boss is the previous boss's pattern plus one twist**. For us:

| Act | Boss | Pattern | Twist |
|---|---|---|---|
| I | Sphinx | attack, expose, chase, three hits | the riddle (the *expose* step is picking the right plinth) |
| II | Talos | same | the ankle plug (a weak spot) |
| III | Hydra | same | heads regrow unless you cauterise |
| IV | Colossus | same | you fight on a rising and falling arena floor |
| V | Zeus | same | he changes shape (swan, bull) mid-fight |
| VI | Aita | same | the arena is dark, lit by your lamp |
| VII | Throne of Solomon | same | lions and birds come in waves |
| VIII | Great Vandal | floor-smash (Bowser-style) | the arena changes medium |

#### Map gimmicks worth lifting

| SMB3 | Ours |
|---|---|
| **Toggling bridges** (World 3) | **Tide bridges** in Act II, **gates** that open and shut as the Muse Shrine lights |
| **Rock plus Hammer gives a Canoe** | The Chisel breaks a rock to free a **dolphin** or a **boat** |
| **Treasure Ship replaces a Hammer Bro** if its unlock conditions are met | A **treasure galley** that replaces a Satyr when you meet a hidden condition (e.g. 100 tesserae) |
| **Hammer Bro battle rewards differ per pair** (Hammer, Starman, cloud, P-Wing, Music Box) | Each Satyr fight shows which prize is at stake before you commit |
| **Star spaces** (World 5) | **Constellation spaces**: warp points on the map |
| **Map shapes** (the Giant Land map is a turtle) | Each Act's map island has a **shape** (the tortoise for Act III, a bull's horns for Act II, a ship for Act I) |

## 10. The Great Vandal and the ending

**Who he is.** A towering **Sweeper**: broom in one hand, maul in the other. He cannot bear a mess, and to him every picture is mess: it *looks back at him*, it takes up wall. So he **smashes pictures into tesserae and sweeps them into the Hoard**, then whitewashes the bare wall. (Decided: the Sweeper.) The name has two layers. The Vandals were a real Germanic people who sacked Rome in AD 455, and the word "vandalism" was coined much later (in 1794) and is not fair to them; we make the character entirely fictional and the gallery card explains the word. His title is **the Iconoclast**, "image-breaker", from the real Byzantine period (726 to 843) when some images were whitewashed or removed. We **never mock any living religion's imagery**: no sacred figure is ever an enemy, a target or a prop that you smash.

**The fight (SMB3 Bowser style).** The Vandal's lair is a giant version of Sosus's *Unswept Floor*. He leaps and smashes the floor, so the tiles drop away in rows; wait for him to leap and **hit the floor so he falls through the gaps**. Phase one is Egyptian flat colour, phase two a Greek vase, phase three gold mosaic: the arena **changes medium** as he gets more frantic. Three falls, and the Hoard opens under him.

**Ending.** The loose tesserae of the Hoard **fly up and re-lay themselves**, tile by tile, into the Great Floor, with Mnemosyne in the middle. Thespis bows. The credits scroll over the Floor as it mends, with every cast member in a corner. The Vandal gets a last scene: he sweeps one final crumb, and **leaves a single tessera out of place**, which he looks at for a long time, and then sits in the front row of the theatre to watch the play. The tragedy mask turns into a comedy mask. Last frame: a tiny mouse (from *The Unswept Floor*) nibbles that one tessera.

## 11. Art direction

Three layers of look, as in the other two games.

1. **Frame** (menus, curtain, theatre, map): the Greek theatre and the Roman floor. Fixed look.
2. **World** (one per Act): the medium's own rules (section below).
3. **Hero** (each artist drawn in their own medium), so they clash gently with the world they walk through.

**Tesserae as pixels.** The mosaic Acts, and the map, are drawn on a chunky grid, so "tile = tesserae" is literal. Proposed: the 48-px game tile (as in part 1) is **8 by 8 tesserae of 6 px**. Real mosaics lay tesserae in lines that follow the shape (the *andamento*), so our filter does not use a plain grid but **flowing rows around outlines**, with a darker grout line.

| Act | Palette (hex) | Rendering |
|---|---|---|
| Theatre and map | stage curtain `#8E1F2B`, gold fringe `#D8A93C`, limestone `#D9CDB4`, sky `#9FD0E8`, laurel `#4E8A4A`, mask white `#F4EAD9`; floor: marble `#E9E2D0`, slate `#2A2626`, terracotta `#B8573A` | Hard-edged mosaic floor; curtain in painted folds |
| I Nile | papyrus green `#3C8D5A`, Egyptian blue `#2E5FA8`, red ochre `#B5452D`, yellow ochre `#D9A441`, plaster `#F0E4C8`, black `#1A1411` (the six-colour Egyptian palette) | Flat fills, dark outline, strict profile figures; **registers** drawn as bands; plaster grain |
| II Crete | Knossos blue `#2F6DB5`, sea teal `#3FA7A3`, Minoan red `#C0472E`, saffron `#E0A94A`, lime plaster `#F5EEDC`, outline `#1B1B1F` | Curving, springy outlines; wavy borders; lots of spirals; plaster cracks |
| III Vases | kiln orange `#C8662B`, glaze black `#17100E`, white slip `#F2E6D0`, purple-red `#7A2338`, red-figure highlight `#D88A4F` | Black silhouettes with **incised** lines (scratched lighter lines); fine crazing; **Kiln flip** swaps to red-figure (orange figures on black) |
| IV Rome | Pompeian red `#B3322B`, wall ochre `#E1B04C`, marble `#EDE6D6`, sea tesserae `#3A6EA5`, terracotta `#C46B3B`, laurel `#5E8C57`, outline `#1C1918` | Fantastical architecture panels, painted garlands; black and white floor mosaics; lead-pipe grey |
| V Olympus | sky `#8CC6E8`, cloud `#F5F2EA`, Aegean `#2F72B8`, gold `#E5B94A`, laurel `#4F8A4C`, thunder violet `#6A4C93`, outline `#23284A` | Clouds as bold puffy shapes; marble relief with drop shadows; vase-painting gods in red-figure |
| VI Etruscan tombs | tomb ochre `#C45A3B`, yellow `#E2B857`, deep blue `#244B7E`, tuff brown `#4A2C22`, plaster `#E8D9B5`, torch `#F08A2E`, shadow `#15100E`, Charun blue `#5B8FA8` | Lively, loose profiles with almond eyes; **lamp light** radius and soft darkness |
| VII Byzantium | gold `#D9A93A`, lapis `#1F3A8A`, imperial purple `#5B2A6E`, emerald `#2E7D5B`, vermilion `#C8432B`, ivory `#F3EAD0`, backing `#1A1530` | **Gold-ground mosaic** with tesserae at tiny angles, so gold glints; wide-eyed, front-facing figures |
| VIII Hoard | whitewash `#ECE8DF`, plaster grey `#B8B2A5`, shadow `#4B4640`, ink `#1F1B18`, plus loose tesserae: gold `#E2B64A`, lapis `#2F5BA8`, vermilion `#C8432B`, malachite `#3C8D5A` | Bare plaster and rubble, with heaps of loose tiles in every colour from the other Acts. Colour returns when you re-lay |

**Readability rules** (so style never wins over play):

- The **gameplay layer is always drawn with a dark contour** and clear value contrast against the layer behind it (target: at least 3:1 luminance contrast; we should write a build-time check script).
- **Solid platform tops get one consistent lit edge** in every Act, and solid ground carries a **border pattern** (meander, palmette, brick) that nothing hostile ever uses.
- **Never colour alone**: hostile things have **jagged or spiky silhouettes**; grabbable ones have **round or haloed** ones; solid ones have a **border pattern**. (Accessibility for colour-blind players.)
- **Act-specific traps:**
  - *Act III (black-figure):* figures and platforms are both black. Platforms always have a meander top band; enemies have white/purple accents and eyes. After a Kiln flip, the same rules hold in red-figure.
  - *Act VI (dark):* darkness is a mood, not hidden information. **Hazards keep a faint lit rim in the dark**, and nothing dangerous is placed farther than about 6 tiles outside the lamp's light.
  - *Act VII (gold):* gold on gold would hide coins and platforms. **Gold is for backgrounds and decoration only.** Gameplay objects are dark lapis, purple or emerald with a bright edge; collectibles glint **white-silver** with a dark rim; background gold sits on a darker backing plane.
  - *Act I:* the registers must be visually obvious: each lane has its own ground colour band and a clear divider.
- Tesserae noise lives on **background and surface** layers. Sprites use **fewer, coarser tesserae** than scenery.
- **Stage frame:** the curtain and Greek theatre are shown only on menus, intros and outros, not during play (the screen is small).
- **The Greek theatre vs the amphitheatre.** Strictly, a Greek building for plays is a *theatre* (semi-circular, with the *orchestra* and the *skene* backdrop). *Amphitheatres* are Roman, oval, and held gladiator shows. We use the Greek theatre as the frame, and the amphitheatre only in Act IV's Colosseum.

### The title screen and loading screens

**The reference** is SMB3's title screen (screenshot supplied): a **black stage**, a **black-and-white checkerboard floor** with a bright line along its top edge, the hero **standing on the floor** at the bottom, a big **light-blue logo with a dark-blue offset shadow**, a fat orange **"3"** underneath, and a few **drifting icons** (a mushroom, a leaf, a star, and white **spiral clouds**). It is already a stage, which is why the theatre frame fits so well. We keep the *composition* and swap every element.

| SMB3 title | Our title |
|---|---|
| Black background | A **night-blue black** (`#0B0A14`), the dark of a theatre before the show |
| Checkerboard floor with a bright top line | A **black-and-white Roman floor mosaic** (a chessboard of tesserae, with a pale meander border along the top edge as the stage lip) |
| Hero standing on it | The **hero stands on the stage**: a random one of the four artists each time, drawn in their own medium |
| Logo in light blue with a dark-blue offset shadow | **MASKS & MOSAICS** in **light lapis tesserae** (face `#6FA8E8`) with a **deep lapis offset shadow** (`#1E3A8A`); each letter is built from visible tesserae with grout lines, and the shadow is shifted by one tile |
| Big orange "3" | A big gold **"III"** in gold tesserae (`#E2A93A`, shadow `#8A5A10`), set on a small marble plaque |
| Drifting mushroom, leaf, star | A drifting **Ambrosia Cake** (mushroom), a **laurel leaf** (the Leaf), and the **Aegis** (the star) |
| Spiral clouds | White **Minoan spirals** and **wave scrolls**, the Greek ornament for waves, drifting like the clouds |
| (Press Start, 1 or 2 players) | A **laurel-leaf cursor** and "**One Actor**" and "**Two Actors**" |

**Sequence:**
1. Black. A drone from the aulos.
2. The **curtain** (crimson, gold fringe) **rises** with a short aulos and drum fanfare.
3. The floor, the logo and the drifting icons are revealed. The hero walks on from the left, takes a **bow**, and stands.
4. Two **masks** (comedy and tragedy) hang at the upper corners and **blink** now and then.
5. After a pause, the **demo** plays: a Tessera Beetle wanders on and the hero stomps it into gold tesserae.
6. Tap or press Start to open the menu.

**Layout** (16:9, 960 by 540; keep the logo within the top third and the floor in the bottom fifth, so the phone's safe area and thumbs don't cover them):

```
   (mask)                      *aegis*                   (mask)
        ~spiral~     ___________________      ~spiral~
                    |  MASKS & MOSAICS  |
      cake*         |___________________|     *leaf
                           [ III ]
                         ~spiral~
                   hero
  ============================================================
  ##  ##  ##  ##  ##  ##  ##  ##  ##  ##  ##  ##  ##  ##  ##
    ##  ##  ##  ##  ##  ##  ##  ##  ##  ##  ##  ##  ##  ##
```

**Loading screens** (these double as the SMB3-style world intro):
- **Level load:** the curtain **falls**, the Chorus card shows the Act and Scene ("Act II, Scene 3: The Marine Style Seabed") and the **hero's mask**, and the curtain rises into the level. Short loads show only the mask.
- **Act intro:** the same black stage and checkerboard floor as the title, with the **Keeper (transformed)** standing on it, and a line from Thespis.
- **Game over:** the **tragedy mask** on the black stage; the Chorus gently turns it back into a comedy mask. Tap to continue.
- **Level clear:** the **Victor's Dais** card, with tesserae counted up.
- Every one of these reuses the one set of assets (the floor, the curtain, the masks), so it costs almost nothing.

**Mock-ups:** see [`LEVEL_MAPS.md`](LEVEL_MAPS.md) for pictures of the title screen, the loading card and the Act I map (`mockups/`).

**Build notes:** the logo is vector text run through the mosaic filter (`tools/mos/`), so we can change the title without redrawing. The checkerboard floor is a tiling pattern. The curtain is a simple animated vector. All four heroes' title poses come free from their normal idle sprites.

## 12. Audio

Same approach as the other games: our own **generated** music, no recordings. The repo already makes its audio from code (`tools/audio/make_audio.py`, with `synth.py`); we extend it.

- **Ancient music is largely lost.** We have a few written fragments (the Seikilos epitaph, 1st century AD, is the oldest complete tune we can read; the Delphic Hymns, 2nd century BC). We use the *modes* (Dorian, Phrygian, Lydian) and the *sound world* of ancient instruments, plus our own melodies, rather than reconstructions. If we quote the Seikilos tune, we transcribe it ourselves, since modern transcriptions and recordings can have their own rights.
- **Instruments** (synthesised): aulos (double reed, with a reedy tone and a drone), lyre and kithara (plucked: reuse the lute voice), frame drum and clappers, Egyptian arched harp, sistrum, hydraulis (water organ), and for Byzantium a chant-like drone with choral pads.
- **Per Act:** Nile (arched harp, sistrum, a steady pentatonic), Crete (flowing aulos and tambourine), Vases (a Dorian lyre march), Rome (hydraulis and brass-like fanfares), Olympus (high airy aulos with lyres), Etruria (a lively aulos and a drum, with a dark underlayer), Byzantium (drone and slow chant, with bells), Hoard (the Acts' tunes broken and out of order, then **re-assembling** as you recolour).
- **Theatre structure:** the title and Act cards use a **parodos** (entrance) tune, the map uses a **stasimon** (the chorus's interlude song), boss themes are the **agon** (contest), and the credits are the **exodos**.
- **Curtain cues:** the curtain rising and falling has its own short fanfares (aulos and drum), used as the "loading" cue.
- **Sound effects:** tesserae clinks for coins (a small stone click, pitched up per coin in a chain), pot clatter for the Oarsmen's throws, a lyre pluck for Chelone shells.

## 13. Tech notes

- **Same engine and pipeline as parts 1 and 2.** Unity 6 (`unity/`) and the code-driven art pipeline, where SVG drawings run through a style filter and Chromium renders them (`tools/emb/`). Proposed: a new `tools/mos/` with one filter per medium, and a shared "frame" set for the theatre and map:
  - **mosaic filter** (tessera grid with flowing rows and grout; **gold shimmer** by baking two or three tile-angle variants and cycling them);
  - **vase filter** (black glaze with incised lines, crazing; red-figure inversion for the Kiln flip);
  - **tomb filter** (flat fills, dark outline, plaster grain, register lines);
  - **fresco filter** (lime-plaster grain, soft cracks and lacunae for Crete, Rome, Etruria).
- **In-engine:**
  - One tilemap per Act; **the Kiln flip** is a second prebuilt tilemap and sprite set for black-figure and red-figure that we swap, rather than live inversion.
  - A **per-Act post-process** applied to background layers only (gold shimmer, lamp-light mask in Act VI, plaster grain).
  - **Registers** are normal tilemap rows with one-way platforms and a tag, so they cost nothing special.
  - **Vase wrap** is a level whose left and right edges connect (camera wraps).
  - **Water flow** (Act IV) is a tilemap of flow vectors that moves the player and items, the same primitive we use for Act II currents.
  - **Lamp light** is a mask over background and surface layers only; the gameplay layer and hazards are drawn on top with a lit rim.
  - **Suits** are data: one controller with per-suit abilities (glide, swim, flight, statue, mallet), so adding a suit is not a new controller.
- **Reusable from this repo:** touch controls with see-through UI, level briefings, the gallery screen, audio generation, and the level generators (`tools/unity/make_levels.py`, `tools/unity/preview_level.py`).
- **New systems:** the winged meter and flight, glide, statue pose, swimming (a stronger version of part 1's), the Great Floor map with wandering Satyrs and shrines, Vase Match, the Capsa inventory, Kiln flip, registers, water flow and valves, lamp light, whitewash scraping, ship levels with autoscroll, and the theatre frame (curtain, Chorus, masks).
- **Art budget.** Each Act needs about 3 to 5 enemy types, 2 bosses (mini and main), tiles, 2 to 3 background layers, and a few items. Mosaic Acts (VII, VIII, the map) are cheapest per sprite; tomb and vase Acts are cheap too (flat colour); fresco Acts (II, IV, VI) are the most work.

## 14. Legal and sourcing

*This is a planning note, not legal advice. Get a lawyer's view before release, especially on the points marked with a flag.*

- **Antiquity is in the public domain.** The works themselves, from the Egyptian tomb painters to the Ravenna mosaicists, are thousands of years old. Style and composition are free.
- **Redraw, don't trace.** As in parts 1 and 2, we **redraw everything in our own style** and do not trace museum photographs, 3D scans or catalogue images, which can carry their own rights or contractual limits. Many large museums offer open-access images, but even then we redraw. We do not use scans of objects such as the Nefertiti bust.
- **Restorations count as modern work.** Many "ancient" pictures are heavily restored. The Knossos frescoes in particular were reconstructed in the early 20th century by Arthur Evans's team, with painters including **Émile Gilliéron (father and son)**. Their restorations are *modern art*. I understand both artists died before 1956 (the father in 1924, the son in 1939), but please verify. Safest: draw from the *motifs* (bull-leaping, dolphins), not copy a specific restoration.
- **Flag: heritage-image laws.** Some countries restrict commercial use of images of state-owned heritage:
  - **Italy** (the Cultural Heritage Code, articles 107 and 108) lets state museums and sites require authorisation and fees for commercial reproductions of their cultural goods, and Italian courts have enforced this (the cases around Michelangelo's *David* and Leonardo's *Vitruvian Man* are the well-known ones). This touches **Act IV (Pompeii, the Alexander Mosaic in Naples), Act VI (Tarquinia) and Act VII (Ravenna)**.
  - **Greece** has its own rules on the commercial use of images of ancient monuments (check the current position for Knossos and Akrotiri).
  - **Egypt** has periodically proposed fees for commercial use of ancient monuments. Check what is in force.
  - **How we reduce risk:** the Acts are *inspired by* these works, not reproductions. We invent our own compositions in the style, make our own figures, and cite sources in the gallery cards. Do a proper check before launch, particularly before selling in Italy.
- **Invented hieroglyphs.** Real hieroglyphs are a script, and misspelled ones are a small but real embarrassment. Either use a short, **checked** real inscription (a name, "life, prosperity, health") with an Egyptology review, or use clearly decorative pseudo-glyphs. The default: **decorative pseudo-glyphs**, flagged as such.
- **Religion.** Ancient gods (Zeus, Hades, the Egyptian deities) are fair game for gentle comedy, but nothing sacred to a living religion is mocked. Byzantine mosaics are religious images in a living tradition: we draw our own figures and never make a sacred figure a hazard, enemy or smash target. The Great Vandal's backstory is fictional.
- **Distance from SMB3.** Game mechanics (running, stomping, suits that give flight or swimming) are not protected, but **characters, art, sounds, music, level layouts and names are**. We do not reuse any, and we **check names against Nintendo's** (hence the caution on *Odyssey* and *Wonder* in section 0). Our suits and enemies are original ancient-themed designs, not recolours.
- **Remains.** Mummies appear only as comic **Bandage Scamps**; nothing realistic.

## 15. Platforms and business

Same plan as parts 1 and 2 (see `docs/painters/DESIGN.md`, section 16):

- **Phone first** (iOS and Android), then **PC on Steam** (and Steam Deck), then consoles if it earns it. Engine: Unity.
- **Business model: first Act free (the Nile), one unlock purchase** for the rest, no ads, as in the other two games.
- **Phone-first design consequences:** a stick (full push = run) plus **Jump** and **Action**; levels of **3 to 5 minutes** with braziers as checkpoints; see-through controls; and gamepad and keyboard layouts planned from the start.
- **HUD and controls:** see [`HUD.md`](HUD.md) for the phone-size layout, button sizes, states and options.
- **SMB3-specific phone worries:** flight (tap Jump repeatedly) must be forgiving, and the 28-slot Capsa must be easy to scroll with a thumb. Playtest both early.

## 16. Roadmap (order, not dates)

1. **Greybox**: one artist, SMB3 movement (run with the winged meter, stomp, shell kick, block bounce), a flat test level. Check that it feels good on a phone.
2. **Filter spikes**: run three sample sprites each through the mosaic, vase, tomb and fresco filters and **look at them on a phone at arm's length**. Gate: if gold, black-figure or dark-tomb sprites aren't readable, fix the rules (section 11) before building more.
3. **Vertical slice**: scene I-1 (*The Marsh of Nebamun*) in tomb style with the theatre-curtain intro, one Muse Shrine, the Victor's Dais and one pithos.
4. **Act I complete**: all seven scenes (including the Causeway and the Shaduf Barges), the Scribe's Tomb and the Sphinx; Laurel Wreath glide; first run of the Great Floor map with one Satyr.
5. **Second style**: Act II (Crete), to prove the filter pipeline generalises: swimming, Conch, bull vaults, the Minotaur.
6. **The map and meta game**: Muse Shrines, Vase Match, the Capsa, Urania's Star Chart. These systems can be built alongside Acts I and II.
7. **Acts III to VII** in order, adding one big system each (Kiln flip and Hydra's fire; water flow and valves; Winged Sandals flight and ship autoscroll; Lamp light; whitewash scraping).
8. **Act VIII and the ending**, then music pass, gallery extras and polish.

**Gallery extras.** In the part 2 style: a gallery card per hero, per Act and per source work, saying where the *real* object lives, with a note on invented faces and restorations.

**Scope warning.** 41 levels is a lot: part 1 shipped 29 levels in 7 worlds and part 2 plans 20. The fallback if production slips is to merge each Act's fortress and ship into one (II to VII), which brings the total to 35, with Act I kept whole as the free sample. Act VII can also launch as an update.

## 17. Stretch ideas and alternatives

- **Two-player co-op** (SMB3's alternating two-player became a real co-op), with a shared Winged meter.
- **A "Mosaic Maker"**: build and share your own mosaic level (tile-based, so very natural).
- **More Acts:** Mesopotamia (Assyrian palace reliefs, lamassu, the Ishtar Gate), Persia (Persepolis), Nubia, the Indus, Roman Britain.
- **Daily challenge** levels as Vase Match or Winged Meter runs.
- **Alternatives for part 3 (and part 4)**, each fitting a different Mario game:
  - **Renaissance and Baroque, framed as theatre.** Stage settings, Palladio's theatre in Vicenza, Baroque ceilings with figures bursting through the frame (quadratura), Caravaggio's spotlights (a ready-made lamp-light mechanic). Strong for a light-based game.
  - **The Silk Road.** Persian miniatures, Chinese hand-scrolls (a level that *is* a long horizontal scroll), Japanese folding screens (a screen that opens like a level), Indian court painting. Very natural for a horizontally scrolling game, and many of the works are public domain.
  - **Islamic geometric art** and **Mesoamerican codices** are rich and underused, though tricky on cultural-sensitivity grounds.

## 18. Decisions

**From the brief**
1. **Part 3 is SMB3-style, set in antiquity.** Egypt, Crete, Greece, Rome, Etruria, Byzantium; mosaics, vases, tomb paintings and frescoes; all public domain.
2. **Theatre staging.** A Greek theatre with masks and a curtain opens the game; the world map is a Roman floor mosaic.
3. **Suits:** Laurel Wreath, Winged Sandals, Marble Statue, Triton's Conch, Sculptor's Mallet and Chisel, Giant Sandal, Orpheus's Lyre.
4. **Map features:** wandering Satyrs, Muse Shrines, vase-matching, ship battle plus Wonder or monster boss per Act.
5. **Eight worlds** as listed in section 9, with the Great Vandal as the villain.
6. **Tone:** gentle and whimsical. **Platforms:** phone first, then PC. **Engine:** Unity, with the code-driven art pipeline.

**Confirmed after review** (all recommendations accepted)
1. **Title:** *Masks & Mosaics*, in the *Into the Frame* series. (A store and trademark search is still to do.)
2. **The Great Vandal** is a tidy-minded Sweeper with a maul and broom, who leaves one tessera out of place at the end.
3. **Four heroes with invented faces:** Exekias, Iaia, Thutmose and Sosus. Iaia's gallery card says how little survives about her.
4. **Scope:** keep the full set, with the fallback in section 16 if production slips.
5. **Ship battle then boss** in every Act.
6. **Hieroglyphs** are decorative pseudo-glyphs, flagged as such.
7. **Heritage-image laws:** Acts IV, VI and VII are "inspired by", with no copies of specific frescoes. A lawyer's view is needed before launch, especially before selling in Italy.
8. **No cameos** of parts 1 and 2.
9. **Where it lives:** this document stays in `docs/antiquity/` until the repo is decided.
10. **Business model:** first Act free (the Nile), one unlock purchase, no ads.
11. **Act I has seven scenes**, making the total **41 levels**. (This supersedes the 39 in the scope answer: 39 is the count for the other Acts plus a five-scene Act I.)
12. **All life-farms are capped.**
13. **Transformed Keepers** (Ovid-style) in every Act.
14. **Title screen** shows a random artist each time.
15. **Scene bank** stays as stretch content; nothing is promoted beyond the two Act I scenes in decision 11.

**Still open**
- Which repo and account will hold the project (decision 9).
- The store and trademark search for the title.
- Legal review (decision 7), and verification of the ancient-art facts and dates in this document before release.
