﻿using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace MiniGame
{
    public class PlayerMovement : MonoBehaviour
    {
        
        [SerializeField, FoldoutGroup("Movement")]
        private float  moveSpeed, acceleration ,decceleration,deccelTime;
        [SerializeField, FoldoutGroup("Movement")] private Transform icon,totalDisplay;
        [SerializeField, FoldoutGroup("Movement")]public MoveDirection moveDirection;
        [SerializeField, FoldoutGroup("Jump")]
        private float jumpForce;
        [SerializeField,FoldoutGroup("Jump"), PropertyRange(0, 1)] private float jumpCutMultiple;
        [SerializeField, FoldoutGroup("Jump")]
        private float gravityScale, fallGravityMultiple;
        [SerializeField, FoldoutGroup("Check")]
        private new Rigidbody2D rigidbody2D;

        [SerializeField, FoldoutGroup("Check")]
        private LayerMask groundLayer;
        [SerializeField, FoldoutGroup("Check")]
        private LayerMask wallLayer;
        [SerializeField, FoldoutGroup("Check")]
        private LayerMask boxLayer;
        [SerializeField, FoldoutGroup("Check")]
        private Transform groundCheckPoint;

        [SerializeField, FoldoutGroup("Check")] private float ballRadius;

        [SerializeField, FoldoutGroup("Check")] private float JumpCoyoteTime, jumpBufferTime;

        [SerializeField, FoldoutGroup("Effect")]
        private float forcePushBox;
        [SerializeField, FoldoutGroup("Effect")] private GameObject landingEffect, moveEffect;
        [SerializeField, FoldoutGroup("Effect")]private float effectByForceTime = 0.5f;
        private MoveDirection _roll;
        private float lastGroundedTime, lastJumpTime,countDeccelTime;
        private bool isJumping,isJumpButtonRelease = true;
        private bool effectByForce;
       
        private float effectByForceCountTime = 0.5f;
        private bool _moveEffectSpawnRunning,_collisionWithWall;
        private PlayerController _controller;
        public void Init(PlayerController controller)
        {
            _controller = controller;
        }

        private void Update()
        {
            
            #region Check Ground

            if (CheckIsGrounded())
            {
                if (lastGroundedTime<0f)
                {
                    var obj = Instantiate(landingEffect);
                    obj.transform.SetParent(transform.parent);
                    obj.transform.localPosition = transform.localPosition + new Vector3(0, -0.67f, 0f);
                    // SoundInGameManager.Instance.PlayBallLandingSound();
                }
                lastGroundedTime = JumpCoyoteTime;
                if (!_moveEffectSpawnRunning)
                {
                    _moveEffectSpawnRunning = true;
                    InvokeRepeating(nameof(RunEffect),0f,0.2f);
                }
                rigidbody2D.gravityScale = gravityScale;
            }
            else
            {
                lastGroundedTime -= Time.deltaTime;
                _moveEffectSpawnRunning = false;
                CancelInvoke(nameof(RunEffect));
            }
            if (Physics2D.OverlapPoint(groundCheckPoint.position + new Vector3(ballRadius*(int)moveDirection,0,0f),  wallLayer))
            {
                _collisionWithWall = true;
            }
            else
            {
                _collisionWithWall = false;
            }
            #endregion
            
            #region Jump gravity

            if (rigidbody2D.velocity.y < 0)
                rigidbody2D.gravityScale = gravityScale * fallGravityMultiple;
            else
                rigidbody2D.gravityScale = gravityScale;

            #endregion
            
            #region Jump

            if (!isJumpButtonRelease)
                lastJumpTime = jumpBufferTime;
            else
                lastJumpTime -= Time.deltaTime;

            if (!effectByForce)
            {
                if (rigidbody2D.velocity.y > 0 && isJumping && isJumpButtonRelease)
                {
                    rigidbody2D.AddForce(Vector2.down*rigidbody2D.velocity.y* jumpCutMultiple,ForceMode2D.Impulse);
                    lastJumpTime = 0;
                }
                else if (rigidbody2D.velocity.y < 0 && isJumping)
                {
                    isJumping = false;
                }
            }

            #endregion
            
            #region Movement

            if (effectByForce)
            {
                effectByForceCountTime -= Time.deltaTime;
                if (effectByForceCountTime <= 0)
                {
                    effectByForce = false;
                }
                
            }
            else
            {
                if (!_collisionWithWall)
                {
                    if (moveDirection != MoveDirection.None)
                    {
                        countDeccelTime = deccelTime;
                        if (rigidbody2D.velocity.x *(int)moveDirection< moveSpeed)
                        {
                            var velocity = rigidbody2D.velocity;
                            velocity.x += (int) moveDirection * acceleration * Time.fixedDeltaTime;
                            rigidbody2D.velocity = velocity;
                        }
                    }
                    else
                    {
                        countDeccelTime -= Time.deltaTime;
                        if (countDeccelTime > 0)
                        {
                            var velocity = rigidbody2D.velocity;
                            if (velocity.x < 0)
                            {
                                velocity.x += decceleration * Time.deltaTime;
                                if (velocity.x > 0) velocity.x = 0;
                            }
                            else if (velocity.x > 0)
                            {
                                velocity.x -= decceleration * Time.deltaTime;
                                if (velocity.x < 0) velocity.x = 0;
                            }
                            rigidbody2D.velocity = velocity;
                        }
                    }
                }
            }
            #endregion
            #region Check Box
            //
            // if (Physics2D.OverlapPoint(groundCheckPoint.position + new Vector3(ballRadius*(int)moveDirection,0,0f),  groundLayer) &&
            //     Physics2D.OverlapPoint(groundCheckPoint.position - new Vector3(0, ballRadius, 0f), boxLayer))
            // {
            //     var box = Physics2D.OverlapPoint(groundCheckPoint.position - new Vector3(0, ballRadius, 0f), boxLayer);
            //     // if(box.gameObject.GetComponent<Box>()!=null)
            //         // box.gameObject.GetComponent<Box>().PushBox(moveDirection,forcePushBox);
            // }

            #endregion

        }
        private bool CheckIsGrounded()
        {
            Vector2 boundSize = gameObject.GetComponent<CircleCollider2D>().bounds.size;
            boundSize = new Vector2(boundSize.x - 0.1f, boundSize.y);
            RaycastHit2D hit2D = Physics2D.BoxCast(gameObject.GetComponent<CircleCollider2D>().bounds.center, boundSize,
                0, Vector2.down, 0.1f, groundLayer);

            
            return hit2D.collider != null && hit2D.point.y< transform.position.y && !hit2D.collider.isTrigger;
        }
        #region Controller

        public void TouchMoveLeft(bool release = false)
        {
            if (release) // release button left
                moveDirection = MoveDirection.None;
            else
            {
                moveDirection = MoveDirection.Left;
                var scale = transform.localScale;
                totalDisplay.localScale= new Vector3(Mathf.Abs(scale.x)*(int)moveDirection,scale.y,scale.z);
            }
        }

        public void TouchMoveRight(bool release = false)
        {
            if (release)// release button right
                moveDirection = MoveDirection.None;
            else
            {
                moveDirection = MoveDirection.Right;
                var scale = transform.localScale;
                totalDisplay.localScale= new Vector3(Mathf.Abs(scale.x)*(int)moveDirection,scale.y,scale.z);
            }
        }

        public void TouchJump(bool release = false)
        {
            if (release) // release button jump
            {
                isJumpButtonRelease = true;
            }
            else
            {
                isJumpButtonRelease = false;
                lastJumpTime = jumpBufferTime;
                if (lastGroundedTime > 0 && lastJumpTime > 0 && !isJumping && rigidbody2D.velocity.y<4f)
                {
                    var velocity = rigidbody2D.velocity;
                    velocity.y = jumpForce;
                    rigidbody2D.velocity = velocity;
                    lastGroundedTime = 0;
                    lastJumpTime = 0;
                    isJumping = true;
                    // SoundInGameManager.Instance.PlayBallJumpSound();
                }
            }
        }

       
        public void SetForce(Vector2 force,bool special = false)
        {
            SetForceInstant(force);
            if (!special) return;
            effectByForce = true;
            effectByForceCountTime = effectByForceTime;
        }
        public void SetForceInstant(Vector2 force)
        {
            rigidbody2D.velocity = force;
        }
        #endregion

        #region Effect

        private void RunEffect()
        {
            if (moveDirection != MoveDirection.None)
            {
                var obj = Instantiate(moveEffect);
                obj.transform.position = transform.position + new Vector3(0, -0.6f, 0f);
            }
        }

        #endregion

        
    }
}