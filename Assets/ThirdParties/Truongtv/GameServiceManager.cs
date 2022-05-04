using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv.AdsManager;
using ThirdParties.Truongtv.IAP;
using ThirdParties.Truongtv.LogManager;
using ThirdParties.Truongtv.Notification;
using ThirdParties.Truongtv.Rating;
using ThirdParties.Truongtv.RemoteConfig;
using ThirdParties.Truongtv.Utilities;
using Truongtv.PopUpController;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using System.Xml;

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
        [SerializeField, OnValueChanged(nameof(OnAdServiceChange))]
        private AdService adService;

        [SerializeField, OnValueChanged(nameof(OnLogServiceChange))]
        private LogService logService;

        [SerializeField, OnValueChanged(nameof(OnRemoteServiceChange))]
        private RemoteConfigService remoteConfigService;

        [SerializeField, OnValueChanged(nameof(OnRateServiceChange))]
        private RatingService ratingService;

        [SerializeField, OnValueChanged(nameof(OnCloudMessagingServiceChange))]
        private CloudMessagingService cloudMessagingService;

        [SerializeField, OnValueChanged(nameof(OnIapServiceChange))]
        private IapService iapService;
        #endif
        private AdManager _adManager;
        private LogEventManager _logEventManager;
        private RatingHelper _ratingHelper;
        private RemoteConfigManager _remoteConfigManager;
        private MobileNotification _mobileNotification;
        private IapManager _iapManager;
        private static GameServiceManager _instance;
        
        private DateTime _lastTimeShowAd;
        #region public

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
            _instance.ShowRewardedAdPrivate(location,adResult);
        }
        #endregion

        #region Log Event
        public static void LogEvent(string eventName, Dictionary<string, object> parameters)
        {
            _instance.LogEventPrivate(eventName,parameters);
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
            _instance.PurchaseProductPrivate(sku,pAction);
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
                remoteConfigManager.Init();
                logEventManager.Init();
                mobileNotification.Init();
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

        public void HideBannerPrivate()
        {
            _adManager.HideBanner();
        }

        public void ShowInterstitialAdPrivate(
            Action adResult = null)
        {
            adResult?.Invoke();
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return;
            }

            if (GameDataManager.Instance.IsPurchaseBlockAd() ||GameDataManager.Instance.cheated)
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
        public void ShowRewardedAdPrivate(string location, Action adResult = null)
        {
            if (GameDataManager.Instance.cheated)
            {
                adResult?.Invoke();
                return;
            }
            LogEvent("ads_reward_click", new Dictionary<string, object>
            {
                {"reward_for", location}
            });
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PopupController.Instance.ShowNoInternet();
                LogEvent("ads_reward_fail", new Dictionary<string, object>
                {
                    {"cause", "no_internet"}
                });
                return;
            }
            if (!_adManager.IsRewardVideoLoaded())
            {
                PopupController.Instance.ShowToast("Ads is still coming. Please try again later.");
                LogEvent("ads_reward_fail", new Dictionary<string, object>
                {
                    {"cause", "no_fill"}
                });
                return;
            }
            _adManager.ShowRewardedAd(result =>
            {
                if (!result)
                {
                    LogEvent("ads_reward_fail", new Dictionary<string, object>
                    {
                        {"cause", "not_complete"}
                    });
                    return;
                }
                _lastTimeShowAd = DateTime.Now;
                LogEvent("ads_reward_complete", new Dictionary<string, object>
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
        private void OnAdServiceChange()
        {
            var symbolList = DefineSymbol.GetAllDefineSymbols();
            symbolList.Remove(DefineSymbol.MaxSymbol);
            symbolList.Remove(DefineSymbol.AdMobSymbol);
            symbolList.Remove(DefineSymbol.IronSourceSymbol);
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

            SaveProperties("adS_service", adService.ToString());
            DefineSymbol.UpdateDefineSymbols(symbolList);
        }

        private void OnLogServiceChange()
        {
            var symbolList = DefineSymbol.GetAllDefineSymbols();
            symbolList.Remove(DefineSymbol.LogUnity);
            symbolList.Remove(DefineSymbol.LogFirebase);
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

            SaveProperties("log", logService.ToString());
            DefineSymbol.UpdateDefineSymbols(symbolList);
        }

        private void OnRemoteServiceChange()
        {
            var symbolList = DefineSymbol.GetAllDefineSymbols();
            symbolList.Remove(DefineSymbol.RemoteFirebase);
            symbolList.Remove(DefineSymbol.RemoteUnity);
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

            SaveProperties("remote_config", remoteConfigService.ToString());
            DefineSymbol.UpdateDefineSymbols(symbolList);
        }

        private void OnRateServiceChange()
        {
            var symbolList = DefineSymbol.GetAllDefineSymbols();
            symbolList.Remove(DefineSymbol.InAppReview);
            if (ratingService == RatingService.InApp)
            {
                symbolList.Add(DefineSymbol.InAppReview);
            }

            SaveProperties("rate", ratingService.ToString());
            DefineSymbol.UpdateDefineSymbols(symbolList);
        }

        private void OnCloudMessagingServiceChange()
        {
            var symbolList = DefineSymbol.GetAllDefineSymbols();
            symbolList.Remove(DefineSymbol.InAppReview);
            if (cloudMessagingService == CloudMessagingService.Firebase)
            {
                symbolList.Add(DefineSymbol.FirebaseMessaging);
            }

            SaveProperties("cloud_message", cloudMessagingService.ToString());
            DefineSymbol.UpdateDefineSymbols(symbolList);
        }

        private void OnIapServiceChange()
        {
            var symbolList = DefineSymbol.GetAllDefineSymbols();
            symbolList.Remove(DefineSymbol.IAP);
            symbolList.Remove(DefineSymbol.UDP);
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

            SaveProperties("iap", iapService.ToString());
            DefineSymbol.UpdateDefineSymbols(symbolList);
        }

        private static void SaveProperties(string property, string value)
        {
            var XmlPath = "ThirdParties/Truongtv/CustomService.xml";
            var path = Path.Combine(Application.dataPath, XmlPath);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            var document = new XmlDocument();
            document.Load(path);
            var node = document.DocumentElement.SelectSingleNode(property);
            if (node == null)
            {
                node = document.CreateNode(XmlNodeType.Element, property, "");
                node.InnerText = value;
                document.DocumentElement.AppendChild(node);
            }

            node.InnerText = value;
            document.Save(path);
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