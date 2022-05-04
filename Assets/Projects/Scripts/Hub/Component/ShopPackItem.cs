using System.Collections.Generic;
using Projects.Scripts.Scriptable;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Hub.Component
{
    public class ShopPackItem : MonoBehaviour
    {
        [ValueDropdown(nameof(GetAllShopPackId))] public string shopId;

        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Button button;
        private ShopData _shopData;
        private ShopItemData _item;
        private List<string> GetAllShopPackId()
        {
            return ShopData.Instance.GetAllShopPackId();
        }

        public void Init(ShopData shopData)
        {
            _shopData = shopData;
            _item = _shopData.shopPackList.Find(a => a.shopId == shopId);
            priceText.text = $"{GameServiceManager.GetItemLocalPriceString(_item.skuId)}";
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(BuyIap);
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
        }
    }
}