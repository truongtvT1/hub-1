using System;
using System.Collections.Generic;

namespace Projects.Scripts.Data
{
    [Serializable]
    public class UserInfo
    {
        public Dictionary<string, int> currencies;
        public Dictionary<string, string> unlockedSkins;
        public Dictionary<string, string> currentSkin;
        public Dictionary<string, int> difficult;
    }
}