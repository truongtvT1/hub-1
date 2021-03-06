using System;
using System.Collections.Generic;
using Projects.Scripts.Popup;
using Projects.Scripts.Scriptable;
using Truongtv.PopUpController;
using UnityEngine;

namespace Projects.Scripts.Menu
{
    [RequireComponent(typeof(PopupController))]
    public class PopupMenuController : MonoBehaviour
    {
        public static PopupMenuController Instance;
        
        [SerializeField] private PopupSetting popupSetting;
        [SerializeField] private PopupLeaderBoard popupLeaderBoard;
        [SerializeField] private PopupCustomizeCharacter popupCustomizeCharacter;
        [SerializeField] private PopupUserInfo popupUserInfo;
        [SerializeField] private PopupRule popupRule;
        [SerializeField] private PopupShop popupShop;
        // [SerializeField] private PopupChooseMode popupChooseMode;
        [SerializeField] private PopupOpenChest popupOpenChest;
        [SerializeField] private PopupReceivePack popupReceivePack;
        private PopupController _controller;

        private void Awake()
        {
            if(Instance!=null)
                Destroy(gameObject);
            Instance = this;
            _controller = GetComponent<PopupController>();
        }

        public void ShowPopupSetting()
        {
            popupSetting.gameObject.SetActive(true);
            popupSetting.Init();
            popupSetting.Show(_controller);
        }

        public void ShowPopupLeaderBoard()
        {
            popupLeaderBoard.gameObject.SetActive(true);
            popupLeaderBoard.Init();
            popupLeaderBoard.Show(_controller);
        }

        public void ShowPopupUserInfo()
        {
            popupUserInfo.gameObject.SetActive(true);
            popupUserInfo.Init();
            popupUserInfo.Show(_controller);
        }
        
        public void ShowPopupCustomizeCharacter()
        {
            popupCustomizeCharacter.gameObject.SetActive(true);
            popupCustomizeCharacter.Init();
            popupCustomizeCharacter.Show(_controller);
        }
        public void ShowPopupRule()
        {
            popupRule.gameObject.SetActive(true);
            popupRule.Show(_controller);
        }
        public void ShowPopupShop(ShopType type = ShopType.Chest)
        {
            popupShop.gameObject.SetActive(true);
            popupShop.Init(type);
            popupShop.Show(_controller);
        }
        public void ShowPopupChooseMode()
        {
            // popupChooseMode.gameObject.SetActive(true);
            // popupChooseMode.Init();
            // popupChooseMode.Show(_controller);
        }

        public void ShowPopupOpenChest(List<SkinInfo> skinList,Action onClose = null)
        {
            popupOpenChest.gameObject.SetActive(true);
            popupOpenChest.Init(skinList,onClose);
            popupOpenChest.Show(_controller);
        }

        public void ShowPopupReceivePack(Sprite sprite,Action moveToCustomizeCharacter = null)
        {
            popupReceivePack.gameObject.SetActive(true);
            popupReceivePack.Init(sprite,moveToCustomizeCharacter);
            popupReceivePack.Show(_controller);
        }
    }
}
