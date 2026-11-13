# Tapestry Adventures: Unity project

## First run (on your computer)
1. Install **Unity Hub** from unity.com/download, and sign in with a free Unity account (Unity Personal).
2. In Unity Hub, go to **Installs → Install Editor** and install **Unity 6** (the project is set to 6000.4.8f1, but any Unity 6 works). Tick these modules:
   - **Android Build Support** (with OpenJDK and Android SDK & NDK)
   - **iOS Build Support** (Mac only)
   - **Windows Build Support** or **Mac Build Support**, for the computer you're on
3. Clone this repository and check out the branch `ccr-87c99057-cx6bj3`. If you'd rather not use git, use GitHub's *Code → Download ZIP* on that branch.
4. In Unity Hub, go to **Projects → Add → Add project from disk** and pick the **`unity`** folder. Open it. If Unity asks about the editor version, choose the Unity 6 you installed.
5. The first open takes a few minutes while Unity imports the art. If a dialog asks to **enable the new Input System backends**, click **Yes**. Unity restarts.
6. The project creates `Assets/Scenes/Main.unity` and opens it. Press **Play** ▶.

If something looks off, the **Tapestry** menu has *Create Main Scene*, *Reimport Art* and *Apply Player Settings*.

## Controls
| | Keyboard | Gamepad (Xbox / PlayStation / Switch) | Touch |
|---|---|---|---|
| Move | ← → or A D | left stick / d-pad | joystick (bottom left); push it to the rim to run |
| Jump (hold to jump higher) | Space, Z, K, ↑ | A / Cross / B (Switch south button) | gold button showing your knight leaping |
| Run (and shoot, with the crossbowe) | Shift, X, J | X / Square, or right trigger | blue crossbow button (pushing the joystick to the rim also runs, but only the button shoots) |
| Get off Bayard | ↓ or S | stick down / d-pad down | v |
| Change knight (testing) | Q / E or Tab | shoulder buttons | (none) |
| Pause | Enter, Esc, P | Start / Options | II |
| Mute music and sound | M | Select / View / Share | (none) |

## The levels
- **Worlde 1-1, Ye Meadowe of Beastes:** an open meadow with towers, a sea pit and brick rows. It ends at the keep upon the motte.
- **Worlde 1-2, Ye Foreste of Wyverns:** treetop platforms you can jump up through, three gaps bridged by trees, a stream, perching owls and wyverns. It has its own wandering tune in A Dorian and ends at the abbey in the wood.
- **Worlde 1-3, Ye Crypte Gate and Ye Undercroft:** it starts above ground; walk through the door of ye Way Downe and you descend into a vaulted crypt. At its far end a stair climbs back up into daylight (Ye Lyght of Daye), where the flag and ye Rounde Tower wait with a stone ceiling, low arches, and lifts that glide sideways or rise and fall over dark pits. Soup Cats guard the kitchens. The crypt has a slow tune in E Phrygian.
- **Worlde 1-4, Ye Cockatrice's Keepe:** the World 1 castle. It is a stone hall hung with tapestries and lit by torches.
  - **Hazards:** fire pits with stone stepping blocks, and iron flails swinging round their blocks (Mario's fire bars).
  - **The boss:** the **Cockatrice** paces a drawbridge over the fire. It hops, and glares a petrifying gaze that turns you to stone for a moment.
  - **Beating it:** stomp its crowned head three times, or shoot it (three bolts count as one stomp), or slip past it and strike the axe beyond. The bridge falls and the Cockatrice with it.
  - **The ending:** Brother Odo the monk thanks you: the Princess is in another castle.
  - It has its own castle march and boss music, and the boss has its own tapestry health bar.
- **Worlde 2-1, Ye Shipwrights' Yarde** (World 2: the Bayeux Channel crossing): the Norman shore. It has sand underfoot and timber crates for bricks, with the fleet sailing past behind.
  - **Log piles** roll logs at you. Jump them, or stomp one to knock it loose so it tumbles away (100 glory); bolts split them. Logs that roll off screen are gone for good.
  - **Gulls** wheel overhead and dive at you; **crabs** scuttle and hop along the beach.
  - It has a rolling 6/8 sea dance (D Mixolydian) and ends at **Ye Harbour Gate**.
- **Worlde 2-2, Beneath ye Waves:** an underwater level. Bayard waits on the shore.
  - **Swimming:** tap JUMPE to stroke upward; you sink slowly and can't rise past the surface. The knight swims lying flat, head first, and stands up to walk when he reaches the sea-bed (if there is headroom).
  - **Sea creatures** can't be stomped, so dodge them or shoot them with the crossbowe, which is found right at the start: the **Bayeux fish** swims in waves, the **sea-serpent** patrols in coils, and the **sea-monk** sinks and then pulses after you.
  - **The way through:** rock pillars and a reef tunnel to swim through, and two abysses.
  - It has a slow 3/4 waltz in F Lydian. Swim into **Ye Wrecke**, a sunken longship, to finish.
- **Worlde 2-3, Ye Crossing:** the fleet crossing the open sea; falling in is fatal.
  - **Longships:** leap between them. Some rock at anchor and some sail to and fro, carrying you with them.
  - **Footholds:** sea-rock stacks and three islets, including a halfway banner and Bayard on the middle islet.
  - **Wind gusts:** blue swirls blow across the screen and push you, more often against you than behind you.
  - **Leaping fish:** they burst from the waves in arcs. They *can* be stomped above water.
  - It has a brisk 6/8 crossing tune (A Dorian, with a shawm lead) and ends at **Ye Beacon** on the far shore.
- **Worlde 2-4, Ye Landing and ye Sea-Castle:** the World 2 boss level.
  - **The landing:** the Norman landing on the beach (crossbowe first, log pile, beasts, halfway banner).
  - **The harbour:** longships moored over deep water, where **Aspidochelone, the Island-Whale**, hides below. It surfaces a few strides from your ship, spouts a column of water that hurls you skyward, snaps its jaws and dives.
  - **Beating it:** stomp its back while it is up, or shoot it (three bolts to a stomp). Each blow sends it under; three and it sinks for good.
  - **The end:** Brother Odo waits by **Ye Tower by ye Sea**. While the whale lives, the screen stays on the harbour and you can't ride past it. Once it's vanquished, a pointing hand (a scribe's manicule) shows the way to him.
- **Worlde 3-1, Ye Firmament** (World 3: the Tapestry of Creation, Girona): a sky level in Romanesque needlework, with the great wheel of creation and its sun, moon and star roundels behind.
  - **Footholds:** cloud islands, thin cloud bands you can jump up through, and gold **star boards** that glide or rise and fall over the gaps (falling is fatal).
  - **Wind-heads:** puffed-cheeked faces blow a steady wind across 12 tiles of sky, sometimes at your back and sometimes in your face.
  - **Birds of the air** fly across in waves; roundel blocks are this world's bricks.
  - It has a tune in the manner of early organum (a melody doubled a fifth below, with bells) and ends at **Ye Gate of Paradyse**.
- **Worlde 3-2, Ye Waters Above:** swimming in the waters above the firmament, with the wheel of creation behind.
  - **The way through:** white cloud walls and a cloud-reef tunnel to swim around.
  - **Creatures:** the new **hippocamp** (the bestiary's sea-horse) paddles near its home. If you come close and level with it, it rears and flashes as a warning, then makes one straight dash of about 4 tiles and rests (swim up or down out of its line to dodge); fish, sea-serpents and sea-monks swim among the clouds. The crossbowe is right at the start.
  - It has a slow 6/8 tune in G Dorian and ends at **Ye Fountaine of Lyfe**.
- **Worlde 3-3, Ye Naming of ye Beastes:** the garden where Adam names the animals, with green turf over Romanesque zigzag bands and trees of life behind.
  - **Stampedes:** in three stretches of meadow, herds of three spotted **stags** come thundering in from the right (three herds per stampede, one at a time). They turn back at walls and leave the field after a while. Leap them, stomp them, or hop up onto the planks until they pass.
  - It has a bright, galloping 2/4 estampie and ends at **Ye Bower of Eden**.
- **Worlde 3-4, Ye Wheel of ye Yeare:** the World 3 castle.
  - **Turning wheels:** great Wheels of the Year, each with twelve month-roundels on its rim, carry four star boards round and round over the fire pits. The boards stay level, like a fair's big wheel.
  - **The boss, Ye Foure Windes:** four great wind-heads, two on each side of a hall with a fire pit in its middle.
    - **Blowing:** in turn, each one blows a strong wind that shoves you across the hall toward the fire.
    - **Drawing breath:** then it flashes gold for about three seconds and a manicule points at it. While it does, touch it or shoot it to defeat it.
    - **Reaching them:** ledges lead up to the high heads. Beat all four and Brother Odo waits beyond.
- **Worlde 4-1, Ye Boare Hunte** (World 4: the Devonshire Hunting Tapestries): from here the worlds are **woven**, with dense verdure of oak leaves and flowers. The knights stay stitched.
  - **Hounds:** a hunting horn sounds and **talbots** (hunting hounds) come running in from behind you every few seconds. Keep moving, leap them, or stomp them.
  - **Wild boars** charge when they see you. It has a horn-call hunting tune and ends at **Ye Hunting Pavilion**.
- **Worlde 4-2, Ye Otter and ye Swanne:** the river.
  - **Rafts:** log rafts set off from the near bank and drift downstream (to the right). Ride them across; they sink away at the far end of their run.
  - **Otters** leap up out of the water at you as you pass; **swans** glide on the water and charge if you are level with them.
  - It has a flowing river tune and ends at **Ye Mill**.
- **Worlde 4-3, Ye Falconers' Heath:** open purple heath with tall **hunting stands** to climb. **Falcons** circle high and dive at you. It ends at **Ye Mews** (the falcons' tower).
- **Worlde 4-4, Ye Hunting Lodge:** the World 4 castle, with hearth fires and flails.
  - **The boss, Ye Greate Boar:** it paws the ground, then charges across the hall.
    - **Its hide is too tough** to stomp while it stands; you just bounce off.
    - **Dodge it** (leap over it) so it rams a wall. Then it is **dazed**, with stars round its head, for about two seconds: stomp it or shoot it then. Three stomps fell it.
    - Each hit makes it angrier and faster. When it falls, the gate at the far end of the hall crumbles and Brother Odo waits beyond.
- **Worlde 5-1, Ye Hunters Set Out** (World 5: the Unicorn Tapestries): a woven **millefleur** wood, a deep blue-green ground strewn with countless little flowers, with oak, holly and orange trees and a château on the hill.
  - **Huntsmen** walk their beat and hurl spears at you in long arcs. **Greyhounds** lie in wait, then race at you. **Pheasants** burst up out of the flowers as you pass.
  - It has a stately basse-danse tune and ends at **Ye Castel of ye Hunte**.
- **Worlde 5-2, Ye Fountaine:** the stream is **poisoned** (murky violet); touching it hurts.
  - Leap the narrow stretches. Partway along, **ye Unicorne's horne** lies on the path: carry it (it floats over your head) to **ye fountaine**.
  - The waters then run pure and blue, and you can wade the wide stretches beyond, which are too wide to leap.
  - **Genets** pounce at you in long low leaps. It ends at **Ye Chapel by ye Fountaine**.
- **Worlde 5-3, Ye Unicorne at Baye:** a **chase**. After a horn call, the hunt (a line of huntsmen with levelled spears, and their greyhounds) sweeps on from behind and never falls far back.
  - If it catches you, you are wounded and it drops back a little. Keep running, and leap the paling hedges.
  - It has a breathless chase tune and ends at **Ye Gatehouse of ye Parke**.
- **Worlde 5-4, Ye Unicorne in Captivitie:** the World 5 castle.
  - **The boss, Ye Spotted Unicorne:** white, spotted red with pomegranate juice, and frantic in its enclosure. It **cannot be harmed**: stomping it only bounces you off.
    - It rears, then charges. Leap over it so that it rams the **fence gate** on the right. Each ram splinters the gate (its bar shows the gate's strength), and the third breaks it. The stone wall on the left only stuns it.
    - **Spared:** it is calmed, and stays to be ridden. **Ye Unicorne** is a second mount: faster than Bayard, and at a gallop (hold RUN) its **horn fells any beast it runs into**. It has no flutter-leap.
    - If you are riding Bayard you can change to the unicorn by touching it. A hit throws you off either steed, and it carries on into the next level like Bayard. From World 6, **unicorn bricks** can hold it too.
- **Worlde 6-1, Sight** (World 6: The Lady and the Unicorn): red millefleur grounds strewn with flowers and rabbits, blue flower islands, and banners with the Le Viste crescents.
  - **Ye Ladye's Mirrour:** strike a mirror block from below and the **gold panes** and **silver panes** change places. Only the bright set bears you; the faint one is just a reflection.
  - Bridges of panes cross the pits. Near the end, stand on the stone pillar to strike the mirror, so the gold vanishes from under someone else's feet, not yours.
  - It has a courtly chanson and ends at **Ye Ladye's Tente**.
- **Worlde 6-2, Hearing:** great **organ pipes** cross the pits and rise and fall with the music, changing height on every second beat. Together they make a staircase that climbs and falls away. **Popinjayes** (parakeets) flutter overhead.
  - It has an organ tune and ends at **Ye Halle of Musick**.
- **Worlde 6-3, Taste and Smell:** **baskets of flowers** spring you high (hold JUMP to spring higher), over wide pits and up to high garlands. **Monkeys** fling comfits at you in arcs; **foxes** dart at you when your back is turned; **lapdogs** yap and hop.
  - It has a merry dance and ends at **Ye Rose Bower**.
- **Worlde 6-4, A mon seul desir:** the World 6 castle, the Lady's tent.
  - **The boss, Ye Lyon and ye Unicorne:** two foes at once, three blows each.
    - Stomp **the Lyon** while he crouches to spring; stomp **the Unicorne** while it rears. At any other time you just bounce off.
    - Each blow to one makes the other angrier and faster. When both are overcome the way on opens, and Brother Odo waits beyond.
- **Unicorn bricks:** from World 6, some bricks hold the spared unicorn.
- **Worlde 7-1, Ye Seven Trumpettes** (World 7: the Apocalypse Tapestry of Angers): scenes on alternating red and blue grounds under a border of clouds.
  - **Seven angels** sound their trumpets in turn. Each blast drives you backward, so wait for the silence, then go.
  - **Stars fall from heaven,** often right above you. It has a solemn fanfare and ends at **Ye Gate of Heaven**.
- **Worlde 7-2, Ye Locustes:** swarms of **crowned locusts** with men's faces drift after you. **Soup Cats** (the Cat King's servants) guard the ground. It has a buzzing, restless tune and ends at **Ye Gate of ye Holy Citie**.
- **Worlde 7-3, Ye Foure Horsemen:** the White, Red and Black Riders and Death on the Pale Horse gallop in from behind, one after another. Leap them, wait on a cloud ledge, or stomp them. Bayard waits in the bricks.
  - It has a thundering gallop and ends at **Ye Keep of ye Cat King**.
- **Worlde 7-4, Ye Beaste of Seven Heddes:** the World 7 castle.
  - **The boss:** seven crowned heads sway on long necks. Each in turn spits fire, then lunges low across the floor and droops there, dazed: **stomp it then** (or two bolts).
  - Fell all seven. The fewer that are left, the faster they strike.
- **Worlde 7-5, Ye Greate Halle (the finale):** the Cat King's hall, hung with his banners.
  - **The Cat King** stands upon his throne flinging soup, and summons Soup Cats. On his throne he cannot be harmed. Every few flings he leaps down and **prowls after you: stomp him then!** Four blows.
  - Then the gate behind the throne crumbles and **Princess Aelfgyva** is free. Walk to her, and the story of the whole adventure scrolls past: **Ye Ende**.

**Crouching:** on foot, hold DOWN (or pull the touch joystick down) to crouch. A crouched knight is much lower (about one tile high), so he can duck beneath thrown spears, flung comfits and falling stars, and creep through a one-tile gap, but he moves slowly and cannot run. He stands up as soon as there is room above him. When riding, DOWN still dismounts.

**Ye Tapestries:** the title screen has a "Ye Tapestries" link: one page for each real tapestry behind the game (the Bayeux Tapestry, the Tapestry of Creation, the Devonshire Hunting Tapestries, the Unicorn Tapestries, the Lady and the Unicorn, the Apocalypse of Angers), with a picture, where and when it was made, where it is now, and how it appears in the game. A last page tells where the beasts, Bayard and the Cat King came from.

**Reward blocks are shuffled every play:** the prizes (bezants, Bayard, the unicorn, the capon, the crossbowe, the chalice) change places among the bricks. The holy relique stays where it is, and the chalice is never put near the end of a level or beside the relique.

**The free tapestry and the unlock:** World 1 is free. At the end of 1-4, or when a later realm is chosen, a screen offers **Ye Whole Tapestrie**, one purchase (£3.99) that unlocks Worlds 2 to 7 for good. There is also an "Unlocke" link on the title screen. In the Unity editor the purchase is simulated; use **Tapestry > Store > Forget the Purchase** to see the locks again. For the Google Play steps see `docs/PLAY_STORE.md`.

**Briefing:** before each realme, a parchment names its beasts and hazards, and the things that help or must be done (the unicorn's horn, mounts in the bricks, the halfway banner, and so on), each with a picture and a word of counsel. In a castle it also says how to vanquish the boss. Press JUMPE or touch to ride on. It isn't shown again after a death.

**Gauntlets:** after every holy relique a run of beasts waits along the way. Keep running while the relique lasts (nine seconds), and the fifth beast smitten in a row gives a lyfe.

Finishing a level rides on to the next one. Lives, bezants and glory carry over, and so do your size (small or large), crossbowe and Bayard.

**Level select:** once you have passed a realme, the title screen lets you choose it (after choosing your knight). Progress is saved.

The longer levels have a **halfway banner**. Touch it and it turns red; if you die after that, you rise again at the banner instead of the start.

## What's in the game
- **Knights:** all four, chosen on the title screen. Azura glides, Verdain begins every realme large, and Aurelia is fastest. The others begin small.
- **Bricks:** every brick breaks when a **large** knight bumps it from below (a small knight only bumps it). Some hide something; those give it up and leave a used block.
  - a bezant (the coin)
  - **Bayard**, the horse (Mario's Yoshi)
  - a **Golden Chalice**: one more life (the 1-Up). It slides along, so chase it.
  - a **Roast Capon**: makes a **small** knight **large** (the mushroom). A knight who is already large gets a feast of glory (2000) instead. It also slides along.
  - a **Crossbowe**: RUN shoots bolts, at most two in the air at once (the fire flower). Bolts fell any beast, even the burning lion. A hit makes a large knight small and knocks the crossbowe away. A crossbowe block gives a **capon** to a small knight (as the fire flower is only a mushroom to a small Mario).
  - a **Beanstalk** (a brick like any other, here and there): bump it and a **vine grows** up from it. Stand on the brick (or beside the vine, above it) and press **UP** to climb (W or the arrow keys, the d-pad or stick up, or push the touch joystick up). UP and DOWN climb, left and right drift off the vine, and a JUMP button (not the Up key) leaps off. At the top you are carried up to a **heaven of bezants**: a cloud floor, a stair of cloud planks and about thirty-five bezants. Walk to the hanging vine at the far end to come back down onto the beanstalk brick. The coins stay collected, so it cannot be farmed. Beasts below stay asleep while you are up there, and no pole, door or banner counts. (Level 1-1 has one, past the halfway banner.)
  - a **Holy Relique** (always found before the level's halfway banner): about nine seconds in which no beast can harm you and any beast you touch is smitten (the star). It bounces about. While it lasts, the knight flickers gold and white inside a spinning aura of golden rays; the flicker slows in the last two seconds.
- **Bayard:**
  - Riding him makes you faster and lets you jump higher.
  - Hold JUMP while falling for a flutter-leap.
  - He crushes any beast he lands on.
  - A hit throws you off (instead of costing health); he rears and bolts. If he gallops into a pit he is lost, and you must find him again in another brick (three of the level's bricks hold him).
- **Beasts:** Snail-Hound (red shell: stomp it, then kick the shell), Mossy Snail-Hound (green shell: stomp it, hold RUN to pick the shell up, carry it, and let go of RUN to hurl it; a held shell also fells the first beast it touches), Killer Hare, Fiery Lion (shoots fireballs), Bayeux Hound, Camel-Dragon, Wyvern, Spotted Unicorn (charges), Man-Faced Owl (swoops) and Soup Cat (throws soup).
- **Chains (as in Mario):** stomping beast after beast without touching the ground, a sliding shell, or the holy relique doubles its glory for every beast it fells in a row: 100, 200, 400, 800. The 5th beast in a row, and every one after it, earns a free life. The chain starts again when the shell stops or is picked up, or when the relique wears off.
- **Tyme:** each level counts down in **Roman numerals** from CCC (300), about 2½ a second, as in Mario.
  - At 100 a warning appears, the counter turns red and the music quickens.
  - At 0 the knight is lost.
  - Each tick left at the goal is worth 50 glory.
- **Small and large:** a **small** knight dies from one blow and can only *bump* ordinary bricks (they do not break); a **large** knight is taller, breaks bricks, and when struck is only made small again (he loses his crossbowe, and is safe for a moment). Item bricks work for both. If a capon grows you where there is no headroom, you stay crouched until there is room.
- **Title screen:** a top row (knight, bezants, world, tyme), a hanging banner for the name, a cursor-and-menu (a capon points at "1 KNYGHT ADVENTURE"; UP/DOWN or touch to choose, JUMP or ENTER to ride forth) and "BEST GLORIE" (the saved high score). Four knights stand on the ground; LEFT/RIGHT picks one.
- **Best Glorie:** the highest score is saved and shown on the title and the game-over screen ("A new Best Glorie!" when beaten).
- **Bells at the pole:** when a level ends, the last figure of your Tyme count decides the bells: 1, 3 or 6 ring (1, 3 or 6 bells) as the knight walks into the castle, each worth 500 glory. Others ring none.
- **Smythes:** hammer-throwing smiths (Worlds 4 and up; always in pairs). They walk a short beat, hop, and hurl hammers in arcs. Stomp or shoot them.
- **Leaping Embers:** in castles, a spark leaps out of the middle of every fire pit three or more tiles wide, then falls back in. It cannot be stomped or shot, and burns a knight who touches it.
- **Urn-Plantes:** a biting plant that rises from an urn and snaps, then sinks; it stays down while you stand on or beside the urn. Stomping it hurts; bolts fell it.
- **Wells:** stand on a stone well and press DOWN to climb in to a treasure cellar (about thirty coins); a pipe-like exit leads back to the well. Wells are in Levels 2 and 5.
- **Ye Wronge Syde:** after finishing the game, a "Wronge Syde" choice appears on the title menu: the same tapestries with beasts that are faster and with the Tyme running 25% quicker.
- **Rules:** your size is shown as a strip of tapestry stitched in two halves (one filled: small; both: large), and you have three lives. 100 bezants earn an extra life. The sea and pits kill. Touch the goal pole, slide down it and walk into the castle. The higher you grasp the pole, the more glory: 100, 200, 400, 800, 2000 or 5000, and the very top gives an extra life. Points float up wherever they are won.
- **Text:** all the on-screen writing is in olde English.
- **Music and sound:** original tunes in medieval modes on a synthesized band of recorder, lute, hurdy-gurdy drone, shawm and tabor.
  - **Tunes:** a stately title theme (D Dorian), a lively meadow dance (G Mixolydian), a fast shawm-and-drum tune while the relique lasts, and short victory, death and game-over pieces.
  - **Sound effects:** about 20, covering jumps, stomps, coin bells, breaking bricks, the crossbowe, Bayard's hooves and whinny, the lion's roar, soup and more.
  - All of it is made by `tools/audio/make_audio.py` (numpy and ffmpeg), so it is free to use.

## How the code is laid out
| | |
|---|---|
| `Assets/Scripts/Game/GameManager.cs` | game states, camera, parallax, HUD, touch buttons, title / knight select |
| `Assets/Scripts/Actors/` | `Knight` (player, riding, crossbowe, relique), `Knights` (the four knights' stats), `Enemy` (every beast's behaviour and the projectiles), `Bayard`, bezants, `Items` (power-ups) |
| `Assets/Scripts/Core/` | `Level` (tile map, collision, block bumps), `Art` (sprite loading), `TapestryInput` (keyboard, gamepad and touch, with either input system) |
| `Assets/Scripts/Levels/` | `LevelDef` (what a level is) and the level list, plus `Level1.cs`, `Level2.cs`: the maps as ASCII, generated by `tools/unity/make_levels.py` |
| `Assets/Resources/Art/` | the embroidered art, copied in by `tools/unity/export_art.py` |
| `Assets/Resources/Audio/` | music and sound effects, made by `tools/audio/make_audio.py` |
| `Assets/Scripts/Core/Audio.cs` | plays the music and effects |

Everything is built from code at runtime, so there are no prefabs to keep in sync. To change the art, edit `tools/emb/`, then run `python3 tools/emb/build.py` and `python3 tools/unity/export_art.py`.

## Building for each platform
*File → Build Profiles* (Unity 6):
- **Windows / Mac / Linux:** available now.
- **Android:** switch platform → Build. To publish, you need a Google Play developer account (one-time fee).
- **iPhone / iPad / Mac App Store:** Unity builds an Xcode project on a Mac. To publish, you need the Apple Developer Program (yearly fee).
- **Xbox, PlayStation 5 and Nintendo Switch:** first apply to each platform holder's developer programme:
  - ID@Xbox, through Microsoft's Xbox developer site;
  - PlayStation Partners, at partners.playstation.net;
  - the Nintendo Developer Portal, at developer.nintendo.com.

  Once approved, each one gives you its own Unity build support, under NDA, and the code here carries over. Approval usually needs a playable build and a short pitch, which this game is building toward.

## Android: test build on your phone
The project is set up as **Ye Tapestrie Adventures**:

| Setting | Value |
|---|---|
| Developer | pezacide |
| Package ID | `com.pezacide.tapestryadventures` (permanent once published) |
| Version | 0.1.0 |
| Code | 64-bit (IL2CPP, ARM64) |
| Minimum Android | 8.0 |
| Screen | landscape only |
| Icon | `Assets/Icons/icon.png` (made by `tools/emb/icon.py`) |

### One-time phone setup
1. On the phone, open **Settings → About phone** and tap **Build number** 7 times. This turns on Developer options.
2. Open **Settings → System → Developer options** and turn on **USB debugging**.
3. Plug the phone into the PC with a USB cable. When the phone asks *Allow USB debugging?*, tap **Allow**.

### Build and play
- **Easiest:** in Unity, choose **Tapestry → Android → Build and Run on Connected Phone**. The first build takes 10–20 minutes, because IL2CPP compiles everything to native code; later builds are quicker. The game then starts on the phone by itself.
- **Without a cable:** choose **Tapestry → Android → Build Test APK**. The folder holding `YeTapestrieAdventures.apk` opens when it finishes. Copy the file to the phone (for example by Google Drive or email), tap it, and allow *Install unknown apps* for that app when asked.

If Unity warns that Active Input Handling is "Both", switch it to **Input System Package (New)** (Edit → Project Settings → Player → Other Settings) and restart Unity. The build menu now does this for you.

If Unity says Android isn't installed, open Unity Hub and go to **Installs → (your Unity 6) → Manage → Add modules**. Tick **Android Build Support**, together with **OpenJDK** and **Android SDK & NDK Tools**.

### On the phone
- The touch controls sit inside the safe area, clear of camera notches:
  - bottom-left corner: a fixed **joystick**. Push it left or right to walk, push it to the rim to run, and pull it down to get off Bayard.
  - bottom-right: two round buttons. The big gold **jump** button shows the knight you are playing, mid-leap. The blue **crossbow** button runs when held, shoots when tapped with the crossbowe, and carries a green shell when held. Slide your thumb between them to run and jump.
  - top-right: a round **pause** button.
- The pause menu has tap buttons: Continewe, Returne to ye beginnynge, and Silence ye musick.
- The phone's **Back** button pauses the game.
- The screen won't dim or sleep while you play.

### Before the Google Play Store (later)
- **Earning money:** choose how the game will make money (ads, paid, or World 1 free with later worlds unlocked). The ads and unlock options need extra work built in.
- **An upload key:** create one (a keystore). Keep it backed up; losing it means losing the ability to update the game.
- **Build an .aab:** the Play Store needs an `.aab` file rather than an `.apk`, and the version code goes up by one for every upload.
- **Google Play Console:** create a developer account (one-time fee).
- **The store listing:** screenshots, a 1024×500 feature graphic, a privacy policy and the content-rating questionnaire.
