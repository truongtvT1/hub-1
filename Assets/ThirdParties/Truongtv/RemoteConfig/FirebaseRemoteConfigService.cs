#if USING_REMOTE_FIREBASE
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using UnityEngine;

namespace ThirdParties.Truongtv.RemoteConfig
{
    public class FirebaseRemoteConfigService : IRemoteConfigService
    {
        private RemoteConfigManager _remoteConfigManager;
        public void Init(RemoteConfigManager manager)
        {
            _remoteConfigManager = manager;
            InitRemoteConfig();
        }

        public void OnFetchComplete()
        {
            _remoteConfigManager.OnFetchComplete();
        }

        public int GetIntValue(string key)
        {
            if(!FirebaseRemoteConfig.DefaultInstance.AllValues.ContainsKey(key))
            {
                Debug.LogError("not found key "+key);
                return 0;
            }
            return (int)FirebaseRemoteConfig.DefaultInstance.AllValues[key].LongValue;
        }

        public bool GetBoolValue(string key)
        {
            if(!FirebaseRemoteConfig.DefaultInstance.AllValues.ContainsKey(key))
            {
                Debug.LogError("not found key "+key);
                return false;
            }
            return FirebaseRemoteConfig.DefaultInstance.AllValues[key].BooleanValue;
        }

        public string GetStringValue(string key)
        {
            if(!FirebaseRemoteConfig.DefaultInstance.AllValues.ContainsKey(key))
            {
                Debug.LogError("not found key "+key);
                return string.Empty;
            }
            return FirebaseRemoteConfig.DefaultInstance.AllValues[key].StringValue;
        }

        public float GetFloatValue(string key)
        {
            if(!FirebaseRemoteConfig.DefaultInstance.AllValues.ContainsKey(key))
            {
                Debug.LogError("not found key "+key);
                return 0;
            }
            return (float) FirebaseRemoteConfig.DefaultInstance.AllValues[key].DoubleValue;
        }

        private void InitRemoteConfig()
        {
            var defaults = new  Dictionary<string, object>();
            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
            var timeSpan = TimeSpan.Zero;
#if !UNITY_EDITOR
            timeSpan = TimeSpan.FromHours(12);
#endif
            FirebaseRemoteConfig.DefaultInstance.FetchAsync(timeSpan).ContinueWithOnMainThread(FetchComplete);
        }
        
        private void FetchComplete(Task fetchTask) {
            if (fetchTask.IsCanceled) {
                Debug.Log("Fetch canceled.");
            } else if (fetchTask.IsFaulted) {
                Debug.Log("Fetch encountered an error.");
            } else if (fetchTask.IsCompleted) {
                Debug.Log("Fetch completed successfully!");
            }
            var info = FirebaseRemoteConfig.DefaultInstance.Info;
            switch (info.LastFetchStatus) {
                case LastFetchStatus.Success:
                    FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                        .ContinueWithOnMainThread(task => {
                            Debug.Log($"Remote data loaded and ready (last fetch time {info.FetchTime}).");
                            OnFetchComplete();
                        });

                    break;
                case LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason) {
                        case FetchFailureReason.Error:
                            Debug.Log("Fetch failed for unknown reason");
                            break;
                        case FetchFailureReason.Throttled:
                            Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                            break;
                    }
                    break;
                case LastFetchStatus.Pending:
                    Debug.Log("Latest Fetch call still pending.");
                    break;
            }
        }
    }
}
#endif