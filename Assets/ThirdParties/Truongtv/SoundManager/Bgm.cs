using System.Diagnostics;
using UnityEngine;

namespace ThirdParties.Truongtv.SoundManager
{
    public class Bgm : BaseAudio
    {
        private static Bgm instance;
        public static Bgm Instance => instance;
        protected override void Awake()
        {
            base.Awake();
            if(instance != null)
                Destroy(instance.gameObject);
            instance = this;
        }
        public bool IsPlaying()
        {
            return AudioSource.isPlaying;
        }

        public bool IsPlayingClip(AudioClip clip)
        {
            return AudioSource.clip == clip;
        }

        public void Play(AudioClip clip)
        {
            PlayLoop(clip);
        }
        public new void Stop()
        {
            if (AudioSource)
            {
                AudioSource.Stop();
                AudioSource.clip = null;
            }
        }
    }
}
