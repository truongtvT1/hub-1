using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Projects.Scripts.Hub;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniGame.SquidGame
{
    public class SquidPlayer : MonoBehaviour
    {
        private SquidGameController _controller;
        public bool isReady, isMoving, isDead, isWin;
        public Transform start, end;
        public ParticleSystem blood;
        public CharacterAnimation anim;
        public Sprite[] skullSprites;
        public SpriteRenderer skull;
        public float moveSpeed;
        private Coroutine _coroutineRed,_coroutineGreen;
        private float difficulty, delayGreen = .1f, delayRed = .1f;
        private float rdValue;
        public void Init(SquidGameController controller, List<string> skin, Color color, float difficulty)
        {
            isReady = false;
            isWin = false;
            this.difficulty = difficulty;
            rdValue = Random.value;
            anim.SetSkin(skin);
            anim.SetSkinColor(color);
            skull.sortingOrder = anim.GetSortingOrder();
            skull.sprite = skullSprites[Random.Range(0, skullSprites.Length)];
            skull.gameObject.SetActive(false);
            var distance = Random.Range(1f, 3f);
            anim.transform.parent.transform.position = start.position + new Vector3(distance, 0);
            _controller = controller;
            _controller.onRedLight += () =>
            {
                if (!isDead && _controller.state != GameState.End)
                {
                    _coroutineRed = StartCoroutine(RedLight());
                }
            };
            _controller.onGreenLight += () =>
            {
                if (!isDead && _controller.state != GameState.End)
                {
                    _coroutineGreen = StartCoroutine(GreenLight());
                }
            };
            anim.transform.parent.DOMoveX(start.position.x, moveSpeed)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnStart(() =>
                {
                    anim.PlayRun();
                })
                .OnComplete(() =>
                {
                    isReady = true;
                    anim.PlayIdle();
                });
            anim.transform.parent.DOMoveX(end.position.x, moveSpeed)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnKill(() =>
                {
                    isMoving = false;
                })
                .OnComplete(() =>
                {
                    anim.PlayWin();
                    anim.PlayWin(trackIndex:1);
                    // SoundInGameManager.Instance.PlayBallWinSound();
                })
                .Pause();
        }

        private bool tempMoving = false;

        private void Update()
        {
            if (isReady)
            {
                if (HitFinishLine() && !isWin)
                {
                    isWin = true;
                    _controller.playerRank++;
                }

                if (!isWin && HitByMeteor())
                {
                    Kill();
                }
            }
        }

        bool HitByMeteor()
        {
            RaycastHit2D hit = Physics2D.Raycast(anim.transform.position + new Vector3(0,.1f), Vector2.left, .2f,1 << 9);
            if (hit)
            {
                return hit;    
            }

            return false;
        }
        
        bool HitFinishLine()
        {
            RaycastHit2D hit = Physics2D.Raycast(anim.transform.position, Vector2.left, .2f,1 << 0);
            if (hit)
            {
                return hit;    
            }

            return false;
        }
        
        public void RunOrWalk()
        {
            
        }
        
        IEnumerator RedLight()
        {
            if (isWin || isDead)
            {
                yield break;
            }
            var rd = Random.Range(0, delayRed - Random.Range(0,difficulty));
            yield return new WaitForSeconds(rd);
            if (_coroutineGreen != null)
            {
                StopCoroutine(_coroutineGreen);
            }
            isMoving = false;
            anim.transform.parent.DOPause();
            anim.PlayStopPose();
            anim.PlayStopPose(trackIndex:1);
        }

        IEnumerator GreenLight()
        {
            if (isWin || isDead)
            {
                yield break;
            }
            var rd = Random.Range(0, delayGreen - Random.Range(0,difficulty));
            yield return new WaitForSeconds(rd);
            if (_coroutineRed != null)
            {
                StopCoroutine(_coroutineRed);
            }
            isMoving = true;
            if (rdValue > 0.5f)
            {
                anim.PlayRun();
                if (rdValue > 0.75f)
                {
                    anim.PlayRunNaruto();
                }
                else
                {
                    anim.PlayDodge();
                }

            }
            else
            {
                anim.PlayRun();
                anim.PlayRun(trackIndex:1);
            }
            anim.transform.parent.DOTogglePause();
            
        }


        public async void Kill()
        {
            if (isDead) return;
            StopAllCoroutines();
            isDead = true;
            anim.transform.parent.DOKill();
            blood.gameObject.SetActive(true);
            blood.Play(true);
            skull.gameObject.SetActive(true);
            anim.gameObject.SetActive(false);
            await Task.Delay(500);
            blood.gameObject.SetActive(false);
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(start.position,.1f);
            Gizmos.DrawSphere(end.position,.1f);
        }
#endif
    }
}