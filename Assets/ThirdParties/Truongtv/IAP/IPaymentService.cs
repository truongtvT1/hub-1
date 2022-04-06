using System;

namespace ThirdParties.Truongtv.IAP
{
    public interface IPaymentService
    {
        bool IsInitialized();
        void UpdateCallback( Action<bool,string> purchaseActionCallback);
        void PurchaseProduct(string sku);
        string GetItemLocalPriceString(string sku);
        string GetItemLocalCurrency(string sku);
        float GetItemLocalPrice(string sku);
        void RestorePurchase();
    }
}