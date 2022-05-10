using DG.Tweening;
using Spine.Unity;
using Spine.Unity.Examples;
using UnityEngine;

namespace MiniGame.StickRun
{
    public class BladeFlyNinja : Ninja
    {
        [SpineAnimation(dataField = nameof(anim))]
        public string flyBackAnim;
        public SkeletonGhost bladeGhost;
        protected override void Idle()
        {   
            onStartIdle?.Invoke();
            if (target)
            {
                currentEntry = anim.state.SetAnimation(0, flyBackAnim, false);
                tweener = target.DOLocalMove(start.localPosition, moveSpeed)
                    .SetSpeedBased(true)
                    .SetEase(moveBackEase)
                    .OnUpdate(() => { })
                    .SetUpdate(UpdateType.Normal, false)
                    .OnComplete(() =>
                    {
                        PlayIdle();
                    })
                    .Play();
            }
        }

        protected override void Attack()
        {
            onStartAttack?.Invoke();
            bladeGhost.ghostingEnabled = true;
            if (target)
            {
                PlayAttack(false);
                tweener = target.DOLocalMove(end.localPosition, moveSpeed)
                    .SetSpeedBased(true)
                    .SetEase(moveNextEase)
                    .OnUpdate(() =>
                    {
                        
                    })
                    .OnComplete(() =>
                    {
                        PlayIdle();
                        bladeGhost.ghostingEnabled = false;
                        isAttacking = false;
                    })
                    .SetUpdate(UpdateType.Normal,false)
                    .Play();
            }
        }
    }
}