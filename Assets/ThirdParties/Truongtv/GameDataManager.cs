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
        [SerializeField, FoldoutGroup("Start Data"),ValueDropdown(nameof(GetAllSkinName))] public List<string> startSkin;
        
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
            Debug.Log(JsonConvert.SerializeObject(_userInfo));
        }
        [Button]
        public void CreateUserInfo()
        {
            _userInfo = new UserInfo();
            _userInfo.InitSkin(startSkin);
            SaveUserInfo();
        }

        private void SaveUserInfo()
        {
            ES3.Save("user_info",_userInfo);
        }
        #endregion

        #region Currency

        public void UpdateCurrency()
        {
            
        }

        public int GetCurrencyValue(string name)
        {
            return 0;
        }

        #endregion

        #region Private

        public List<string> GetAllSkinName()
        {
            return skinData.GetAllSkinName();
        }

        #endregion
    }
    
}