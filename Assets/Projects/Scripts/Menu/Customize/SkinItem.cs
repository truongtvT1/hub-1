using System;
using Projects.Scripts.Scriptable;
using Spine.Unity;
using ThirdParties.Truongtv;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Menu.Customize
{
    [RequireComponent(typeof(Toggle))]
    public class SkinItem : MonoBehaviour
    {
        [SerializeField] private SkeletonGraphic skin;
        [SerializeField] private GameObject container, selected, locked;
        [SerializeField] private Image bg;
        [SerializeField] private Color S, A, B, C;
        private SkinInfo _item;
        private Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
        }

        public void Hide()
        {
            container.SetActive(false);
            _toggle.interactable = false;
        }
        public void Init(SkinInfo set,ToggleGroup group,Action<SkinItem> onColorSelected)
        {
            _item = set;
            skin.initialSkinName = _item.skinName;
            switch (_item.rank)
            {
                case SkinRank.S:
                    bg.color = S;
                    break;
                case SkinRank.A:
                    bg.color = A;
                    break;
                case SkinRank.B:
                    bg.color = B;
                    break;
                case SkinRank.C:
                    bg.color = C;
                    break;
                
            }

            ;
            skin.Initialize(true);
            _toggle.group = group;
            _toggle.interactable = true;
            _toggle.onValueChanged.AddListener(value=>{
                if (value)
                {
                    onColorSelected.Invoke(this);
                }
            });
            var on = GameDataManager.Instance.GetCurrentSkin().Contains(_item.skinName);
            selected.SetActive(on);
            _toggle.isOn = on;
            _toggle.onValueChanged.Invoke(on);
            locked.SetActive(false);
            locked.SetActive(false);
        }

        public void SetSelected()
        {
            selected.SetActive(GameDataManager.Instance.GetCurrentSkin().Contains(_item.skinName));
        }

        public string GetSkinName()
        {
            return _item.skinName;
        }
    }
}
