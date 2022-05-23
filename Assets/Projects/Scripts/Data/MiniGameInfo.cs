using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Projects.Scripts.Data
{
    [Serializable]
    public class Reward
    {
        public int ticket, trophy;
    }
    
    
    
    [Serializable]
    public class MiniGameInfo
    {
        public string gameId;
        public string name;
        public Sprite bg, loadingBg;
        public Reward rank1, rank2, rank3;
        [SerializeField]
        private int ticketToUnlock;
        [SerializeField, ShowIf("@this.scene == null")]
        public UnityEngine.Object scene;
        [ReadOnly] public string gameScene;
        [HideInInspector] public bool mostPlay, recentPlay;
        [HideInInspector] public int total, win, lose;
        
#if UNITY_EDITOR
        [Button]
        private void GetSceneName()
        {
            gameScene = scene.name;
        }
#endif
        
        
        
        
        public string GetScene
        {
            get { return gameScene; }
        }
    }
}