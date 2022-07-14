using System.Collections.Generic;
using UnityEngine;

namespace Projects.Scripts.Data
{
    [CreateAssetMenu(fileName = "LogEventConfig", menuName = "Truongtv/GameSetting/LogEventConfig", order = 0)]
    public class LogEventConfig : ScriptableObject
    {
        public string levelStart,
            levelWin,
            levelLose,
            interstitial,
            rewardAdClick,
            rewardAdComplete,
            rewardAdFail,
            rewardForShopFreeTicket,
            rewardForShopFreeDrawChest,
            rewardForCustomSkinColor,
            rewardForCustomTrySkin,
            rewardForCustomUnlockSkin,
            rewardForBonusWin;
    }
}