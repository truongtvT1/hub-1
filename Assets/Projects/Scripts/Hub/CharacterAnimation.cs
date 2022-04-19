﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using ThirdParties.Truongtv.SoundManager;
using UnityEngine;
using Animation = Spine.Animation;

namespace Projects.Scripts.Hub
{
    public class CharacterAnimation : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation skeletonAnimation;
        [SerializeField, SpineSkin] private string baseSkin;

        [SerializeField, ValueDropdown(nameof(GetAllAnimationName))]
        private List<string> winAnimations;

        [SerializeField, ValueDropdown(nameof(GetAllAnimationName))]
        private List<string> loseAnimations;

        [SerializeField, ValueDropdown(nameof(GetAllAnimationName))]
        private List<string> dieAnimations;

        [SerializeField, ValueDropdown(nameof(GetAllAnimationName))]
        private List<string> stopAnimations;

        [SerializeField, SpineAnimation(dataField = nameof(skeletonAnimation))]
        private string runScareAnim;
        [SerializeField, SpineAnimation(dataField = nameof(skeletonAnimation))]
        private string runNaruto;
        [SerializeField, SpineAnimation(dataField = nameof(skeletonAnimation))]
        private string stun;
        [SerializeField, SpineAnimation(dataField = nameof(skeletonAnimation))]
        private string getHit;
        private Skin _characterSkin;
        private SkeletonData _skeletonData;
        private List<string> _skinList;
        private Color _color;
        private string currentAnimationName = "";

        private void Start()
        {
            _skeletonData = skeletonAnimation.skeleton.Data;
        }

        List<string> GetAllAnimationName()
        {
            if (_skeletonData == null)
            {
                _skeletonData = skeletonAnimation.skeleton.Data;
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
            if (_skeletonData == null)
                _skeletonData = skeletonAnimation.skeleton.Data;
            _skinList = skinList;
            _characterSkin = new Skin("character-base");
            _characterSkin.AddSkin(_skeletonData.FindSkin(baseSkin));
            for (var i = 0; i < _skinList.Count; i++)
            {
                _characterSkin.AddSkin(_skeletonData.FindSkin(_skinList[i]));
            }

            UpdateCombineSkin();
        }

        private void UpdateCombineSkin()
        {
            var skeleton = skeletonAnimation.Skeleton;
            var resultCombinedSkin = new Skin("character-combined");
            resultCombinedSkin.AddSkin(_characterSkin);

            skeleton.SetSkin(resultCombinedSkin);
            skeleton.SetSlotsToSetupPose();
        }

        private void ChangeSlotColor(string slotName)
        {
            var slot = skeletonAnimation.Skeleton.FindSlot(slotName);
            slot.R = _color.r;
            slot.G = _color.g;
            slot.B = _color.b;
        }

        public TrackEntry PlayIdle(bool loop = true, Action callback = null)
        {
            return PlayAnim("idle", loop: loop, callback: callback);
        }

        public TrackEntry PlayRun(bool loop = true, Action callback = null)
        {
            return PlayAnim("run", 0, loop, callback: callback);
        }

        public TrackEntry PlayRunFast(bool loop = true, Action callback = null)
        {
            return PlayAnim("run2", 0, loop, callback: callback);
        }
        
        public TrackEntry PlayJumpUp(bool loop = true, Action callback = null)
        {
            return PlayAnim("jump_up", 0, loop, callback: callback);
        }
        
        public TrackEntry PlayJumpDown(bool loop = true, Action callback = null)
        {
            return PlayAnim("jump_down", 0, loop, callback: callback);
        }
        
        
        
        TrackEntry PlayAnim(string animName, int trackIndex = 0,
            bool loop = false,
            AudioClip sfx = null, Action callback = null)
        {
            var animationState = skeletonAnimation.AnimationState;
            if (trackIndex != 0)
            {
                if (sfx != null)
                {
                    SoundManager.Instance.PlaySfx(sfx);
                }
                TrackEntry trackEntry = animationState.SetAnimation(trackIndex, animName, loop);
                trackEntry.Complete += entry => { callback?.Invoke(); };
                return trackEntry;
            }
            else
            {
                if (!string.IsNullOrEmpty(currentAnimationName) 
                    && animationState.GetCurrent(0) != null
                    && currentAnimationName.Equals(animName) 
                    && animationState.GetCurrent(0).Loop)
                {
                    return null;
                }

                if (sfx != null)
                {
                    SoundManager.Instance.PlaySfx(sfx);
                }

                currentAnimationName = animName;
                TrackEntry trackEntry = animationState.SetAnimation(0, animName, loop);
                trackEntry.Complete += entry => { callback?.Invoke(); };
                return trackEntry;
            }
        }
    }
}