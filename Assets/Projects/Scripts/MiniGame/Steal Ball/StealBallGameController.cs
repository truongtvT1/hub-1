using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using Projects.Scripts.Data;
using RandomNameAndCountry.Scripts;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv;
using UnityEngine;

namespace MiniGame.Steal_Ball
{
    public class StealBallGameController : GamePlayController
    {
        [FoldoutGroup("Character")] public PlayerStealBallController playerSaiyan;
        [FoldoutGroup("Object"), SerializeField] private BallNest[] ballNest;
        [FoldoutGroup("Object"), SerializeField] private Pool[] ballPool;
        [FoldoutGroup("Object"), SerializeField] private Pool[] obstaclePool;
        [FoldoutGroup("Bot"), SerializeField] private PlayerStealBallController botPrefab;
        [FoldoutGroup("Bot"), SerializeField] private List<PlayerStealBallController> listBot = new List<PlayerStealBallController>();
        [FoldoutGroup("Bot"), SerializeField] private List<AIBrain> botBrain = new List<AIBrain>();
        [FoldoutGroup("GamePlay"), SerializeField] private GameDifficulty difficulty;
        [FoldoutGroup("GamePlay"), SerializeField] private int level;
        
        private MiniGameInfo gameInfo;

        protected override void Start()
        {
            base.Start();
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
            //init player
            var skin = GameDataManager.Instance.GetSkinInGame();
            var color = GameDataManager.Instance.GetCurrentColor();
            playerSaiyan.Init(skin,color,ballNest[0]);
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
                var bot = Instantiate(botPrefab,ballNest[i].transform.position, Quaternion.identity);
                yield return new WaitUntil(() => bot != null);
                var listSkin = GameDataManager.Instance.RandomSkinList();
                var skinColor = GameDataManager.Instance.RandomColor();
                bot.Init(listSkin,skinColor,ballNest[i],botBrain[(int) difficulty]);
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
                listBot[i].InitRank(botInfo);
                LeaderBoardInGame.Instance.ListRanking.Add(botInfo);
            }
        }
    }
}