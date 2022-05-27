#if USING_LOG_FIREBASE

using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine;

namespace ThirdParties.Truongtv.LogManager
{
    public class FirebaseLogService : ILogService
    {
        public void Init()
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        }
       
        public void LogEvent(string eventName)
        {
            FirebaseAnalytics.LogEvent(eventName);
            Debug.Log("FirebaseLogService: event "+eventName);
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameter)
        {
            var paramList = new List<Parameter>();
            foreach (var keyValuePair in parameter)
            {
                if (long.TryParse(keyValuePair.Value.ToString(), out var resultLong))
                {
                    paramList.Add(new Parameter(keyValuePair.Key,resultLong));
                }
                else if (double.TryParse(keyValuePair.Value.ToString(), out var resultDouble))
                {
                    paramList.Add(new Parameter(keyValuePair.Key,Math.Round(resultDouble,2)));
                }
                else
                {
                    paramList.Add(new Parameter(keyValuePair.Key,keyValuePair.Value.ToString()));
                }
                Debug.Log("FirebaseLogService: event "+eventName+"| key: "+keyValuePair.Key+" |value: "+keyValuePair.Value);
            }
            FirebaseAnalytics.LogEvent(eventName,paramList.ToArray());
        }

        
        public void LogUserProperties(string property, string value)
        {
            FirebaseAnalytics.SetUserProperty(property,value);
        }
    }
}
#endif