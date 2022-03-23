using DG.Tweening;
using ThirdParties.Truongtv.SoundManager;
using Truongtv.PopUpController;
using UnityEngine;
using UnityEngine.UI;

public class PopupNoInternet : BasePopup
{
    [SerializeField] private Transform container;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(ClosePopup);
    }

    public void Init()
    {
        openAction = OnStart;
        closeAction = OnClose;
    }
    private void OnStart()
    {
        container.localScale = Vector3.zero;
        SoundManager.Instance.PlayPopupOpenSound();
        container.DOScale(1, DURATION).SetUpdate(UpdateType.Normal, true).SetEase(Ease.OutBack);
    }

    private void OnClose()
    {
        SoundManager.Instance.PlayPopupCloseSound();
        container.DOScale(0, DURATION).SetUpdate(UpdateType.Normal, true).SetEase(Ease.InBack);
    }
    private void ClosePopup()
    {
        SoundManager.Instance.PlayButtonSound();
        Close();
    }
}
