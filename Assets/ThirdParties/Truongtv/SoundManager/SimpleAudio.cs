using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace ThirdParties.Truongtv.SoundManager
{
    public class SimpleAudio : BaseAudio
    {
        [SerializeField] public bool autoPlay,loop;
        private void Start()
        {
            AudioSource.loop = loop;
            if (autoPlay)
            {
                AudioSource.Play();
            }
        }
        public void Play()
        {
            AudioSource.Play();
        }

        public void Play(AudioClip clip, bool isLoop = false)
        {
            AudioSource.loop = isLoop;
            AudioSource.clip = clip;
            AudioSource.Play();
        }

        public void SetLoop(bool auto)
        {
            AudioSource.loop = auto;
        }
    }
}
