using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MiniGame.MemoryMatter
{
    public class MemoryGameController : MonoBehaviour
    {
        [SerializeField] private Button settingButton;
        [SerializeField] private List<Transform> stairsList;
        [SerializeField] private List<SpriteRenderer> fruitList;
        [SerializeField] private List<Sprite> fruitSprites;
        [FoldoutGroup("Character")] public PlayerController playerPrefabs;
        [SerializeField,FoldoutGroup("Bot")] private int maxBot = 8;
        [SerializeField,FoldoutGroup("Bot")] private PlayerController botPrefab;
        [SerializeField,FoldoutGroup("Bot")] private List<PlayerController> listBot;
        [SerializeField,FoldoutGroup("Bot")] private BrainStateData[] botBrainData;
        [SerializeField, FoldoutGroup("Bot")] private Transform[] spawnRange;
        [SerializeField, FoldoutGroup("Trap")] private Pool[] trapPool;
        [SerializeField, FoldoutGroup("Trap")] private float trapDuration;
        [SerializeField, FoldoutGroup("Trap")] private int trapAmountPerTurn;
        [SerializeField, FoldoutGroup("Trap")] private Pool warningPool;
        [SerializeField] private GameDifficulty _difficulty;
        [SerializeField] private Image resultObj;
        [SerializeField] private float showDuration, resultDuration, turnDuration;
        [SerializeField] private int maxTurn, maxRound, maxNumberObjToShowPerTurn;
        [SerializeField] private TextMeshProUGUI timeInGameText, timeCounterText;
        [SerializeField] private float timeToStart;
        
        private int currentRound = 0, currentTurn = 0, indexObj;
        private bool isShowing, isHiding;
        private float timeCounter;
        private GamePlayController _gamePlayController;
        private List<int> lastShownObjIndex = new List<int>();
        private float cacheShowDuration, cacheTurnDuration, deltaDifficulty;
        private int cacheMaxTurn, cacheMaxNumberObjs;
        private List<BotSkin> cacheBotSkin;
        private List<string> playerSkin;
        private Color playerColor;
        
        private void Awake()
        {
            settingButton.onClick.AddListener(() =>
            {
                InGamePopupController.Instance.ShowPopupSetting(() =>
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }, () =>
                {
                    SceneManager.LoadScene("Menu");
                }, null);
            });
            deltaDifficulty = (float) _difficulty / 10;
            cacheShowDuration = showDuration;
            cacheTurnDuration = turnDuration;
            cacheMaxTurn = maxTurn;
            cacheMaxNumberObjs = maxNumberObjToShowPerTurn;
            _gamePlayController = GetComponent<GamePlayController>();
            listBot = new List<PlayerController>(maxBot);
            cacheBotSkin = new List<BotSkin>();
            Hide();
        }

        private async void Start()
        {
            playerSkin = GameDataManager.Instance.GetSkinInGame();
            playerColor = GameDataManager.Instance.GetCurrentColor(); 
            for (int i = 0; i < maxBot; i++)
            {
                var listSkin = GameDataManager.Instance.RandomSkinList();
                var color = GameDataManager.Instance.RandomColor();
                var skin = new BotSkin();
                skin.color = color;
                skin.skin = listSkin;
                cacheBotSkin.Add(skin);
                var rd = Random.Range(spawnRange[0].position.x, spawnRange[1].position.x);
                var bot = Instantiate(botPrefab);
                await Task.Delay(10);
                bot.transform.position = new Vector3(rd,spawnRange[0].position.y, bot.transform.position.z);
                listBot.Add(bot);
                bot.BotInit(botBrainData[Random.Range(0,botBrainData.Length)], (BotDifficulty) Random.Range(0,4),listSkin,color);
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
            else if (_gamePlayController.state != GameState.Playing && _gamePlayController.state != GameState.End) 
            {
                _gamePlayController.StartGame("memory_game");
                timeCounterText.transform.root.gameObject.SetActive(false);
                timeCounterText.text = "";
                NextRound();
            }
            
            if (_gamePlayController.state == GameState.Playing)
            {
                timeInGameText.text = timeCounter.ToString("F");
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
            countTimeObj = 0;
            objCount = 0;
            for (int i = 0; i < listBot.Count; i++)
            {
                listBot[i].SetTarget(null);
            }
            
            if (currentRound == maxRound)
            {
                //Show popup Game over
                _gamePlayController.state = GameState.End;
                DOTween.KillAll(true);
                StopAllCoroutines();
                await Task.Delay(2500);
                SceneManager.LoadScene("Menu");
                GameDataManager.Instance.ResetSkinInGame();
                return;
            }
            currentRound++;
            
            if (_gamePlayController.player == null)
            {
                var rd1 = Random.Range(spawnRange[0].position.x, spawnRange[1].position.x);
                var player = Instantiate(playerPrefabs);
                player.Animation.SetVisible();
                await Task.Delay(10);
                player.Animation.SetVisible(true);
                player.transform.position = new Vector3(rd1, spawnRange[0].position.y,player.transform.position.z);
                player.Init();
                _gamePlayController.player = player;
            }
            
            for (int i = 0; i < listBot.Count; i++)
            {
                if (listBot[i] == null)
                {
                    var rd1 = Random.Range(spawnRange[0].position.x, spawnRange[1].position.x);
                    var bot = Instantiate(botPrefab);
                    bot.Animation.SetVisible();
                    await Task.Delay(10);
                    bot.Animation.SetVisible(true);
                    bot.transform.position = new Vector3(rd1,spawnRange[0].position.y, bot.transform.position.z);
                    bot.BotInit(botBrainData[Random.Range(0,botBrainData.Length)], (BotDifficulty) Random.Range(0,4),cacheBotSkin[i].skin,cacheBotSkin[i].color);
                    listBot[i] = bot;
                }
            }
            
            NextTurn();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            DOTween.KillAll(true);
            foreach (var bot in listBot)
            {
                Destroy(bot.gameObject);
            }
        }

        async void NextTurn()
        {
            if (currentTurn == maxTurn)
            {
                Result();
                return;
            }

            await Task.Delay(1000);
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


        private int objCount = 0;
        private float countTimeObj = 0;

        void Result()
        {
            indexObj = Random.Range(0, fruitList.Count);
            resultObj.sprite = fruitList[indexObj].sprite;
            resultObj.SetNativeSize();
            resultObj.transform.parent.gameObject.SetActive(true);
            isShowing = true;
            
            //random list bot ignore target
            int count = (int) _difficulty;
            var rd = new System.Random();
            var ignoreList = listBot.OrderBy(_ => rd.Next()).Take(count).ToList();
            int k = 0;
            for (int i = 0; i < listBot.Count; i++)
            {
                if (k < count)
                {
                    if (listBot[i] == ignoreList[k])
                    {
                        k++;
                        continue;
                    }
                }
                listBot[i].SetTarget(fruitList[indexObj].transform);
            }

            var showDuration = Mathf.RoundToInt(cacheShowDuration * (1 - deltaDifficulty) + 1f);
            StopAllCoroutines();
            StartCoroutine(CountTime(showDuration, 0.5f, f =>
            {
                timeCounterText.text = "" + (int) (showDuration - f);
                timeCounterText.transform.root.gameObject.SetActive(true);
                if (countTimeObj < trapDuration && objCount < trapAmountPerTurn)
                {
                    countTimeObj += Time.deltaTime;
                }
                else
                {
                    ObjectFall();
                }
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

            StartCoroutine(CountTime(resultDuration, 0, time =>
                {
                    if (countTimeObj < trapDuration && objCount < trapAmountPerTurn)
                    {
                        countTimeObj += Time.deltaTime;
                    }
                    else
                    {
                        ObjectFall();
                    }
                },callback: async () =>
            {
                await Task.Delay(2500);
                //Score player
                
                currentTurn = 0;
                NextRound();
            }
                ,() => !isShowing));
        }

        async void ObjectFall()
        {
            var amount = Random.Range(0, trapAmountPerTurn + 1 - objCount);
            GameObject item = null;
            List<Vector3> rdPos = new List<Vector3>();
            for (int i = 0; i < amount; i++)
            {
                item = warningPool.nextThing;
                rdPos.Add(new Vector3(Random.Range(spawnRange[1].position.x, spawnRange[0].position.x), 0));
                item.transform.position = new Vector3(rdPos[i].x,warningPool.transform.position.y);
            }
            objCount += amount;
            await Task.Delay(500);
            for (int i = 0; i < amount; i++)
            {
                item = trapPool[Random.Range(0,trapPool.Length)].nextThing;
                item.transform.position = new Vector3(rdPos[i].x,spawnRange[0].position.y);
            }
            countTimeObj = 0;
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