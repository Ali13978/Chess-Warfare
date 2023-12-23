using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener //This class only contains code for Android Platform
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    // specific mapping to Unity Purchasing's AddProduct, below.
    private static string Gold_10K = "chess_gold_10";
    private static string Gold_25K = "chess_gold_25";
    private static string Gold_60K = "chess_gold_60";
    private static string Gold_150K = "chess_gold_150";
    private static string Gold_500K = "chess_gold_500";
    private static string Gold_1500K = "chess_gold_1500";


    void Awake() 
    {
        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.Save();
    }

    public InfoPanel infoPanel;
    void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }

    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        builder.AddProduct(Gold_10K, ProductType.Consumable);
        builder.AddProduct(Gold_25K, ProductType.Consumable);
        builder.AddProduct(Gold_60K, ProductType.Consumable);
        builder.AddProduct(Gold_150K, ProductType.Consumable);
        builder.AddProduct(Gold_500K, ProductType.Consumable);
        builder.AddProduct(Gold_1500K, ProductType.Consumable);

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyGold(int selection)
    {
        //ProfileSaver profileSaver = new ProfileSaver();
        //PlayerProfile playerProfile = profileSaver.LoadProfile();

        Debug.Log(selection);
        if (selection == 1)
        {
            //if((playerProfile.pD.Gld+10000)<999999999)
            //{
                BuyProductID(Gold_10K);
            //}
            //else
            //{
            //    infoPanel.SetText("Can't buy coins your invertory is almost full");
            //    infoPanel.ShowInfoPanel();
            //}
        }
        else if (selection == 2)
        {
            //if ((playerProfile.pD.Gld + 25000) < 999999999)
            //{
                BuyProductID(Gold_25K);
            //}
            //else
            //{
            //    infoPanel.SetText("Can't buy coins your invertory is almost full");
            //    infoPanel.ShowInfoPanel();
            //}
        }
        else if (selection == 3)
        {
            //if ((playerProfile.pD.Gld + 60000) < 999999999)
            //{
                BuyProductID(Gold_60K);
            //}
            //else
            //{
            //    infoPanel.SetText("Can't buy coins your invertory is almost full");
            //    infoPanel.ShowInfoPanel();
            //}
        }
        else if (selection == 4)
        {
            //if ((playerProfile.pD.Gld + 150000) < 999999999)
            //{
                BuyProductID(Gold_150K);
            //}
            //else
            //{
            //    infoPanel.SetText("Can't buy coins your invertory is almost full");
            //    infoPanel.ShowInfoPanel();
            //}
        }
        else if (selection == 5)
        {
            //if ((playerProfile.pD.Gld + 500000) < 999999999)
            //{
                BuyProductID(Gold_500K);
            //}
            //else
            //{
            //    infoPanel.SetText("Can't buy coins your invertory is almost full");
            //    infoPanel.ShowInfoPanel();
            //}
        }
        else if (selection == 6)
        {
            //if ((playerProfile.pD.Gld + 1500000) < 999999999)
            //{
                BuyProductID(Gold_1500K);
            //}
            //else
            //{
            //    infoPanel.SetText("Can't buy coins your invertory is almost full");
            //    infoPanel.ShowInfoPanel();
            //}
        }
    }

    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                InitializePurchasing();
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {

        Debug.Log("processing Result 1");

        ProfileSaver profileSaver = new ProfileSaver();
        PlayerProfile playerProfile = profileSaver.LoadProfile();

        Debug.Log("processing Result 2");

        Debug.Log("product found :"+ args.purchasedProduct.definition.id);


        // A consumable product has been purchased by this user.
        if (String.Equals(args.purchasedProduct.definition.id, Gold_10K, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            infoPanel.SetText("You bought 10K Gold");
            infoPanel.ShowInfoPanel();
            playerProfile.pD.Gld += 10000;
            profileSaver.SaveProfile(playerProfile);


        }
        else if (String.Equals(args.purchasedProduct.definition.id, Gold_25K, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            infoPanel.SetText("You bought 25K Gold");
            infoPanel.ShowInfoPanel();
            playerProfile.pD.Gld += 25000;
            profileSaver.SaveProfile(playerProfile);


        }
        else if (String.Equals(args.purchasedProduct.definition.id, Gold_60K, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            infoPanel.SetText("You bought 60K Gold");
            infoPanel.ShowInfoPanel();
            playerProfile.pD.Gld += 60000;
            profileSaver.SaveProfile(playerProfile);


        }
        else if (String.Equals(args.purchasedProduct.definition.id, Gold_150K, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            infoPanel.SetText("You bought 150K Gold");
            infoPanel.ShowInfoPanel();
            playerProfile.pD.Gld += 150000;
            profileSaver.SaveProfile(playerProfile);


        }
        else if (String.Equals(args.purchasedProduct.definition.id, Gold_500K, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            infoPanel.SetText("You bought 500K Gold");
            infoPanel.ShowInfoPanel();
            playerProfile.pD.Gld += 500000;
            profileSaver.SaveProfile(playerProfile);


        }
        else if (String.Equals(args.purchasedProduct.definition.id, Gold_1500K, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            infoPanel.SetText("You bought 1500K Gold");
            infoPanel.ShowInfoPanel();
            playerProfile.pD.Gld += 1500000;
            profileSaver.SaveProfile(playerProfile);


        }
        // Or ... an unknown product has been purchased by this user. Fill in additional products here....
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            infoPanel.SetText("You bought an unknown product");
            infoPanel.ShowInfoPanel();
        }

        ProfileController profileController = FindObjectOfType<ProfileController>();
        profileController.SetCoins(playerProfile.pD.Gld);
        DatabaseController.Instance.UpdateCoins(playerProfile.UID, playerProfile.pD.Gld);

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        infoPanel.SetText("Buying Gold was unsuccessfull . " + failureReason.ToString());
        infoPanel.ShowInfoPanel();
    }
}