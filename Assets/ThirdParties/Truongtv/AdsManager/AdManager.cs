using System;
using Truongtv.Services.Ad;
using UnityEngine;

namespace ThirdParties.Truongtv.AdsManager
{
    public class AdManager:MonoBehaviour
    {
        private IAdClient _adClient;
        #region Public Function
        public void Init()
        {
            #if USING_MAX
            _adClient = new MAXAdClient();
            #elif USING_ADMOB
            _adClient = new AdMobClient();
            #elif USING_IRON_SOURCE
            _adClient = new IronSourceAdClient();
            #else
            _adClient = new EditorAdClient();
            #endif
            _adClient.Initialized();
        }
        
        public bool IsRewardVideoLoaded()
        {
            return _adClient.IsRewardVideoLoaded();
        }
        public bool IsInterstitialLoaded()
        {
            return _adClient.IsInterstitialLoaded();
        }
        public void ShowBanner()
        {
            _adClient.ShowBannerAd();
        }
        public void HideBanner()
        {
            _adClient.HideBannerAd();
        }
        public void ShowInterstitialAd(
            Action<bool> adResult = null)
        {
            _adClient.ShowInterstitial(adResult);
        }
        public void ShowRewardedAd( Action<bool> adResult = null)
        {
            _adClient.ShowRewardVideo(adResult);
        }
        #endregion

       
    }

    
}
