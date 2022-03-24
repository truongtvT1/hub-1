

using System;
#if USING_IAP
using UnityEngine.Purchasing;
#endif
namespace ThirdParties.Truongtv.IapManager
{
    [Serializable]
    public class SkuItem
    {
        public string skuId;
#if USING_IAP
        public ProductType productType = ProductType.Consumable;
#endif
        public string defaultValue;
    }
}
