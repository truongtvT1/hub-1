using System;
using System.Collections;
using System.Collections.Generic;
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
        
        private  static readonly System.Random rng = new System.Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
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

        public static IEnumerator CountTime(float duration, float delay, Action<float> onCounting = null, Action callback = null, Func<bool> waitUntil = null)
        {
            if (waitUntil != null)
            {
                yield return new WaitUntil(waitUntil);
            }
            yield return new WaitForSeconds(delay);
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                onCounting?.Invoke(time);
                yield return null;
            }
            callback?.Invoke();
        }
    }
    
}
