using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Projects.Scripts.Menu;
using Projects.Scripts.Scriptable;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Hub.Component
{
    public class ShopTicketItem : MonoBehaviour
    {
        [ValueDropdown(nameof(GetALlShopTicketId))] public string shopId;
        [SerializeField] private TextMeshProUGUI valueText, priceText;
        [SerializeField] private Button button;
        private ShopData _shopData;
        private ShopItemData _item;
        private List<string> GetALlShopTicketId()
        {
            return ShopData.Instance.GetAllShopTicketId();
        }
        public void Init(ShopData shopData)
        {
            _shopData = shopData;
            _item = _shopData.shopTicketList.Find(a => a.shopId == shopId);
            valueText.text = $"{_item.reward.ticket}";
            button.onClick.RemoveAllListeners();
            if (_item.purchaseType == PurchaseType.Ad)
            {
                var time = GameDataManager.Instance.GetLastTimeClaimFreeTicket();
                if (DateTime.Now.Date.Subtract(time.Date).TotalDays >= 1)
                {
                    GameDataManager.Instance.ResetFreeTicketCountInDay();
                }
                var count = GameDataManager.Instance.GetFreeTicketCountInDay();
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
                priceText.text = GameServiceManager.GetItemLocalPriceString(_item.skuId);
                button.onClick.AddListener(BuyIap);
            }
            
        }

        private void BuyByAd()
        {
            GameServiceManager.ShowRewardedAd("shop_free_ticket", () =>
            {
                var count = GameDataManager.Instance.GetFreeTicketCountInDay()+1;
                GameDataManager.Instance.UpdateFreeTicketCountInDay(count);
                PurchaseSuccess();
            });
        }

        private void BuyIap()
        {
            GameServiceManager.PurchaseProduct(_item.skuId, (result, sku) =>
            {
                if (result)
                {
                    PurchaseSuccess();
                }
            });
        }

        private void PurchaseSuccess()
        {
            MenuController.Instance.AddTicket(_item.reward.ticket);
            Init(_shopData);
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
            
            Init(_shopData);
        }
    }
}