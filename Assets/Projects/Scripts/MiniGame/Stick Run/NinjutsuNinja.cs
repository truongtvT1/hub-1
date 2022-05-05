using System.Collections;
using System.Threading.Tasks;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGame.StickRun
{
    public class NinjutsuNinja : MonoBehaviour
    {
        public SkeletonAnimation ninja;
        public SkeletonAnimation plant;
        [SpineAnimation(dataField = nameof(ninja))] public string ninjaIdle,ninjaCastSpell;
        [SpineAnimation(dataField = nameof(plant))] public string plantAttack;
        public float attackDelay, idleDelay, idleDuration, attackDuration;
        public bool isAttacking;
        public GameObject damageObj;
        public UnityEvent onStartAttack, onAttackComplete, onStartIdle, onIdleComplete;

        protected virtual void Start()
        {
            StartCoroutine(IdleCoroutine());
        }
        
        async void Attack()
        {
            ninja.state.SetAnimation(1, ninjaCastSpell, false).Complete += async entry =>
            {
                plant.state.SetAnimation(0, plantAttack, false);
                await Task.Delay(100);
                damageObj.SetActive(true);
            };
            await Task.Delay((int)attackDuration/2 * 1000);            
            ninja.state.SetEmptyAnimation(1,.02f);
            ninja.state.SetAnimation(1, ninjaIdle, true);
            damageObj.SetActive(false);
            isAttacking = false;
        }

        
        IEnumerator AttackCoroutine()
        {
            yield return new WaitForSeconds(attackDelay);
            isAttacking = true;
            Attack();
            yield return new WaitUntil(() => !isAttacking);
            onAttackComplete?.Invoke();
            StartCoroutine(IdleCoroutine());
        }

        IEnumerator IdleCoroutine()
        {
            yield return new WaitForSeconds(idleDelay);
            isAttacking = false;
            yield return new WaitForSeconds(idleDuration);
            onIdleComplete?.Invoke();
            StartCoroutine(AttackCoroutine());
        }

    }
}