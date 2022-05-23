using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Base;
using Com.LuisPedroFonseca.ProCamera2D;
using Projects.Scripts.Hub;
using ThirdParties.Truongtv;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MiniGame.StickRun
{
    public class StickRunGameController : MonoBehaviour
    {
        public Button settingButton;
        public GameObject holdToRunFastText;
        public List<StickmanPlayerController> listBot = new List<StickmanPlayerController>();
        public StickmanPlayerController botPrefabs;
        public GameDifficulty difficulty;
        public GameState state = GameState.None;
        public StickmanPlayerController player;
        public StickmanPlayerController playerPrefab;
        public Vector2 startPos;
        public Pool[] trapPoolEasy;
        public Pool[] trapPoolNormal;
        public Pool[] trapPoolHard;
        public Pool[] trapPoolHell;
        public GameObject gateFinish;
        [SerializeField] private int maxTimeRevive = 3;
        [SerializeField] private int maxBotNumbers = 3;
        private List<PlayerSkin> cacheBotSkin = new List<PlayerSkin>();
        public static StickRunGameController Instance
        {
            get => instance;
        }

        private int deadTime;
        private Transform checkPoint;
        private static StickRunGameController instance = null;
        private float deltaDifficulty, mapLength;
        private bool isHoldingSprint, isFirstTap;
        
        
        private const float _mapStartOffset = 14f;
        private const float OBJECT_AVGWIDTH = 0f;
        private const int EASY_OBJECT_AMOUNT = 6;
        private const int NORMAL_OBJECT_AMOUNT = 7;
        private const int HARD_OBJECT_AMOUNT = 8;
        private const int HELL_OBJECT_AMOUNT = 9;
        private const float EASY_MAPLENGTH= 110f;
        private const float NORMAL_MAPLENGTH = 120f;
        private const float HARD_MAPLENGTH = 130f;
        private const float HELL_MAPLENGTH = 140f;
        // private const float EASY_MINDISTANCE = 10f;
        // private const float NORMAL_MINDISTANCE = 8f;
        // private const float HARD_MINDISTANCE = 7f;
        // private const float HELL_MINDISTANCE = 5f;
        // private const float EASY_MAXDISTANCE = 15f;
        // private const float NORMAL_MAXDISTANCE = 12f;
        // private const float HARD_MAXDISTANCE = 10f;
        // private const float HELL_MAXDISTANCE = 8f;
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
            settingButton.onClick.AddListener(() =>
            {
                InGamePopupController.Instance.ShowPopupSetting(() =>
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }, () =>
                {
                    Loading.Instance.LoadMenu();
                }, null);
            });
        }

        private void Start()
        {
            StartCoroutine(Init());
        }

        IEnumerator Init()
        {
            deadTime = 0;
            var camBoundaries = ProCamera2D.Instance.GetComponent<ProCamera2DNumericBoundaries>();
            //gen map
            int count = 0;
            float pos = _mapStartOffset;
            float delta = 3f; //sai so ngau nhien khoang cach giua cac obj
            var gate = Instantiate(gateFinish, null, true);
            GameObject lastObj = null;
            switch (difficulty)
            {
                case GameDifficulty.Easy:
                    camBoundaries.RightBoundary = EASY_MAPLENGTH + _mapStartOffset;
                    gate.transform.position = new Vector3(camBoundaries.RightBoundary - 5f, gate.transform.position.y, gate.transform.position.z);
                    while (count < EASY_OBJECT_AMOUNT && pos <= camBoundaries.RightBoundary - 20)
                    {
                        var trap = trapPoolEasy[Random.Range(0, trapPoolEasy.Length)].nextThing;
                        yield return new WaitUntil(() => trap);
                        if (lastObj != null){
                            if (lastObj.name.Contains("Wood Ninja") && trap.name.Contains("Wood Ninja") 
                                || lastObj.name.Contains("Underground") && trap.name.Contains("Underground") 
                                || lastObj.name.Contains("Roll") && trap.name.Contains("Roll"))
                            {
                                continue;
                            }
                        }
                        lastObj = trap;
                        if (trap.TryGetComponent(out AutoMoving movingItem))
                        {
                            movingItem.UpdateListPoint();
                        }
                        trap.transform.position = new Vector3(pos, trap.transform.position.y,trap.transform.position.z);
                        pos += OBJECT_AVGWIDTH + EASY_MAPLENGTH/EASY_OBJECT_AMOUNT + Random.Range(-delta, delta);
                        count++;
                    }
                    break;
                case GameDifficulty.Normal: 
                    camBoundaries.RightBoundary = NORMAL_MAPLENGTH + _mapStartOffset;
                    gate.transform.position = new Vector3(camBoundaries.RightBoundary - 5f, gate.transform.position.y, gate.transform.position.z);
                    while (count < NORMAL_OBJECT_AMOUNT && pos <= camBoundaries.RightBoundary - 20)
                    {
                        var trap = trapPoolNormal[Random.Range(0, trapPoolNormal.Length)].nextThing;
                        yield return new WaitUntil(() => trap);
                        if (lastObj != null){
                            if (lastObj.name.Contains("Wood Ninja") && trap.name.Contains("Wood Ninja") 
                            || lastObj.name.Contains("Underground") && trap.name.Contains("Underground") 
                            || lastObj.name.Contains("Roll") && trap.name.Contains("Roll"))
                            {
                                continue;
                            }
                        }
                        lastObj = trap;
                        if (trap.TryGetComponent(out AutoMoving movingItem))
                        {
                            movingItem.UpdateListPoint();
                        }
                        trap.transform.position = new Vector3(pos, trap.transform.position.y,trap.transform.position.z);
                        pos += OBJECT_AVGWIDTH + NORMAL_MAPLENGTH/NORMAL_OBJECT_AMOUNT + Random.Range(-delta, delta);
                        count++;
                    }
                    break;
                case GameDifficulty.Hard: 
                    camBoundaries.RightBoundary = HARD_MAPLENGTH + _mapStartOffset;
                    gate.transform.position = new Vector3(camBoundaries.RightBoundary - 5f, gate.transform.position.y, gate.transform.position.z);
                    while (count < HARD_OBJECT_AMOUNT && pos <= camBoundaries.RightBoundary - 20)
                    {
                        var trap = trapPoolHard[Random.Range(0, trapPoolHard.Length)].nextThing;
                        yield return new WaitUntil(() => trap);
                        if (lastObj != null){
                            if (lastObj.name.Contains("Wood Ninja") && trap.name.Contains("Wood Ninja") 
                                || lastObj.name.Contains("Underground") && trap.name.Contains("Underground") 
                                || lastObj.name.Contains("Roll") && trap.name.Contains("Roll"))
                            {
                                continue;
                            }
                        }
                        lastObj = trap;
                        if (trap.TryGetComponent(out AutoMoving movingItem))
                        {
                            movingItem.UpdateListPoint();
                        }
                        trap.transform.position = new Vector3(pos, trap.transform.position.y,trap.transform.position.z);
                        pos += OBJECT_AVGWIDTH + HARD_MAPLENGTH/HARD_OBJECT_AMOUNT + Random.Range(-delta, delta);
                        count++;
                    }
                    break;
                case GameDifficulty.Hell: 
                    camBoundaries.RightBoundary = HELL_MAPLENGTH + _mapStartOffset;
                    gate.transform.position = new Vector3(camBoundaries.RightBoundary - 5f, gate.transform.position.y, gate.transform.position.z);
                    while (count < HELL_OBJECT_AMOUNT && pos <= camBoundaries.RightBoundary - 20)
                    {
                        var trap = trapPoolHell[Random.Range(0, trapPoolHell.Length)].nextThing;
                        yield return new WaitUntil(() => trap);
                        if (lastObj != null){
                            if (lastObj.name.Contains("Wood Ninja") && trap.name.Contains("Wood Ninja") 
                                || lastObj.name.Contains("Underground") && trap.name.Contains("Underground") 
                                || lastObj.name.Contains("Roll") && trap.name.Contains("Roll"))
                            {
                                continue;
                            }
                        }
                        lastObj = trap;
                        if (trap.TryGetComponent(out AutoMoving movingItem))
                        {
                            movingItem.UpdateListPoint();
                        }
                        trap.transform.position = new Vector3(pos, trap.transform.position.y,trap.transform.position.z);
                        pos += OBJECT_AVGWIDTH + HELL_MAPLENGTH/HELL_OBJECT_AMOUNT + Random.Range(-delta, delta);
                        count++;
                    }
                    break;
            }
            
            
            //init bot
            for (int i = 0; i < maxBotNumbers; i++)
            {
                var listSkin = GameDataManager.Instance.RandomSkinList();
                var color = GameDataManager.Instance.RandomColor();
                cacheBotSkin.Add(new PlayerSkin(color,listSkin));
                var bot = Instantiate(botPrefabs);
                bot.name = "bot" + i;
                yield return new WaitUntil(() => bot);
                bot.transform.position = startPos + new Vector2(Random.Range(-1,2) * i, 0);
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
                Loading.Instance.LoadMenu();
            });
        }
        
        public void EndGame()
        {
            state = GameState.End;
            GameServiceManager.ShowInterstitialAd(() =>
            {
                GameDataManager.Instance.ResetSkinInGame();
                Loading.Instance.LoadMenu();
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