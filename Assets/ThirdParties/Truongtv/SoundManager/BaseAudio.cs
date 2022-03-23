using System;
using System.Collections;
using UnityEngine;

namespace ThirdParties.Truongtv.SoundManager
{
    
    [RequireComponent(typeof(AudioSource))]
    public class BaseAudio : MonoBehaviour
    {
        protected AudioSource AudioSource;

        protected virtual void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
        }

        protected void PlayOnceShot(AudioClip clip, float delay = 0f, Action complete = null)
        {
            AudioSource.clip = clip;
            AudioSource.PlayDelayed(delay);
            StartCoroutine(WaitToFinish(complete));
        }

        protected void PlayLoop(AudioClip clip)
        {
            AudioSource.clip = clip;
            AudioSource.loop = true;
            AudioSource.Play();
            
        }

        private IEnumerator WaitToFinish(Action complete = null)
        {
            while (AudioSource.isPlaying)
            {
                yield return null;
            }
            complete?.Invoke();
            Stop();
        }
        public void Pause()
        {
            AudioSource.Pause();
        }

        public void Resume()
        {
            AudioSource.UnPause();
        }

        public void Stop()
        {
            AudioSource.Stop();
            AudioSource.clip = null;
            AudioSource.gameObject.SetActive(false);
        }
        private void OnValidate()
        {
            GetComponent<AudioSource>().playOnAwake = false;
        }

    }
}
