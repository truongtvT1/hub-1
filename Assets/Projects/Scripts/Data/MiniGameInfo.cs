using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Projects.Scripts.Data
{
    [Serializable]
    public class MiniGameInfo
    {
        public string gameId;
        public string name;
        public Sprite bg,loadingBg;
        public Object gameScene;
        [HideInInspector] public bool mostPlay,recentPlay;
        [HideInInspector] public int total, win, lose;
    }
}