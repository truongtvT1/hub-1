using System;
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
    public class ShopPackItem : MonoBehaviour
    {
        [ValueDropdown(nameof(GetAllShopPackId))] public string shopId;
        [SerializeField] private Sprite sprite;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Button button;
        private ShopData _shopData;
        private ShopItemData _item;

        private void Awake()
        {
            button.onClick.AddListener(BuyIap);
        }

        private List<string> GetAllShopPackId()
        {
            return ShopData.Instance.GetAllShopPackId();
        }

        public void Init(ShopData shopData)
        {
            _shopData = shopData;
            _item = _shopData.shopPackList.Find(a => a.shopId == shopId);
            priceText.text = $"{GameServiceManager.GetItemLocalPriceString(_item.skuId)}";
            
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
            Init(_shopData);
            if (_item.reward.blockAd)
            {
                GameDataManager.Instance.SetPurchased();
            }
            foreach (var skin in _item.reward.skinList)
            {
                GameDataManager.Instance.UnlockSkin(skin);
            }
            if(_item.reward.ticket>0)
                MenuController.Instance.AddTicket(_item.reward.ticket);
            if (_item.reward.skinList.Count > 0)
            {
                PopupMenuController.Instance.ShowPopupReceivePack(sprite,PopupMenuController.Instance.ShowPopupCustomizeCharacter);
            }
            else
            {
                PopupMenuController.Instance.ShowPopupReceivePack(sprite);
            }
        }
    }
}