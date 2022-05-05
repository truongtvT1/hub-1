#if USING_IAP
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace ThirdParties.Truongtv.IAP
{
    public class UnityInAppPurchase:IStoreListener,IPaymentService
    {
        private IStoreController _controller;
        private bool _isInit;
        private bool _purchaseInProgress;
        private CrossPlatformValidator _validator;
        private IAppleExtensions _appleExtensions;
        private IGooglePlayStoreExtensions _googlePlayStoreExtensions;
         private readonly List<SubscriptionInfo> _subscriptionInfos;
        public UnityInAppPurchase(IEnumerable<SkuItem> skuItems)
        {
             _subscriptionInfos = new List<SubscriptionInfo>();
            Initialized(skuItems);
        }
        private event Action<bool,string> PurchaseActionCallback;
        #region Usage
        public string GetItemLocalPriceString(string id)
        {
            
            return _controller.products.WithID(id)?.metadata.localizedPriceString;
        }

        public void RestorePurchase()
        {
            if (_controller == null || _appleExtensions == null) return;

            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                _appleExtensions.RestoreTransactions((result) =>
                {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                    // no purchases are available to be restored.
                    //Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                _googlePlayStoreExtensions.RestoreTransactions(result =>
                {
                    
                });
            }
        }

        public string GetItemLocalCurrency(string id)
        {
            return _controller.products.WithID(id)?.metadata.isoCurrencyCode;
        }

        public float GetItemLocalPrice(string id)
        {
            return (float)(_controller.products.WithID(id) == null ? 0 : _controller.products.WithID(id).metadata.localizedPrice);
        }
       

        public bool IsInitialized()
        {
            return _isInit;
        }

        public void UpdateCallback(Action<bool, string> purchaseActionCallback)
        {
            PurchaseActionCallback += purchaseActionCallback;
        }

        public void PurchaseProduct(string productId)
        {
            if (_purchaseInProgress)
            {
                //Debug.Log("Please wait, purchase in progress"); 
                PurchaseActionCallback?.Invoke(false,productId);
                return;
            }

            if (_controller == null)
            {
                //Debug.LogError("Purchasing is not initialized");
                PurchaseActionCallback?.Invoke(false,productId);
                return;
            }

            if (_controller.products.WithID(productId) == null)
            {
                //Debug.LogError("No product has id " + productId);
                PurchaseActionCallback?.Invoke(false,productId);
                return;
            }

            _purchaseInProgress = true;
            
            // if (_controller.products.WithID(productId).definition.type == ProductType.Subscription)
            // {
            //     ChangeSubscription(productId);
            // }
            // else
            // {
                _controller.InitiatePurchase(_controller.products.WithID(productId));
            // }
           
        }

       
        #endregion

        #region Private

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            _isInit = false;
        }

        private void Initialized(IEnumerable<SkuItem> skuItems)
        {
            
            var module = StandardPurchasingModule.Instance();
            var builder = ConfigurationBuilder.Instance(module);
            //var skuItems = GameService.Instance.GetIAPSku();
            foreach (var sku in skuItems)
            {
                var type =sku.productType;
#if UNITY_ANDROID
                builder.AddProduct(sku.skuId, type, new IDs
                {
                    {sku.skuId, GooglePlay.Name}
                });
                
#elif UNITY_IOS||UNITY_IPHONE
            builder.AddProduct(sku.skuId,type, new IDs
            {
                {sku.skuId, AppleAppStore.Name}
            });
#endif
                
            }

            UnityPurchasing.Initialize(this, builder);
#if !UNITY_EDITOR
_validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                AppleTangle.Data(), Application.identifier);
#endif
            
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            try
            {
                _validator.Validate(e.purchasedProduct.receipt);
            }
            catch (IAPSecurityException)
            {
                PurchaseActionCallback?.Invoke(false,e.purchasedProduct.definition.id);
                return PurchaseProcessingResult.Complete;
            }
            _purchaseInProgress = false;
            if (e.purchasedProduct.definition.type == ProductType.Subscription)
            {
                if (CheckIfProductIsAvailableForSubscriptionManager(e.purchasedProduct.receipt))
                {
                    var dict = _appleExtensions.GetIntroductoryPriceDictionary();
                    var introJson = (dict == null || !dict.ContainsKey(e.purchasedProduct.definition.storeSpecificId))
                        ? null
                        : dict[e.purchasedProduct.definition.storeSpecificId];
                    var p = new SubscriptionManager(e.purchasedProduct, introJson);
                    var info = p.getSubscriptionInfo();
                    _subscriptionInfos.Add(info);
                }
            }
            PurchaseActionCallback?.Invoke(true,e.purchasedProduct.definition.id);
            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
        {
            _purchaseInProgress = false;
            PurchaseActionCallback?.Invoke(false,item.definition.id);
        }


        
        public void OnInitialized(IStoreController storeController, IExtensionProvider extensions)
        {
            _controller = storeController;
            _isInit = true;
            _googlePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
            _appleExtensions = extensions.GetExtension<IAppleExtensions>();
            _appleExtensions.RegisterPurchaseDeferredListener(Deferred);
            var dict = _appleExtensions.GetIntroductoryPriceDictionary();
            foreach (var item in _controller.products.all)
            {
                if (!item.availableToPurchase) continue;
               
                if (item.receipt != null)
                {
                    if (item.definition.type == ProductType.Subscription)
                    {
                        if (CheckIfProductIsAvailableForSubscriptionManager(item.receipt))
                        {
                            var introJson = (dict == null || !dict.ContainsKey(item.definition.storeSpecificId))
                                ? null
                                : dict[item.definition.storeSpecificId];
                            var p = new SubscriptionManager(item, introJson);
                            var info = p.getSubscriptionInfo();
                            _subscriptionInfos.Add(info);
                        }
                    }
                }
            }
        }


        private void Deferred(Product item)
        {
            //Debug.Log("Purchase deferred: " + item.definition.id);
        }
        private bool CheckIfProductIsAvailableForSubscriptionManager(string receipt)
        {
            var receiptWrapper = (Dictionary<string, object>) MiniJson.JsonDecode(receipt);
            if (!receiptWrapper.ContainsKey("Store") || !receiptWrapper.ContainsKey("Payload"))
            {
                //Debug.Log("The product receipt does not contain enough information");
                return false;
            }
        
            var store = (string) receiptWrapper["Store"];
            var payload = (string) receiptWrapper["Payload"];
        
            if (payload != null)
            {
                switch (store)
                {
                    case GooglePlay.Name:
                    {
                        var payloadWrapper = (Dictionary<string, object>) MiniJson.JsonDecode(payload);
                        if (!payloadWrapper.ContainsKey("json"))
                        {
                            // Debug.Log(
                            //     "The product receipt does not contain enough information, the 'json' field is missing");
                            return false;
                        }
        
                        var originalJsonPayloadWrapper =
                            (Dictionary<string, object>) MiniJson.JsonDecode((string) payloadWrapper["json"]);
                        if (originalJsonPayloadWrapper == null ||
                            !originalJsonPayloadWrapper.ContainsKey("developerPayload"))
                        {
                            // Debug.Log(
                            //     "The product receipt does not contain enough information, the 'developerPayload' field is missing");
                            return false;
                        }
        
                        var developerPayloadJson = (string) originalJsonPayloadWrapper["developerPayload"];
                        var developerPayloadWrapper =
                            (Dictionary<string, object>) MiniJson.JsonDecode(developerPayloadJson);
                        if (developerPayloadWrapper == null || !developerPayloadWrapper.ContainsKey("is_free_trial") ||
                            !developerPayloadWrapper.ContainsKey("has_introductory_price_trial"))
                        {
                            // Debug.Log(
                            //     "The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                            return false;
                        }
        
                        return true;
                    }
                    case AppleAppStore.Name:
                    case AmazonApps.Name:
                    case MacAppStore.Name:
                    {
                        return true;
                    }
                    default:
                    {
                        return false;
                    }
                }
            }
        
            return false;
        }
        private void ChangeSubscription(string skuId)
        {
            var t = GetSubscriptionData();
            if (t == null || t.Count == 0)
            {
                _controller?.InitiatePurchase(skuId);
                return;
            }
            Action<string, string> googlePlayCallback = _googlePlayStoreExtensions.UpgradeDowngradeSubscription;
            Action<Product,string> onSubUpgrade = _controller.InitiatePurchase;
            SubscriptionManager.UpdateSubscription( _controller.products.WithID(skuId),_controller.products.WithID(t[t.Count-1].getProductId()),"developerPayload",onSubUpgrade,googlePlayCallback);
        }
        public List<SubscriptionInfo> GetSubscriptionData()
        {
            return _subscriptionInfos;
        }

        
        #endregion
    }
}
#endif