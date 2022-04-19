using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Projects.Scripts.Hub
{
    [RequireComponent(typeof(SkeletonGraphic))]
    public class CharacterAnimationGraphic : MonoBehaviour
    {
        private SkeletonGraphic _skeletonGraphic;
        [SerializeField,SpineSkin] private string baseSkin;
        [SerializeField, SpineAnimation] private string idleAnim, changeClothesAnim;
        private Skin _characterSkin;
        private SkeletonData _skeletonData;
        private List<string> _skinList;
        private Color _color;

        private void Awake()
        {
            _skeletonGraphic = GetComponent<SkeletonGraphic>();
            _skeletonData = _skeletonGraphic.Skeleton.Data;
            if (!string.IsNullOrEmpty(idleAnim))
                _skeletonGraphic.AnimationState.SetAnimation(0, idleAnim, true);
        }
        public void SetSkinColor(Color color)
        {
            _color = color;
            ChangeSlotColor("head");
            ChangeSlotColor("hand_L");
            ChangeSlotColor("hand_R");
            if (_skinList.Contains("body/body_00"))
            {
                ChangeSlotColor("leg_L");
                ChangeSlotColor("leg_R");
                ChangeSlotColor("body");
            }
        }
      
        public void SetSkin(List<string> skinList)
        {
            _skinList = skinList;
            _characterSkin = new Skin("character-base");
            _characterSkin.AddSkin(_skeletonData.FindSkin(baseSkin));
            for (var i = 0; i < _skinList.Count; i++)
            {
                _characterSkin.AddSkin(_skeletonData.FindSkin(_skinList[i]));
            }
            UpdateCombineSkin();
            
        }

        public void ChangeClothes()
        {
            if (!string.IsNullOrEmpty(changeClothesAnim))
            {
                var trackEntry = _skeletonGraphic.AnimationState.SetAnimation(0, changeClothesAnim, false);
                trackEntry.Complete += entry =>
                {
                    if (!string.IsNullOrEmpty(idleAnim))
                        _skeletonGraphic.AnimationState.SetAnimation(0, idleAnim, true);
                };
            }
        }
        
        #region private
        private void UpdateCombineSkin()
        {
            var skeleton = _skeletonGraphic.Skeleton;
            var resultCombinedSkin = new Skin("character-combined");
            resultCombinedSkin.AddSkin(_characterSkin);

            skeleton.SetSkin(resultCombinedSkin);
            skeleton.SetSlotsToSetupPose();
        }

        private void ChangeSlotColor(string slotName)
        {
            var slot = _skeletonGraphic.Skeleton.FindSlot(slotName);
            slot.R = _color.r;
            slot.G = _color.g;
            slot.B = _color.b;
        }
        

        #endregion
    }
}