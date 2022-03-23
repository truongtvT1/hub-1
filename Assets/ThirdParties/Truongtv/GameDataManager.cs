﻿using Sirenix.OdinInspector;
using ThirdParties.Truongtv.RemoteConfig;
using UnityEngine;

namespace ThirdParties.Truongtv
{
   
    public class GameDataManager : MonoBehaviour
    {
        [SerializeField,FoldoutGroup("Game Setting")] public bool cheated = false;
        [SerializeField, FoldoutGroup("Game Setting")] public bool debugger = false;
        [HideInInspector]public string versionReview;
        private static GameDataManager _instance;
        public static GameDataManager Instance => _instance;
        
        
        private void Awake()
        {
            if (_instance != null)
                Destroy(_instance.gameObject);
            _instance = this ;
        }
        private void Start()
        {
            Application.targetFrameRate = 300;
            versionReview = Application.version;
            GameServiceManager.FetchComplete += OnFetchDataComplete;
        }
        
        public bool IsPurchaseBlockAd()
        {
            return false;
        }

        private void OnFetchDataComplete(RemoteConfigManager manager)
        {
            
        }
    }
    
}