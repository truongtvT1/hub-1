using System;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MiniGame
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerMovement Movement;
        public PlayerAnimation Animation;
        [SerializeField] private int MaxHp, CurrentHp;
        public bool IsBot, jetpackMode;
        private MoveDirection _touchDirection = MoveDirection.None;
        private bool _holdJump, _isJetpackingLeft, _isJetpackingRight;
        private AIBrain brain;
        private static PlayerController _instance;

        public bool IsDead
        {
            get
            {
                return CurrentHp <= 0;
            }
        }

        public static PlayerController Instance => _instance;

        private void Awake()
        {
            if (IsBot)
            {
                return;
            }
            
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }

            _instance = this;
        }

        private void Start()
        {
            Movement.Init(this);
        }

        public void Init(BrainStateData brainData)
        {
            brain.Init(this, brainData);
            brain.ActiveBrain("Jump");
        }


        private bool CanControl()
        {
            return GamePlayController.Instance.state == GameState.Playing && !IsDie();
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


            if (Input.GetKey(KeyCode.LeftArrow))
                _touchDirection = MoveDirection.Left;
            if (Input.GetKey(KeyCode.RightArrow))
                _touchDirection = MoveDirection.Right;
            if (Input.GetKey(KeyCode.UpArrow))
                _holdJump = true;
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

        #region AI Action
        
        public void JumpStart()
        {
            _holdJump = true;
        }
        
        
        
        #endregion
        
        
        public void CancelAllMove()
        {
            _touchDirection = MoveDirection.None;
            _holdJump = false;
        }
    }
}

