using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace MiniGame.StickRun
{
    public class WaterTrap : MonoBehaviour
    {
        public float maxStayTime = 1.5f;
        public Vector2 boxCastSize;
        public Pool handPool;
        public LayerMask layerToCast;
        public DamageType damageType;
        public Transform checkPoint;
        private TrackEntry track;
        private float timeStay, duration;
        private bool isStayInTrigger, isSprinting;
        private List<StickmanPlayerController> listPlayer = new List<StickmanPlayerController>();
        
        private void Start()
        {
            isStayInTrigger = false;
            isSprinting = false;
        }

        protected bool BoxCast()
        {
            var hit = Physics2D.BoxCastAll(transform.position, boxCastSize, 0, Vector2.up, layerToCast);
            if (hit.Length != 0)
            {
                listPlayer = new List<StickmanPlayerController>(hit.Length);
                for (int i = 0; i < hit.Length; i++)
                {
                    var player = hit[i].collider.GetComponent<StickmanPlayerController>();
                    if (player != null)
                    {
                        listPlayer.Add(player);
                    }
                }
                return true;
            }
            return false;
        }
        
        private void Update()
        {

            if (BoxCast())
            {
                isStayInTrigger = true;
                if (listPlayer.Count == 1)
                {
                    if (!listPlayer[0].isSprinting)
                    {
                        // listPlayer[0].isInSpecialTrap = true;
                        isSprinting = false;
                    }
                    else if (listPlayer[0].isSprinting)
                    {
                        // listPlayer[0].isInSpecialTrap = false;
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
                            // player.isInSpecialTrap = true;
                            countSprinting++;
                        }
                        else if (player.isSprinting)
                        {
                            // player.isInSpecialTrap = false;
                            countNotSprinting++;
                        }
                    }

                    if (countSprinting > countNotSprinting)
                    {
                        Debug.Log("sprinting");
                        isSprinting = true;
                    }
                    else
                    {
                        Debug.Log("not sprinting");
                        isSprinting = false;
                    }
                }
            }
            else
            {
                isStayInTrigger = false;
                isSprinting = false;
            }
            
            if (isStayInTrigger)
            {
                if (!isSprinting)
                {
                    timeStay += Time.deltaTime;
                    if (timeStay >= maxStayTime)
                    {
                        for (int i = 0; i < listPlayer.Count; i++)
                        {
                            var player = listPlayer[i];
                            if (!player.isDead && !player.isSprinting)
                            {
                                Kill(player);
                            }
                        }
                        timeStay = 0;
                        isStayInTrigger = false;
                        isSprinting = false;
                        listPlayer = new List<StickmanPlayerController>();
                    }
                }
                else
                {
                    timeStay -= Time.deltaTime;
                }
            }
        }

        void Kill(StickmanPlayerController player)
        {
            var hand = handPool.nextThing;
            hand.transform.position = player.transform.position + new Vector3(.3f, 0);
            var anim = hand.GetComponent<SkeletonAnimation>();
            anim.Initialize(true);
            var track = anim.state.SetAnimation(0, "catch", false);
            track.Complete += entry =>
            {
                hand.SetActive(false);
            };
            track.Event += (entry1, event1) =>
            {
                if (event1.ToString().Equals("catch"))
                {
                    player.Die(damageType, checkPoint, player.transform.position + new Vector3(0, -4f));
                }
            };
        }        

        
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(transform.position,boxCastSize);
        }
    }
}