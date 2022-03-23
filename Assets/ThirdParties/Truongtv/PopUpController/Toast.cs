using System;
using System.Collections;
using ThirdParties.Truongtv.SoundManager;
using TMPro;
using UnityEngine;

namespace Truongtv.PopUpController
{
    public class Toast : BasePopup
    {
        [SerializeField] private TextMeshProUGUI descriptionTxt;
        [SerializeField] private float stayDuration;
        private void Awake()
        {
            RegisterEvent();
        }

        public void Initialized(string description)
        {
            descriptionTxt.text = description;
        }

        private void RegisterEvent()
        {
            openAction = OnStart;
            closeAction = OnClose;
            openCompleteAction = async () =>
            {
                StopAllCoroutines();
                StartCoroutine(OnOpenComplete());
            };
        }
        private void OnStart()
        {
            SoundManager.Instance.PlayPopupOpenSound();
        }

        private IEnumerator OnOpenComplete()
        {
            yield return new WaitForSeconds(stayDuration);
            Close();
        }
        private void OnClose()
        {
            SoundManager.Instance.PlayPopupCloseSound();
        }
    }
}