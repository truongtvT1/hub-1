using System;
using Truongtv.PopUpController;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Popup
{
    public class PopupReceivePack : BasePopup
    {
        [SerializeField] private Image image;
        [SerializeField] private Button button;

        public void Init(Sprite sprite, Action moveToCustomizeCharacter = null)
        {
            image.sprite = sprite;
            image.SetNativeSize();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                Close();
                moveToCustomizeCharacter?.Invoke();
            });
        }
    }
}