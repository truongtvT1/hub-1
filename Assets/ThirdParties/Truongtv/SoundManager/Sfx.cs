using System;
using UnityEngine;

namespace ThirdParties.Truongtv.SoundManager
{
    public class Sfx : BaseAudio
    {
        public void Play(AudioClip clip, bool isLoop = false, float delay = 0f, Action complete = null)
        {
            if (isLoop)
            {
                PlayLoop(clip);
                return;
            }
            PlayOnceShot(clip,delay,complete);
        }
    }
}
