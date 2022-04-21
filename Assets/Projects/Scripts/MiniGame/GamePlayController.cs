using System;
using System.Collections.Generic;
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
        [FoldoutGroup("Character")] public PlayerController playerPrefabs;
        [SerializeField, FoldoutGroup("UI")] private Button pauseButton;
        private static GamePlayController _instance;
        public static GamePlayController Instance => _instance;
        public GameState state = GameState.None;
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }

            _instance = this;
        }

        private void Start()
        {
            if (pauseButton) 
            {
                pauseButton.onClick.AddListener(() =>
                {
                    SoundManager.Instance.PlayButtonSound();
                    // PopupInGameController.Instance.OpenPopupPause();
                });
            }
        }
        
        public void StartGame(string game = null, int level = 0)
        {
            state = GameState.Playing;
            // GameServiceManager.Instance.LogEvent("level_start", new Dictionary<string, object> {{game, "lv_" + level}});
        }

        public void Respawn(Vector3 position)
        {
            var player = Instantiate(playerPrefabs);
            player.transform.position = position;
            player.Init();
            this.player = player;
        }
        
        #region Controller
        
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
    }
}