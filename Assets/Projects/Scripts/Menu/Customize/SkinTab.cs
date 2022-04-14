using System.Collections.Generic;
using Projects.Scripts.Hub;
using Projects.Scripts.Popup;
using Projects.Scripts.Scriptable;
using ThirdParties.Truongtv;
using Truongtv.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Menu.Customize
{
    [RequireComponent(typeof(ToggleGroup))]
    public class SkinTab : MonoBehaviour
    {
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private SkinItemGroup prefab;
        [SerializeField] private CharacterAnimationGraphic customCharacter;
        private ToggleGroup _group;
        private List<SkinInfo> _skins;
        private SkinItem _selected;
        private PopupCustomizeCharacter _controller;
        private bool _init;
        private void Awake()
        {
            _group = GetComponent<ToggleGroup>();
            //changeColorButton.onClick.AddListener(OnChangeColorButtonClick);
        }

        public void SetSkin(List<SkinInfo> skinList)
        {
            _skins = skinList;
        }
        public void Init(PopupCustomizeCharacter customizeCharacter)
        {
            
            if(_init) return;
            _controller = customizeCharacter;
            scroll.content.RemoveAllChild();
            var round = _skins.Count % 3 == 0 ? _skins.Count / 3 : _skins.Count / 3 + 1;
            for (var i = 0; i < round; i++)
            {
                var group = Instantiate(prefab, scroll.content);
                for (var j = 0; j <  3;j++)
                {
                    if (i * 3 + j < _skins.Count)
                    {
                        group.items[j].GetComponent<SkinItem>().Init(_skins[i * 3 + j],_group,OnSkinSelected);
                    }
                    else
                    {
                        group.items[j].GetComponent<SkinItem>().Hide();
                    }
                }
                scroll.onValueChanged.AddListener(group.UpdateLayoutPosition);
            }

            _init = true;
        }
        private void OnSkinSelected(SkinItem item)
        {
            _selected = item;
            // customCharacter.Set();
       
            var skinName = _selected.GetSkinName();
            if (skinName.Contains("hair/"))
            {
                _controller.skinList.RemoveAll(a => a.Contains("hair/"));
            }
            else if (skinName.Contains("gloves/"))
            {
                _controller.skinList.RemoveAll(a => a.Contains("gloves/"));
            }
            else if (skinName.Contains("body/"))
            {
                _controller.skinList.RemoveAll(a => a.Contains("body/"));
            }
            else if (skinName.Contains("glass/"))
            {
                _controller.skinList.RemoveAll(a => a.Contains("glass/"));
            }
            else if (skinName.Contains("wing/"))
            {
                _controller.skinList.RemoveAll(a => a.Contains("wing/"));
            }
            _controller.skinList.Add(skinName);
            customCharacter.UpdateSkin(_controller.skinList);
            customCharacter.SetSkinColor(_controller.color);
        }

        private void OnChangeColorButtonClick()
        {
            // check value here
            _selected.SetSelected();
            GameDataManager.Instance.UpdateCurrentSkin(_controller.skinList);
            customCharacter.UpdateSkin(_controller.skinList);
            
        }
    }
}
