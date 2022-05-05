using System;
using System.Collections.Generic;
using Projects.Scripts.Menu.Customize;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Menu
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class SkinItemGroup : MonoBehaviour
    {
        public List<SkinItem> items;
        private Vector2 _startPos;
        private ScrollRect _scroll;
        private float _moveLength;
        private float move = 20f/240;
        private RectTransform rect;
        public void Init(Vector2 pos,ScrollRect scroll)
        {
            rect = GetComponent<RectTransform>();
            rect.localPosition = pos;
            _startPos = pos;
            _scroll = scroll;
            _moveLength = _scroll.content.sizeDelta.y- _scroll.GetComponent<RectTransform>().sizeDelta.y;
        }
        public void UpdateLayoutPosition(Vector2 normalize)
        {

            var distanceMove = (1 - normalize.y) * _moveLength;
            var pos = rect.localPosition;
            pos.x = _startPos.x + distanceMove * move;
            rect.localPosition = pos;
        }
    }
}