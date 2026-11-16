# Publishing *Into the Frame* on every platform

A guide to getting the games onto Google Play, the Apple App Store, Steam, Xbox, PlayStation and Nintendo consoles. It covers what each platform needs, what it costs, how long it takes, and the order we should do them in.

**How reliable is this?** It was assembled in November 2026 from web searches, with no access to the platforms' own partner portals. Facts marked **(official)** came from the platform's own documentation, and facts marked **(third-party)** came from guides and press that could be out of date. Anything unmarked is from this repo. **Console terms are mostly under NDA**, so they cannot be fully known until each platform approves us. Check each platform's own pages before you pay or sign anything.

Android is already written up in detail in [`PLAY_STORE.md`](PLAY_STORE.md). This guide does not repeat it.

---

## 1. At a glance

| Platform | Up-front cost | Gate to get in | Rough time to approval | Revenue cut | Unity route | Difficulty |
|---|---|---|---|---|---|---|
| **Google Play** | about $25 once | Developer account; new personal accounts need a 14-day closed test with 12 testers (see `PLAY_STORE.md`) | days, plus the 14-day test | 15% on the first $1M a year | built in | easy |
| **Apple App Store** | $99 a year **(official)** | Apple Developer Program, plus a **Mac with Xcode** | 1 to 3 days per review (third-party) | 30%, or 15% under the Small Business Program (third-party) | built in; Xcode project built on a Mac | medium |
| **Steam (PC, Mac, Linux, Steam Deck)** | $100 per game **(third-party, widely quoted)**, credited back after $1,000 in revenue | Steamworks account, tax and bank forms, store page, Valve review | about a week; the store page review is separate (third-party) | 30%, falling to 25% and 20% at higher revenue (third-party) | Windows, Mac and Linux builds plus Steamworks.NET or Facepunch.Steamworks | medium |
| **Xbox** | no fee to apply or certify **(official)**; a small one-time Partner Center fee may apply | Register as an Xbox partner, ID@Xbox application, concept approval | weeks | not public; check on approval | Unity's GDK support | hard |
| **PlayStation** | not public; Sony has run a dev-kit loan program | PlayStation Partners registration (a legal entity), then Sony's developer agreement | 4 to 8 weeks (third-party) | not public | Unity's PlayStation support, after approval | hard |
| **Nintendo Switch / Switch 2** | not public; dev kit price is under NDA | Nintendo Developer Portal (open registration), then a separate and selective step for dev kit access | uncertain | not public | Unity's Switch module, after approval | hard |

**Recommended order:** Google Play (already planned) → Apple → Steam → Xbox → Switch → PlayStation. The mobile launches prove the game and the paywall, Steam is the cheapest big step, and the consoles are worth doing only once the game has sales and a track record. **Treat consoles as "decide after Steam".**

---

## 2. Before you start: things every platform asks for

Collect these once. They are the slow part, and they are reused everywhere.

- **A legal identity.** A registered company is much easier, and PlayStation expects a legal entity (some countries also allow sole traders). Xbox asks for business information, and a **D-U-N-S number** is optional but speeds up identity checks (third-party). If we publish as a company, set it up before applying to any console.
- **Tax and bank details**, for payouts. Expect US tax forms (W-8BEN or W-9) on Steam and the others.
- **A public privacy policy URL.** Required by Apple and Google. `privacy-policy.md` is a draft.
- **Age ratings.** Google Play and several other storefronts use the **IARC** questionnaire (check each one). Apple has its own questionnaire, and Apple's news says responses become required for new apps and updates from September 2026 **(official)**. Physical releases need PEGI and ESRB ratings from the real boards.
- **Store assets.** Icon, key art, screenshots, a trailer, short and long descriptions. Sizes differ by store, so shoot screenshots at 16:9 and crop. One trailer works everywhere.
- **Support contact and a website or landing page.** Steam, Xbox and Nintendo all show these.
- **Legal clean-up.** The game must use only public-domain or our own art (we redraw everything) and our own music (generated from code).

### Game design consequences

- **Our model (World 1 free, one unlock) works on phones.** On Steam the equivalent is either a **free demo plus a paid full game**, or a free-to-play app with a DLC unlock. On consoles, expect to sell a **paid full game**, with an optional free demo, because console stores have their own rules about in-app purchases.
- **`Store.cs` is tied to Unity IAP, which only covers the phone stores.** Before the PC or console ports, put the unlock behind a small interface such as "does this player own the full game?", and write one implementation per platform: Google Play and Apple through Unity IAP, Steam through the Steamworks DLC or ownership check, and each console through its own SDK. Doing this early is cheap and avoids a rewrite later.
- **Controllers.** Consoles and Steam Deck need full gamepad support, with no mouse or touch needed to navigate any menu. The tapestry game already has controller support.
- **Save data, achievements and trophies** are required or expected on consoles (each has its own system), and good practice on Steam.

---

## 3. Google Play (Android)

Follow [`PLAY_STORE.md`](PLAY_STORE.md). In short: Play Console account (about $25), signed `.aab`, the in-app product `ye_whole_tapestrie`, listing, privacy policy, content rating, Data safety, then **Internal testing → Closed test (12 testers, 14 days) → Production**. Start the closed test early, since the 14 days is the real delay.

## 4. Apple App Store (iPhone, iPad, Mac)

1. **Join the Apple Developer Program** ($99 a year, **official**). A free Apple ID only installs onto your own device for 7 days.
2. **Get a Mac with Xcode.** Unity makes the Xcode project, but only Xcode can sign and upload it. A second-hand Mac mini, a rented cloud Mac or Unity's cloud build service all work (see the iOS notes in the repo's Unity README).
3. In **App Store Connect**, create the app record. The bundle ID is fixed once published, so choose it carefully (follow the `com.pezacide.…` pattern used on Android).
4. Create the same non-consumable product (`ye_whole_tapestrie`). Apple **requires** a visible "Restore purchases" link, which the game already has.
5. Fill in the listing, screenshots (required sizes per device class), **privacy nutrition labels** (**official**: required for new apps and updates; declare what Unity IAP sends), the age-rating questionnaire, and the privacy policy URL.
6. Test with **TestFlight** (internal testers need no review, and external testers need a short beta review).
7. In Xcode use **Product → Archive → Distribute App → App Store Connect**, then submit for review. Expect rejections to be rare for a simple game, but common reasons are missing restore-purchase, broken links, and crashes on a device we did not test.
8. **Build requirements move every year** (Xcode and SDK versions). Check Apple's current "Submit your apps" page before the build, not after.

## 5. Steam (PC, Mac, Linux, Steam Deck)

1. **Create a Steamworks account** and complete the onboarding: identity, tax and bank details. This step is the slowest, so do it early.
2. **Pay the app fee** (about $100 per game; **third-party**, and usually described as credited back once the game earns $1,000. Confirm Valve's current wording).
3. **Create the app** in Steamworks. You get an App ID, which also gives you a store page you can build up well ahead of launch.
4. **Build the store page** (capsule art in several sizes, screenshots, a trailer, tags, a short description). Publish it as "coming soon" **as early as possible**, because wishlists drive a launch.
5. **Build the game**: Windows is the priority, plus Mac and Linux if the Unity builds are clean. Add the Steamworks SDK wrapper for achievements, cloud saves and the ownership check. Upload through the **SteamPipe** tools and set a build live on a branch.
6. **Steam Deck**: it is nearly free if there is full controller support, readable text at 800p and no launcher or login screens. Valve gives a "Verified", "Playable" or "Unsupported" status.
7. **Valve reviews** the build and the store page (separate reviews, each taking days). After release a store page can be changed but the **release date and "coming soon" period have minimums**, so check the current rules.
8. **Model:** one paid game (a low price, with the "first world as a demo" as a **free Demo app**) is the usual Steam fit. The mobile paywall does not translate directly.

## 6. Xbox

1. **Register as an Xbox partner** with a Microsoft account, then the **Xbox Partner Dashboard**, and submit business information and an NDA. A company MSA cannot be used for this (**official**).
2. **Enroll in Partner Center** and note the Seller ID, and complete the Windows and Xbox enrollment.
3. **Apply to ID@Xbox** (the self-publishing program for independent developers). Microsoft states there are **no fees** to apply, certify, publish or update (**official**); there may be a small one-time cost for the Partner Center account.
4. **Concept approval.** Until the game is approved you have only limited services and cannot publish to Xbox consoles. Once approved, send the game's Store ID to the Xbox team and ask them to onboard the product.
5. **Build with the Microsoft GDK** (Unity supports it once you have access). Add Xbox features such as Xbox Live sign-in, achievements and save data, which Xbox expects.
6. **Certification:** submit the packages for testing against the Xbox Requirements (XRs), fix any failures, then release. Console certification fails games for small things, so budget for at least one rejection.
7. **Play Anywhere** and the Xbox PC app are options once one build exists.

## 7. PlayStation

1. **Register on PlayStation Partners** (partners.playstation.net). Expect to give company details, a corporate domain email, an IP address and a short project pitch (third-party). Applicants are generally legal entities, with some European exceptions for sole traders.
2. **Wait for approval** (4 to 8 weeks is a third-party estimate; Sony publishes no timeline). Then sign Sony's developer agreement, which unlocks the tools and documentation.
3. **Dev kits.** Sony has run a **Development Hardware Loan program** for indies in the past, giving newly licensed partners a PS5 dev kit and a test kit free. Terms may have changed. Ask when you apply.
4. **Build** with Unity's PlayStation support (available to approved partners), and add trophies, save data and the PlayStation controller features.
5. **Certification** against Sony's technical requirements, then release. Sony also approves the **concept**, so a prototype or trailer helps.
6. **Plan for it being the most closed of the three.** If the game has sales elsewhere first, the application is much easier.

## 8. Nintendo Switch and Switch 2

1. **Register on the Nintendo Developer Portal.** Nintendo says this is its single developer program and anyone may register. Minors may not, and individuals can (**official**).
2. **Dev kit access is a separate, selective step** (third-party reports, with the criteria unpublished). A shipped game, store momentum or a publisher attached all help. Kit prices are under NDA.
3. **Unity's Switch module** is available only once Nintendo confirms access (engine vendors commonly require the proof of approved status). Budget real time for **performance work**, since Switch hardware is far weaker than a PC and our art uses a heavy per-pixel embroidery filter that may need a cheaper pre-rendered path.
4. **Nintendo-specific features:** touch (handheld), HD controllers, Joy-Con, and the eShop rules on in-app purchases.
5. **Ratings:** digital releases generally use IARC; physical releases need the real boards.
6. **Be realistic.** Because dev-kit access is selective, the simplest route is often a **third-party publisher or porting company** that already has Nintendo access. Treat "do it ourselves" as a decision after Steam sales exist.

---

## 9. What this guide could not confirm

- **No official fee or revenue split for PlayStation or Nintendo.** Both are under NDA.
- **Steam's fee, refund wording and revenue tiers are from third-party guides.** Valve's own Steamworks documentation is the source to check.
- **Apple's Xcode and SDK minimum version for new submissions** (third-party guides say Xcode 26 or later) and the exact September 2026 age-rating deadline.
- **Timelines.** Every approval time here (apart from Google's 14-day test) is an estimate.
- **The game's own readiness:** the Android build, the Apple build and the interface for ownership checks (section 2) are not done.

## 10. Checklist per game

1. Choose the store name and subtitle (see the naming note in [`NAME_CHECK.md`](NAME_CHECK.md)).
2. Build the interface for ownership checks, then each platform's implementation.
3. Pass the age-rating questionnaires.
4. Create the store page and assets, and publish a "coming soon" page on Steam early.
5. Run the beta: Play internal and closed test, TestFlight, Steam beta branch.
6. Submit, then fix any rejection and resubmit.
7. Launch on mobile and Steam together if possible, then decide on consoles.
