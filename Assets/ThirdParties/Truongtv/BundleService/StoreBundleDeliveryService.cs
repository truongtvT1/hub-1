using System;
using System.Collections;
using Google.Play.AssetDelivery;
using System.Collections.Generic;
using System.Threading.Tasks;
using MEC;
#if UNITY_ANDROID
using Google.Play.AssetDelivery;
#endif
using UnityEngine;
using UnityEngine.iOS;

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

        public void GetDownloadSize(string bundleName, Action<long> complete = null, Action failed = null)
        {
#if UNITY_ANDROID
            Timing.RunCoroutine(GetDownLoadIEnumerator(),bundleName);
            IEnumerator<float> GetDownLoadIEnumerator()
            {
                var request = PlayAssetDelivery.GetDownloadSize(bundleName);
                yield return Timing.WaitUntilDone(request);
                if (request.IsSuccessful)
                {
                    complete?.Invoke(request.GetResult());
                    yield break;
                }
                Debug.Log(request.Error.ToString());
                failed?.Invoke();
            }
#endif
            
        }

        public void UnloadBundle(string bundleName)
        {
        }

        public void DownLoadBundle(string bundleName, Action<float> progressCallback = null, Action<AssetBundle> complete = null, Action failed = null)
        {
            IEnumerator DownLoadBundleIEnumerator()
            {
                PlayAssetBundleRequest bundleRequest =
                    PlayAssetDelivery.RetrieveAssetBundleAsync(bundleName);

                while (!bundleRequest.IsDone) {
                    if(bundleRequest.Status == AssetDeliveryStatus.WaitingForWifi) {
                        var userConfirmationOperation = PlayAssetDelivery.ShowCellularDataConfirmation();
                        yield return userConfirmationOperation;
                        if((userConfirmationOperation.Error != AssetDeliveryErrorCode.NoError) ||
                           (userConfirmationOperation.GetResult() != ConfirmationDialogResult.Accepted)) {
                            failed?.Invoke();
                            yield break;
                        }
                        yield return new WaitUntil(() => bundleRequest.Status != AssetDeliveryStatus.WaitingForWifi);
                    }

                    // Use bundleRequest.Status to track the status of request.
                    progressCallback?.Invoke(bundleRequest.DownloadProgress);
                    yield return null;
                }

                if (bundleRequest.Error != AssetDeliveryErrorCode.NoError) {
                    yield return null;
                }
            }
            
        }

        public IEnumerator<AssetBundle> DownLoadBundle(string bundleName, Action<float> progressCallback = null)
        {
            
            yield return null;
        }

        public void LoadLocalBundle(string bundleName, Action<long> complete = null, Action failed = null)
        {
            
        }
        public bool IsDownLoaded(string bundleName)
        {
#if UNTIY_ANDROID
           return PlayAssetDelivery.IsDownloaded(bundleName); 
#elif UNITY_IOS||UNITY_IPHONE
#endif
            return AssetBundle.LoadFromFile(bundleName) != null;
        }

    }
}