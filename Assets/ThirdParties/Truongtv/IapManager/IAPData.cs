using Sirenix.OdinInspector;
using UnityEngine;

namespace ThirdParties.Truongtv.IapManager
{
    [CreateAssetMenu(fileName = "IAPData", menuName = "Truongtv/GameSetting/IAPData", order = 1)]
    public class IAPData : ScriptableObject
    {

        [BoxGroup("Android",CenterLabel = true),ListDrawerSettings(Expanded = true),SerializeField]private SkuItem[] androidSkuId;
        [BoxGroup("IOS",CenterLabel = true),ListDrawerSettings(Expanded = true),SerializeField]private SkuItem[] iosSkuId;
        public SkuItem[] GetSkuItems()
        {
#if UNITY_EDITOR||UNITY_ANDROID
            return androidSkuId;
#elif UNITY_IOS||UNITY_IPHONE
            return iosSkuId;
#endif
        }
        
    }
    
}
