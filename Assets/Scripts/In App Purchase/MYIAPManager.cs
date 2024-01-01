using System;
using UnityEngine;
//using UnityEngine.Purchasing;


public class MYIAPManager : MonoBehaviour/*, IStoreListener*/
    {
        public static MYIAPManager instance { set; get; }

        //private static IStoreController m_StoreController;          // The Unity Purchasing system.
        //private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
        
        public string[] coinsProducts;

        void Awake()
        {
           instance = this;
        }
        void Start()
        {
            ////kProductIDNoAds = ServicesManager.instance.noAdsID;

            //// If we haven't set up the Unity Purchasing reference
            //if (m_StoreController == null)
            //{
            //    // Begin to configure our connection to Purchasing
            //    InitializePurchasing();
            //}
        }
        public void InitializePurchasing()
        {
        //    // If we have already connected to Purchasing ...
        //    if (IsInitialized())
        //    {
        //        // ... we are done here.
        //        return;
        //    }

        //    // Create a builder, first passing in a suite of Unity provided stores.
        //    var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
                           
        //    for(int k=0;k< coinsProducts.Length;k++)
        //    {
        //        builder.AddProduct(coinsProducts[k], ProductType.Consumable);

        //    }

        //UnityPurchasing.Initialize(this, builder);
        }
        private bool IsInitialized()
        {
        //// Only say we are initialized if both the Purchasing references are set.
        //return m_StoreController != null && m_StoreExtensionProvider != null;
        return false;
        }
        public void BuyNoAds()
        {
        }

         public void BuyCoinsPack(int id) 
         {
             BuyProductID(coinsProducts[id]);
             }

    void BuyProductID(string productId)
        {
            //// If Purchasing has been initialized ...
            //if (IsInitialized())
            //{
            //    // ... look up the Product reference with the general product identifier and the Purchasing 
            //    // system's products collection.
            //    Product product = m_StoreController.products.WithID(productId);

            //    // If the look up found a product for this device's store and that product is ready to be sold ... 
            //    if (product != null && product.availableToPurchase)
            //    {
            //        Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
            //        // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
            //        // asynchronously.
            //        m_StoreController.InitiatePurchase(product);
            //    }
            //    // Otherwise ...
            //    else
            //    {
            //        // ... report the product look-up failure situation  
            //        Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            //    }
            //}
            //// Otherwise ...
            //else
            //{
            //    // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            //    // retrying initiailization.
            //    Debug.Log("BuyProductID FAIL. Not initialized.");
            //}
        }
        public void RestorePurchases()
        {
            //// If Purchasing has not yet been set up ...
            //if (!IsInitialized())
            //{
            //    // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            //    Debug.Log("RestorePurchases FAIL. Not initialized.");
            //    return;
            //}

            //// If we are running on an Apple device ... 
            //if (Application.platform == RuntimePlatform.IPhonePlayer ||
            //    Application.platform == RuntimePlatform.OSXPlayer)
            //{
            //    // ... begin restoring purchases
            //    Debug.Log("RestorePurchases started ...");

            //    // Fetch the Apple store-specific subsystem.
            //    var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            //    // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            //    // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            //    apple.RestoreTransactions((result) => {
            //        // The first phase of restoration. If no more responses are received on ProcessPurchase then 
            //        // no purchases are available to be restored.
            //        Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            //    });
            //}
            //// Otherwise ...
            //else
            //{
            //    // We are not running on an Apple device. No work is necessary to restore purchases.
            //    Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            //}
        }
        public void OnInitialized(/*IStoreController controller, IExtensionProvider extensions*/)
        {
            //// Purchasing has succeeded initializing. Collect our Purchasing references.
            //Debug.Log("OnInitialized: PASS");

            //// Overall Purchasing system, configured with products for this application.
            //m_StoreController = controller;
            //// Store specific subsystem, for accessing device-specific store features.
            //m_StoreExtensionProvider = extensions;
        }
        public void OnInitializeFailed(/*InitializationFailureReason error*/)
        {
            //// Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            //Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }
        public /*PurchaseProcessingResult*/ void ProcessPurchase(/*PurchaseEventArgs args*/)
        {

        //for(int k=0;k < coinsProducts.Length;k++)
        //{
        //    if (String.Equals(args.purchasedProduct.definition.id, coinsProducts[k], StringComparison.Ordinal))
        //    {
        //        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
        //        Debug.Log("You have just purchased coins!");

        //        onCoinPurchaseSuccess(k);
        //    }
        //    else
        //    {
        //        Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        //    }
        //}



           


        //    return PurchaseProcessingResult.Complete;
        }
    public InfoPanel infoPanel;

    void onCoinPurchaseSuccess(int id) 
        {

        infoPanel.SetText("You bought Gold");
        infoPanel.ShowInfoPanel();


        int gold = 0;

        if (id == 0)
        {
            gold = 10000;
        }
        if (id == 1)
        {
            gold = 25000;
        }
        if (id == 2)
        {
            gold = 60000;
        }
        if (id == 3)
        {
            gold = 150000;
        }
        if (id == 4)
        {
            gold = 500000;
        }
        if (id == 5)
        {
            gold = 1500000;
        }

        print("you got gold :" + gold);


        ProfileSaver profileSaver = new ProfileSaver();
        PlayerProfile playerProfile = profileSaver.LoadProfile();


        playerProfile.pD.Gld += 10000;
        profileSaver.SaveProfile(playerProfile);


        ProfileController profileController = FindObjectOfType<ProfileController>();
        profileController.SetCoins(playerProfile.pD.Gld);
        DatabaseController.Instance.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);
    }


    public void OnPurchaseFailed(/*Product product, PurchaseFailureReason failureReason*/)
        {
            //// A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            //// this reason with the user to guide their troubleshooting actions.
            //Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }
    }