using System.Collections.Generic;
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
        public List<Color> skinColors;
        public int sTierTicket = 150, aTierTicket = 100, bTierTicket = 50, cTierTicket = 30;
        public List<string> GetAllSkinName()
        {
            var result = new List<string>();
            var totalSkin = skeleton.GetSkeletonData(true).Skins.Items;
            foreach (var skin in totalSkin)
            {
                if(!result.Contains(skin.Name))
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
            hairSkins = new List<SkinInfo>();
            var list = GetAllSkinsByName("hair/");
            foreach (var item in list)
            {
                hairSkins.Add(new SkinInfo{skinName = item});
            }
        }
        [Button]
        private void UpdateGloveSkinList()
        {
            gloveSkins = new List<SkinInfo>();
            var list = GetAllSkinsByName("gloves/");
            foreach (var item in list)
            {
                gloveSkins.Add(new SkinInfo{skinName = item});
            }
        }
        [Button]
        private void UpdateBodySkinList()
        {
            bodySkins = new List<SkinInfo>();
            var list  = GetAllSkinsByName("body/");
            foreach (var item in list)
            {
                bodySkins.Add(new SkinInfo{skinName = item});
            }
        }
        [Button]
        private void UpdateGlassSkinList()
        {
            var list  = GetAllSkinsByName("glass/");
            glassSkins = new List<SkinInfo>();
            foreach (var item in list)
            {
                glassSkins.Add(new SkinInfo{skinName = item});
            }
        }
        [Button]
        private void UpdateWingSkinList()
        {
            var list  = GetAllSkinsByName("wing/");
            cloakSkins = new List<SkinInfo>();
            foreach (var item in list)
            {
                cloakSkins.Add(new SkinInfo{skinName = item});
            }
        }
        
    }
    
    
}