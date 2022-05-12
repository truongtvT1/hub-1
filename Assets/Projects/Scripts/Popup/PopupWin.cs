using System;
using MiniGame;
using Projects.Scripts.Data;
using Projects.Scripts.Hub;
using ThirdParties.Truongtv;
using TMPro;
using Truongtv.PopUpController;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Popup
{
    public class PopupWin : BasePopup
    {
        [SerializeField] private TextMeshProUGUI trophyText, ticketText, winText, loseText, rankingText;
        [SerializeField] private CharacterAnimationGraphic top1Skeleton, top2Skeleton, top3Skeleton;
        [SerializeField] private Button watchAdButton, continueButton;
        private int currentTrophy, currentTicket;
        private void Awake()
        {
            continueButton.onClick.AddListener(() =>
            {
                GameServiceManager.ShowInterstitialAd(() =>
                {
                    //TODO: reward ticket & trophy
                    
                    GameDataManager.Instance.ResetSkinInGame();
                    Close();
                });
            });
            watchAdButton.onClick.AddListener(OnWatchAdClick);
        }

        public void Init(MiniGameInfo info)
        {
            if (GamePlayController.Instance != null)
            {
                if (GamePlayController.Instance.player.GetRank() <= 3)
                {
                    GameDataManager.Instance.UpdateMiniGameWinCount(info.gameId);
                }
                else
                {
                    GameDataManager.Instance.UpdateMiniGameLoseCount(info.gameId);
                }
            }
            currentTicket = GameDataManager.Instance.GetTotalTicket();
            currentTrophy = GameDataManager.Instance.GetTotalTrohpy();
            trophyText.text = currentTrophy + "";
            ticketText.text = currentTicket + "";
            winText.text = "Win: " + GameDataManager.Instance.GetMiniGameWinCount(info.gameId);
            loseText.text = "Lose: " + GameDataManager.Instance.GetMiniGameLoseCount(info.gameId);
            if (LeaderBoardInGame.Instance != null)
            {
                rankingText.text = LeaderBoardInGame.Instance.ListRanking.Find(_ => !_.isBot).rank + "";
                top1Skeleton.SetSkin(LeaderBoardInGame.Instance.ListRanking[0].playerSkin.skin);
                top2Skeleton.SetSkin(LeaderBoardInGame.Instance.ListRanking[1].playerSkin.skin);
                top3Skeleton.SetSkin(LeaderBoardInGame.Instance.ListRanking[2].playerSkin.skin);
                top1Skeleton.SetSkinColor(LeaderBoardInGame.Instance.ListRanking[0].playerSkin.color);
                top2Skeleton.SetSkinColor(LeaderBoardInGame.Instance.ListRanking[1].playerSkin.color);
                top3Skeleton.SetSkinColor(LeaderBoardInGame.Instance.ListRanking[2].playerSkin.color);
                top1Skeleton.PlayMixWin();
                top2Skeleton.PlayMixWin();
                top3Skeleton.PlayMixWin();
            }
        }
        
        void OnWatchAdClick()
        {
            GameServiceManager.ShowRewardedAd("win_bonus",() =>
            {
                //TODO: random x value
                //TODO: reward ticket & trophy
                
                GameDataManager.Instance.ResetSkinInGame();
                Close();
            });
        }
    }
}