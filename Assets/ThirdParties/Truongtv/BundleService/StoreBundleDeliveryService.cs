using System;
using Google.Play.AssetDelivery;
using System.Collections.Generic;
using System.Threading.Tasks;
#if UNITY_ANDROID
using Google.Play.AssetDelivery;
#endif
using UnityEngine;

namespace ThirdParties.Truongtv.BundleService
{
    public class StoreBundleDeliveryService : IBundleDeliveryService
    {
        public long GetDownloadSize(string bundleName)
        {
            // var request = PlayAssetDelivery.GetDownloadSize(bundleName);
            //  request.Completed+= result =>
            //  {
            //      
            //  }
             throw new NotImplementedException();
        }

        public void UnloadBundle(string bundleName)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<AssetBundle> DownLoadBundle(string bundleName, Action<float> progressCallback = null)
        {
            
            yield return null;
        }

        public bool IsDownLoaded(string bundleName)
        {
#if UNTIY_ANDROID
           return PlayAssetDelivery.IsDownloaded(bundleName); 
#elif UNITY_IOS||UNITY_IPHONE
#endif
            return true;
        }

    }
}