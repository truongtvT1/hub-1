using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Truongtv.Utilities
{
    public static class Extended 
    {
        public static void RemoveAllChild(this Transform root, Func<GameObject, bool> condition = null)
        {
            if (root == null||root.childCount == 0)
            {
                return;
            }
            for (var i = root.childCount - 1; i >= 0; i--)
            {
                var t = root.GetChild(i);
                if (condition == null || condition(t.gameObject))
                {
                    if(Application.isEditor)
                        UnityEngine.Object.DestroyImmediate(t.gameObject);
                    else
                    {
                        UnityEngine.Object.Destroy(t.gameObject);
                    }
                }
            }
        }
        public static bool IsInLayerMask(GameObject obj, LayerMask layerMask)
        {
            return ((layerMask.value & (1 << obj.layer)) > 0);
        }
        
        
        public static int RollADice(int numberOfSides)
        {
            Random.InitState(DateTime.UtcNow.Millisecond);
            return Random.Range(1,numberOfSides+1);
        }

        public static float RandomFloat(float min, float max)
        {
            Random.InitState(DateTime.UtcNow.Millisecond);
            return Random.Range(min, max);
        }
    }
    
}
