using System;
using System.Collections;
using System.Collections.Generic;
using Projects.Scripts.Menu;
using Projects.Scripts.Scriptable;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv;
using TMPro;
using Truongtv.PopUpController;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Hub.Component
{
    public class ShopChestItem : MonoBehaviour
    {
        [ValueDropdown(nameof(GetALlShopTicketId))] public string shopId;
        [SerializeField] private TextMeshProUGUI  priceText;
        [SerializeField] private Button button;
        private ShopData _shopData;
        private ChestData _item;
        private List<string> GetALlShopTicketId()
        {
            return ShopData.Instance.GetAllShopChestId();
        }

        public void Init(ShopData shopData)
        {
            _shopData = shopData;
            _item = _shopData.shopChestList.Find(a => a.shopId == shopId);
            
            button.onClick.RemoveAllListeners();
            if (_item.purchaseType == PurchaseType.Ad)
            {
                var time = GameDataManager.Instance.GetLastTimeClaimFreeChest();
                if (DateTime.Now.Date.Subtract(time.Date).TotalDays >= 1)
                {
                    GameDataManager.Instance.UpdateFreeChestCountInDay(0);
                }
                var count = GameDataManager.Instance.GetFreeChestCountInDay();
                var remain = _item.freePerDay - count;
                if (_item.freePerDay - count > 0)
                {
                    priceText.text = $"Free({remain})";
                    button.onClick.AddListener(BuyByAd);
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
            GameServiceManager.Instance.ShowRewardedAd("shop_free_chest", () =>
            {
                var count = GameDataManager.Instance.GetFreeChestCountInDay()+1;
                GameDataManager.Instance.UpdateFreeChestCountInDay(count);
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
            Init(_shopData);
            // show open chest
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