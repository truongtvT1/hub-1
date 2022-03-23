#if USING_IRON_SOURCE
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThirdParties.Truongtv.LogManager;
using Truongtv.Services.Ad;
using UnityEngine;
#if UNITY_IOS || UNITY_IPHONE
using Unity.Advertisement.IosSupport;

#if UNITY_EDITOR 
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Xml;
#endif
#endif
#region IS class
public class IronSourceAdClient : IAdClient
{
#if UNITY_ANDROID
    private const string appKey = "13aee11a9";
#elif UNITY_IPHONE || UNITY_IOS
    private const string appKey = "13d249df1";
#endif
    private Action<bool> _adCallback;
    private Action<bool> _bannerCallback;
    private AdState _rewardVideoState = AdState.None;
    private AdState _interstitialState = AdState.None;

    enum AdState
    {
        None,
        Failed,
        Loading,
        Loaded
    }

    #region public function

    public void Initialized()
    {
#if UNITY_IPHONE || UNITY_IOS
        var trackingStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

        if (trackingStatus == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif

        IronSource.Agent.init(appKey);
        IronSource.Agent.setMetaData("Facebook_IS_CacheFlag", "IMAGE");
        IronSource.Agent.setMetaData("AppLovin_AgeRestrictedUser", "true");
        IronSource.Agent.setMetaData("AdMob_TFCD", "true");
        IronSource.Agent.setMetaData("AdMob_TFUA", "true");
        IronSource.Agent.shouldTrackNetworkState(true);
        IronSource.Agent.setAdaptersDebug(false);
        IronSource.Agent.validateIntegration();

        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent += IronSourceEventsOnonRewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdClickedEvent += IronSourceEventsOnonRewardedVideoAdClickedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent += IronSourceEventsOnonRewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;


        IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;

        IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
        IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
        IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
        IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
        IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
        IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;

        IronSourceEvents.onImpressionSuccessEvent += ImpressionSuccessEvent;
        
        RequestInterstitial();
        LoadBanner();
    }

    private void ImpressionSuccessEvent(IronSourceImpressionData impressionData)
    {
        if (impressionData != null)
        {
            Firebase.Analytics.Parameter[] AdParameters =
            {
                new Firebase.Analytics.Parameter("ad_platform", "ironSource"),
                new Firebase.Analytics.Parameter("ad_source", impressionData.adNetwork),
                new Firebase.Analytics.Parameter("ad_unit_name", impressionData.adUnit),
                new Firebase.Analytics.Parameter("ad_format", impressionData.instanceName),
                new Firebase.Analytics.Parameter("currency", "USD"),
                new Firebase.Analytics.Parameter("value", impressionData.revenue.Value)
            };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
        }
    }

    public void OnApplicationPause(bool isPause)
    {
        if (!isPause)
        {
            AppOpenAdManager.Instance.ShowAdIfAvailable();
        }
        IronSource.Agent.onApplicationPause(isPause);
    }

    public bool IsRewardVideoLoaded()
    {
        return _rewardVideoState == AdState.Loaded;
    }

    public void ShowRewardVideo(Action<bool> actionFinishAd = null)
    {
        AppsflyerConfig.LogEvent("af_rewarded_ad_eligible");
        if (IsRewardVideoLoaded())
        {
            _adCallback = actionFinishAd;
            AppOpenAdManager.isJustPauseByMediation = true;
            AppsflyerConfig.LogEvent("af_rewarded_api_called");
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            _adCallback.Invoke(false);
        }
    }

    public bool IsInterstitialLoaded()
    {
        return _interstitialState == AdState.Loaded;
    }

    public void ShowInterstitial(Action<bool> actionFinishAd = null)
    {
        AppsflyerConfig.LogEvent("af_inters_ad_eligible");
        if (IsInterstitialLoaded())
        {
            _adCallback = actionFinishAd;
            AppOpenAdManager.isJustPauseByMediation = true;
            AppsflyerConfig.LogEvent("af_inters_api_called");
            IronSource.Agent.showInterstitial();
        }
        else
        {
            _adCallback?.Invoke(false);
            _adCallback = null;
        }
    }

    public void ShowBannerAd(Action<bool> actionFinishAd = null)
    {
        _bannerCallback = actionFinishAd;
        IronSource.Agent.displayBanner();
        _bannerCallback?.Invoke(true);
    }

    public void HideBannerAd()
    {
        IronSource.Agent.hideBanner();
    }

    #endregion

    #region Reward Video

    public bool IsRewardVideoLoaded(string placement = null)
    {
        if (string.IsNullOrEmpty(placement))
            return _rewardVideoState == AdState.Loaded;
        return _rewardVideoState == AdState.Loaded &&
               !IronSource.Agent.isRewardedVideoPlacementCapped(placement);
    }

    public void ShowRewardVideo(string placement = null, Action<bool> actionCloseAd = null)
    {
        _adCallback = null;
        _adCallback = actionCloseAd;
        if (IsRewardVideoLoaded(placement))
        {
            if (string.IsNullOrEmpty(placement))
                IronSource.Agent.showRewardedVideo();
            else
                IronSource.Agent.showRewardedVideo(placement);
        }
        else
            _adCallback.Invoke(false);
    }

    void RewardedVideoAdOpenedEvent()
    {
        AppsflyerConfig.LogEvent("af_rewarded_ad_displayed");
    }

    async void RewardedVideoAdClosedEvent()
    {
        await Task.Delay(TimeSpan.FromSeconds(0.4f));
        if (_adCallback == null) return;
        _adCallback?.Invoke(false);
        _adCallback = null;
    }

    void RewardedVideoAvailabilityChangedEvent(bool available)
    {
        _rewardVideoState = available ? AdState.Loaded : AdState.Failed;
    }

    void RewardedVideoAdRewardedEvent(IronSourcePlacement placement)
    {
        _adCallback?.Invoke(true);
        _adCallback = null;
    }

    void RewardedVideoAdShowFailedEvent(IronSourceError error)
    {
        AppOpenAdManager.isJustPauseByMediation = false;
        _adCallback?.Invoke(false);
        _adCallback = null;
    }

    private void IronSourceEventsOnonRewardedVideoAdStartedEvent()
    {
    }

    private void IronSourceEventsOnonRewardedVideoAdEndedEvent()
    {
    }

    private void IronSourceEventsOnonRewardedVideoAdClickedEvent(IronSourcePlacement obj)
    {
    }

    #endregion

    #region Interstitial

    public bool IsInterstitialLoaded(string placement = null)
    {
        switch (_interstitialState)
        {
            case AdState.Failed:
            case AdState.None:
                _interstitialState = AdState.Loading;
                IronSource.Agent.loadInterstitial();
                return false;
            case AdState.Loading:
                return false;
            case AdState.Loaded:
                return string.IsNullOrEmpty(placement) || !IronSource.Agent.isInterstitialPlacementCapped(placement);
            default:
                return false;
        }
    }

    public void ShowInterstitial(string placement = null, Action<bool> actionCloseAd = null)
    {
        _adCallback = actionCloseAd;
        if (IsInterstitialLoaded(placement))
        {
            if (string.IsNullOrEmpty(placement))
            {
                IronSource.Agent.showInterstitial();
            }
            else
                IronSource.Agent.showInterstitial(placement);
        }
        else
        {
            _adCallback?.Invoke(false);
            _adCallback = null;
        }
    }

    private void RequestInterstitial()
    {
        _interstitialState = AdState.Loading;
        IronSource.Agent.loadInterstitial();
    }

    private void InterstitialAdLoadFailedEvent(IronSourceError error)
    {
        _interstitialState = AdState.Failed;
    }

    private void InterstitialAdReadyEvent()
    {
        _interstitialState = AdState.Loaded;
    }

    private void InterstitialAdShowSucceededEvent()
    {
    }

    private void InterstitialAdShowFailedEvent(IronSourceError error)
    {
        AppOpenAdManager.isJustPauseByMediation = false;
        _adCallback?.Invoke(false);
        _adCallback = null;
    }

    private void InterstitialAdClickedEvent()
    {
    }

    private void InterstitialAdClosedEvent()
    {
        RequestInterstitial();
        _adCallback?.Invoke(true);
        _adCallback = null;
    }

    private void InterstitialAdOpenedEvent()
    {
        AppsflyerConfig.LogEvent("af_inters_displayed");
    }

    #endregion

    #region Banner

    void LoadBanner()
    {
        #if UNITY_IOS|| UNITY_IPHONE
        IronSource.Agent.loadBanner(new IronSourceBannerSize(280, 50), IronSourceBannerPosition.BOTTOM);
        #else
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
        #endif
        
         //IronSourceBannerSize.BANNER.SetAdaptive(true);
    }

    void BannerAdLoadedEvent()
    {
        Debug.Log("Banner ad loaded!");
    }

    void BannerAdLoadFailedEvent(IronSourceError error)
    {
        Debug.Log("Banner ad failed to load. Error: " + error.getDescription());
    }

    void BannerAdClickedEvent()
    {
        
    }

    void BannerAdScreenPresentedEvent()
    {
        
    }

    void BannerAdScreenDismissedEvent()
    {
        _bannerCallback = null;
    }

    void BannerAdLeftApplicationEvent()
    {
        
    }

    #endregion
}
#endregion

#if UNITY_IOS || UNITY_IPHONE
#if UNITY_EDITOR 
public class IronSourcePlistAutoUpdate
{
    private static string xmlPath = "ThirdParties/Truongtv/AdsManager/SKAdNetworkId.xml";
    private const string KEY_SK_ADNETWORK_ITEMS = "SKAdNetworkItems";

    private const string KEY_SK_ADNETWORK_ID = "SKAdNetworkIdentifier";
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath)
    {   
        if(buildTarget!= BuildTarget.iOS) return;
        var plistPath = Path.Combine(buildPath, "Info.plist");
        PBXProject project = new PBXProject();
        var projectPath = PBXProject.GetPBXProjectPath(buildPath);
        project.ReadFromFile(projectPath);
        var plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
        if (plist != null)
        {
            PlistElementDict elementDict = plist.root;

            elementDict.SetString("NSUserTrackingUsageDescription", "This identifier will be used to deliver personalized ads to you.");
            elementDict.SetString("NSAdvertisingAttributionReportEndpoint", "https://postbacks-is.com");
            List<string> skNetworkIds = ReadSKAdNetworkIdentifiersFromXML();
            if (skNetworkIds.Count > 0)
            {
                AddSKAdNetworkIdentifier(plist, skNetworkIds);
            }

            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
    private static void AddSKAdNetworkIdentifier(PlistDocument document, List<string> skAdNetworkIds)
    {
        PlistElementArray array = GetSKAdNetworkItemsArray(document);
        if (array != null)
        {
            foreach (string id in skAdNetworkIds)
            {
                if (!ContainsSKAdNetworkIdentifier(array, id))
                {
                    PlistElementDict added = array.AddDict();
                    added.SetString(KEY_SK_ADNETWORK_ID, id);
                }
            }
        }
        else
        {
            NotifyBuildFailure("SKAdNetworkItems element already exists in Info.plist, but is not an array.");
        }
    }
    private static PlistElementArray GetSKAdNetworkItemsArray(PlistDocument document)
    {
        PlistElementArray array;
        if (document.root.values.ContainsKey(KEY_SK_ADNETWORK_ITEMS))
        {
            try
            {
                PlistElement element;
                document.root.values.TryGetValue(KEY_SK_ADNETWORK_ITEMS, out element);
                array = element.AsArray();
            }
#pragma warning disable 0168
            catch (Exception e)
#pragma warning restore 0168
            {
                // The element is not an array type.
                array = null;
            }
        }
        else
        {
            array = document.root.CreateArray(KEY_SK_ADNETWORK_ITEMS);
        }
        return array;
    }
    private static bool ContainsSKAdNetworkIdentifier(PlistElementArray skAdNetworkItemsArray, string id)
    {
        foreach (PlistElement elem in skAdNetworkItemsArray.values)
        {
            try
            {
                PlistElementDict elemInDict = elem.AsDict();
                PlistElement value;
                bool identifierExists = elemInDict.values.TryGetValue(KEY_SK_ADNETWORK_ID, out value);

                if (identifierExists && value.AsString().Equals(id))
                {
                    return true;
                }
            }
#pragma warning disable 0168
            catch (Exception e)
#pragma warning restore 0168
            {
                // Do nothing
            }
        }

        return false;
    }
    private static List<string> ReadSKAdNetworkIdentifiersFromXML()
    {
        List<string> skAdNetworkItems = new List<string>();

        string path = Path.Combine(Application.dataPath, xmlPath);
        try
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }
            using (FileStream fs = File.OpenRead(path))
            {
                XmlDocument document = new XmlDocument();
                document.Load(fs);

                XmlNode root = document.FirstChild;

                XmlNodeList nodes = root.SelectNodes("dict");
                foreach (XmlNode node in nodes)
                {
                    var dat = node.SelectSingleNode("string");
                    skAdNetworkItems.Add(dat.InnerText);
                }
            }
        }
#pragma warning disable 0168
        catch (FileNotFoundException e)
#pragma warning restore 0168
        {
            NotifyBuildFailure("ThirdParties/Truongtv/AdsManager/SKAdNetworkId.xml not found");
        }
        catch (IOException e)
        {
            NotifyBuildFailure("Failed to read ThirdParties/Truongtv/AdsManager/SKAdNetworkId.xml: " + e.Message);
        }

        return skAdNetworkItems;
    }
    private static void NotifyBuildFailure(string message)
    {
        string dialogTitle = "Truongtv_IR";
        string dialogMessage = "Error: " + message;

        EditorUtility.DisplayDialog(dialogTitle, dialogMessage, "Close");

        ThrowBuildException("[Truongtv_IR] " + message);
    }
    private static void ThrowBuildException(string message)
    {
#if UNITY_2017_1_OR_NEWER
        throw new BuildPlayerWindow.BuildMethodException(message);
#else
        throw new OperationCanceledException(message);
#endif
    }
}
#endif
#endif
#endif