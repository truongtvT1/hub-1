using System;
using System.Collections.Generic;
using Projects.Scripts.Hub;
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
        [SerializeField] private CharacterAnimationGraphic mainCharacter;
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
            mainCharacter.UpdateSkin(GameDataManager.Instance.GetCurrentSkin());
            mainCharacter.SetSkinColor(GameDataManager.Instance.GetSkinColor());
            Debug.Log(GameDataManager.Instance.GetSkinColor());
            GameServiceManager.Instance.ShowBanner();
        }

        #region Button Event

        private void OnPlayButtonClick()
        {
            
        }

        private void OnSwitchButtonClick()
        {
            PopupMenuController.Instance.ShowPopupCustomizeCharacter();
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
        
        

        #endregion

        public void UpdateCharacter()
        {
            mainCharacter.UpdateSkin(GameDataManager.Instance.GetCurrentSkin());
            mainCharacter.SetSkinColor(GameDataManager.Instance.GetSkinColor());
        }
    }
}
