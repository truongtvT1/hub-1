using System;

namespace Truongtv.Services.Ad
{
   public interface IAdClient
   {
      void Initialized();
      void OnApplicationPause(bool isPause);
      bool IsRewardVideoLoaded();
      void ShowRewardVideo(Action<bool> actionFinishAd=null);
      bool IsInterstitialLoaded();
      void ShowInterstitial(Action<bool> actionFinishAd = null);

      void ShowBannerAd(Action<bool> actionFinishAd = null);

      void HideBannerAd();
   }
}
