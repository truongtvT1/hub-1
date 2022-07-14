using ThirdParties.Truongtv.SoundManager;
using UnityEngine;

namespace Projects.Scripts.Menu
{
    [RequireComponent(typeof(SoundManager))]
    public class SoundMenuController : MonoBehaviour
    {
        [SerializeField] private AudioClip bgmMain;
        [SerializeField] private AudioClip openChest;
        [SerializeField] private AudioClip itemShow;
        [SerializeField] private AudioClip itemsShow;
        [SerializeField] private AudioClip customizeClick;
        [SerializeField] private AudioClip unlock;
        
        public static SoundMenuController Instance;
        private SoundManager _controller;
        private void Awake()
        {
            if(Instance!=null)
                Destroy(gameObject);
            Instance = this;
            _controller = GetComponent<SoundManager>();
        }

        public void PlayBgm()
        {
            _controller.PlayBgm(bgmMain);
        }

        public void PlayUnlock()
        {
            _controller.PlaySfx(unlock);
        }
        
        public void PlayCustomizeSelect()
        {
            _controller.PlaySfx(customizeClick);
        }
        
        public void PlayOneItemShow()
        {
            _controller.PlaySfx(itemShow);
        }
        
        public void PlayManyItemsShow()
        {
            _controller.PlaySfx(itemsShow);
        }
        
        public void PlayOpenChest()
        {
            _controller.PlaySfx(openChest);
        }
    }
}
