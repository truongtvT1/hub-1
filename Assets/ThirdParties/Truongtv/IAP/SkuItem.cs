

using System;

#if USING_IAP
using UnityEngine.Purchasing;
#endif
namespace ThirdParties.Truongtv.IAP
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
