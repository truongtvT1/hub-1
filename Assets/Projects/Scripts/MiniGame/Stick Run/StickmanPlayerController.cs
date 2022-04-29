using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
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
        public SpriteRenderer mountIcon;
        public GameObject dodgeCollider, narutoCollider;
        public bool isSprinting, isHoldingSprint, isNormalRun, isCollideWall, isInSpecialTrap, isDead, isBot;
        private Tween tween;
        private Vector2 cacheColliderSize, cacheOffsetCollider, runLine;
        private Transform checkPoint;
        private Color currentColor;
        private List<string> currentSkin = new List<string>();
        private void Awake()
        {
            cacheColliderSize = collider.size;
            cacheOffsetCollider = collider.offset;
            runLine = transform.position;
        }

        public bool CheckCollidingObject()
        {
            var hit = Physics2D.Raycast(transform.position + new Vector3(cacheColliderSize.x/2 + .1f,cacheColliderSize.y / 2 + cacheOffsetCollider.y),Vector2.right,.1f);
            if (hit)
            {
                if (!hit.collider.isTrigger)
                {
                    return true;
                }
            }
            return false;
        }
        
        private void Start()
        {
            if (!isBot)
            {
                currentSkin = GameDataManager.Instance.GetSkinInGame();
                currentColor = GameDataManager.Instance.GetCurrentColor();
                mountIcon.gameObject.SetActive(true);
            }
            Init();
        }

        public void Init()
        {
            if (!isBot)
            {
                anim.PlayIdle();
                anim.SetSkin(currentSkin);
                anim.SetSkinColor(currentColor);
            }
        }
        
        void NormalRun()
        {
            isNormalRun = true;
            isSprinting = false;
            tween = transform.DOMoveX(transform.position.x + (int) moveDirection * 100f, normalSpeed)
                .SetSpeedBased(true)
                .SetRelative(true)
                .Play();
            anim.PlayRun();
            anim.PlayRunScare();
            collider.size = cacheColliderSize;
            collider.offset = cacheOffsetCollider;
            dodgeCollider.SetActive(false);
            narutoCollider.SetActive(false);
        }

        void Sprint()
        {
            isSprinting = true;
            isNormalRun = false;
            tween = transform.DOMoveX(transform.position.x + (int) moveDirection * 100f, sprintSpeed)
                .SetSpeedBased(true)
                .SetRelative(true)
                .Play();
            anim.PlayRunFast();
            if (Random.value > 0.5f)
            {
                anim.PlayRunNaruto();
                // narutoCollider.SetActive(true);
                // dodgeCollider.SetActive(false);
            }
            else
            {
                anim.PlayDodge();
                // narutoCollider.SetActive(false);
                // dodgeCollider.SetActive(true);
            }
            var size = collider.size;
            var offset = collider.offset;
            collider.size = new Vector2(size.x,sprintingCollider.y);
            collider.offset = new Vector2(offset.x,sprintingCollider.x);
        }

        private float bufferTimeCollideWall = .5f;
        private void Update()
        {
            if (StickRunGameController.Instance.state == GameState.Playing && !isDead)
            {
                if (CheckCollidingObject())
                {
                    isCollideWall = true;
                    anim.PlayRun();
                    anim.PlayRunScare();
                    // dodgeCollider.SetActive(false);
                    // narutoCollider.SetActive(false);
                    transform.DOKill();
                }
                else if (isCollideWall)
                {
                    isCollideWall = false;
                    NormalRun();
                }

                if (!isCollideWall)
                {
                    if (isInSpecialTrap && transform.position.y == runLine.y)
                    {
                        transform.DOMoveY(runLine.y - 2, 2.5f).SetEase(Ease.Linear);
                    }
                    else if (!isInSpecialTrap && transform.position.y != runLine.y)
                    {
                        transform.DOMoveY(runLine.y, 1).SetEase(Ease.Linear);
                    }
                    else if (isHoldingSprint && transform.position.y != runLine.y)
                    {
                        transform.DOMoveY(runLine.y, 1).SetEase(Ease.Linear);
                    }
                    
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
        }

        public void Die(DamageType type, Transform checkPoint, Transform die = null)
        {
            if (isDead)
            {
                return;
            }
            isDead = true;
            collider.enabled = false;
            mountIcon.gameObject.SetActive(false);
            // dodgeCollider.SetActive(false);
            // narutoCollider.SetActive(false);
            if (!isBot)
            {
                StickRunGameController.Instance.Dead();
            }
            this.checkPoint = checkPoint;
            transform.DOKill();
            if (die)
            {
                transform.DOMove(die.position, .5f).SetEase(Ease.InSine).OnComplete(() =>
                {
                    Dead();
                });
            }
            else
            {
                Dead();
            }
            void Dead()
            {
                switch (type)
                {
                    case DamageType.Water:
                        anim.PlayDie(callback:() =>
                        {
                            Respawn();
                        });
                        break;
                    case DamageType.Object:
                        anim.PlayDie(callback:() =>
                        {
                            Respawn();
                        });
                        break;
                }
            }
            
        }

        async void Respawn()
        {
            if (isBot)
            {
                await Task.Delay(3000);
                collider.enabled = true;
                transform.position = new Vector3(checkPoint.position.x, runLine.y);
                isDead = false;
            }
            else
            {
                if (StickRunGameController.Instance.CheckCanRevive())
                {
                    await Task.Delay(3000);
                    mountIcon.gameObject.SetActive(true);
                    collider.enabled = true;
                    Init();
                    transform.position = new Vector3(checkPoint.position.x, runLine.y);
                    await Task.Delay(200);
                    isDead = false;
                    
                }
                else
                {
                    //show popup revive ad
                    
                }
            }
        }
        
        public void TouchSprint(bool isHolding = false)
        {
            isHoldingSprint = isHolding;
            moveDirection = MoveDirection.Right;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + new Vector3(cacheColliderSize.x/2 + .1f,cacheColliderSize.y/2 + cacheOffsetCollider.y),transform.position + new Vector3(cacheColliderSize.x/2 + .2f,cacheColliderSize.y/2 + cacheOffsetCollider.y));
        }
    }
}