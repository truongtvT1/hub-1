using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoreMountains.Tools;
using Projects.Scripts.Hub;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniGame
{
    [Serializable]
    public enum BotDifficulty
    {
        Easy = 0,
        Normal = 1,
        Hard = 2,
        Hell = 3
    }
    
    public class PlayerController : MonoBehaviour
    {
        public PlayerMovement Movement;
        public CharacterAnimation Animation;
        [SerializeField] private int MaxHp, CurrentHp;
        [SerializeField, ShowIf(nameof(IsBot))] private Transform target;
        [SerializeField, ShowIf(nameof(IsBot))] private bool isFollowTarget, isReachTarget;
        [SerializeField, ShowIf(nameof(IsBot))] private float targetDistance = 2.5f;
        [ShowIf(nameof(IsBot))] public float minTimeToThink;
        [ShowIf(nameof(IsBot))] public float maxTimeToThink;
        public bool IsBot, jetpackMode;
        private MoveDirection _touchDirection = MoveDirection.None;
        private bool _holdJump, _isJetpackingLeft, _isJetpackingRight;
        private AIBrain brain;

        private const float minEasyTime = 3.5f;
        private const float minNormalTime = 2.5f;
        private const float minHardTime = 1.5f;
        private const float minHellTime = 0.5f;
        private const float maxEasyTime = 8.5f;
        private const float maxNormalTime = 6.5f;
        private const float maxHardTime = 4.5f;
        private const float maxHellTime = 2.5f;
        
        
        
        public bool IsDead
        {
            get
            {
                return CurrentHp <= 0;
            }
        }


        private void Awake()
        {
            if (IsBot)
            {
                brain = GetComponentInChildren<AIBrain>();
            }
        }

        private async void Start()
        {
            Movement.Init(this);
            await Task.Delay(1000);
            Init();
        }

        public void Init()
        {
            if (!IsBot)
            {
                var currentSkin = GameDataManager.Instance.GetSkinInGame();
                var currentColor = GameDataManager.Instance.GetCurrentColor(); 
                Animation.SetSkin(currentSkin);
                Animation.SetSkinColor(currentColor);
            }
        }
        
        public void BotInit(BrainStateData brainData, BotDifficulty difficulty, List<string> skin, Color color)
        {
            Animation.SetSkin(skin);
            Animation.SetSkinColor(color);
            
            switch (difficulty)
            {
                case BotDifficulty.Easy:
                    minTimeToThink = minEasyTime;
                    maxTimeToThink = maxEasyTime;
                    break;
                case BotDifficulty.Normal:
                    minTimeToThink = minNormalTime;
                    maxTimeToThink = maxNormalTime;
                    break;
                case BotDifficulty.Hard:
                    minTimeToThink = minHardTime;
                    maxTimeToThink = maxHardTime;
                    break;
                case BotDifficulty.Hell:
                    minTimeToThink = minHellTime;
                    maxTimeToThink = maxHellTime;
                    break;
            }
            brain.Init(this, brainData);
            brain.ResetBrain();
            brain.ActiveBrain();
        }

        private bool CanControl()
        {
            return !IsDie();
        }
        
        public bool IsDie()
        {
            return IsDead;
        }

        private void Update()
        {
            #region Jetpack

            // if (jetpackMode&& !bullet.bulletActive)
            // {
            //     //jetpack
            //     if (Input.GetKey(KeyCode.LeftArrow))
            //     {
            //         _isJetpackingLeft = true;
            //     }
            //
            //     if (Input.GetKey(KeyCode.RightArrow))
            //     {
            //         _isJetpackingRight = true;
            //     }
            //     if (Input.GetKey(KeyCode.UpArrow))
            //     {
            //         _holdJump = true;
            //     }
            //
            //     if (Input.GetKeyUp(KeyCode.LeftArrow))
            //     {
            //         _isJetpackingLeft = false;
            //     }
            //     if (Input.GetKeyUp(KeyCode.RightArrow))
            //     {
            //         _isJetpackingRight = false;
            //     }
            //     if (Input.GetKeyUp(KeyCode.UpArrow))
            //     {
            //         _holdJump = false;
            //     }
            //
            //     if (_isJetpackingLeft && !_isJetpackingRight)
            //     {
            //         jetpack.TouchJetpackLeft();
            //     }
            //     else if (_isJetpackingRight && !_isJetpackingLeft)
            //     {
            //         jetpack.TouchJetpackRight();
            //     }
            //     else
            //     {
            //         jetpack.TouchJetpackRight(true);
            //         jetpack.TouchJetpackLeft(true);
            //     }
            //     jetpack.TouchJetpackUp(!_holdJump);
            //     return;
            // }


            #endregion

            #region Movement

            if (!IsBot)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                    _touchDirection = MoveDirection.Left;
                if (Input.GetKey(KeyCode.RightArrow))
                    _touchDirection = MoveDirection.Right;
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    _holdJump = true;
                }
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                    _touchDirection = MoveDirection.None;
                if (Input.GetKeyUp(KeyCode.RightArrow))
                    _touchDirection = MoveDirection.None;
                if (Input.GetKeyUp(KeyCode.UpArrow))
                    _holdJump = false;
                if (!CanControl())
                {
                    _touchDirection = MoveDirection.None;
                }
            }


            switch (_touchDirection)
            {
                case MoveDirection.None:
                    Movement.TouchMoveLeft(true);
                    Movement.TouchMoveRight(true);
                    break;
                case MoveDirection.Left:
                    Movement.TouchMoveLeft();
                    break;
                case MoveDirection.Right:
                    Movement.TouchMoveRight();
                    break;
            }

            if (!CanControl())
                return;
            Movement.TouchJump(!_holdJump);

            #endregion
        }

        public void JetpackLeft(bool release = false)
        {
            _isJetpackingLeft = !release;
        }
        
        public void JetpackRight(bool release = false)
        {
            _isJetpackingRight = !release;
        }

        public void MoveLeft(bool release = false)
        {
            if (IsBot)
            {
                _touchDirection = MoveDirection.Left;
                return;
            }

            _touchDirection = release ? MoveDirection.None : MoveDirection.Left;
        }

        public void MoveRight(bool release = false)
        {
            if (IsBot)
            {
                _touchDirection = MoveDirection.Right;
                return;
            }
            _touchDirection = release ? MoveDirection.None : MoveDirection.Right;
        }

        public void Jump(bool release = false)
        {
            _holdJump = !release;
        }

        public void Damage(DamageType type, int damage, bool lostLifeWhenDie)
        {
            switch (type)
            {
                case DamageType.Water: 
                    Die();
                    break;
            }
        }

        void Die()
        {
            //anim die
            CurrentHp = 0;
            StopAllCoroutines();
            Destroy(gameObject);
        }
        
        #region AI Action

        public bool IsCollidingObject()
        {
            return Movement.CheckCollidingObject();
        }

        public bool IsGrounded()
        {
            return Movement.CheckIsGrounded() || Movement.CheckIsOnOtherPlayer();
        }
        
        public void Idle()
        {
            CancelAllMove();
        }

        public void FollowTarget(bool enable = true)
        {
            isFollowTarget = enable;
            if (enable && target == null)
            {
                var listBot = FindObjectsOfType<PlayerController>();
                target = listBot[Random.Range(0, listBot.Length)].transform;
            }
        }

        public void SwitchRandomTarget()
        {
            var listBot = FindObjectsOfType<PlayerController>();
            target = listBot[Random.Range(0, listBot.Length)].transform;
        }
        
        public bool IsFollowTarget()
        {
            return isFollowTarget;
        }        
        
        public Transform GetTarget()
        {
            return target;
        }
        
        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public bool IsReachTarget()
        {
            return isReachTarget;
        }

        public void SetReachTarget(bool reached = true)
        {
            isReachTarget = reached;
        }
        
        public void SetTargetDistance(float distance)
        {
            targetDistance = distance;
        }
        
        public float GetTargetDistance()
        {
            return targetDistance;
        }
        
        public void JumpStart()
        {
            _holdJump = true;
        }

        public void JumpEnd()
        {
            _holdJump = false;
        }
        
        #endregion
        
        
        public void CancelAllMove()
        {
            _touchDirection = MoveDirection.None;
            _holdJump = false;
        }
    }

}

