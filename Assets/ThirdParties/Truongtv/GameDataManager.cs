using System.Collections.Generic;
using Newtonsoft.Json;
using Projects.Scripts.Data;
using Projects.Scripts.Scriptable;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv.RemoteConfig;
using UnityEngine;

namespace ThirdParties.Truongtv
{
   
    public class GameDataManager : SerializedMonoBehaviour
    {
        [SerializeField,FoldoutGroup("Game Setting")] public bool cheated = false;
        [SerializeField, FoldoutGroup("Game Setting")] public bool debugger = false;
        [SerializeField, FoldoutGroup("Game Data")] public SkinData skinData;
        [SerializeField, FoldoutGroup("Game Data")] public RemoteConfigValue remoteConfigValue;
        [SerializeField, FoldoutGroup("Start Data"),ValueDropdown(nameof(GetAllSkinName))] private List<string> startSkin;
        [SerializeField, FoldoutGroup("Start Data"),ValueDropdown(nameof(GetAllSkinColors))] public Color startColor;
        private static GameDataManager _instance;
        public static GameDataManager Instance => _instance;
        private UserInfo _userInfo;
        private void Awake()
        {
            if (_instance != null)
                Destroy(_instance.gameObject);
            _instance = this ;
        }
        private void Start()
        {
            Application.targetFrameRate = 300;
        }
        public bool IsPurchaseBlockAd()
        {
            return false;
        }
        #region UserInfo

        public bool IsUserCreate()
        {
            return ES3.KeyExists("user_info");
        }
        [Button]
        public void LoadUserInfo()
        {
            _userInfo = ES3.Load<UserInfo>("user_info");
        }
        [Button]
        public void CreateUserInfo()
        {
            _userInfo = new UserInfo();
            _userInfo.InitSkin(startSkin,startColor);
            SaveUserInfo();
        }

        private void SaveUserInfo()
        {
            ES3.Save("user_info",_userInfo);
        }
        #endregion

        #region Currency

        private void UpdateCurrency(string name,int value)
        {
            if (!_userInfo.currencies.ContainsKey(name))
            {
                _userInfo.currencies.Add(name,0);
            }
            _userInfo.currencies[name] += value;
            SaveUserInfo();
        }

        private int GetCurrencyValue(string name)
        {
            if (!_userInfo.currencies.ContainsKey(name))
            {
                _userInfo.currencies.Add(name,0);
                SaveUserInfo();
            }
            return _userInfo.currencies[name];
        }

        public int GetTotalTicket()
        {
            return GetCurrencyValue("ticket");
        }

        public void UpdateTicket(int value)
        {
            UpdateCurrency("ticket", value);
        }
        #endregion

        #region Skin

        public List<string> GetCurrentSkin()
        {
            return _userInfo.currentSkin;
        }

        public void UpdateCurrentSkin(List<string> skins)
        {
            _userInfo.currentSkin = skins;
            SaveUserInfo();
        }
        public Color GetSkinColor()
        {
            ColorUtility.TryParseHtmlString("#"+_userInfo.currentSkinColor, out var result);
            return result;
        }

        public void SetSkinColor(Color c)
        {
            _userInfo.currentSkinColor = ColorUtility.ToHtmlStringRGB(c);
            SaveUserInfo();
        }
        #endregion

        #region Private

        private List<string> GetAllSkinName()
        {
            return skinData.GetAllSkinName();
        }

        public List<Color> GetAllSkinColors()
        {
            return skinData.skinColors;
        }
        #endregion
    }
    
}