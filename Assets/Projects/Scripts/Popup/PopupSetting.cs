using System;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv;
using ThirdParties.Truongtv.SoundManager;
using Truongtv.PopUpController;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Projects.Scripts.Popup
{
    public class PopupSetting : BasePopup
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Toggle bgmToggle, sfxToggle, vibrateToggle;
        [SerializeField] private Button homeButton, replayButton;
        public bool isInGame = false;
        private Action replayCallback, homeCallback;
        private void Awake()
        {
            if (isInGame)
            {
                if(homeButton!=null)
                    homeButton.onClick.AddListener(OnHomeButtonClick);
                if(replayButton!=null)
                    replayButton.onClick.AddListener(OnReplayButtonClick);
            }
            closeButton.onClick.AddListener(Close);
            bgmToggle.onValueChanged.AddListener(OnBgmToggleChange);
            sfxToggle.onValueChanged.AddListener(OnSfxToggleChange);
            vibrateToggle.onValueChanged.AddListener(OnVibrateToggleChange);
        }

        public void Init(Action replay = null, Action home = null, Action close = null)
        {
            closeAction = close;
            homeCallback = home;
            replayCallback = replay;
            sfxToggle.isOn = SoundManager.IsSfx();
            bgmToggle.isOn = SoundManager.IsBgm();
            vibrateToggle.isOn = PlayerPrefs.GetInt("vibration") == 0;
            if (isInGame)
            {
                homeButton.gameObject.SetActive(true);
                replayButton.gameObject.SetActive(true);
            }
            else
            {
                if(homeButton!=null)
                    homeButton.gameObject.SetActive(false);
                if(homeButton!=null)
                    replayButton.gameObject.SetActive(false);
            }
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

        private void OnHomeButtonClick()
        {
            GameServiceManager.ShowInterstitialAd(() =>
            {
                homeCallback?.Invoke();
                SceneManager.LoadScene("Menu");
            });
        }
        
        private void OnReplayButtonClick()
        {
            replayCallback?.Invoke();
            closeAction = null;
            Close();
        }
    }
}
