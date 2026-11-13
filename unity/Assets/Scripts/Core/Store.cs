using UnityEngine;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
#endif

namespace Tapestry
{
    /// <summary>The one purchase: "Ye Whole Tapestrie" unlocks Worlds 2 to 7 for good. World 1 is free.
    /// It is a non-consumable in-app product. On Google Play, Unity IAP hands back everything the player already owns each time
    /// it starts, so a re-install or a new phone restores the unlock by itself.
    ///
    /// This is written for Unity IAP 4.x (pinned to 4.15.1 on purpose: Google Play Billing 8.3.0, and no personal data collected).
    /// IAP 5.4+ has a different, event-based interface (StoreController) and collects personal data; see docs/PLAY_STORE.md before moving to it.
    ///
    /// Without Unity's In-App Purchasing package (UNITY_PURCHASING) a real build cannot sell anything (and so never gives the
    /// unlock away); inside the Unity editor a "purchase" is simulated, so the paywall can be tried.</summary>
    public static class Store
    {
        /// <summary>The product id; create the same one in the Google Play Console (Monetize, In-app products).</summary>
        public const string ProductId = "ye_whole_tapestrie";
        public const string OwnedKey = "owned:" + ProductId;
        public const int FreeWorlds = 1;

        public static bool Owned { get; private set; }
        public static string Price = "";      // the player's own currency, once the store has answered
        public static string Message = "";    // what the paywall shows after a try
        public static bool Busy { get; private set; }

        public static bool WorldLocked(int world) { return world > FreeWorlds && !Owned; }

        static bool started;

        public static void Init()
        {
            if (started) return;
            started = true;
            Owned = PlayerPrefs.GetInt(OwnedKey, 0) == 1;
#if UNITY_PURCHASING
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(ProductId, ProductType.NonConsumable);
            UnityPurchasing.Initialize(listener, builder);
#endif
        }

        public static void Buy()
        {
            Message = "";
            if (Owned) return;
#if UNITY_EDITOR
            Debug.Log("Tapestry: (editor) simulated purchase of " + ProductId);
            Grant();
#elif UNITY_PURCHASING
            if (controller == null) { Message = "Ye shoppe is not open yet. Try agayne in a moment."; return; }
            Busy = true;
            controller.InitiatePurchase(ProductId);
#else
            Message = "Ye shoppe is not open yet.";
#endif
        }

        /// <summary>Ask the store again what the player owns (for a new phone or a re-install).</summary>
        public static void Restore()
        {
            Message = "";
#if UNITY_EDITOR
            Message = PlayerPrefs.GetInt(OwnedKey, 0) == 1 ? "" : "(editor) nothing to restore.";
#elif UNITY_PURCHASING
            if (controller == null) { Message = "Ye shoppe is not open yet. Try agayne in a moment."; return; }
            var p = controller.products.WithID(ProductId);
            if (p != null && p.hasReceipt) Grant();
            else Message = "No former purchase was founde on this account.";
#else
            Message = "Ye shoppe is not open yet.";
#endif
        }

        static void Grant()
        {
            Busy = false;
            if (Owned) return;
            Owned = true; Message = "";
            PlayerPrefs.SetInt(OwnedKey, 1); PlayerPrefs.Save();
        }

#if UNITY_PURCHASING
        static IStoreController controller;
        static readonly Listener listener = new Listener();

        class Listener : IDetailedStoreListener
        {
            public void OnInitialized(IStoreController c, IExtensionProvider e)
            {
                controller = c;
                var p = c.products.WithID(ProductId);
                if (p == null) return;
                Price = p.metadata.localizedPriceString ?? "";
                if (p.hasReceipt) Grant();   // already bought: restored
            }
            public void OnInitializeFailed(InitializationFailureReason error) { Message = "Ye shoppe is closed (" + error + ")."; }
            public void OnInitializeFailed(InitializationFailureReason error, string message) { Message = "Ye shoppe is closed (" + error + ")."; }
            public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
            {
                if (e.purchasedProduct.definition.id == ProductId) Grant();
                return PurchaseProcessingResult.Complete;
            }
            public void OnPurchaseFailed(Product p, PurchaseFailureReason reason)
            {
                Busy = false;
                Message = reason == PurchaseFailureReason.UserCancelled ? "" : "The purchase fayled (" + reason + "). Thou wast not charged.";
            }
            public void OnPurchaseFailed(Product p, PurchaseFailureDescription d)
            {
                Busy = false;
                Message = d.reason == PurchaseFailureReason.UserCancelled ? "" : "The purchase fayled (" + d.reason + "). Thou wast not charged.";
            }
        }
#endif
    }
}
