using System;
using System.Collections.Generic;
using MiniGame;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Projects.Scripts.Data
{
    [Serializable]
    public class UserInfo
    {
        public Dictionary<string, DateTime> times;
        public Dictionary<string, int> currencies;
        public List<string> currentSkin,trySkin;
        public string currentSkinColor;
        public List<string> unlockedSkins;
        public List<string> unlockedMode;
        public UserRanking ranking;
        public List<UserRanking> fakeRankingData;
        public string lastPlayed;
        public bool purchased, firstOpen;
        public UserInfo()
        {
            times = new Dictionary<string, DateTime> {{"date_create", DateTime.Now}};
            currencies = new Dictionary<string, int>();
            unlockedMode = new List<string>();
            ranking = new UserRanking();
            fakeRankingData = new List<UserRanking>();
            firstOpen = true;
        }

        public void InitSkin(List<string> currentSkinList,Color color)
        {
            currentSkin = new List<string>(currentSkinList);
            unlockedSkins = new List<string>(currentSkinList);
            currentSkinColor = ColorUtility.ToHtmlStringRGB(color);
            trySkin = new List<string>();
        }
        
    }
}