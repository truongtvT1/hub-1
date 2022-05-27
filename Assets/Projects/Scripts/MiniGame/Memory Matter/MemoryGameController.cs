using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Projects.Scripts.Data;
using RandomNameAndCountry.Scripts;
using Projects.Scripts.Hub;
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
        [SerializeField] private GameObject tutorialObj;
        [SerializeField] private Button settingButton;
        [SerializeField] private List<Transform> stairsList;
        [SerializeField] private List<SpriteRenderer> fruitList;
        [SerializeField] private List<Sprite> fruitSprites;
        [SerializeField] private GameObject UIContainer;
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
        private bool isShowing, isHiding, isObjFalling;
        private float timeCounter;
        private GamePlayController _gamePlayController;
        private List<int> lastShownObjIndex = new List<int>();
        private List<PlayerSkin> cacheBotSkin; 
        private float cacheShowDuration, cacheTurnDuration, deltaDifficulty;
        private int cacheMaxTurn, cacheMaxNumberObjs;
        private int objCount = 0;
        private float countTimeObj = 0;
        private int level;
        private MiniGameInfo gameInfo;
        private void Awake()
        {
            settingButton.onClick.AddListener(() =>
            {
                InGamePopupController.Instance.ShowPopupSetting(() =>
                {
                    StopAllCoroutines();
                    GameDataManager.Instance.ResetSkinInGame();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }, () =>
                {
                    StopAllCoroutines();
                    GameDataManager.Instance.ResetSkinInGame();
                }, null);
            });
            cacheShowDuration = showDuration;
            cacheTurnDuration = turnDuration;
            cacheMaxTurn = maxTurn;
            cacheMaxNumberObjs = maxNumberObjToShowPerTurn;
            _gamePlayController = GetComponent<GamePlayController>();
            listBot = new List<PlayerController>(maxBot);
            Hide();
        }

        private async void Start()
        {
            gameInfo = GameDataManager.Instance.miniGameData.miniGameList.Find(_ => _.gameId.Contains("squid"));
            level = GameDataManager.Instance.GetMiniGameMasterPoint(gameInfo.gameId);
            var enumCount = Enum.GetValues(typeof(GameDifficulty)).Length;
            for (int i = 1; i <= enumCount; i++)
            {
                if ((i - 1) * 3 < level && level <= i * 3)
                {
                    _difficulty = (GameDifficulty) (i - 1);
                    break;
                }
            } 
            deltaDifficulty = (float) _difficulty / 10;
            var currentColor = GameDataManager.Instance.GetCurrentColor();
            var currentSkin = GameDataManager.Instance.GetSkinInGame();
            var playerInfo = new RankIngame
            {
                isBot = false,
                isFinish = false,
                playerSkin = new PlayerSkin
                {
                    color = currentColor,
                    skin = currentSkin
                },
                name = "Me",
                rank = 0,
                score = 0
            };
            LeaderBoardInGame.Instance.ListRanking.Add(playerInfo);
            _gamePlayController.player.InitRank(playerInfo);
            for (int i = 0; i < maxBot; i++)
            {
                var listSkin = GameDataManager.Instance.RandomSkinList();
                var color = GameDataManager.Instance.RandomColor();
                var skin = new PlayerSkin();
                skin.color = color;
                skin.skin = listSkin;
                string botName = RandomNameAndCountryPicker.Instance.GetRandomPlayerInfo().playerName;
                RankIngame botInfo = new RankIngame
                {
                    isBot = true,
                    isFinish = false,
                    score = 0,
                    rank = 0,
                    name = botName,
                    playerSkin = skin
                };
                LeaderBoardInGame.Instance.ListRanking.Add(botInfo);
                var rd = Random.Range(spawnRange[0].position.x, spawnRange[1].position.x);
                var bot = Instantiate(botPrefab);
                await Task.Delay(10);
                bot.transform.position = new Vector3(rd,spawnRange[0].position.y, bot.transform.position.z);
                listBot.Add(bot);
                bot.BotInit(botBrainData[Random.Range(0,botBrainData.Length)], (BotDifficulty) Random.Range(0,4),listSkin,color);
                bot.InitRank(botInfo);
            }
            LeaderBoardInGame.Instance.Init(LevelGoal.Score);
        }

        private void Update()
        {
            if (timeToStart >= 0 && _gamePlayController.state == GameState.None)
            {
                timeCounterText.transform.parent.gameObject.SetActive(true);
                timeCounterText.text = "" + (int) timeToStart;
                timeToStart -= Time.deltaTime;
            }
            else if (_gamePlayController.state != GameState.Playing && _gamePlayController.state != GameState.End) 
            {
                _gamePlayController.StartGame("memory_game",_difficulty.ToString());
                tutorialObj.SetActive(false);
                timeCounterText.transform.parent.gameObject.SetActive(false);
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
            //Score player
            if (currentRound != 0)
            {
                for (int i = 0; i < listBot.Count; i++)
                {
                    if (!listBot[i].IsDead)
                    {
                        listBot[i].Score(1);
                    }
                }
                if (!_gamePlayController.player.IsDead)
                {
                    _gamePlayController.player.Score(1);
                }
                LeaderBoardInGame.Instance.UpdateBoard();
                LeaderBoardInGame.Instance.Show();
            }
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
                UIContainer.SetActive(false);
                InGamePopupController.Instance.ShowPopupWin(GameDataManager.Instance.miniGameData.miniGameList.Find(_ => _.gameId.Contains("memory")));
                return;
            }
            currentRound++;
            
            await Task.Delay(500);
            if (_gamePlayController.player.IsDead)
            {
                var rd1 = Random.Range(spawnRange[0].position.x, spawnRange[1].position.x);
                _gamePlayController.player.transform.position = new Vector3(rd1, spawnRange[0].position.y,_gamePlayController.player.transform.position.z);
                _gamePlayController.player.Revive();
            }
            
            for (int i = 0; i < listBot.Count; i++)
            {
                if (listBot[i].IsDead)
                {
                    var rd1 = Random.Range(spawnRange[0].position.x, spawnRange[1].position.x);
                    listBot[i].transform.position = new Vector3(rd1,spawnRange[0].position.y, listBot[i].transform.position.z);
                    listBot[i].Revive();
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
                if (bot)
                {
                    Destroy(bot.gameObject);
                }
            }
        }

        async void NextTurn()
        {
            if (currentTurn == maxTurn)
            {
                Result();
                return;
            }

            await Task.Delay(2000);
            LeaderBoardInGame.Instance.Hide();
            Show();
            currentTurn++;
            
            //difficulty
            showDuration -= showDuration * deltaDifficulty;
            turnDuration -= deltaDifficulty;
            float timeShow = 0;
            StopAllCoroutines();
            StartCoroutine(CountTime(turnDuration, 0, f =>
            {
                if (countTimeObj < trapDuration)
                {
                    countTimeObj += Time.deltaTime;
                }
                else
                {
                    if (objCount < trapAmountPerTurn && !isObjFalling)
                    {
                        ObjectFall();
                    }
                }
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
                int count = 0;
                for (int i = 0; i < lastShownObjIndex.Count; i++)
                {
                    if (count > 1)
                    {
                        break;
                    }
                    var item = fruitList[lastShownObjIndex[i]];
                    if (item != null && temp.Contains(item))
                    {
                        temp.Remove(item);
                        count++;
                    }
                }
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
                timeCounterText.transform.parent.gameObject.SetActive(true);
            }, () =>
            {
                timeCounterText.transform.parent.gameObject.SetActive(false);
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
                },callback: async () =>
            {
                await Task.Delay(2500);
                currentTurn = 0;
                NextRound();
            }
                ,() => !isShowing));
        }

        async void ObjectFall()
        {
            isObjFalling = true;
            var pos = Random.Range(spawnRange[1].position.x, spawnRange[0].position.x);
            var warning = warningPool.nextThing;
            warning.transform.position = new Vector3(pos,warningPool.transform.position.y);
            objCount++;
            await Task.Delay(500);
            var item = trapPool[Random.Range(0,trapPool.Length)].nextThing;
            item.SetActive(false);
            item.transform.position = new Vector3(pos,spawnRange[0].position.y);
            item.SetActive(true);
            await Task.Delay(1000);
            countTimeObj = 0;
            isObjFalling = false;
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