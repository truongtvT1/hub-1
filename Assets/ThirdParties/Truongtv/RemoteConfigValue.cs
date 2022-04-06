using System;
using ThirdParties.Truongtv.RemoteConfig;

namespace ThirdParties.Truongtv
{
    [Serializable]
    public class RemoteConfigValue
    {
        public string versionReview;
        public int blockAdTime = 30;
        
        public void OnFetchComplete(RemoteConfigManager remoteConfigManager)
        {
            
        }
    }
}