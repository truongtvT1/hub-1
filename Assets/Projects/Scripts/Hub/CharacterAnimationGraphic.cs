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
        [SerializeField] private SkeletonDataAsset _skeletonDataAsset;
        [SerializeField,SpineSkin] private string baseSkin;
        [SerializeField, SpineAnimation] private string idleAnim, changeClothesAnim;
        [SerializeField, ValueDropdown(nameof(GetAllAnimationName))]
        private List<string> winAnimations;
        private Skin _characterSkin;
        private SkeletonData _skeletonData;
        private List<string> _skinList;
        private Color _color;

        private void Awake()
        {
            _skeletonGraphic = GetComponent<SkeletonGraphic>();
            
            _skeletonData = _skeletonDataAsset.GetSkeletonData(true);
            if (!string.IsNullOrEmpty(idleAnim))
                _skeletonGraphic.AnimationState.SetAnimation(0, idleAnim, true);
        }
        
        List<string> GetAllAnimationName()
        {
            if (_skeletonData == null)
            {
                _skeletonData = _skeletonDataAsset.GetSkeletonData(true);
            }
            
            var listName = new List<string>();
            var listAnimation = _skeletonData.Animations.Items;
            for (int i = 0; i < listAnimation.Length; i++)
            {
                listName.Add(listAnimation[i].Name);
            }

            return listName;
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

        public void PlayMixWin()
        {
            var r = UnityEngine.Random.Range(0, winAnimations.Count);
            var entry = _skeletonGraphic.AnimationState.SetAnimation(1, winAnimations[r], false);
            entry.Complete += trackEntry =>
            {
                PlayMixWin();
            };
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