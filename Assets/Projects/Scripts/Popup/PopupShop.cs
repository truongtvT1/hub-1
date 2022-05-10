using System;
using System.Collections.Generic;
using DG.Tweening;
using Projects.Scripts.Hub.Component;
using Projects.Scripts.Menu;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv;
using TMPro;
using Truongtv.PopUpController;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Popup
{
    public class PopupShop : BasePopup
    {
        [SerializeField, BoxGroup("UI")] private Button closeButton;
        [SerializeField, BoxGroup("Chest")] private Button ruleButton;
        [SerializeField, BoxGroup("Chest")] private List<ShopChestItem> chestItemList;
        [SerializeField, BoxGroup("Chest")] private Image chestProgress;
        [SerializeField, BoxGroup("Chest")] private float[] fillProgress;
        [SerializeField, BoxGroup("Ticket")] private List<ShopTicketItem> ticketItemList;

        private void Awake()
        {
            closeButton.onClick.AddListener(Close);
            ruleButton.onClick.AddListener(() => { PopupMenuController.Instance.ShowPopupRule(); });
        }

        public void Init(ShopType shopType)
        {
            chestProgress.fillAmount = fillProgress[GameDataManager.Instance.GetTotalChestOpen() % 10];
            foreach (var ticketItem in ticketItemList)
            {
                ticketItem.Init(GameDataManager.Instance.shopData);
            }

            foreach (var chestItem in chestItemList)
            {
                chestItem.Init(GameDataManager.Instance.shopData, this);
            }
        }

        public void UpdateChestProgress(int from, int to)
        {
            chestProgress.fillAmount = fillProgress[to % 10];
            Debug.Log(to);
            // int count = from;
            // if (to / 10 > 0)
            // {
            //     chestProgress.fillAmount = fillProgress[count];
            //     DOTween.To(() => count, x => count = x, to, 0.5f)
            //         .OnStart(() => { chestProgress.fillAmount = fillProgress[count]; })
            //         .OnUpdate(() => { })
            //         .OnComplete(() => { chestProgress.fillAmount = fillProgress; });
            // }
            // else
            // {
            // }
        }
    }

    public enum ShopType
    {
        Chest,
        Pack,
        Ticket
    }
}