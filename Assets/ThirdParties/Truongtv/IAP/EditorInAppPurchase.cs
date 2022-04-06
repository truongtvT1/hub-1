using System;
using System.Collections.Generic;
using System.Linq;

namespace ThirdParties.Truongtv.IAP
{
    public class EditorInAppPurchase : IPaymentService
    {
        private event Action<bool, string> PurchaseActionCallback;
        private List<SkuItem> _skuItems;

        public bool IsInitialized()
        {
            return true;
        }

        public EditorInAppPurchase(IEnumerable<SkuItem> skuItems)
        {
            _skuItems = skuItems.ToList();
        }

        public void UpdateCallback(Action<bool, string> purchaseActionCallback)
        {
            PurchaseActionCallback = purchaseActionCallback;
        }

        public void PurchaseProduct(string sku)
        {
            PurchaseActionCallback?.Invoke(true, sku);
        }

        public string GetItemLocalPriceString(string sku)
        {
            if (!_skuItems.Exists(a => a.skuId.Equals(sku)))
                return string.Empty;
            var item = _skuItems.Find(a => a.skuId.Equals(sku));
            return item.defaultValue;
        }

        public string GetItemLocalCurrency(string sku)
        {
            return "$";
        }

        public float GetItemLocalPrice(string sku)
        {
            if (!_skuItems.Exists(a => a.skuId.Equals(sku)))
                return 0;
            var item = _skuItems.Find(a => a.skuId.Equals(sku));
            var price = item.defaultValue.Replace("$", "");
            if (float.TryParse(price, out var result))
                return result;
            return 0;
        }

        public void RestorePurchase()
        {
        }
    }
}