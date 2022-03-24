using System;
using System.Collections.Generic;
using Truongtv.PopUpController;
using Truongtv.Services.Ad;
using UnityEngine;

namespace ThirdParties.Truongtv.AdsManager
{
    public class AdManager:MonoBehaviour
    {
        private IAdClient _adClient;
        private DateTime _lastTimeInterstitialShow;
        private int _countLevel;
        #region Unity Function
        public void Init()
        {
            #if USING_MAX
            _adClient = new MAXAdClient();
            #elif USING_ADMOB
            _adClient = new AdMobClient();
            #elif USING_IRON_SOURCE
            _adClient = new AdMobClient();
            #else
            _adClient = new EditorAdClient();
            #endif
            _adClient.Initialized();
        }
        #endregion
        
        #region Private Function

        private void ShowRewardVideo(string placement = null, Action<bool> actionCloseAd=null)
        {
            
            _adClient.ShowRewardVideo((result) =>
            {
                if(result)
                    _lastTimeInterstitialShow = DateTime.Now;
                actionCloseAd?.Invoke(result);
                
            });
        }

        private bool IsRewardVideoLoaded()
        {
            return _adClient.IsRewardVideoLoaded();
        }

        private void ShowInterstitial(Action<bool> actionCloseAd=null)
        {
            
            _lastTimeInterstitialShow = DateTime.Now;
            _countLevel++;
            _adClient.ShowInterstitial(actionCloseAd);
        }

        private bool IsInterstitialLoaded()
        {
            return _adClient.IsInterstitialLoaded();
        }

        private bool IsInterstitialAvailableToShow()
        {
            if (DateTime.Now.Subtract(_lastTimeInterstitialShow).TotalSeconds < GameDataManager.BlockAdTime)
                return false;
            return true;
        }
        
        #endregion
        #region Public Function
        public void ShowBanner(Action<bool> result = null)
        {
#if UNITY_IOS|| UNITY_IPHONE
            if (GameDataManager.Instance.versionReview.Equals(Application.version))
            {
                result?.Invoke(false);
                return;
            }
#endif
            if (GameDataManager.Instance.IsPurchaseBlockAd()||GameDataManager.Instance.cheated)
            {
                result?.Invoke(false);
                return;
            }
            _adClient.ShowBannerAd(result);
        }

        public void HideBanner()
        {
            _adClient.HideBannerAd();
        }
        public void ShowInterstitialAd(
            Action adResult = null)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                adResult?.Invoke();
                return;
            }
            if (GameDataManager.Instance.cheated)
            {
                adResult?.Invoke();
                return;
            }
            if (IsInterstitialLoaded() && IsInterstitialAvailableToShow())
            {
                ShowInterstitial(result =>
                {
                    GameServiceManager.Instance.logEventManager.LogEvent("ads_interstitial");
                    adResult?.Invoke();
                });
            }
            else
            {
                adResult?.Invoke();
            }
        }
        
        public void ShowRewardedAd(string location, Action adResult = null)
        {
            
            if (GameDataManager.Instance.cheated)
            {
                adResult?.Invoke();
                return;
            }
            GameServiceManager.Instance.logEventManager.LogEvent("ads_reward_click",new Dictionary<string, object>
            {
                {"reward_for",location}
            });
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PopupController.Instance.ShowNoInternet();
                GameServiceManager.Instance.logEventManager.LogEvent("ads_reward_fail",new Dictionary<string, object>
                {
                    {"cause","no_internet"}
                });
                _countLevel++;
                return;
            }
            if (!IsRewardVideoLoaded())
            {
                PopupController.Instance.ShowToast("Ads is still coming. Please try again later.");
                GameServiceManager.Instance.logEventManager.LogEvent("ads_reward_fail",new Dictionary<string, object>
                {
                    {"cause","no_ad"}
                });
                return;
            }
            ShowRewardVideo(location,result =>
            {
                if (!result)
                {
                    GameServiceManager.Instance.logEventManager.LogEvent("ads_reward_fail",new Dictionary<string, object>
                    {
                        {"cause","not_complete"}
                    });
                    return;
                }
                adResult?.Invoke();
                GameServiceManager.Instance.logEventManager.LogEvent("ads_reward_complete");
            });
        }
        #endregion

       
    }

    
}
