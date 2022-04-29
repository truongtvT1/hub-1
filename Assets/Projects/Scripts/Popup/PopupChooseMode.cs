using System;
using System.Collections.Generic;
using Projects.Scripts.Data;
using Projects.Scripts.Hub.Component;
using ThirdParties.Truongtv;
using Truongtv.PopUpController;
using Truongtv.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Popup
{
    public class PopupChooseMode : BasePopup
    {
        [SerializeField] private ModeGameLauncher prefab;
        [SerializeField] private Transform prefabContainer;
        [SerializeField] private Button closeButton;
        private List<MiniGameInfo> _miniGameList;
        private void Awake()
        {
            closeButton.onClick.AddListener(Close);
        }
        public void Init()
        {
            prefabContainer.RemoveAllChild();
            PrepareData();
            for (var i = 0; i < GameDataManager.Instance.miniGameData.maxGameCount; i++)
            {
                if (i < _miniGameList.Count)
                {
                    var item = Instantiate(prefab, prefabContainer);
                    item.Init(_miniGameList[i]);
                }
                else
                {
                    var item = Instantiate(prefab, prefabContainer);
                    item.Init();
                }
            }
        }

        private void PrepareData()
        {
            _miniGameList = new List<MiniGameInfo>(GameDataManager.Instance.miniGameData.miniGameList);
            var lastPlayed = GameDataManager.Instance.GetLastPlayed();
            var max = 0;
            foreach (var info in _miniGameList)
            {
                info.total = GameDataManager.Instance.GetMiniGameCountPlayed(info.gameId);
                info.win = GameDataManager.Instance.GetMiniGameWinCount(info.gameId);
                info.lose = GameDataManager.Instance.GetMiniGameLoseCount(info.gameId);
                info.recentPlay = lastPlayed.Equals(info.gameId);
                if (max < info.total)
                    max = info.total;
            }

            foreach (var info in _miniGameList)
            {
                info.mostPlay = (max == info.total)&&max>0;
            }
        }
    }

}