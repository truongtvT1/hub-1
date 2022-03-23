using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Truongtv.Utilities.PlayerPrefsEditor
{
    [Serializable]
    public class PreferenceEntry
    {
        public enum PrefTypes
        {
            String = 0,
            Int = 1,
            Float = 2
        }
        public string m_key;
        public PrefTypes m_typeSelection = PrefTypes.Int;

        public string m_strValue;
        public int m_intValue;
        public float m_floatValue;
    }
    
}