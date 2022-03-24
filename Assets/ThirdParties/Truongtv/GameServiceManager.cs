﻿using System;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv.AdsManager;
using ThirdParties.Truongtv.LogManager;
using ThirdParties.Truongtv.Notification;
using ThirdParties.Truongtv.Rating;
using ThirdParties.Truongtv.RemoteConfig;
using ThirdParties.Truongtv.Utilities;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using System.Xml;
#endif
namespace ThirdParties.Truongtv
{
    [RequireComponent(typeof(AdManager))]
    [RequireComponent(typeof(LogEventManager))]
    [RequireComponent(typeof(RatingHelper))]
    [RequireComponent(typeof(RemoteConfigManager))]
    [RequireComponent(typeof(MobileNotification))]
    public class GameServiceManager : MonoBehaviour
    {
        [SerializeField, OnValueChanged(nameof(OnAdServiceChange))]
        private AdService adService;
        [SerializeField, OnValueChanged(nameof(OnLogServiceChange))]
        private LogService logService;
        [SerializeField, OnValueChanged(nameof(OnRemoteServiceChange))]
        private RemoteConfigService remoteConfigService;
        [SerializeField, OnValueChanged(nameof(OnRateServiceChange))]
        private RatingService ratingService;
        [SerializeField, OnValueChanged(nameof(OnCloudMessagingServiceChange))]
        private CloudMessagingService cloudMessagingService;
        [HideInInspector] public AdManager adManager;
        [HideInInspector] public LogEventManager logEventManager;
        [HideInInspector] public RatingHelper ratingHelper;
        [HideInInspector] public RemoteConfigManager remoteConfigManager;
        [HideInInspector] public MobileNotification mobileNotification;
        public static Action<RemoteConfigManager> FetchComplete;
        
        public static GameServiceManager Instance;

        public void Awake()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            Instance = this;
            if (Application.isPlaying)
                DontDestroyOnLoad(gameObject);
            adManager = GetComponent<AdManager>();
            logEventManager = GetComponent<LogEventManager>();
            ratingHelper = GetComponent<RatingHelper>();
            remoteConfigManager = GetComponent<RemoteConfigManager>();
            mobileNotification = GetComponent<MobileNotification>();
        }

        private void Start()
        {
            remoteConfigManager.fetchComplete += FetchComplete;
            adManager.Init();
            #if USING_LOG_FIREBASE||USING_REMOTE_FIREBASE
             FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                remoteConfigManager.Init();
                logEventManager.Init();
                mobileNotification.Init();
            });
            #else
            remoteConfigManager.Init();
            logEventManager.Init();
            mobileNotification.Init();
            #endif
            
        }
#if UNITY_EDITOR
        private void OnAdServiceChange()
        {
            var symbolList = DefineSymbol.GetAllDefineSymbols();
            symbolList.Remove(DefineSymbol.MaxSymbol);
            symbolList.Remove(DefineSymbol.AdMobSymbol);
            symbolList.Remove(DefineSymbol.IronSourceSymbol);
            switch (adService)
            {
                case AdService.None:
                    break;
                case AdService.AdMob:
                    symbolList.Add(DefineSymbol.AdMobSymbol);
                    break;
                case AdService.IronSource:

                    symbolList.Add(DefineSymbol.IronSourceSymbol);
                    break;
                case AdService.Max:

                    symbolList.Add(DefineSymbol.MaxSymbol);
                    break;
            }

            SaveProperties("adS_service", adService.ToString());
            DefineSymbol.UpdateDefineSymbols(symbolList);
        }

        private void OnLogServiceChange()
        {
            var symbolList = DefineSymbol.GetAllDefineSymbols();
            symbolList.Remove(DefineSymbol.LogUnity);
            symbolList.Remove(DefineSymbol.LogFirebase);
            switch (logService)
            {
                case LogService.None:
                    break;
                case LogService.Firebase:
                    symbolList.Add(DefineSymbol.LogFirebase);
                    break;
                case LogService.Unity:
                    symbolList.Add(DefineSymbol.LogUnity);
                    break;
            }

            SaveProperties("log", logService.ToString());
            DefineSymbol.UpdateDefineSymbols(symbolList);
        }

        private void OnRemoteServiceChange()
        {
            var symbolList = DefineSymbol.GetAllDefineSymbols();
            symbolList.Remove(DefineSymbol.RemoteFirebase);
            symbolList.Remove(DefineSymbol.RemoteUnity);
            switch (remoteConfigService)
            {
                case RemoteConfigService.None:
                    break;
                case RemoteConfigService.Firebase:
                    symbolList.Add(DefineSymbol.RemoteFirebase);
                    break;
                case RemoteConfigService.Unity:
                    symbolList.Add(DefineSymbol.RemoteUnity);
                    break;
            }

            SaveProperties("remote_config", remoteConfigService.ToString());
            DefineSymbol.UpdateDefineSymbols(symbolList);
        }

        private void OnRateServiceChange()
        {
            var symbolList = DefineSymbol.GetAllDefineSymbols();
            symbolList.Remove(DefineSymbol.InAppReview);
            if (ratingService == RatingService.InApp)
            {
                symbolList.Add(DefineSymbol.InAppReview);
            }

            SaveProperties("rate", ratingService.ToString());
            DefineSymbol.UpdateDefineSymbols(symbolList);
        }

        private void OnCloudMessagingServiceChange()
        {
            var symbolList = DefineSymbol.GetAllDefineSymbols();
            symbolList.Remove(DefineSymbol.InAppReview);
            if (cloudMessagingService == CloudMessagingService.Firebase)
            {
                symbolList.Add(DefineSymbol.FirebaseMessaging);
            }

            SaveProperties("cloud_message", cloudMessagingService.ToString());
            DefineSymbol.UpdateDefineSymbols(symbolList);
        }

        private static void SaveProperties(string property, string value)
        {
            var XmlPath = "ThirdParties/Truongtv/CustomService.xml";
            var path = Path.Combine(Application.dataPath, XmlPath);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            var document = new XmlDocument();
            document.Load(path);
            var node = document.DocumentElement.SelectSingleNode(property);
            if (node == null)
            {
                node = document.CreateNode(XmlNodeType.Element, property, "");
                node.InnerText = value;
                document.DocumentElement.AppendChild(node);
            }

            node.InnerText = value;
            document.Save(path);
        }
#endif
    }

    enum LogService
    {
        None,
        Firebase,
        Unity
    }

    enum AdService
    {
        None,
        AdMob,
        IronSource,
        Max
    }

    enum RemoteConfigService
    {
        None,
        Firebase,
        Unity
    }

    enum RatingService
    {
        OpenLink,
        InApp
    }

    enum CloudMessagingService
    {
        None,Firebase
    }
}