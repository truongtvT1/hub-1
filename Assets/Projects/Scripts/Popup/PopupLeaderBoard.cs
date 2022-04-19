using System;
using System.Collections.Generic;
using Projects.Scripts.Data;
using Truongtv.PopUpController;
using Truongtv.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Popup
{
    public class PopupLeaderBoard : BasePopup
    {
        [SerializeField] private LeaderBoardItem prefab;
        [SerializeField] private Button backButton;
        [SerializeField] private Transform itemContainer;
        private List<UserRanking> _ranks;
        private void Awake()
        {
            backButton.onClick.AddListener(Close);
            _ranks = new List<UserRanking>();
        }
        public void Init()
        {
            itemContainer.RemoveAllChild();
            //read all data
            for (var i = 0; i < _ranks.Count; i++)
            {
                var item = Instantiate(prefab,itemContainer);
                item.Init(_ranks[i]);
            }
            for (var i = _ranks.Count; i < 10; i++)
            {
                var item = Instantiate(prefab,itemContainer);
                item.Init();
            }
        }
    }
}
