using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

namespace Projects.Scripts.Scriptable
{
    [CreateAssetMenu(fileName = "SkinData", menuName = "Truongtv/GameData/SkinData", order = 0)]
    public class SkinData : ScriptableObject
    {
        [SerializeField] private SkeletonDataAsset skeleton;
        [SerializeField,ValueDropdown(nameof(GetAllSkinName))] public string baseSkin;
        [SerializeField,ValueDropdown(nameof(GetAllSkinName))] public List<string> hairSkins;
        [SerializeField,ValueDropdown(nameof(GetAllSkinName))] public List<string> faceSkins;
        [SerializeField,ValueDropdown(nameof(GetAllSkinName))] public List<string> bodySkins;
        public List<string> GetAllSkinName()
        {
            var result = new List<string>();
            var totalSkin = skeleton.GetSkeletonData(true).Skins.Items;
            foreach (var skin in totalSkin)
            {
                if(!result.Contains(skin.Name)&&!skin.Name.Equals("default"))
                    result.Add(skin.Name);
            }
            return result;
        }
        [Button]
        private void UpdateHairSkinList()
        {
            var prefix = "hair/";
            var total = GetAllSkinName();
            total.RemoveAll(a => !a.Contains(prefix));
            hairSkins = total;
        }
    }
}