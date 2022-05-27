using System;
using System.Collections;
using System.Collections.Generic;
using Projects.Scripts.Menu;
using Projects.Scripts.Popup;
using Projects.Scripts.Scriptable;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv;
using TMPro;
using Truongtv.PopUpController;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Projects.Scripts.Hub.Component
{
    public class ShopChestItem : MonoBehaviour
    {
        [ValueDropdown(nameof(GetALlShopTicketId))] public string shopId;
        [SerializeField] private TextMeshProUGUI  priceText;
        [SerializeField] private Button button;
        private ShopData _shopData;
        private ChestData _item;
        private PopupShop _shop;
        private SkinData _data;
        private List<string> GetALlShopTicketId()
        {
            return ShopData.Instance.GetAllShopChestId();
        }

        public void Init(ShopData shopData,PopupShop popupShop)
        {
            _shopData = shopData;
            _shop = popupShop;
            _data = GameDataManager.Instance.skinData;
            _item = _shopData.shopChestList.Find(a => a.shopId == shopId);
            
            button.onClick.RemoveAllListeners();
            if (_item.purchaseType == PurchaseType.Ad)
            {
                var time = GameDataManager.Instance.GetLastTimeClaimFreeChest();
                if (DateTime.Now.Date.Subtract(time.Date).TotalDays >= 1)
                {
                    GameDataManager.Instance.ResetFreeChestCountInDay();
                }
                var count = GameDataManager.Instance.GetFreeChestCountInDay();
                var remain = _item.freePerDay - count;
                if (_item.freePerDay - count > 0)
                {
                    var timeRemain = GameDataManager.Instance.GetLastTimeClaimFreeChest().AddSeconds(_item.coolDown)
                        .Subtract(DateTime.Now);
                    if (timeRemain.TotalSeconds > 0)
                    {
                        StartCoroutine(CountDown(timeRemain));
                    }
                    else
                    {
                        priceText.text = $"Free({remain})";
                        button.onClick.AddListener(BuyByAd);
                    }
                }
                else
                {
                    var nextDay = DateTime.Now.AddDays(1).Date;
                    var countdown = nextDay.Subtract(DateTime.Now);
                    StartCoroutine(CountDown(countdown));
                }
            }
            else
            {
                priceText.text = $"{_item.price}";
                button.onClick.AddListener(BuyByTicket);
            }
        }
        private void BuyByAd()
        {
            GameServiceManager.ShowRewardedAd(GameServiceManager.eventConfig.rewardForShopFreeDrawChest, () =>
            {
                GameDataManager.Instance.UpdateFreeChestCountInDay(1);
                PurchaseSuccess();
            });
        }

        private void BuyByTicket()
        {
            if (GameDataManager.Instance.GetTotalTicket() < _item.price)
            {
                PopupController.Instance.ShowToast("Not Enough Tickets");
                return;
            }
            MenuController.Instance.UseTicket(_item.price);
            PurchaseSuccess();
        }

        private void PurchaseSuccess()
        {
            Init(_shopData,_shop);
            var listItem = new List<SkinInfo>();
            var lastChestOpen = GameDataManager.Instance.GetTotalChestOpen();
            int numberSTier = 0;
            if (lastChestOpen % 10 + _item.numberItemReward >= 10)
            {
                numberSTier = (lastChestOpen % 10 + _item.numberItemReward) / 10;
                listItem.AddRange(_data.GetSkinByRank(numberSTier,SkinRank.S));
            }
            for (var i = 0; i < _item.numberItemReward-numberSTier; i++)
            {
                var r = Random.Range(0, 100f);
                if (r < _shopData.sPercent)
                {
                    var item = _data.GetSkinByRank(1, SkinRank.S);
                    while (listItem.Contains(item[0]))
                    {
                        item = _data.GetSkinByRank(1, SkinRank.S);
                    }
                    listItem.Add(item[0]);
                }
                else if (r < _shopData.aPercent)
                {
                    var item = _data.GetSkinByRank(1, SkinRank.A);
                    while (listItem.Contains(item[0]))
                    {
                        item = _data.GetSkinByRank(1, SkinRank.A);
                    }
                    listItem.Add(item[0]);
                }
                else if (r < _shopData.bPercent)
                {
                    var item = _data.GetSkinByRank(1, SkinRank.B);
                    while (listItem.Contains(item[0]))
                    {
                        item = _data.GetSkinByRank(1, SkinRank.B);
                    }
                    listItem.Add(item[0]);
                }
                else
                {
                    var item = _data.GetSkinByRank(1, SkinRank.C);
                    while (listItem.Contains(item[0]))
                    {
                        item = _data.GetSkinByRank(1, SkinRank.C);
                    }
                    listItem.Add(item[0]);
                }
            }
            GameDataManager.Instance.UpdateChestOpenNumber(_item.numberItemReward);
            foreach (var item in listItem)
            {
                GameDataManager.Instance.UnlockSkin(item.skinName);
            }
            _shop.UpdateChestProgress(lastChestOpen % 10,lastChestOpen % 10 + _item.numberItemReward);
            PopupMenuController.Instance.ShowPopupOpenChest(listItem, () =>
            {
                if (_item.numberItemReward == 10)
                {
                    PopupMenuController.Instance.ShowPopupCustomizeCharacter();
                }
            });



        }
        private IEnumerator CountDown(TimeSpan timSpan)
        {
            var time = timSpan;
            priceText.text = time.ToString(@"hh\:mm\:ss");
            while (time.TotalSeconds>0)
            {
                yield return new WaitForSeconds(1f);
                time = time.Subtract(TimeSpan.FromSeconds(1));
                priceText.text = time.ToString(@"hh\:mm\:ss");
            }
            Init(_shopData,_shop);
        }
    }
}