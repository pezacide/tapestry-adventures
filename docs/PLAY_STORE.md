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
2. **Unity**: open the project once so it fetches the In-App Purchasing package. The project uses **In-App Purchasing 5.5.0** (Google Play Billing Library 9; see *About IAP 5.5.0* below). Check the Console for errors, and that **Window > Package Manager** shows 5.5.0. `Store.cs` is written for version 5's `StoreController`, so a different major version will not compile. You do **not** need to link a cloud project or sign in to Unity services: the product list is in the code, and Unity's guide says such projects can skip linking (their sales then just do not appear in Unity's dashboard; use the Play Console's reports).
2b. **Switch off Unity Diagnostics** (so Unity collects no extra crash or device data beyond the purchase service): **Edit > Project Settings > Services > Diagnostics**, set *Diagnostics Data* to **Disabled**. (Google Play's own Android vitals still report crashes to you, with no code in the game.)
3. **Signing key**: Project Settings > Player > Android > Publishing Settings > tick *Custom Keystore*, then *Keystore Manager* to create one. **Back up the keystore file and its passwords** (losing them means you cannot update the game). Leave **Play App Signing** on in the Play Console.
4. **Version**: raise `VersionCode` in `unity/Assets/Scripts/Editor/AndroidBuild.cs` for every upload.

### 2. In the Play Console
1. **Create the app** (name "Ye Tapestrie Adventures", free, game). The package name is `com.pezacide.tapestryadventures`, and it can never change once published.
2. **Set up the product**: *Monetize > Products > In-app products > Create product*. Product ID `ye_whole_tapestrie`, name "Ye Whole Tapestrie", a short description, **price £3.99** (Play converts it to other currencies, and shows the player their own). Activate it.
3. **Store listing**: short and long descriptions, a **512x512 icon** (done), a **1024x500 feature graphic**, and at least 2 phone screenshots (landscape is fine).
4. **Privacy policy URL**: a web page is required. `docs/privacy-policy.md` is a ready draft. Host it somewhere public (GitHub Pages is free), fill in the contact line, and paste the address in.
5. **App content forms**: the **content rating** questionnaire (expect "Everyone": mild cartoon fantasy violence), **target audience** (13+ avoids the Families Policy extras; if you choose to include children, the Designed for Families rules apply), **Data safety**, and **Ads** (answer *No ads*).
6. **Data safety**: Unity IAP 5.4 and later sends data to Unity, so you must declare it. Unity's own page ([Google Play data safety, IAP 5.4 and later](https://docs.unity.com/en-us/iap/privacy-and-consent/google-play-data-safety)) lists these, all **collected, not shared, required** (the player cannot opt out), and **encrypted in transit**:
   - **Location: approximate location** (ephemeral). Purposes: app functionality, analytics.
   - **Personal info: user IDs / personal identifiers**. Purposes: app functionality, analytics.
   - **Financial info: purchase history**. Purposes: app functionality, analytics.
   - **App info and performance: crash logs** (analytics), **diagnostics**, and **other app performance data** (app functionality, analytics).
   - **Device or other IDs**. Purposes: app functionality, analytics.

   For "can users request that their data is deleted?", the SDK has no built-in way; Unity handles requests manually through its IAP support form, on your instruction. Answer *yes* and say in the privacy policy that people can email you, then you pass the request to Unity. The game itself collects nothing else (no accounts, no ads, no analytics), and the payment is handled by Google Play. **Check Unity's page again before you submit**, because it changes with the SDK version.
7. **Testing**: upload the `.aab` to **Internal testing** first, add yourself under *License testers* (Setup > License testing) so test purchases cost nothing, and try the whole buy/restore flow on a real phone with a Play-installed build (purchases do not work in sideloaded APKs or the editor).
8. **Closed test, then Production**: run the closed test (see step 1 above), then promote to production.

### 3. After release
- Updates: bump `VersionCode`, build the `.aab`, upload to a track.
- **Fees**: Google keeps 15% of sales on the first $1M a year; VAT/sales tax is also taken out of the price. £3.99 leaves roughly £2.80 to you before income tax.

## Known things to watch
- **About IAP 5.5.0.** Google now requires **Play Billing Library 8 or later** for new apps and updates; IAP 5.4 and later uses Billing Library 9.0.0 (per Unity's upgrade guide), which also covers the next Google deadline (about 31 August 2027, for moving off Billing Library 8 on new updates; check Google's deprecation FAQ). The cost is the data in step 6. **Consent:** Unity's privacy overview says purchase events "are sent regardless of the consent state", and that consent (the `UnityConsent` API) only affects advertising identifiers, which this game does not use, so the game sets no consent. If you ever add ads or analytics, add a consent screen first.
- **`Store.cs` was written from Unity's guides, not run against the real package.** It uses only calls that appear in Unity's own upgrade and purchases guides (`StoreController`, `Connect`, `FetchProducts`, `FetchPurchases`, `PurchaseProduct`, `ConfirmPurchase`, and the `OnPurchasePending`, `OnPurchasesFetched`, `OnPurchaseFailed` and related events). The price text is read by name, so if Unity renames that field the paywall just shows no price. If Unity shows compile errors in `Store.cs`, copy them to whoever maintains the game. Test the whole buy and restore flow from **Internal testing** (step 7) before release.
- **Unity recommends `PurchasingController` for new projects from 5.5**, which handles several stores at once. `StoreController` (used here) is still supported and is what the upgrade guide documents; moving later is a small change to `Store.cs`.
- **Target API level.** Google also raises the required Android target API level each year (API 36 from 31 August 2026, according to current guidance). Install SDK Platform 36 through Unity Hub (Installs > your editor > Add modules / Android SDK) if the Play Console rejects the upload for that reason.
- **Refunds.** The unlock is remembered on the phone. A refunded purchase is not taken back automatically; for a £3.99 game that is a reasonable trade for simplicity.
- **iPhone later.** The same product works on Apple (`ye_whole_tapestrie` in App Store Connect), and Apple *requires* the "Restore a former purchase" link that is already there.
