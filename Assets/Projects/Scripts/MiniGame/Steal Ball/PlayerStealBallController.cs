using System;
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
        [SerializeField] private Transform target, hand;
        [SerializeField] private CharacterAnimation anim;
        [SerializeField] private GameObject display;
        [SerializeField] private bool IsBot;
        [SerializeField,Range(0, 0.99f)] private float Smoothing = 0.25f;
        [SerializeField] private float TargetLerpSpeed = 1;
        [SerializeField] private List<Ball> listBall = new List<Ball>();
        [SerializeField] private bool isHoldingBall, isMoving, isReachTarget, isForcedMove;
        
        private AIBrain brain;
        private NavMeshAgent agent;
        private Vector3 TargetDirection;
        private float LerpTime = 0;
        private Vector3 LastDirection;
        private Vector3 MovementVector;
        private Vector3 driftPos;
        private BallNest ballNest;
        [SerializeField] private Ball ballOnHand;
        private RankIngame rankInfo;
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateUpAxis = false;
            agent.updateRotation = false;
            brain = GetComponentInChildren<AIBrain>();
            // agent.autoBraking = true;
        }

        [Button]
        public void Test(Vector3 force)
        {
            StartCoroutine(Force(force));
        }

        IEnumerator Force(Vector3 force)
        {
            isForcedMove = true;
            var cachePos = transform.position;
            var targetPos = cachePos;
            float lerpTime = 0;
            var agentDrift = 0.0001f; // minimal
            driftPos = agentDrift * Random.insideUnitCircle;
            Debug.Log($"stopping distance {agent.stoppingDistance}");
            while (Vector3.SqrMagnitude(agent.transform.position - (cachePos + force + driftPos)) > agent.stoppingDistance)
            {
                targetPos = Vector3.Lerp(transform.position,
                    cachePos + force + driftPos,
                    Mathf.Clamp01(lerpTime * TargetLerpSpeed * (1 - Smoothing)));
                agent.Warp(targetPos);
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
                lerpTime += Time.deltaTime;
                Debug.Log($"distance {Vector3.SqrMagnitude(agent.transform.position - (cachePos + force + driftPos))}");
                yield return null;
            }
            isForcedMove = false;
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
        
        public void Init(List<string> skin, Color color, BallNest ballCollection, BrainStateData brainData = null)
        {
            anim.SetSkin(skin);
            anim.SetSkinColor(color);
            ballNest = ballCollection;
            ballNest.Init(this);
            if (IsBot)
            {
                brain.InitSteal(this, brainData);
                brain.ResetBrain();
                brain.ActiveBrain();
            }
        }

        void SetDestination(Transform target)
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

        Ball CheckReachBall()
        {
            for (int i = 0; i < listBall.Count; i++)
            {
                if (listBall[i].state == Ball.State.Unavailable || ballNest.IsBallInNest(listBall[i]))
                {
                    continue;
                }
                
                if (Vector3.SqrMagnitude(transform.position - (listBall[i].transform.position + driftPos)) 
                    <= agent.stoppingDistance * 3)
                {
                    return listBall[i];
                }
            }
            return null;
        }

        bool CheckReachBallNest()
        {
            return Vector3.SqrMagnitude(transform.position - (ballNest.transform.position + driftPos)) <=
                    agent.stoppingDistance * 5;
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

        void MoveTo(Vector3 offset)
        {
            
        }
        
        private void Update()
        {
            if (!IsBot)
            {
                #region Movement

                if (isForcedMove)
                {
                    return;
                }
                
                MovementVector = Vector3.zero;

                if (Input.GetKey(KeyCode.UpArrow))
                {
                    MovementVector += Vector3.up;
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    MovementVector += Vector3.left;
                    display.transform.localScale = new Vector3(-1,1,1);
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
                if (MovementVector != LastDirection)
                {
                    LerpTime = 0;
                }
                LastDirection = MovementVector;
                var agentDrift = 0.0001f; // minimal
                driftPos = agentDrift * Random.insideUnitCircle;
                MovementVector += driftPos;
                TargetDirection = Vector3.Lerp(
                    TargetDirection, 
                    MovementVector,  
                    Mathf.Clamp01(LerpTime * TargetLerpSpeed * (1 - Smoothing))
                );
                agent.Move(TargetDirection * agent.speed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.y);
                LerpTime += Time.deltaTime;
                #endregion
                
                var ball = CheckReachBall();
                if (!isHoldingBall && ball)
                {
                    isHoldingBall = true;
                    ballOnHand = ball;
                    anim.PlayStealBall();
                    ball.transform.SetParent(hand);
                    ball.Take();
                    ball.transform.localPosition = new Vector3(0.05f, -0.7f);
                }

                if (isHoldingBall && CheckReachBallNest())
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

        private void CheckHit()
        {
            if (!target)
            {
                return;
            }

            if (Vector3.SqrMagnitude(transform.position - (target.position + driftPos)) <= agent.stoppingDistance * 2)
            {
                anim.PlayStealBall();
                target.SetParent(hand);
                target.GetComponent<Ball>().Take();
                target.localPosition = new Vector3(0.05f, -0.7f);
            }
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
    }
}