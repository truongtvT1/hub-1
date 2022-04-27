using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace MiniGame.StickRun
{
    public class BladeFlyNinja : Ninja
    {
        [SpineAnimation(dataField = nameof(anim))]
        public string flyBackAnim;

        protected override void Idle()
        {   
            onStartIdle?.Invoke();
            if (target)
            {
                currentEntry = anim.state.SetAnimation(0, flyBackAnim, false);
                tweener = target.DOLocalMove(start.localPosition, moveSpeed)
                    .SetSpeedBased(true)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() => { })
                    .SetUpdate(UpdateType.Normal, false)
                    .OnComplete(() =>
                    {
                        PlayIdle();
                    })
                    .Play();
            }
        }
        
        
    }
}