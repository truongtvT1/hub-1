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
        [SerializeField, BoxGroup("Pack")] private List<ShopPackItem> packItemList;
        [SerializeField, BoxGroup("Ticket")] private List<ShopTicketItem> ticketItemList;
        [SerializeField] private TextMeshProUGUI getLegendItemInText;
        private void Awake()
        {
            closeButton.onClick.AddListener(Close);
            ruleButton.onClick.AddListener(() => { PopupMenuController.Instance.ShowPopupRule(); });
        }

        public void Init(ShopType shopType)
        {
            int chestOpen = GameDataManager.Instance.GetTotalChestOpen() % 10;
            getLegendItemInText.text = $"Get a <b><color=#FFCF03>S</color></b> Equipment in {10 - chestOpen % 10} times";
            chestProgress.fillAmount = fillProgress[chestOpen];
            foreach (var ticketItem in ticketItemList)
            {
                ticketItem.Init(GameDataManager.Instance.shopData);
            }

            foreach (var chestItem in chestItemList)
            {
                chestItem.Init(GameDataManager.Instance.shopData, this);
            }
            foreach (var packItem in packItemList)
            {
                packItem.Init(GameDataManager.Instance.shopData);
            }
        }

        public void UpdateChestProgress(int from, int to)
        {
            getLegendItemInText.text = $"Get a <b><color=#FFCF03>S</color></b> equipment in {10 - to % 10} times";
            if (to - from != 10)
            {
                DOTween.To(() => chestProgress.fillAmount, x => chestProgress.fillAmount = x, fillProgress[to % 10], 1f);
            }
            else
            {
                DOTween.To(() => chestProgress.fillAmount, x => chestProgress.fillAmount = x, fillProgress[9], 1f)
                    .OnComplete(() =>
                    {
                        DOTween.To(() => chestProgress.fillAmount, x => chestProgress.fillAmount = x, fillProgress[from % 10],
                            .5f);
                    });
            }
        }
    }

    public enum ShopType
    {
        Chest,
        Pack,
        Ticket
    }
}