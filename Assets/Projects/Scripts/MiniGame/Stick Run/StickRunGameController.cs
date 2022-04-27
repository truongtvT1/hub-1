using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame.StickRun
{
    public class StickRunGameController : MonoBehaviour
    {
        public GameObject holdToRunFastText;
        public List<StickmanPlayerController> listBot;
        public GameDifficulty difficulty;
        public GameState state = GameState.None;
        public StickmanPlayerController player;
        public StickmanPlayerController playerPrefab;
        public static StickRunGameController Instance
        {
            get => instance;
        }

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
            //gen map
            
            //init bot
            yield return new WaitUntil(() => isFirstTap);
            holdToRunFastText.gameObject.SetActive(false);
            state = GameState.Playing;
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