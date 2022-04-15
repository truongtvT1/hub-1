using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MiniGame.MemoryMatter
{
    public class MemoryGameController : MonoBehaviour
    {
        [SerializeField] private List<Transform> stairsList;
        [SerializeField] private List<SpriteRenderer> fruitList;
        [SerializeField] private List<Sprite> fruitSprites;
        [SerializeField] private Image resultObj;
        [SerializeField] private float showDuration, resultDuration, turnDuration;
        [SerializeField] private int maxBot = 3, maxTurn, maxRound, maxNumberObjToShowPerTurn;
        [SerializeField] private TextMeshProUGUI timeInGameText, timeCounterText;
        [SerializeField] private float timeToStart;
        [SerializeField] private GameDifficulty _difficulty;
        [SerializeField] private PlayerController botPrefab;
        [SerializeField] private List<PlayerController> listBot;
        [SerializeField] private BrainStateData botBrainData;
        private int currentRound = 0, currentTurn = 0, indexObj;
        private bool isShowing, isHiding;
        private float timeCounter;
        private GamePlayController _gamePlayController;
        private List<int> lastShownObjIndex = new List<int>();
        private float cacheShowDuration, cacheTurnDuration, deltaDifficulty;
        private int cacheMaxTurn, cacheMaxNumberObjs;

        private void Awake()
        {
            deltaDifficulty = (float) _difficulty / 10;
            cacheShowDuration = showDuration;
            cacheTurnDuration = turnDuration;
            cacheMaxTurn = maxTurn;
            cacheMaxNumberObjs = maxNumberObjToShowPerTurn;
            _gamePlayController = GamePlayController.Instance;
            listBot = new List<PlayerController>(maxBot);
            Hide();
        }

        private void Start()
        {
            for (int i = 0; i < maxBot; i++)
            {
                var bot = Instantiate(botPrefab);
                listBot.Add(bot);
                bot.Init(botBrainData);
            }
        }

        private void Update()
        {
            if (timeToStart >= 0 && _gamePlayController.state == GameState.None)
            {
                timeCounterText.transform.root.gameObject.SetActive(true);
                timeCounterText.text = "" + (int) timeToStart;
                timeToStart -= Time.deltaTime;
            }
            else if (_gamePlayController.state != GameState.Playing)
            {
                _gamePlayController.StartGame("memory_game");
                timeCounterText.transform.root.gameObject.SetActive(false);
                timeCounterText.text = "";
                NextRound();
            }
            
            if (_gamePlayController.state == GameState.Playing)
            {
                timeInGameText.text = timeCounter.ToString("00.00");
                timeCounter += Time.deltaTime;
            }
        }

        void Refresh()
        {
            var rd = new System.Random();
            var rdList = fruitSprites.OrderBy(_ => rd.Next()).Take(fruitSprites.Count).ToList();
            for (int i = 0; i < fruitList.Count; i++)
            {
                fruitList[i].sprite = rdList[i];
                fruitList[i].transform.parent.gameObject.SetActive(false);
            }
            if (currentRound != 0)
            {
                for (int i = 0; i < stairsList.Count; i++)
                {
                    stairsList[i].DOLocalMoveY(0, .5f).SetEase(Ease.Linear);
                }
            }
        }

        void ReassignParam()
        {
            showDuration = cacheShowDuration;
            turnDuration = cacheTurnDuration;
            maxTurn = cacheMaxTurn;
            maxNumberObjToShowPerTurn = cacheMaxNumberObjs;
        }
        
        async void NextRound()
        {
            Refresh();
            ReassignParam();
            if (currentRound == maxRound)
            {
                //Show popup Game over
                _gamePlayController.state = GameState.End;
                DOTween.KillAll(true);
                StopAllCoroutines();
                return;
            }
            currentRound++;
            await Task.Delay(2500);
            NextTurn();
        }

        void NextTurn()
        {
            if (currentTurn == maxTurn)
            {
                Result();
                return;
            }

            
            Show();
            currentTurn++;
            
            //difficulty
            showDuration -= showDuration * deltaDifficulty;
            turnDuration -= deltaDifficulty;
            float timeShow = 0;
            StopAllCoroutines();
            StartCoroutine(CountTime(turnDuration, 0, f =>
            {
                if (timeShow < showDuration && isShowing)
                {
                    timeShow += Time.deltaTime;
                }
                else if (!isHiding)
                {
                    Hide();
                }
            }, () =>
            {
                NextTurn();
            }));
        }
        
        void Show()
        {
            isShowing = true;
            isHiding = false;
            var num = Random.Range(2, maxNumberObjToShowPerTurn + 1);
            var random = new System.Random();
            if (currentTurn == 0)
            {
                var objsToShow = fruitList.OrderBy(_ => random.Next()).Take(num).ToList();
                for (int i = 0; i < objsToShow.Count; i++)
                {
                    objsToShow[i].transform.parent.gameObject.SetActive(true);
                    Debug.Log("fruit index last shown " + fruitList.IndexOf(objsToShow[i]));
                    lastShownObjIndex.Add(fruitList.IndexOf(objsToShow[i]));
                }
            }
            else
            {
                // Bỏ random obj đã show 
                List<SpriteRenderer> temp = new List<SpriteRenderer>();
                temp.AddRange(fruitList);
                temp.RemoveAt(lastShownObjIndex[Random.Range(0,lastShownObjIndex.Count)]);
                var objsToShow = temp.OrderBy(_ => random.Next()).Take(num).ToList();
                for (int i = 0; i < objsToShow.Count; i++)
                {
                    objsToShow[i].transform.parent.gameObject.SetActive(true);
                    Debug.Log("fruit index last shown " + fruitList.IndexOf(objsToShow[i]));
                    lastShownObjIndex = new List<int>();
                    lastShownObjIndex.Add(fruitList.IndexOf(objsToShow[i]));
                }
            }
        }

        void Hide()
        {
            isShowing = false;
            isHiding = true;
            for (int i = 0; i < fruitList.Count; i++)
            {
                fruitList[i].transform.parent.gameObject.SetActive(false);
            }
        }

        void Result()
        {
            indexObj = Random.Range(0, fruitList.Count);
            resultObj.sprite = fruitList[indexObj].sprite;
            resultObj.transform.parent.gameObject.SetActive(true);
            isShowing = true;
            var showDuration = Mathf.RoundToInt(cacheShowDuration * (1 - deltaDifficulty) + 1f);
            StopAllCoroutines();
            StartCoroutine(CountTime(showDuration, 0.5f, f =>
            {
                timeCounterText.text = "" + (int) (showDuration - f);
                timeCounterText.transform.root.gameObject.SetActive(true);
            }, () =>
            {
                timeCounterText.transform.root.gameObject.SetActive(false);
                timeCounterText.text = "";
                resultObj.transform.parent.gameObject.SetActive(false);
                
                fruitList[indexObj].transform.parent.gameObject.SetActive(true);
                for (int i = 0; i < stairsList.Count; i++)
                {
                    if (i != indexObj)
                    {
                        stairsList[i].DOLocalMoveY(-10, .5f).SetEase(Ease.InOutSine);
                    }
                }
                isShowing = false;
            }));

            StartCoroutine(CountTime(resultDuration, 0,callback: async () =>
            {
                //Score player

                await Task.Delay(1000);
                currentTurn = 0;
                NextRound();
            }
                ,waitUntil:() => !isShowing));
        }
        
        IEnumerator CountTime(float duration, float delay, Action<float> onCounting = null, Action callback = null, Func<bool> waitUntil = null)
        {
            if (waitUntil != null)
            {
                yield return new WaitUntil(waitUntil);
            }
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