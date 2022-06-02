using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Projects.Scripts.Data;
using Projects.Scripts.Hub;
using RandomNameAndCountry.Scripts;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using ThirdParties.Truongtv;
using ThirdParties.Truongtv.SoundManager;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Event = Spine.Event;
using Random = UnityEngine.Random;

namespace MiniGame.SquidGame
{
    public class SquidGameController : GamePlayController
    {
        [FoldoutGroup("UI")] public Transform holdToMove, container;
        [FoldoutGroup("UI")] public SpriteRenderer warningSprite;
        [FoldoutGroup("UI")] public TextMeshProUGUI gameTimeCountText;
        [FoldoutGroup("UI")] public Image greenProgress;
        public float gameDuration, greenDuration, redDuration, moveSpeed;
        public GameDifficulty difficulty;
        [SerializeField, Range(0, .3f)] private float difficultyDelta;
        private int maxMeteorPerTurn;
        public Vector2 fallRange;
        public GameObject meteorPrefab;

        [SpineAnimation(dataField = nameof(animBoss))]
        public string idle, redLight, greenLight, warning;

        public CharacterAnimation playerAnimation;
        public SkeletonAnimation animBoss, animStaff1, animStaff2;
        public float shootSpeed = 18f;
        public GameObject shotFx;
        public ParticleSystem blowEffect;
        public GameObject skull;
        public Transform start, end, playerTransform;
        public Action onRedLight, onGreenLight;
        public List<SquidPlayer> listNPC;
        public AudioSource audio;
        public AudioMixerGroup pitchBend;
        public AudioClip greenLightSound, redLightSound;
        private bool isHoldingMove, canKillBall;
        private float gameTimeCount, backUpPitchShift, progressImageWitdh;
        private int level;
        private Color cacheColor;
        private List<SquidPlayer> npcToKill = new List<SquidPlayer>();
        private List<SquidPlayer> listToKill1 = new List<SquidPlayer>();
        private List<SquidPlayer> listToKill2 = new List<SquidPlayer>();
        private MiniGameInfo gameInfo;
        protected override void Awake()
        {
            base.Awake();
            gameInfo = GameDataManager.Instance.miniGameData.miniGameList.Find(_ => _.gameId.Contains("squid"));
            pauseButton.onClick.AddListener(() =>
            {
                InGamePopupController.Instance.ShowPopupSetting(
                    () =>
                    {
                        GameDataManager.Instance.ResetSkinInGame();
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    },
                    () =>
                    {
                        // update result
                        GameDataManager.Instance.UpdateMiniGameLoseCount(gameInfo.gameId);
                        GameDataManager.Instance.ResetSkinInGame();
                    },
                    () => { });
            });
            audio.outputAudioMixerGroup = pitchBend;
            audio.clip = greenLightSound;
            pitchBend.audioMixer.GetFloat("pitchBend", out float pitchBendValue);
            backUpPitchShift = pitchBendValue;
            cacheColor = greenProgress.color;
            progressImageWitdh = greenProgress.GetComponent<RectTransform>().rect.width;
            maxMeteorPerTurn = (int) difficulty;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            pitchBend.audioMixer.SetFloat("pitchBend", backUpPitchShift);
        }

        protected override void Start()
        {
            
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
            difficultyDelta = (float) difficulty / 10;
            StartCoroutine(Init());
            GameServiceManager.LogEvent(GameServiceManager.eventConfig.levelStart, new Dictionary<string, object> {{"red_line", difficulty.ToString()}});
        }

        IEnumerator Init()
        {
            greenProgress.rectTransform.localPosition =
                new Vector3(-progressImageWitdh, greenProgress.rectTransform.localPosition.y);
            gameTimeCountText.text = TimeSpan.FromSeconds(gameDuration).ToString(@"mm\:ss");
            animBoss.state.SetAnimation(0, idle, true);
            holdToMove.transform.parent.GetComponent<CustomButton>().enabled = false;
            animStaff1.state.Event += StateOnEvent1;
            animStaff2.state.Event += StateOnEvent2;

            //init ball
            var skin = GameDataManager.Instance.GetSkinInGame();
            var color = GameDataManager.Instance.GetCurrentColor();
            playerAnimation.SetSkin(skin);
            playerAnimation.SetSkinColor(color);
            playerInfo = new RankIngame
            {
                isBot = false,
                isFinish = false,
                playerSkin = new PlayerSkin
                {
                    color = color,
                    skin = skin
                },
                name = "Me",
                rank = 11,
                score = 0
            };
            LeaderBoardInGame.Instance.ListRanking.Add(playerInfo);
            var distance = Random.Range(1f, 2f);
            playerTransform.position = start.position + new Vector3(distance, 0);
            playerTransform.DOMoveX(start.position.x, moveSpeed)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnStart(() => { playerAnimation.PlayRun(); })
                .OnComplete(() => { playerAnimation.PlayIdle(); });

            //init npc
            for (int i = 0; i < listNPC.Count; i++)
            {
                var listSkin = GameDataManager.Instance.RandomSkinList();
                var skinColor = GameDataManager.Instance.RandomColor();
                listNPC[i].Init(this, listSkin, skinColor, difficultyDelta);
                string botName = RandomNameAndCountryPicker.Instance.GetRandomPlayerInfo().playerName;
                RankIngame botInfo = new RankIngame
                {
                    isBot = true,
                    isFinish = false,
                    score = 0,
                    rank = 11,
                    name = botName,
                    playerSkin = new PlayerSkin
                    {
                        skin = listSkin,
                        color = skinColor
                    }
                };
                listNPC[i].InitRank(botInfo);
                LeaderBoardInGame.Instance.ListRanking.Add(botInfo);
            }

            yield return new WaitUntil(() => !listNPC.Exists(p => !p.isReady));
            holdToMove.gameObject.SetActive(true);
            holdToMove.transform.parent.GetComponent<CustomButton>().enabled = true;
            LeaderBoardInGame.Instance.Init(LevelGoal.Racing);
            //wait for player start
            yield return new WaitUntil(() => isHoldingMove);
            state = GameState.Playing;
            holdToMove.gameObject.SetActive(false);
            container.DOLocalMoveY(-65f, 1f).SetEase(Ease.Linear);
            if (level != 0)
            {
                var pitchShift = audio.pitch * (1 + level / 100f);
                audio.pitch = pitchShift;
                pitchBend.audioMixer.SetFloat("pitchBend", 1f / pitchShift);
                greenDuration -= greenDuration * level / 100f;
            }

            GreenLight();
        }

        private void StateOnEvent2(TrackEntry trackentry, Event e)
        {
            if (e.Data.Name.Equals("attack"))
            {
                for (int i = 0; i < listToKill2.Count; i++)
                {
                    var item = listToKill2[i];
                    var shot = Instantiate(shotFx);
                    shot.transform.position = animStaff2.transform.GetChild(0).transform.position;
                    shot.transform.DOMove(item.anim.transform.position, shootSpeed).SetSpeedBased(true).OnComplete(() =>
                    {
                        item.Kill();
                        Destroy(shot.gameObject);
                    });
                }
            }
        }

        private void StateOnEvent1(TrackEntry trackentry, Event e)
        {
            if (e.Data.Name.Equals("attack"))
            {
                for (int i = 0; i < listToKill1.Count; i++)
                {
                    var item = listToKill1[i];
                    var shot = Instantiate(shotFx);
                    shot.transform.position = animStaff1.transform.GetChild(0).transform.position;
                    shot.transform.DOMove(item.anim.transform.position, shootSpeed).SetSpeedBased(true).OnComplete(() =>
                    {
                        item.Kill();
                        Destroy(shot.gameObject);
                    });
                }

                if (canKillBall)
                {
                    var shot = Instantiate(shotFx);
                    shot.transform.position = animStaff1.transform.GetChild(0).transform.position;
                    shot.transform.DOMove(playerAnimation.transform.position, 15f).SetSpeedBased(true).OnComplete(() =>
                    {
                        KillBall();
                        Destroy(shot.gameObject);
                    });
                }
            }
        }


        void GreenLight()
        {
            onGreenLight?.Invoke();
            {
            animBoss.state.SetAnimation(0, greenLight, false).Complete += entry =>
                animBoss.state.SetAnimation(0, greenLight, true);
            };
            listToKill1 = new List<SquidPlayer>();
            listToKill2 = new List<SquidPlayer>();
            greenProgress.color = cacheColor;
            warningSprite.DOFade(0, 0);
            StopAllCoroutines();
            var isWarning = false;
            audio.Play();
            StartCoroutine(CountTime(greenDuration, 0, count =>
            {
                for (int i = 0; i < listNPC.Count; i++)
                {
                    if (!listNPC[i].isDead && listNPC[i].anim.transform.position.x < closestXPos)
                    {
                        closestXPos = listNPC[i].anim.transform.position.x;
                    }
                }

                if (playerAnimation.transform.position.x < closestXPos)
                {
                    closestXPos = playerAnimation.transform.position.x;
                }

                if (count >= meteorFallCount * greenDuration / maxMeteorPerTurn + 1.5f / (int) difficulty)
                {
                    if (meteorFallCount < maxMeteorPerTurn)
                    {
                        var meteor = Instantiate(meteorPrefab);
                        var posY = Random.Range(fallRange.x, fallRange.y);
                        meteor.transform.position =
                            new Vector3(Random.Range(animBoss.transform.position.x + 2f, closestXPos), posY);
                        meteor.GetComponent<Meteor>().Init(posY);
                        meteorFallCount++;
                    }
                }

                if (count / greenDuration >= 2/3f && !isWarning)
                {
                    greenProgress.color = Color.red;
                    isWarning = true;
                    warningSprite.DOFade(50 / 255f, .3f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                    animBoss.state.SetAnimation(0, warning, true);
                }


                greenProgress.rectTransform.localPosition = new Vector3(
                    progressImageWitdh * count / greenDuration - progressImageWitdh,
                    greenProgress.rectTransform.localPosition.y);
            }, () =>
            {
                if (state != GameState.End)
                {
                    meteorFallCount = 0;
                    warningSprite.DOKill();
                    var pitchShift = audio.pitch * (1 + difficultyDelta);
                    audio.pitch = pitchShift;
                    pitchBend.audioMixer.SetFloat("pitchBend", 1f / pitchShift);
                    greenDuration -= greenDuration * difficultyDelta;
                    RedLight();
                }
            }));
        }


        void RedLight()
        {
            SoundManager.Instance.PlaySfx(redLightSound);
            animBoss.state.SetAnimation(0, redLight, false).Complete += entry =>
            {
                onRedLight?.Invoke();
                animBoss.state.SetAnimation(0, idle, true);
            };
            warningSprite.DOFade(0 / 255f, 0);
            StopAllCoroutines();
            StartCoroutine(CountTime(redDuration, .3f, f =>
            {
                if (state != GameState.End)
                {
                    //check if any player move
                    for (int i = 0; i < listNPC.Count; i++)
                    {
                        if (!listNPC[i].isWin && listNPC[i].isMoving)
                        {
                            if (!npcToKill.Contains(listNPC[i]))
                            {
                                npcToKill.Add(listNPC[i]);
                            }
                        }
                    }

                    if (f >= .3f)
                    {
                        var npcToKillCount = npcToKill.Count;
                        if (npcToKillCount > 0)
                        {
                            if (npcToKillCount > 2)
                            {
                                var temp = new List<SquidPlayer>();
                                foreach (var item in npcToKill)
                                {
                                    var tempItem = item;
                                    temp.Add(tempItem);
                                }

                                for (int i = 0; i < temp.Count; i++)
                                {
                                    if (i <= temp.Count / 2)
                                    {
                                        listToKill1.Add(temp[i]);
                                    }
                                    else
                                    {
                                        listToKill2.Add(temp[i]);
                                    }
                                }

                                npcToKill = new List<SquidPlayer>();
                                animStaff1.state.SetAnimation(0, "attack", false)
                                    .Complete += entry => { animStaff1.state.SetAnimation(0, "idle", true); };
                                animStaff2.state.SetAnimation(0, "attack", false)
                                    .Complete += entry => { animStaff2.state.SetAnimation(0, "idle", true); };
                            }
                            else
                            {
                                var item = npcToKill[0];
                                var rd = Random.value > 0.5f;
                                if (rd)
                                {
                                    listToKill1.Add(item);
                                    animStaff1.state.SetAnimation(0, "attack", false)
                                        .Complete += entry => { animStaff1.state.SetAnimation(0, "idle", true); };
                                }
                                else
                                {
                                    listToKill2.Add(item);
                                    animStaff2.state.SetAnimation(0, "attack", false)
                                        .Complete += entry => { animStaff2.state.SetAnimation(0, "idle", true); };
                                }

                                npcToKill = new List<SquidPlayer>();
                            }
                        }
                    }

                    if (isHoldingMove && !HitFinishLine() && !canKillBall)
                    {
                        canKillBall = true;
                        animStaff1.state.SetAnimation(0, "attack", false).Complete += trackEntry =>
                        {
                            animStaff1.state.SetAnimation(0, "idle", true);
                        };
                    }
                }
            }, () =>
            {
                if (state != GameState.End)
                {
                    SoundManager.Instance.PlaySfx(redLightSound);
                    GreenLight();
                }
            }));
        }


        IEnumerator CountTime(float duration, float delay, Action<float> onCounting = null, Action callback = null)
        {
            yield return new WaitForSeconds(delay);
            float time = 0;
            while (time < duration && state == GameState.Playing)
            {
                time += Time.deltaTime;
                onCounting?.Invoke(time);
                yield return null;
                if (state == GameState.Pause)
                {
                    yield return new WaitUntil(() => state == GameState.Playing);
                }
            }

            callback?.Invoke();
        }

        private void KillBall()
        {
            StopAllCoroutines();
            LeaderBoardInGame.Instance.OnMainPlayerDie(playerInfo);
            LeaderBoardInGame.Instance.UpdateBoard();
            OnAttack();
        }

        private async void OnAttack()
        {
            state = GameState.End;

            //lose
            blowEffect.gameObject.SetActive(true);
            blowEffect.Play(true);
            skull.SetActive(true);
            playerAnimation.gameObject.SetActive(false);
            await Task.Delay(2500);
            Lose();
            //revive
            // PopupInGameController.Instance.OpenPopupRevive(
            //     () =>
            //     {
            //         SceneLoader.ReloadScene();
            //     }, () =>
            //     {
            //         SoundInGameManager.Instance.PlayLoseSound(Lose);
            //     },GameDataManager.Instance.GetCurrentSkin());
        }

        void Lose()
        {
            InGamePopupController.Instance.ShowPopupWin(gameInfo);
        }

        public override void Win()
        {
            StopAllCoroutines();
            state = GameState.End;
            animBoss.state.SetAnimation(0, idle, true);
            warningSprite.GetComponent<DOTweenAnimation>().DOKill(true);
            playerTransform.DOMoveX(end.position.x, moveSpeed)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnStart(() =>
                {
                    playerAnimation.PlayRun();
                    playerAnimation.PlayRun(trackIndex: 1);
                });
            StartCoroutine(Cheers());
        }

        IEnumerator Cheers()
        {
            yield return new WaitForSeconds(2f);
            playerAnimation.PlayWin();
            playerAnimation.PlayWin(trackIndex: 1);
            audio.outputAudioMixerGroup = pitchBend;
            yield return new WaitForSeconds(1.5f);
            InGamePopupController.Instance.ShowPopupWin(gameInfo);
        }

        private int meteorFallCount;
        private float timeToMeteorFall;
        private float closestXPos = 100;

        private void Update()
        {
            if (state == GameState.Playing)
            {

                if (HitFinishLine())
                {
                    playerInfo.isFinish = true;
                    LeaderBoardInGame.finishAction.Invoke(playerInfo);
                    LeaderBoardInGame.Instance.Show(.75f);
                    Win();
                }

                if (!HitFinishLine() && HitByMeteor())
                {
                    KillBall();
                }

                if (gameTimeCount < gameDuration)
                {
                    gameTimeCount += Time.deltaTime;
                    gameTimeCountText.text = TimeSpan.FromSeconds(gameDuration - gameTimeCount).ToString(@"mm\:ss");
                }
                else if (!canKillBall)
                {
                    canKillBall = true;
                    animStaff1.state.SetAnimation(0, "attack", false).Complete += entry =>
                    {
                        animStaff1.state.SetAnimation(0, "idle", true);
                    };
                }

                if (isHoldingMove && (playerTransform.position.x >= end.position.x || !HitFinishLine()))
                {
                    playerTransform.position = Vector3.MoveTowards(playerTransform.position, end.position,
                        moveSpeed * Time.deltaTime);
                }
            }
        }


        bool HitByMeteor()
        {
            RaycastHit2D hit = Physics2D.Raycast(playerAnimation.transform.position + new Vector3(0, .1f), Vector2.left,
                .2f, 1 << 9);
            if (hit)
            {
                return hit;
            }

            return false;
        }

        bool HitFinishLine()
        {
            RaycastHit2D hit = Physics2D.Raycast(playerAnimation.transform.position, Vector2.left, .2f, 1 << 0);
            if (hit)
            {
                Debug.Log("hit collider " + hit.collider.name);
                return hit;
            }

            return false;
        }

        public void TouchMove(bool release = false)
        {
            if (state != GameState.End)
            {
                isHoldingMove = !release;
                if (release)
                {
                    playerAnimation.PlayStopPose();
                }
                else
                {
                    if (Random.value > 0.75f)
                    {
                        playerAnimation.PlayRun();
                        if (Random.value > 0.875f)
                        {
                            playerAnimation.PlayDodge();
                        }
                        else
                        {
                            playerAnimation.PlayRunNaruto();
                        }
                    }
                    else
                    {
                        playerAnimation.PlayRun();
                        playerAnimation.PlayRun(trackIndex: 1);
                    }
                }
            }
        }
    }
}