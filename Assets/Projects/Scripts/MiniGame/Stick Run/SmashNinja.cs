using Spine.Unity;
using UnityEngine;

namespace MiniGame.StickRun
{
    public class SmashNinja : Ninja
    {
        [SpineAnimation(dataField = nameof(anim))]
        public string prepareAttack;

        protected override void Attack()
        {
            base.Attack();
            currentEntry = anim.state.SetAnimation(0, prepareAttack, false);
            currentEntry.Complete += entry =>
            {
                
                PlayAttack(false,() =>
                {
                    isAttacking = false;
                });
            };
        }

        protected override void Idle()
        {
            base.Idle();
            PlayIdle();
        }
    }
}