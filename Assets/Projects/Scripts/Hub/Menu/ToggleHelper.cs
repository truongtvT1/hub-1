using System;
using DG.Tweening;
using ThirdParties.Truongtv.SoundManager;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Menu
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleHelper : MonoBehaviour
    {
        private Toggle _toggle;
        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(value =>
            {
                SoundManager.Instance.PlayButtonSound();
                if (value)
                {
                    _toggle.graphic.transform.DOLocalMoveX(35, 0f);
                }
                else
                {
                    _toggle.graphic.transform.DOLocalMoveX(0, 0f);
                }
            });
        }
    }
}
