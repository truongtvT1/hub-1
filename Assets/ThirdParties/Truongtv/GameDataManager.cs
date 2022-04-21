using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Projects.Scripts.Data;
using Projects.Scripts.Scriptable;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv.RemoteConfig;
using UnityEngine;
using Random = UnityEngine.Random;

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
            if (IsUserCreate())
            {
                LoadUserInfo();
            }
            else
            {
                CreateUserInfo();
            }
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
        public void LoadUserInfo()
        {
            _userInfo = ES3.Load<UserInfo>("user_info");
        }
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
            return new List<string>(_userInfo.currentSkin);
        }

        private string RandomSkin(List<SkinInfo> skinList)
        {
            var count = skinList.Count;
            return skinList[Random.Range(0, count)].skinName;
        }

        public List<string> RandomSkinList()
        {
            var list = new List<string>();
            list.Add(RandomSkin(skinData.bodySkins));
            list.Add(RandomSkin(skinData.cloakSkins));
            list.Add(RandomSkin(skinData.glassSkins));
            list.Add(RandomSkin(skinData.gloveSkins));
            list.Add(RandomSkin(skinData.hairSkins));
            return list;
        }


        public Color RandomColor()
        {
            var count = skinData.skinColors.Count;
            return skinData.skinColors[Random.Range(0, count)];
        }
        public List<string> GetSkinInGame()
        {
            var list =GetCurrentSkin();
            foreach (var item in _userInfo.trySkin)
            {
                list = UpdateSkinForList(list, item);
            }

            for (int i = 0; i < list.Count; i++)
            {
                Debug.Log("skin " + list[i]);
            }
            return list;
        }

        public void TrySkin(string skinName)
        {
            _userInfo.trySkin = UpdateSkinForList(_userInfo.trySkin, skinName);
            SaveUserInfo();
        }
        public void ResetSkinInGame()
        {
            _userInfo.trySkin = new List<string>();
            SaveUserInfo();
        }

        public void UpdateCurrentSkin(string skins)
        {
            _userInfo.currentSkin = UpdateSkinForList(_userInfo.currentSkin,skins);
            SaveUserInfo();
        }

        public List<string> UpdateSkinForList(List<string> skinList, string changeSkin)
        {
            var prefix = GetSkinPrefix(changeSkin);
            var remove = skinList.Find(a => a.Contains(prefix));
            skinList.Remove(remove);
            skinList.Add(changeSkin);
            return skinList;
        }
        public Color GetCurrentColor()
        {
            ColorUtility.TryParseHtmlString("#"+_userInfo.currentSkinColor, out var result);
            return result;
        }

        public void SetSkinColor(Color c)
        {
            _userInfo.currentSkinColor = ColorUtility.ToHtmlStringRGB(c);
            SaveUserInfo();
        }

        public bool IsSkinUnlock(string skin)
        {
            return _userInfo.unlockedSkins.Contains(skin);
        }

        public void UnlockSkin(string skin)
        {
            if (!_userInfo.unlockedSkins.Contains(skin))
            {
                _userInfo.unlockedSkins.Add(skin);
                SaveUserInfo();
            }
        }

        public int GetSkinPriceByRank(SkinRank rank)
        {
            switch (rank)
            {
                case SkinRank.SS:
                    return -1;
                case SkinRank.S:
                    return skinData.sTierTicket;
                case SkinRank.A:
                    return skinData.aTierTicket;
                case SkinRank.B:
                    return skinData.bTierTicket;
                case SkinRank.C:
                    return skinData.cTierTicket;
            }
            return 0;
        }

        public int GetColorPrice()
        {
            return skinData.colorTicket;
        }
        public string GetSkinPrefix(string skin)
        {
            if (skinData.bodySkins.Exists(a => a.skinName.Equals(skin)))
                return SkinData.SuitPrefix;
            if (skinData.hairSkins.Exists(a => a.skinName.Equals(skin)))
                return SkinData.HairPrefix;
            if (skinData.glassSkins.Exists(a => a.skinName.Equals(skin)))
                return SkinData.GlassPrefix;
            if (skinData.gloveSkins.Exists(a => a.skinName.Equals(skin)))
                return SkinData.GlovePrefix;
            if (skinData.cloakSkins.Exists(a => a.skinName.Equals(skin)))
                return SkinData.CloakPrefix;
            return string.Empty;
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