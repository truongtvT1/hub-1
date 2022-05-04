using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv.IAP;
using Truongtv.Utilities;
using UnityEngine;

namespace Projects.Scripts.Scriptable
{
    [CreateAssetMenu(fileName = "ShopData", menuName = "Truongtv/GameData/ShopData", order = 0)]
    public class ShopData : SingletonScriptableObject<ShopData>
    {
        [ShowIf("@this.iapData == null")]public IAPData iapData;
        [BoxGroup("Treasure")]public List<ChestData> shopChestList;
        [BoxGroup("Treasure")] public float sPercent, aPercent, bPercent, cPercent;
        [BoxGroup("Pack")] public List<ShopItemData> shopPackList;
        [BoxGroup("Ticket")] public List<ShopItemData> shopTicketList;

        public string[] GetAllSkuItem()
        {
            return iapData.GetSkuItems().Select(a => a.skuId).ToArray();
        }

        public List<string> GetAllShopTicketId()
        {
            return shopTicketList.Select(a => a.shopId).ToList();
        }

        public List<string> GetAllShopChestId()
        {
            return shopChestList.Select(a => a.shopId).ToList();
        }
        public List<string> GetAllShopPackId()
        {
            return shopPackList.Select(a => a.shopId).ToList();
        }
    }

    [Serializable]
    public class ShopItemData
    {
        public string shopId;
        public PurchaseType purchaseType;
        [ShowIf(nameof(purchaseType), PurchaseType.Iap),ValueDropdown(nameof(GetAllSkuItem))] public string skuId;
        [ShowIf(nameof(purchaseType), PurchaseType.Ad)] public int freePerDay,coolDown;

        [ShowIf(nameof(purchaseType), PurchaseType.Ticket)] public int price;
        [SerializeField,HideIf(nameof(purchaseType), PurchaseType.Ticket)] public RewardData reward;
        private string[] GetAllSkuItem()
        {
            return ShopData.Instance.GetAllSkuItem();
        }
    }
    public enum PurchaseType
    {
        Iap,Ad,Ticket
    }

    [Serializable]
    public class RewardData
    {
        public int ticket;
        public List<string> skinList;
        
        
    }

    [Serializable]
    public class ChestData
    {
        public string shopId;
        public PurchaseType purchaseType;
        [ShowIf(nameof(purchaseType), PurchaseType.Iap),ValueDropdown(nameof(GetAllSkuItem))] public string skuId;
        [ShowIf(nameof(purchaseType), PurchaseType.Ad)] public int freePerDay,coolDown;
        [ShowIf(nameof(purchaseType), PurchaseType.Ticket)] public int price;
        public int numberItemReward;
        private string[] GetAllSkuItem()
        {
            return ShopData.Instance.GetAllSkuItem();
        }
    }
}
