using System;
using System.Collections.Generic;
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

        [SerializeField, FoldoutGroup("Game Data")]
        public MiniGameData miniGameData;
        [SerializeField, FoldoutGroup("Game Data")]
        public ShopData shopData;
        [SerializeField, FoldoutGroup("Game Data")] public RemoteConfigValue remoteConfigValue;
        [SerializeField, FoldoutGroup("Start Data"),ValueDropdown(nameof(GetAllSkinName))] public List<string> startSkin;
        [SerializeField, FoldoutGroup("Start Data"),ValueDropdown(nameof(GetAllSkinColors))] public Color startColor;
        
        private static GameDataManager _instance;
        public static GameDataManager Instance => _instance;
        [SerializeField]private UserInfo _userInfo;
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

        public UserRanking GetUserRanking()
        {
            return _userInfo.ranking;
        }

        public string GetUserName()
        {
            return _userInfo.ranking.userName;
        }

        public void SetUserName(string name)
        {
            _userInfo.ranking.userName = name;
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
        private void SetCurrency(string name,int value)
        {
            if (!_userInfo.currencies.ContainsKey(name))
            {
                _userInfo.currencies.Add(name,0);
            }
            _userInfo.currencies[name] = value;
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
        private void SetTime(string key)
        {
            if (!_userInfo.times.ContainsKey(key))
            {
                _userInfo.times.Add(key,DateTime.Now);
                
            }
            _userInfo.times[key] = DateTime.Now;
            SaveUserInfo();
        }

        private DateTime GetTime(string key)
        {
            if (!_userInfo.times.ContainsKey(key))
            {
                _userInfo.times.Add(key,DateTime.MinValue);
                SaveUserInfo();
            }
            return _userInfo.times[key];
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

        #region Shop

        
        public int GetFreeChestCountInDay()
        {
            return GetCurrencyValue("free_chest");
        }

        public DateTime GetLastTimeClaimFreeChest()
        {
            return GetTime("free_chest");
        }

        public DateTime GetLastTimeClaimFreeTicket()
        {
            return GetTime("free_ticket");
        }
        public void UpdateFreeChestCountInDay(int value)
        {
            UpdateCurrency("free_chest", value);
            SetTime("free_chest");
        }

        public void ResetFreeChestCountInDay()
        {
            SetCurrency("free_chest", 0);
        }
        public void ResetFreeTicketCountInDay()
        {
            SetCurrency("free_ticket", 0);
        }
        public int GetFreeTicketCountInDay()
        {
            return GetCurrencyValue("free_ticket");
        }
        public void UpdateFreeTicketCountInDay(int value)
        {
            UpdateCurrency("free_ticket", value);
            SetTime("free_ticket");
        }

        public int GetTotalChestOpen()
        {
            return GetCurrencyValue("chest");
        }

        public void UpdateChestOpenNumber(int value)
        {
            UpdateCurrency("chest", value);
        }
        #endregion

        #region Mode Game

        public int GetMiniGameCountPlayed(string miniGame)
        {
            return GetCurrencyValue(miniGame+"_played");
        }
        public int GetMiniGameWinCount(string miniGame)
        {
            return GetCurrencyValue(miniGame+"_win");
        }
        public int GetMiniGameLoseCount(string miniGame)
        {
            return GetCurrencyValue(miniGame+"_lose");
        }
        public void UpdateMiniGameCountPlayed(string miniGame)
        {
            var count = GetCurrencyValue(miniGame + "_played") + 1;
            UpdateCurrency(miniGame,count);
        }
        public void UpdateMiniGameWinCount(string miniGame)
        {
            var count = GetCurrencyValue(miniGame + "_win") + 1;
            UpdateCurrency(miniGame,count);
        }
        public void UpdateMiniGameLoseCount(string miniGame)
        {
            var count = GetCurrencyValue(miniGame + "_lose") + 1;
            UpdateCurrency(miniGame,count);
        }

        public void UpdateLastPlayed(string miniGame)
        {
            _userInfo.lastPlayed = miniGame;
            SaveUserInfo();
        }

        public string GetLastPlayed()
        {
            return _userInfo.lastPlayed;
        }
        #endregion
    }
    
}