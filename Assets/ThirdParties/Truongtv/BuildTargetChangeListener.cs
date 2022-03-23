#if UNITY_EDITOR

using System;
using System.IO;
using System.Xml;
using ThirdParties.Truongtv.Utilities;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace ThirdParties.Truongtv
{
    public class BuildTargetChangeListener:IActiveBuildTargetChanged
    {
        private const string XmlPath = "ThirdParties/Truongtv/CustomService.xml";
        
        public int callbackOrder { get; }
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            Debug.Log("OnActiveBuildTargetChanged "+newTarget);
            OnAdServiceChange(GetProperties("adService"));
            OnLogServiceChange(GetProperties("log"));
            OnRemoteServiceChange(GetProperties("remoteConfig"));
            OnRateServiceChange(GetProperties("rate"));
        }
        private string GetProperties(string property)
        {
            var path = Path.Combine(Application.dataPath, XmlPath);
            var document = new XmlDocument();
            document.Load(path);
            var node = document.SelectSingleNode(property);
            return node == null ? string.Empty : node.InnerText;
        }
        private void OnAdServiceChange(string data)
        {
            var adService = data==string.Empty? AdService.None:(AdService)Enum.Parse(typeof(AdService), data);
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
            SaveProperties("adService", adService.ToString());
            DefineSymbol.UpdateDefineSymbols(symbolList);
        }

        private void OnLogServiceChange(string data)
        {
            var logService = data==string.Empty? LogService.None:(LogService)Enum.Parse(typeof(LogService), data);
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

        private void OnRemoteServiceChange(string data)
        {
            var remoteConfigService = data==string.Empty? RemoteConfigService.None:(RemoteConfigService)Enum.Parse(typeof(RemoteConfigService), data);
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
            SaveProperties("remoteConfig", remoteConfigService.ToString());
            DefineSymbol.UpdateDefineSymbols(symbolList);
        }

        private void OnRateServiceChange(string data)
        {
            var ratingService = data==string.Empty? RatingService.OpenLink:(RatingService)Enum.Parse(typeof(RatingService), data);
            var symbolList = DefineSymbol.GetAllDefineSymbols();
            symbolList.Remove(DefineSymbol.InAppReview);
            if (ratingService == RatingService.InApp)
            {
                symbolList.Add(DefineSymbol.InAppReview);
            }
            SaveProperties("rate",  ratingService.ToString());
            DefineSymbol.UpdateDefineSymbols(symbolList);
        }
        private static void SaveProperties(string property, string value)
        {
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
    }
}

#endif