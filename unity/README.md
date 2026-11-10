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
| Move | ← → or A D | left stick / d-pad | < > buttons |
| Jump (hold to jump higher) | Space, Z, K, ↑ | A / Cross / B (Switch south button) | JUMPE |
| Run (and shoot, with the crossbowe) | Shift, X, J | X / Square, or right trigger | RUNNE / SHOOTE |
| Get off Bayard | ↓ or S | stick down / d-pad down | v |
| Change knight (testing) | Q / E or Tab | shoulder buttons | (none) |
| Pause | Enter, Esc, P | Start / Options | II |
| Mute music and sound | M | Select / View / Share | (none) |

## The levels
- **Worlde 1-1, Ye Meadowe of Beastes:** an open meadow with towers, a sea pit and brick rows. It ends at the keep upon the motte.
- **Worlde 1-2, Ye Foreste of Wyverns:** treetop platforms you can jump up through, three gaps bridged by trees, a stream, perching owls and wyverns. It has its own wandering tune in A Dorian and ends at the abbey in the wood.
- **Worlde 1-3, Ye Undercroft:** a vaulted crypt with a stone ceiling, low arches, and lifts that glide sideways or rise and fall over dark pits. Soup Cats guard the kitchens. It has a slow tune in E Phrygian and ends by climbing out to ye Rounde Tower.

Finishing a level rides on to the next one; lives, bezants and glory carry over.

## What's in the game
- **Knights:** all four, chosen on the title screen. Azura glides, Verdain has four hearts, and Aurelia is fastest.
- **Bricks:** every brick breaks when bumped from below. Some hide something; those give it up and leave a used block.
  - a bezant (the coin)
  - **Bayard**, the horse (Mario's Yoshi)
  - a **Golden Chalice**: one more life (the 1-Up). It slides along, so chase it.
  - a **Roast Capon**: mends one heart (the mushroom). It also slides along.
  - a **Crossbowe**: RUN shoots bolts, at most two in the air at once (the fire flower). Bolts fell any beast, even the burning lion. You lose the crossbowe if you take a hit.
  - a **Holy Relique**: about nine seconds in which no beast can harm you and any beast you touch is smitten (the star). It bounces about.
- **Bayard:**
  - Riding him makes you faster and lets you jump higher.
  - Hold JUMP while falling for a flutter-leap.
  - He crushes any beast he lands on.
  - A hit throws you off; he rears and bolts. If he gallops into a pit he is lost, and you must find him again in another brick (three of the level's bricks hold him).
- **Beasts:** Snail-Hound (red shell: stomp it, then kick the shell), Mossy Snail-Hound (green shell: stomp it, hold RUN to pick the shell up, carry it, and let go of RUN to hurl it; a held shell also fells the first beast it touches), Killer Hare, Fiery Lion (shoots fireballs), Bayeux Hound, Camel-Dragon, Wyvern, Spotted Unicorn (charges), Man-Faced Owl (swoops) and Soup Cat (throws soup).
- **Rules:** three hearts (Verdain has four) and three lives. 100 bezants earn an extra life. The sea and pits kill. Touch the goal pole, slide down it and walk into the castle. The higher you grasp the pole, the more glory: 100, 200, 400, 800, 2000 or 5000, and the very top gives an extra life. Points float up wherever they are won.
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
