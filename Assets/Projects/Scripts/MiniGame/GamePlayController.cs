using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv;
using ThirdParties.Truongtv.SoundManager;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGame
{
    public class GamePlayController : MonoBehaviour
    {
        [FoldoutGroup("Character")] public PlayerController player;
        [SerializeField, FoldoutGroup("UI")] protected Button pauseButton;
        public static GamePlayController Instance => _instance;
        public GameState state = GameState.None;
        private static GamePlayController _instance;
        protected RankIngame playerInfo;

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }
            _instance = this;
            if (pauseButton) 
            {
                pauseButton.onClick.AddListener(() =>
                {
                    SoundManager.Instance.PlayButtonSound();
                    // PopupInGameController.Instance.OpenPopupPause();
                });
            }
        }

        protected virtual void Start()
        {
            
        }
        
        public void StartGame(string game = null, string difficulty = "")
        {
            state = GameState.Playing;
            GameServiceManager.LogEvent(GameServiceManager.eventConfig.levelStart, new Dictionary<string, object> {{game,difficulty}});
        }

        public virtual async void Respawn(Vector3 position)
        {
            
        }
        
        public virtual RankIngame GetPlayerInfo()
        {
            return playerInfo;
        }

        public virtual void Win()
        {
            
        }
        
        #region Controller
        
        #region Button

        public void TouchMoveLeft(bool release)
        {
            if (player != null && state != GameState.Playing && !player.IsDie()) return;
            if (!player.jetpackMode)
            {
                player.MoveLeft(release);
            }
            else
            {
                player.JetpackLeft(release);
            }
        }
        
        public void TouchMoveRight(bool release)
        {
            if (player != null && state != GameState.Playing && !player.IsDie()) return;
            if (!player.jetpackMode)
            {
                player.MoveRight(release);
            }
            else
            {
                player.JetpackRight(release);
            }
        }

        public void TouchJump(bool release)
        {
            if (player != null && state != GameState.Playing && !player.IsDie()) return;
            player.Jump(release);
        }


        #endregion

        #region JoyStick

        

        #endregion
        
        #endregion
    }
}