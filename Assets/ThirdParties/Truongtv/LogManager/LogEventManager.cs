using System.Collections.Generic;
using UnityEngine;

namespace ThirdParties.Truongtv.LogManager
{
    public class LogEventManager : MonoBehaviour
    {
        private ILogService _logService;
        public void Init()
        {
            #if USING_LOG_FIREBASE
            _logService = new FirebaseLogService();
            #else
            _logService = new EditorLogService();
            #endif
            _logService.Init();
        }
        public void LogEvent(string eventName, Dictionary<string,object> parameters)
        {
            _logService.LogEvent(eventName,parameters);
        }
        public void LogEvent(string eventName)
        {
            _logService.LogEvent(eventName);
        }
        public void SetUserProperties(string userProperty,string value)
        {
            _logService.LogUserProperties(userProperty,value);
        }
    }
    
   
}
