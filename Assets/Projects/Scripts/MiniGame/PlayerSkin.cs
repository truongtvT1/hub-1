using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    [Serializable]
    public class PlayerSkin
    {
        public Color color;
        public List<string> skin;

        public PlayerSkin()
        {
            
        }
        
        public PlayerSkin(Color color, List<string> skin)
        {
            this.color = color;
            this.skin = skin;
        }
    }
}