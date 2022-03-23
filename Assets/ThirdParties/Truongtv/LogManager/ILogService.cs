using System.Collections.Generic;

namespace ThirdParties.Truongtv.LogManager
{
    public interface ILogService
    {
        void Init();
        void LogEvent(string eventName);
        void LogEvent(string eventName, Dictionary<string, object> parameter);
        void LogUserProperties(string property, string value);
    }
}
