using System;
using System.Threading.Tasks;
using Projects.Scripts.Hub;
using ThirdParties.Truongtv;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace MiniGame.Steal_Ball
{
    public class PlayerStealBallController : MonoBehaviour
    {
        public Transform target, hand;
        public CharacterAnimation anim;
        public GameObject display;
        private NavMeshAgent agent;
        private void Start()
        {
            var skin = GameDataManager.Instance.GetSkinInGame();
            var color = GameDataManager.Instance.GetCurrentColor();
            anim.SetSkin(skin);
            anim.SetSkinColor(color);
            agent = GetComponent<NavMeshAgent>();
            agent.updateUpAxis = false;
            agent.updateRotation = false;
            agent.autoBraking = true;
            SetDestination(target);
        }
        void SetDestination(Transform target)
        {
            var agentDrift = 0.0001f; // minimal
            var driftPos = target.position + (Vector3)(agentDrift * Random.insideUnitCircle);
            if (target.position.x > transform.position.x)
            {
                display.transform.localScale = Vector3.one;
            }
            else
            {
                display.transform.localScale = new Vector3(-1, 1, 1);
            }
            agent.SetDestination(driftPos);
        }
        
        private void Update()
        {
            CheckHit();
            if (agent.hasPath)
            {
                anim.PlayRun();
            } 
            else 
            {
                anim.PlayIdle();
            }
        }

        private void CheckHit()
        {
            if (agent.Raycast(target.position,out NavMeshHit hit))
            {
                if (hit.distance == 0)
                {
                    anim.PlayStealBall();
                    target.SetParent(hand);
                    target.localPosition = Vector3.zero;
                }
            }
        }
    }
}