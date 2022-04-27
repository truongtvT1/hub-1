using System;
using DG.Tweening;
using Projects.Scripts.Hub;
using ThirdParties.Truongtv;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniGame.StickRun
{
    public class StickmanPlayerController : MonoBehaviour
    {
        public float normalSpeed, sprintSpeed;
        public CharacterAnimation anim;
        public CapsuleCollider2D collider;
        public MoveDirection moveDirection;
        public Vector2 sprintingCollider;
        public bool isSprinting, isHoldingSprint, isNormalRun, isCollideWall, isBot;
        private Tweener tween;
        private Vector2 cacheColliderSize, cacheOffsetCollider;
        private void Awake()
        {
            cacheColliderSize = collider.size;
            cacheOffsetCollider = collider.offset;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            isCollideWall = true;
            if (tween.IsPlaying())
            {
                tween.Pause();
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            isCollideWall = false;
            if (!tween.IsPlaying())
            {
                tween.Play();
            }
        }

        private void Start()
        {
            if (!isBot)
            {
                var currentSkin = GameDataManager.Instance.GetSkinInGame();
                var currentColor = GameDataManager.Instance.GetCurrentColor(); 
                anim.SetSkin(currentSkin);
                anim.SetSkinColor(currentColor);
                anim.PlayIdle();
            }
        }

        public void Init()
        {
            
        }
        
        void NormalRun()
        {
            isNormalRun = true;
            isSprinting = false;
            tween = transform.DOMoveX(transform.position.x + (int) moveDirection * 100f, normalSpeed)
                .SetSpeedBased(true).SetRelative(true)
                .Play();
            anim.PlayRun();
            anim.PlayRunScare();
            collider.size = cacheColliderSize;
            collider.offset = cacheOffsetCollider;
        }

        void Sprint()
        {
            isSprinting = true;
            isNormalRun = false;
            tween = transform.DOMoveX(transform.position.x + (int) moveDirection * 100f, sprintSpeed)
                .SetSpeedBased(true).SetRelative(true)
                .Play();
            anim.PlayRunFast();
            if (Random.value > 0.5f)
            {
                anim.PlayRunNaruto();
            }
            else
            {
                anim.PlayDodge();
            }
            var size = collider.size;
            var offset = collider.offset;
            collider.size = new Vector2(size.x,sprintingCollider.y);
            collider.offset = new Vector2(offset.x,sprintingCollider.x);
        }
        
        private void Update()
        {
            if (StickRunGameController.Instance.state == GameState.Playing)
            {
                if (!isHoldingSprint && !isNormalRun)
                {
                    NormalRun();
                }
                else if (isHoldingSprint && !isSprinting)
                {
                    Sprint();
                }
            }
        }

        public void TouchSprint(bool isHolding = false)
        {
            isHoldingSprint = isHolding;
            moveDirection = MoveDirection.Right;
        }
    }
}