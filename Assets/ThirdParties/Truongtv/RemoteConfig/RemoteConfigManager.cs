using System;
using UnityEngine;

namespace ThirdParties.Truongtv.RemoteConfig
{
    public class RemoteConfigManager : MonoBehaviour
    {
        private IRemoteConfigService _iRemoteConfigService;
        public Action<RemoteConfigManager> fetchComplete;

        public void Init()
        {
#if USING_REMOTE_FIREBASE
            _iRemoteConfigService = new FirebaseRemoteConfigService();
#else
#endif
            _iRemoteConfigService?.Init(this);
        }
        public void OnFetchComplete()
        {
            fetchComplete.Invoke(this);
        }
        public int GetIntValue(string key)
        {
            return _iRemoteConfigService.GetIntValue(key);
        }
        public bool GetBoolValue(string key)
        {
            return _iRemoteConfigService.GetBoolValue(key);
        }
        public string GetStringValue(string key)
        {
            return _iRemoteConfigService.GetStringValue(key);
        }
        public float GetFloatValue(string key)
        {
            return _iRemoteConfigService.GetFloatValue(key);
        }
    }
}