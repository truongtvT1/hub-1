using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    [Serializable]
    public class BotSkin
    {
        public Color color;
        public List<string> skin;

        public BotSkin()
        {
            
        }
        
        public BotSkin(Color color, List<string> skin)
        {
            this.color = color;
            this.skin = skin;
        }
    }
}