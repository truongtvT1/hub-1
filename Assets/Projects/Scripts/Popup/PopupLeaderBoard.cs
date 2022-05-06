using System;
using System.Collections.Generic;
using System.Linq;
using Projects.Scripts.Data;
using ThirdParties.Truongtv;
using Truongtv.PopUpController;
using Truongtv.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Popup
{
    public class PopupLeaderBoard : BasePopup
    {
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private LeaderBoardItem prefab;
        [SerializeField] private Button backButton;
        [SerializeField] private Transform itemContainer;
        private List<UserRanking> _ranks;
        
        private void Awake()
        {
            backButton.onClick.AddListener(Close);
            _ranks = new List<UserRanking>();
            //add user rank
            var userRank = GameDataManager.Instance.GetUserRanking();
            _ranks.Add(userRank);
            _ranks = _ranks.OrderBy(_ => _.trophy).ToList();

        }
        
        public void Init()
        {
            itemContainer.RemoveAllChild();
            var rect = scroll.content.sizeDelta;
            rect.y = 792;
            scroll.content.sizeDelta = rect;
            
            
            //read all data
            for (var i = 0; i < _ranks.Count; i++)
            {
                _ranks[i].rank = i + 1;
                var item = Instantiate(prefab,itemContainer);
                item.Init(_ranks[i], new Vector2(60 - i * 7, -69 * i),scroll);
                scroll.onValueChanged.AddListener(item.UpdateLayoutPosition);
            }
            for (var i = _ranks.Count; i < 10; i++)
            {
                var item = Instantiate(prefab,itemContainer);
                item.Init(null,new Vector2(60 - i * 7, -69 * i),scroll);
                scroll.onValueChanged.AddListener(item.UpdateLayoutPosition);
            }
            
        }
    }
}
