using System;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using ThirdParties.Truongtv.SoundManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Projects.Scripts.UIController
{
    [RequireComponent(typeof(Button))]
    public class ButtonEffect : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler,IPointerUpHandler
    {
       
        private Button _button;
        private bool _isEnter;
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_button.IsInteractable()) return;
            MMVibrationManager.Haptic (HapticTypes.Selection );
            SoundManager.Instance.PlayButtonSound();
            transform.DOKill();
            transform.DOScale(new Vector3(0.95f, 0.95f, 0.95f), 0.1f).SetEase(Ease.InQuad)
                .SetUpdate(UpdateType.Normal, true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_button.IsInteractable()) return;
            if(_isEnter) return;
            _isEnter = true;
            transform.DOKill();
            transform.DOScale(new Vector3(0.95f, 0.95f, 0.95f), 0.1f).SetEase(Ease.InQuad)
                .SetUpdate(UpdateType.Normal, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isEnter = false;
            transform.DOKill();
            transform.DOScale(new Vector3(1, 1, 1), 0.1f).SetEase(Ease.InQuad)
                .SetUpdate(UpdateType.Normal, true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.DOKill();
            transform.DOScale(new Vector3(1, 1, 1), 0.1f).SetEase(Ease.InQuad)
                .SetUpdate(UpdateType.Normal, true);
        }

        private void OnValidate()
        {
            GetComponent<Button>().transition = Selectable.Transition.None;
        }
    }
}