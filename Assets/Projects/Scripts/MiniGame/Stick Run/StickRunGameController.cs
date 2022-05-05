using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThirdParties.Truongtv;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniGame.StickRun
{
    public class StickRunGameController : MonoBehaviour
    {
        public GameObject holdToRunFastText;
        public List<StickmanPlayerController> listBot = new List<StickmanPlayerController>();
        public StickmanPlayerController botPrefabs;
        public GameDifficulty difficulty;
        public GameState state = GameState.None;
        public StickmanPlayerController player;
        public StickmanPlayerController playerPrefab;
        public Vector2 startPos;
        [SerializeField] private int maxTimeRevive = 3;
        [SerializeField] private int maxBotNumbers = 3;
        private List<BotSkin> cacheBotSkin = new List<BotSkin>();
        public static StickRunGameController Instance
        {
            get => instance;
        }

        private int deadTime;
        private Pool trapPool;
        private Transform checkPoint;
        private static StickRunGameController instance = null;
        private float deltaDifficulty;
        private bool isHoldingSprint, isFirstTap;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance.gameObject);
            }
        }

        private void Start()
        {
            StartCoroutine(Init());
        }

        IEnumerator Init()
        {
            deadTime = 0;
            //gen map
            
            //init bot
            for (int i = 0; i < maxBotNumbers; i++)
            {
                var listSkin = GameDataManager.Instance.RandomSkinList();
                var color = GameDataManager.Instance.RandomColor();
                cacheBotSkin.Add(new BotSkin(color,listSkin));
                var bot = Instantiate(botPrefabs);
                yield return new WaitUntil(() => bot);
                bot.transform.position = startPos;
                var botController = bot.GetComponent<StickmanPlayerController>();
                var difficulty = (int) this.difficulty;
                botController.Init(listSkin,color,(BotDifficulty) difficulty);
                listBot.Add(botController);
            }
            yield return new WaitUntil(() => isFirstTap);
            holdToRunFastText.gameObject.SetActive(false);
            state = GameState.Playing;
        }

        public void Dead()
        {
            deadTime++;
        }
        
        public bool CheckCanRevive()
        {
            return deadTime <= maxTimeRevive;
        }

        public async void Win()
        {
            state = GameState.End;
            await Task.Delay(2000);
            GameServiceManager.ShowInterstitialAd(() =>
            {
                GameDataManager.Instance.ResetSkinInGame();
                SceneManager.LoadScene("Menu");
            });
        }
        
        public void EndGame()
        {
            state = GameState.End;
            GameServiceManager.ShowInterstitialAd(() =>
            {
                GameDataManager.Instance.ResetSkinInGame();
                SceneManager.LoadScene("Menu");
            });
        }
        
        private void Update()
        {
            if (state == GameState.Playing)
            {
                player.TouchSprint(isHoldingSprint);
            }
        }

        public void Tap()
        {
            isFirstTap = true;
        }
        
        public void TouchSprint(bool release = false)
        {
            if (state != GameState.End)
            {
                isHoldingSprint = !release;
            }
        }
    }
}