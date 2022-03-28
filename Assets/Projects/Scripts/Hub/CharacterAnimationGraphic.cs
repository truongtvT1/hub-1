using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Projects.Scripts.Hub
{
    public class CharacterAnimationGraphic : MonoBehaviour
    {
        [SerializeField] private SkeletonGraphic skeletonGraphic;
        [SerializeField,SpineSkin] private string baseSkin;
        private Skin _characterSkin;
        private SkeletonData _skeletonData;
        private List<string> _skinList;
        private void Start()
        {
            _skeletonData = skeletonGraphic.Skeleton.Data;
        }
        public void Init(List<string> skinList)
        {
            _skinList = new List<string>(skinList);
            UpdateSkin(_skinList);
        }

        public void ChangeSkin(string current, string next)
        {
            _skinList.Remove(current);
            _skinList.Add(next);
            UpdateSkin(_skinList);
        }
        public void UpdateSkin(List<string> skinList)
        {
            _characterSkin = new Skin("character-base");
            _characterSkin.AddSkin(_skeletonData.FindSkin(baseSkin));
            for (var i = 0; i < skinList.Count; i++)
            {
                _characterSkin.AddSkin(_skeletonData.FindSkin(skinList[i]));
            }
            UpdateCombineSkin();
        }
        private void UpdateCombineSkin()
        {
            var skeleton = skeletonGraphic.Skeleton;
            var resultCombinedSkin = new Skin("character-combined");
            resultCombinedSkin.AddSkin(_characterSkin);

            skeleton.SetSkin(resultCombinedSkin);
            skeleton.SetSlotsToSetupPose();
        }
    }
}