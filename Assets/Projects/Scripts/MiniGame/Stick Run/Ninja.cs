using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGame.StickRun
{
    public class Ninja : MonoBehaviour
    {
        public SkeletonAnimation anim;
        [SerializeField, SpineAnimation(dataField = nameof(anim))]
        public string attackAnim;
        [SerializeField, SpineAnimation(dataField = nameof(anim))]
        public string idleAnim;
        
        public float attackDelay, idleDelay, moveSpeed, idleDuration;
        public Transform target, start, end;
        public LineRenderer line;
        public UnityEvent onStartAttack, onAttackComplete, onStartIdle, onIdleComplete;
        protected TrackEntry currentEntry;
        protected Tweener tweener;
        protected List<Vector3> listPoints = new List<Vector3>();
        protected bool isAttacking, isIdling;

        private void Awake()
        {
            if (line)
            {
                listPoints.Add(start.localPosition);
                listPoints.Add(end.localPosition);
            }
        }

        protected virtual void Start()
        {
            StartCoroutine(IdleCoroutine());
        }

        public void PlayAttack(bool loop = true, Action callback = null)
        {
            currentEntry = anim.state.SetAnimation(0, attackAnim, loop);
            currentEntry.Complete += entry =>
            {
                callback?.Invoke();
            };
        }

        public void PlayIdle(bool loop = true, Action callback = null)
        {
            currentEntry = anim.state.SetAnimation(0, idleAnim, loop);
            currentEntry.Complete += entry =>
            {
                callback?.Invoke();
            };
        }

        protected virtual void Attack()
        {
            onStartAttack?.Invoke();
            if (target)
            {
                PlayAttack(false);
                tweener = target.DOLocalMove(end.localPosition, moveSpeed)
                    .SetSpeedBased(true)
                    .SetEase(Ease.InSine)
                    .OnUpdate(() =>
                    {
                        
                    })
                    .OnComplete(() =>
                    {
                        isAttacking = false;
                    })
                    .SetUpdate(UpdateType.Normal,false)
                    .Play();
            }
        }

        protected virtual void Idle()
        {
            onStartIdle?.Invoke();
            if (target)
            {
                PlayIdle();
                tweener = target.DOLocalMove(start.localPosition, moveSpeed)
                    .SetSpeedBased(true)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() => { })
                    .SetUpdate(UpdateType.Normal, false)
                    .Play();
            }
        }
        
        IEnumerator AttackCoroutine()
        {
            yield return new WaitForSeconds(attackDelay);
            isAttacking = true;
            isIdling = false;
            Attack();
            yield return new WaitUntil(() => !isAttacking);
            onAttackComplete?.Invoke();
            StartCoroutine(IdleCoroutine());
        }

        IEnumerator IdleCoroutine()
        {
            yield return new WaitForSeconds(idleDelay);
            isAttacking = false;
            isIdling = true;
            Idle();
            yield return new WaitForSeconds(idleDuration);
            onIdleComplete?.Invoke();
            StartCoroutine(AttackCoroutine());
        }

        protected void Update()
        {
            if (line)
            {
                listPoints[0] = start.position;
                listPoints[1] = target.position;
                line.SetPositions(listPoints.ToArray());
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                if (target)
                {
                    target.transform.localPosition = end.transform.localPosition;
                }
            }
        }
    }
}