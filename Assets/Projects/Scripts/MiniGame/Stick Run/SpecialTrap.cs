using System;
using System.Collections.Generic;
using DG.Tweening;
using RavingBots.Water2D;
using Spine;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniGame.StickRun
{
    public class SpecialTrap : MonoBehaviour
    {
        public float maxStayDuration = .8f;
        public SkeletonAnimation anim;
        [SpineAnimation(dataField = nameof(anim))]
        public string attackAnim;
        [SpineAnimation(dataField = nameof(anim))]
        public string idleAnim;
        public DamageType damageType;
        public Vector2 boxCastSize;
        public LayerMask layerToCast;
        public Transform diePos;
        public Transform checkPoint;
        
        protected TrackEntry track;
        protected float timeStay, duration;
        protected bool isStayInTrigger, isSprinting;
        private List<StickmanPlayerController> listPlayer = new List<StickmanPlayerController>();

        

        protected virtual void Start()
        {
            ResetTrap();
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

        protected bool BoxCast()
        {
            var hit = Physics2D.BoxCastAll(transform.position, boxCastSize, 0, Vector2.up, layerToCast);
            if (hit.Length != 0)
            {
                if (listPlayer.Count == 0)
                {
                    listPlayer = new List<StickmanPlayerController>(hit.Length);
                }
                for (int i = 0; i < hit.Length; i++)
                {
                    var player = hit[i].collider.GetComponent<StickmanPlayerController>();
                    if (player != null)
                    {
                        if (!listPlayer.Contains(player))
                        {
                            listPlayer.Add(player);
                        }
                    }
                }
                return true;
            }
            return false;
        }
        
        protected virtual void Update()
        {
            if (BoxCast())
            {
                isStayInTrigger = true;
                if (listPlayer.Count == 1)
                {
                    if (!listPlayer[0].isSprinting)
                    {
                        listPlayer[0].isInSpecialTrap = true;
                        isSprinting = false;
                    }
                    else if (listPlayer[0].isSprinting)
                    {
                        listPlayer[0].isInSpecialTrap = false;
                        isSprinting = true;
                    }
                }
                else
                {
                    int countSprinting = 0;
                    int countNotSprinting = 0;
                    for (int i = 0; i < listPlayer.Count; i++)
                    {
                        var player = listPlayer[i];
                        if (!player.isSprinting)
                        {
                            player.isInSpecialTrap = true;
                            countSprinting++;
                        }
                        else if (player.isSprinting)
                        {
                            player.isInSpecialTrap = false;
                            countNotSprinting++;
                        }
                    }

                    if (countSprinting > countNotSprinting)
                    {
                        isSprinting = true;
                    }
                    else
                    {
                        isSprinting = false;
                    }
                }
            }
            else
            {
                isStayInTrigger = false;
                isSprinting = false;
                for (int i = 0; i < listPlayer.Count; i++)
                {
                    var player = listPlayer[i];
                    player.isInSpecialTrap = false;
                }
            }
            
            if (isStayInTrigger)
            {
                if (!isSprinting)
                {
                    if (track != null)
                    {
                        track.TimeScale = anim.timeScale;
                        timeStay += Time.deltaTime;
                        if (timeStay >= maxStayDuration)
                        {
                            anim.state.Update(track.TrackEnd);
                            for (int i = 0; i < listPlayer.Count; i++)
                            {
                                var player = listPlayer[i];
                                if (!player.isDead)
                                {
                                    Kill(player);
                                }
                            }
                            ResetTrap();
                            listPlayer = new List<StickmanPlayerController>();
                            return;
                        }
                        track.TrackTime = (timeStay * duration/maxStayDuration) * anim.timeScale;
                        anim.state.Update(track.TrackTime);
                    }
                }
                else
                {
                    if (timeStay >= 0)
                    {
                        timeStay -= Time.deltaTime;
                        if (track != null)
                        {
                            track.TrackTime = timeStay * duration / maxStayDuration * anim.timeScale;
                            anim.state.Update(track.TrackTime);
                        }
                    }
                }
            }
        }

        protected virtual void Kill(StickmanPlayerController player)
        {
            player.Die(damageType,checkPoint,diePos.position,Ease.InSine);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(transform.position,boxCastSize);
        }
    }
}