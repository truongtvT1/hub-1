using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Base;
using Com.LuisPedroFonseca.ProCamera2D;
using Projects.Scripts.Data;
using Projects.Scripts.Hub;
using RandomNameAndCountry.Scripts;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MiniGame.StickRun
{
    public class StickRunGameController : GamePlayController
    {
        [FoldoutGroup("UI")] public Button settingButton;
        [FoldoutGroup("UI")] public GameObject holdToRunFastText;
        [FoldoutGroup("UI")] public GameObject camera;
        public List<StickmanPlayerController> listBot = new List<StickmanPlayerController>();
        public StickmanPlayerController botPrefabs;
        public GameDifficulty difficulty;
        public StickmanPlayerController playerStickman;
        public Vector2 startPos;
        public Pool[] trapPoolEasy;
        public Pool[] trapPoolNormal;
        public Pool[] trapPoolHard;
        public Pool[] trapPoolHell;
        public GameObject gateFinish;
        [SerializeField] private int maxTimeRevive = 3;
        [SerializeField] private int maxBotNumbers = 3;
        private MiniGameInfo gameInfo;
        public static StickRunGameController Instance
        {
            get => instance;
        }


        private List<PlayerSkin> cacheBotSkin = new List<PlayerSkin>();
        private int deadTime, level;
        private Transform checkPoint;
        private static StickRunGameController instance;
        private float deltaDifficulty, mapLength;
        private bool isHoldingSprint, isFirstTap;
        
        
        private const float _mapStartOffset = 14f;
        private const float OBJECT_AVGWIDTH = 0f;
        private const int EASY_OBJECT_AMOUNT = 6;
        private const int NORMAL_OBJECT_AMOUNT = 7;
        private const int HARD_OBJECT_AMOUNT = 8;
        private const int HELL_OBJECT_AMOUNT = 9;
        private const float EASY_MAPLENGTH= 110f;
        private const float NORMAL_MAPLENGTH = 120f;
        private const float HARD_MAPLENGTH = 130f;
        private const float HELL_MAPLENGTH = 140f;
        // private const float EASY_MINDISTANCE = 10f;
        // private const float NORMAL_MINDISTANCE = 8f;
        // private const float HARD_MINDISTANCE = 7f;
        // private const float HELL_MINDISTANCE = 5f;
        // private const float EASY_MAXDISTANCE = 15f;
        // private const float NORMAL_MAXDISTANCE = 12f;
        // private const float HARD_MAXDISTANCE = 10f;
        // private const float HELL_MAXDISTANCE = 8f;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance.gameObject);
            }
            gameInfo = GameDataManager.Instance.miniGameData.miniGameList.Find(_ => _.gameId.Contains("run"));
            level = GameDataManager.Instance.GetMiniGameMasterPoint(gameInfo.gameId);
            var enumCount = Enum.GetValues(typeof(GameDifficulty)).Length;
            for (int i = 1; i <= enumCount; i++)
            {
                if ((i - 1) * 3 < level && level <= i * 3)
                {
                    difficulty = (GameDifficulty) (i - 1);
                    break;
                }
            }
            settingButton.onClick.AddListener(() =>
            {
                InGamePopupController.Instance.ShowPopupSetting(() =>
                {
                    GameDataManager.Instance.ResetSkinInGame();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }, () =>
                {
                    GameDataManager.Instance.UpdateMiniGameLoseCount(gameInfo.gameId);
                    GameDataManager.Instance.ResetSkinInGame();
                }, null);
            });
        }

        private void Start()
        {
            StartCoroutine(Init());
        }

        IEnumerator Init()
        {
            GameServiceManager.LogEvent(GameServiceManager.eventConfig.levelStart,new Dictionary<string, object>{{"stick_run",difficulty.ToString()}});
            
            deadTime = 0;
            var camBoundaries = ProCamera2D.Instance.GetComponent<ProCamera2DNumericBoundaries>();
            #region gen map
            int count = 0;
            float pos = _mapStartOffset;
            float delta = 3f; //sai so ngau nhien khoang cach giua cac obj
            var gate = Instantiate(gateFinish, null, true);
            GameObject lastObj = null;
            switch (difficulty)
            {
                case GameDifficulty.Easy:
                    camBoundaries.RightBoundary = EASY_MAPLENGTH + _mapStartOffset;
                    gate.transform.position = new Vector3(camBoundaries.RightBoundary - 5f, gate.transform.position.y, gate.transform.position.z);
                    while (count < EASY_OBJECT_AMOUNT && pos <= camBoundaries.RightBoundary - 20)
                    {
                        var trap = trapPoolEasy[Random.Range(0, trapPoolEasy.Length)].nextThing;
                        yield return new WaitUntil(() => trap);
                        if (lastObj != null){
                            if (lastObj.name.Contains("Wood Ninja") && trap.name.Contains("Wood Ninja") 
                                || lastObj.name.Contains("Underground") && trap.name.Contains("Underground") 
                                || lastObj.name.Contains("Roll") && trap.name.Contains("Roll")
                                || lastObj.name.Equals(trap.name))
                            {
                                Destroy(trap);
                                continue;
                            }
                        }
                        lastObj = trap;
                        Debug.Log($"pos object {count} : {pos}");
                        trap.transform.position = new Vector3(pos, trap.transform.position.y,trap.transform.position.z);
                        if (trap.TryGetComponent(out AutoMoving movingItem))
                        {
                            movingItem.UpdateListPoint();
                        }
                        pos += OBJECT_AVGWIDTH + EASY_MAPLENGTH/EASY_OBJECT_AMOUNT + Random.Range(-delta, delta);
                        count++;
                    }
                    break;
                case GameDifficulty.Normal: 
                    camBoundaries.RightBoundary = NORMAL_MAPLENGTH + _mapStartOffset;
                    gate.transform.position = new Vector3(camBoundaries.RightBoundary - 5f, gate.transform.position.y, gate.transform.position.z);
                    while (count < NORMAL_OBJECT_AMOUNT && pos <= camBoundaries.RightBoundary - 20)
                    {
                        var trap = trapPoolNormal[Random.Range(0, trapPoolNormal.Length)].nextThing;
                        yield return new WaitUntil(() => trap);
                        if (lastObj != null){
                            if (lastObj.name.Contains("Wood Ninja") && trap.name.Contains("Wood Ninja") 
                            || lastObj.name.Contains("Underground") && trap.name.Contains("Underground") 
                            || lastObj.name.Contains("Roll") && trap.name.Contains("Roll")
                            || lastObj.name.Equals(trap.name))
                            {
                                Destroy(trap);
                                continue;
                            }
                        }
                        lastObj = trap;
                        Debug.Log($"pos object {count} : {pos}");
                        trap.transform.position = new Vector3(pos, trap.transform.position.y,trap.transform.position.z);
                        if (trap.TryGetComponent(out AutoMoving movingItem))
                        {
                            movingItem.UpdateListPoint();
                        }
                        pos += OBJECT_AVGWIDTH + NORMAL_MAPLENGTH/NORMAL_OBJECT_AMOUNT + Random.Range(-delta, delta);
                        count++;
                    }
                    break;
                case GameDifficulty.Hard: 
                    camBoundaries.RightBoundary = HARD_MAPLENGTH + _mapStartOffset;
                    gate.transform.position = new Vector3(camBoundaries.RightBoundary - 5f, gate.transform.position.y, gate.transform.position.z);
                    while (count < HARD_OBJECT_AMOUNT && pos <= camBoundaries.RightBoundary - 20)
                    {
                        var trap = trapPoolHard[Random.Range(0, trapPoolHard.Length)].nextThing;
                        yield return new WaitUntil(() => trap);
                        if (lastObj != null){
                            if (lastObj.name.Contains("Wood Ninja") && trap.name.Contains("Wood Ninja") 
                                || lastObj.name.Contains("Underground") && trap.name.Contains("Underground") 
                                || lastObj.name.Contains("Roll") && trap.name.Contains("Roll")
                                || lastObj.name.Equals(trap.name))
                            {
                                Destroy(trap);
                                continue;
                            }
                        }
                        lastObj = trap;
                        trap.transform.position = new Vector3(pos, trap.transform.position.y,trap.transform.position.z);
                        if (trap.TryGetComponent(out AutoMoving movingItem))
                        {
                            movingItem.UpdateListPoint();
                        }
                        pos += OBJECT_AVGWIDTH + HARD_MAPLENGTH/HARD_OBJECT_AMOUNT + Random.Range(-delta, delta);
                        count++;
                    }
                    break;
                case GameDifficulty.Hell: 
                    camBoundaries.RightBoundary = HELL_MAPLENGTH + _mapStartOffset;
                    gate.transform.position = new Vector3(camBoundaries.RightBoundary - 5f, gate.transform.position.y, gate.transform.position.z);
                    while (count < HELL_OBJECT_AMOUNT && pos <= camBoundaries.RightBoundary - 20)
                    {
                        var trap = trapPoolHell[Random.Range(0, trapPoolHell.Length)].nextThing;
                        yield return new WaitUntil(() => trap);
                        if (lastObj != null){
                            if (lastObj.name.Contains("Wood Ninja") && trap.name.Contains("Wood Ninja") 
                                || lastObj.name.Contains("Underground") && trap.name.Contains("Underground") 
                                || lastObj.name.Contains("Roll") && trap.name.Contains("Roll")
                                || lastObj.name.Equals(trap.name))
                            {
                                Destroy(trap);
                                continue;
                            }
                        }
                        lastObj = trap;
                        trap.transform.position = new Vector3(pos, trap.transform.position.y,trap.transform.position.z);
                        if (trap.TryGetComponent(out AutoMoving movingItem))
                        {
                            movingItem.UpdateListPoint();
                        }
                        pos += OBJECT_AVGWIDTH + HELL_MAPLENGTH/HELL_OBJECT_AMOUNT + Random.Range(-delta, delta);
                        count++;
                    }
                    break;
            }
            #endregion
            
            //init leader board
            var skin = GameDataManager.Instance.GetSkinInGame();
            var skinColor = GameDataManager.Instance.GetCurrentColor();
            var playerInfo = new RankIngame
            {
                isBot = false,
                isFinish = false,
                playerSkin = new PlayerSkin
                {
                    color = skinColor,
                    skin = skin
                },
                name = "Me",
                rank = maxBotNumbers + 1,
                score = 0
            };
            playerStickman.InitRank(playerInfo);
            LeaderBoardInGame.Instance.ListRanking.Add(playerInfo);
            
            //init bot
            for (int i = 0; i < maxBotNumbers; i++)
            {
                var listSkin = GameDataManager.Instance.RandomSkinList();
                var color = GameDataManager.Instance.RandomColor();
                cacheBotSkin.Add(new PlayerSkin(color,listSkin));
                var bot = Instantiate(botPrefabs);
                bot.name = "bot" + i;
                yield return new WaitUntil(() => bot);
                bot.transform.position = startPos + new Vector2(Random.Range(-1,2) * i, 0);
                var botController = bot.GetComponent<StickmanPlayerController>();
                var difficulty = (int) this.difficulty;
                botController.Init(listSkin,color,(BotDifficulty) difficulty);
                string botName = RandomNameAndCountryPicker.Instance.GetRandomPlayerInfo().playerName;
                RankIngame botInfo = new RankIngame
                {
                    isBot = true,
                    isFinish = false,
                    score = 0,
                    rank = maxBotNumbers + 1,
                    name = botName,
                    playerSkin = new PlayerSkin
                    {
                        skin = listSkin,
                        color = color
                    }
                };
                botController.InitRank(botInfo);
                LeaderBoardInGame.Instance.ListRanking.Add(botInfo);
                listBot.Add(botController);
            }
            LeaderBoardInGame.Instance.Init(LevelGoal.Racing);
            LeaderBoardInGame.Instance.Show();
            yield return new WaitUntil(() => isFirstTap);
            holdToRunFastText.gameObject.SetActive(false);
            state = GameState.Playing;
        }

        public void Dead()
        {
            deadTime++;
        }
        
        public bool CheckCanRevive()
        {
            return deadTime <= maxTimeRevive;
        }

        public async void Win()
        {
            state = GameState.End;
            await Task.Delay(2000);
            camera.SetActive(false);
            InGamePopupController.Instance.ShowPopupWin(gameInfo);
        }
        
        public void EndGame()
        {
            state = GameState.End;
            LeaderBoardInGame.Instance.OnMainPlayerDie(playerStickman.GetRankInfo());
            LeaderBoardInGame.Instance.UpdateBoard();
            camera.SetActive(false);
            InGamePopupController.Instance.ShowPopupWin(gameInfo);
        }
        
        private void Update()
        {
            if (state == GameState.Playing)
            {
                playerStickman.TouchSprint(isHoldingSprint);
            }
        }

        public void Tap()
        {
            isFirstTap = true;
        }
        
        public void TouchSprint(bool release = false)
        {
            if (state != GameState.End)
            {
                isHoldingSprint = !release;
            }
        }
    }
}