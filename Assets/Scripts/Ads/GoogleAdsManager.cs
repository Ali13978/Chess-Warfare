using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class GoogleAdsManager : MonoBehaviour
{
    public static GoogleAdsManager Instance;

#if UNITY_ANDROID

    private string appid = "ca-app-pub-9944277757397838~8040859475";
    private string RewardedAdId = "ca-app-pub-3940256099942544/5224354917";
   // private string RewardedAdId = "ca-app-pub-9944277757397838/2788532791"; //this is production id     

   private string BannerAdId = "ca-app-pub-3940256099942544/6300978111";
    //private string BannerAdId = "ca-app-pub-9944277757397838/2366213581"; //production id
#endif

#if UNITY_IOS
    private string appid = "ca-app-pub-9944277757397838~1016993966";
    private string RewardedAdId = "ca-app-pub-3940256099942544/5224354917";
    //private string RewardedAdId = "ca-app-pub-9944277757397838/5746576460"; //production id

    private string BannerAdId = "ca-app-pub-3940256099942544/6300978111";
    //private string BannerAdId = "ca-app-pub-9944277757397838/3883858123"; //production id
#endif

    private RewardedAd rewardedAd=null;
    private BannerView bannerView=null;

    private bool isLoading;
    private bool earnedReward=false;
    private int processNumber = -1;
    private bool ProcessCompleted = false;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appid);

        CreateAndLoadSilent();
    }

    public void SetEarnedReward(bool earnedReward)
    {
        this.earnedReward = earnedReward;
    }

    public bool GetEarnedReward()
    {
        return earnedReward;
    }

    public void SetProcess(int processNumber)
    {
        this.processNumber = processNumber;
    }

    public int GetProcesNumber()
    {
        return processNumber;
    }

    public void SetProcessCompleted(bool processCompleted)
    {
        this.ProcessCompleted = processCompleted;
    }

    public bool isProcessCompleted()
    {
        return ProcessCompleted;
    }

    public bool IsLoading()
    {
        return isLoading;
    }

    public void PlayRewarded()
    {
        if(isLoading)
        {
            InfoPanel.Instance.SetText("Loading Ads this might take some moment.. try again after some time");
            InfoPanel.Instance.ShowInfoPanel();
            processNumber = -1;
            ProcessCompleted = true;
            return;
        }
        else
        {
            if (this.rewardedAd.IsLoaded())
            {
                Debug.Log("showing ad");
                this.rewardedAd.Show();
            }
            else
            {
                CreateAndLoadSilent();
            }
        }
    }

    private void CreateAndLoadSilent()
    {
        Debug.Log("create and load silent");
        this.rewardedAd = new RewardedAd(RewardedAdId);

        isLoading = true;
        // Called when an ad request has successfully loaded.
        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;

        // Called when an ad request failed to show.
        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }


    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        isLoading = false;
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        isLoading = false;
        CreateAndLoadSilent();
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        isLoading = false;
        Debug.Log("showing ad");
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        ProcessCompleted = true;
        InfoPanel.Instance.SetText("Something went wrong when displaying ad");
        InfoPanel.Instance.ShowInfoPanel();
        CreateAndLoadSilent();
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        ProcessCompleted = true;
        CreateAndLoadSilent();
        MonoBehaviour.print("HandleRewardedAdClosed event received");
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        earnedReward = true;
        ProcessCompleted = true;
        if (processNumber==1) //50 coins
        {
            ProfileSaver profileSaver = new ProfileSaver();
            PlayerProfile playerProfile = profileSaver.LoadProfile();

            if ((playerProfile.pD.Gld + 50) < 999999999)
            {
                playerProfile.pD.Gld += 50;
                InfoPanel.Instance.SetText("Congratulations! You have earned 50 coins");
                InfoPanel.Instance.ShowInfoPanel();
            }
            profileSaver.SaveProfile(playerProfile);

            ProfileController profileController = FindObjectOfType<ProfileController>();
            profileController.SetCoins(playerProfile.pD.Gld);
        }
        processNumber = -1;
        CreateAndLoadSilent();
    }

    //banner ads
    
    public void ShowBanner(int option)
    {
        this.RequestBanner(option);
    }

    private void RequestBanner(int option)
    {
        Debug.Log("Showing Banner");

        if(option==1)
        {
            // Create a 320x50 banner at the bottom of the screen.
            this.bannerView = new BannerView(BannerAdId, AdSize.Banner, AdPosition.BottomRight);
        }
        else if(option==2)
        {
            // Create a 320x50 banner at the bottom of the screen.
            this.bannerView = new BannerView(BannerAdId, AdSize.Banner, AdPosition.BottomRight);
        }
        else if (option == 3)
        {
            // Create a 320x50 banner at the bottom of the screen.
            
            this.bannerView = new BannerView(BannerAdId, AdSize.Banner, AdPosition.TopRight);
        }

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }

    public void CloseBanner()
    {
        this.bannerView.Destroy();
    }

    public void CloseAndLoadNew(int option)
    {
        this.bannerView.Destroy();
        this.RequestBanner(option);
    }
}