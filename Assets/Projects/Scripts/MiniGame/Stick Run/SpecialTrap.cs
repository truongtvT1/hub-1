using System;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace MiniGame.StickRun
{
    public class SpecialTrap : MonoBehaviour
    {
        public float maxStayDuration = 1.5f, timeStay;
        public SkeletonAnimation anim;
        [SpineAnimation(dataField = nameof(anim))]
        public string attackAnim;
        [SpineAnimation(dataField = nameof(anim))]
        public string idleAnim;
        public DamageType damageType;
        public Transform diePos;
        public Transform checkPoint;
        protected TrackEntry track;
        protected float duration;
        protected bool isStayInTrigger, isSprinting;

        protected virtual void Start()
        {
            ResetTrap();
        }

        protected virtual void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<StickmanPlayerController>();
                if (player && !player.isDead)
                {
                    if (timeStay >= maxStayDuration)
                    {
                        Kill(player);
                        ResetTrap();
                        return;
                    }
                    isStayInTrigger = true;
                    if (!player.isSprinting)
                    {
                        player.isInSpecialTrap = true;
                        isSprinting = false;
                    }
                    else if (player.isSprinting && timeStay != 0)
                    {
                        isSprinting = true;
                        player.isInSpecialTrap = false;
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<StickmanPlayerController>();
                if (player)
                {
                    player.isInSpecialTrap = false;
                    ResetTrap();
                }
            }
        }

        protected virtual void ResetTrap()
        {
            isStayInTrigger = false;
            isSprinting = false;
            anim.state.SetAnimation(0, idleAnim, false)
                .Complete += entry =>
            {
                track = anim.state.SetAnimation(0, attackAnim, false);
                track.TrackTime = 0;
                track.TimeScale = 0;
                duration = track.Animation.Duration;
                timeStay = 0;
            };
        }

        protected virtual void Update()
        {
            if (isStayInTrigger)
            {
                if (!isSprinting)
                {
                    track.TimeScale = anim.timeScale;
                    timeStay += Time.deltaTime;
                    if (timeStay >= maxStayDuration)
                    {
                        anim.state.Update(track.TrackEnd);
                        return;
                    }
                    track.TrackTime = (timeStay * duration/maxStayDuration) * anim.timeScale;
                    anim.state.Update(track.TrackTime);
                }
                else
                {
                    timeStay -= Time.deltaTime;
                    track.TrackTime = timeStay * duration/maxStayDuration * anim.timeScale;
                    anim.state.Update(track.TrackTime);
                }
            }
        }

        protected virtual void Kill(StickmanPlayerController player)
        {
            player.Die(damageType,checkPoint,diePos);
            // await Task.Delay(500);
            // anim.state.SetAnimation(0, idleAnim, false);
        }
    }
}