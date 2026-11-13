using System.Collections.Generic;
using UnityEngine;

namespace Tapestry
{
    /// <summary>Runs the whole game: title & knight select, the level, camera, parallax, HUD and touch buttons.
    /// It builds everything from code, so it works in any scene (Bootstrap adds it automatically).</summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager I { get; private set; }

        enum State { Title, Playing, Paused, Dying, Won, GameOver, Entering, Briefing, Ending, Paywall, About, Tapestries }
        State state = State.Title;

        const float ViewHalfHeight = 5.625f;           // 540 design px tall = 11.25 tiles
        static readonly Color Linen = new Color(0.925f, 0.886f, 0.788f);
        static readonly Color Thread = new Color(0.2f, 0.145f, 0.114f);

        Camera cam;
        Transform world, backdrop;
        Level level;
        Knight player;
        Bayard bayard;
        readonly List<Enemy> enemies = new List<Enemy>();
        readonly List<Shot> shots = new List<Shot>();
        readonly List<Bezant> bezants = new List<Bezant>();
        readonly List<Debris> debris = new List<Debris>();
        readonly List<Item> items = new List<Item>();
        readonly List<Lift> lifts = new List<Lift>();
        Lift riding;   // the lift the knight stands on
        Lift pendingBalance;   // the first board of a balance pair, waiting for its other
        // halfway banner
        SpriteRenderer checkpointView;
        Vector2 checkpointPos;
        bool hasCheckpoint, checkpointClaimed;
        // what the knight carries on into the next level
        bool carry; bool carryLarge, carryCrossbow; string carryMount;
        // World 5: ye unicorne's horne and ye poysoned fountaine; and the hunt closing in (the chase)
        SpriteRenderer hornView, huntView; Vector2 hornPos, fountainPos; bool hasHorn, carryingHorn, hasFountain, purifiedSaved, toldFoul, toldFountain;
        float huntX, huntDelay; int hornCombo, pipeCount;
        // the bonus heaven up a beanstalk (a stretch of the same map far to the right of the level)
        bool hasBonus, inBonus, bonusToldOf; Vector2 bonusStart, bonusExit, bonusReturn; float bonusMinX, bonusMaxX;
        // World 7: falling stars, the four horsemen, and the Princesse (the finale)
        float starT, horsemanT; int horsemanNext;
        SpriteRenderer princessView; Vector2 princessPos; bool hasPrincess, rescuedPrincess;
        readonly List<Enemy> pendingEnemies = new List<Enemy>();
        // castles: flails, the boss, the axe that drops the bridge, and the rescued monk
        readonly List<Flail> flails = new List<Flail>();
        readonly List<Log> logs = new List<Log>();
        readonly List<WindHead> winds = new List<WindHead>();
        readonly List<Transform> wheels = new List<Transform>(); const float WheelSpeed = 0.6f; float wheelAngle;
        readonly List<Vector2> herds = new List<Vector2>(); readonly List<int> herdsSent = new List<int>(); float herdT;
        readonly List<Vector2> raftLaunches = new List<Vector2>(); readonly List<float> raftTimers = new List<float>();
        float houndT; bool houndsToldOf;
        const int HerdsPerStampede = 3;
        readonly List<Vector2> logPiles = new List<Vector2>(); readonly List<float> logTimers = new List<float>();
        BossBase boss;
        bool toldOfBoss;
        bool bossMusic, deathFire;
        int relicCombo;   // beasts smitten in a row by the holy relique
        int stompCombo;   // beasts stomped without the knight's feet touching the ground
        // the crossing: gusts of wind, and fish leaping from the sea
        float windT, wind, leapT; bool windToldOf;
        public bool Underwater { get { return L.Underwater; } }
        // the level's tyme: counts down Mario-fashion; at 100 the musick quickens; at 0 the knight is lost
        const float TymeTick = 0.4f;
        float tymeLeft; bool hurried, deathTyme; int tymeBonus;
        SpriteRenderer axeView; Vector2 axePos; bool hasAxe, collapsing;
        List<Vector2Int> collapseTiles; float collapseT, collapseDoneT;
        SpriteRenderer monkView; Vector2 monkPos; bool hasMonk, rescuedMonk;
        // title screen: 0 = choose a knight, 1 = choose a realme (only realmes already passed)
        int titleStep, realmChoice;
        readonly List<int> realmStarts = new List<int>();

        static string PassedKey(string title) { return "passed:" + title; }
        static bool Passed(string title) { return PlayerPrefs.GetInt(PassedKey(title), 0) == 1; }

        /// <summary>The first level of each realme (e.g. 1-3 starts at Ye Crypte Gate) that may be chosen:
        /// 1-1 always, and any realme already passed (in the Unity editor, all of them).</summary>
        void FindRealms()
        {
            realmStarts.Clear();
            string seen = null;
            for (int i = 0; i < Levels.All.Length; i++)
            {
                var t = Levels.All[i].Title;
                if (t == seen) continue;
                seen = t;
                bool open = i == 0 || Passed(t);
#if UNITY_EDITOR
                open = true;   // play-testing in the Unity editor: every realme may be chosen (phone builds still unlock as they are passed)
#endif
                if (open) realmStarts.Add(i);
            }
        }

        // ---- the one purchase: Worlds 2 to 7 are locked until "Ye Whole Tapestrie" is bought
        int paywallTarget;   // the level to ride on to once it is bought
        bool Locked(int level) { return Store.WorldLocked(WorldOf(Levels.All[level])); }
        int FirstLockedLevel() { for (int i = 0; i < Levels.All.Length; i++) if (Locked(i)) return i; return -1; }
        void OpenPaywall(int target) { paywallTarget = target; Store.Message = ""; SetState(State.Paywall); }

        void LeavePaywall(bool bought)
        {
            if (bought)
            {
                levelIndex = paywallTarget; checkpointClaimed = false; briefedTitle = null;
                LoadLevel();
                player.SetKnight(Knights.All[knightIndex]);
                Audio.Play("confirm");
                SetState(FirstState());
            }
            else { carry = false; levelIndex = 0; checkpointClaimed = false; LoadLevel(); SetState(State.Title); }
        }

        Rect PaywallRect(int i)   // 0 buy, 1 not now, 2 restore
        {
            float W = Screen.width, H = Screen.height;
            if (i == 2) return new Rect(W * 0.5f - U * 0.3f, H * 0.82f, U * 0.6f, U * 0.06f);
            float w = U * 0.52f, h = U * 0.09f, y = H * 0.66f;
            return i == 0 ? new Rect(W * 0.5f - w - U * 0.02f, y, w, h) : new Rect(W * 0.5f + U * 0.02f, y, w, h);
        }

        Rect UnlockLinkRect() { return new Rect(Screen.width * 0.5f - U * 0.35f, Screen.height * 0.9f, U * 0.7f, U * 0.07f); }
        Rect TapestriesLinkRect() { return new Rect(Screen.width * 0.02f, Screen.height * 0.9f, U * 0.42f, U * 0.07f); }
        Rect AboutLinkRect() { return new Rect(Screen.width * 0.98f - U * 0.42f, Screen.height * 0.9f, U * 0.42f, U * 0.07f); }

        // ---- the "what is this?" screen: shown the first time the game is opened, and from a link on the title screen
        // ---- "Ye Tapestries": the real tapestries behind the game, one page each (reached from the title screen)
        class TapestryInfo { public string Name, Where, Picture, Text, InGame; }
        static readonly TapestryInfo[] TapestryPages =
        {
            new TapestryInfo { Name = "Ye Bayeux Tapestrie", Where = "Worlde 1 and 2  -  embroidered in England in ye 1070s; now at Bayeux, Normandy", Picture = "backgrounds/far",
                Text = "About 68 metres long and half a metre tall, it telleth how William of Normandy conquered England, ending at ye Battle of Hastings in 1066. It is not truly a tapestry at all, but wool embroidered upon linen. It was most likely ordered by Bishop Odo of Bayeux, William's half-brother, and stitched by English embroiderers, perhaps at Canterbury. Look for ye mysterious lady named Aelfgyva, shown beside a priest: nobody knoweth who she was.",
                InGame = "Worlde 1 and ye sea crossing of Worlde 2 borrow its stitched wool-on-linen look, its horsemen, hounds and felled trees, and its longships. Princesse Aelfgyva is named for that mysterious lady." },
            new TapestryInfo { Name = "Ye Tapestrie of Creacioun", Where = "Worlde 3  -  embroidered about 1100; now in Girona Cathedral, Catalonia", Picture = "backgrounds/creation_far",
                Text = "A great Romanesque embroidery in wool, couched upon linen, of which about 3.7 by 4.7 metres survive. Christ sitteth at ye centre of circles; ye four winds blow in ye corners; and a border telleth ye months of ye yeare. Scholars date it to ye 11th or 12th century, and are not agreed what it was for: an altar canopy, a curtain, or a carpet.",
                InGame = "Worlde 3 is set in its sky: ye wind-heads that blow thee about, ye sun and moon, and ye Wheel of ye Yeare in a castle of turning wheels." },
            new TapestryInfo { Name = "Ye Devonshire Hunting Tapestries", Where = "Worlde 4  -  woven in ye Low Countries, probably at Arras, about 1425-1450; now in ye Victoria and Albert Museum, London", Picture = "backgrounds/verdure_far",
                Text = "Four huge woven hangings: a boar and bear hunt, an otter and swan hunt, a deer hunt, and falconry. They shew lords and ladies in fashionable dress hunting through woods crowded with leaves and flowers. Ye Dukes of Devonshire kept them at Hardwick Hall until they came to ye museum in 1957. From these, ye game's art turneth from stitched to woven.",
                InGame = "Worlde 4 hath ye hounds, boars, otters, swans and falcons of those hunts, ye dense leafy verdure, and ye Greate Boar." },
            new TapestryInfo { Name = "Ye Unicorne Tapestries", Where = "Worlde 5  -  seven hangings, about 1495-1505, probably designed in Paris and woven in Brussels; now at ye Cloisters, New York", Picture = "backgrounds/millefleur_far",
                Text = "Wool, silk and gilded thread. They follow hunters who seek, chase and at last take ye unicorn, who endeth chained to a pomegranate tree within a little fence. In one, ye unicorn dippeth its horn to purify a poisoned stream. Ye letters AE are woven into ye hangings, but nobody knoweth whose they were.",
                InGame = "Worlde 5 hath ye hunters and hounds, ye poisoned stream that ye unicorn's horn maketh pure, and ye unicorn in its fence: here it is spared, and becometh thy steede." },
            new TapestryInfo { Name = "Ye Ladye and ye Unicorne", Where = "Worlde 6  -  six hangings, about 1500, probably woven in Flanders from designs made in Paris; now in ye Musee de Cluny, Paris", Picture = "backgrounds/red_far",
                Text = "Five hangings shew ye senses: taste, hearing, sight, smell and touch. Ye sixth beareth ye words A mon seul desir (To my sole desire), and its meaning is still a mystery. A lady standeth on a blue island of flowers before a red ground strewn with flowers and little beastes, with a lion and a unicorn at her sides. Ye arms of ye Le Viste family appear upon them, but who ordered them is still debated.",
                InGame = "Worlde 6: Sight (ye mirrour), Hearing (ye organ), Taste and Smell, and ye Ladye's tente, where ye Lyon and ye Unicorne guard her." },
            new TapestryInfo { Name = "Ye Apocalypse of Angers", Where = "Worlde 7  -  woven in Paris in ye 1370s and 1380s; now in ye Chateau d'Angers, France", Picture = "backgrounds/apoc_far",
                Text = "Ordered by Louis I, Duke of Anjou, about 1373, from designs by Hennequin de Bruges, painter to King Charles V, and woven in ye workshop of Nicolas Bataille. It told ye Book of Revelation: ye seven trumpets, ye locusts, ye four horsemen and ye beast with seven heads. It was about 140 metres long, and roughly 100 metres survive. Its scenes sit against alternating red and blue grounds.",
                InGame = "Worlde 7: ye seven trumpets, ye crowned locusts, ye four horsemen and ye Beaste of Seven Heddes, then ye Cat King's own hall." },
            new TapestryInfo { Name = "Beastes, Bayard and ye Cat King", Where = "Whence ye rest came", Picture = "backgrounds/castle_far",
                Text = "Ye strange beastes come from medieval bestiaries: Aspidochelone, ye whale that sailors mistook for an island; ye cockatrice; ye hippocamp; ye man-faced owl. Bayard is named for ye magic horse of ye old romance of ye Four Sons of Aymon, who could carry all four brothers at once. Ye knights, ye Cat King and his Soup Cats are inventions of this game, and alle ye pictures were drawn for it, in ye manner of ye old tapestries. Ye true tapestries themselves are centuries old; if thou art ever near one, go and see it.",
                InGame = "" },
        };
        int tapestryPage;

        Rect TapPrevZone() { return new Rect(0, Screen.height * 0.2f, Screen.width * 0.16f, Screen.height * 0.7f); }
        Rect TapBackRect() { return new Rect(Screen.width * 0.5f - U * 0.3f, Screen.height * 0.92f, U * 0.6f, U * 0.07f); }

        void TickTapestries()
        {
            if (stateTime < 0.3f) return;
            bool next = TapestryInput.JumpPressed || TapestryInput.RightPressed || TapestryInput.StartPressed;
            bool prev = TapestryInput.LeftPressed, back = TapestryInput.RunPressed || TapestryInput.DownPressed;
            if (TapestryInput.TapPosition.HasValue)
            {
                var tap = TapestryInput.TapPosition.Value;
                if (TapBackRect().Contains(tap)) back = true;
                else if (TapPrevZone().Contains(tap)) prev = true;
                else next = true;
            }
            if (back) { Audio.Play("select"); SetState(State.Title); }
            else if (prev && tapestryPage > 0) { tapestryPage--; Audio.Play("select"); stateTime = 0f; }
            else if (next)
            {
                Audio.Play("select");
                if (++tapestryPage >= TapestryPages.Length) SetState(State.Title); else stateTime = 0f;
            }
        }

        void DrawTapestries(float W, float H)
        {
            Panel(new Rect(0, 0, W, H));
            var p = TapestryPages[tapestryPage];
            // a strip of the tapestry's own art, framed
            float bh = H * 0.27f, bw = Mathf.Min(W * 0.8f, bh * 4.4f); bh = bw / 4.4f;
            var br = new Rect((W - bw) * 0.5f, H * 0.03f, bw, bh);
            var c = GUI.color; GUI.color = new Color(0.2f, 0.145f, 0.114f, 0.9f);
            GUI.DrawTexture(new Rect(br.x - 5f, br.y - 5f, br.width + 10f, br.height + 10f), Texture2D.whiteTexture); GUI.color = c;
            var tex = Art.Texture(p.Picture);
            if (tex != null) GUI.DrawTextureWithTexCoords(br, tex, new Rect(0f, 0.3f, 1f, 0.4f));
            float y = br.yMax + H * 0.02f;
            GUI.Label(new Rect(0, y, W, U * 0.08f), p.Name, big); y += U * 0.085f;
            GUI.Label(new Rect(W * 0.1f, y, W * 0.8f, U * 0.08f), p.Where, center); y += U * 0.085f;
            GUI.Label(new Rect(W * 0.1f, y, W * 0.8f, H * 0.9f - y - U * 0.1f),
                p.Text + (string.IsNullOrEmpty(p.InGame) ? "" : "\n\nIn ye game:  " + p.InGame), tipText);
            var cc = GUI.color; GUI.color = new Color(1f, 1f, 1f, 0.55f + 0.45f * Mathf.Abs(Mathf.Sin(Time.time * 2.5f)));
            if (tapestryPage > 0) GUI.Label(new Rect(W * 0.01f, H * 0.5f, W * 0.1f, U * 0.1f), "<", big);
            GUI.Label(new Rect(W * 0.89f, H * 0.5f, W * 0.1f, U * 0.1f), ">", big);
            GUI.Label(TapBackRect(), "Returne  (" + (tapestryPage + 1) + " of " + TapestryPages.Length + ")", center);
            GUI.color = cc;
        }

        const string SeenAboutKey = "seen:about";
        int aboutPage;
        const int AboutPages = 4;
        void TickAbout()
        {
            if (stateTime < 0.35f) return;
            if (TapestryInput.JumpPressed || TapestryInput.StartPressed || TapestryInput.RightPressed || TapestryInput.TapPosition.HasValue)
            {
                Audio.Play("select");
                if (++aboutPage >= AboutPages)
                {
                    PlayerPrefs.SetInt(SeenAboutKey, 1); PlayerPrefs.Save();
                    SetState(State.Title);
                }
                else stateTime = 0f;
            }
            else if (TapestryInput.LeftPressed && aboutPage > 0) { aboutPage--; Audio.Play("select"); stateTime = 0f; }
        }

        static string[] AboutText(bool touch)
        {
            var tapestries = new System.Text.StringBuilder();
            for (int w = 1; w < WorldNames.Length; w++) tapestries.Append(w + "   " + WorldNames[w] + "\n");
            return new[]
            {
                "Ye Tapestrie Adventures is a two-dimensional, side-scrolling platform game: runne, leape and stompe from left to ryghte, over pittes and past perils, to ye pole at ye ende of every realme.\n\n" +
                "Every scene is stitched or woven like a tapestry of ye Middle Ages. Ye first three are wool embroidery upon linen, in ye manner of ye Bayeux Tapestrie; from ye fourth they are woven, thick with leaves and flowers, whyle thy knyghtes remayne stitched.\n\n" +
                "Thy foes are strange beastes lifted from ye bestiaries and ye margins of olde manuscripts.",

                (touch
                    ? "Moove with ye joystick: push it to ye rim to runne, pull it downe to crouche (or, when thou ridest, to dismounte), and push it up to climbe a beanstalk. Ye large button leapeth (holde it to leape higher); ye small one runneth, and shooteth a crossbowe if thou bearest one."
                    : "LEFTE and RYGHTE moove thee; holde RUNNE to runne, and it shooteth a crossbowe if thou bearest one. JUMPE leapeth (holde it to leape higher); DOWNE crouchethe (or dismounteth, when thou ridest), and UPPE climbeth a beanstalk. Crouche to duck spears and flung sweetes, or to creepe through a low passage.") + "\n\n" +
                "Thou begynnest small: one blow slayeth thee, and a small knyghte can but bumpe a brick, not breake it. A roaste capon maketh thee great, and a great knyght who is struck is but made small agayne. Stompe beastes from above; touche them from ye side and thou art hurte. Bumpe bricks from beneath (a great knyght breaketh them): some hide a bezant (a few give many, if thou bumpest them again and again), a capon (maketh thee great), a golden chalice (a lyfe; one is hidden in a block thou canst not see), a crossbowe (but a small knyghte findeth a capon there), a holy relique (nothing may harme thee for a whyle), or Bayard ye magic horse, whom thou mayst ryde.\n\n" +
                "Touche a halfway banner and thou shalt ryse agayne there. Mind ye Tyme, for it runneth out.",

                "Seven tapestries, four realmes in each, and a castle with a mighty boss at ye ende of every one:\n\n" + tapestries.ToString() + "\nTouche \"Ye Tapestries\" on ye title screene to learne of ye true tapestries that inspired them.",

                "Princesse Aelfgyva hath been carried off by ye Cat King, who fleeth ever deeper into ye tapestries.\n\n" +
                "Ryde after him with four brave knyghtes (Sir Rubeus, Dame Azura, Sir Verdain and Lady Aurelia), and Bayard ye magic horse at thy side. In every castle a good monke telleth thee that ye Princesse is in another; thou must overcome ye guardian of each, and at last ye Cat King himselfe upon his throne.\n\n" +
                "Before every realme thou shalt be tolde of its beastes and perils. Heede them well."
            };
        }

        static readonly string[] AboutHeads = { "What is this?", "How to playe", "Ye Seven Tapestries", "Ye Storye" };

        void DrawAbout(float W, float H)
        {
            Panel(new Rect(0, 0, W, H));
            var t = Art.Texture("scenery/title");
            float y = H * 0.03f;
            if (t != null) { float tw = Mathf.Min(W * 0.5f, U * 0.8f), th = tw * t.height / t.width; GUI.DrawTexture(new Rect((W - tw) * 0.5f, y, tw, th), t); y += th; }
            GUI.Label(new Rect(0, y, W, U * 0.08f), AboutHeads[aboutPage], big);
            y += U * 0.085f;
            GUI.Label(new Rect(W * 0.1f, y, W * 0.8f, H * 0.9f - y - U * 0.12f), AboutText(TapestryInput.ShowTouchControls)[aboutPage], center);
            if (aboutPage == 0)   // the four knights, in a row
            {
                float pw = U * 0.1f, gap = pw * 0.5f, total = 4 * pw + 3 * gap;
                for (int i = 0; i < Knights.All.Length; i++)
                    DrawFrame(Knights.All[i].Sheet, (int)(Time.time * 3f + i) % 3, new Rect((W - total) * 0.5f + i * (pw + gap), H * 0.8f, pw, pw * 1.5f));
            }
            float a = 0.55f + 0.45f * Mathf.Abs(Mathf.Sin(Time.time * 2.5f));
            var cc = GUI.color; GUI.color = new Color(1f, 1f, 1f, a);
            string go = aboutPage == AboutPages - 1 ? (TapestryInput.ShowTouchControls ? "Touche to begynne" : "JUMPE to begynne") : (TapestryInput.ShowTouchControls ? "Touche to continue" : "JUMPE to continue");
            GUI.Label(new Rect(0, H * 0.93f, W, U * 0.05f), go + "     (" + (aboutPage + 1) + " of " + AboutPages + ")", center);
            GUI.color = cc;
        }

        void TickPaywall()
        {
            if (Store.Owned) { LeavePaywall(true); return; }
            if (stateTime < 0.35f || Store.Busy) return;
            bool buy = TapestryInput.JumpPressed || TapestryInput.StartPressed, no = TapestryInput.DownPressed || TapestryInput.RunPressed;
            if (TapestryInput.TapPosition.HasValue)
            {
                var tap = TapestryInput.TapPosition.Value;
                if (PaywallRect(0).Contains(tap)) buy = true;
                else if (PaywallRect(1).Contains(tap)) no = true;
                else if (PaywallRect(2).Contains(tap)) { Store.Restore(); Audio.Play("select"); }
            }
            if (buy) { Audio.Play("select"); Store.Buy(); }
            else if (no) { Audio.Play("select"); LeavePaywall(false); }
        }

        void DrawPaywall(float W, float H)
        {
            Panel(new Rect(0, 0, W, H));
            var t = Art.Texture("scenery/title");
            if (t != null) { float tw = Mathf.Min(W * 0.7f, U * 1.0f), th = tw * t.height / t.width; GUI.DrawTexture(new Rect((W - tw) * 0.5f, H * 0.04f, tw, th), t); }
            GUI.Label(new Rect(0, H * 0.25f, W, U * 0.09f), "Ye Whole Tapestrie", big);
            GUI.Label(new Rect(W * 0.12f, H * 0.35f, W * 0.76f, U * 0.3f),
                "Thou hast ridden through ye Bayeux Tapestrie, and it was free.\n" +
                "Six more tapestries await: twenty-five realmes, seven mighty bosses, and ye Cat King himselfe.\n" +
                "One purchase, thyne for ever. No ads, and no further payments.", center);
            var c = GUI.color;
            for (int i = 0; i < 2; i++)
            {
                var r = PaywallRect(i);
                GUI.color = i == 0 ? new Color(0.66f, 0.26f, 0.18f, Store.Busy ? 0.25f : 0.55f) : new Color(0.2f, 0.145f, 0.114f, 0.18f);
                GUI.DrawTexture(r, Texture2D.whiteTexture);
                GUI.color = c;
                GUI.Label(r, i == 0 ? (Store.Busy ? "One moment..." : "Unlocke" + (string.IsNullOrEmpty(Store.Price) ? "" : "  " + Store.Price)) : "Not now", center);
            }
            GUI.Label(PaywallRect(2), "Restore a former purchase", center);
            if (!string.IsNullOrEmpty(Store.Message)) GUI.Label(new Rect(W * 0.1f, H * 0.78f, W * 0.8f, U * 0.05f), Store.Message, center);
            if (!TapestryInput.ShowTouchControls) GUI.Label(new Rect(0, H * 0.92f, W, U * 0.05f), "JUMPE to unlocke  -  DOWNE to returne", center);
        }

        void StartAt(int index)
        {
            if (Locked(index)) { carry = false; OpenPaywall(index); return; }
            levelIndex = index; checkpointClaimed = false;
            LoadLevel();
            player.SetKnight(Knights.All[knightIndex]);
            Audio.Play("confirm");
            briefedTitle = null;
            SetState(FirstState());
        }
        // the briefing before each level: its beasts, hazards and boss (shown once per realme, not after a death)
        string briefedTitle; List<Bestiary.Entry> briefing; Bestiary.Entry briefBoss;
        State FirstState()
        {
            if (L.Title == briefedTitle) return State.Playing;   // the later parts of a realme (1-3) were already told of
            briefedTitle = L.Title;
            briefing = Bestiary.For(L, out briefBoss);
            return briefing.Count > 0 || briefBoss != null ? State.Briefing : State.Playing;
        }
        string toast; float toastTime;
        class Popup { public Vector2 Pos; public string Text; public float T; }
        readonly List<Popup> popups = new List<Popup>();
        readonly List<Layer> layers = new List<Layer>();

        int knightIndex, bezantCount, lives = 3, score, levelIndex;

        // the saved best score ("Best Glorie"), Ye Wronge Syde (a harder second playing, unlocked by finishing the game), the bells at the pole
        const string BestKey = "best:glorie", BeatenKey = "beaten:game";
        int BestScore { get { return PlayerPrefs.GetInt(BestKey, 0); } }
        bool Beaten { get { return PlayerPrefs.GetInt(BeatenKey, 0) == 1; } }
        bool hardMode, newBest; int menuIndex;
        void RecordBest() { if (score > BestScore) { PlayerPrefs.SetInt(BestKey, score); PlayerPrefs.Save(); newBest = true; } }
        int bellsLeft; float bellT;
        class BellFx { public Transform T; public float Age; public Vector2 Base; }
        readonly List<BellFx> bellFx = new List<BellFx>();
        LevelDef L { get { return Levels.All[levelIndex]; } }
        string farKey, nearKey;
        float stateTime, camX, winTime;
        bool deathFell;
        int winPhase;   // 0 slide down the pole, 1 walk to the castle, 2 inside

        class Layer { public Transform[] Copies; public float Period, Speed; }

        // ------------------------------------------------------------------ setup
        void Awake()
        {
            if (I != null && I != this) { Destroy(gameObject); return; }
            I = this;
            Store.Init();
            bool firstTime = PlayerPrefs.GetInt("seen:about", 0) == 0;
            if (Application.isMobilePlatform) { Application.targetFrameRate = 60; Screen.sleepTimeout = SleepTimeout.NeverSleep; }
            SetupCamera();
            Audio.Init(gameObject);
            SetupBackdrop();
            LoadLevel();
            SetState(firstTime ? State.About : State.Title);
        }

        void SetupCamera()
        {
            cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera");
                go.tag = "MainCamera";
                cam = go.AddComponent<Camera>();
            }
            if (Object.FindAnyObjectByType<AudioListener>() == null) cam.gameObject.AddComponent<AudioListener>();
            cam.orthographic = true;
            cam.orthographicSize = ViewHalfHeight;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Linen;
            cam.transform.position = new Vector3(0f, ViewHalfHeight, -10f);
        }

        float HalfWidth { get { return ViewHalfHeight * cam.aspect; } }

        void SetupBackdrop()
        {
            // rebuilt when a level asks for different background layers
            if (backdrop != null && farKey == L.FarLayer && nearKey == L.NearLayer) return;
            if (backdrop != null) Destroy(backdrop.gameObject);
            layers.Clear();
            farKey = L.FarLayer; nearKey = L.NearLayer;
            backdrop = new GameObject("Backdrop").transform;
            AddLayer("backgrounds/linen", 0f, 0f, -100, 3);
            AddLayer(L.FarLayer, 0.25f, 0f, -90, 3);
            AddLayer(L.NearLayer, 0.5f, -0.6f, -80, 3);
            AddLayer("scenery/border", 1f, ViewHalfHeight * 2f - 64f / Art.DesignPerTile, 90, 7);
        }

        void AddLayer(string key, float speed, float y, int order, int copies)
        {
            var sheet = Art.Sheet(key);
            var l = new Layer { Speed = speed, Period = sheet.FrameW / Art.DesignPerTile, Copies = new Transform[copies] };
            for (int i = 0; i < copies; i++)
                l.Copies[i] = Art.Make(key, backdrop, new Vector2(0f, y), order).transform;
            layers.Add(l);
        }

        void UpdateLayers()
        {
            foreach (var l in layers)
            {
                float offset = camX * (1f - l.Speed);
                int k = Mathf.FloorToInt((camX - offset) / l.Period);
                int m = l.Copies.Length / 2;
                for (int i = 0; i < l.Copies.Length; i++)
                {
                    var p = l.Copies[i].localPosition;
                    p.x = offset + (k + i - m) * l.Period + l.Period * 0.5f;
                    l.Copies[i].localPosition = p;
                }
            }
        }

        void LoadLevel()
        {
            if (world != null) Destroy(world.gameObject);
            enemies.Clear(); shots.Clear(); bezants.Clear(); debris.Clear(); items.Clear(); lifts.Clear(); riding = null; pendingBalance = null; popups.Clear(); hasCheckpoint = false; checkpointView = null;
            windT = 3f; wind = 0f; leapT = 2f; windToldOf = false;
            tymeLeft = L.Time; hurried = false; deathTyme = false; Audio.MusicPitch(1f);
            flails.Clear(); winds.Clear(); wheels.Clear(); wheelAngle = 0f; herds.Clear(); herdsSent.Clear(); raftLaunches.Clear(); raftTimers.Clear(); houndT = 3f; houndsToldOf = false; herdT = 2f; logs.Clear(); showManicule = false; logPiles.Clear(); logTimers.Clear(); boss = null; toldOfBoss = false; bossMusic = false; hasAxe = collapsing = false; hasMonk = rescuedMonk = false; bayard = null; winPhase = 0; toast = null;
            hornView = huntView = null; pipeCount = 0; hasBonus = inBonus = false; bonusToldOf = false; bellsLeft = 0; bellFx.Clear(); starT = 3f; horsemanT = 4f; horsemanNext = 0; princessView = null; hasPrincess = rescuedPrincess = false; pendingEnemies.Clear(); hasHorn = carryingHorn = hasFountain = toldFoul = toldFountain = false; hornCombo = 0;
            SetupBackdrop();
            world = new GameObject("World").transform;
            level = new Level(L.Map, world, L);

            foreach (var d in L.Props)
                Art.Make(d.Key, world, new Vector2(d.X, level.GroundAt(d.X)), -20);
            // the castle at the end, centred on its doorway; drawn behind the knight, who walks in and vanishes
            if (!string.IsNullOrEmpty(L.Castle)) Art.Make(L.Castle, world, new Vector2(L.CastleX, level.GroundAt(L.CastleX)), -5);

            foreach (var s in level.Spawns)
            {
                Vector2 p = s.Value;
                string kind = null;
                switch (s.Key)
                {
                    case 'o': bezants.Add(new Bezant(p + new Vector2(0f, 0.25f), world, false)); break;
                    case 'G': Art.Make("scenery/goal_pole", world, p, 5); break;
                    case 's': kind = "snail_hound"; break;
                    case 'S': kind = "snail_moss"; break;
                    case 'g': kind = "gull"; break;
                    case 'x': kind = "crab"; break;
                    case 'b': kind = "bird"; break;
                    case 'v': herds.Add(p); herdsSent.Add(0); break;
                    case 'j': kind = "boar"; break;
                    case 'i': kind = "otter"; break;
                    case 'n': kind = "swan"; break;
                    case 'a': kind = "falcon"; break;
                    case 'r': raftLaunches.Add(p); raftTimers.Add(0f); break;
                    case '5': boss = new GreatBoar(p, world); break;
                    case '<': case '>': winds.Add(new WindHead(p, s.Key == '>', world, L.WindSkin)); break;
                    case 'f': kind = "fish"; break;
                    case 'z': kind = "sea_serpent"; break;
                    case 'm': kind = "sea_monk"; break;
                    case 'e': kind = "hippocamp"; break;
                    case 'Q': logPiles.Add(p); logTimers.Add(1.5f); Art.Make("scenery/log_pile", world, p, 3); break;
                    case 'F': flails.Add(new Flail(p, world, flails.Count % 2 == 0, flails.Count * 1.7f)); break;
                    case 'K': boss = new Boss(p, world); break;
                    case 'J': boss = new Whale(p, world); break;
                    case '4': boss = new FourWinds(p, world); break;
                    case 'Z':   // a turning wheel: its rim drawn behind, four star boards riding round it
                        var rim = new GameObject("wheel").transform; rim.SetParent(world, false);
                        rim.localPosition = new Vector3(p.x, p.y + 0.5f, 0f);
                        Art.Make("scenery/wheel_year", rim, new Vector2(0f, -3f), -4);
                        wheels.Add(rim);
                        for (int k = 0; k < 4; k++)
                            lifts.Add(Lift.OnWheel(new Vector2(p.x, p.y + 0.5f), 2.7f, k * Mathf.PI * 0.5f, WheelSpeed, world, L.LiftSkin));
                        break;
                    case 'A': hasAxe = true; axePos = p; axeView = Art.Make("scenery/bridge_axe", world, p, 5); break;
                    case 'N': hasMonk = true; monkPos = p; monkView = Art.Make("allies/monk", world, p, 9); monkView.flipX = true; break;
                    case 'C':
                        hasCheckpoint = true; checkpointPos = p;
                        checkpointView = Art.Make("scenery/checkpoint", world, p, 4, checkpointClaimed ? 1 : 0);
                        break;
                    case 'L': case 'E': case 'Y': case 'U':
                        var kindL = s.Key == 'E' ? Lift.Kind.BoardUpDown : s.Key == 'Y' ? Lift.Kind.ShipRocking : s.Key == 'U' ? Lift.Kind.ShipSailing : Lift.Kind.BoardSideways;
                        lifts.Add(new Lift(new Vector2(p.x - 0.5f, p.y + 1f), kindL, world, lifts.Count * 1.3f, L.LiftSkin));
                        break;
                    case 'ĺ': lifts.Add(new Lift(new Vector2(p.x - 0.5f, p.y + 1f), Lift.Kind.Falling, world, 0f, L.LiftSkin)); break;
                    case 'ǿ':   // balance boards come in pairs: each second one is hung against the one before it
                        var bal = new Lift(new Vector2(p.x - 0.5f, p.y + 1f), Lift.Kind.Balance, world, 0f, L.LiftSkin);
                        if (pendingBalance != null) { bal.Partner = pendingBalance; pendingBalance.Partner = bal; pendingBalance = null; } else pendingBalance = bal;
                        lifts.Add(bal);
                        break;
                    case 'h': kind = "killer_hare"; break;
                    case 'l': kind = "fiery_lion"; break;
                    case 'd': kind = "bayeux_hound"; break;
                    case 'c': kind = "camel_dragon"; break;
                    case 'w': kind = "wyvern"; break;
                    case 'u': kind = "unicorn"; break;
                    case 'O': kind = "owl_face"; break;
                    case 'k': kind = "soup_cat"; break;
                    case 'y': kind = "hunter"; break;
                    case 'q': kind = "greyhound"; break;
                    case 'p': kind = "pheasant"; break;
                    case 'ß': kind = "smith"; break;
                    case 'þ': kind = "snail_spike"; break;
                    case 'ȼ': kind = "bombard"; break;
                    case 'ƒ': kind = "snail_wing"; break;
                    case 'é': kind = "ember"; break;
                    case 'ø': kind = "urn_plant"; break;
                    case 'I': kind = "genet"; break;
                    case '{': kind = "lapdog"; break;
                    case '}': kind = "fox"; break;
                    case '@': kind = "monkey"; break;
                    case '!': kind = "parakeet"; break;
                    case '$': boss = new TwinBoss(p, world); break;
                    case '2': boss = new Hydra(p, world); break;
                    case '3': boss = new CatKing(p, world); break;
                    case '&': kind = "locust"; break;
                    case ']': hasPrincess = true; princessPos = p; princessView = Art.Make("princess/aelfgyva", world, p, 9); princessView.flipX = true; break;
                    case '9': lifts.Add(new Lift(new Vector2(p.x - 0.5f, p.y + 1f), Lift.Kind.OrganPipe, world, pipeCount++ % 3)); break;
                    case '6': boss = new SpottedUnicorn(p, world); break;
                    case '[': hasBonus = true; bonusStart = p; break;
                    case ':': bonusExit = p; Art.Make("scenery/vine", world, new Vector2(p.x, p.y + 1f), 2); Art.Make("scenery/vine", world, p, 2); break;   // a hanging vine: the way down
                    case '0': hasFountain = true; fountainPos = p; Art.Make("scenery/fountain", world, p, -4); break;
                    case '+':
                        if (hasCheckpoint && checkpointClaimed && purifiedSaved) break;   // the waters were already made pure
                        hasHorn = true; hornPos = p + new Vector2(0f, 0.5f); hornView = Art.Make("items/horn", world, hornPos, 12); break;
                }
                if (kind != null) enemies.Add(new Enemy(kind, p, world));
            }
            if (hasBonus) { bonusMinX = bonusStart.x - 3f; bonusMaxX = Mathf.Min(level.Width, bonusExit.x + 4f); if (bonusExit.x <= bonusStart.x) hasBonus = false; }
            var start = hasCheckpoint && checkpointClaimed ? checkpointPos : level.Start;
            if (hasCheckpoint && checkpointClaimed && purifiedSaved) level.Purify();
            if (L.Chase) { huntX = start.x - 14f; huntDelay = 2.5f; huntView = Art.Make("scenery/hunt_line", world, new Vector2(huntX, 2f), 15); }
            player = new Knight(Knights.All[knightIndex], start, world);
            if (carry)
            {
                if (carryMount != null && L.Underwater) Toast((carryMount == "unicorn" ? "Ye Unicorne" : "Bayard") + " waiteth on ye shore. Swimme on!");
                player.ApplyCarry(carryLarge, carryCrossbow, L.Underwater ? null : carryMount);   // no steed beneath the waves
                carry = false;
            }
            camX = ClampCam(start.x + 4f);
            PlaceCamera();
        }

        float ClampCam(float x)
        {
            float lo = HalfWidth, hi = Mathf.Max(HalfWidth, (hasBonus ? bonusMinX : level.Width) - HalfWidth);   // the level ends where the heaven begins
            if (inBonus) { lo = bonusMinX + HalfWidth; hi = Mathf.Max(lo, bonusMaxX - HalfWidth); }
            return Mathf.Clamp(x, lo, hi);
        }

        /// <summary>The knight has climbed to the top of a beanstalk: up to the heaven of bezants.</summary>
        public void EnterBonus(int vineX)
        {
            int b = level.VineBottom(vineX);
            EnterBonusAt(new Vector2(vineX + 0.5f, b >= 0 ? b : player.Body.Pos.y), "Ye heaven of bezantes! Gather alle thou canst, then ryde ye vine downe.");   // back down to the top of the beanstalk brick
        }

        /// <summary>The knight has climbed into a well: down to a secret cellar of bezants.</summary>
        public void EnterWell(int wx, int wy)
        {
            EnterBonusAt(new Vector2(wx + 0.5f, wy + 1f), "Ye secret cellar! Gather ye bezantes, then climbe ye rope.");   // and back out on top of the well
        }

        void EnterBonusAt(Vector2 returnPos, string welcome)
        {
            if (!hasBonus || inBonus) return;
            bonusReturn = returnPos;
            inBonus = true;
            player.Body.Pos = bonusStart; player.Body.Vel = Vector2.zero;
            camX = ClampCam(bonusStart.x + 4f); PlaceCamera();
            Audio.Play("powerup");
            if (!bonusToldOf) { bonusToldOf = true; Toast(welcome); }
        }

        void ExitBonus()
        {
            inBonus = false;
            player.Body.Pos = bonusReturn; player.Body.Vel = Vector2.zero;
            camX = ClampCam(bonusReturn.x + 4f); PlaceCamera();
            Audio.Play("item_appear");
        }

        void PlaceCamera()
        {
            cam.transform.position = new Vector3(camX, ViewHalfHeight, -10f);
            UpdateLayers();
        }

        // ------------------------------------------------------------------ loop
        void Update()
        {
            TapestryInput.Poll();
            if (TapestryInput.MutePressed) Audio.ToggleMute();
            float dt = Mathf.Min(Time.deltaTime, 1f / 30f);
            stateTime += dt;
            switch (state)
            {
                case State.Title: TickTitle(); break;
                case State.Playing: TickPlay(dt); break;
                case State.Paused:
                    if (TapestryInput.TapPosition.HasValue)
                    {
                        if (PauseButtonRect(0).Contains(TapestryInput.TapPosition.Value)) { SetState(State.Playing); break; }
                        if (PauseButtonRect(1).Contains(TapestryInput.TapPosition.Value)) { levelIndex = 0; checkpointClaimed = false; LoadLevel(); SetState(State.Title); break; }
                        if (PauseButtonRect(2).Contains(TapestryInput.TapPosition.Value)) { Audio.ToggleMute(); break; }
                    }
                    if (TapestryInput.StartPressed || TapestryInput.JumpPressed) SetState(State.Playing);
                    else if (TapestryInput.DownPressed) { levelIndex = 0; checkpointClaimed = false; LoadLevel(); SetState(State.Title); }
                    break;
                case State.Dying:
                    player.Tick(dt, level);
                    if (stateTime > 2.6f)
                    {
                        lives--;
                        if (lives > 0) { LoadLevel(); SetState(State.Playing); }
                        else SetState(State.GameOver);
                    }
                    break;
                case State.Won:
                    TickWin(dt);
                    player.Tick(dt, level);
                    TickEntities(dt);
                    if (winPhase == 3 && winTime > 4f) { SetState(State.Ending); break; }
                    if (winPhase == 2 && winTime > 1.2f && (TapestryInput.JumpPressed || TapestryInput.StartPressed || TapestryInput.TapPosition.HasValue))
                    {
                        if (levelIndex + 1 < Levels.All.Length) NextLevel();
                        else { levelIndex = 0; checkpointClaimed = false; LoadLevel(); SetState(State.Title); }
                    }
                    break;
                case State.Briefing:
                    if (stateTime > 0.5f && (TapestryInput.JumpPressed || TapestryInput.StartPressed || TapestryInput.TapPosition.HasValue))
                    { Audio.Play("select"); SetState(State.Playing); }
                    break;
                case State.Paywall: TickPaywall(); break;
                case State.About: TickAbout(); break;
                case State.Tapestries: TickTapestries(); break;
                case State.Ending:   // the story told, scrolling up; then back to the title
                    if (stateTime > 5f && (TapestryInput.JumpPressed || TapestryInput.StartPressed || TapestryInput.TapPosition.HasValue) || stateTime > EndingLength)
                    { levelIndex = 0; checkpointClaimed = false; LoadLevel(); SetState(State.Title); }
                    break;
                case State.Entering:   // walked into the crypt door: fade, then on to the next part
                    if (stateTime > 1.1f) NextLevel();
                    break;
                case State.GameOver:
                    if (stateTime > 1.5f && (TapestryInput.JumpPressed || TapestryInput.StartPressed || TapestryInput.TapPosition.HasValue))
                    { lives = 3; bezantCount = 0; score = 0; newBest = false; levelIndex = 0; checkpointClaimed = false; LoadLevel(); SetState(State.Title); }
                    break;
            }
        }

        string LevelMusic { get { return bossMusic ? (L.Finale ? "finale" : "boss") : L.Music; } }
        bool relicMusic;

        void NextLevel()
        {
            carry = true; carryLarge = player.Large; carryCrossbow = player.HasCrossbow; carryMount = player.Mounted ? player.MountKind : null;
            if (levelIndex + 1 < Levels.All.Length && Locked(levelIndex + 1)) { OpenPaywall(levelIndex + 1); return; }   // the end of the free tapestry
            levelIndex++;
            checkpointClaimed = false;
            LoadLevel();
            SetState(FirstState());
        }

        void SetState(State s)
        {
            var was = state;
            if (s == State.Title || s == State.GameOver || s == State.Won || s == State.Ending) RecordBest();
            if (s == State.Title) titleStep = 0;
            state = s; stateTime = 0f;
            switch (s)
            {
                case State.Title: Audio.Music("title"); break;
                case State.Briefing: Audio.Music(L.Music); break;
                case State.About: Audio.Music("title"); if (was != State.About) aboutPage = 0; break;
                case State.Tapestries: if (was != State.Tapestries) tapestryPage = 0; break;
                case State.Ending: Audio.Music("ending", false); PlayerPrefs.SetInt(BeatenKey, 1); PlayerPrefs.Save(); break;   // Ye Wronge Syde is unlocked
                case State.Playing:
                    if (was == State.Paused) Audio.ResumeMusic();
                    else if (was == State.Briefing) Toast(L.Title + "  ·  " + L.Name);
                    else { relicMusic = false; Audio.Music(LevelMusic); Toast(L.Title + "  ·  " + L.Name); }
                    break;
                case State.Paused: Audio.PauseMusic(); Audio.Play("pause"); break;
                case State.Dying: Audio.Music("death", false); break;
                case State.Won: Audio.Play("pole"); Audio.Music("victory", false); break;
                case State.Entering: Audio.Play("pole", 0.7f); break;
                case State.GameOver: Audio.Music("game_over", false); break;
            }
        }

        /// <summary>The holy relique has its own tune while it lasts.</summary>
        /// <summary>Mario-fashion chains: 100, 200, 400, 800 for the first four beasts in a row; the 5th and every one after is a free lyfe.</summary>
        void ChainKill(int n, Vector2 at)
        {
            if (n <= 4) { AddScore(100 << (n - 1), at); return; }
            lives++;
            popups.Add(new Popup { Pos = at, Text = "1 LYFE!" });
            Audio.Play("oneup");
        }

        /// <summary>Gusts: calm for a while, then the wind blows (alternately from ahead and behind) for a couple of seconds.
        /// Leaping fish: every so often a fish bursts from the sea near the knight.</summary>
        void TickWindAndSea(float dt)
        {
            if (L.Wind && !player.Dead)
            {
                windT -= dt;
                if (windT <= 0f)
                {
                    if (wind == 0f)
                    {
                        wind = Random.value < 0.6f ? -2.4f : 2.0f;   // more often against you
                        windT = 2.2f;
                        Audio.Play("fireball", 0.5f);
                        if (!windToldOf) { windToldOf = true; Toast("Ye wynde bloweth! Brace thyself."); }
                    }
                    else { wind = 0f; windT = 3.5f + Random.value * 2f; }
                }
                if (wind != 0f) player.Body.Nudge(level, wind * dt);
            }
            if (L.LeapingFish && !player.Dead)
            {
                leapT -= dt;
                if (leapT <= 0f)
                {
                    leapT = 1.4f + Random.value * 1.2f;
                    float x = player.Body.Pos.x + (Random.value * 2f - 1f) * HalfWidth * 0.8f;
                    float d = Random.value < 0.5f ? -1f : 1f;
                    var e = new Enemy("leaper", new Vector2(x, -1.5f), world);
                    e.Active = true;
                    e.Body.Vel = new Vector2(d * 2.6f, 13.5f);
                    enemies.Add(e);
                    Audio.Play("soup", 0.4f);
                }
            }
        }

        /// <summary>A stampede: while the knight is in the stretch of meadow before a herd marker, a herd of three stags
        /// comes thundering in from the right every few seconds.</summary>
        void TickStampede(float dt)
        {
            if (herds.Count == 0 || player.Dead) return;
            int stretch = -1;
            for (int i = 0; i < herds.Count; i++)
                if (player.Body.Pos.x > herds[i].x - 24f && player.Body.Pos.x < herds[i].x + 2f) stretch = i;
            if (stretch < 0 || herdsSent[stretch] >= HerdsPerStampede) return;   // each stampede sends three herds, no more
            foreach (var e in enemies) if (e.Kind == "stag" && e.Alive) return;   // the last herd must be gone first
            herdT -= dt;
            if (herdT > 0f) return;
            herdT = 5.5f;
            herdsSent[stretch]++;
            for (int i = 0; i < 3; i++)
            {
                float x = camX + HalfWidth + 1.5f + i * 1.6f;
                var st = new Enemy("stag", new Vector2(x, level.GroundAt(x)), world) { Active = true };
                enemies.Add(st);
            }
            Audio.Play("mount", 0.6f);
            Toast("A stampede! Leap them or stomp them!");
        }

        /// <summary>The hunt's hounds loosed after the knight from behind; and rafts launched downstream on the river.</summary>
        void TickHuntAndRiver(float dt)
        {
            if (L.HuntingHounds && !player.Dead && player.Body.Pos.x < level.Width - 30f)
            {
                houndT -= dt;
                if (houndT <= 0f)
                {
                    houndT = 4.5f;
                    float x = camX - HalfWidth - 1f;
                    if (x > 1f)
                    {
                        enemies.Add(new Enemy("talbot", new Vector2(x, level.GroundAt(x)), world) { Active = true });
                        Audio.Play("horn", 0.8f);
                        if (!houndsToldOf) { houndsToldOf = true; Toast("Ye hounds are loosed! Runne, or stomp them!"); }
                    }
                }
            }
            for (int i = 0; i < raftLaunches.Count; i++)
            {
                if (Mathf.Abs(raftLaunches[i].x - player.Body.Pos.x) > HalfWidth + 14f) continue;
                raftTimers[i] -= dt;
                if (raftTimers[i] <= 0f)
                {
                    raftTimers[i] = 3.6f;
                    // each raft sails from its launch to the far bank, sinking as its prow touches the shore
                    int bank = Mathf.FloorToInt(raftLaunches[i].x) + 1, ry = Mathf.FloorToInt(raftLaunches[i].y);
                    while (bank < level.Width && level.Water(bank, ry)) bank++;
                    float run = Mathf.Max(1f, bank - 3f - (raftLaunches[i].x - 0.5f));
                    lifts.Add(Lift.Raft(new Vector2(raftLaunches[i].x - 0.5f, raftLaunches[i].y + 1f), run, world));
                }
            }
            if (riding != null && riding.Gone) riding = null;
            lifts.RemoveAll(l => l.Gone);
        }

        void UpdateRelicMusic()
        {
            if (!player.Blessed) relicCombo = 0;
            if (player.Blessed && !relicMusic) { relicMusic = true; Audio.Music("relic"); }
            else if (!player.Blessed && relicMusic) { relicMusic = false; Audio.Music(LevelMusic); }
        }

        /// <summary>After touching the pole: slide down it, walk into the castle, then show the verse.</summary>
        void TickWin(float dt)
        {
            // the bells peal, 500 glorie each
            if (bellsLeft > 0)
            {
                bellT -= dt;
                if (bellT <= 0f)
                {
                    bellT = 0.5f; bellsLeft--;
                    float bx = L.CastleX + Random.Range(-4f, 4f), by = level.GroundAt(L.CastleX) + Random.Range(3.5f, 6.5f);
                    bellFx.Add(new BellFx { T = Art.Make("scenery/bell", world, new Vector2(bx, by), 30).transform, Age = 0f, Base = new Vector2(bx, by) });
                    Audio.Play("bezant", 1f);
                    AddScore(500, new Vector2(bx, by + 0.9f));
                }
            }
            for (int i = bellFx.Count - 1; i >= 0; i--)
            {
                var b = bellFx[i]; b.Age += dt;
                if (b.T == null || b.Age > 1.3f) { if (b.T != null) Destroy(b.T.gameObject); bellFx.RemoveAt(i); continue; }
                b.T.localPosition = new Vector3(b.Base.x, b.Base.y + b.Age * 0.8f, 0f);
                b.T.localEulerAngles = new Vector3(0f, 0f, Mathf.Sin(b.Age * 18f) * 14f);   // swinging
            }
            var pb = player.Body;
            winTime += dt;
            float ground = level.GroundAt(pb.Pos.x);
            if (winPhase == 0)
            {
                pb.Vel = Vector2.zero;
                pb.Pos.y = Mathf.Max(ground, pb.Pos.y - 7f * dt);
                if (pb.Pos.y <= ground && winTime > 0.4f) { winPhase = 1; winTime = 0f; pb.Pos.x = level.GoalX + pb.W * 0.5f + 0.1f; }
            }
            else if (winPhase == 1)
            {
                pb.Vel = new Vector2(3.2f, 0f);
                pb.Pos.x += 3.2f * dt;
                pb.Pos.y = level.GroundAt(pb.Pos.x);
                if (pb.Pos.x >= L.CastleX || winTime > 6f) { winPhase = 2; winTime = 0f; pb.Vel = Vector2.zero; player.Hide(); }
            }
        }

        void OnApplicationPause(bool paused) { if (paused && state == State.Playing) SetState(State.Paused); }

        void TickTitle()
        {
            if (titleStep == 1) { TickRealmSelect(); return; }
            int n = Knights.All.Length;
            if (TapestryInput.LeftPressed) { knightIndex = (knightIndex + n - 1) % n; Audio.Play("select"); }
            if (TapestryInput.RightPressed) { knightIndex = (knightIndex + 1) % n; Audio.Play("select"); }
            int items = Beaten ? 2 : 1;
            if (menuIndex >= items) menuIndex = 0;
            if (items > 1 && (TapestryInput.UpPressed || TapestryInput.DownPressed)) { menuIndex = 1 - menuIndex; Audio.Play("select"); }
            // the Up key also jumps, so only a real jump button (or ENTER) starts the game from the title
            bool go = stateTime > 0.3f && (TapestryInput.JumpOnlyPressed || TapestryInput.StartPressed);
            if (TapestryInput.TapPosition.HasValue && TapestriesLinkRect().Contains(TapestryInput.TapPosition.Value))
            { Audio.Play("select"); SetState(State.Tapestries); return; }
            if (TapestryInput.TapPosition.HasValue && AboutLinkRect().Contains(TapestryInput.TapPosition.Value))
            { Audio.Play("select"); SetState(State.About); return; }
            if (TapestryInput.TapPosition.HasValue && !Store.Owned && UnlockLinkRect().Contains(TapestryInput.TapPosition.Value))
            { Audio.Play("select"); OpenPaywall(FirstLockedLevel()); return; }
            if (TapestryInput.TapPosition.HasValue)
            {
                var tap = TapestryInput.TapPosition.Value;
                for (int i = 0; i < n; i++)
                    if (PortraitRect(i).Contains(tap))
                    {
                        if (i == knightIndex) go = true;
                        else Audio.Play("select");
                        knightIndex = i;
                    }
                for (int m = 0; m < items; m++)
                    if (MenuRect(m).Contains(tap))
                    {
                        if (m == menuIndex) go = true; else Audio.Play("select");
                        menuIndex = m;
                    }
            }
            if (go)
            {
                hardMode = menuIndex == 1;
                Enemy.TimeScale = hardMode ? 1.3f : 1f;
                FindRealms();
                if (realmStarts.Count <= 1) { StartAt(0); return; }
                titleStep = 1; realmChoice = realmStarts.Count - 1; stateTime = 0f;   // offer the furthest realme first
                Audio.Play("select");
            }
        }

        void TickRealmSelect()
        {
            int n = realmStarts.Count;
            if (TapestryInput.LeftPressed) { realmChoice = (realmChoice + n - 1) % n; Audio.Play("select"); }
            if (TapestryInput.RightPressed) { realmChoice = (realmChoice + 1) % n; Audio.Play("select"); }
            if (TapestryInput.RunPressed) { titleStep = 0; Audio.Play("select"); return; }   // back to the knights
            bool go = stateTime > 0.25f && (TapestryInput.JumpPressed || TapestryInput.StartPressed);
            if (TapestryInput.TapPosition.HasValue)
            {
                var tap = TapestryInput.TapPosition.Value;
                var page = RealmPage(realmChoice);
                for (int k = 0; k < page.Count; k++)
                    if (RealmRect(k, page.Count).Contains(tap))
                    {
                        if (page[k] == realmChoice) go = true; else Audio.Play("select");
                        realmChoice = page[k];
                    }
                if (PageArrowRect(-1).Contains(tap)) TurnPage(-1);
                if (PageArrowRect(1).Contains(tap)) TurnPage(1);
            }
            if (go) { titleStep = 0; StartAt(realmStarts[realmChoice]); }
        }

        // The realme select shows one worlde at a time (its passed levels, up to four), with arrows to turn the page.
        static readonly string[] WorldNames = { "", "Ye Bayeux Tapestrie", "Ye Crossing of ye Sea", "Ye Tapestrie of Creacioun",
            "Ye Hunting Tapestries", "Ye Unicorne Tapestries", "Ye Lady and ye Unicorne", "Ye Apocalypse" };
        static int WorldOf(LevelDef d)
        {
            int k = d.Title.IndexOf(' ');
            int w; return k >= 0 && int.TryParse(d.Title.Substring(k + 1).Split('-')[0], out w) ? w : 1;
        }
        int RealmWorld(int i) { return WorldOf(Levels.All[realmStarts[i]]); }
        /// <summary>The realmes (indices into realmStarts) on the same page as realme i.</summary>
        List<int> RealmPage(int i)
        {
            var page = new List<int>(); int w = RealmWorld(i);
            for (int j = 0; j < realmStarts.Count; j++) if (RealmWorld(j) == w) page.Add(j);
            return page;
        }
        Rect RealmRect(int slot, int count)
        {
            float w = Mathf.Min(Screen.width * 0.7f / 4f - 14f, U * 0.34f), h = U * 0.2f;
            float total = count * w + (count - 1) * 14f;
            return new Rect((Screen.width - total) * 0.5f + slot * (w + 14f), Screen.height * 0.44f, w, h);
        }
        Rect PageArrowRect(int side)
        {
            float s = U * 0.13f;
            return new Rect(side < 0 ? Screen.width * 0.08f : Screen.width * 0.92f - s, Screen.height * 0.44f + U * 0.035f, s, s);
        }
        void TurnPage(int side)
        {
            int w = RealmWorld(realmChoice);
            if (side > 0) { for (int j = 0; j < realmStarts.Count; j++) if (RealmWorld(j) > w) { realmChoice = j; break; } }
            else { int prev = -1; for (int j = 0; j < realmStarts.Count; j++) if (RealmWorld(j) < w) prev = j; if (prev >= 0) { var pg = RealmPage(prev); realmChoice = pg[pg.Count - 1]; } }
            Audio.Play("select");
        }

        void TickPlay(float dt)
        {
            if (TapestryInput.StartPressed) { SetState(State.Paused); return; }
            if (!collapsing && !rescuedMonk)
            {
                tymeLeft -= dt / TymeTick * (hardMode ? 1.25f : 1f);
                if (!hurried && tymeLeft <= 100f)
                {
                    hurried = true;
                    Audio.Play("pause"); Audio.MusicPitch(1.15f);
                    Toast("Make haste! Ye tyme groweth shorte!");
                }
                if (tymeLeft <= 0f && !player.Dead) { tymeLeft = 0f; deathTyme = true; player.Die(); return; }
            }
            if (TapestryInput.NextKnightPressed) SwitchKnight(1);
            if (TapestryInput.PrevKnightPressed) SwitchKnight(-1);

            level.Tick(dt);
            foreach (var l in lifts) { l.Stood = l.StoodFrame; l.StoodFrame = false; }
            foreach (var l in lifts) l.Tick(dt);
            wheelAngle += WheelSpeed * dt;
            foreach (var w in wheels) w.localEulerAngles = new Vector3(0f, 0f, wheelAngle * Mathf.Rad2Deg);
            foreach (var f in flails) f.Tick(dt);
            if (collapsing) TickCollapse(dt);
            player.Tick(dt, level);
            TickWindAndSea(dt);
            foreach (var w in winds) w.Tick(dt, level, player);
            TickStampede(dt);
            TickHuntAndRiver(dt);
            TickChase(dt);
            TickStarsAndHorsemen(dt);
            TickEntities(dt);
            if (player.Dead) return;
            UpdateRelicMusic();

            var pb = player.Body;
            if (pb.Grounded) stompCombo = 0;
            if (Mathf.Abs(pb.Vel.x) < 9.5f) hornCombo = 0;
            foreach (var e in enemies)
            {
                if (!e.Active || !e.Alive || e.Carried || e.Untouchable || !pb.Overlaps(e.Body)) continue;
                // a stomp if we were falling and our feet were in the top half of the beast last frame
                if (player.Blessed)   // the holy relique smites any beast it touches - and the glory doubles each time
                {
                    var at = e.Body.Pos + new Vector2(0f, e.Body.H);
                    e.Flip(false);
                    ChainKill(++relicCombo, at);
                    continue;
                }
                // the unicorn's horn: at a gallop it fells any beast it runs into
                if (player.OnUnicorn && Mathf.Abs(pb.Vel.x) > 9.5f && Mathf.Sign(e.Body.Pos.x - pb.Pos.x) == Mathf.Sign(pb.Vel.x))
                {
                    var at = e.Body.Pos + new Vector2(0f, e.Body.H);
                    e.Flip(false); Audio.Play("kick");
                    ChainKill(++hornCombo, at);
                    continue;
                }
                bool fromAbove = !Underwater && pb.Vel.y < 0f && pb.PrevBottom >= e.Body.Pos.y + e.Body.H * (player.Mounted ? 0.35f : 0.5f);   // under water nothing can be stomped
                var stompAt = e.Body.Pos + new Vector2(0f, e.Body.H);
                if (e.OnKnight(player, fromAbove)) ChainKill(++stompCombo, stompAt);   // stomp after stomp without landing: the glory doubles
                if (player.Dead) return;
            }
            foreach (var s in enemies)
            {
                if (!s.Alive || !(s.ShellMoving || s.Carried)) continue;
                foreach (var o in enemies)
                    if (o != s && o.Alive && o.Active && !o.Carried && s.Body.Overlaps(o.Body))
                    {
                        var at = o.Body.Pos + new Vector2(0f, o.Body.H);
                        o.Flip(false);
                        ChainKill(++s.ShellCombo, at);     // a sliding shell's glory doubles with every beast in a row
                        if (s.Carried) { s.Flip(); break; }   // a held shell is spent on the first beast it strikes
                    }
            }
            if (BossHoldsArena && pb.Right > boss.ArenaMax) { pb.Pos.x = boss.ArenaMax - pb.W * 0.5f; if (pb.Vel.x > 0f) pb.Vel.x = 0f; }
            foreach (var f in flails)
                if (f.Hits(pb)) player.Hurt();
            foreach (var lg in logs)
            {
                if (lg.Removed || lg.Knocked || !pb.Overlaps(lg.Body)) continue;
                if (player.Blessed) { lg.Destroy(); AddScore(200, lg.Body.Pos + new Vector2(0f, 1f)); continue; }
                bool onTop = pb.Vel.y < 0f && pb.PrevBottom >= lg.Body.Pos.y + lg.Body.H * 0.5f;
                if (onTop) { player.Bounce(); lg.Knock(); Audio.Play("kick"); AddScore(100, lg.Body.Pos + new Vector2(0f, 1f)); }   // stomped: it tumbles away
                else player.Hurt();
            }
            if (boss != null) boss.Contact(player);
            foreach (var s in shots)
            {
                if (s.Removed) continue;
                if (!s.Friendly)
                {
                    if (s.Body.Overlaps(pb))
                    {
                        if (s.Kind == "gaze") player.Petrify(1.3f); else player.Hurt();
                        s.Destroy();
                    }
                    continue;
                }
                if (boss != null && boss.BoltHits(s.Body)) { s.Destroy(); continue; }
                foreach (var lg in logs)   // a bolt splits a log
                    if (!lg.Removed && s.Body.Overlaps(lg.Body)) { lg.Destroy(); s.Destroy(); AddScore(200, lg.Body.Pos + new Vector2(0f, 1f)); break; }
                if (s.Removed) continue;
                foreach (var e in enemies)   // a crossbowe bolt fells any beast, burning lions too
                    if (e.Active && e.Alive && !e.Carried && !e.Untouchable && e.Kind != "ember" && e.Kind != "bombard" && s.Body.Overlaps(e.Body)) { e.Flip(); s.Destroy(); break; }
            }
            foreach (var it in items)
                if (!it.Removed && it.Ready && it.Body.Overlaps(pb)) { Collect(it); it.Destroy(); }
            foreach (var b in bezants)
                if (!b.Removed && b.Touches(pb)) { b.Destroy(); AddBezant(); }
            if (bayard != null && !bayard.Removed && bayard.CanMount && pb.Overlaps(bayard.Body)
                && (!player.Mounted || (bayard.Kind == "unicorn" && !player.OnUnicorn)))   // from Bayard, a knight may change to the unicorn
            {
                string old = player.Mounted ? player.MountKind : null;
                var at = bayard.Body.Pos;
                player.Mount(bayard.Kind);
                bayard.Destroy();
                bayard = null;
                if (old != null) { bayard = new Bayard(at, world, old); bayard.Calm(); Toast("Thou rydest ye Unicorne! Bayard shall wayte for thee."); }
            }

            deathFire = level.Fire(Mathf.FloorToInt(pb.Pos.x), Mathf.FloorToInt(pb.Pos.y + 0.3f));
            if (pb.Pos.y < -0.6f || deathFire || level.Water(Mathf.FloorToInt(pb.Pos.x), Mathf.FloorToInt(pb.Pos.y + 0.3f))) { player.Die(true); return; }

            // strike the axe: the drawbridge falls, and whatever stands on it
            if (hasAxe && !collapsing && pb.Right > axePos.x - 0.4f && pb.Left < axePos.x + 0.4f && pb.Pos.y < axePos.y + 2f)
            {
                collapsing = true; collapseT = collapseDoneT = 0f;
                collapseTiles = level.BridgeTiles();
                player.Freeze();
                if (axeView != null) Destroy(axeView.gameObject);
                Audio.Play("kick");
            }

            // the bonus heaven: held within its own stretch of the map, and nothing there counts as progress through the level
            if (hasBonus)
            {
                if (inBonus)
                {
                    pb.Pos.x = Mathf.Clamp(pb.Pos.x, bonusMinX + pb.W * 0.5f, bonusMaxX - pb.W * 0.5f);
                    if (Mathf.Abs(pb.Pos.x - bonusExit.x) < 1.0f && Mathf.Abs(pb.Pos.y - bonusExit.y) < 3f) ExitBonus();
                }
                else if (pb.Right > bonusMinX) { pb.Pos.x = bonusMinX - pb.W * 0.5f; if (pb.Vel.x > 0f) pb.Vel.x = 0f; }
            }
            if (inBonus) return;

            // the finale: the Princesse, behind the Cat King's throne
            if (hasPrincess && !rescuedPrincess && pb.Pos.x >= princessPos.x - 1.4f && (boss == null || boss.Dead))
            {
                rescuedPrincess = true; rescuedMonk = true; showManicule = false;
                if (princessView != null) princessView.sprite = Art.Frames("princess/aelfgyva")[2];
                player.Freeze(); player.FaceRight();
                PlayerPrefs.SetInt(PassedKey(L.Title), 1); PlayerPrefs.Save();
                score += 10000;
                TymeBonus();
                Toast("Ye Princesse Aelfgyva is free!");
                winPhase = 3; winTime = 0f;
                SetState(State.Won);
                return;
            }

            // the rescued monk ends a castle
            if (hasMonk && !rescuedMonk && boss != null && !boss.Dead && pb.Pos.x >= monkPos.x - 1.2f)
            {
                if (!toldOfBoss) { toldOfBoss = true; Toast("Brother Odo crieth: first vanquish " + boss.Title + "!"); }
            }
            else if (hasMonk && !rescuedMonk && pb.Pos.x >= monkPos.x - 1.2f)
            {
                rescuedMonk = true; showManicule = false;
                if (monkView != null) monkView.sprite = Art.Frames("allies/monk")[1];
                player.Freeze(); player.FaceRight();
                PlayerPrefs.SetInt(PassedKey(L.Title), 1); PlayerPrefs.Save();
                score += 1000;
                TymeBonus();
                winPhase = 2; winTime = 0f;
                SetState(State.Won);
                return;
            }

            // a basket of flowers springs the knight high
            if (pb.Grounded && pb.Vel.y <= 0f && level.Basket(Mathf.FloorToInt(pb.Pos.x), Mathf.FloorToInt(pb.Pos.y - 0.1f)))
            { pb.Vel.y = TapestryInput.JumpHeld ? 21f : 17f; pb.Grounded = false; Audio.Play("spring"); }
            TickHornAndFountain(pb);
            if (player.Dead) return;
            if (hasCheckpoint && !checkpointClaimed && pb.Pos.x >= checkpointPos.x - 0.3f)
            {
                checkpointClaimed = true; purifiedSaved = level.Purified;
                if (checkpointView != null) checkpointView.sprite = Art.Frames("scenery/checkpoint")[1];
                Audio.Play("item_appear");
                Toast("Ye halfway banner is raysed! Here shalt thou ryse agayne.");
            }

            if (L.EndAtDoor && !rescuedMonk && pb.Pos.x >= L.CastleX - 0.3f && pb.Pos.y < 4f)
            {
                rescuedMonk = true;   // (reused as 'level finished')
                player.Freeze(); player.Hide();
                PlayerPrefs.SetInt(PassedKey(L.Title), 1); PlayerPrefs.Save();
                TymeBonus();
                winPhase = 2; winTime = 0f;
                SetState(State.Won);
                return;
            }

            if (L.Entrance && pb.Pos.x >= L.CastleX - 0.2f && pb.Grounded)
            {
                player.Freeze(); player.Hide();
                SetState(State.Entering);
                return;
            }

            if (level.GoalX > 0f && pb.Right >= level.GoalX - 0.1f && !player.Dead)
            {
                player.Freeze();
                player.FaceRight();
                TymeBonus();
                int lastFigure = Mathf.CeilToInt(Mathf.Max(0f, tymeLeft)) % 10;   // as Mario's fireworks: a last figure of 1, 3 or 6 sets the bells ringing
                bellsLeft = lastFigure == 1 ? 1 : lastFigure == 3 ? 3 : lastFigure == 6 ? 6 : 0; bellT = 1.2f;
                if (bellsLeft > 0) Toast("Ye bells ring for thy happy tyme!");
                PoleScore(pb.Pos.y - level.GroundAt(level.GoalX), new Vector2(level.GoalX, pb.Pos.y + pb.H));
                pb.Pos.x = level.GoalX - pb.W * 0.5f - 0.05f;
                winPhase = 0; winTime = 0f;
                PlayerPrefs.SetInt(PassedKey(L.Title), 1); PlayerPrefs.Save();   // unlocks it in the level select
                SetState(State.Won);
            }
        }

        void TickEntities(float dt)
        {
            Cleanup();
            float wake = (inBonus ? bonusReturn.x : camX) + HalfWidth + 2f;   // up in heaven nothing below wakes
            if (boss != null)
            {
                if (!boss.Active && boss.Body.Pos.x < wake - 2f)
                {
                    boss.Active = true; bossMusic = true; Audio.Music(LevelMusic);
                    Toast(boss.Title + " barreth thy way!");
                }
                if (boss.Active) boss.Tick(dt, level, player);
            }
            foreach (var e in enemies)
            {
                if (!e.Active && e.Body.Pos.x < wake) e.Active = true;
                if (e.Active) e.Tick(dt, level, player);
            }
            foreach (var s in shots) s.Tick(dt, level);
            foreach (var b in bezants) b.Tick(dt);
            foreach (var d in debris) d.Tick(dt);
            // log piles roll a log toward the knight every few seconds while he is near
            for (int i = 0; i < logPiles.Count; i++)
            {
                var lp = logPiles[i];
                if (Mathf.Abs(lp.x - player.Body.Pos.x) > HalfWidth + 3f) continue;
                logTimers[i] -= dt;
                if (logTimers[i] <= 0f)
                {
                    logTimers[i] = 3.2f;
                    float d = player.Body.Pos.x < lp.x ? -1f : 1f;
                    logs.Add(new Log(lp + new Vector2(d * 1.2f, 0.05f), d, world));
                    Audio.Play("bump", 0.5f);
                }
            }
            foreach (var lg in logs)
            {
                lg.Tick(dt, level);
                if (!lg.Removed && Mathf.Abs(lg.Body.Pos.x - camX) > HalfWidth + 1f) lg.Destroy();   // rolled off screen: gone, never to rebound
            }
            foreach (var it in items) it.Tick(dt, level);
            if (bayard != null)
            {
                bayard.Tick(dt, level);
                if (bayard.Removed)
                {
                    if (bayard.Lost) { Toast("Alas! " + bayard.Name + " is loste. Seeke " + (bayard.Kind == "unicorn" ? "it" : "him") + " agayne in ye bryckes."); Audio.Play("bayard_lost"); }
                    bayard = null;
                }
            }
            if (toastTime > 0f) toastTime -= dt;
            foreach (var p in popups) p.T += dt;
            popups.RemoveAll(p => p.T > 1.1f);
            Cleanup();
        }

        void Cleanup()
        {
            if (pendingEnemies.Count > 0) { enemies.AddRange(pendingEnemies); pendingEnemies.Clear(); }
            enemies.RemoveAll(e => e.Removed);
            shots.RemoveAll(s => s.Removed);
            bezants.RemoveAll(b => b.Removed);
            debris.RemoveAll(d => d.Removed);
            logs.RemoveAll(l => l.Removed);
            items.RemoveAll(i => i.Removed);
        }

        void LateUpdate()
        {
            if (player == null || level == null) return;
            if (state == State.Playing || state == State.Won)
            {
                float target = ClampCam(player.Body.Pos.x + (player.FacingRight ? 1.5f : -1.5f));
                if (BossHoldsArena)   // the screen stays on the fight until the boss is beaten
                {
                    // never scroll past the far side of the arena (the knight is held back there too)
                    target = Mathf.Min(target, Mathf.Max(boss.ArenaMax - HalfWidth, boss.ArenaMin + HalfWidth));
                }
                camX = Mathf.Lerp(camX, target, 1f - Mathf.Exp(-6f * Mathf.Min(Time.deltaTime, 0.05f)));
            }
            camX = ClampCam(camX);
            PlaceCamera();
        }

        void SwitchKnight(int d)
        {
            int n = Knights.All.Length;
            knightIndex = (knightIndex + d + n) % n;
            player.SetKnight(Knights.All[knightIndex]);
        }

        // ------------------------------------------------------------------ called by actors
        public void KnightHitBlock(int x, int y)
        {
            var r = level.HitFromBelow(x, y, player.Large);
            switch (r)
            {
                case Level.BumpResult.Broke: Audio.Play("brick"); break;
                case Level.BumpResult.Thud: Audio.Play("bump", 0.8f); break;
                case Level.BumpResult.Bezant: case Level.BumpResult.None: break;
                default: Audio.Play("item_appear"); break;
            }
            var top = new Vector2(x + 0.5f, y + 1f);
            switch (r)
            {
                case Level.BumpResult.Bezant:
                    bezants.Add(new Bezant(top, world, true)); AddBezant(); break;
                case Level.BumpResult.Bayard:
                    if (!player.Mounted && bayard == null) { bayard = new Bayard(top, world); bayard.Hop(); }
                    else { bezants.Add(new Bezant(top, world, true)); AddBezant(); }
                    break;
                case Level.BumpResult.Vine: Toast("A beanstalk! Presse UP to climbe it."); break;
                case Level.BumpResult.Mirror:
                    level.FlipMirrors(); Audio.Play("mirror");
                    UnstickFromPanes(player.Body);
                    break;
                case Level.BumpResult.Unicorn:
                    if (!player.OnUnicorn && (bayard == null || bayard.Kind != "unicorn"))
                    { if (bayard != null) bayard.Destroy(); bayard = new Bayard(top, world, "unicorn"); bayard.Hop(); }
                    else { bezants.Add(new Bezant(top, world, true)); AddBezant(); }
                    break;
                case Level.BumpResult.Chalice: items.Add(new Item("chalice", top, world, player.FacingRight)); break;
                case Level.BumpResult.Capon: items.Add(new Item("capon", top, world, player.FacingRight)); break;
                case Level.BumpResult.Crossbow:   // like the fire flower, which is only a mushroom to a small Mario
                    items.Add(new Item(player.Large ? "crossbow" : "capon", top, world, player.FacingRight)); break;
                case Level.BumpResult.Relic: items.Add(new Item("relic", top, world, player.FacingRight)); break;
                case Level.BumpResult.Broke:
                    score += 50;
                    var c = new Vector2(x + 0.5f, y + 0.3f);
                    debris.Add(new Debris(c, new Vector2(-2.5f, 9f), world));
                    debris.Add(new Debris(c, new Vector2(2.5f, 9f), world));
                    debris.Add(new Debris(c, new Vector2(-1.8f, 6f), world));
                    debris.Add(new Debris(c, new Vector2(1.8f, 6f), world));
                    break;
            }
            if (r != Level.BumpResult.None)
                foreach (var e in enemies)   // beasts standing on a bumped block are knocked flying
                    if (e.Alive && Mathf.Abs(e.Body.Pos.x - top.x) < 0.8f && Mathf.Abs(e.Body.Pos.y - top.y) < 0.2f) e.Flip();
        }

        public void ReleaseBayard(Vector2 pos, bool knightFacingRight, bool thrown, string kind = "bayard")
        {
            if (bayard != null) bayard.Destroy();
            bayard = new Bayard(pos, world, kind ?? "bayard");
            if (thrown) bayard.Bolt(!knightFacingRight); else bayard.Calm();
        }

        public void SpawnShot(string kind, Vector2 pos, Vector2 vel) { shots.Add(new Shot(kind, pos, vel, world)); }
        public void SpawnEnemy(string kind, Vector2 pos) { pendingEnemies.Add(new Enemy(kind, pos, world) { Active = true }); }
        public int CountEnemies(string kind) { int n = 0; foreach (var e in enemies) if (e.Kind == kind && e.Alive && !e.Removed) n++; return n + pendingEnemies.FindAll(e => e.Kind == kind).Count; }

        static readonly string[] HorsemanKinds = { "horseman_white", "horseman_red", "horseman_black", "horseman_pale" };
        static readonly string[] HorsemanNames = { "Ye Ryder on ye White Horse", "Ye Ryder on ye Red Horse", "Ye Ryder on ye Blacke Horse", "Death upon ye Pale Horse" };

        /// <summary>World 7: stars fall from heaven (often right above the knight); the four horsemen gallop in from behind in turn.</summary>
        void TickStarsAndHorsemen(float dt)
        {
            if (player.Dead) return;
            if (L.FallingStars)
            {
                starT -= dt;
                if (starT <= 0f)
                {
                    starT = 1.4f + Random.value * 1.2f;
                    float x = Random.value < 0.5f ? player.Body.Pos.x + Random.Range(-2.5f, 2.5f) : camX + Random.Range(-HalfWidth + 1.5f, HalfWidth - 1.5f);
                    SpawnShot("star", new Vector2(x, ViewHalfHeight * 2f + 0.5f), new Vector2(0f, -2.5f));
                    Audio.Play("star", 0.35f);
                }
            }
            if (L.Horsemen && player.Body.Pos.x < level.Width - 30f)
            {
                horsemanT -= dt;
                if (horsemanT <= 0f)
                {
                    float x = camX - HalfWidth - 1.5f;
                    if (x > 1f && level.GroundAt(x) > 0.5f)
                    {
                        horsemanT = 6.5f;
                        int i = horsemanNext++ % 4;
                        pendingEnemies.Add(new Enemy(HorsemanKinds[i], new Vector2(x, level.GroundAt(x)), world) { Active = true });
                        Audio.Play("trumpet", 0.7f);
                        Toast(HorsemanNames[i] + " cometh! Leape him!");
                    }
                    else horsemanT = 0.5f;
                }
            }
        }
        public void SpawnBolt(Vector2 pos, bool right) { shots.Add(new Shot("bolt", pos, new Vector2(right ? 13f : -13f, 0f), world)); }
        public int BoltCount { get { int n = 0; foreach (var s in shots) if (s.Friendly && !s.Removed) n++; return n; } }

        public void Toast(string text) { toast = text; toastTime = 3f; }

        void Collect(Item it)
        {
            score += 1000;
            switch (it.Kind)
            {
                case "chalice": Audio.Play("oneup"); lives++; Toast("A golden chalice! One more lyfe is thyne."); break;
                case "capon":
                    Audio.Play("powerup");
                    if (player.Grow(level)) Toast("A roaste capon! Thou growest great and strong.");
                    else { AddScore(2000, player.Body.Pos + new Vector2(0f, player.Body.H + 0.5f)); Toast("A roaste capon! Thou art well fed alreadie: a feast of glorie."); }
                    break;
                case "crossbow": Audio.Play("powerup"); player.GiveCrossbow(); Toast("A crossbowe! Presse RUNNE to loose a bolte."); break;
                case "relic": Audio.Play("powerup"); player.RelicTime = 9f; Toast("A holy relique! No beaste may harme thee for a whyle."); break;
            }
        }
        public void AddScore(int n) { score += n; }

        /// <summary>Every tick of tyme left over at the goal is worth 50 glorie.</summary>
        /// <summary>Roman numerals, as a scribe of 1066 would have written them (nulla for nought).</summary>
        public static string Roman(int n)
        {
            if (n <= 0) return "N";
            int[] v = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            string[] r = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < v.Length; i++)
                while (n >= v[i]) { sb.Append(r[i]); n -= v[i]; }
            return sb.ToString();
        }

        void TymeBonus()
        {
            tymeBonus = Mathf.CeilToInt(Mathf.Max(0f, tymeLeft)) * 50;
            score += tymeBonus;
            Audio.MusicPitch(1f);
        }

        bool BossHoldsArena { get { return boss != null && boss.Active && !boss.Dead; } }
        bool showManicule;

        public void BossDefeated()
        {
            if (level.OpenGates() > 0) Audio.Play("brick");   // the way on is opened
            showManicule = hasMonk;
            if (hasMonk) Toast("Ryde on to Brother Odo!");
            bossMusic = false;
            Audio.Music(L.Music);
            Toast(boss != null ? boss.DefeatedText : "Huzzah! Ye beaste is slayne!");
        }

        /// <summary>When the mirror panes change places, lift anything now standing inside a solid pane up on top of it.</summary>
        void UnstickFromPanes(Body b)
        {
            for (int guard = 0; guard < 4; guard++)
            {
                bool moved = false;
                int x0 = Mathf.FloorToInt(b.Left + 0.02f), x1 = Mathf.FloorToInt(b.Right - 0.02f);
                int y0 = Mathf.FloorToInt(b.Pos.y + 0.02f), y1 = Mathf.FloorToInt(b.Top - 0.02f);
                for (int x = x0; x <= x1 && !moved; x++)
                    for (int y = y0; y <= y1 && !moved; y++)
                        if (level.Solid(x, y)) { b.Pos.y = y + 1f; b.Vel.y = 0f; moved = true; }
                if (!moved) return;
            }
        }

        /// <summary>Ye Spotted Unicorne rammed the gate: knock out part of it.</summary>
        public void CrackGate(float strengthLeft)
        {
            foreach (var t in level.CrackGate(strengthLeft / 3f))
            {
                var c = new Vector2(t.x + 0.5f, t.y + 0.5f);
                debris.Add(new Debris(c, new Vector2(Random.Range(-3f, 3f), Random.Range(5f, 10f)), world));
            }
        }

        /// <summary>The calmed unicorn stays in the enclosure as a mount.</summary>
        public void SpareUnicorn(Vector2 pos, bool facingRight)
        {
            if (bayard != null) bayard.Destroy();
            bayard = new Bayard(pos, world, "unicorn");
            bayard.Calm();
            Toast(player.Mounted ? "Touche ye Unicorne to change thy steede!" : "Leape upon ye Unicorne, and ryde!");
        }

        /// <summary>World 5-2: carry ye unicorne's horne to ye fountaine; until then the stream is poisoned.</summary>
        void TickHornAndFountain(Body pb)
        {
            if (hasHorn && !carryingHorn && hornView != null && Mathf.Abs(pb.Pos.x - hornPos.x) < 0.8f && hornPos.y > pb.Pos.y - 0.5f && hornPos.y < pb.Top + 0.5f)
            {
                carryingHorn = true; Audio.Play("item_appear");
                Toast("Ye Unicorne's horne! Beare it to ye fountaine, and ye waters shall runne pure.");
            }
            if (carryingHorn && hornView != null)   // it floats above the knight's head as he carries it
                hornView.transform.localPosition = new Vector3(pb.Pos.x, pb.Top + 0.45f + Mathf.Sin(Time.time * 5f) * 0.1f, 0f);
            if (hasFountain && !level.Purified && Mathf.Abs(pb.Pos.x - fountainPos.x) < 2f)
            {
                if (carryingHorn)
                {
                    level.Purify(); carryingHorn = false; hasHorn = false;
                    if (hornView != null) { Destroy(hornView.gameObject); hornView = null; }
                    Audio.Play("purify");
                    AddScore(2000, fountainPos + new Vector2(0f, 3f));
                    Toast("Ye waters runne pure! Now thou mayst wade ye streame.");
                }
                else if (!toldFountain) { toldFountain = true; Toast("Ye fountaine is poysoned. Fynde ye unicorne's horne!"); }
            }
            if (level.Foul(Mathf.FloorToInt(pb.Pos.x), Mathf.FloorToInt(pb.Pos.y + 0.3f)))
            {
                if (!toldFoul) { toldFoul = true; Toast("Ye streame is poysoned! Leape over it."); }
                player.Hurt();
                if (!player.Dead) pb.Vel.y = 11f;   // it stings him up out of the water
            }
        }

        /// <summary>World 5-3: the hunt sweeps on from behind. It keeps pace; if it reaches the knight he is wounded and it falls back a little.</summary>
        void TickChase(float dt)
        {
            if (!L.Chase || huntView == null || player.Dead) return;
            var pb = player.Body;
            if (huntDelay > 0f)
            {
                huntDelay -= dt;
                if (huntDelay <= 0f) { Audio.Play("horn"); Toast("Ye hunte is upon thee! Runne!"); }
                return;
            }
            if (level.GoalX > 0f && huntX > level.GoalX - 10f) return;   // it halts at the edge of the park
            float lead = pb.Pos.x - huntX;
            huntX += (lead > 16f ? 8f : 4.8f) * dt;   // never far behind
            huntView.transform.localPosition = new Vector3(huntX, 2f, 0f);
            if (pb.Left < huntX + 2.0f && !player.Invulnerable)
            {
                player.Hurt();
                huntX -= 4f;
            }
        }

        void TickCollapse(float dt)
        {
            collapseT += dt;
            while (collapseT > 0.07f && collapseTiles.Count > 0)
            {
                collapseT -= 0.07f;
                var t = collapseTiles[0]; collapseTiles.RemoveAt(0);
                level.RemoveTile(t.x, t.y);
                if (collapseTiles.Count % 3 == 0) Audio.Play("brick", 0.6f);
            }
            if (collapseTiles.Count == 0)
            {
                collapseDoneT += dt;
                if (collapseDoneT > 1.2f) { collapsing = false; hasAxe = false; player.Unfreeze(); }
            }
        }

        /// <summary>Move a body standing on a lift along with it (call before the body steps).</summary>
        public void CarryOnLift(Body b)
        {
            if (riding != null) b.Pos += riding.Delta;
        }

        /// <summary>Land a falling body on any lift top it crosses (call after the body steps).</summary>
        public void LandOnLifts(Body b)
        {
            riding = null;
            if (b.Vel.y > 0f) return;
            foreach (var l in lifts)
            {
                if (b.Right <= l.X0 + 0.05f || b.Left >= l.X1 - 0.05f) continue;
                if (b.PrevBottom >= l.Top - 0.25f && b.Pos.y <= l.Top + 0.02f)
                {
                    b.Pos.y = l.Top; b.Vel.y = 0f; b.Grounded = true; riding = l; if (b == player.Body) l.StoodFrame = true;
                    return;
                }
            }
        }
        public void AddScore(int n, Vector2 at) { score += n; popups.Add(new Popup { Pos = at, Text = n.ToString() }); }

        /// <summary>The higher on the pole ye grasp it, the more glorie (Mario-fashion). The very top gives a lyfe.</summary>
        void PoleScore(float height, Vector2 at)
        {
            if (height >= 7f)
            {
                lives++; score += 5000;
                popups.Add(new Popup { Pos = at, Text = "1 LYFE!" });
                Audio.Play("oneup");
                Toast("Ye toppe of ye pole! A lyfe is thyne.");
                return;
            }
            int pts = height >= 5.5f ? 5000 : height >= 4f ? 2000 : height >= 2.5f ? 800 : height >= 1.2f ? 400 : height >= 0.4f ? 200 : 100;
            AddScore(pts, at);
        }
        public void PlayerDied(bool fell) { deathFell = fell; SetState(State.Dying); }

        void AddBezant()
        {
            Audio.Play("bezant", 0.7f);
            bezantCount++; score += 10;
            if (bezantCount >= 100) { bezantCount -= 100; lives++; }
        }

        // ------------------------------------------------------------------ GUI
        GUIStyle big, small, center, popupStyle, tipName, tipText, menuStyle, menuCenter, linkStyle;

        /// <summary>GUI unit: screen height, or less when the window is narrower than 16:9.</summary>
        static float U { get { return Mathf.Min(Screen.height, Screen.width * 9f / 16f); } }

        void Styles()
        {
            if (big != null && big.fontSize == Mathf.RoundToInt(U / 16f)) return;
            big = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(U / 16f), fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            big.normal.textColor = Thread;
            small = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(U / 30f) };
            small.normal.textColor = Thread;
            center = new GUIStyle(small) { alignment = TextAnchor.MiddleCenter, wordWrap = true };
            tipName = new GUIStyle(small) { fontStyle = FontStyle.Bold, alignment = TextAnchor.LowerLeft, fontSize = Mathf.RoundToInt(U / 28f) };
            tipText = new GUIStyle(small) { alignment = TextAnchor.UpperLeft, wordWrap = true, fontSize = Mathf.RoundToInt(U / 34f) };
            popupStyle = new GUIStyle(small) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            popupStyle.normal.textColor = new Color(0.66f, 0.26f, 0.18f);
            menuStyle = new GUIStyle(small) { fontSize = Mathf.RoundToInt(U / 24f), fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            menuCenter = new GUIStyle(menuStyle) { alignment = TextAnchor.MiddleCenter };
            linkStyle = new GUIStyle(small) { alignment = TextAnchor.MiddleCenter, wordWrap = true, fontSize = Mathf.RoundToInt(U / 38f) };
            linkStyle.normal.textColor = new Color(0.95f, 0.9f, 0.76f);
        }

        void DrawFrame(string key, int frame, Rect r)
        {
            var tex = Art.Texture(key);
            if (tex == null) return;
            int n = Mathf.Max(1, Art.Sheet(key).Frames);
            GUI.DrawTextureWithTexCoords(r, tex, new Rect(frame / (float)n, 0f, 1f / n, 1f));
        }

        float TitleGroundTop { get { return Screen.height - 2f * Screen.height * 0.085f; } }

        /// <summary>The four knights stand in a row on the ground at the left of the title scene.</summary>
        Rect PortraitRect(int i)
        {
            float kh = Screen.height * 0.18f, kw = kh * 64f / 96f;
            return new Rect(Screen.width * 0.03f + i * kw * 1.25f, TitleGroundTop - kh + Screen.height * 0.004f, kw, kh);
        }

        Rect MenuRect(int i) { return new Rect(Screen.width * 0.42f, Screen.height * (0.545f + i * 0.068f), Screen.width * 0.4f, Screen.height * 0.062f); }

        void Panel(Rect r) { var c = GUI.color; GUI.color = new Color(Linen.r, Linen.g, Linen.b, 0.88f); GUI.DrawTexture(r, Texture2D.whiteTexture); GUI.color = c; }

        void OnGUI()
        {
            Styles();
            float W = Screen.width, H = Screen.height;
            switch (state)
            {
                case State.Briefing: DrawBriefing(W, H); break;
                case State.Ending: DrawEnding(W, H); break;
                case State.Paywall: DrawPaywall(W, H); break;
                case State.About: DrawAbout(W, H); break;
                case State.Tapestries: DrawTapestries(W, H); break;
                case State.Title:
                    if (titleStep == 1) { Panel(new Rect(0, H * 0.12f, W, H * 0.88f)); DrawRealmSelect(W, H); break; }
                    DrawTitle(W, H);
                    break;

                case State.Playing:
                case State.Paused:
                case State.Dying:
                case State.Won:
                case State.GameOver:
                case State.Entering:
                    Hud(W, H);
                    if (state == State.Entering)
                    {
                        var fc = GUI.color; GUI.color = new Color(0.12f, 0.09f, 0.07f, Mathf.Clamp01(stateTime / 0.9f));
                        GUI.DrawTexture(new Rect(0, 0, W, H), Texture2D.whiteTexture); GUI.color = fc;
                    }
                    if (state == State.Paused)
                    {
                        Panel(new Rect(W * 0.2f, H * 0.26f, W * 0.6f, H * 0.5f));
                        GUI.Label(new Rect(0, H * 0.28f, W, H * 0.1f), "THOU ART AT REST", big);
                        if (TapestryInput.ShowTouchControls)
                        {
                            string[] labels = { "Continewe", "Returne to ye beginnynge", Audio.Muted ? "Unsilence ye musick" : "Silence ye musick" };
                            for (int i = 0; i < 3; i++)
                            {
                                var r = PauseButtonRect(i);
                                var bc = GUI.color; GUI.color = new Color(0.66f, 0.26f, 0.18f, 0.3f); GUI.DrawTexture(r, Texture2D.whiteTexture); GUI.color = bc;
                                GUI.Label(r, labels[i], center);
                            }
                        }
                        else GUI.Label(new Rect(0, H * 0.42f, W, H * 0.2f), "JUMPE or Starte to continewe\nDOWNE to returne unto ye beginnynge\nM (or Selecte) to silence ye musick", center);
                    }
                    if (state == State.Won && winPhase == 2)
                    {
                        if (tymeBonus > 0) GUI.Label(new Rect(0, H * 0.73f, W, H * 0.06f), "Tyme bonus: " + tymeBonus + " glorie", center);
                        Panel(new Rect(W * 0.15f, H * 0.24f, W * 0.7f, H * 0.57f));
                        GUI.Label(new Rect(0, H * 0.27f, W, H * 0.1f), "HERE YE KNYGHTE IS VICTORIOUS", big);
                        GUI.Label(new Rect(W * 0.15f, H * 0.37f, W * 0.7f, H * 0.17f), hasMonk
                            ? "Grammercy, good knyghte! " + (boss != null ? boss.Title : "Ye beaste") + " is vanquished.\nYet ye Princesse Aelfgyva is held in another castel..." + (levelIndex + 1 < Levels.All.Length ? "\nPresse JUMPE to ryde on." : "\nMore of ye tapestrie is yet to be sewn...")
                            : levelIndex + 1 < Levels.All.Length
                            ? "Huzzah! Ye beastes are put to flyghte.\nYet ye Princesse Aelfgyva lyeth in another castel...\nPresse JUMPE to ryde on."
                            : "Huzzah! Thou hast passed every realme yet stitched.\nMore of ye tapestrie is yet to be sewn...", center);
                        if (hasMonk) DrawFrame("allies/monk", 1, new Rect(W * 0.5f - H * 0.05f, H * 0.56f, H * 0.1f, H * 0.17f)); else
                        DrawFrame("princess/aelfgyva", 1, new Rect(W * 0.5f - H * 0.05f, H * 0.54f, H * 0.1f, H * 0.19f));
                    }
                    if (state == State.Dying)
                    {
                        Panel(new Rect(W * 0.25f, H * 0.32f, W * 0.5f, H * 0.22f));
                        GUI.Label(new Rect(0, H * 0.34f, W, H * 0.1f), deathTyme ? "YE TYME IS UP!" : deathFire ? "ALAS, HE IS BURNT!" : deathFell ? "ALAS, HE IS FALLEN!" : "ALAS, HE IS SLAYNE!", big);
                        GUI.Label(new Rect(0, H * 0.44f, W, H * 0.08f), (deathTyme ? "Ye knyghte tarried too long. " : deathFire ? "Ye knyghte hath fallen into ye fyre. " : deathFell ? "Ye knyghte hath fallen into ye pitte. " : "Ye knyghte is struck downe. ") + (lives > 1 ? (lives - 1) + " lyves yet remayne." : "No lyves remayne..."), center);
                    }
                    if (state == State.GameOver)
                    {
                        Panel(new Rect(W * 0.25f, H * 0.3f, W * 0.5f, H * 0.3f));
                        GUI.Label(new Rect(0, H * 0.34f, W, H * 0.12f), "HERE ENDETH YE TALE", big);
                        GUI.Label(new Rect(0, H * 0.46f, W, H * 0.06f), "Ye beastes have wonne this daye. Presse JUMPE.", center);
                        GUI.Label(new Rect(0, H * 0.52f, W, H * 0.06f), (newBest ? "A NEW BEST GLORIE:  " : "Thy glorie:  ") + score.ToString("000000") + "     Best:  " + BestScore.ToString("000000"), center);
                    }
                    if (state == State.Playing && TapestryInput.ShowTouchControls) TouchButtons();
                    break;
            }
        }

        /// <summary>A long strip of tapestry: the embroidered part is the knight's health, stitched in segments.</summary>
        void HealthBar(Rect r)
        {
            var empty = Art.Texture("scenery/health_empty");
            var full = Art.Texture("scenery/health_full");
            if (empty == null || full == null) return;
            const int max = 2;   // two stitched halves: small = one filled, large = both
            float frac = player.Large ? 1f : 0.5f;
            GUI.DrawTexture(r, empty);
            // the fringe takes the first and last 8/240 of the strip; fill between them
            const float edge = 8f / 240f;
            float k = edge + (1f - 2f * edge) * frac;
            if (frac > 0f) GUI.DrawTextureWithTexCoords(new Rect(r.x, r.y, r.width * k, r.height), full, new Rect(0f, 0f, k, 1f));
            var c = GUI.color; GUI.color = Thread;
            for (int i = 1; i < max; i++)
            {
                float x = r.x + r.width * (edge + (1f - 2f * edge) * i / max);
                GUI.DrawTexture(new Rect(x - 1f, r.y + r.height * 0.12f, 2f, r.height * 0.76f), Texture2D.whiteTexture);
            }
            GUI.color = c;
        }

        Rect PauseButtonRect(int i)
        {
            float w = Screen.width * 0.44f, h = Screen.height * 0.085f;
            return new Rect((Screen.width - w) * 0.5f, Screen.height * 0.4f + i * h * 1.2f, w, h);
        }

        void Gusts()
        {
            if (wind == 0f || state != State.Playing) return;
            var tex = Art.Texture("scenery/gust");
            if (tex == null) return;
            float w = U * 0.4f, h = w * tex.height / tex.width;
            for (int i = 0; i < 4; i++)
            {
                float speed = Screen.width * 0.9f;
                float x = Mathf.Repeat(Time.time * speed * Mathf.Sign(wind) + i * Screen.width * 0.3f, Screen.width + w) - w;
                float y = Screen.height * (0.28f + 0.14f * i);
                var r = wind < 0f ? new Rect(x + w, y, -w, h) : new Rect(x, y, w, h);   // mirrored when blowing from the right
                var c = GUI.color; GUI.color = new Color(1f, 1f, 1f, 0.8f); GUI.DrawTexture(r, tex); GUI.color = c;
            }
        }

        void Popups()
        {
            foreach (var p in popups)
            {
                var sp = cam.WorldToScreenPoint(new Vector3(p.Pos.x, p.Pos.y + p.T * 1.4f, 0f));
                var c = GUI.color; GUI.color = new Color(1f, 1f, 1f, Mathf.Clamp01(1.6f - p.T * 1.4f));
                GUI.Label(new Rect(sp.x - U * 0.15f, Screen.height - sp.y - U * 0.03f, U * 0.3f, U * 0.06f), p.Text, popupStyle);
                GUI.color = c;
            }
        }

        /// <summary>The title scene, laid out as an old arcade title: a row of figures along the top, the banner, the menu with its
        /// cursor, the best score, and the knights standing on the ground before a hill and a shrub.</summary>
        void DrawTitle(float W, float H)
        {
            Panel(new Rect(0, 0, W, H));
            float ts = H * 0.085f, groundTop = TitleGroundTop;
            var def = Knights.All[knightIndex];

            // the scene: stitched trees and a shrub, then two rows of earth
            var tree = Art.Texture("scenery/tree");
            if (tree != null)
            {
                float th = H * 0.46f, tw = th * tree.width / tree.height;
                GUI.DrawTexture(new Rect(W * 0.005f, groundTop - th, tw, th), tree);
                float th2 = H * 0.36f, tw2 = th2 * tree.width / tree.height;
                GUI.DrawTexture(new Rect(W * 0.9f, groundTop - th2, tw2, th2), tree);
            }
            var shrub = Art.Texture("scenery/shrub");
            if (shrub != null) { float sh = H * 0.1f, sw = sh * shrub.width / shrub.height; GUI.DrawTexture(new Rect(W * 0.8f, groundTop - sh, sw, sh), shrub); GUI.DrawTexture(new Rect(W * 0.68f, groundTop - sh * 0.7f, sw * 0.7f, sh * 0.7f), shrub); }
            var gt = Art.Texture("scenery/ground_top"); var gf = Art.Texture("scenery/ground_fill");
            for (float x = 0f; x < W; x += ts)
            {
                if (gt != null) GUI.DrawTexture(new Rect(x, groundTop, ts + 1f, ts), gt);
                if (gf != null) GUI.DrawTexture(new Rect(x, groundTop + ts, ts + 1f, ts + 1f), gf);
            }

            // the row of figures along the top
            float hy = H * 0.02f, hl = U * 0.045f;
            string knightName = def.Title.Substring(def.Title.LastIndexOf(' ') + 1).ToUpper();
            GUI.Label(new Rect(W * 0.07f, hy, W * 0.22f, hl), "KNYGHT", small);
            GUI.Label(new Rect(W * 0.07f, hy + hl, W * 0.22f, hl), knightName, small);
            DrawFrame("scenery/bezant", 0, new Rect(W * 0.33f, hy + hl * 0.9f, hl, hl));
            GUI.Label(new Rect(W * 0.33f + hl * 1.1f, hy + hl, W * 0.15f, hl), "\u00D7" + "00", small);
            GUI.Label(new Rect(W * 0.58f, hy, W * 0.2f, hl), "WORLDE", small);
            GUI.Label(new Rect(W * 0.58f, hy + hl, W * 0.2f, hl), "1-1", small);
            GUI.Label(new Rect(W * 0.8f, hy, W * 0.2f, hl), "TYME", small);

            // the banner
            var lg = Art.Texture("scenery/logo");
            if (lg != null) { float lh = H * 0.36f, lw = lh * lg.width / lg.height; GUI.DrawTexture(new Rect((W - lw) * 0.5f, H * 0.105f, lw, lh), lg); }
            GUI.Label(new Rect(W * 0.1f, H * 0.468f, W * 0.8f, U * 0.05f), def.Trait, center);

            // the menu, with a capon for its cursor
            int items = Beaten ? 2 : 1;
            for (int m = 0; m < items; m++)
            {
                var r = MenuRect(m);
                GUI.Label(new Rect(r.x + r.height * 1.4f, r.y, r.width, r.height), m == 0 ? "1 KNYGHT ADVENTURE" : "YE WRONGE SYDE", menuStyle);
                if (m == menuIndex) { float bob = Mathf.Sin(Time.time * 6f) * r.height * 0.05f; DrawFrame("items/capon", 0, new Rect(r.x, r.y + bob, r.height * 1.1f, r.height * 1.1f)); }
            }
            var br = MenuRect(items - 1);
            GUI.Label(new Rect(br.x + br.height * 1.4f, H * (0.545f + items * 0.068f + 0.005f), br.width, br.height), "BEST GLORIE  -  " + BestScore.ToString("000000"), menuStyle);

            // the knights, standing before the hill; the chosen one walks on the spot
            for (int i = 0; i < Knights.All.Length; i++)
            {
                var r = PortraitRect(i);
                bool sel = i == knightIndex;
                if (sel) { var c = GUI.color; GUI.color = new Color(0.66f, 0.26f, 0.18f, 0.16f); GUI.DrawTexture(new Rect(r.x - r.width * 0.2f, r.y - r.height * 0.06f, r.width * 1.4f, r.height * 1.06f), Texture2D.whiteTexture); GUI.color = c; }
                else GUI.color = new Color(1f, 1f, 1f, 0.6f);
                DrawFrame(Knights.All[i].Sheet, sel ? 1 + (int)(Time.time * 4f) % 2 : 0, r);
                GUI.color = Color.white;
            }

            // instructions and links, in cream upon the earth
            GUI.Label(new Rect(0, groundTop + H * 0.012f, W, H * 0.05f),
                TapestryInput.ShowTouchControls ? "Touche a knyghte to chuse, and touche agayne to ryde forth" : "LEFTE / RYGHTE chuse thy knyghte  -  UPPE / DOWNE chuse  -  JUMPE or ENTER ryde forth", linkStyle);
            if (!Store.Owned) GUI.Label(UnlockLinkRect(), "Unlocke Worldes 2 to 7  (touche here)", linkStyle);
            GUI.Label(AboutLinkRect(), "What is this?", linkStyle);
            GUI.Label(TapestriesLinkRect(), "Ye Tapestries", linkStyle);
            GUI.Label(new Rect(0, H * 0.96f, W, H * 0.04f), "Anno Domini MMXXVI", linkStyle);
        }

        void DrawRealmSelect(float W, float H)
        {
            var t = Art.Texture("scenery/title");
            float y = H * 0.14f;
            if (t != null) { float tw = Mathf.Min(W * 0.9f, U * 1.3f), th = tw * t.height / t.width; GUI.DrawTexture(new Rect((W - tw) * 0.5f, y, tw, th), t); }
            GUI.Label(new Rect(0, H * 0.3f, W, U * 0.08f), "Chuse thy realme", big);
            int world = RealmWorld(realmChoice);
            GUI.Label(new Rect(0, H * 0.37f, W, U * 0.06f), "Worlde " + world + (world < WorldNames.Length ? " - " + WorldNames[world] : ""), center);
            var page = RealmPage(realmChoice);
            for (int k = 0; k < page.Count; k++)
            {
                int i = page[k];
                var r = RealmRect(k, page.Count);
                var def = Levels.All[realmStarts[i]];
                var c = GUI.color;
                GUI.color = i == realmChoice ? new Color(0.66f, 0.26f, 0.18f, 0.4f) : new Color(0.2f, 0.145f, 0.114f, 0.12f);
                GUI.DrawTexture(r, Texture2D.whiteTexture);
                GUI.color = c;
                if (Locked(realmStarts[i])) GUI.color = new Color(1f, 1f, 1f, 0.45f);   // not yet bought
                string num = def.Title.Substring(def.Title.IndexOf(' ') + 1);   // "Worlde 3-2" -> "3-2"
                GUI.Label(new Rect(r.x, r.y + r.height * 0.06f, r.width, r.height * 0.4f), num, big);
                GUI.Label(new Rect(r.x + r.width * 0.05f, r.y + r.height * 0.48f, r.width * 0.9f, r.height * 0.5f), def.Name, center);
                GUI.color = Color.white;
            }
            bool more = RealmWorld(realmStarts.Count - 1) > world, less = RealmWorld(0) < world;
            if (less) GUI.Label(PageArrowRect(-1), "<", big);
            if (more) GUI.Label(PageArrowRect(1), ">", big);
            GUI.Label(new Rect(0, H * 0.62f, W, U * 0.08f), Knights.All[knightIndex].Title + " shall ryde", center);
            GUI.Label(new Rect(W * 0.05f, H * 0.78f, W * 0.9f, U * 0.1f),
                TapestryInput.ShowTouchControls ? "Touche a realme, and touche agayne to ryde" : "LEFTE or RYGHTE to chuse  -  JUMPE to ryde  -  RUNNE to chuse another knyghte", center);
        }

        /// <summary>The briefing: a parchment with the realme's name, its beasts and hazards (picture, name, counsel), and how to vanquish its boss.</summary>
        void DrawBriefing(float W, float H)
        {
            Panel(new Rect(0, 0, W, H));
            var safe = TapestryInput.Safe;
            float x0 = Mathf.Max(safe.x, W * 0.04f), x1 = Mathf.Min(safe.xMax, W * 0.96f);
            GUI.Label(new Rect(0, H * 0.04f, W, U * 0.08f), L.Title + "  ·  " + L.Name, big);
            GUI.Label(new Rect(0, H * 0.04f + U * 0.08f, W, U * 0.05f), "Heede well, sir knyghte:", center);
            int n = Mathf.Min(briefing.Count, 16);
            int rows = Mathf.Max(1, (n + 1) / 2);
            float top = H * 0.06f + U * 0.14f;
            float bottom = H * (briefBoss != null ? 0.7f : 0.86f);
            float rowH = Mathf.Min(U * 0.12f, (bottom - top) / rows);
            float colW = (x1 - x0) * 0.5f;
            for (int i = 0; i < n; i++)
            {
                var e = briefing[i];
                float x = x0 + (i % 2) * colW, y = top + (i / 2) * rowH;
                DrawIcon(e.Sprite, new Rect(x, y + rowH * 0.05f, rowH * 1.25f, rowH * 0.9f));
                GUI.Label(new Rect(x + rowH * 1.35f, y, colW - rowH * 1.45f, rowH * 0.4f), e.Name, tipName);
                GUI.Label(new Rect(x + rowH * 1.35f, y + rowH * 0.36f, colW - rowH * 1.45f, rowH * 0.64f), e.Tip, tipText);
            }
            if (briefBoss != null)
            {
                float y = H * 0.71f, h = H * 0.17f;
                var c = GUI.color; GUI.color = new Color(0.66f, 0.26f, 0.18f, 0.18f);
                GUI.DrawTexture(new Rect(x0, y, x1 - x0, h), Texture2D.whiteTexture); GUI.color = c;
                DrawIcon(briefBoss.Sprite, new Rect(x0 + 8f, y + 6f, h * 1.3f, h - 12f));
                GUI.Label(new Rect(x0 + h * 1.45f, y + h * 0.05f, x1 - x0 - h * 1.55f, h * 0.35f), "To vanquishe " + briefBoss.Name + ":", tipName);
                GUI.Label(new Rect(x0 + h * 1.45f, y + h * 0.38f, x1 - x0 - h * 1.55f, h * 0.6f), briefBoss.Tip, tipText);
            }
            float a = 0.55f + 0.45f * Mathf.Abs(Mathf.Sin(Time.time * 2.5f));
            var cc = GUI.color; GUI.color = new Color(1f, 1f, 1f, a);
            GUI.Label(new Rect(0, H * 0.9f, W, U * 0.06f), TapestryInput.ShowTouchControls ? "Touche to ryde forth" : "JUMPE to ryde forth", center);
            GUI.color = cc;
        }

        // ------------------------------------------------------------------ ye ende
        static readonly string[] EndingLines =
        {
            "YE ENDE", "",
            "Thus did ye knyghtes ryde through seven tapestries,",
            "from ye Bayeux strip to ye Apocalypse of Angers.", "",
            "They humbled ye Cockatrice upon her bridge,",
            "and Aspidochelone, ye Island-Whale, sank beneath ye sea.",
            "Ye Foure Windes were stilled in ye Wheel of ye Yeare,",
            "and ye Greate Boar was brought to bay in ye Hunting Lodge.", "",
            "Ye Spotted Unicorne was spared, and bore them on;",
            "ye Lyon and ye Unicorne bowed before ye Ladye's tente;",
            "and ye Beaste of Seven Heddes lost every one of them.", "",
            "Last of alle, ye Cat King was cast downe from his throne",
            "(and sent to bedde without any soupe).", "",
            "And Princesse Aelfgyva was brought home with great joye,",
            "and there was feasting for seven dayes,",
            "and soupe for alle.", "", "",
            "Here endeth ye tapestrie.", "", "",
            "Made with needle, loom and code.",
        };
        const float EndingLength = 46f;

        void DrawEnding(float W, float H)
        {
            Panel(new Rect(0, 0, W, H));
            float lineH = U * 0.075f, speed = U * 0.06f;
            float y0 = H + U * 0.05f - stateTime * speed;
            // the knight and the Princesse, together at the top of the parchment
            float pw = U * 0.16f;
            var kd = Knights.All[knightIndex];
            DrawIcon(kd.Sheet, new Rect(W * 0.5f - pw * 1.1f, H * 0.03f, pw, pw * 1.5f));
            DrawFrameFit("princess/aelfgyva", 2, new Rect(W * 0.5f + pw * 0.1f, H * 0.03f, pw, pw * 1.5f));
            for (int i = 0; i < EndingLines.Length; i++)
            {
                float y = y0 + i * lineH;
                if (y < H * 0.03f + pw * 1.55f || y > H) continue;
                GUI.Label(new Rect(0, y, W, lineH), EndingLines[i], i == 0 ? big : center);
            }
            if (stateTime > 5f)
            {
                var c = GUI.color; GUI.color = new Color(1f, 1f, 1f, 0.5f + 0.4f * Mathf.Sin(Time.time * 2.5f));
                GUI.Label(new Rect(0, H * 0.93f, W, U * 0.05f), TapestryInput.ShowTouchControls ? "Touche to returne" : "JUMPE to returne", small);
                GUI.color = c;
            }
        }

        void DrawFrameFit(string key, int frame, Rect r)
        {
            var tex = Art.Texture(key);
            if (tex == null) return;
            int n = Mathf.Max(1, Art.Sheet(key).Frames);
            float fw = tex.width / (float)n, fh = tex.height, k = Mathf.Min(r.width / fw, r.height / fh);
            var d = new Rect(r.x + (r.width - fw * k) * 0.5f, r.y + (r.height - fh * k) * 0.5f, fw * k, fh * k);
            GUI.DrawTextureWithTexCoords(d, tex, new Rect(frame / (float)n, 0f, 1f / n, 1f));
        }

        /// <summary>A sprite's first frame, fitted into r without stretching.</summary>
        void DrawIcon(string key, Rect r)
        {
            var tex = Art.Texture(key);
            if (tex == null) return;
            int n = Mathf.Max(1, Art.Sheet(key).Frames);
            float fw = tex.width / (float)n, fh = tex.height, k = Mathf.Min(r.width / fw, r.height / fh);
            var d = new Rect(r.x + (r.width - fw * k) * 0.5f, r.y + (r.height - fh * k) * 0.5f, fw * k, fh * k);
            GUI.DrawTextureWithTexCoords(d, tex, new Rect(0f, 0f, 1f / n, 1f));
        }

        void Manicule()
        {
            if (!showManicule || state != State.Playing) return;
            var tex = Art.Texture("scenery/manicule");
            if (tex == null) return;
            var safe = TapestryInput.Safe;
            float w = U * 0.16f, h = w * tex.height / tex.width;
            float bob = Mathf.Sin(Time.time * 6f) * w * 0.12f;
            var c = GUI.color; GUI.color = new Color(1f, 1f, 1f, 0.6f + 0.4f * Mathf.Abs(Mathf.Sin(Time.time * 3f)));
            GUI.DrawTexture(new Rect(safe.xMax - w * 1.25f + bob, Screen.height * 0.42f, w, h), tex);
            GUI.color = c;
        }

        void Hud(float W, float H)
        {
            Gusts();
            Manicule();
            Popups();
            if (boss != null && boss.Active && !boss.Dead)
            {
                var r = new Rect(W * 0.5f - U * 0.3f, H * 0.2f, U * 0.6f, U * 0.05f);
                GUI.Label(new Rect(r.x, r.y - U * 0.055f, r.width, U * 0.05f), boss.Title, center);
                var e = Art.Texture("scenery/health_empty"); var f = Art.Texture("scenery/health_full");
                if (e != null && f != null)
                {
                    GUI.DrawTexture(r, e);
                    float k = Mathf.Clamp01(boss.Health / boss.MaxHealth);
                    const float edge = 8f / 240f;
                    float u = edge + (1f - 2f * edge) * k;
                    if (k > 0f) GUI.DrawTextureWithTexCoords(new Rect(r.x, r.y, r.width * u, r.height), f, new Rect(0f, 0f, u, 1f));
                }
            }
            float y = H * 0.125f, s = U * 0.05f;
            var safe = TapestryInput.Safe;   // keep the HUD clear of camera notches and rounded corners
            GUI.BeginGroup(new Rect(safe.x, 0, safe.width, H));
            W = safe.width;
            GUI.Label(new Rect(W * 0.02f, y, W * 0.3f, s * 1.2f), player.Def.Title + (player.Mounted ? " upon " + player.MountName : ""), small);
            HealthBar(new Rect(W * 0.02f, y + s * 1.25f, s * 6f, s * 0.75f));
            DrawFrame("scenery/bezant", 0, new Rect(W * 0.34f, y + s * 0.1f, s, s));
            GUI.Label(new Rect(W * 0.34f + s * 1.2f, y, W * 0.12f, s * 1.2f), "× " + bezantCount, small);
            GUI.Label(new Rect(W * 0.47f, y, W * 0.15f, s * 1.2f), "Lyves × " + lives, small);
            GUI.Label(new Rect(W * 0.62f, y, W * 0.2f, s * 1.2f), "Glorie " + score.ToString("000000"), small);
            var tc = small.normal.textColor;
            if (hurried) small.normal.textColor = new Color(0.66f, 0.26f, 0.18f);   // red when tyme is short
            GUI.Label(new Rect(W * 0.80f, y, W * 0.2f, s * 1.2f), "Tyme " + Roman(Mathf.CeilToInt(Mathf.Max(0f, tymeLeft))), small);
            small.normal.textColor = tc;
            if (hardMode) GUI.Label(new Rect(W * 0.80f, y + s * 1.2f, W * 0.2f, s * 1.2f), "Wronge Syde", small);
            float ix = W * 0.02f + s * 6.4f;
            if (player.HasCrossbow) { DrawFrame("items/crossbow", 0, new Rect(ix, y + s * 1.15f, s * 1.3f, s)); ix += s * 1.5f; }
            if (player.Blessed) DrawFrame("items/relic", 1, new Rect(ix, y + s * 1.1f, s * 1.1f, s * 1.1f));
            if (toast != null && toastTime > 0f)
            {
                var c = GUI.color; GUI.color = new Color(1f, 1f, 1f, Mathf.Clamp01(toastTime));
                Panel(new Rect(W * 0.2f, y + s * 2.6f, W * 0.6f, s * 1.4f));
                GUI.Label(new Rect(W * 0.2f, y + s * 2.6f, W * 0.6f, s * 1.4f), toast, center);
                GUI.color = c;
            }
            GUI.EndGroup();
        }

        void TouchButtons()
        {
            // the fixed joystick: base, then the knob where the thumb has pushed it
            // all the controls are see-through (about a third opaque), a little firmer while touched, so they do not hide the game
            bool joyHeld = TapestryInput.JoyKnob.sqrMagnitude > 1f;
            DrawDisc("scenery/ui_joy_base", TapestryInput.JoyCentre, TapestryInput.JoyRadius, 82f / 90f, 1f, joyHeld ? 0.55f : 0.38f);
            DrawDisc("scenery/ui_joy_knob", TapestryInput.JoyCentre + TapestryInput.JoyKnob, TapestryInput.JoyRadius * 0.42f, 36f / 40f, 1f, joyHeld ? 0.72f : 0.5f);
            // JUMPE shows the knight being played; RUNNE/SHOOTE shows the crossbowe
            DrawDisc("scenery/ui_jump_" + player.Def.Key, TapestryInput.JumpCentre, TapestryInput.JumpRadius, 64f / 70f, 1f, TapestryInput.TouchJump ? 0.65f : 0.42f);
            DrawDisc("scenery/ui_run", TapestryInput.RunCentre, TapestryInput.RunRadius, 50f / 56f, 1f, TapestryInput.TouchRun ? 0.65f : 0.42f);
            DrawDisc("scenery/ui_pause", TapestryInput.PauseCentre, TapestryInput.PauseRadius, 23f / 26f, 1f, 0.34f);
        }

        /// <summary>Draw a round control: the disc fills discFrac of its square texture; brightness < 1 shows it pressed.</summary>
        void DrawDisc(string key, Vector2 centre, float radius, float discFrac, float brightness, float alpha = 0.95f)
        {
            var tex = Art.Texture(key);
            if (tex == null) return;
            float size = 2f * radius / discFrac;
            var c = GUI.color;
            GUI.color = new Color(brightness, brightness, brightness, alpha);
            GUI.DrawTexture(new Rect(centre.x - size * 0.5f, centre.y - size * 0.5f, size, size), tex);
            GUI.color = c;
        }
    }

    /// <summary>Starts the game in whatever scene is open, so pressing Play always works.</summary>
    public static class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            if (Object.FindAnyObjectByType<GameManager>() == null)
                new GameObject("Tapestry Game").AddComponent<GameManager>();
        }
    }
}
