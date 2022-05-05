using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using Newtonsoft.Json;
#endif

using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

namespace Projects.Scripts.Scriptable
{
    [CreateAssetMenu(fileName = "SkinData", menuName = "Truongtv/GameData/SkinData", order = 0)]
    public class SkinData : ScriptableObject
    {
        [SerializeField] private SkeletonDataAsset skeleton;
        [SerializeField] public List<SkinInfo> hairSkins;
        [SerializeField] public List<SkinInfo> gloveSkins;
        [SerializeField] public List<SkinInfo> bodySkins;
        [SerializeField] public List<SkinInfo> glassSkins;
        [SerializeField] public List<SkinInfo> cloakSkins;
        public const string HairPrefix = "hair/";
        public const string GlovePrefix = "gloves/";
        public const string SuitPrefix = "body/";
        public const string GlassPrefix = "glass/";
        public const string CloakPrefix = "wing/";
        public List<Color> skinColors;
        public int sTierTicket = 150, aTierTicket = 100, bTierTicket = 50, cTierTicket = 30, colorTicket = 5;

        public List<string> GetAllSkinName()
        {
            var result = new List<string>();
            var totalSkin = skeleton.GetSkeletonData(true).Skins.Items;
            foreach (var skin in totalSkin)
            {
                if (!result.Contains(skin.Name))
                    result.Add(skin.Name);
            }

            return result;
        }

        private List<string> GetAllSkinsByName(string prefix)
        {
            var total = GetAllSkinName();
            total.RemoveAll(a => !a.Contains(prefix));
            return total;
        }

        [Button]
        private void UpdateHairSkinList()
        {
            if (hairSkins == null)
                hairSkins = new List<SkinInfo>();
            var list = GetAllSkinsByName(HairPrefix);
            foreach (var item in list)
            {
                if (!hairSkins.Exists(a => a.skinName.Equals(item)))
                    hairSkins.Add(new SkinInfo(SkinType.Hair, item));
            }
        }

        [Button]
        private void UpdateGloveSkinList()
        {
            if (gloveSkins == null)
                gloveSkins = new List<SkinInfo>();
            var list = GetAllSkinsByName(GlovePrefix);
            foreach (var item in list)
            {
                if (!gloveSkins.Exists(a => a.skinName.Equals(item)))
                    gloveSkins.Add(new SkinInfo(SkinType.Glove, item));
            }
        }

        [Button]
        private void UpdateBodySkinList()
        {
            if (bodySkins == null)
                bodySkins = new List<SkinInfo>();
            var list = GetAllSkinsByName(SuitPrefix);
            foreach (var item in list)
            {
                if (!bodySkins.Exists(a => a.skinName.Equals(item)))
                    bodySkins.Add(new SkinInfo(SkinType.Suit, item));
            }
        }

        [Button]
        private void UpdateGlassSkinList()
        {
            var list = GetAllSkinsByName(GlassPrefix);
            if (glassSkins == null)
                glassSkins = new List<SkinInfo>();
            foreach (var item in list)
            {
                if (!glassSkins.Exists(a => a.skinName.Equals(item)))
                    glassSkins.Add(new SkinInfo(SkinType.Glass, item));
            }
        }

        [Button]
        private void UpdateWingSkinList()
        {
            var list = GetAllSkinsByName(CloakPrefix);
            if (cloakSkins == null)
                cloakSkins = new List<SkinInfo>();
            foreach (var item in list)
            {
                if (!cloakSkins.Exists(a => a.skinName.Equals(item)))
                    cloakSkins.Add(new SkinInfo(SkinType.Cloak, item));
            }
        }
        
        #if UNITY_EDITOR
        [Button]
        private void UpdateData(TextAsset asset, SkinType type)
        {
            var data = asset.text;
            var list = JsonConvert.DeserializeObject<List<SkinInfo>>(data);
            List<SkinInfo> temp = null;
            switch (type)
            {
                case SkinType.Hair:
                    temp = hairSkins;
                    break;
                case SkinType.Glove:
                    temp = gloveSkins;
                    break;
                case SkinType.Suit:
                    temp = bodySkins;
                    break;
                case SkinType.Glass:
                    temp = glassSkins;
                    break;
                case SkinType.Cloak:
                    temp = cloakSkins;
                    break;
            }

            foreach (var info in list)
            {
                if (temp.Exists(a => a.skinName.Equals(info.skinName)))
                {
                    temp.Find(a => a.skinName.Equals(info.skinName)).rank = info.rank;
                }
            }
        }
        #endif
    }
}