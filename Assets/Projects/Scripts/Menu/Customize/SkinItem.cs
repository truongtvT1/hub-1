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
        public SkinInfo item;
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
        public void Init(SkinInfo set,ToggleGroup group,Action<SkinItem> onItemToggle)
        {
            item = set;
            skin.initialSkinName = item.skinName;
            switch (item.rank)
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
            
            _toggle.onValueChanged.AddListener(value=>
            {
                OnToggle(value, onItemToggle);
            });
            
            var on = GameDataManager.Instance.GetSkinInGame().Contains(item.skinName);
            _toggle.isOn = on;
            _toggle.onValueChanged.Invoke(on);
            
        }

        public void SetSelected()
        {
            var result = GameDataManager.Instance.GetSkinInGame().Contains(item.skinName);
            selected.SetActive(result);
            _toggle.onValueChanged.Invoke(result);
        }

        private void OnToggle(bool value,Action<SkinItem>onItemToggle)
        {
            if (value)
            {
                onItemToggle.Invoke(this);
                
            }
            
            var on = GameDataManager.Instance.GetSkinInGame().Contains(item.skinName);
            selected.SetActive(on);
            var unlock = GameDataManager.Instance.IsSkinUnlock(item.skinName);
            locked.SetActive(!unlock);
        }
    }
}
