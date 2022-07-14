#if USING_ADMOB
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.Mediation.AppLovin;
using GoogleMobileAds.Common;
using Truongtv.Services.Ad;
#if UNITY_IOS|| UNITY_IPHONE
using System.Runtime.InteropServices;
using Unity.Advertisement.IosSupport;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#endif
#endif
namespace ThirdParties.Truongtv.AdsManager
{
    public class AdMobClient : IAdClient
    {
        private const string InterstitialAdAndroidId = "ca-app-pub-9179752697212712/9036393906";
        private const string InterstitialAdIosId = "ca-app-pub-9179752697212712/5229559578";
        private const string RewardedAdAndroidId = "ca-app-pub-9179752697212712/1673457946";
        private const string RewardedAdIosId = "ca-app-pub-9179752697212712/2603396235";
        private const string BannerAdAndroidId = "ca-app-pub-9179752697212712/6170272006";
        private const string BannerAdIosId = "ca-app-pub-9179752697212712/6204015307";
        private InterstitialAd _interstitialAd;
        private RewardedAd _rewardedAd;
        private BannerView bannerView;

        private Action<bool> _adCallback;
        private Action<bool> _bannerCallback;

        #region public function

        public void Initialized()
        {
            MobileAds.SetiOSAppPauseOnBackground(true);

            #region AppLovin Mediation

            AppLovin.SetHasUserConsent(true);
            AppLovin.SetIsAgeRestrictedUser(true);
            AppLovin.Initialize();

            #endregion



            RequestConfiguration requestConfiguration =
                new RequestConfiguration.Builder()
                    .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.False)
                    .build();

            MobileAds.SetRequestConfiguration(requestConfiguration);
            MobileAds.Initialize(HandleInitCompleteAction);
#if UNITY_IPHONE || UNITY_IOS
            var trackingStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

            if (trackingStatus == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                ATTrackingStatusBinding.RequestAuthorizationTracking();
            }
            #region Facebook Audience
            AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(false);
            #endregion
#endif
        }

        public void OnApplicationPause(bool isPause)
        {
        }

        public bool IsRewardVideoLoaded()
        {
            var result = _rewardedAd!= null && _rewardedAd.IsLoaded();
            if (!result)
            {
                RequestAndLoadRewardedAd();
            }

            return result;
        }

        public void ShowRewardVideo(Action<bool> actionFinishAd = null)
        {
            _adCallback = actionFinishAd;
            _rewardedAd.Show();
        }

        public bool IsInterstitialLoaded()
        {
            var result =  _rewardedAd!= null && _interstitialAd.IsLoaded();
            if (!result)
            {
                RequestAndLoadInterstitialAd();
            }
            return result;
        }

        public void ShowInterstitial(Action<bool> actionFinishAd = null)
        {
            _adCallback = actionFinishAd;
            _interstitialAd.Show();
        }

        public void ShowBannerAd(Action<bool> actionFinishAd = null)
        {
            RequestBanner();
            _bannerCallback = actionFinishAd;
        }

        public void HideBannerAd()
        {
            bannerView?.Hide();
        }

        #endregion

        #region private

        private void HandleInitCompleteAction(InitializationStatus initstatus)
        {
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                RequestAndLoadInterstitialAd();
                RequestAndLoadRewardedAd();
            });
        }

        #region HELPER METHODS

        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();
        }

        private string InterstitialAdId()
        {
#if UNITY_IOS|| UNITY_IPHONE
            return InterstitialAdIosId;
#else
            return InterstitialAdAndroidId;
#endif
        }

        private string RewardedAdId()
        {
#if UNITY_IOS|| UNITY_IPHONE
            return RewardedAdIosId;
#else
            return RewardedAdAndroidId;
#endif
        }

        private string BannerAdId()
        {
#if UNITY_IOS|| UNITY_IPHONE
            return BannerAdIosId;
#else
            return BannerAdAndroidId;
#endif
        }

        #endregion

        #region INTERSTITIAL ADS

        private void RequestAndLoadInterstitialAd()
        {
            _interstitialAd?.Destroy();
            _interstitialAd = new InterstitialAd(InterstitialAdId());
            _interstitialAd.OnAdClosed += OnInterstitialAdClosed;
            _interstitialAd.LoadAd(CreateAdRequest());
        }

        private void OnInterstitialAdClosed(object sender, EventArgs args)
        {
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                _adCallback?.Invoke(true);
                RequestAndLoadInterstitialAd();
            });
        }

        #endregion

        #region REWARDED ADS

        private void RequestAndLoadRewardedAd()
        {
            _rewardedAd = new RewardedAd(RewardedAdId());
            _rewardedAd.OnAdClosed += HandleRewardedAdClosed;
            _rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            _rewardedAd.LoadAd(CreateAdRequest());
        }

        private void HandleRewardedAdClosed(object sender, EventArgs args)
        {
            MobileAdsEventExecutor.ExecuteInUpdate(async () =>
            {
                await Task.Delay(400);
                _adCallback.Invoke(false);
                RequestAndLoadRewardedAd();
            });
        }

        private void HandleUserEarnedReward(object sender, Reward args)
        {
            MobileAdsEventExecutor.ExecuteInUpdate(() => { _adCallback.Invoke(true); });
        }

        #endregion

        #region BANNER ADS

        private void RequestBanner()
        {
            if (bannerView != null)
            {
                bannerView.Destroy();
            }
            var adaptiveSize =
                AdSize.GetLandscapeAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
#if !UNITY_EDITOR
            adaptiveSize = new AdSize(adaptiveSize.Width, 40);
#endif
            bannerView = new BannerView(BannerAdId(), adaptiveSize, AdPosition.Bottom);
            bannerView.OnAdLoaded += HandleOnAdLoaded;
            bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
            bannerView.LoadAd(CreateAdRequest());
        }

        private void HandleOnAdLoaded(object sender, EventArgs args)
        {
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                bannerView.Show();
                _bannerCallback?.Invoke(true);
                _bannerCallback = null;
            });
        }

        private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                _bannerCallback?.Invoke(false);
                _bannerCallback = null;
            });
        }

        #endregion

        #endregion
    }

#if UNITY_IOS || UNITY_IPHONE

#if UNITY_EDITOR
    public class BuildPostprocessorInfo
    {
        [PostProcessBuild]
        public static void OnPostBuildProcessInfo(BuildTarget target, string pathXcode)
        {
            if (target == BuildTarget.iOS)
            {
                var infoPlistPath = pathXcode + "/Info.plist";

                PlistDocument document = new PlistDocument();
                document.ReadFromString(File.ReadAllText(infoPlistPath));


                PlistElementDict elementDict = document.root;

                elementDict.SetString("NSUserTrackingUsageDescription", "This identifier will be used to deliver personalized ads to you.");

                File.WriteAllText(infoPlistPath, document.WriteToString());
            }
        }
    }
#endif
    namespace AudienceNetwork
    {
        public static class AdSettings
        {
            [DllImport("__Internal")]
            private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);

            public static void SetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled)
            {
                FBAdSettingsBridgeSetAdvertiserTrackingEnabled(advertiserTrackingEnabled);
            }
        }
    }
#endif
}
#endif