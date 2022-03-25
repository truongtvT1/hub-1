using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdParties.Truongtv.BundleService
{
    public interface IBundleDeliveryService
    {
        long GetDownloadSize(string bundleName);
        void UnloadBundle(string bundleName);
        IEnumerator<AssetBundle> DownLoadBundle(string bundleName, Action<float> progressCallback=null);
        bool IsDownLoaded(string bundleName);
    }
}