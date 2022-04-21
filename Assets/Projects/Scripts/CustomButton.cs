using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool interactive = true;
    [SerializeField] private Transform targetGraphic;
    public UnityEvent onClick, onEnter, onExit;
    private bool _isEnter;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactive) return;
        targetGraphic.DOKill();
        targetGraphic.DOScale(new Vector3(0.95f, 0.95f, 0.95f), 0.1f).SetEase(Ease.InOutSine)
            .SetUpdate(UpdateType.Normal, true);

        onClick.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!interactive) return;
        onEnter.Invoke();
        if (_isEnter) return;
        _isEnter = true;
        targetGraphic.DOKill();
        targetGraphic.DOScale(new Vector3(0.95f, 0.95f, 0.95f), 0.1f).SetEase(Ease.InOutSine)
            .SetUpdate(UpdateType.Normal, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!interactive) return;
        onExit.Invoke();
        _isEnter = false;
        targetGraphic.DOKill();
        targetGraphic.DOScale(new Vector3(1, 1, 1), 0.1f).SetEase(Ease.InOutSine)
            .SetUpdate(UpdateType.Normal, true);
    }
}