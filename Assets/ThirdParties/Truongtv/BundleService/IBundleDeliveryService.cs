using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdParties.Truongtv.BundleService
{
    public interface IBundleDeliveryService
    {
        void GetDownloadSize(string bundleName,Action<long> complete=null,Action failed = null);
        void UnloadBundle(string bundleName);
        void DownLoadBundle(string bundleName, Action<float> progressCallback=null,Action<AssetBundle> complete=null,Action failed = null);
        bool IsDownLoaded(string bundleName);
        void LoadLocalBundle(string bundleName,Action<long> complete=null,Action failed = null);
        
    }
}