using System;
using ThirdParties.Truongtv;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Menu
{
    [RequireComponent(typeof(Toggle))]
    public class SkinColorItem : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private GameObject container, selected, locked;
        private Color _color;
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
        public void Init(Color set,ToggleGroup group,Action<SkinColorItem> onColorSelected)
        {
            _toggle.group = group;
                _color = set;
            image.color = _color;
            _toggle.interactable = true;
            _toggle.onValueChanged.AddListener(value=>{
                if (value)
                {
                    onColorSelected.Invoke(this);
                }
            });
            var on = GameDataManager.Instance.GetCurrentColor() == _color;
            selected.SetActive(on);
            _toggle.isOn = on;
            _toggle.onValueChanged.Invoke(on);
            locked.SetActive(false);
        }

        public void SetSelected()
        {
            var result = GameDataManager.Instance.GetCurrentColor() == _color;
            selected.SetActive(result);
            _toggle.onValueChanged.Invoke(result);
        }

        public Color GetColor()
        {
            return _color;
        }
    }
}