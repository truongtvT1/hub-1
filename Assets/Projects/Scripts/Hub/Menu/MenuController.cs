using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private static MenuController _instance;
        public static MenuController Instance => _instance;
        
        private void Awake()
        {
            if (_instance != null)
                Destroy(gameObject);
            _instance = this;
        }

        [SerializeField] private Button playButton,
            settingButton,
            switchButton,
            specialOfferButton,
            noAdButton,
            roomButton,
            userInfoButton,
            leaderBoardButton,
            shopButton;

        [SerializeField] private CharacterAnimationGraphic mainCharacter;
        [SerializeField] private ParticleGold ticketEffect;
        [SerializeField] private TextMeshProUGUI ticketText, userNameText, userTrophyText;
        [SerializeField] private CharacterAnimationGraphic userAvatar;

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
            shopButton.onClick.AddListener(OnShopButtonClick);
            mainCharacter.SetSkin(GameDataManager.Instance.GetSkinInGame());
            mainCharacter.SetSkinColor(GameDataManager.Instance.GetCurrentColor());
            ticketText.text = GameDataManager.Instance.GetTotalTicket() + "";
            userNameText.text = GameDataManager.Instance.GetUserName();
            userTrophyText.text = GameDataManager.Instance.GetTotalTrophy() + "";
            userAvatar.SetSkin(GameDataManager.Instance.GetCurrentSkin());
            userAvatar.SetSkinColor(GameDataManager.Instance.GetCurrentColor());
            if (GameDataManager.Instance.IsFirstOpen())
            {
                PopupMenuController.Instance.ShowPopupChooseMode();
            }
            if (GameDataManager.Instance.CheckCanUnlockNewMode() != null)
            {
                PopupMenuController.Instance.ShowPopupChooseMode();
                //TODO: tutorial for unlock new mode
            }
            GameServiceManager.ShowBanner();
        }

        #region Button Event

        private void OnPlayButtonClick()
        {
            PopupMenuController.Instance.ShowPopupChooseMode();
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
            PopupMenuController.Instance.ShowPopupUserInfo();
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

        private void OnShopButtonClick()
        {
            PopupMenuController.Instance.ShowPopupShop();
        }

        #endregion

        public void UpdatePlayerName()
        {
            userNameText.text = GameDataManager.Instance.GetUserName();
        }
        
        public async void UpdateCharacter()
        {
            mainCharacter.SetSkin(GameDataManager.Instance.GetSkinInGame());
            mainCharacter.SetSkinColor(GameDataManager.Instance.GetCurrentColor());
            userAvatar.Freeze(false);
            userAvatar.SetSkin(GameDataManager.Instance.GetCurrentSkin());
            userAvatar.SetSkinColor(GameDataManager.Instance.GetCurrentColor());
            await Task.Delay(100);
            userAvatar.Freeze();
        }

        public void AddTicket(int value)
        {
            ticketEffect.gameObject.SetActive(true);
            ticketEffect.Play();
            var current = GameDataManager.Instance.GetTotalTicket();
            var last = current + value;
            DOTween.To(() => current, x => current = x, last, 0.5f).SetEase(Ease.Linear)
                .OnUpdate(() => { ticketText.text = $"{current}"; })
                .OnComplete(() => { ticketText.text = $"{last}"; });
            GameDataManager.Instance.UpdateTicket(value);
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