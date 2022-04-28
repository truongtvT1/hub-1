using System;
using System.Collections;
using DG.Tweening;
using ThirdParties.Truongtv.SoundManager;
using TMPro;
using UnityEngine;

namespace Truongtv.PopUpController
{
    public class Toast : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI descriptionTxt;
        [SerializeField] private float stayDuration;
        private RectTransform _rect;
        private Sequence _sequence;

        public void Initialized(string description)
        {
            descriptionTxt.text = description;
            _rect = GetComponent<RectTransform>();
            SoundManager.Instance.PlayPopupOpenSound();
            if(_sequence.IsActive())
                _sequence.Kill(true);
            _sequence = DOTween.Sequence();
            _sequence.Append(_rect.DOLocalMoveY(0, 0.35f).SetEase(Ease.OutQuad));
            _sequence.Append(_rect.DOLocalMoveY(400, 0.35f).SetEase(Ease.InQuad).SetDelay(stayDuration).OnStart(() =>
            {
                SoundManager.Instance.PlayPopupCloseSound();
            }));
            _sequence.Play();
        }
    }
}