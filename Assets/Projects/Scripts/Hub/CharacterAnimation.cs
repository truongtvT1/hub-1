using System;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Projects.Scripts.Hub
{
    public class CharacterAnimation : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation skeletonAnimation;
        private Skin _characterSkin;
        private SkeletonData _skeletonData;
        private void Start()
        {
            _skeletonData = skeletonAnimation.skeleton.Data;
        }

        public void UpdateSkin()
        {
            _characterSkin = new Skin("character-base");
        }

        public void UpdateCombineSkin()
        {
            
        }
    }
}