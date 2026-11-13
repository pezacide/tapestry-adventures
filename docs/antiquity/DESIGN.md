# Working title: *Masks & Mosaics*

*A Super Mario Bros. 3 style platformer staged as a play in antiquity: Egyptian tomb walls, Minoan frescoes, Greek vases, Roman floors, Etruscan tombs and Byzantine gold, all drawn as the art they come from.*

Status: **planning only**. Nothing here is built. This is part 3 of a series. It lives in `docs/antiquity/` until we decide where it goes (see Open questions), alongside `docs/painters/DESIGN.md` (part 2) and the tapestry game in the rest of this repo (part 1).

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
- **Cameos.** Each part may hide a few cameos of the others (a stitched banner on a villa wall, a grey frame in a tomb). They are jokes, not requirements.

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
| 88 levels | **39 levels** across 8 Acts (section 9). Part 1 is 29, part 2 is 20 |
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

**39 levels** in 8 Acts: each of Acts I to VII has **three scenes, a fortress with a mini-boss, and a ship battle that leads to a Wonder or monster boss**; Act VIII has four. Order mirrors SMB3's biomes (the user's direction, loosely): Desert, Water, Giant, Pipe, Sky, Dark, a polished-gold "ice" world, and the final world.

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
| I-4 | *The Scribe's Tomb* (fortress) | A dark tomb of brick, hieroglyph-ish walls (invented glyphs, see section 14) | Bandage Scamps; **mini-boss Khepri** rolls the sun-ball: hop it, then stomp him three times |
| I-5 | *Barque of Millions of Years* | A scrolling **solar barque** battle (the sun-boat of Ra) as a ship level | Oarsmen toss pots; then **boss: the Sphinx** (below) |

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

### More SMB3 ideas worth borrowing

Added after reading the Super Mario Wiki's page on SMB3. Each is mapped to our setting; none are scheduled yet.

**Level furniture** (cheap to build, and each has an ancient version):

| SMB3 | Ours | Act |
|---|---|---|
| Switch Block (bricks become coins) | **Kiln Switch** also turns mudbricks into gold tesserae for a few seconds, and reveals hidden "blue" tesserae (lapis) | all |
| Jump (note) block | **Aulos block**: a bouncy note block; time your jump for height | all |
| Magic Note Block to Coin Heaven | An aulos block that carries you to a cloud with a tessera shower | V |
| Vine block | **Grapevine block**: hit it and a vine grows up (Dionysus) | III |
| Ice block, frozen coin | **Marble block** that slides; a tessera "frozen" in glass melts by Oil Lamp | VII |
| Conveyor, bolt lift, donut lift, rotary lift | **Treadmill** (a Roman treadwheel crane), **screw-pump** (Archimedes' screw) lifts, **crumbling plinths**, **spinning millstones** | II, IV, V |
| Tide (rising and falling water) | **The Nile flood** (Inundation) raising and lowering the water in Act I; also Act II | I, II |
| Bubble-jet pipes (currents underwater) | **Dolphin-jet** and fountain jets | II |
| Waterfalls you can swim up | Same, in Act II | II |
| White block (crouch to step behind) | **Painted-door panel**: crouch on it to step into the wall painting | VI |
| Magic Door (changes your size) | **Mirror door** in Iaia's honour: step through and your reflection leads, mirrored controls for one room (used once) | VI |
| Warp Door in fortresses | **False door** (already in Act VI), and **Labyrinth** doors in Act II | II, VI |
| Brick floor breaks under Bowser | The Vandal's **tessera floor** (section 10) | VIII |

**Enemy variants worth copying** (SMB3 has families, not single enemies):
- **Satyr family** (SMB3's Hammer, Boomerang, Fire and Sledge Brothers): a Satyr with a wineskin (hammer analogue), one with a **discus** (boomerang), one with a **torch** (fire; rare), and a large **Silenus** (sledge: stuns when he lands).
- **Giant variants** for Act III of every common enemy (a giant Tessera Beetle, giant Chelone), as in Giant Land.
- **Muncher** (invincible plant) becomes a **thistle**; the Kiln Switch removes it, as in SMB3.
- **Chain Chomp breaks free** after a long time: the Cave Canem Hound slips its chain if you linger.
- **Boo** only moves when you're not looking: our **Shades** (already in).
- **Roto-Disc** that only a Statue can stop: a **spinning shield-disc** that Thutmose's statue form can pass.
- **Boss Bass / Big Bertha**: a **giant fish** that lunges from the water in Act II.
- **Jelectro** (stationary electric jellyfish): a **stinging jellyfish** in Act II, a pure hazard.
- **Hot Foot, Podoboo** (lava fireballs): hot **cinders** in the Kiln and a leaping fire in the Hoard.
- **Tornado** pushes you back: a **dust whirlwind** (a haboob) in Act I.

**Level types SMB3 has and we haven't used:**
- **Tank levels, Battleships and Hand Traps** (SMB3's world 8): our Act VIII fleet scene (VIII-3) can use *tank* analogues (siege towers and war elephants as moving platforms) and a **hand-trap** room (statues' stone hands that grab at you) before the lair.
- **Towers** (SMB3 has a tower in some worlds): Act V's Atlas climb is ours.
- **Coin Ship and bonus courses:** a **treasure ship** that appears on the map for a one-screen bonus, and a **Diogenes pithos** as a bonus-room door.
- **Warp Zone:** Urania's Star Chart takes you to a **Zodiac Room** of nodes labelled with constellations, one per Act (and one deliberately blank).
- **Map items beyond the suits:** SMB3's **hammer** (breaks map boulders and reveals shortcuts) becomes the **Chisel** item for the map; **Jugem's Cloud** (skips a level) becomes **Nephele's cloud**; the **Anchor** (stops an airship from leaving) becomes the **Sea Anchor** that keeps a Satyr's boat from fleeing.

**Rules and feel:**
- **Hit rules.** In SMB3's international version a hit suit drops to Super form first, then small, with a puff of smoke; the Japanese version drops straight to small. We use the gentler international rule, in line with the tone: a hit suit becomes the "grown" form (with a *poof* of marble dust), and a hit "grown" artist shrinks.
- **Carry and kick.** SMB3 lets you pick up and carry shells and throw them. Chelone shells and **marble blocks** are carry-and-throw objects.
- **Giant Sandal doesn't replace your suit** (as in SMB3's Goomba's Shoe): it is worn *over* it.
- **Goal roulette:** hitting the same icon panel three times in a row gives the extra-life fireworks (our Victor's Dais does the same).
- **Board-game map.** The wiki notes the SMB3 map was reportedly modelled on a board game. Our **Great Floor** is literally a game board (the Romans played board games on floors and steps), so **Satyrs can be represented as game pieces**.
- **Battle Mode** (two players who touch on the map fight): a **Duel on the Dais**, a versus mini-game for two players in a one-screen arena, and the natural first piece of the co-op stretch idea (section 17).
- **Development lesson.** SMB3 began as a Zelda-like overhead view and switched to side-on because judging jumps was hard. Our map is top-down but the levels are side-on; keep it that way, and do not put a top-down level in the main route.
- **Level types named on the wiki's navigation:** *Ghost House*, *Pyramid*, *Mini-Fortress*, *Enemy Course* and *Treasure Ship*. Ours: a **Shade House** (a small Act VI or Act IV room where Shades move only when you look away; one per Act at most), a real **Pyramid** interior in Act I (SMB3's has ceiling-walking Buzzy Beetles, so ours has **Tessera Beetles on the ceiling** that drop when you pass), a **half-length fortress** on the map, **Enemy Courses** (the one-screen Satyr fights), and a **Treasure Ship** bonus (already listed).
- **Climb out of water.** The manual calls out jumping out of water as new in SMB3. Our swimming should let you **leap from the water onto a ledge** cleanly; it matters a lot on a phone.
- **Ending beat.** The wiki notes SMB3's ending has a praying Koopa. We have a similar quiet joke already: the Vandal's front-row seat and the mouse on the last tile.
- **Secret reward.** After the credits SMB3 starts a new game with every P-Wing. Ours can give **the Winged Sandals in the Capsa** as a replay reward.

### Act I as the free tutorial: lessons from SMB3's World 1

Act I is the free Act, so it is the first thing every player sees. I read the Super Mario Wiki pages for SMB3's World 1 (levels 1-1 to 1-6, the fortress, the airship and Larry) and took what they teach about **pacing a first world**. The wiki's difficulty ratings (from the Japanese manga guide) climb gently: 1-1 is 2/10, 1-2 and 1-3 are 3/10, 1-5 and 1-6 are 4/10, 1-4 and the fortress are 5/10.

**What World 1 does, and our version of each beat**

| SMB3 World 1 | What it teaches | Our version (Act I) |
|---|---|---|
| **1-1**: semisolid platforms, a Venus Fire Trap in a pipe, a Super Leaf in a block you can only open by throwing a shell at it | Safe first contact; the shell-as-tool idea; power-up in place | **I-1 Marsh of Nebamun.** The first amphora block sits on the ground, so you need a kicked Chelone shell to open it. It holds the **Laurel Wreath** |
| 1-1 secret: run up, fly to a cloud area with a 1-Up, then to a bonus room whose coins form a "3" | Vertical exploration; flight shows a level is taller than it looks | A **sky-marsh of birds** above the reeds, reached by a run-up **glide** off a high reed, with tesserae spelling a **Roman numeral** |
| 1-1 P Switch hidden under a brick stack | Rewards noticing an out-of-the-way block | A **Kiln Switch** under a mudbrick stack turns bricks to tesserae |
| **1-2**: pipes that spawn endless enemies, a hidden switch route, Super Star in the last jump block | Low-stakes warm-up; a secret that changes the route | **Pithos nests** spawn Tessera Beetles. **Decision: cap them** (SMB3's spawners allow farming lives); each pithos spawns at most a few, then goes quiet |
| **1-3**: a long flat plain, boomerang thrower, a **hidden note block** to Coin Heaven, side-bumping for a hidden power-up, and the **white block**: crouch on it and you drop behind the scenery to find a Toad House with the **Magic Whistle** | Off-path secrets; side-bumping; background areas; a skill check (crouching) | **I-2 Harvest Registers** hides an **aulos block** to a tessera cloud, a **painted-door panel** (crouch to step into the wall) and, behind it, **Urania's Star Chart**: the same early-warp gift SMB3 hides in its first world |
| **1-4**: first **autoscroll**; lifts that drop when you land; bricks that can push you off | Momentum; scroll pressure; shells break bricks; a **coin threshold** (44 coins) makes a P-Wing house appear on the map | **I-3 Sands of Giza** gains an **autoscroll causeway** section of crumbling stones. If you collect all its tesserae, a **gold shrine** appears on the Great Floor with a Winged Sandals item |
| **1-5**: opens with a slope slide into a pool; a **fork** (above-ground path with a hidden note block, underground path of pools and a power-up) that **reconverges**; Piranha Plants facing different ways | Slide and swim before enemies; fork without a second difficulty curve | A **dune slide into the Nile** at the start of a scene; a fork between the bank and the riverbed; **Acanthus Snappers** (or reed snappers here) in pots facing up, down and sideways |
| **1-6**: island gaps, rail-mounted lifts, bounce blocks, wire-hung platforms, an erratic track; some lifts fall at the end of their track | Introduce moving platforms gradually; make risky ones look different | A **shaduf** (the Egyptian counterweight water-lift) as a **see-saw platform**, **rope-and-pulley barges** on the river, and one erratic barge that **frays** at the end (rope fibres show it) |
| **Fortress**: Podoboos that leap between gaps, **Roto-Discs**, a Dry Bones, a **moving spiked ceiling** you must duck, a rising door; Boom Boom's room; defeating him destroys the fortress and **opens the map lock** | One new hazard at a time before combinations; timing taught through visible motion | **I-4 The Scribe's Tomb**: leaping **fire-cobras** (uraeus) from pots, spinning **sun-discs** (Roto-Disc analogue), a **Bandage Scamp**, a **descending spiked ceiling**; beating **Khepri** destroys the tomb door and **opens the Sphinx Gate** on the map |
| Fortress secret: fill the P-meter, fly into the ceiling, reach a treasure chest with the **Whistle**; taking it **ends the fortress early but leaves the lock shut** | A power-up-gated secret with a **tradeoff** | A hidden room in the tomb with a second **Star Chart** copy: it ends the tomb at once but **doesn't open the Gate**, so you must still come back |
| **Airship**: the Toad says the king's wand was stolen and he was **transformed**; autoscroll under cannon fire; crushed by the screen edge is fatal; a quartet cannon at the midpoint; the power-up block gives a Fire Flower if you are already powered up | Cannon placement is the whole challenge; steady forward pressure | **I-5 Barque of Millions of Years** (section 9). Cannons are **pot-lobbing catapults** and Oarsmen. The block at the midpoint gives a **Lamp** if you're already grown |
| **Larry**: three beats: a magic blast, a shell retreat and chase after a stomp, another blast; three stomps | A readable boss pattern to copy | Our **boss template**, used for Act bosses: *attack, expose, chase*, three hits (the Sphinx and others vary it) |

**Rules to copy**
- **Power-up substitution.** A power-up block gives a Mushroom-equivalent (an Ambrosia Cake) if you are small, and the **next item up** (a Laurel Wreath or a Lamp) if you are already grown. Rewards stay meaningful at every stage.
- **Rewards on a small skill check.** Hidden things are found by side-bumping, crouching on a special block or a run-up, not by guessing.
- **Coin thresholds unlock map rewards** (as the 1-4 P-Wing house does). We use tessera thresholds, and a gold shrine appears.
- **A shortcut costs something** (the Whistle that skips the fortress but leaves the lock shut).
- **The first Act's difficulty curve:** I-1 2, I-2 3, I-3 3 to 4, I-4 5, I-5 5. Playtest against this.
- **A cheap beginning.** SMB3's 1-1 has 57 coins, 3 Mushrooms and 3 Leaves, so a first-timer is rarely short of items. Ours should be generous.

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

**Act I might deserve more.** SMB3's World 1 has six levels, a fortress and an airship; ours has three scenes, a fortress and a ship. Since Act I is the free sample, consider giving it two more scenes: **I-3b *The Collapsing Causeway*** (the autoscroll level) and **I-6 *The Shaduf Barges*** (the lift level). That makes **41 levels**. See Open questions.

### What the other SMB3 worlds teach (Worlds 2 to 8)

The same exercise for the rest of the game. I read the wiki's pages for **World 2's levels** (2-1 to 2-5, the Fortress, the Airship, Morton) and its overview pages for **Water, Giant, Sky, Ice, Pipe and Dark Land**. The overview pages give each level a line or two, so those Acts get less detail than Act I. Two gaps: the wiki's 2-Pyramid page came back with a short summary only, and I did not read the Koopalings' pages past the first 100,000 characters.

**Which SMB3 world is which Act.** Our order follows the biomes in the brief, not SMB3's order, and we have no Grass Land (Act I's tutorial pacing comes from SMB3's World 1, above):

| SMB3 world | Biome | Our Act |
|---|---|---|
| 1 Grass Land | tutorial pacing | **Act I**, pacing only |
| 2 Desert Land | desert | **Act I** (Nile), content |
| 3 Water Land | water | **Act II** (Crete) |
| 4 Giant Land | giant | **Act III** (Heroes' Vases) |
| 7 Pipe Land | pipes | **Act IV** (Rome) |
| 5 Sky Land | sky | **Act V** (Olympus) |
| 8 Dark Land (fortress, door mazes) | dark | **Act VI** (Etruscan tombs) |
| 6 Ice Land | slippery | **Act VII** (Byzantium) |
| 8 Dark Land (tanks, fleet, final castle) | final | **Act VIII** (the Hoard) |

#### Act I, desert content (SMB3 World 2)

| SMB3 World 2 | Our version |
|---|---|
| **2-1**: hidden **Pile Driver Micro-Goombas** under brick stacks; Firesnakes on the pipes; an early Starman; a hidden switch room reached by flying | **Mudbrick Mimics**: little scarabs that hide inside a mudbrick stack and pop out as you pass. **Fire-cobras** hop on the reed pots |
| **2-2**: **quicksand** with Venus Fire Traps inside it; an **oasis** with Cheep-Cheeps in the water, crossed on a lift; collect 30 coins and a hidden switch opens a map house with an **Anchor** | **Quicksand with Snappers** that lunge out; an **oasis** crossed on a **reed raft** while fish leap; 30 tesserae plus a hidden Kiln Switch makes a **shrine** appear on the map |
| **2-3**: three **pyramids** in a row, first semisolid, then brick; hidden P Switch; **kick a shell into the bricks** to uncover the exit pipe | Three **stepped pyramids** drawn in different materials (sandstone, mudbrick); the exit is hidden behind a brick wall that a **kicked Chelone shell** opens |
| **2-4**: a **lower and upper route** (the upper needs flight); **128 coins** | The upper route needs a Laurel run-up glide; a tessera-rich high road |
| **2-5**: **Chain Chomps** are the main hazard; a vine grown by kicking a shell into a gap | Chained **jackals** (the Egyptian version of the Chain Chomp; Rome gets the Cave Canem hound) and a **grapevine** (or papyrus stalk) grown by a shell |
| **2-Pyramid**: an underground **maze** where you use **Buzzy Beetle shells to break walls** and beetles walk on the ceiling | **The Great Pyramid maze** (optional scene): crawl the corridors, break a sealed wall with a shell, and watch for **ceiling Beetles** |
| **Fortress**: Thwomps, Dry Bones, the first **Boos** (they appear after the second Thwomp), rising and falling **spikes**; three stomps on Boom Boom opens a map door; stomping the Dry Bones is an **infinite-lives trick** | Falling **column drums** (Thwomps) in the Scribe's Tomb, Bandage Scamps that **reassemble** (cap this, as with the spawner pipes), and the first **Shade**; Khepri takes three stomps |
| **Airship**: cannon corridors, **Rocky Wrenches** appear for the first time near the end, a single power-up at the midpoint, a tall post with crates around it before the boss | **Oarsmen** pop out of the barque's oar ports for the first time here; a post-and-crate climb before the Sphinx |
| **Morton**: the same pattern as Larry, but **slower and with more wand shots** | **Boss progression**: each Act's boss reuses the previous boss's pattern with one twist (below) |

Also from World 2: **Angry Sun** returns later in World 8, so our **Aten** returns in Act VIII, as a callback.

#### Act II, Crete (SMB3 World 3: Water)

| SMB3 World 3 | Our version |
|---|---|
| **3-1** underwater with Bloobers; **3-5** Big Berthas and Jelectros; a fully **underwater fortress** with Stretches; **3-9** mixes above-water and underwater | Swimming levels in the **Dolphin Hall** and **Marine Style Seabed**, with Octopus, Jellyfish and Giant Fish. A **flooded Labyrinth** variant. A scene that **alternates shore and sea** |
| **3-3** and **3-8**: the **water level rises and falls** (and Boss Bass lunges) | **The tide**: the Minoan sea rises and falls in a shore scene. The Giant Fish rises with it |
| **3-6** autoscroll with donut lifts | A **raft-and-crate** autoscroll, with crates that sink |
| **First fortress**: **find the right door** to reach Boom Boom | **The Labyrinth**: Ariadne's thread shows the right door; wrong doors loop you back |
| **Airship**: **diagonal cannons** (Wendy) | Talos's galley fires **rocks in diagonals** |
| **Map**: green **bridges toggle** every time you finish a course or lose a life; a **rock broken with the Hammer yields a Canoe** to an island with houses; the **Hammer Bros** fight changes between land and water | The Great Floor's **tide bridges** rise and sink in turn; the Chisel breaks a rock to free a **dolphin** that ferries you to a **Muse island**; Satyr fights alternate between **land and a raft** |

#### Act III, the Heroes' Vases (SMB3 World 4: Giant)

| SMB3 World 4 | Our version |
|---|---|
| **4-1** introduces **oversized enemies**; **4-2** has large pipes and **leaping Cheep-Cheeps** | Giant Tessera Beetles and Chelone; **giant pithoi** as platforms; leaping **fish painted on the vase** |
| **4-5**: **Missile Bills** (homing) | **Homing discs** thrown by a Discobolus |
| **4-6**: **Magic Doors** that change the size of enemies and obstacles | **Kiln doors** painted on a vase that **swap scale**: you become the giant and the world shrinks. Gives us a size-change mechanic for exactly one scene |
| **Fortresses**: **sliding Thwomps**, **Hot Feet**, donut lifts | Sliding **column drums**; **embers** that creep only when your back is turned (our Hot Foot); crumbling plinths |
| **Sledge Brothers** (giant Hammer Bros that stun when they land) on the map, giving a Starman, a cloud or a P-Wing | **Silenus** (the big Satyr) fights, giving an Aegis, a Nephele's cloud or Winged Sandals |
| The map **looks like a giant turtle** | The Act III island is a **tortoise**: *chelone*, and the animal Hermes made the lyre from |

#### Act IV, Rome (SMB3 World 7: Pipe)

| SMB3 World 7 | Our version |
|---|---|
| **7-1** and **7-6**: **vertical stages** climbed through pipes; **directional lifts** | **Aqueduct climbs**: vertical scenes through lead pipes; **valve-driven lifts** that change direction when you turn a tap |
| **7-5** and **7-9**: **pipe mazes** | **The Cloaca Maxima**: Rome's great sewer as a maze of channels (a real monument, drawn gently: it is a *clean* water-channel, a cartoon, with no sewage) |
| Two small **Piranha Plant stages** (a whole level of plants), and **7-8** with many types | **The Garden Room**: a short level that is a Roman garden fresco full of **Acanthus Snappers** of different kinds |
| The **first fortress has no enemies**, only a room of bricks to turn into coins | **The Treasury** in the Hypogeum: no enemies, a quiet puzzle |
| **7-7**: cross a field of **Munchers** using a **Starman** | **The Thistle Run**: cross a field of thistles using the **Aegis** |
| **7-4**: autoscrolling **underwater** level | **The Tiber Flume**: an autoscroll down a water-channel |
| Second fortress: **pipe platforms over lava** | **Hypogeum lifts over Vesuvius's lava** |
| Piranha Plants on the **map** | **Acanthus** growing across the Great Floor's paths |

#### Act V, Olympus (SMB3 World 5: Sky)

| SMB3 World 5 | Our version |
|---|---|
| **Tower**: a bridge between **ground and sky**; a pipe in the clouds drops you to a short descending level | **Pelion upon Ossa**: the giants Otus and Ephialtes piled one mountain on another to reach Olympus. A tower climb from the earth to the clouds, then a short descent |
| **5-3**: **Goomba's Shoe** debuts | Giant Sandal returns for one scene |
| **5-4, 5-5**: first sky level, **rotary** and **donut lifts** | Cloud platforms that **rotate**, **drifting cloud puffs** that sink when stood on |
| **5-6**: **Para-Beetles as moving platforms** | **Pegasus and winged horses** you ride between platforms |
| **5-9**: **diagonal autoscroll**, Fire Chomps | A diagonal climb up the **Chariot of Helios** path, chased by the **Chimera's** fire head |
| **Second fortress**: broken platforms **between lava above and below** | Atlas's climb with **sun-fire** over and under |
| **Map**: ground path and sky path; the **airship** crosses between; **Star spaces**; the sky **Hammer Bros** leave a **Music Box** | Great Floor clouds above the ground map; **constellation spaces**; the sky Satyr leaves **Orpheus's Lyre** |

#### Act VI, Etruscan Tombs (SMB3 World 8: the dark maze)

| SMB3 World 8 | Our version |
|---|---|
| **Fortress**: a maze of doors and traps | **The Tomb of Orcus**: false doors, and a **correct-door maze** |
| **Bowser Statues** firing lasers, **Roto-Discs**, **Hot Feet**, Thwomps | **Stone lions** (Etruscan tomb guardians) with glowing eyes, and **spinning shield-discs** (Roto-Disc analogue). Shades in place of Hot Feet |
| **Hand Traps** (rooms packed with traps) | **Charun's workshop**: a trap room of hammers and falling slabs |

#### Act VII, Byzantium (SMB3 World 6: Ice)

| SMB3 World 6 | Our version |
|---|---|
| **6-1** icy ground, **Ptooies** (a plant that blows a spiked ball) | **Polished gold floors** (slippery); a **peacock** that spits a **feather-ball** |
| **6-2, 6-7**: autoscrolls over **moving clouds** and **donut lifts** | Autoscroll across **floating mosaic fragments** and **swinging chandeliers** |
| **6-3, 6-4**: **small ice blocks** and **rotary lifts** on ice pillars | **Marble blocks** that slide; rotating **icon discs** |
| **6-10**: **Frozen Coins** and **White Blocks** | **Tesserae sealed in glass** that melt by Lamp; Mallet scrapes to reveal |
| **Third fortress**: **conveyor belts** and Stretches | A **treadwheel** hall where the floor runs under you |
| **Airship**: **bolt lifts** (Lemmy) | **The Dromon** with rising and sinking oar platforms |
| Three Hammer Bro sets | Three **Whitewasher** squads on the map |

#### Act VIII, the Hoard (SMB3 World 8: tanks, fleet and castle)

| SMB3 World 8 | Our version |
|---|---|
| **Big Tanks** (autoscroll, cannons that fire cannonballs and Bob-ombs) | **Siege towers and war elephants** as moving platforms; the cannons are pot-lobbers and **Pot Bombs** |
| **Battleships** | A fleet of **triremes** (VIII-3) |
| **Hand Traps** | **Statue hands** that grab at you, in a room of the Whitewashed Gallery |
| **8-2**: the **Angry Sun** returns | The **Aten** returns from Act I, as a callback |
| **Super Tank** (a huge, heavily armed tank) | **The Trojan Horse**: a giant wooden horse on wheels, loaded with the Vandal's Hoard, that you cross on top of while it fires |
| **Castle of Koopa**: statue lasers, Roto-Discs, Thwomps, Hot Feet, then **Bowser** | **The Unswept Floor**: callbacks of every Act's hazard in order, then the Vandal |

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

#### Scene bank (optional extras)

If 39 levels feels too few for the main route and the ideas above are too good to waste, these are the bonus scenes, **not** counted in the 39: *The Great Pyramid* (I), *The Collapsing Causeway* (I), *The Shaduf Barges* (I), *The Flooded Labyrinth* (II), *Kiln Doors* (III), *The Cloaca Maxima* (IV), *The Garden Room* (IV), *The Tiber Flume* (IV), *Pelion upon Ossa* (V), *The Treadwheel Hall* (VII), *Charun's Workshop* (VI), *The Trojan Horse* (VIII).

### Level by level: ideas from the rest of SMB3's level pages

This pass covers the wiki's individual pages for Worlds 3 to 8: every numbered level, the fortresses, the airships, the World 8 special levels, and the Koopaling and Bowser fights. Each row below is a **specific SMB3 level** and what we could take from it. They are menus of ideas, not a commitment to build all of them.

**Pages I could not read:** 5-9, 6-10, the World 5 tower, the second Piranha Plant level, and Bowser's own page (the Castle of Koopa page did describe the fight). The Koopaling pages only gave me their first 100,000 characters, so boss details are what those first sections said.

#### Act II, Crete (SMB3 World 3)

| SMB3 level | What it does | Our scene idea |
|---|---|---|
| **3-1** | Underwater, bubble-jet pipes, Bloobers; a **split** (upper path has a Blooper Nanny, lower has bouncing blocks and a 1-Up) | **Dolphin Hall**: a mother **octopus** with her little ones (the Nanny); a fork where the upper path is risky and the lower is bouncy |
| **3-2** | Zig-zag **rail lifts** over jumping Cheep-Cheeps; a Starman hidden in an L of wood blocks | **Raft rails** across a bay of leaping fish; an **Aegis** hidden in a ring of crates |
| **3-3, 3-8** | **Tide** rises and falls; **Boss Bass** chases you; you **throw White Blocks at it** | The tide scene: you **throw amphorae at the Giant Fish** to drive it off. Revisit the same idea with smaller platforms (3-8) |
| **3-4** | Hills with a **Lakitu** raining Spiny Eggs for the whole second half | A hill scene where a **harpy** drops sea urchins |
| **3-5** | A dense field of **Jelectros** (19 of them), wind pipes that push you up, a bonus room only the **Frog Suit** can reach | A **jellyfish forest** to swim through slowly; **dolphin-spouts** lift you, and the Conch is the key to a bonus room |
| **3-6** | **Autoscroll** over Donut and Rotary Lifts | A **sinking-crates** autoscroll (above) |
| **3-7** | **Spikes** throw spiked balls (the manga advice: "wait for the toss, then stomp"); hidden beanstalk to Coin Heaven | **Crabs** that throw urchins; a hidden **vine** to a cloud of tesserae |
| **3-9** | Above-water and underwater halves; a Bullet Bill **life-farming trick** (we cap it) | A **shore-and-sea** scene; a Frog-Suit-equivalent bonus room |
| **Fortress 1** | **Find the right door**; Boom Boom **grows wings after the first hit** | The Labyrinth: Ariadne's thread. **Minotaur changes after the first hit**: he rams a wall, then **charges through a gap** |
| **Fortress 2** | **Underwater** with **Stretches** (their first appearance) between Roto-Discs | The **flooded Labyrinth**: swim between **swinging doors** |
| **Airship / Wendy** | **Bolt Lift** over Rocket Engines; Wendy's **candy rings bounce off the walls**, and she **jumps higher after each stomp** | Talos's galley with a **rising and falling oar platform**; **Talos's rocks ricochet**, and he **leaps higher** after each hit on the ankle plug |

#### Act III, the Heroes' Vases (SMB3 World 4)

| SMB3 level | What it does | Our scene idea |
|---|---|---|
| **4-1** | Big pipes, a **Big Piranha Plant**; **kick a giant shell** into a **Mega ? Block**; swim up **waterfalls** to a hidden pipe with 1-Ups | **Giant amphora blocks** opened by a **kicked giant Chelone shell**; a waterfall painted on the vase to swim up |
| **4-2** | **Bobbing and sinking platforms** with leaping fish; kick a White Block into the Mega block; **24 coins** unlock a map house | Floating **olive-oil rafts**; 24 tesserae unlock a shrine |
| **4-3** | A **cavern** with Buzzy Beetles and Spinies on floor *and ceiling*; **"tricky footing"**; two **Sledge Bros** on the plain | A **kiln cave** with creatures on the ceiling; two **Silenus** on the approach |
| **4-4** | Underwater, **Lakitus** throw Spiny Eggs, **bubble-jet** blocks the way unless you wear the **Frog Suit**; **43 blue coins** | A **vase-bottom lake** with harpies; the Conch is the key; 43 **lapis tesserae** |
| **4-5** | A cannon gauntlet with **Missile Bills** that **make U-turns**; a hidden **vine** to a Tanooki Suit | **Homing discs** that loop back; a hidden **grapevine** to a statue suit |
| **4-6** | Two **Magic Doors** that swap you between the giant and small sides | **Kiln doors**: two doors that swap scale (see Act III table above) |
| **Fortress 1** | A choice of **pipes**; **sliding Thwomps**; a long **Hot Foot** hall; Boom Boom on the ground | Sliding **column drums**; **embers** in a long hall; the **Nemean Lion** on the ground |
| **Fortress 2** | **Donut Lifts** over lava; a Switch reveals a **vertical wrap-around** area with pipes and lifts | **Crumbling plinths** over a kiln fire; a **wrap-around** room (the vase wrap) |
| **Airship / Iggy** | **Rocket Engines firing in four directions** (some randomly skip); **uneven platforms** make stomping Iggy difficult; Iggy is **quick** and **retreats into his shell** | **Hydra fight on a tilted raft**; the Hydra is quick, retreating under the water between attacks |

#### Act IV, Rome (SMB3 World 7)

| SMB3 level | What it does | Our scene idea |
|---|---|---|
| **7-1** | **Screen wrap** and vertical climbing through pipes; **kick a shell** around a curved pipe to hit a block | The **aqueduct climb** with **wrap-around arches**; a shell bounced through a curved pipe |
| **7-2** | Part above-ground, part underwater; **throw a White Block** into a block; **invisible Jump Blocks form a bridge**; 46 underwater coins that only the Frog Suit can collect | **Cistern scene**; **hidden marble blocks** that become a bridge; a Conch-only coin cache |
| **7-3** | **Six Starmen** in a row, so you stay **invincible** to the goal | **The Triumph**: a chain of **Aegis** pickups to cross a whole level invincible, as a Roman triumphal parade |
| **7-4** | An **underwater autoscroll** through a Jelectro field; a hidden area behind the wall found by flying | **The Tiber Flume**; a hidden alcove behind the wall |
| **7-5** | An underground **pipe maze** where **Bob-ombs break bricks**, **hidden blocks bridge the gaps**, and two pipes dead-end | **The Cloaca Maxima**: **Pot Bombs** blow open walls; dead-end channels |
| **7-6** | **Directional Lifts** that change direction each time you jump on them; a hidden 1-Up; you can fly over the stone barrier at the start | **Valve lifts** that reverse when you jump |
| **7-7** | Cross a **Muncher** field with a chain of **Starmen**; you must **crouch-slide** | **The Thistle Run**; crouch-slide under a low arch |
| **7-8** | A level of **Ptooies and Walking Piranhas**; a **Hammer Suit** bonus | **Acanthus Snappers that walk** (a *Garden Room* variant); Mallet bonus |
| **7-9** | A **broken pipe maze**: study the whole map before moving; **White Blocks** to clear | A broken-aqueduct maze to plan before you move |
| **Piranha Plant stage** | Short, optional; **Munchers open and close in rhythm**; **200-second timer**; ends with a P-Wing | **Garden Room**: thistles that **open and close to a beat**; the prize is **Winged Sandals** |
| **Fortress 1** | **No enemies**; a **brick room** turned to coins; a **reset trick** gives infinite coins (we cap it); Roto-Discs and Hot Feet; "Mario has to **fly** to reach the end" | **The Treasury** puzzle room; the Winged Sandals needed to finish |
| **Fortress 2** | **Pipe platforms over lava**; Boos; Roto-Discs; crouch-slide under a wall | **Hypogeum lifts over lava** |
| **Airship / Ludwig** | **Bolt Lifts that move backward** when stepped on; Ludwig fires **rapid wand shots** and **charges** after a stomp; **he can hide in his shell at will** | A liburnian with **backward-moving platforms**; the **Colossus** can **sit down** to protect his head |

#### Act V, Olympus (SMB3 World 5)

| SMB3 level | What it does | Our scene idea |
|---|---|---|
| **5-1** | A **block mountain** where **Chain Chomps bite from below**; flight reaches a Music Box chest | A cloud mountain where **Cerberus's pups bite from below**; flight to the **Lyre** |
| **5-2** | You **fall down a pipe** and **must stay still to land on a note block**; two routes, the secret one leads to a **three-1-Up room** | **Fall from Olympus**: stand still on the aulos block; a secret route to a Muse Shrine |
| **5-3** | The only level with **Kuribo's Shoe**; the shoe lets you **stomp Munchers** | A **Giant Sandal** scene across a thistle field |
| **5-4** | **Rotary Lifts**; a **Leaf shortcut** (run, then float over everything); a **Lakitu** on the goal cloud | Spinning cloud platforms; glide over the whole level; **Nephele** at the end |
| **5-5** | Nearly all **Donut Lifts**; **28 coins** unlock a P-Wing house | **Cloud puffs** that sink; 28 tesserae unlock a Winged Sandals shrine |
| **5-6** | **Para-Beetles** as moving platforms in an autoscroll | **Pegasus and winged horses** as platforms (above) |
| **5-7** | **Pile Driver Micro-Goombas** between blocks; Lakitu; Turtle Cannons; a pipe to a hidden ground area | **Mudbrick Mimics** again; a hidden **ground** below the clouds |
| **5-8** | **Clouds**, a **Lakitu that chases you** to the end, the manga's **7/10** | **Nephele's chase**: the hardest scene in the Act, with a *throw a shell at her* answer |
| **Fortress 1** | **Roto-Disc orbs**; stand on a disc's **middle** to bypass a Thwomp | **Spinning shield-discs**; stand on the centre hub |
| **Fortress 2** | **Lava above and below**, about 22 Podoboos, a Starman | **Sun-fire above and below** |
| **Airship / Roy** | A **rotating cannon** with a **ditch** that avoids its fire; Roy **shakes the room when he lands**, and **paralyses you if you're standing** | **Zeus's boat**; the **shockwave** from Zeus's stamp paralyses you unless you're **in the air** |

#### Act VI, Etruscan Tombs and Act VIII (SMB3 World 8)

| SMB3 level | What it does | Our scene idea |
|---|---|---|
| **Fortress** (9/10) | A **maze of doors** that loop back; **conveyor belt** over lava with spikes above (crouch); hidden **P-switch door** | **The Tomb of Orcus**: false doors; a treadmill under a **descending spiked ceiling** |
| **Hand Traps** | Three **trap levels** on the map, each with a **50% chance** to pull you in; clearing one gives a **Super Leaf**; skipping all three has a **12.5%** chance | **Statue hands** on the Great Floor that grab you at random for a trap room; the reward is a Laurel Wreath |
| **8-1** | Turtle Cannons, pipes, a Bullet Bill **life farm** (we cap it); flight reaches a **three-1-Up room** | A **discus-thrower gauntlet** |
| **8-2** | The **Angry Sun returns** and chases you; a **sandfall shortcut** skips the first chasm and the Sun | **The Aten chases you** across the Hoard's desert; **a sand-slide** shortcut |
| **Big Tanks** | **Only the tanks move**; standing on the ground doesn't scroll you; the convoy (13 tanks) ends at a **Boomerang Bro** | A **siege-tower convoy**; end with a **Silenus boss** |
| **Battleships** | Three warships; you can **swim under** them but are **pushed down**; Small Mario can dive under | **Trireme fleet**; a diving route under the ships for the *small* artist |
| **Airships** | **Faster autoscroll**, a Rocket Engine and a **Rocky Wrench** on almost every platform; "**step on the Wrenches' heads**" | The Vandal's **fastest ship**; **Oarsmen** to bounce on |
| **Super Tank** | A **small lead tank** and three longer ones; many cannons, **flying Boom Boom** at the end | **The Trojan Horse** (above) |
| **Castle of Koopa** | **Bowser Statues** fire lasers when approached; a lift that rises when stepped on; Donut Lifts over lava; a **four-way split** where two lead to Bowser | **Statue lions** with eye-beams; **lifts**; a four-way fork, two of which reach the Vandal |
| **Bowser** | Breathes fire **at varying heights** (jump or crouch), then **jumps and breaks the bricks** under him; land him in the **same spot** two or three times to open a pit; **35 fireballs** or three hammers also work | **The Vandal** (section 10): **smashes at different heights** (jump or crouch), then **leaps and breaks tiles**; the Lamp (our fire) or the Mallet are alternative ways to win |

#### Act VII, Byzantium (SMB3 World 6)

| SMB3 level | What it does | Our scene idea |
|---|---|---|
| **6-1** | **Ptooies** (spiked balls), a **warp door visible only with a flight suit** | A **peacock** spitting spiked feather balls; a **door that only the Laurel glide can see** |
| **6-2** | **Cloud Lifts** autoscroll with slippery landings; the manga's 7/10 | Floating **mosaic fragments** over a gold floor |
| **6-3** | **Ice Blocks**, a hidden **beanstalk** to a Tanooki Suit | **Marble blocks**, a hidden grapevine |
| **6-4** | **Rotary Lifts** and **96 coins**, with many in Coin Heaven | Spinning **icon discs** |
| **6-5** | A cave you **must fly** through; **Nipper Plants** block the exit; **protect your suit**; **400-second timer** | **The Dome's attic**: glide to the exit and **don't get hit** (you'd lose the Laurel) |
| **6-6** | A large underground cave with a Starman and zero secrets | A **cistern** with calm swimming |
| **6-7** | **Diagonal autoscroll**; **Frozen Coins** you **melt with fire**; **78 coins** unlock an Anchor house | **Tesserae in glass** you melt with the Lamp; 78 tesserae |
| **6-8** | **White Blocks** to kill enemies cleverly; the manga's **7/10**; Hammer Suit | **Marble blocks** thrown at the Whitewashers; the Mallet |
| **6-9** | A **flight shortcut** over the structure, or a **flooded cavern** with Munchers; a **Frog Suit** secret | A **flooded crypt**; the Laurel glide skips it |
| **Fortress 1** | An **elevator block**, a **long rail-lift** section | **Rising lifts**, a long treadwheel |
| **Fortress 2** | **Seven sliding Thwomps**, Boos, Roto-Discs, all on **ice** | **Sliding column drums** on a polished floor |
| **Fortress 3** (8/10) | **Conveyor belts**, **Stretches**, a **Duo-Roto-Disc**, "**keep braking**"; Boom Boom jumps between five blocks | **Treadwheel hall** with slippery braking |
| **Airship / Lemmy** | **Bolt Lifts**, then a flat room where Lemmy's **balls bounce around**; **bounce on his head** | The Throne of Solomon's **lions throw bouncing golden balls** |

**Boss patterns, as the wiki describes them**

| SMB3 boss | Pattern | Take for our boss |
|---|---|---|
| Larry | blast, shell retreat, blast; 3 stomps | the template (Act I) |
| Morton | same, slower, more wand shots | the same pattern with a different **tempo** |
| Wendy | **ricochet** rings; she **jumps higher** after each hit; 3 hits | Talos's **ricocheting rocks** |
| Iggy | **quick**, **uneven** floor, retreats in his shell | the Hydra **on a tilted raft** |
| Roy | **ground-shake** that **paralyses** you if you're standing; 3 hits | Zeus's **stamp** |
| Lemmy | **bouncing balls** in a flat room | the lions' **golden balls** |
| Ludwig | **rapid shots**, shell charge, can retreat at will | the Colossus **sits down** |
| Bowser | **fire at varying heights**, then **floor-smash** | the Vandal |
| Boom Boom | grounded in W1 to W2; **grows wings** after the first hit from W3 on; **jumps between blocks** in the hardest fortress | **mini-bosses get a second move after the first hit** |

**Cross-cutting lessons**

- **Suits as keys.** Flight, the Frog Suit, the Tanooki statue and the Hammer all open a hidden room somewhere. Make every one of our suits the key to at least one secret per Act.
- **One power-up airships.** Several airships have a single power-up and no secrets, because the cannon gauntlet *is* the level. Ours do the same.
- **A level of nothing but a challenge.** Some levels (5-8, 7-3, the Piranha Plant stage) have one idea and no secrets. That is fine: it keeps the Act's pace varied.
- **Coin thresholds** vary (24, 28, 30, 44, 78) and each unlocks a map house. We use tessera thresholds with different rewards (an anchor, Winged Sandals).
- **Life farms.** SMB3 has several (stomping Bullet Bills, Dry Bones, spawner pipes, the brick reset). Decide once: **cap each one** (my default).
- **Difficulty.** The wiki's manga ratings run from **2/10** (1-1) up to **9/10** (the World 8 fortress and airships). For a phone game aimed at a gentle audience, **we should cap scenes at about 7/10** and keep the 8 to 9 range for optional bonus scenes.
- **Timers.** SMB3 uses 300 seconds by default, 200 for the Piranha Plant stage and 400 for 6-5. We have no timer; scenes that rely on one (6-5's careful flight) use a **pacing device** instead (a rising tide, a closing door).

## 10. The Great Vandal and the ending

**Who he is.** A towering **Sweeper**: broom in one hand, maul in the other. He cannot bear a mess, and to him every picture is mess: it *looks back at him*, it takes up wall. So he **smashes pictures into tesserae and sweeps them into the Hoard**, then whitewashes the bare wall. (Proposed characterisation: see Open questions.) The name has two layers. The Vandals were a real Germanic people who sacked Rome in AD 455, and the word "vandalism" was coined much later (in 1794) and is not fair to them; we make the character entirely fictional and the gallery card explains the word. His title is **the Iconoclast**, "image-breaker", from the real Byzantine period (726 to 843) when some images were whitewashed or removed. We **never mock any living religion's imagery**: no sacred figure is ever an enemy, a target or a prop that you smash.

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
- **SMB3-specific phone worries:** flight (tap Jump repeatedly) must be forgiving, and the 28-slot Capsa must be easy to scroll with a thumb. Playtest both early.

## 16. Roadmap (order, not dates)

1. **Greybox**: one artist, SMB3 movement (run with the winged meter, stomp, shell kick, block bounce), a flat test level. Check that it feels good on a phone.
2. **Filter spikes**: run three sample sprites each through the mosaic, vase, tomb and fresco filters and **look at them on a phone at arm's length**. Gate: if gold, black-figure or dark-tomb sprites aren't readable, fix the rules (section 11) before building more.
3. **Vertical slice**: scene I-1 (*The Marsh of Nebamun*) in tomb style with the theatre-curtain intro, one Muse Shrine, the Victor's Dais and one pithos.
4. **Act I complete**: three scenes, the Scribe's Tomb and the Sphinx; Laurel Wreath glide; first run of the Great Floor map with one Satyr.
5. **Second style**: Act II (Crete), to prove the filter pipeline generalises: swimming, Conch, bull vaults, the Minotaur.
6. **The map and meta game**: Muse Shrines, Vase Match, the Capsa, Urania's Star Chart. These systems can be built alongside Acts I and II.
7. **Acts III to VII** in order, adding one big system each (Kiln flip and Hydra's fire; water flow and valves; Winged Sandals flight and ship autoscroll; Lamp light; whitewash scraping).
8. **Act VIII and the ending**, then music pass, gallery extras and polish.

**Gallery extras.** In the part 2 style: a gallery card per hero, per Act and per source work, saying where the *real* object lives, with a note on invented faces and restorations.

**Scope warning.** 39 levels is a lot: part 1 shipped 29 levels in 7 worlds and part 2 plans 20. If it is too much, merge each Act's fortress and ship into one (32 levels) or cut Act VIII's fleet scene (38). Act VII could also launch as an update.

## 17. Stretch ideas and alternatives

- **Two-player co-op** (SMB3's alternating two-player became a real co-op), with a shared Winged meter.
- **A "Mosaic Maker"**: build and share your own mosaic level (tile-based, so very natural).
- **More Acts:** Mesopotamia (Assyrian palace reliefs, lamassu, the Ishtar Gate), Persia (Persepolis), Nubia, the Indus, Roman Britain.
- **Daily challenge** levels as Vase Match or Winged Meter runs.
- **Alternatives for part 3 (and part 4)**, each fitting a different Mario game:
  - **Renaissance and Baroque, framed as theatre.** Stage settings, Palladio's theatre in Vicenza, Baroque ceilings with figures bursting through the frame (quadratura), Caravaggio's spotlights (a ready-made lamp-light mechanic). Strong for a light-based game.
  - **The Silk Road.** Persian miniatures, Chinese hand-scrolls (a level that *is* a long horizontal scroll), Japanese folding screens (a screen that opens like a level), Indian court painting. Very natural for a horizontally scrolling game, and many of the works are public domain.
  - **Islamic geometric art** and **Mesoamerican codices** are rich and underused, though tricky on cultural-sensitivity grounds.

## 18. Decisions so far

(From the brief.)
1. **Part 3 is SMB3-style, set in antiquity.** Egypt, Crete, Greece, Rome, Etruria, Byzantium; mosaics, vases, tomb paintings and frescoes; all public domain.
2. **Theatre staging.** A Greek theatre with masks and a curtain opens the game; the world map is a Roman floor mosaic.
3. **Suits:** Laurel Wreath, Winged Sandals, Marble Statue, Triton's Conch, Sculptor's Mallet and Chisel, Giant Sandal, Orpheus's Lyre.
4. **Map features:** wandering Satyrs, Muse Shrines, vase-matching, ship battle plus Wonder or monster boss per Act.
5. **Eight worlds** as listed in section 9, with the Great Vandal as the villain.
6. **Tone:** gentle and whimsical. **Platforms:** phone first, then PC. **Engine:** Unity, with the code-driven art pipeline.

## 19. Open questions

1. **Part 3 title** and **series name** (section 0). My pick: *Masks & Mosaics* in the *Into the Frame* series.
2. **The Vandal's character.** I proposed a tidy-minded **Sweeper** with a maul and broom, who ends up leaving one tessera out of place. Alternatives: a more classic villain (a stern Iconoclast in plain robes, with no comic side), or a *former apprentice* nobody noticed. Which do we want?
3. **Four heroes with invented faces**, or fewer (two, as SMB3 has Mario and Luigi) or more? And do we want **Iaia** (she makes a good case, but we only have Pliny's few lines about her)?
4. **Scope:** 39 levels, 32 (fortress and ship merged), or Acts released in two halves?
5. **Is the ship-battle-then-boss structure right for every Act**, or do some Acts end in a fortress alone?
6. **Hieroglyphs:** decorative pseudo-glyphs (my default), or real ones with an Egyptologist's check?
7. **Italian and other heritage-image laws:** how cautious do we want to be about Acts IV, VI and VII before launch?
8. **Does part 3 cameo parts 1 and 2**, or stay self-contained? (They are quick and charming, but a sibling-game reference needs at least one other game to exist.)
9. **Where does the new repo live**, and under which account? As in part 2, this document moves across when it exists.
10. **Business model:** first Act free, one unlock (matching the other games), or something different for a larger game?
11. **Act I's size.** Keep it at five nodes, or expand to seven (the autoscroll causeway and the shaduf barges) so the free Act matches SMB3's World 1? This raises the total to 41 levels.
12. **Spawner pipes.** Cap the number of enemies a pithos can spawn (my default), or allow SMB3-style life farming?
13. **Transformed Keepers.** Do we want the Ovid-style transformation of each Act's Keeper (section 9), or keep the emblema theft alone?
14. **Title screen hero.** A random artist each time (my default), or the last one you played?
15. **Scene bank.** Keep the optional scenes in section 9 as stretch content, or promote some of them into the main route?
