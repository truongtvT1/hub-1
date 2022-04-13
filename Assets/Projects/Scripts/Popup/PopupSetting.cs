using System;
using ThirdParties.Truongtv.SoundManager;
using Truongtv.PopUpController;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Popup
{
    public class PopupSetting : BasePopup
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Toggle bgmToggle, sfxToggle, vibrateToggle;

        private void Awake()
        {
            closeButton.onClick.AddListener(Close);
            bgmToggle.onValueChanged.AddListener(OnBgmToggleChange);
            sfxToggle.onValueChanged.AddListener(OnSfxToggleChange);
            vibrateToggle.onValueChanged.AddListener(OnVibrateToggleChange);
        }

        public void Init()
        {
            sfxToggle.isOn = SoundManager.IsSfx();
            bgmToggle.isOn = SoundManager.IsBgm();
            vibrateToggle.isOn = PlayerPrefs.GetInt("vibration") == 0;
            
        }
        private void OnSfxToggleChange(bool value)
        {
            SoundManager.Instance.PlayButtonSound();
            SoundManager.Instance.SetSfx(value);
        }
        private void OnBgmToggleChange(bool value)
        {
            SoundManager.Instance.PlayButtonSound();
            SoundManager.Instance.SetBgm(value);
        }

        private void OnVibrateToggleChange(bool value)
        {
            SoundManager.Instance.PlayButtonSound();
            PlayerPrefs.SetInt("vibration", value?0:-1);
            PlayerPrefs.Save();
        }
    }
}
