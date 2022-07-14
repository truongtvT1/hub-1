using Sirenix.OdinInspector;
using ThirdParties.Truongtv.SoundManager;
using UnityEngine;

namespace MiniGame
{    
    [RequireComponent(typeof(SoundManager))]
    public class SoundInGameController : MonoBehaviour
    {
        [SerializeField] private AudioClip bgmStealBall;
        [SerializeField] private AudioClip bgmMemory;
        [SerializeField] private AudioClip bgmStickRun;
        [SerializeField] private AudioClip rewardBonus;
        [SerializeField] private AudioClip win;
        public static SoundInGameController Instance;
        private SoundManager _controller;
        private void Awake()
        {
            if(Instance!=null)
                Destroy(gameObject);
            Instance = this;
            _controller = GetComponent<SoundManager>();
        }

        public void PlayMemoryBGM()
        {
            _controller.PlayBgm(bgmMemory);
        }
        
        public void PlayStealBGM()
        {
            _controller.PlayBgm(bgmStealBall);
        }
        
        public void PlayStickBGM()
        {
            _controller.PlayBgm(bgmStickRun);
        }

        public void PlayWin()
        {
            _controller.PlaySfx(win);
        }
        
        public void PlayRewardBonus(bool loop = true)
        {
            _controller.PlaySfx(rewardBonus, loop);
        }

        public void StopRewardBonusSfx()
        {
            _controller.StopLoopSfx(rewardBonus);
        }

    }
}