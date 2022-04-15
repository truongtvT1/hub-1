using System.Collections;
using DG.Tweening;
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
        // public BallAnimation anim;
        public float rollSpeed, moveSpeed;
        private Coroutine _coroutineRed,_coroutineGreen;
        private float difficulty, finishPosX, delayGreen = .6f, delayRed = .4f;
        public void Init(SquidGameController controller, string skin, float finishPos, float difficulty)
        {
            isReady = false;
            isWin = false;
            this.difficulty = difficulty;
            finishPosX = finishPos;
            // anim.SetSkin(skin);
            moveSpeed += moveSpeed * Random.Range(0,difficulty);
            rollSpeed += rollSpeed * Random.Range(0,difficulty);
            var distance = Random.Range(1f, 3f);
            transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.y);
            // anim.transform.parent.transform.position = start.position + new Vector3(distance, 0);
            // anim.gameObject.SetActive(true);
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
            // anim.transform.parent.DOMoveX(start.position.x, moveSpeed)
            //     .SetSpeedBased(true)
            //     .SetEase(Ease.Linear)
            //     .OnUpdate(() =>
            //     {
            //         anim.transform.parent.rotation = Quaternion.Lerp(anim.transform.parent.rotation,Quaternion.Euler(anim.transform.parent.rotation.eulerAngles + new Vector3(0,0,10f)),rollSpeed * Time.deltaTime);
            //     })
            //     .OnComplete(() =>
            //     {
            //         isReady = true;
            //     });
            // anim.transform.parent.DOMoveX(end.position.x, moveSpeed)
            //     .SetSpeedBased(true)
            //     .SetEase(Ease.Linear)
            //     .OnKill(() =>
            //     {
            //         isMoving = false;
            //     })
            //     .OnUpdate(() =>
            //     {
            //         if (anim.transform.parent.position.x <= finishPosX && !isWin)
            //         {
            //             isWin = true;
            //             _controller.onGreenLight = null;
            //             _controller.onRedLight = null;
            //             _controller.playerRank++;
            //         }
            //         if (isMoving && _controller.state != SquidGameController.GameState.Pause)
            //         {
            //             anim.transform.parent.rotation = Quaternion.Lerp(anim.transform.parent.rotation,Quaternion.Euler(anim.transform.parent.rotation.eulerAngles + new Vector3(0,0,10f)),rollSpeed * Time.deltaTime);
            //         }
            //     })
            //     .OnComplete(() =>
            //     {
            //         anim.PlaySmile();
            //         SoundInGameManager.Instance.PlayBallWinSound();
            //     })
            //     .Pause();
        }

        private bool tempMoving = false;
        
        public void Pause()
        {
            tempMoving = isMoving;
            isMoving = false;
            // anim.transform.parent.DOPause();
            // anim.PauseAnim(false);
        }

        public void Resume()
        {
            isMoving = tempMoving;
            if (isMoving)
            {
                // anim.PauseAnim(true);
                // anim.PlayMix();
                // anim.transform.parent.DOTogglePause();
            }
        }
        
        IEnumerator RedLight()
        {
            var rd = Random.Range(0, delayRed - Random.Range(0,difficulty));
            yield return new WaitForSeconds(rd);
            if (_coroutineGreen != null)
            {
                StopCoroutine(_coroutineGreen);
            }
            isMoving = false;
            // anim.transform.parent.DOPause();
            // anim.PauseAnim(false);
        }

        IEnumerator GreenLight()
        {
            var rd = Random.Range(0, delayGreen - Random.Range(0,difficulty));
            yield return new WaitForSeconds(rd);
            if (_coroutineRed != null)
            {
                StopCoroutine(_coroutineRed);
            }
            isMoving = true;
            // anim.PauseAnim(true);
            // anim.PlayMix();
            // anim.transform.parent.DOTogglePause();

        }

        public void Kill()
        {
            Debug.Log("kill " + gameObject.name);
            StopAllCoroutines();
            blood.gameObject.SetActive(true);
            blood.Play(true);
            // anim.transform.parent.DOKill();
            // anim.PlayDie(DamageType.Object, () =>
            // {
            //     isDead = true;
            //     anim.PauseAnim(false);
            // });
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.y);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(start.position,.1f);
            Gizmos.DrawSphere(end.position,.1f);
        }
#endif
    }
}