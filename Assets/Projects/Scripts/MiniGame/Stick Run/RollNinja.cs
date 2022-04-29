using System;
using System.Threading.Tasks;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace MiniGame.StickRun
{
    public class RollNinja : Ninja
    {
        public Animator anim1, anim2;

        [SerializeField, SpineAnimation(dataField = nameof(targetAnim))]
        private string rollAnim;
        public SkeletonAnimation targetAnim;
        async void PlayAttack(Action callback = null)
        {
            anim1.Play("attack");
            anim2.Play("attack");
            var length = anim1.GetCurrentAnimatorStateInfo(0).length;
            await Task.Delay((int) length * 1000);
            callback?.Invoke();
        }
        
        async void PlayIdle(Action callback = null)
        {
            anim1.Play("idle");
            anim2.Play("idle");
            var length = anim1.GetCurrentAnimatorStateInfo(0).length;
            await Task.Delay((int) length * 1000);
            callback?.Invoke();
        }

        protected override void Attack()
        {
            onStartAttack?.Invoke();
            PlayAttack(() =>
            {
                PlayIdle();
            });
            targetAnim.state.SetAnimation(0, rollAnim, true);
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

        protected override void Idle()
        {
            target.localPosition = start.localPosition;
            targetAnim.state.ClearTrack(0);
        }
    }
}