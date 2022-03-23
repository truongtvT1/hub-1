namespace ThirdParties.Truongtv.RemoteConfig
{
    public interface IRemoteConfigService
    {
        void Init(RemoteConfigManager manager);
        void OnFetchComplete();

        int GetIntValue(string key);
        bool GetBoolValue(string key);
        string GetStringValue(string key);
        float GetFloatValue(string key);
    }
}