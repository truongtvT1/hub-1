using System;

namespace Projects.Scripts.Scriptable
{
    [Serializable]
    public class SkinInfo
    {
        public string skinName;
        public SkinRank rank;
            
    }
    

    public enum SkinRank
    {
        S,A,B,C
    }

}