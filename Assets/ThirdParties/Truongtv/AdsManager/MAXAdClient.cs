#if USING_MAX
using System;
using System.Threading.Tasks;
using ThirdParties.Truongtv.LogManager;
using ThirdParties.Truongtv.SoundManager;
using Truongtv.Services.Ad;
using UnityEngine;
public class MAXAdClient : IAdClient
{
    private const string MaxSdkKey =
        "ZoNyqu_piUmpl33-qkoIfRp6MTZGW9M5xk1mb1ZIWK6FN9EBu0TXSHeprC3LMPQI7S3kTc1-x7DJGSV8S-gvFJ";

#if UNITY_ANDROID
    private const string INTERSTITIAL_AD_UNIT_ID = "4e5521fbd4bcce18";
    private const string REWARDED_AD_UNIT_ID = "d31157e19af986a7";
    private const string BANNER_AD_UNIT_ID = "3b807d99fe628b76";
#elif UNITY_IOS
    private const string INTERSTITIAL_AD_UNIT_ID = "c975ffa4ae3924e2";
    private const string REWARDED_AD_UNIT_ID = "603f4d0aaae6f89d";
    private const string BANNER_AD_UNIT_ID = "2324ff5dc9cded9e";
#endif
    private Action<bool> _adCallback;
    private Action<bool> _bannerCallback;

    #region public method

    public void Initialized()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            Debug.Log("MAX SDK Initialized");
            InitializeInterstitialAds();
            InitializeRewardedAds();
            InitializeBannerAds();
           
        };

        MaxSdk.SetSdkKey(MaxSdkKey);
        MaxSdk.InitializeSdk();
    }

    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        double revenue = impressionData.Revenue;
        var impressionParameters = new[]
        {
            new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
            new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
            new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
            new Firebase.Analytics.Parameter("ad_format", impressionData.Placement), 
            new Firebase.Analytics.Parameter("value", revenue),
            new Firebase.Analytics.Parameter("currency", "USD"), // All Applovin revenue is sent in USD
        };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    }

    public void OnApplicationPause(bool isPause)
    {
        // Display the app open ad when the app is foregrounded
        if (!isPause)
        {
            Debug.Log("AppOpenAdManager.Instance.ShowAdIfAvailable");
            AppOpenAdManager.Instance.ShowAdIfAvailable();
        }
    }

    public bool IsRewardVideoLoaded()
    {
        return MaxSdk.IsRewardedAdReady(REWARDED_AD_UNIT_ID);
    }

    public void ShowRewardVideo(Action<bool> actionFinishAd = null)
    {
       AppsflyerConfig.LogEvent("af_rewarded_ad_eligible");
        if (MaxSdk.IsRewardedAdReady(REWARDED_AD_UNIT_ID))
        {
            AppOpenAdManager.isJustPauseByMediation = true;
            AppsflyerConfig.LogEvent("af_rewarded_api_called");
            _adCallback = actionFinishAd;
            MaxSdk.ShowRewardedAd(REWARDED_AD_UNIT_ID);
        }
        else
        {
            actionFinishAd?.Invoke(false);
        }
    }

    public bool IsInterstitialLoaded()
    {
        return MaxSdk.IsInterstitialReady(INTERSTITIAL_AD_UNIT_ID);
    }

    public void ShowInterstitial(Action<bool> actionFinishAd = null)
    {
        AppsflyerConfig.LogEvent("af_inters_ad_eligible");
        if (MaxSdk.IsInterstitialReady(INTERSTITIAL_AD_UNIT_ID))
        {
            AppOpenAdManager.isJustPauseByMediation = true;
            AppsflyerConfig.LogEvent("af_inters_api_called");
            _adCallback = actionFinishAd;
            MaxSdk.ShowInterstitial(INTERSTITIAL_AD_UNIT_ID);
        }
        else
        {
            actionFinishAd?.Invoke(false);
        }
    }

    public void ShowBannerAd(Action<bool> actionFinishAd = null)
    {
        _bannerCallback = actionFinishAd;
        MaxSdk.ShowBanner(BANNER_AD_UNIT_ID);
        _bannerCallback?.Invoke(true);
    }

    public void HideBannerAd()
    {
        MaxSdk.HideBanner(BANNER_AD_UNIT_ID);
    }

    #endregion

    #region Interstitial Ads

    int _retryAttempt;

    private void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(INTERSTITIAL_AD_UNIT_ID);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        _retryAttempt = 0;
    }

    private async void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        _retryAttempt++;
        var retryDelay = Math.Pow(2, Math.Min(6, _retryAttempt));
        await Task.Delay(TimeSpan.FromSeconds(retryDelay));
        LoadInterstitial();
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        AppsflyerConfig.LogEvent("af_inters_displayed");
        Bgm.Instance.Pause();
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo)
    {
        AppOpenAdManager.isJustPauseByMediation = false;
        _adCallback?.Invoke(false);
        LoadInterstitial();
    }
    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        _adCallback?.Invoke(true);
        Bgm.Instance.Resume();
        LoadInterstitial();
    }

    #endregion

    #region Rewarded Ads

    private int _rewardedRetryAttempt;
    private void InitializeRewardedAds()
    {
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(REWARDED_AD_UNIT_ID);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        
    }

    private async void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        _rewardedRetryAttempt++;
        var retryDelay = Math.Pow(2, Math.Min(6, _rewardedRetryAttempt));
        await Task.Delay(TimeSpan.FromSeconds(retryDelay));
        LoadRewardedAd();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
       AppsflyerConfig.LogEvent("af_rewarded_ad_displayed");
        Bgm.Instance.Pause();
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo)
    {
        AppOpenAdManager.isJustPauseByMediation = false;
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        LoadRewardedAd();
        Bgm.Instance.Resume();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
       _adCallback?.Invoke(true);
    }

    #endregion

    #region Banner ads

    private void InitializeBannerAds()
    {
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdk.CreateBanner(BANNER_AD_UNIT_ID, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerExtraParameter(BANNER_AD_UNIT_ID, "adaptive_banner", "true");
        MaxSdk.SetBannerBackgroundColor(BANNER_AD_UNIT_ID,  new Color(1f, 1f, 1f, 0f));
    }
    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Banner ad loaded");
        
    }

    private void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Banner ad failed to load. MAX will automatically try loading a new ad internally.
        Debug.Log("Banner ad failed to load with error code: " + errorInfo.Code);
    }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Banner ad clicked");
    }

    #endregion
}
#endif