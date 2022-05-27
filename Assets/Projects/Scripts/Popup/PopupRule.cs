using System;
using Truongtv.PopUpController;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Popup
{
    public class PopupRule : BasePopup

    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Color S, A, B, C;
        [SerializeField] private Image rankSColor, rankAColor, rankBColor, rankCColor;
        private void Awake()
        {
            closeButton.onClick.AddListener(Close);
        }

        private void Start()
        {
            rankSColor.color = S;
            rankAColor.color = A;
            rankBColor.color = B;
            rankCColor.color = C;
        }
    }
}
