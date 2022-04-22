using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace MiniGame.SquidGame
{
    public class Meteor : MonoBehaviour
    {
        public GameObject effectGround, warning, meteor,trigger;
        public SpriteRenderer effectBlow;

        private void Start()
        {
            StartCoroutine(Fall());
        }

        private IEnumerator Fall()
        {
            var tween = effectBlow.DOFade(50 / 255f, .2f).SetLoops(-1, LoopType.Yoyo).Play();
            yield return new WaitForSeconds(1.5f);
            warning.SetActive(false);
            meteor.SetActive(true);
            tween.TogglePause();
            yield return new DOTweenCYInstruction.WaitForCompletion(meteor.GetComponent<DOTweenAnimation>().tween);
            effectBlow.gameObject.SetActive(false);
            effectGround.SetActive(true);
            trigger.SetActive(true);
            yield return new WaitForSeconds(.5f);
            meteor.GetComponent<SpriteRenderer>().DOFade(0, .5f)
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }
    }
}