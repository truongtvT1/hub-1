using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Projects.Scripts.Hub;
using Sirenix.OdinInspector;
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
        public Collider2D headCollider;
        public bool isSprinting, isHoldingSprint, isNormalRun, isCollideWall, isInSpecialTrap, isDead, isBot;
        public LayerMask itemLayer;
        private BotDifficulty botDifficulty;
        [TitleGroup("Bot"), SerializeField, ShowIf(nameof(isBot))] private float sprintRate, sprintDuration, botVision, minimumDistanceToSprint, timeStable, timeSprinting, distanceToObj;
        private Tween tween;
        private Vector2 cacheColliderSize, cacheOffsetCollider, cacheHeadColliderOffset, runLine;
        private Transform checkPoint;
        private Color currentColor;
        private List<string> currentSkin = new List<string>();

        private const float _EASY_NORMALSPEED = 1;
        private const float _NORMAL_NORMALSPEED = 1.3f;
        private const float _HARD_NORMALSPEED = 1.6f;
        private const float _HELL_NORMALSPEED = 1.9f;
        private const float _EASY_SPRINTSPEED = 3;
        private const float _NORMAL_SPRINTSPEED = 3.3f;
        private const float _HARD_SPRINTSPEED = 3.6f;
        private const float _HELL_SPRINTSPEED = 3.9f;
        private const float _EASY_SPRINTRATE = 2f;
        private const float _NORMAL_SPRINTRATE = 1.5f;
        private const float _HARD_SPRINTRATE = 1f;
        private const float _HELL_SPRINTRATE = 1.5f;
        private const float _EASY_SPRINTDURATION = 3;
        private const float _NORMAL_SPRINTDURATION = 3.2f;
        private const float _HARD_SPRINTDURATION = 3.4f;
        private const float _HELL_SPRINTDURATION = 3.6f;
        private void Awake()
        {
            cacheColliderSize = collider.size;
            cacheOffsetCollider = collider.offset;
            cacheHeadColliderOffset = headCollider.offset;
            
        }

        public bool CheckCollidingObject()
        {
            var hit = Physics2D.Raycast(transform.position + new Vector3(cacheColliderSize.x/2 + .1f,cacheColliderSize.y / 2 + cacheOffsetCollider.y),Vector2.right,.1f,itemLayer);
            if (hit)
            {
                if (!hit.collider.isTrigger)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        
        private async void Start()
        {
            if (!isBot)
            {
                currentSkin = GameDataManager.Instance.GetSkinInGame();
                currentColor = GameDataManager.Instance.GetCurrentColor();
                mountIcon.gameObject.SetActive(true);
                await Task.Delay(100);
                anim.PlayIdle();
                anim.SetSkin(currentSkin);
                anim.SetSkinColor(currentColor);
                runLine = transform.position;
            }
        }

        public void Init(List<string> skin, Color color, BotDifficulty difficulty)
        {
            currentSkin = skin;
            currentColor = color;
            anim.PlayIdle();
            anim.SetSkin(skin);
            anim.SetSkinColor(color);
            runLine = transform.position;
            switch (difficulty)
            {
                case BotDifficulty.Easy:
                    normalSpeed = Random.Range(0.8f, _EASY_NORMALSPEED);
                    sprintSpeed = Random.Range(2.8f, _EASY_SPRINTSPEED);
                    sprintRate = Random.Range(_EASY_SPRINTRATE,2.2f);
                    sprintDuration = Random.Range(2.8f, _EASY_SPRINTDURATION);
                    break;
                case BotDifficulty.Normal: 
                    normalSpeed = Random.Range(0.8f, _NORMAL_NORMALSPEED);
                    sprintSpeed = Random.Range(2.8f, _NORMAL_SPRINTSPEED);
                    sprintRate = Random.Range(_NORMAL_SPRINTRATE,2.2f);
                    sprintDuration = Random.Range(2.8f, _NORMAL_SPRINTDURATION);
                    break;
                case BotDifficulty.Hard: 
                    normalSpeed = Random.Range(0.8f, _HARD_NORMALSPEED);
                    sprintSpeed = Random.Range(2.8f, _HARD_SPRINTSPEED);
                    sprintRate = Random.Range(_HARD_SPRINTRATE,2.2f);
                    sprintDuration = Random.Range(2.8f, _HARD_SPRINTDURATION);
                    break;
                case BotDifficulty.Hell: 
                    normalSpeed = Random.Range(0.8f, _HELL_NORMALSPEED);
                    sprintSpeed = Random.Range(2.8f, _HELL_SPRINTSPEED);
                    sprintRate = Random.Range(_HELL_SPRINTRATE,2.2f);
                    sprintDuration = Random.Range(2.8f, _HELL_SPRINTDURATION);
                    break;
            }
        }

        void Refresh()
        {
            anim.PlayIdle();
            anim.SetSkin(currentSkin);
            anim.SetSkinColor(currentColor);
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
            headCollider.offset = cacheHeadColliderOffset;
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
                headCollider.offset = new Vector2(0.48f, -.57f);
            }
            else
            {
                anim.PlayDodge();
                headCollider.offset = cacheHeadColliderOffset;
            }
            var size = collider.size;
            var offset = collider.offset;
            collider.size = new Vector2(size.x,sprintingCollider.y);
            collider.offset = new Vector2(offset.x,sprintingCollider.x);
        }

        #region AI

        float DetectDistanceToObject()
        {
            float distance = 0;
            RaycastHit2D hit =
                Physics2D.Raycast(
                    transform.position + new Vector3(cacheColliderSize.x / 2 + .1f,
                        cacheColliderSize.y / 2 + cacheOffsetCollider.y), Vector2.right, botVision, itemLayer);
            if (hit)
            {
                distance = hit.distance;
            }
            return distance;
        }

        
        
        #endregion
        
        private void Update()
        {
            if (StickRunGameController.Instance.state == GameState.Playing && !isDead)
            {
                if (CheckCollidingObject())
                {
                    isCollideWall = true;
                    anim.PlayRun();
                    anim.PlayRunScare();
                    headCollider.offset = cacheHeadColliderOffset;
                    transform.DOKill();
                    timeStable = 0;
                    timeSprinting = 0;
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

                    if (isBot)
                    {
                        moveDirection = MoveDirection.Right;
                        distanceToObj = DetectDistanceToObject();
                        // if (distanceToObj != 0)
                        // {
                            // if (distanceToObj <= minimumDistanceToSprint)
                            // {
                            //     if (timeSprinting >= sprintDuration)
                            //     {
                            //         isHoldingSprint = false;
                            //         NormalRun();
                            //     }
                            //     
                            //     timeSprinting += Time.deltaTime;
                            //     isHoldingSprint = true;
                            //     Sprint();
                            // }
                        // }
                        // else
                        {
                            if (timeStable >= sprintRate)
                            {
                                isHoldingSprint = true;
                                isSprinting = false;
                                isNormalRun = true;
                            }
                            else if (timeSprinting >= sprintDuration)
                            {
                                isHoldingSprint = false;
                                isSprinting = true;
                                isNormalRun = false;
                            }
                            else if (timeStable == 0 && timeSprinting == 0)
                            {
                                isHoldingSprint = false;
                                isSprinting = false;
                                isNormalRun = false;
                            }
                            
                            if (!isHoldingSprint && !isNormalRun)
                            {
                                NormalRun();
                                timeSprinting = 0;
                            }
                            else if (isHoldingSprint && !isSprinting)
                            {
                                Sprint();
                                timeStable = 0;
                            }
                            
                            if (isHoldingSprint && isSprinting)
                            {
                                timeSprinting += Time.deltaTime;
                            }
                            else if (!isHoldingSprint && isNormalRun)
                            {
                                timeStable += Time.deltaTime;
                            }

                        }
                        return;
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

        public void Die(DamageType type, Transform checkPoint, Vector3 diePos, Ease ease = Ease.Linear)
        {
            if (isDead)
            {
                return;
            }
            isDead = true;
            isInSpecialTrap = false;
            collider.enabled = false;
            isNormalRun = false;
            isSprinting = false;
            isHoldingSprint = false;
            headCollider.enabled = false;
            timeStable = 0;
            timeSprinting = 0;
            if (!isBot)
            {
                StickRunGameController.Instance.Dead();
                mountIcon.gameObject.SetActive(false);
            }
            this.checkPoint = checkPoint;
            transform.DOKill();
            transform.DOMove(diePos, .5f).SetEase(ease).OnComplete(() =>
            {
                switch (type)
                {
                    case DamageType.Water:
                        anim.PlayDie(callback:() =>
                        {
                            anim.PlayIdle();
                            Respawn();
                        });
                        break;
                    case DamageType.Object:
                        anim.PlayDie(callback:() =>
                        {
                            anim.PlayIdle();
                            Respawn();
                        });
                        break;
                }
            });
        }

        async void Respawn()
        {
            if (isBot)
            {
                await Task.Delay(2000);
                collider.enabled = true;
                headCollider.enabled = true;
                transform.position = new Vector3(checkPoint.position.x, runLine.y);
                Refresh();
                await Task.Delay(200);
                isDead = false;
            }
            else
            {
                if (StickRunGameController.Instance.CheckCanRevive())
                {
                    await Task.Delay(2000);
                    mountIcon.gameObject.SetActive(true);
                    collider.enabled = true;
                    headCollider.enabled = true;
                    transform.position = new Vector3(checkPoint.position.x, runLine.y);
                    Refresh();
                    await Task.Delay(200);
                    isDead = false;
                    
                }
                else
                {
                    //show popup revive ad
                    StickRunGameController.Instance.EndGame();
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