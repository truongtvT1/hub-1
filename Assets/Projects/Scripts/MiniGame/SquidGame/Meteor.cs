using System.Collections;
using DG.Tweening;
using ThirdParties.Truongtv.SoundManager;
using UnityEngine;

namespace MiniGame.SquidGame
{
    public class Meteor : MonoBehaviour
    {
        public GameObject effectGround, warning, meteor,trigger;
        public ParticleSystem bloomFx;
        public SpriteRenderer effectBlow;
        public AudioClip impactGroundSound;
        public void Init(float y)
        {
            var spriteRenderer = meteor.GetComponent<SpriteRenderer>();
            if (y < -4.64f)
            {
                spriteRenderer.sortingOrder = 21;
            }
            else if (y >= -4.64f && y < -3.84f)
            {
                spriteRenderer.sortingOrder = 19;
            }
            else if (y >= -3.84f && y < -2.56f)
            {
                spriteRenderer.sortingOrder = 17;
            }
            else if (y >= -2.56f && y < -2.09f)
            {
                spriteRenderer.sortingOrder = 15;
            }
            else if (y >= -2.09f && y < -0.42f)
            {
                spriteRenderer.sortingOrder = 13;
            }
            else if (y >= -0.42f && y < 0.17f)
            {
                spriteRenderer.sortingOrder = 11;
            }
            else if (y >= 0.17f && y < 0.73f)
            {
                spriteRenderer.sortingOrder = 9;
            }
            else if (y >= 0.73f && y < 1.26f)
            {
                spriteRenderer.sortingOrder = 7;
            }
            else if (y >= 1.26f && y < 1.6f)
            {
                spriteRenderer.sortingOrder = 5;
            }
            else if (y >= 1.6f && y < 2.86f)
            {
                spriteRenderer.sortingOrder = 3;
            }
            else
            {
                spriteRenderer.sortingOrder = 1;
            }
            StartCoroutine(Fall());
        }
        
        private IEnumerator Fall()
        {
            var tween = effectBlow.DOFade(50 / 255f, .2f).SetLoops(-1, LoopType.Yoyo).Play();
            yield return new WaitForSeconds(2f);
            warning.SetActive(false);
            meteor.SetActive(true);
            tween.TogglePause();
            yield return new DOTweenCYInstruction.WaitForCompletion(meteor.GetComponent<DOTweenAnimation>().tween);
            SoundManager.Instance.PlaySfx(impactGroundSound);
            bloomFx.gameObject.SetActive(true);
            effectBlow.gameObject.SetActive(false);
            effectGround.SetActive(true);
            trigger.SetActive(true);
            yield return new WaitForSeconds(.2f);
            trigger.SetActive(false);
            meteor.transform.GetChild(0).gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            meteor.GetComponent<SpriteRenderer>().DOFade(0, .5f)
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }
    }
}