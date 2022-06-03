using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using Projects.Scripts.Hub;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace MiniGame.Steal_Ball
{
    public class PlayerStealBallController : MonoBehaviour
    {
        [SerializeField] private Transform hand;

        [SerializeField, ShowIf(nameof(IsBot))]
        private Ball ballTarget;

        [ShowIf(nameof(IsBot))] public float minTimeToThink, maxTimeToThink;
        [SerializeField] private bool IsBot;
        [SerializeField] private CharacterAnimation anim;
        [SerializeField] private GameObject display;
        [SerializeField, Range(0, 0.99f)] private float Smoothing = 0.25f;
        [SerializeField] private float TargetLerpSpeed = 1;
        [SerializeField] private List<Ball> listBall = new List<Ball>();
        [SerializeField] private bool isHoldingBall, isMoving, isReachTarget, isForcedMove;

        private AIBrain brain;
        private NavMeshAgent agent;
        private Vector3 TargetDirection;
        private float LerpTime = 0;
        private Vector3 LastDirection;
        private Vector3 MovementVector, JoyStickVector;
        private Vector3 driftPos;
        private BallNest ballNest;
        [SerializeField] private Ball ballOnHand;
        private RankIngame rankInfo;
        private const float minEasyTime = 1f;
        private const float minNormalTime = 0.5f;
        private const float minHardTime = 0.2f;
        private const float minHellTime = 0f;
        private const float maxEasyTime = 1.5f;
        private const float maxNormalTime = 1f;
        private const float maxHardTime = 0.7f;
        private const float maxHellTime = 0.5f;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateUpAxis = false;
            agent.updateRotation = false;
            brain = GetComponentInChildren<AIBrain>();
        }

        private void Start()
        {
            var temp = GameObject.FindGameObjectsWithTag("Ball");
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].TryGetComponent(out Ball b))
                {
                    listBall.Add(b);
                }
            }
        }

        private void Update()
        {
            if (!IsBot)
            {
                if (GamePlayController.Instance.state == GameState.End)
                {
                    anim.PlayIdle();
                    return;
                }

                #region Movement

                if (isForcedMove)
                {
                    return;
                }
#if UNITY_EDITOR
                MovementVector = Vector3.zero;
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    MovementVector += Vector3.up;
                }

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    MovementVector += Vector3.left;
                    display.transform.localScale = new Vector3(-1, 1, 1);
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    MovementVector += Vector3.right;
                    display.transform.localScale = Vector3.one;
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    MovementVector += Vector3.down;
                }
                MovementVector.Normalize();
#else
                MovementVector = Vector3.zero;
                if (JoyStickVector.x < 0)
                {
                    MovementVector += Vector3.left;
                    display.transform.localScale = new Vector3(-1, 1, 1);
                }
                if (JoyStickVector.x > 0)
                {
                    MovementVector += Vector3.right;
                    display.transform.localScale = Vector3.one;
                }

                if (JoyStickVector.y < 0)
                {
                    MovementVector += Vector3.down;
                }
                if (JoyStickVector.y > 0)
                {
                    MovementVector += Vector3.up;
                }
                
                MovementVector.Normalize();
#endif
                if (MovementVector != LastDirection)
                {
                    LerpTime = 0;
                }

                LastDirection = MovementVector;
                var agentDrift = 0.0001f; // minimal
                driftPos = agentDrift * Random.insideUnitCircle;
                MovementVector += driftPos;
                TargetDirection = Vector3.Lerp(TargetDirection
                    , MovementVector
                    ,LerpTime * TargetLerpSpeed * (1 - Smoothing));
                agent.Move(TargetDirection * agent.speed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
                LerpTime += Time.deltaTime;
                #endregion

                var ball = CheckReachBall();
                if (!isHoldingBall && ball)
                {
                    TakeBall(ball);
                }

                if (isHoldingBall && CheckReachTarget())
                {
                    ReleaseBall();
                }

                if (MovementVector != driftPos)
                {
                    anim.PlayRun();
                }
                else
                {
                    anim.PlayIdle();
                }
            }
            else
            {
                if (isForcedMove)
                {
                    return;
                }

                // Update Z
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);

                if (GamePlayController.Instance.state == GameState.End)
                {
                    brain.ResetBrain();
                    brain.DeActiveBrain();
                    agent.ResetPath();
                }

                if (agent.hasPath)
                {
                    anim.PlayRun();
                }
                else
                {
                    anim.PlayIdle();
                }
            }
        }


        public void Init(List<string> skin, Color color, BallNest ballCollection,int priority,
            BotDifficulty difficulty = BotDifficulty.Easy, BrainStateData brainData = null)
        {
            anim.SetSkin(skin);
            anim.SetSkinColor(color);
            ballNest = ballCollection;
            ballNest.Init(this);
            agent.avoidancePriority -= 5 * priority;
            if (IsBot)
            {
                brain.InitSteal(this, brainData);
                brain.ResetBrain();
                brain.ActiveBrain();
                agent.autoBraking = true;
                switch (difficulty)
                {
                    case BotDifficulty.Easy:
                        minTimeToThink = minEasyTime;
                        maxTimeToThink = maxEasyTime;
                        break;
                    case BotDifficulty.Normal:
                        agent.autoBraking = true;
                        minTimeToThink = minNormalTime;
                        maxTimeToThink = maxNormalTime;
                        break;
                    case BotDifficulty.Hard:
                        agent.autoBraking = true;
                        minTimeToThink = minHardTime;
                        maxTimeToThink = maxHardTime;
                        break;
                    case BotDifficulty.Hell:
                        agent.autoBraking = true;
                        minTimeToThink = minHellTime;
                        maxTimeToThink = maxHellTime;
                        break;
                }
            }

            StealBallGameController.onDropBall += ball =>
            {
                if (!listBall.Contains(ball))
                {
                    listBall.Add(ball);
                }
            };
        }

        [Button]
        public void AddForce(Vector3 force)
        {
            StartCoroutine(Force(force));
        }

        IEnumerator Force(Vector3 force)
        {
            isForcedMove = true;
            var cachePos = transform.position;
            var targetPos = Vector3.zero;
            float lerpTime = 0;
            float agentDrift = 0.0001f; // minimal
            driftPos = agentDrift * Random.insideUnitCircle;
            var distance = Vector3.SqrMagnitude(transform.position - (cachePos + force + driftPos));
            while (distance > agent.stoppingDistance)
            {
                targetPos = Vector3.Lerp(transform.position,
                    cachePos + force + driftPos,
                    Mathf.Clamp01(lerpTime * TargetLerpSpeed * (1 - Smoothing)));
                if (agent.Raycast(targetPos, out NavMeshHit hit))
                {
                    //check va chạm với Not Walkable Area
                    if (hit.mask == 0)
                    {
                        break;
                    }
                }

                agent.Warp(targetPos);
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
                lerpTime += Time.deltaTime;
                distance = Vector3.SqrMagnitude(transform.position - (cachePos + force + driftPos));
                yield return null;
            }

            isForcedMove = false;
        }

        Ball CheckReachBall()
        {
            for (int i = 0; i < listBall.Count; i++)
            {
                if (listBall[i].state == Ball.State.Unavailable || ballNest.IsBallInNest(listBall[i]))
                {
                    continue;
                }

                var distance = Vector3.SqrMagnitude(transform.position - (listBall[i].transform.position + driftPos));
                if (distance <= agent.stoppingDistance * 3)
                {
                    return listBall[i];
                }
            }

            return null;
        }

        public void Idle()
        {
            agent.ResetPath();
        }

        public void TakeBall(Ball ball)
        {
            isHoldingBall = true;
            ballOnHand = ball;
            anim.PlayStealBall();
            ball.transform.SetParent(hand);
            ball.Take();
            ball.transform.localPosition = new Vector3(0.05f, -0.7f);
            SetBallTarget(null);
        }

        public void ReleaseBall()
        {
            //release ball
            anim.ClearTrack(1);
            ballOnHand.Release();
            ballNest.OnBallRelease(ballOnHand);
            ballOnHand = null;
            Score(1);
            isHoldingBall = false;
        }

        public Ball GetTargetBall()
        {
            return ballTarget;
        }

        public void SetBallTarget(Ball ball)
        {
            ballTarget = ball;
        }

        public BallNest GetBallNest()
        {
            return ballNest;
        }

        void GetBallToSetTarget()
        {
            float minDistance = 1000;
            int index = -1;
            for (int i = 0; i < listBall.Count; i++)
            {
                if (listBall[i].state == Ball.State.Unavailable || ballNest.IsBallInNest(listBall[i]))
                {
                    continue;
                }

                var distance = Vector3.SqrMagnitude(listBall[i].transform.position - transform.position);
                if (minDistance >= distance)
                {
                    index = i;
                    minDistance = distance;
                }
            }
            if (index != -1)
            {
                SetBallTarget(listBall[index]);
            }
        }

        public void GetOtherBallToSetTarget()
        {
            float minDistance = 1000;
            int index = -1;
            for (int i = 0; i < listBall.Count; i++)
            {
                if (ballTarget == listBall[i])
                {
                    continue;
                }
                if (listBall[i].state == Ball.State.Unavailable || ballNest.IsBallInNest(listBall[i]))
                {
                    continue;
                }
                var distance = Vector3.SqrMagnitude(listBall[i].transform.position - transform.position);
                if (minDistance >= distance)
                {
                    index = i;
                    minDistance = distance;
                }
            }
            if (index != -1)
            {
                SetBallTarget(listBall[index]);
            }
        }
        
        public bool HasBallToTarget()
        {
            GetBallToSetTarget();
            return ballTarget != null;
        }

        public bool IsHoldingBall => isHoldingBall;

        public void SetDestination(Transform target)
        {
            var agentDrift = 0.0001f; // minimal
            driftPos = agentDrift * Random.insideUnitCircle;
            if (target.position.x > transform.position.x)
            {
                display.transform.localScale = Vector3.one;
            }
            else
            {
                display.transform.localScale = new Vector3(-1, 1, 1);
            }

            agent.SetDestination(target.position + driftPos);
        }

        public bool CheckReachTarget()
        {
            float distance = 1000;
            if (!ballTarget)
            {
                distance = Vector3.SqrMagnitude(transform.position - (ballNest.transform.position + driftPos));
                return distance <= agent.stoppingDistance * 10;
            }

            distance = Vector3.SqrMagnitude(transform.position - (ballTarget.transform.position + driftPos));
            return distance <= agent.stoppingDistance * 3;
        }

        public void OnBallStolen()
        {
            Score(-1);
        }

        public void InitRank(RankIngame rankInfo)
        {
            this.rankInfo = rankInfo;
        }

        public int GetRank()
        {
            return rankInfo.rank;
        }

        public void Score(int score)
        {
            rankInfo.score += score;
            if (LeaderBoardInGame.Instance != null)
            {
                LeaderBoardInGame.scoreAction.Invoke(rankInfo);
            }
        }

        #region Controller

        public void TouchMove(Vector3 moveVector, bool isRelease = false)
        {
            if (isRelease)
            {
                JoyStickVector = Vector3.zero;
            }
            else
            {
                JoyStickVector = moveVector;
            }
        }

        #endregion
    }
}