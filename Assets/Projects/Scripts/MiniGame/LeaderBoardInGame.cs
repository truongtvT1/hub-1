using System;
using System.Collections.Generic;
using System.Linq;
using RandomNameAndCountry.Scripts;
using ThirdParties.Truongtv;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MiniGame
{
    public class LeaderBoardInGame : MonoBehaviour
    {
        private List<RankIngame> listRanking = new List<RankIngame>();
        private LevelGoal goalType;
        [SerializeField] private List<BoardRow> boardRows = new List<BoardRow>();
        [SerializeField] private CanvasGroup canvasGroup;
        private static LeaderBoardInGame _instance;
        private int currentRank;
        public static Action<RankIngame> scoreAction, finishAction;
        public static LeaderBoardInGame Instance
        {
            get => _instance;
        }

        public List<RankIngame> ListRanking
        {
            get 
            {
                if (listRanking == null)
                {
                    listRanking = new List<RankIngame>();
                    return listRanking;
                }
                return listRanking;
            } 
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(_instance.gameObject);
            }
            listRanking = new List<RankIngame>();
            scoreAction = OnPlayerScore;
            finishAction = OnPlayerFinish;
        }

        void OnPlayerScore(RankIngame rankIngame)
        {
            var playerScore = listRanking.Find(_ => _.name == rankIngame.name);
            playerScore.score = rankIngame.score;
        }

        void OnPlayerFinish(RankIngame rankIngame)
        {
            var playerScore = listRanking.Find(_ => _.name == rankIngame.name);
            playerScore.isFinish = true;
            playerScore.rank = currentRank + 1;
            currentRank++;
            UpdateBoard();
        }
        
        public void Init(LevelGoal goalType)
        {
            this.goalType = goalType;
            currentRank = 0;
            if (listRanking.Count < boardRows.Count)
            {
                boardRows.RemoveRange(listRanking.Count, boardRows.Count - listRanking.Count);
            }

            for (int i = 0; i < boardRows.Count; i++)
            {
                boardRows[i].gameObject.SetActive(true);
                if (goalType == LevelGoal.Score)
                {
                    boardRows[i].UpdateScore(listRanking[i], listRanking[i].isBot);
                }
                else
                {
                    boardRows[i].UpdateFinish(listRanking[i], listRanking[i].isBot);
                }
            }
        }

        public void UpdateBoard()
        {
            if (goalType == LevelGoal.Racing)
            {
                listRanking = listRanking.OrderBy(_ => _.rank).ToList();
                for (int i = 0; i < boardRows.Count; i++)
                {
                    boardRows[i].UpdateFinish(listRanking[i], listRanking[i].isBot);
                }
            }
            else
            {
                listRanking = listRanking.OrderByDescending(_ => _.score).ToList();
                for (int i = 0; i < boardRows.Count; i++)
                {
                    listRanking[i].rank = i + 1;
                    boardRows[i].UpdateScore(listRanking[i], listRanking[i].isBot);
                }
            }
        }

        public void Show()
        {
            canvasGroup.alpha = 1;
        }
        
        public void Hide()
        {
            canvasGroup.alpha = 0;
        }
    }
    
    [Serializable]
    public enum LevelGoal
    {
        Racing,
        Score
    }
    
    [Serializable]
    public class RankIngame
    {
        public PlayerSkin playerSkin;
        public string name;
        public int rank, score;
        public bool isBot, isFinish;
    }
}