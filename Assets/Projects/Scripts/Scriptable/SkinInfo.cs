using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine.Unity;

namespace Projects.Scripts.Scriptable
{
    [Serializable]
    public class SkinInfo
    {
        
        [ReadOnly]public string skinName;
        public SkinType skinType;
        public SkinRank rank;
        public SkinInfo(SkinType type,string name)
        {
            skinName = name;
            skinType = type;
        }
        
    }
    

    public enum SkinRank
    {
        SS,S,A,B,C
    }

    public enum SkinType
    {
        Hair,Glove,Suit,Glass,Cloak
    }
   

}