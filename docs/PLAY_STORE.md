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
2. **Unity**: open the project once so it fetches the In-App Purchasing package (`com.unity.purchasing`). Check the Console for errors, and that **Window > Package Manager** shows it installed.
3. **Signing key**: Project Settings > Player > Android > Publishing Settings > tick *Custom Keystore*, then *Keystore Manager* to create one. **Back up the keystore file and its passwords** (losing them means you cannot update the game). Leave **Play App Signing** on in the Play Console.
4. **Version**: raise `VersionCode` in `unity/Assets/Scripts/Editor/AndroidBuild.cs` for every upload.

### 2. In the Play Console
1. **Create the app** (name "Ye Tapestrie Adventures", free, game). The package name is `com.pezacide.tapestryadventures`, and it can never change once published.
2. **Set up the product**: *Monetize > Products > In-app products > Create product*. Product ID `ye_whole_tapestrie`, name "Ye Whole Tapestrie", a short description, **price £3.99** (Play converts it to other currencies, and shows the player their own). Activate it.
3. **Store listing**: short and long descriptions, a **512x512 icon** (done), a **1024x500 feature graphic**, and at least 2 phone screenshots (landscape is fine).
4. **Privacy policy URL**: a web page is required. `docs/privacy-policy.md` is a ready draft. Host it somewhere public (GitHub Pages is free), fill in the contact line, and paste the address in.
5. **App content forms**: the **content rating** questionnaire (expect "Everyone": mild cartoon fantasy violence), **target audience** (13+ avoids the Families Policy extras; if you choose to include children, the Designed for Families rules apply), **Data safety**, and **Ads** (answer *No ads*).
6. **Data safety**: the game itself collects nothing and has no accounts; progress is stored only on the phone. Purchases are handled by Google Play. **Check Unity's own IAP/analytics data notes** (docs.unity.com, "Unity IAP" and "Data privacy") before declaring, since the SDK may report purchase events to Unity; declare whatever Unity says it collects.
7. **Testing**: upload the `.aab` to **Internal testing** first, add yourself under *License testers* (Setup > License testing) so test purchases cost nothing, and try the whole buy/restore flow on a real phone with a Play-installed build (purchases do not work in sideloaded APKs or the editor).
8. **Closed test, then Production**: run the closed test (see step 1 above), then promote to production.

### 3. After release
- Updates: bump `VersionCode`, build the `.aab`, upload to a track.
- **Fees**: Google keeps 15% of sales on the first $1M a year; VAT/sales tax is also taken out of the price. £3.99 leaves roughly £2.80 to you before income tax.

## Known things to watch
- **Billing library version.** The project pins Unity IAP **4.12.2**. Google raises the minimum Play Billing Library version every year; if the Play Console warns that the billing library is too old when you upload, update the package (Window > Package Manager > In-App Purchasing). Version 5 of the package has a different code interface, so `Store.cs` would need a small rewrite.
- **Refunds.** The unlock is remembered on the phone. A refunded purchase is not taken back automatically; for a £3.99 game that is a reasonable trade for simplicity.
- **iPhone later.** The same product works on Apple (`ye_whole_tapestrie` in App Store Connect), and Apple *requires* the "Restore a former purchase" link that is already there.
