using System;
using ThirdParties.Truongtv;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Menu
{
    public class MenuController : MonoBehaviour
    {
        
        public static MenuController Instance;
        private void Awake()
        {
            if(Instance!=null)
                Destroy(gameObject);
            Instance = this;
        }

        [SerializeField] private Button playButton, settingButton, switchButton, specialOfferButton, noAdButton, roomButton,userInfoButton,leaderBoardButton;

        private void Start()
        {
            playButton.onClick.AddListener(OnPlayButtonClick);
            settingButton.onClick.AddListener(OnSettingButtonClick);
            switchButton.onClick.AddListener(OnSwitchButtonClick);
            specialOfferButton.onClick.AddListener(OnSpecialOfferButtonClick);
            noAdButton.onClick.AddListener(OnNoAdButtonClick);
            roomButton.onClick.AddListener(OnRoomButtonClick);
            userInfoButton.onClick.AddListener(OnUserInfoButtonClick);
            leaderBoardButton.onClick.AddListener(OnLeaderBoardButtonClick);
            GameServiceManager.Instance.ShowBanner();
        }

        private void OnPlayButtonClick()
        {
            
        }

        private void OnSwitchButtonClick()
        {
            
        }

        private void OnSpecialOfferButtonClick()
        {
            
        }

        private void OnUserInfoButtonClick()
        {
            
        }

        private void OnNoAdButtonClick()
        {
            
        }

        private void OnSettingButtonClick()
        {
            PopupMenuController.Instance.ShowPopupSetting();
        }

        private void OnRoomButtonClick()
        {
            
        }
        private void OnLeaderBoardButtonClick()
        {
            PopupMenuController.Instance.ShowPopupLeaderBoard();
        }
    }
}
