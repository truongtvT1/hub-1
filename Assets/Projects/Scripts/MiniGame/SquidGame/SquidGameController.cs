using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Projects.Scripts.Hub;
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
    public class SquidGameController : MonoBehaviour
    {
        [FoldoutGroup("UI")] public Transform holdToMove, container;
        [FoldoutGroup("UI")] public SpriteRenderer warningSprite;
        [FoldoutGroup("UI")] public TextMeshProUGUI gameTimeCountText;
        [FoldoutGroup("UI")] public Image greenProgress;
        [FoldoutGroup("UI")] public Button pauseButton;
        public float gameDuration, greenDuration, redDuration, moveSpeed;
        [Range(0, .3f)] public float difficultyDelta;
        [SpineAnimation(dataField = nameof(animBoss))]
        public string idle, redLight, greenLight, warning;
        public SkeletonAnimation animBoss, animStaff1, animStaff2;

        public CharacterAnimation playerAnimation;
        public GameObject shotFx;
        public ParticleSystem blowEffect;
        public Transform start, end, playerTransform;
        public Action onRedLight, onGreenLight;
        public List<SquidPlayer> listNPC;
        public AudioSource audio;
        public AudioMixerGroup pitchBend;
        public AudioClip greenLightSound, redLightSound;
        public int playerRank;
        public GameState state = GameState.None;
        public static SquidGameController Instance => _instance;
        private static SquidGameController _instance;
        private bool isHoldingMove;
        private float gameTimeCount, backUpPitchShift, progressImageWitdh;
        private int level;
        private Color cacheColor;

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
            pauseButton.onClick.AddListener(() =>
            { 
                Pause();
                InGamePopupController.Instance.ShowPopupSetting(() =>
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                },
                () =>
                {
                    // update result
                    
                },
                () =>
                {
                    Resume();
                });
            });
            audio.outputAudioMixerGroup = pitchBend;
            audio.clip = greenLightSound;
            pitchBend.audioMixer.GetFloat("pitchBend", out float pitchBendValue);
            backUpPitchShift = pitchBendValue;
            cacheColor = greenProgress.color;
            progressImageWitdh = greenProgress.GetComponent<RectTransform>().rect.width;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            pitchBend.audioMixer.SetFloat("pitchBend", backUpPitchShift);
        }

        private void Start()
        {
            // level = GameDataManager.Instance.GetSquidGameLevel();
            // difficultyDelta = difficultyDelta + GameDataManager.Instance.GetSquidLevelDifficulty();
            StartCoroutine(Init());
            GameServiceManager.LogEvent("level_start", new Dictionary<string, object>{{"red_line",level}});
        }

        public void Pause()
        {
            if (state == GameState.None) return;
            audio.Pause();
            state = GameState.Pause;
            foreach (var npc in listNPC)
            {
                npc.Pause();
            }
        }

        public void Resume()
        {
            if (state == GameState.None) return;
            audio.UnPause();
            state = GameState.Playing;
            foreach (var npc in listNPC)
            {
                npc.Resume();
            }
        }

        IEnumerator Init()
        {
            greenProgress.rectTransform.localPosition = new Vector3(-progressImageWitdh,greenProgress.rectTransform.localPosition.y);
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
            
            var distance = Random.Range(1f, 2f);
            playerTransform.position = start.position + new Vector3(distance, 0);
            playerTransform.DOMoveX(start.position.x, moveSpeed)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnStart(() =>
                {
                    playerAnimation.PlayRun();
                })
                .OnComplete(() =>
                {
                    playerAnimation.PlayIdle();
                });

            //init npc
            for (int i = 0; i < listNPC.Count; i++)
            {
                var listSkin = GameDataManager.Instance.RandomSkinList();
                var skinColor = GameDataManager.Instance.RandomColor();
                listNPC[i].Init(this,listSkin,skinColor,difficultyDelta);
            }
            yield return new WaitUntil(() => !listNPC.Exists(p => !p.isReady));
            holdToMove.gameObject.SetActive(true);
            holdToMove.transform.parent.GetComponent<CustomButton>().enabled = true;
            
            //wait for player start
            yield return new WaitUntil(() => isHoldingMove);
            Debug.Log("start game");
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
            if (e.Data.Name.Equals("fire"))
            {
                // shotFx[1].gameObject.SetActive(true);
                // shotFx[1].Play(true);
            }
        }

        private void StateOnEvent1(TrackEntry trackentry, Event e)
        {
            if (e.Data.Name.Equals("fire"))
            {
                var shot = Instantiate(shotFx);
                shot.transform.position = animStaff1.transform.GetChild(0).transform.position;
                // shot.transform.
                // shotFx[0].gameObject.SetActive(true);
                // shotFx[0].Play(true);
            }
        }

        void GreenLight()
        {
            animBoss.state.SetAnimation(0, greenLight, false).Complete += entry =>
            {
                onGreenLight?.Invoke();
                animBoss.state.SetAnimation(0, greenLight, true);
            };
            greenProgress.color = cacheColor;
            warningSprite.DOFade(0, 0);
            StopAllCoroutines();
            var isWarning = false;
            audio.Play();
            StartCoroutine(CountTime(greenDuration, 0, count =>
            {
                if (count / greenDuration >= 2 / 3f && !isWarning)
                {
                    greenProgress.color = Color.red;
                    isWarning = true;
                    warningSprite.DOFade(50 / 255f, .3f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                    animBoss.state.SetAnimation(0, warning, true);
                }

                greenProgress.rectTransform.localPosition = new Vector3(progressImageWitdh * count / greenDuration - progressImageWitdh, greenProgress.rectTransform.localPosition.y);
            }, () =>
            {
                if (state != GameState.End)
                {
                    warningSprite.DOKill();
                    var pitchShift = audio.pitch * (1 + difficultyDelta);
                    audio.pitch = pitchShift;
                    pitchBend.audioMixer.SetFloat("pitchBend", 1f / pitchShift);
                    greenDuration -= greenDuration * difficultyDelta;
                    RedLight();
                }
            }));
        }


        private List<SquidPlayer> npcToKill;
        void RedLight()
        {
            // SoundManager.Instance.PlaySfx(redLightSound);
            animBoss.state.SetAnimation(0, redLight, false).Complete += entry =>
            {
                onRedLight?.Invoke();
                animBoss.state.SetAnimation(0, idle, true);
            };
            warningSprite.DOFade(0 / 255f, 0);
            StopAllCoroutines();
            StartCoroutine(CountTime(redDuration, .5f, f =>
            {
                if (state != GameState.End)
                {
                    //check if any player move
                    for (int i = 0; i < listNPC.Count; i++)
                    {
                        if (!listNPC[i].isWin && listNPC[i].isMoving)
                        {
                            var rd = Random.value > 0.5f;
                            if (rd)
                            {
                                animStaff1.state.SetAnimation(0, "attack", false)
                                    .Complete += entry =>
                                {
                                    // shotFx[rd].gameObject.SetActive(false);
                                    animStaff1.state.SetAnimation(0, "idle", true);
                                };
                                listNPC[i].Kill();
                            }
                            else
                            {
                                animStaff2.state.SetAnimation(0, "attack", false)
                                    .Complete += entry =>
                                {
                                    // shotFx[rd].gameObject.SetActive(false);
                                    animStaff2.state.SetAnimation(0, "idle", true);
                                };
                                listNPC[i].Kill();
                            }
                        }
                    }

                    if (isHoldingMove && !HitFinishLine())
                    {
                        KillBall();
                    }
                }
            }, () =>
            {
                if (state != GameState.End)
                {
                    // SoundManager.Instance.PlaySfx(redLightSound);
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
            var rd = Random.Range(0, 2);
            if (rd == 0)
            {
                animStaff1.state.SetAnimation(0, "attack", false).Complete += trackEntry =>
                {
                    // shotFx[rd].gameObject.SetActive(false);
                    animStaff1.state.SetAnimation(0, "idle", true);
                };
            }
            else
            {
                animStaff2.state.SetAnimation(0, "attack", false).Complete += trackEntry =>
                {
                    // shotFx[rd].gameObject.SetActive(false);
                    animStaff2.state.SetAnimation(0, "idle", true);
                };
            }

            OnAttack();
        }

        private void OnAttack()
        {
            state = GameState.End;
            //lose
            // blowEffect.gameObject.SetActive(true);
            // blowEffect.Play(true);
            playerAnimation.PlayDie(callback: () =>
            {
                //revive
                // Debug.Log("ball die");
                // PopupInGameController.Instance.OpenPopupRevive(
                //     () =>
                //     {
                //         SceneLoader.ReloadScene();
                //     }, () =>
                //     {
                //         SoundInGameManager.Instance.PlayLoseSound(Lose);
                //     },GameDataManager.Instance.GetCurrentSkin());
                
            });
        }

        void Lose()
        {
            GameServiceManager.ShowInterstitialAd(() =>
            {
                // SceneLoader.LoadMenu();
            });
        }

        void Win()
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
                });
            StartCoroutine(Cheers());
        }

        IEnumerator Cheers()
        {
            yield return new WaitForSeconds(2f);
            playerAnimation.PlayWin();
            audio.outputAudioMixerGroup = pitchBend;
            // SoundInGameManager.Instance.PlayBallWinSound();
            // SoundInGameManager.Instance.PlayWinSound(() =>
            // {
            //     //load popup win
            //     int coin = 0;
            //     if (finalRank == 1)
            //     {
            //         coin = 2000 + level * 100;
            //     }
            //     else if (finalRank == 2)
            //     {
            //         coin = 1500 + level * 100;
            //
            //     }
            //     else if (finalRank == 3)
            //     {
            //         coin = 1000 + level * 100;
            //     }
            //     else
            //     {
            //         coin = 500;
            //     }
            //     PopupInGameController.Instance.OpenPopupWin(finalRank,coin,level, () =>
            //         {
            //             GameDataManager.Instance.UpdateCoin(coin * 2);
            //             LoadMainMenu();
            //         },
            //         () =>
            //         {
            //             AdManager.Instance.ShowInterstitialAd(() =>
            //             {
            //                 GameDataManager.Instance.UpdateCoin(coin);
            //                 LoadMainMenu();
            //             });
            //         });
            // });
        }

        void LoadMainMenu()
        {
            // GameDataManager.Instance.SetSquidGameLevel(level + 1);
            // Debug.Log("current squid level: " + (level + 1));
            // SceneLoader.LoadMenu();
        }

        private int finalRank = 0;

        private void Update()
        {
            if (state == GameState.Playing)
            {
                if (finalRank != playerRank)
                {
                    finalRank = playerRank;
                }

                if (HitFinishLine())
                {
                    playerRank++;
                    finalRank = playerRank;
                    Win();
                }

                if (gameTimeCount < gameDuration)
                {
                    gameTimeCount += Time.deltaTime;
                    gameTimeCountText.text = TimeSpan.FromSeconds(gameDuration - gameTimeCount).ToString(@"mm\:ss");
                }
                else
                {
                    KillBall();
                }

                if (isHoldingMove && (playerTransform.position.x >= end.position.x || !HitFinishLine()))
                {
                    playerTransform.position = Vector3.MoveTowards(playerTransform.position, end.position, moveSpeed * Time.deltaTime);
                    playerAnimation.PlayRun();
                }
            }
        }


        bool HitFinishLine()
        {
            RaycastHit2D hit = Physics2D.Raycast(playerAnimation.transform.position, Vector2.left, .2f);
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
            }
        }
    }
}