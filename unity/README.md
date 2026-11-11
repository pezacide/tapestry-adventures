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
  - **Stampedes:** in three stretches of meadow, herds of three spotted **stags** come thundering in from the right every few seconds. Leap them, stomp them, or hop up onto the planks until they pass.
  - It has a bright, galloping 2/4 estampie and ends at **Ye Bower of Eden**.

Finishing a level rides on to the next one. Lives, bezants and glory carry over, and so do your hearts, crossbowe and Bayard.

**Level select:** once you have passed a realme, the title screen lets you choose it (after choosing your knight). Progress is saved.

The longer levels have a **halfway banner**. Touch it and it turns red; if you die after that, you rise again at the banner instead of the start.

## What's in the game
- **Knights:** all four, chosen on the title screen. Azura glides, Verdain has four hearts, and Aurelia is fastest.
- **Bricks:** every brick breaks when bumped from below. Some hide something; those give it up and leave a used block.
  - a bezant (the coin)
  - **Bayard**, the horse (Mario's Yoshi)
  - a **Golden Chalice**: one more life (the 1-Up). It slides along, so chase it.
  - a **Roast Capon**: mends one heart (the mushroom). It also slides along.
  - a **Crossbowe**: RUN shoots bolts, at most two in the air at once (the fire flower). Bolts fell any beast, even the burning lion. A hit knocks the crossbowe away instead of costing health.
  - a **Holy Relique** (always found before the level's halfway banner): about nine seconds in which no beast can harm you and any beast you touch is smitten (the star). It bounces about. While it lasts, the knight flickers gold and white inside a spinning aura of golden rays; the flicker slows in the last two seconds.
- **Bayard:**
  - Riding him makes you faster and lets you jump higher.
  - Hold JUMP while falling for a flutter-leap.
  - He crushes any beast he lands on.
  - A hit throws you off (instead of costing health); he rears and bolts. If he gallops into a pit he is lost, and you must find him again in another brick (three of the level's bricks hold him).
- **Beasts:** Snail-Hound (red shell: stomp it, then kick the shell), Mossy Snail-Hound (green shell: stomp it, hold RUN to pick the shell up, carry it, and let go of RUN to hurl it; a held shell also fells the first beast it touches), Killer Hare, Fiery Lion (shoots fireballs), Bayeux Hound, Camel-Dragon, Wyvern, Spotted Unicorn (charges), Man-Faced Owl (swoops) and Soup Cat (throws soup).
- **Chains (as in Mario):** a sliding shell, or the holy relique, doubles its glory for every beast it fells in a row: 100, 200, 400, 800. The 5th beast in a row, and every one after it, earns a free life. The chain starts again when the shell stops or is picked up, or when the relique wears off.
- **Tyme:** each level counts down in **Roman numerals** from CCC (300), about 2½ a second, as in Mario.
  - At 100 a warning appears, the counter turns red and the music quickens.
  - At 0 the knight is lost.
  - Each tick left at the goal is worth 50 glory.
- **Rules:** health is shown as a long strip of tapestry, stitched in three segments (Verdain has four), and you have three lives. 100 bezants earn an extra life. The sea and pits kill. Touch the goal pole, slide down it and walk into the castle. The higher you grasp the pole, the more glory: 100, 200, 400, 800, 2000 or 5000, and the very top gives an extra life. Points float up wherever they are won.
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
