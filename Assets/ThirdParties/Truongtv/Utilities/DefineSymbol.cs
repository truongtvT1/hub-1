#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace ThirdParties.Truongtv.Utilities
{
    public static class DefineSymbol
    {
        #region Ad symbol
        public const string IronSourceSymbol = "USING_IRON_SOURCE";
        public const string AdMobSymbol = "USING_ADMOB";
        public const string MaxSymbol = "USING_MAX";
        #endregion


        #region Log Symbol
        public const string LogUnity = "USING_LOG_UNITY";
        public const string LogFirebase = "USING_LOG_FIREBASE";
        #endregion

        #region Remote config Symbol

        public const string RemoteUnity = "USING_REMOTE_UNITY";
        public const string RemoteFirebase = "USING_REMOTE_FIREBASE";

        #endregion

        #region Rate

        public const string InAppReview = "USING_IN_APP_REVIEW";

        #endregion

        private static string GetAllDefineSymbol()
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            return defines;
        }
        public static void UpdateDefineSymbols(IEnumerable<string> list)
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = list.Aggregate("", (current, variable) => current + variable + ";");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup,defines);
        }
        public static List<string> GetAllDefineSymbols()
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var list = defines.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries).ToList();
            return list;
        }
        public static bool Contain(string value)
        {
            var total = GetAllDefineSymbol();
            return total.Contains(value);
        }

        public static void RemoveSymbol(string value)
        {
            var totals = GetAllDefineSymbols();
            totals.Remove(value);
            UpdateDefineSymbols(totals);
        }
    }
}
#endif