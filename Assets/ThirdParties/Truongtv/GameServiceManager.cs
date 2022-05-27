using System;
using System.Collections.Generic;
#if USING_LOG_FIREBASE || USING_REMOTE_FIREBASE
using Firebase;
using Firebase.Extensions;
#endif
using Projects.Scripts.Data;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv.AdsManager;
using ThirdParties.Truongtv.IAP;
using ThirdParties.Truongtv.LogManager;
using ThirdParties.Truongtv.Notification;
using ThirdParties.Truongtv.Rating;
using ThirdParties.Truongtv.RemoteConfig;
#if UNITY_EDITOR
using ThirdParties.Truongtv.Utilities;
#endif
using Truongtv.PopUpController;
using Unity.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif
namespace ThirdParties.Truongtv
{
    [RequireComponent(typeof(AdManager))]
    [RequireComponent(typeof(LogEventManager))]
    [RequireComponent(typeof(RatingHelper))]
    [RequireComponent(typeof(RemoteConfigManager))]
    [RequireComponent(typeof(MobileNotification))]
    [RequireComponent(typeof(IapManager))]
    public class GameServiceManager : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private AdService adService;

        [SerializeField] private LogService logService;

        [SerializeField] private RemoteConfigService remoteConfigService;

        [SerializeField] private RatingService ratingService;

        [SerializeField] private CloudMessagingService cloudMessagingService;

        [SerializeField] private IapService iapService;

#endif
        [SerializeField] private LogEventConfig EventConfig;
        private AdManager _adManager;
        private LogEventManager _logEventManager;
        private RatingHelper _ratingHelper;
        private RemoteConfigManager _remoteConfigManager;
        private MobileNotification _mobileNotification;
        private IapManager _iapManager;
        private static GameServiceManager _instance;

        private DateTime _lastTimeShowAd;

        #region public

        public static LogEventConfig eventConfig;

        #region Ads Service

        public static void ShowBanner()
        {
            _instance.ShowBannerPrivate();
        }

        public static void HideBanner()
        {
            _instance.HideBannerPrivate();
        }

        public static void ShowInterstitialAd(Action adResult = null)
        {
            _instance.ShowInterstitialAdPrivate(adResult);
        }

        public static void ShowRewardedAd(string location, Action adResult = null)
        {
            _instance.ShowRewardedAdPrivate(location, adResult);
        }

        #endregion

        #region Log Event

        public static void LogEvent(string eventName, Dictionary<string, object> parameters)
        {
            _instance.LogEventPrivate(eventName, parameters);
        }

        public static void LogEvent(string eventName)
        {
            _instance.LogEventPrivate(eventName);
        }

        public static void SetUserProperties(string userProperty, string value)
        {
            _instance.SetUserPropertiesPrivate(userProperty, value);
        }

        #endregion

        #region Rate

        public static void Rate()
        {
            _instance.RatePrivate();
        }

        #endregion

        #region IAP

        public static void PurchaseProduct(string sku, Action<bool, string> pAction)
        {
            _instance.PurchaseProductPrivate(sku, pAction);
        }

        public static string GetItemLocalPriceString(string sku)
        {
            return _instance.GetItemLocalPriceStringPrivate(sku);
        }

        public static void RestorePurchase()
        {
            _instance.RestorePurchasePrivate();
        }

        #endregion

        #region Mobile Notification

        public void SetLuckySpinReminder()
        {
            _mobileNotification.SetLuckySpinReminder();
        }

        public void DailyRewardResetReminder(bool receive)
        {
            _mobileNotification.DailyRewardResetReminder(receive);
        }

        #endregion

        #endregion

        #region Private Function

        #region unity

        private void Awake()
        {
            if (_instance != null)
                Destroy(_instance.gameObject);
            _instance = this;
            if (Application.isPlaying)
                DontDestroyOnLoad(gameObject);
            eventConfig = EventConfig;
            _adManager = GetComponent<AdManager>();
            _logEventManager = GetComponent<LogEventManager>();
            _ratingHelper = GetComponent<RatingHelper>();
            _remoteConfigManager = GetComponent<RemoteConfigManager>();
            _mobileNotification = GetComponent<MobileNotification>();
            _iapManager = GetComponent<IapManager>();
        }

        private void Start()
        {
            _adManager.Init();
            OnFetchComplete(GameDataManager.Instance.remoteConfigValue.OnFetchComplete);
#if USING_LOG_FIREBASE||USING_REMOTE_FIREBASE
             FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available) {
                    _remoteConfigManager.Init();
                    _logEventManager.Init();
                    _mobileNotification.Init();
                } 
                else {
                    Debug.LogError(String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
#else
            _remoteConfigManager.Init();
            _logEventManager.Init();
            _mobileNotification.Init();
#endif
        }

        #endregion

        #region Log Event

        private void LogEventPrivate(string eventName, Dictionary<string, object> parameters)
        {
            _logEventManager.LogEvent(eventName, parameters);
        }

        private void LogEventPrivate(string eventName)
        {
            _logEventManager.LogEvent(eventName);
        }

        private void SetUserPropertiesPrivate(string userProperty, string value)
        {
            _logEventManager.SetUserProperties(userProperty, value);
        }

        #endregion

        #region Ad

        private void ShowBannerPrivate()
        {
#if UNITY_IOS|| UNITY_IPHONE
            if (GameDataManager.Instance.remoteConfigValue.versionReview.Equals(Application.version))
            {
                result?.Invoke(false);
                return;
            }
#endif
            if (GameDataManager.Instance.IsPurchaseBlockAd() || GameDataManager.Instance.cheated)
            {
                return;
            }

            _adManager.ShowBanner();
        }

        private void HideBannerPrivate()
        {
            _adManager.HideBanner();
        }

        private void ShowInterstitialAdPrivate(
            Action adResult = null)
        {
            adResult?.Invoke();
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return;
            }

            if (GameDataManager.Instance.IsPurchaseBlockAd() || GameDataManager.Instance.cheated)
            {
                return;
            }

            if (_adManager.IsInterstitialLoaded() && DateTime.Now.Subtract(_lastTimeShowAd).TotalSeconds <
                GameDataManager.Instance.remoteConfigValue.blockAdTime)
            {
                _adManager.ShowInterstitialAd(result =>
                {
                    if (result)
                        _lastTimeShowAd = DateTime.Now;
                });
                //show popup support us
                LogEvent("ads_interstitial");
            }
        }

        private void ShowRewardedAdPrivate(string location, Action adResult = null)
        {
            if (GameDataManager.Instance.cheated)
            {
                adResult?.Invoke();
                return;
            }

            LogEvent(eventConfig.rewardAdClick, new Dictionary<string, object>
            {
                {"reward_for", location}
            });
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PopupController.Instance.ShowToast(
                    "No internet connection. Make sure to turn on your Wifi/Mobile data and try again.");
                LogEvent(eventConfig.rewardAdFail, new Dictionary<string, object>
                {
                    {"cause", "no_internet"}
                });
                return;
            }

            if (!_adManager.IsRewardVideoLoaded())
            {
                PopupController.Instance.ShowToast("Ads is still coming. Please try again later.");
                LogEvent(eventConfig.rewardAdFail, new Dictionary<string, object>
                {
                    {"cause", "no_fill"}
                });
                return;
            }

            _adManager.ShowRewardedAd(result =>
            {
                if (!result)
                {
                    LogEvent(eventConfig.rewardAdFail, new Dictionary<string, object>
                    {
                        {"cause", "not_complete"}
                    });
                    return;
                }

                _lastTimeShowAd = DateTime.Now;
                LogEvent(eventConfig.rewardAdComplete, new Dictionary<string, object>
                {
                    {"reward_for", location}
                });
                adResult?.Invoke();
            });
        }

        #endregion

        #region IAP

        private void PurchaseProductPrivate(string sku, Action<bool, string> pAction)
        {
            _iapManager.PurchaseProduct(sku, pAction);
        }

        private string GetItemLocalPriceStringPrivate(string sku)
        {
            return _iapManager.GetItemLocalPriceString(sku);
        }

        private void RestorePurchasePrivate()
        {
            _iapManager.RestorePurchase();
        }

        #endregion

        #region Rate

        private void RatePrivate()
        {
            _ratingHelper.Rate();
        }

        #endregion

        #region Remote Config

        private void OnFetchComplete(Action<RemoteConfigManager> fetch)
        {
            _remoteConfigManager.fetchComplete += fetch;
        }

        #endregion

        #endregion

        #region Private Editor

#if UNITY_EDITOR
        [OnInspectorGUI]
        private void Space2()
        {
            GUILayout.Space(20);
        }

        [Button(ButtonSizes.Large), GUIColor(0, 1, 0), HideIf(nameof(IsServiceUpToDate))]
        [InitializeOnLoadMethod]
        private void UpdateService()
        {
            if (IsServiceUpToDate()) return;
            var symbolList = DefineSymbol.GetAllDefineSymbols();
            symbolList.Remove(DefineSymbol.MaxSymbol);
            symbolList.Remove(DefineSymbol.AdMobSymbol);
            symbolList.Remove(DefineSymbol.IronSourceSymbol);
            symbolList.Remove(DefineSymbol.LogUnity);
            symbolList.Remove(DefineSymbol.LogFirebase);
            symbolList.Remove(DefineSymbol.RemoteFirebase);
            symbolList.Remove(DefineSymbol.RemoteUnity);
            symbolList.Remove(DefineSymbol.InAppReview);
            symbolList.Remove(DefineSymbol.FirebaseMessaging);
            symbolList.Remove(DefineSymbol.IAP);
            symbolList.Remove(DefineSymbol.UDP);
            switch (adService)
            {
                case AdService.None:
                    break;
                case AdService.AdMob:
                    symbolList.Add(DefineSymbol.AdMobSymbol);
                    break;
                case AdService.IronSource:

                    symbolList.Add(DefineSymbol.IronSourceSymbol);
                    break;
                case AdService.Max:

                    symbolList.Add(DefineSymbol.MaxSymbol);
                    break;
            }

            switch (logService)
            {
                case LogService.None:
                    break;
                case LogService.Firebase:
                    symbolList.Add(DefineSymbol.LogFirebase);
                    break;
                case LogService.Unity:
                    symbolList.Add(DefineSymbol.LogUnity);
                    break;
            }

            switch (remoteConfigService)
            {
                case RemoteConfigService.None:
                    break;
                case RemoteConfigService.Firebase:
                    symbolList.Add(DefineSymbol.RemoteFirebase);
                    break;
                case RemoteConfigService.Unity:
                    symbolList.Add(DefineSymbol.RemoteUnity);
                    break;
            }

            if (ratingService == RatingService.InApp)
            {
                symbolList.Add(DefineSymbol.InAppReview);
            }

            if (cloudMessagingService == CloudMessagingService.Firebase)
            {
                symbolList.Add(DefineSymbol.FirebaseMessaging);
            }

            switch (iapService)
            {
                case IapService.None:
                    break;
                case IapService.IAP:
                    symbolList.Add(DefineSymbol.IAP);
                    break;
                case IapService.UDP:
                    symbolList.Add(DefineSymbol.UDP);
                    break;
            }

            DefineSymbol.UpdateDefineSymbols(symbolList);
        }

        private bool IsServiceUpToDate()
        {
            var symbolList = DefineSymbol.GetAllDefineSymbols();
            switch (adService)
            {
                case AdService.None:
                    if (symbolList.Contains(DefineSymbol.AdMobSymbol)) return false;
                    if (symbolList.Contains(DefineSymbol.IronSourceSymbol)) return false;
                    if (symbolList.Contains(DefineSymbol.MaxSymbol)) return false;
                    break;
                case AdService.AdMob:
                    if (!symbolList.Contains(DefineSymbol.AdMobSymbol)) return false;

                    break;
                case AdService.IronSource:
                    if (!symbolList.Contains(DefineSymbol.IronSourceSymbol)) return false;
                    break;
                case AdService.Max:
                    if (!symbolList.Contains(DefineSymbol.MaxSymbol)) return false;
                    break;
            }

            switch (logService)
            {
                case LogService.None:
                    if (symbolList.Contains(DefineSymbol.LogFirebase)) return false;
                    if (symbolList.Contains(DefineSymbol.LogUnity)) return false;
                    break;
                case LogService.Firebase:

                    if (!symbolList.Contains(DefineSymbol.LogFirebase)) return false;
                    break;
                case LogService.Unity:
                    if (!symbolList.Contains(DefineSymbol.LogUnity)) return false;
                    break;
            }

            switch (remoteConfigService)
            {
                case RemoteConfigService.None:
                    if (symbolList.Contains(DefineSymbol.RemoteFirebase)) return false;
                    if (symbolList.Contains(DefineSymbol.RemoteUnity)) return false;
                    break;
                case RemoteConfigService.Firebase:
                    if (!symbolList.Contains(DefineSymbol.RemoteFirebase)) return false;
                    break;
                case RemoteConfigService.Unity:
                    if (!symbolList.Contains(DefineSymbol.RemoteUnity)) return false;
                    break;
            }

            switch (ratingService)
            {
                case RatingService.OpenLink:
                    if (symbolList.Contains(DefineSymbol.InAppReview)) return false;
                    break;
                case RatingService.InApp:
                    if (!symbolList.Contains(DefineSymbol.InAppReview)) return false;
                    break;
            }

            switch (cloudMessagingService)
            {
                case CloudMessagingService.None:
                    if (symbolList.Contains(DefineSymbol.FirebaseMessaging)) return false;
                    break;
                case CloudMessagingService.Firebase:
                    if (!symbolList.Contains(DefineSymbol.FirebaseMessaging)) return false;
                    break;
            }

            switch (iapService)
            {
                case IapService.None:
                    if (symbolList.Contains(DefineSymbol.IAP)) return false;
                    if (symbolList.Contains(DefineSymbol.UDP)) return false;
                    break;
                case IapService.IAP:
                    if (!symbolList.Contains(DefineSymbol.IAP)) return false;
                    break;
                case IapService.UDP:
                    if (!symbolList.Contains(DefineSymbol.UDP)) return false;
                    break;
            }

            return true;
        }

#endif

        #endregion
    }

    enum LogService
    {
        None,
        Firebase,
        Unity
    }

    enum AdService
    {
        None,
        AdMob,
        IronSource,
        Max
    }

    enum RemoteConfigService
    {
        None,
        Firebase,
        Unity
    }

    enum RatingService
    {
        OpenLink,
        InApp
    }

    enum CloudMessagingService
    {
        None,
        Firebase
    }

    enum IapService
    {
        None,
        IAP,
        UDP
    }
}