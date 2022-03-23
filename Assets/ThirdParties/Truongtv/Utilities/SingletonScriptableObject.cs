using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Truongtv.Utilities
{
    public class SingletonScriptableObject<T> : SerializedScriptableObject where T: SerializedScriptableObject 
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                }
                return _instance;
            }
        }
    }
}