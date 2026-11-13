# Putting Ye Tapestrie Adventures on Google Play

The plan: **World 1 is free; one in-app purchase, "Ye Whole Tapestrie" (£3.99 / $3.99), unlocks Worlds 2 to 7 for good.** No ads, no accounts.

## What is already built
- World 1 (levels 1-1 to 1-4) plays free. When the free tapestry ends, or when a locked realm is chosen, a paywall offers the unlock. The title screen also has an "Unlocke Worldes 2 to 7" link.
- The unlock is one **non-consumable** product with the id **`ye_whole_tapestrie`** (the code's `Store.ProductId`; the Play Console must use exactly this).
- On Google Play the store hands back what a player already owns each time the game starts, so a re-install or a new phone restores it by itself. There is also a "Restore a former purchase" link.
- In the Unity editor a "purchase" is simulated so the paywall can be tried; **Tapestry > Store > Forget the Purchase** puts the locks back (stop, then press Play again). A real build with no store connection never gives the unlock away.
- **Tapestry > Android > Build Play Store Bundle (.aab)** makes the file Google Play takes.

## The steps
### 1. Before any upload
1. **Google Play developer account.** A one-off fee (about $25). New personal accounts must also run a **closed test with at least 12 testers for 14 days** before they can publish to production. Start this early.
2. **Unity**: open the project once so it fetches the In-App Purchasing package. The project pins **In-App Purchasing 4.15.1** (see *Why IAP 4.15.1* below). Check the Console for errors, and that **Window > Package Manager** shows version 4.15.1 installed. **Do not click "Update" to 5.x**: version 5 has a different code interface, and it collects personal data (see below).
2b. **Switch off Unity Diagnostics** (so the game collects no crash or device data): **Edit > Project Settings > Services > Diagnostics**, set *Diagnostics Data* to **Disabled**. (Google Play's own Android vitals still report crashes to you, with no code in the game.) Then, optionally, check **Unity Dashboard > Developer Data** for anything else switched on.
3. **Signing key**: Project Settings > Player > Android > Publishing Settings > tick *Custom Keystore*, then *Keystore Manager* to create one. **Back up the keystore file and its passwords** (losing them means you cannot update the game). Leave **Play App Signing** on in the Play Console.
4. **Version**: raise `VersionCode` in `unity/Assets/Scripts/Editor/AndroidBuild.cs` for every upload.

### 2. In the Play Console
1. **Create the app** (name "Ye Tapestrie Adventures", free, game). The package name is `com.pezacide.tapestryadventures`, and it can never change once published.
2. **Set up the product**: *Monetize > Products > In-app products > Create product*. Product ID `ye_whole_tapestrie`, name "Ye Whole Tapestrie", a short description, **price £3.99** (Play converts it to other currencies, and shows the player their own). Activate it.
3. **Store listing**: short and long descriptions, a **512x512 icon** (done), a **1024x500 feature graphic**, and at least 2 phone screenshots (landscape is fine).
4. **Privacy policy URL**: a web page is required. `docs/privacy-policy.md` is a ready draft. Host it somewhere public (GitHub Pages is free), fill in the contact line, and paste the address in.
5. **App content forms**: the **content rating** questionnaire (expect "Everyone": mild cartoon fantasy violence), **target audience** (13+ avoids the Families Policy extras; if you choose to include children, the Designed for Families rules apply), **Data safety**, and **Ads** (answer *No ads*).
6. **Data safety**: with IAP 4.15.1 and Unity Diagnostics switched off, answer that the app **does not collect or share any user data types**. Unity's own page for IAP versions before 5.4 states that the SDK "does not collect any personal data about app users" ([Google Play data safety, IAP earlier than 5.4](https://docs.unity.com/en-us/iap/privacy-and-consent/legacy-google-play-data-safety)). Purchases are handled by Google Play. If you ever leave Unity Diagnostics on, Unity's page says it collects crash logs, app diagnostics, performance data and device identifiers ([Google Play data safety, Diagnostics](https://docs.unity.com/en-us/cloud/developer-data/google-data-safety)), and you must declare those and update the privacy policy.
7. **Testing**: upload the `.aab` to **Internal testing** first, add yourself under *License testers* (Setup > License testing) so test purchases cost nothing, and try the whole buy/restore flow on a real phone with a Play-installed build (purchases do not work in sideloaded APKs or the editor).
8. **Closed test, then Production**: run the closed test (see step 1 above), then promote to production.

### 3. After release
- Updates: bump `VersionCode`, build the `.aab`, upload to a track.
- **Fees**: Google keeps 15% of sales on the first $1M a year; VAT/sales tax is also taken out of the price. £3.99 leaves roughly £2.80 to you before income tax.

## Known things to watch
- **Why IAP 4.15.1, and not 5.x.** Google now requires **Play Billing Library 8 or later** for new apps and updates (the extension ended 1 November 2026). IAP 4.15.1 includes Billing Library 8.3.0, so it passes. IAP 5.4 and later (Billing Library 9) is the newer line, but Unity's own documentation says **5.4 and later collects personal data** (purchase history, personal identifiers, approximate location) for its direct-to-consumer features, and expects you to manage user consent. For a game with no accounts and no ads, 4.15.1 keeps the Data Safety form clean. **Plan to move to 5.x later:** Google's published schedule (check its deprecation FAQ) expects a move off Billing Library 8 by about **31 August 2027** for new updates. Everything needed is in Unity's upgrade guide (docs.unity.com/en-us/iap/upgrade-to-iap-v5), and `Store.cs` is the only file that touches the store. When you do, set up Unity's consent (`UnityConsent`) and revisit the Data Safety form and the privacy policy.
- **Target API level.** Google also raises the required Android target API level each year (API 36 from 31 August 2026, according to current guidance). Install SDK Platform 36 through Unity Hub (Installs > your editor > Add modules / Android SDK) if the Play Console rejects the upload for that reason.
- **Refunds.** The unlock is remembered on the phone. A refunded purchase is not taken back automatically; for a £3.99 game that is a reasonable trade for simplicity.
- **iPhone later.** The same product works on Apple (`ye_whole_tapestrie` in App Store Connect), and Apple *requires* the "Restore a former purchase" link that is already there.
