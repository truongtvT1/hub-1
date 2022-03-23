using System.Collections.Generic;
using UnityEngine;

namespace ThirdParties.Truongtv.LogManager
{
    public class EditorLogService:ILogService
    {
        public void Init()
        {
        }

        public void LogEvent(string eventName)
        {
            Debug.Log("EditorLogService: event "+eventName);
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameter)
        {
            foreach (var keyValue in parameter)
            {
                Debug.Log("EditorLogService: event "+eventName+"| key: "+keyValue.Key+" |value: "+keyValue.Value);
            }
        }

        public void LogUserProperties(string property, string value)
        {
            Debug.Log("EditorLogService: UserProperties "+property+" | value: "+value);

        }
    }
}