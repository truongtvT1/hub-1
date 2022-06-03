using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using HedgehogTeam.EasyTouch;
using Projects.Scripts.Data;
using RandomNameAndCountry.Scripts;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv;
using TMPro;
using Truongtv.Utilities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace MiniGame.Steal_Ball
{
    public class StealBallGameController : GamePlayController
    {
        [FoldoutGroup("UI"), SerializeField] private TextMeshProUGUI gameTimeCountText, timeStartText;
        [FoldoutGroup("UI"),SerializeField] private ETCJoystick joyStick;
        [FoldoutGroup("Character")] public PlayerStealBallController playerSaiyan;
        [FoldoutGroup("Object"), SerializeField] private Transform container;
        [FoldoutGroup("Object"), SerializeField] private BallNest[] ballNest;
        [FoldoutGroup("Object"), SerializeField] private Pool[] ballPool;
        [FoldoutGroup("Object"), SerializeField] private Pool[] obstaclePool;
        [FoldoutGroup("Object"), SerializeField] private float objectOffset = 3f;
        [FoldoutGroup("Object"), SerializeField] private NavMeshSurface surface2D;
        [FoldoutGroup("Bot"), SerializeField] private PlayerStealBallController botPrefab;
        [FoldoutGroup("Bot"), SerializeField] private List<PlayerStealBallController> listBot = new List<PlayerStealBallController>();
        [FoldoutGroup("Bot"), SerializeField] private List<BrainStateData> botBrain = new List<BrainStateData>();
        [FoldoutGroup("GamePlay"), SerializeField] private GameDifficulty difficulty;
        [FoldoutGroup("GamePlay"), SerializeField] private float gameDuration;
        [FoldoutGroup("GamePlay"), SerializeField] private int maxBallDrop, level;
        public static Action<Ball> onDropBall;
        
        private float timeCount;
        private int ballCount;
        private bool isDropingBall;
        private MiniGameInfo gameInfo;


        protected override void Start()
        {
            base.Start();
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(() =>
            {
                InGamePopupController.Instance.ShowPopupSetting(() =>
                {
                    StopAllCoroutines();
                    GameDataManager.Instance.ResetSkinInGame();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }, () =>
                {
                    StopAllCoroutines();
                    state = GameState.End;
                    GameDataManager.Instance.ResetSkinInGame();
                }, null);
            });
            gameInfo = GameDataManager.Instance.miniGameData.miniGameList.Find(_ => _.gameId.Contains("steal"));
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
            StartCoroutine(Init());
        }

        IEnumerator Init()
        {
            gameTimeCountText.text = TimeSpan.FromSeconds(gameDuration).ToString(@"mm\:ss");
            //generate map
            surface2D.BuildNavMesh();
            var countObstacle = Random.Range(3, 6);
            int k = 0;
            for (int i = 0; i < countObstacle; i++)
            {
                if (k > 2)
                {
                    k = 0;
                }
                var pos = RandomPos(k);
                var item = obstaclePool[i].nextThing;
                item.transform.SetParent(container);
                item.transform.position = pos;
                listObstaclePos.Add(pos);
                yield return new WaitUntil(() => item);
                var wait = surface2D.BuildNavMeshAsync();
                k++;
                yield return wait.isDone;
            }
            
            //init player
            var skin = GameDataManager.Instance.GetSkinInGame();
            var color = GameDataManager.Instance.GetCurrentColor();
            playerSaiyan.Init(skin,color,ballNest[0],10 - (int) difficulty);
            var playerInfo = new RankIngame
            {
                isBot = false,
                isFinish = false,
                playerSkin = new PlayerSkin
                {
                    color = color,
                    skin = skin
                },
                name = "Me",
                rank = ballNest.Length,
                score = 0
            };
            playerSaiyan.InitRank(playerInfo);
            LeaderBoardInGame.Instance.ListRanking.Add(playerInfo);
            
            //init bot
            for (int i = 1; i < ballNest.Length; i++)
            {
                var bot = Instantiate(botPrefab,container,false);
                bot.transform.position = ballNest[i].transform.position;
                yield return new WaitUntil(() => bot != null);
                var listSkin = GameDataManager.Instance.RandomSkinList();
                var skinColor = GameDataManager.Instance.RandomColor();
                bot.Init(listSkin,skinColor,ballNest[i],10 - (int) difficulty - i,(BotDifficulty) (int) difficulty,botBrain[0]);
                listBot.Add(bot);
                string botName = RandomNameAndCountryPicker.Instance.GetRandomPlayerInfo().playerName;
                RankIngame botInfo = new RankIngame
                {
                    isBot = true,
                    isFinish = false,
                    score = 0,
                    rank = ballNest.Length,
                    name = botName,
                    playerSkin = new PlayerSkin
                    {
                        skin = listSkin,
                        color = skinColor
                    }
                };
                bot.InitRank(botInfo);
                LeaderBoardInGame.Instance.ListRanking.Add(botInfo);
            }
            LeaderBoardInGame.Instance.Init(LevelGoal.Score);
            joyStick.onMove.AddListener(axis =>
            {
                playerSaiyan.TouchMove(axis);
            });
            joyStick.onMoveEnd.AddListener(() =>
            {
                playerSaiyan.TouchMove(Vector3.zero, true);
            });
            StartCoroutine(Extended.CountTime(5, 0, count =>
            {
                timeStartText.text = TimeSpan.FromSeconds(5 - count).ToString(@"ss");
            }, () =>
            {
                timeStartText.gameObject.SetActive(false);
                state = GameState.Playing;
            }));
        }

        
        
        private void Update()
        {
            if (state == GameState.Playing)
            {
                if (timeCount >= gameDuration)
                {
                    EndGame();
                }

                if (ballCount < maxBallDrop && timeCount >= ballCount * gameDuration / maxBallDrop && !isDropingBall)
                {
                    DropBall();
                }
                gameTimeCountText.text = TimeSpan.FromSeconds(gameDuration - timeCount).ToString(@"mm\:ss");
                timeCount += Time.deltaTime;
            }
        }

        private async void EndGame()
        {
            state = GameState.End;
            timeStartText.text = "TIME OVER!";
            timeStartText.gameObject.SetActive(true);
            LeaderBoardInGame.Instance.UpdateBoard();
            LeaderBoardInGame.Instance.Show();
            await Task.Delay(2000);
            InGamePopupController.Instance.ShowPopupWin(gameInfo);
        }
        
        async void DropBall()
        {
            isDropingBall = true;
            var rdPos = RandomPos(ballCount % 3);
            var ball = ballPool[Random.Range(0, ballPool.Length)].nextThing;
            await Task.Delay(10);
            ballCount++;
            ball.transform.position = new Vector3(rdPos.x,rdPos.y + 10f,-5f);
            ball.SetActive(true);
            ball.transform.DOMoveY(rdPos.y, 1f).SetEase(Ease.InSine)
                .OnComplete(() =>
                {
                    isDropingBall = false;
                    var item = ball.GetComponent<Ball>();
                    item.shadow.SetActive(true);
                    ball.GetComponent<UpdateZByY>().enabled = true;
                    onDropBall?.Invoke(item);
                });
        }
        
        Vector3 area1XRange = new Vector3(-8, -3);
        Vector3 area1YRange = new Vector3(-1,1);
        Vector3 area2XRange = new Vector3(3, 8);
        Vector3 area2YRange = new Vector3(-1,1);
        Vector3 area3XRange = new Vector3(-3, 3);
        Vector3 area3YRange = new Vector3(-4,4);
        Vector3 lastPos;
        private List<Vector3> listObstaclePos = new List<Vector3>();
        Vector3 RandomPos(int areaIndex)
        {
            Vector3 pos;
            bool valid = false;
            if (areaIndex == 0)
            {
                pos = new Vector3(Random.Range(area1XRange.x,area1XRange.y)
                    ,Random.Range(area1YRange.x,area1YRange.y));
            }
            else if (areaIndex == 1)
            {
                pos = new Vector3(Random.Range(area2XRange.x,area2XRange.y)
                    ,Random.Range(area2YRange.x,area2YRange.y));
            }
            else
            {
                pos = new Vector3(Random.Range(area3XRange.x,area3XRange.y)
                    ,Random.Range(area3YRange.x,area3YRange.y));
            }
            if (listObstaclePos.Count == 0)
            {
                valid = true;
            }
            while (!valid)
            {
                if (areaIndex == 0)
                {
                    pos = new Vector3(Random.Range(area1XRange.x,area1XRange.y)
                        ,Random.Range(area1YRange.x,area1YRange.y));
                }
                else if (areaIndex == 1)
                {
                    pos = new Vector3(Random.Range(area2XRange.x,area2XRange.y)
                        ,Random.Range(area2YRange.x,area2YRange.y));
                }
                else
                {
                    pos = new Vector3(Random.Range(area3XRange.x,area3XRange.y)
                        ,Random.Range(area3YRange.x,area3YRange.y));
                }

                int k = 0;
                for (int i = 0; i < listObstaclePos.Count; i++)
                {
                    if (Vector2.SqrMagnitude(pos - listObstaclePos[i]) < objectOffset)
                    {
                        k++;
                    }
                }

                if (k == 0)
                {
                    valid = true;
                }
            }
            Debug.Log("new random pos " + pos);
            return pos;
        }
        
    }
}