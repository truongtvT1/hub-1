using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Menu
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class SkinItemGroup : MonoBehaviour
    {
        public List<GameObject> items;
        private HorizontalLayoutGroup _layout;

        private void Awake()
        {
            _layout = GetComponent<HorizontalLayoutGroup>();
        }

        public void UpdateLayoutPosition(Vector2 normalize)
        {
            //_layout.padding.left = 
        }
    }
}