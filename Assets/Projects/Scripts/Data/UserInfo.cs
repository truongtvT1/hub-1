using System;
using System.Collections.Generic;

namespace Projects.Scripts.Data
{
    [Serializable]
    public class UserInfo
    {
        public Dictionary<string, DateTime> times;
        public Dictionary<string, int> currencies;
        public List<string> currentSkin;
        public Dictionary<string, float> difficult;
        public List<string> unlockedSkins;

        public UserInfo( )
        {
           
            times = new Dictionary<string, DateTime> {{"date_create", DateTime.Now}};
            currencies = new Dictionary<string, int>();
            difficult = new Dictionary<string, float>();
            
        }

        public void InitSkin(List<string> currentSkinList)
        {
            currentSkin = new List<string>(currentSkinList);
            unlockedSkins = new List<string>(currentSkinList);
        }
        
    }
}