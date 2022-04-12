using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniGame.MemoryMatter
{
    public class MemoryGameController : MonoBehaviour
    {
        [SerializeField] private List<Transform> stairsList;
        [SerializeField] private List<SpriteRenderer> fruitList;
        [SerializeField] private List<Sprite> fruitSprites;
        [SerializeField] private GameObject resultObj;
        [SerializeField] private float showDuration, resultDuration, turnDuration;
        [SerializeField] private int showQuantity, maxTurn, maxRound, maxNumberObjToShowPerTurn;
        [SerializeField] private float deltaDifficulty;
        private int currentRound = 0, currentTurn = 0, indexObj;
        private bool isShowing, isHiding, isFinishTurn, isWaitNextRound;

        private void Start()
        {
            
        }

        public void Refresh()
        {
            var rd = new System.Random();
            var rdList = fruitSprites.OrderBy(_ => rd.Next()).Take(fruitSprites.Count);
            for (int i = 0; i < fruitList.Count; i++)
            {
                fruitList[i].sprite = rdList.ElementAt(i);
                fruitList[i].transform.parent.gameObject.SetActive(false);
            }
            
            
        }

        async void NextRound()
        {
            if (currentRound == maxRound)
            {
                return;
            }

            for (int i = 0; i < stairsList.Count; i++)
            {
                if (i == indexObj)
                {
                    continue;
                }

                stairsList[i].DOMoveY(0, 1f).SetEase(Ease.OutQuint);
            }

            await Task.Delay(1500);
            NextTurn();
        }

        void NextTurn()
        {
            if (currentTurn == maxTurn)
            {
                Result();
                return;
            }

            currentTurn++;
            float timeShow = 0;
            float timeDelayShow = 0;
            Show();
            StopAllCoroutines();
            StartCoroutine(CountTime(turnDuration, 0, f =>
            {
                if (timeShow < showDuration && isShowing)
                {
                    timeShow += Time.deltaTime;
                }
                else
                {
                    Hide();
                }
            }, () =>
            {
                NextTurn();
            }));


        }


        private List<GameObject> lastShowObjs = new List<GameObject>();
        void Show()
        {
            isShowing = true;
            var num = Random.Range(2, maxNumberObjToShowPerTurn);
            var random = new System.Random();
            var objsToShow = fruitList.OrderBy(_ => random.Next()).Take(num).ToList();
            for (int i = 0; i < fruitList.Count; i++)
            {
                objsToShow[i].transform.parent.gameObject.SetActive(true);
            }
        }

        void Hide()
        {
            if (isHiding)
            {
                return;
            }
            isHiding = true;
            for (int i = 0; i < fruitList.Count; i++)
            {
                fruitList[i].transform.parent.gameObject.SetActive(false);
            }
        }

        void Result()
        {
            indexObj = Random.Range(0, fruitList.Count);
            
            StartCoroutine(CountTime(showDuration, 0, f =>
            {
                
            }, () =>
            {
                
            }));
            StartCoroutine(CountTime(resultDuration, 0, f =>
            {
                
            }, () =>
            {
                
            }));
        }
        
        IEnumerator CountTime(float duration, float delay, Action<float> onCounting = null, Action callback = null)
        {
            yield return new WaitForSeconds(delay);
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                onCounting?.Invoke(time);
                yield return null;
            }
            callback?.Invoke();
        }
    }
}