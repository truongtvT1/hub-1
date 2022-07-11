using System;
using System.Collections.Generic;
using System.Linq;
using Projects.Scripts.Data;
using ThirdParties.Truongtv;
using Truongtv.PopUpController;
using Truongtv.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Projects.Scripts.Popup
{
    public class PopupLeaderBoard : BasePopup
    {
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private LeaderBoardItem prefab;
        [SerializeField] private Button backButton;
        [SerializeField] private Transform itemContainer;
        [SerializeField] private LeaderBoardRankRate rankRate;
        [SerializeField] private int rank1Trophy, rank2Trophy, rank3Trophy;
        private List<UserRanking> _ranks;
        
        private void Awake()
        {
            backButton.onClick.AddListener(Close);
            
        }
        
        public void Init()
        {
            //add user rank
            var userRank = GameDataManager.Instance.GetUserRanking();
            userRank.trophy = GameDataManager.Instance.GetTotalTrophy();
            _ranks = new List<UserRanking>();
            _ranks.Add(userRank);
            var fakeRank = GameDataManager.Instance.GetUserFakeRankData();
            //update fake rank data 
            var lastTime = GameDataManager.Instance.GetTimeUpdateLeaderBoard();
            if (DateTime.Now.Subtract(lastTime).TotalDays > 1)
            {
                for (int i = 0; i < rankRate.ranks.Count; i++)
                {
                    var countMatch = Random.Range(rankRate.minMatchesPlayed, rankRate.maxMatchesPlayed);
                    while (countMatch > 0)
                    {
                        float rd = Random.Range(0,100);
                        if (rd < rankRate.ranks[i].rank4Rate * 100)
                        {
                            fakeRank[i].lose++;
                        }
                        else if (rd < rankRate.ranks[i].rank3Rate * 100)
                        {
                            fakeRank[i].win++;
                            fakeRank[i].trophy += rank3Trophy;
                        }
                        else if (rd < rankRate.ranks[i].rank2Rate * 100)
                        {
                            fakeRank[i].win++;
                            fakeRank[i].trophy += rank2Trophy;
                        }
                        else 
                        {
                            fakeRank[i].win++;
                            fakeRank[i].trophy += rank1Trophy;
                        }
                        countMatch--;
                    }
                }
                GameDataManager.Instance.UpdateUserFakeRankData(fakeRank);
                GameDataManager.Instance.UpdateTimeShowLeaderBoard();
            }
            _ranks.AddRange(fakeRank);
            _ranks = _ranks.OrderByDescending(_ => _.trophy).ToList();
            itemContainer.RemoveAllChild();
            var rect = scroll.content.sizeDelta;
            rect.y = 772;
            scroll.content.sizeDelta = rect;
            for (var i = 0; i < _ranks.Count; i++)
            {
                _ranks[i].rank = i + 1;
                var item = Instantiate(prefab,itemContainer);
                item.Init(_ranks[i], new Vector2(60 - i * 7, -69 * i),scroll);
                scroll.onValueChanged.AddListener(item.UpdateLayoutPosition);
            }
            
        }
    }
}
