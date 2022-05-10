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
        public Dictionary<string, GameDifficulty> difficult;
        public List<string> unlockedSkins;
        public UserRanking ranking;
        public string lastPlayed;
        public UserInfo()
        {
            times = new Dictionary<string, DateTime> {{"date_create", DateTime.Now}};
            currencies = new Dictionary<string, int>();
            difficult = new Dictionary<string, GameDifficulty>();
            ranking = new UserRanking();
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