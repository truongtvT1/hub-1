using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace ThirdParties.Truongtv.SoundManager
{
    public class SoundManager : MonoBehaviour
    {
        private const string INSERT_KEY = "_number_";
        private const string SOUND_SFX = "sfx";
        private const string SOUND_BGM = "bgm";
        [SerializeField] private Sfx sfxPrefab;
        [SerializeField] private List<Sfx> sfxList;
        [SerializeField] private AudioClip buttonSound, popupOpenSound, popupCloseSound;
        [SerializeField] private AudioMixer mixer;
        private static SoundManager _instance;
        public static SoundManager Instance => _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }

            _instance = this;
        }

        private void Start()
        {
            SetBgm(IsBgm());
            SetSfx(IsSfx());
        }

        public static bool IsBgm()
        {
            return PlayerPrefs.GetInt(SOUND_BGM) == 0;
        }

        public static bool IsSfx()
        {
            return PlayerPrefs.GetInt(SOUND_SFX) == 0;
        }
        public void SetSfx(bool isOn)
        {
            SetAudio("Sfx", isOn ? 1:0);
            PlayerPrefs.SetInt(SOUND_SFX, isOn?0:-1);
            PlayerPrefs.Save();
        }
        public void SetBgm(bool isOn)
        {
            SetAudio("Bgm", isOn ?  1:0);
            PlayerPrefs.SetInt(SOUND_BGM, isOn?0:-1);
            PlayerPrefs.Save();
        }
        public void PlaySfx(AudioClip clip, bool isLoop = false,float delay = 0f,Action onComplete = null)
        {
            var simple = GetSfxInstance();
            simple.Play(clip, isLoop,delay,onComplete);
        }

        public void PlayBgm(AudioClip clip)
        {
            if(!Bgm.Instance.IsPlaying())
                Bgm.Instance.Play(clip);
            else
            {
                if(Bgm.Instance.IsPlayingClip(clip))
                    Bgm.Instance.Resume();
                else
                    Bgm.Instance.Play(clip);
            }
        }
        public void Pause(bool isPause)
        {
            if (isPause)
            {
                Bgm.Instance.Pause();
                for (var i = 0; i < sfxList.Count; i++)
                {
                    sfxList[i].Pause();
                }
            }

            else
            {
                Bgm.Instance.Resume();
                for (var i = 0; i < sfxList.Count; i++)
                {
                    sfxList[i].Resume();
                }
            }
        }
        private Sfx GetSfxInstance()
        {
            if (sfxPrefab == null)
                return null;
            for (var i = 0; i < sfxList.Count; i++)
            {
                if (sfxList[i].gameObject.activeSelf) continue;
                sfxList[i].gameObject.SetActive(true);
                return sfxList[i];
            }
            var count = sfxList.Count;
            var go = Instantiate(sfxPrefab, transform);
            go.transform.SetParent(transform);
            go.gameObject.name = SOUND_SFX + INSERT_KEY + count;
            sfxList.Add(go);
            return go;
        }
        private void SetAudio(string nameVolum,float volum)
        {
            var value = Mathf.Log10(Mathf.Clamp(volum, 0.0001f, 1))*20;
            mixer.SetFloat(nameVolum, value );
        }

        public void PlayPopupOpenSound()
        {
            if (popupOpenSound)
            {
                PlaySfx(popupOpenSound);
            }
        }
        public void PlayPopupCloseSound()
        {
            if (popupCloseSound)
            {
                PlaySfx(popupCloseSound);
            }
        }
        
        public void PlayButtonSound(Action complete = null)
        {
            if (buttonSound)
            {
                PlaySfx(buttonSound,onComplete:complete);
            }
        }
    }
}
