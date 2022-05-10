using System;
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

        private void Awake()
        {
            bladeGhost = transform.GetChild(0).GetChild(1).gameObject.GetComponent<SkeletonGhost>();
        }

        protected override void Start()
        {
            bladeGhost.additive = false;
            bladeGhost.spawnInterval = -0.1f;
            bladeGhost.color = new Color32(255, 0, 10, 90);
            bladeGhost.maximumGhosts = 15;
            bladeGhost.fadeSpeed = 10;
            bladeGhost.ghostingEnabled = false;
            base.Start();
        }

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