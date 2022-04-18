using System;
using System.Collections.Generic;
using Projects.Scripts.Hub;
using Projects.Scripts.Popup;
using ThirdParties.Truongtv;
using Truongtv.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Menu
{
    [RequireComponent(typeof(ToggleGroup))]
    public class SkinColorTab : MonoBehaviour
    {
        
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private SkinItemGroup prefab;
        [SerializeField] private CharacterAnimationGraphic customCharacter;
        [SerializeField] private Button changeColorButton;
        private ToggleGroup _group;
        private List<Color> _skinColors;
        private SkinColorItem _selected;
        private PopupCustomizeCharacter _controller;
        private bool _init;
        private void Awake()
        {
            _group = GetComponent<ToggleGroup>();
            changeColorButton.onClick.AddListener(OnChangeColorButtonClick);
        }

        public void Init(PopupCustomizeCharacter customizeCharacter)
        {
            if(_init) return;
            _controller = customizeCharacter;
            scroll.content.RemoveAllChild();
            _skinColors = GameDataManager.Instance.GetAllSkinColors();
            var round = _skinColors.Count % 3 == 0 ? _skinColors.Count / 3 : _skinColors.Count / 3 + 1;
            var rect = scroll.content.sizeDelta;
            rect.y = round * 240;
            scroll.content.sizeDelta = rect;
            for (var i = 0; i < round; i++)
            {
                var group = Instantiate(prefab, scroll.content);
                group.Init(new Vector2(20-i*20,-240*i),scroll);
                for (var j = 0; j <  3;j++)
                {
                    if (i * 3 + j < _skinColors.Count)
                    {
                        group.items[j].GetComponent<SkinColorItem>().Init(_skinColors[i * 3 + j],_group,OnColorSelected);
                    }
                    else
                    {
                        group.items[j].GetComponent<SkinColorItem>().Hide();
                    }
                }
                scroll.onValueChanged.AddListener(group.UpdateLayoutPosition);
            }
           
            _init = true;
        }

        private void OnColorSelected(SkinColorItem item)
        {
            _selected = item;
            _controller.color = _selected.GetColor();
            customCharacter.SetSkinColor(_selected.GetColor());
        }

        private void OnChangeColorButtonClick()
        {
            // check value here
            _selected.SetSelected();
            GameDataManager.Instance.SetSkinColor(_selected.GetColor());
            customCharacter.SetSkinColor(_selected.GetColor());
        }
    }
}