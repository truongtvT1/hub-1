using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RavingBots.Water2D;
using Spine;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniGame.StickRun
{
    public class WaterTrap : MonoBehaviour
    {
        public float maxStayTime = 1.5f;
        public Vector2 boxCastSize;
        public Pool handPool;
        public LayerMask layerToCast;
        public DamageType damageType = DamageType.Water;
        public Transform checkPoint;
        public Water2DSplashFX splashFXPrefab;
        public float SplashFXOffset = 0.2f;
        public AudioClip[] SplashFXSounds;
        public float SplashFXPowerToVolume = 1;
        public float SplashFXPowerToPitch = 1;
        private Water2DSplashFX[] _splashCache;
        private int _splash;
        private TrackEntry track;
        private float timeStay, duration;
        private bool isStayInTrigger, isSprinting;
        private List<StickmanPlayerController> listPlayer = new List<StickmanPlayerController>();
        
        private void Awake()
        {
            _splashCache = new Water2DSplashFX[10];
            var container = new GameObject("Splash Container").transform;
            container.transform.position = Vector3.zero;
            for (var i = 0; i < _splashCache.Length; i++)
            {
                var splash = Instantiate(splashFXPrefab);
                splash.transform.parent = container;
                _splashCache[i] = splash;
            }
        }
        
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
            }
            
            if (isStayInTrigger)
            {
                if (!isSprinting)
                {
                    timeStay += Time.deltaTime;
                }
                else
                {
                    if (timeStay >= 0)
                    timeStay -= Time.deltaTime;
                }
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
                    var splash = _splashCache[_splash];
                    splash.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - SplashFXOffset,100);
                    splash.Play(2.5f, SplashFXSounds[Random.Range(0, SplashFXSounds.Length)], SplashFXPowerToVolume, SplashFXPowerToPitch / 5);
                    _splash = (_splash + 1) % _splashCache.Length;
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