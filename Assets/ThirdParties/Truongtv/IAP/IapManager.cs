using System;
using System.Collections;
using System.Linq;
using Truongtv.PopUpController;
using UnityEngine;

namespace ThirdParties.Truongtv.IAP
{
    public class IapManager :MonoBehaviour
    {
        private IPaymentService _paymentService;
        private Action<bool, string> _purchaseCallback;
        [SerializeField]private IAPData iapData;
        [SerializeField] private bool fakeIap;
        #region Private Function
        public void Init()
        {
            var skuItem = iapData.GetSkuItems();
            #if USING_IAP
            _paymentService = new UnityInAppPurchase(skuItem);
            #else
            _paymentService = new EditorInAppPurchase(skuItem);
            #endif
            _paymentService.UpdateCallback(PurchaseActionCallback);
        }
        private void PurchaseActionCallback(bool isSuccess, string sku)
        {
            if (_purchaseCallback == null) return;
            _purchaseCallback?.Invoke(isSuccess, sku);
            _purchaseCallback = null;
        }
        #endregion
        #region Public Function
        
        private bool IsInitialized()
        {
            return _paymentService.IsInitialized();
        }
        public void PurchaseProduct(string sku, Action<bool, string> pAction)
        {
           #if UNITY_EDITOR
            pAction?.Invoke(true,sku);
            return;
            #endif
            if (fakeIap)
            {
                pAction?.Invoke(true,sku);
                return;
            }
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PopupController.Instance.ShowToast("No internet connection. Make sure to turn on your Wifi/Mobile data.");
                
                return;
            }
            if (!IsInitialized())
            {
                Init();
                
            }
            StopAllCoroutines();
            StartCoroutine(Act());
            IEnumerator Act()
            {
                while (!IsInitialized())
                {
                    yield return new WaitForSeconds(0.1f);
                }
                _purchaseCallback = pAction;
                _paymentService.PurchaseProduct(sku);
            }
           
        }
        public string GetItemLocalPriceString(string sku)
        {
            #if UNITY_EDITOR
            var list2 = iapData.GetSkuItems().ToList();
            return list2.Find(a => a.skuId.Equals(sku)).defaultValue;
            #endif
            if (fakeIap)
            {
                var list3 = iapData.GetSkuItems().ToList();
                return list3.Find(a => a.skuId.Equals(sku)).defaultValue;
            }
            if (!IsInitialized()|| Application.internetReachability == NetworkReachability.NotReachable)
            {
                Init();
                var list = iapData.GetSkuItems().ToList();
                return list.Find(a => a.skuId.Equals(sku)).defaultValue;
            }
            return _paymentService.GetItemLocalPriceString(sku);
        }
        public void RestorePurchase()
        {
            _paymentService.RestorePurchase();
        }
         #endregion
    }
}
