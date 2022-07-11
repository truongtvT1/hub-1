using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Projects.Scripts.Data;
using Projects.Scripts.Hub;
using Projects.Scripts.Hub.Component;
using Projects.Scripts.Popup;
using Sirenix.Utilities;
using ThirdParties.Truongtv;
using TMPro;
using Truongtv.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Menu
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private Button settingButton,
            switchButton,
            ticketButton,
            noAdButton,
            roomButton,
            userInfoButton,
            leaderBoardButton,
            shopButton;
        [SerializeField] private ParticleGold ticketEffect;
        [SerializeField] private TextMeshProUGUI ticketText, userNameText, userTrophyText;
        [SerializeField] private CharacterAnimationGraphic userAvatar;
        [SerializeField] private ModeGameLauncher modeGamePrefab;
        [SerializeField] private Transform modeContainer;
        [SerializeField] private List<Color> modeGameColors;
        private List<MiniGameInfo> _miniGameList;
        private static MenuController _instance;
        public static MenuController Instance => _instance;
        
        private void Awake()
        {
            if (_instance != null)
                Destroy(gameObject);
            _instance = this;
        }


        private void Start()
        {
            settingButton.onClick.AddListener(OnSettingButtonClick);
            switchButton.onClick.AddListener(OnSwitchButtonClick);
            ticketButton.onClick.AddListener(OnTicketButtonClick);
            noAdButton.onClick.AddListener(OnNoAdButtonClick);
            roomButton.onClick.AddListener(OnRoomButtonClick);
            userInfoButton.onClick.AddListener(OnUserInfoButtonClick);
            leaderBoardButton.onClick.AddListener(OnLeaderBoardButtonClick);
            shopButton.onClick.AddListener(OnShopButtonClick);
            ticketText.text = GameDataManager.Instance.GetTotalTicket() + "";
            userNameText.text = GameDataManager.Instance.GetUserName();
            userTrophyText.text = GameDataManager.Instance.GetTotalTrophy() + "";
            userAvatar.SetSkin(GameDataManager.Instance.GetCurrentSkin());
            userAvatar.SetSkinColor(GameDataManager.Instance.GetCurrentColor());
            InitModeGame();
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

        public void InitModeGame()
        {
            _miniGameList = new List<MiniGameInfo>(GameDataManager.Instance.miniGameData.miniGameList);
            modeContainer.RemoveAllChild();
            PrepareData();
            var count = GameDataManager.Instance.miniGameData.maxGameCount;
            for (var i = 0; i < count; i++)
            {
                if (i < _miniGameList.Count)
                {
                    var item = Instantiate(modeGamePrefab, modeContainer);
                    item.Init(modeGameColors[i],_miniGameList[i]);
                }
                else
                {
                    var item = Instantiate(modeGamePrefab, modeContainer);
                    item.Init(Color.clear);
                }
            }
        }

        private void PrepareData()
        {
            var lastPlayed = GameDataManager.Instance.GetLastPlayed();
            var max = 0;
            foreach (var info in _miniGameList)
            {
                info.total = GameDataManager.Instance.GetMiniGameCountPlayed(info.gameId);
                info.win = GameDataManager.Instance.GetMiniGameWinCount(info.gameId);
                info.lose = GameDataManager.Instance.GetMiniGameLoseCount(info.gameId);
                info.recentPlay = !lastPlayed.IsNullOrWhitespace()&&lastPlayed.Equals(info.gameId);
                if (max < info.total)
                    max = info.total;
            }

            foreach (var info in _miniGameList)
            {
                info.mostPlay = (max == info.total) && max>0;
            }
        }
        
        #region Button Event


        private void OnSwitchButtonClick()
        {
            PopupMenuController.Instance.ShowPopupCustomizeCharacter();
        }

        private void OnTicketButtonClick()
        {
            PopupMenuController.Instance.ShowPopupShop(ShopType.Ticket);
        }

        private void OnUserInfoButtonClick()
        {
            PopupMenuController.Instance.ShowPopupUserInfo();
        }

        private void OnNoAdButtonClick()
        {
            PopupMenuController.Instance.ShowPopupShop(ShopType.Pack);
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