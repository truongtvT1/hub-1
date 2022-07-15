using System;
using System.Threading.Tasks;
using DG.Tweening;
using MiniGame;
using Projects.Scripts.Data;
using Projects.Scripts.Hub;
using Projects.Scripts.Menu;
using Sirenix.OdinInspector;
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
        [SerializeField] private Button watchAdButton, replayButton, backButton;
        [SerializeField] private Transform finger, cupTransform, ticketTransform;
        [SerializeField] private ParticleGold ticketFx, cupFx;
        [SerializeField] private float bonusDuration;
        private int currentTrophy, currentTicket, ticketReward, trophyReward, xValue = -1, currentIndex = 0;
        private Sequence _increaseTicket, _increaseTrophy;

        private void Awake()
        {
            backButton.onClick.AddListener(() =>
            {
                SoundInGameController.Instance.StopRewardBonusSfx();
                GameServiceManager.ShowInterstitialAd(() =>
                {
                    GameDataManager.Instance.ResetSkinInGame();
                    Loading.Instance.LoadMenu();
                });
            });
            watchAdButton.onClick.AddListener(OnWatchAdClick);
        }

        void UpdateTicket(int value)
        {
            if (value == 0) return;
            var currentCoin = GameDataManager.Instance.GetTotalTicket();
            GameDataManager.Instance.UpdateTicket(value);
            var newCoinValue = GameDataManager.Instance.GetTotalTicket();
            if (_increaseTicket.IsActive())
                _increaseTicket.Kill(true);
            _increaseTicket = DOTween.Sequence();
            _increaseTicket.Append(DOTween.To(() => currentCoin, x => currentCoin = x, newCoinValue, 1f)
                .SetEase(Ease.InOutSine));
            ticketFx.transformTarget = ticketTransform;
            ticketFx.transform.position = top1Skeleton.transform.position;
            ticketFx.gameObject.SetActive(true);
            ticketFx.Play(value);
            _increaseTicket.OnUpdate(() => { ticketText.text = "" + currentCoin; });
            _increaseTicket.OnComplete(() => { ticketText.text = "" + newCoinValue; });
            _increaseTicket.Play();
        }

        void UpdateTrophy(int value)
        {
            if (value == 0) return;
            var currentCoin = GameDataManager.Instance.GetTotalTrophy();
            GameDataManager.Instance.UpdateTrophy(value);
            var newCoinValue = GameDataManager.Instance.GetTotalTrophy();
            if (_increaseTrophy.IsActive())
                _increaseTrophy.Kill(true);
            _increaseTrophy = DOTween.Sequence();
            _increaseTrophy.Append(DOTween.To(() => currentCoin, x => currentCoin = x, newCoinValue, 1f)
                .SetEase(Ease.InOutSine));
            cupFx.transformTarget = cupTransform;
            cupFx.transform.position = top1Skeleton.transform.position;
            cupFx.gameObject.SetActive(true);
            cupFx.Play(value);
            _increaseTrophy.OnUpdate(() => { trophyText.text = "" + currentCoin; });
            _increaseTrophy.OnComplete(() => { trophyText.text = "" + newCoinValue; });
            _increaseTrophy.Play();
        }

        public void Init(MiniGameInfo info)
        {
            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(() =>
            {
                //TODO: matching then reload scene 
                SoundInGameController.Instance.StopRewardBonusSfx();
                Loading.Instance.LoadMiniGame(info);
                GameDataManager.Instance.ResetSkinInGame();
            });

            openCompleteAction = () =>
            {
                if (GamePlayController.Instance)
                {
                    if (GamePlayController.Instance.player)
                    {
                        var rank = GamePlayController.Instance.player.GetRank();
                        Ranked(rank);
                    }
                    else if (GamePlayController.Instance.GetPlayerInfo() != null)
                    {
                        var rank = GamePlayController.Instance.GetPlayerInfo().rank;
                        Ranked(rank);
                    }
                    else if (LeaderBoardInGame.Instance)
                    {
                        foreach (var rankInGame in LeaderBoardInGame.Instance.ListRanking)
                        {
                            if (!rankInGame.isBot)
                            {
                                Ranked(rankInGame.rank);
                                break;
                            }
                        }
                    }
                }
                else if (LeaderBoardInGame.Instance)
                {
                    foreach (var rankInGame in LeaderBoardInGame.Instance.ListRanking)
                    {
                        if (!rankInGame.isBot)
                        {
                            Ranked(rankInGame.rank);
                            break;
                        }
                    }
                }
            };
            
            void Ranked(int rank)
            {
                if (rank > 3)
                {
                    ticketReward = 1;
                    trophyReward = 0;
                    GameDataManager.Instance.UpdateMiniGameLoseCount(info.gameId);
                }
                else
                {
                    SoundInGameController.Instance.PlayWin();
                    if (rank == 1)
                    {
                        ticketReward = info.rank1.ticket;
                        trophyReward = info.rank1.trophy;
                    }
                    else if (rank == 2)
                    {
                        ticketReward = info.rank2.ticket;
                        trophyReward = info.rank2.trophy;
                    }
                    else
                    {
                        ticketReward = info.rank3.ticket;
                        trophyReward = info.rank3.trophy;
                    }
                    GameDataManager.Instance.UpdateMiniGameWinCount(info.gameId);
                }
                Debug.Log($"give {ticketReward} ticket");
                Debug.Log($"give {trophyReward} trophy");
                UpdateTicket(ticketReward);
                UpdateTrophy(trophyReward);
            }
            
            var startPos = -394f;
            finger.DOLocalMoveX(startPos, bonusDuration)
                .OnStart(() =>
                {
                    SoundInGameController.Instance.PlayRewardBonus();
                })
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .OnUpdate(() =>
                {
                    if (finger.localPosition.x >= startPos && finger.localPosition.x < startPos + 5 + 160)
                    {
                        xValue = 2;
                    }
                    else if (finger.localPosition.x >= startPos + 6 + 159 &&
                             finger.localPosition.x < startPos + 6 + 159 * 2)
                    {
                        xValue = 3;
                    }
                    else if (finger.localPosition.x >= startPos + 6 + 159 * 2 &&
                             finger.localPosition.x < startPos + 6 + 159 * 3)
                    {
                        xValue = 5;
                    }
                    else if (finger.localPosition.x >= startPos + 6 + 159 * 3 &&
                            finger.localPosition.x < startPos + 6 + 159 * 4)
                    {
                        xValue = 3;
                    }
                    else
                    {
                        xValue = 2;
                    }
                })
                .OnKill(() =>
                {
                    SoundInGameController.Instance.StopRewardBonusSfx();
                });
            currentTicket = GameDataManager.Instance.GetTotalTicket();
            currentTrophy = GameDataManager.Instance.GetTotalTrophy();
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
            finger.DOKill();
            GameServiceManager.ShowRewardedAd(GameServiceManager.eventConfig.rewardForBonusWin, async () =>
            {
                //TODO: random x value
                //TODO: reward ticket & trophy
                
                Debug.Log("xValue = " + xValue);
                UpdateTicket(ticketReward * (xValue - 1));
            });
            
        }
        
        
    }
}