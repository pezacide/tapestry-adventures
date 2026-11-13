using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Tapestry
{
    /// <summary>The one purchase: "Ye Whole Tapestrie" unlocks Worlds 2 to 7 for good. World 1 is free.
    /// It is a non-consumable in-app product, sold through the platform's own store (Google Play) with Unity IAP 5.x
    /// (<c>StoreController</c>): connect, fetch the product, fetch the player's purchases (which restores the unlock after a
    /// re-install or on a new phone), and on a pending order grant the unlock and confirm it.
    ///
    /// The product list is given in code (no Remote Catalog and no Unity sign-in), so the game does not ask Unity for
    /// accounts or player IDs. The unlock is only ever granted by the store, or by the simulated purchase in the Unity editor;
    /// if the store cannot be reached nothing is given away.
    ///
    /// Written against Unity's "Upgrade from IAP version 4 to version 5" and "Purchases" guides (docs.unity.com/en-us/iap).</summary>
    public static class Store
    {
        /// <summary>The product id; create the same one in the Google Play Console (Monetize, In-app products).</summary>
        public const string ProductId = "ye_whole_tapestrie";
        public const string OwnedKey = "owned:" + ProductId;
        public const int FreeWorlds = 1;

        public static bool Owned { get; private set; }
        public static string Price = "";      // the player's own currency, once the store has answered (blank if it cannot be read)
        public static string Message = "";    // what the paywall shows after a try
        public static bool Busy { get; private set; }

        public static bool WorldLocked(int world) { return world > FreeWorlds && !Owned; }

        static bool started;

        public static void Init()
        {
            if (started) return;
            started = true;
            Owned = PlayerPrefs.GetInt(OwnedKey, 0) == 1;   // remembered, so it works offline; the store re-confirms it each start
#if !UNITY_EDITOR
            Connect();
#endif
        }

        public static void Buy()
        {
            Message = "";
            if (Owned) return;
#if UNITY_EDITOR
            Debug.Log("Tapestry: (editor) simulated purchase of " + ProductId);
            Grant();
#else
            if (controller == null || !connected) { Message = "Ye shoppe is not open yet. Try agayne in a moment."; return; }
            Product product = null;
            foreach (var p in controller.GetProducts()) if (p.definition.id == ProductId) { product = p; break; }
            if (product == null) { Message = "Ye shoppe hath not yet shewn its wares. Try agayne in a moment."; return; }
            Busy = true;
            controller.PurchaseProduct(product);
#endif
        }

        /// <summary>Ask the store again what the player owns (for a new phone or a re-install).</summary>
        public static void Restore()
        {
            Message = "";
#if UNITY_EDITOR
            Message = PlayerPrefs.GetInt(OwnedKey, 0) == 1 ? "" : "(editor) nothing to restore.";
#else
            if (controller == null || !connected) { Message = "Ye shoppe is not open yet. Try agayne in a moment."; return; }
            restoring = true;
            controller.FetchPurchases();   // a purchase the store knows of arrives through OnPurchasesFetched
#endif
        }

        static void Grant()
        {
            Busy = false;
            if (Owned) return;
            Owned = true; Message = "";
            PlayerPrefs.SetInt(OwnedKey, 1); PlayerPrefs.Save();
        }

#if !UNITY_EDITOR
        static StoreController controller;
        static bool connected, restoring;

        static async void Connect()
        {
            try
            {
                controller = UnityIAPServices.StoreController();

                // handlers go on before Connect, or events that fire during connection are lost
                controller.OnPurchasePending += order =>
                {
                    // a completed purchase (or one left unfinished last time): grant it, then confirm it with the store
                    foreach (var item in order.CartOrdered.Items())
                        if (item.Product.definition.id == ProductId) Grant();
                    controller.ConfirmPurchase(order);
                    Busy = false;
                };
                controller.OnPurchaseConfirmed += order =>
                {
                    Busy = false;
                    if (order is FailedOrder failed)   // the unlock has been granted already; Unity keeps the order pending and tries again later
                        Debug.LogWarning("Tapestry: confirming the purchase failed (" + failed.FailureReason + "): " + failed.Details);
                };
                controller.OnPurchaseFailed += failed =>
                {
                    Busy = false;
                    Message = failed.FailureReason.ToString() == "UserCancelled" ? "" : "The purchase fayled (" + failed.FailureReason + "). Thou wast not charged.";
                };
                controller.OnPurchasesFetched += orders =>
                {
                    foreach (var order in orders.ConfirmedOrders)
                        foreach (var item in order.CartOrdered.Items())
                            if (item.Product.definition.id == ProductId) Grant();
                    foreach (var order in orders.PendingOrders)
                        foreach (var item in order.CartOrdered.Items())
                            if (item.Product.definition.id == ProductId) Grant();
                    if (restoring && !Owned) Message = "No former purchase was founde on this account.";
                    restoring = false;
                };
                controller.OnPurchasesFetchFailed += failure => { restoring = false; Message = "Ye shoppe could not be reached. Try agayne later."; };
                controller.OnProductsFetched += products =>
                {
                    foreach (var p in products) if (p.definition.id == ProductId) Price = PriceOf(p);
                    controller.FetchPurchases();   // recognises a purchase already made (a re-install, a new phone)
                };
                controller.OnProductsFetchFailed += failure => { Message = "Ye shoppe hath no wares to shew just now."; };
                controller.OnStoreDisconnected += description => { connected = false; Message = "Ye shoppe is closed. Try agayne later."; };

                await controller.Connect();
                connected = true;
                controller.FetchProducts(new List<ProductDefinition> { new ProductDefinition(ProductId, ProductType.NonConsumable) });
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Tapestry: the store could not be started: " + e.Message);
                Message = "Ye shoppe is closed. Try agayne later.";
            }
        }

        /// <summary>The player's own-currency price. Read by name so a rename in a later Unity IAP cannot stop the game compiling;
        /// if it cannot be read the paywall simply shows no price (Google Play's own purchase sheet always shows it).</summary>
        static string PriceOf(object product)
        {
            try
            {
                var t = product.GetType();
                object meta = null;
                var mp = t.GetProperty("metadata");
                if (mp != null) meta = mp.GetValue(product, null);
                else { var mf = t.GetField("metadata"); if (mf != null) meta = mf.GetValue(product); }
                if (meta == null) return "";
                var pp = meta.GetType().GetProperty("localizedPriceString");
                if (pp != null) return pp.GetValue(meta, null) as string ?? "";
                var pf = meta.GetType().GetField("localizedPriceString");
                if (pf != null) return pf.GetValue(meta) as string ?? "";
            }
            catch { }
            return "";
        }
#endif
    }
}
