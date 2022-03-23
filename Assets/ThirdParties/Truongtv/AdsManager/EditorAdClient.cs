using System;
using Truongtv.Services.Ad;
using UnityEngine;

namespace ThirdParties.Truongtv.AdsManager
{
    public class EditorAdClient:IAdClient
    {
        public void Initialized()
        {
            Debug.Log("EditorAdClient Initialized");
        }

        public void OnApplicationPause(bool isPause)
        {
        }

        public bool IsRewardVideoLoaded()
        {
            return true;
        }

        public void ShowRewardVideo(Action<bool> actionFinishAd = null)
        {
            Debug.Log("EditorAdClient ShowRewardVideo");
            actionFinishAd?.Invoke(true);
        }

        public bool IsInterstitialLoaded()
        {
            return true;
        }

        public void ShowInterstitial(Action<bool> actionFinishAd = null)
        {
            Debug.Log("EditorAdClient ShowBannerAd");
            actionFinishAd?.Invoke(true);
        }

        public void ShowBannerAd(Action<bool> actionFinishAd = null)
        {
            Debug.Log("EditorAdClient ShowBannerAd");
            actionFinishAd?.Invoke(true);
        }

        public void HideBannerAd()
        {
            Debug.Log("EditorAdClient HideBannerAd");
        }
    }
}