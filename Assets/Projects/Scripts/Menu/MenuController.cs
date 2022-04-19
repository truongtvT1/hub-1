using System;
using System.Collections.Generic;
using DG.Tweening;
using Projects.Scripts.Hub;
using ThirdParties.Truongtv;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Menu
{
    public class MenuController : MonoBehaviour
    {
        public static MenuController Instance;

        private void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            Instance = this;
        }

        [SerializeField] private Button playButton,
            settingButton,
            switchButton,
            specialOfferButton,
            noAdButton,
            roomButton,
            userInfoButton,
            leaderBoardButton;

        [SerializeField] private CharacterAnimationGraphic mainCharacter;
        [SerializeField] private ParticleGold ticketEffect;
        [SerializeField] private TextMeshProUGUI ticketText;

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
            mainCharacter.SetSkin(GameDataManager.Instance.GetSkinInGame());
            mainCharacter.SetSkinColor(GameDataManager.Instance.GetCurrentColor());
            Debug.Log(GameDataManager.Instance.GetCurrentColor());
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
            mainCharacter.SetSkin(GameDataManager.Instance.GetSkinInGame());
            mainCharacter.SetSkinColor(GameDataManager.Instance.GetCurrentColor());
        }

        public void AddTicket(int value)
        {
        }

        public void UseTicket(int value)
        {
            var current = GameDataManager.Instance.GetTotalTicket();
            var last = current - value;
            DOTween.To(() => current, x => current = x, last, 0.5f).SetEase(Ease.Linear)
                .OnUpdate(() => { ticketText.text = $"{current}"; })
                .OnComplete(() => { ticketText.text = $"{last}"; });
            GameDataManager.Instance.UpdateTicket(-value);
        }
    }
}