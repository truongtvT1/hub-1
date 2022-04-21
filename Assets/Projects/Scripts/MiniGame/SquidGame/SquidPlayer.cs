using System.Collections;
using System.Collections.Generic;
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
        private float difficulty, delayGreen = .2f, delayRed = .2f;
        public void Init(SquidGameController controller, List<string> skin, Color color, float difficulty)
        {
            isReady = false;
            isWin = false;
            this.difficulty = difficulty;
            anim.SetSkin(skin);
            anim.SetSkinColor(color);
            skull.sortingOrder = anim.GetSortingOrder();
            skull.sprite = skullSprites[Random.Range(0, skullSprites.Length)];
            skull.gameObject.SetActive(false);
            moveSpeed = Random.value > 0.5f ? moveSpeed + Random.Range(0, difficulty/2) : moveSpeed - Random.Range(0, difficulty/2);
            moveSpeed += moveSpeed * Random.Range(0,difficulty);
            var distance = Random.Range(1f, 3f);
            anim.transform.parent.transform.position = start.position + new Vector3(distance, 0);
            _controller = controller;
            _controller.onRedLight += () =>
            {
                if (!isDead && _controller.state != GameState.End && _controller.state != GameState.Pause)
                {
                    _coroutineRed = StartCoroutine(RedLight());
                }
            };
            _controller.onGreenLight += () =>
            {
                if (!isDead && _controller.state != GameState.End && _controller.state != GameState.Pause)
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
                .OnUpdate(() =>
                {
                    if (HitFinishLine() && !isWin)
                    {
                        isWin = true;
                        _controller.playerRank++;
                    }
                    if (isMoving && _controller.state != GameState.Pause)
                    {
                        anim.PlayRun();
                    }
                })
                .OnComplete(() =>
                {
                    anim.PlayWin();
                    // SoundInGameManager.Instance.PlayBallWinSound();
                })
                .Pause();
        }

        private bool tempMoving = false;
        
        public void Pause()
        {
            tempMoving = isMoving;
            isMoving = false;
            anim.transform.parent.DOPause();
            anim.PauseAnim();
        }

        public void Resume()
        {
            isMoving = tempMoving;
            if (isMoving)
            {
                anim.PauseAnim(false);
                anim.PlayRun();
                anim.transform.parent.DOTogglePause();
            }
        }

        bool HitFinishLine()
        {
            RaycastHit2D hit = Physics2D.Raycast(anim.transform.position, Vector2.left, .2f);
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
            if (isWin)
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
        }

        IEnumerator GreenLight()
        {
            if (isWin)
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
            anim.PlayRun();
            anim.transform.parent.DOTogglePause();
            
        }

        public void Kill()
        {
            Debug.Log("kill " + gameObject.name);
            StopAllCoroutines();
            // blood.gameObject.SetActive(true);
            // blood.Play(true);
            anim.transform.parent.DOKill();
            anim.PlayDie(callback:() =>
            {
                isDead = true;
                
            });
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