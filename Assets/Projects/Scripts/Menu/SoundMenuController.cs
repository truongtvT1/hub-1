using ThirdParties.Truongtv.SoundManager;
using UnityEngine;

namespace Projects.Scripts.Menu
{
    [RequireComponent(typeof(SoundManager))]
    public class SoundMenuController : MonoBehaviour
    {
        public static SoundMenuController Instance;
        private SoundManager _controller;
        private void Awake()
        {
            if(Instance!=null)
                Destroy(gameObject);
            Instance = this;
            _controller = GetComponent<SoundManager>();
        }
    }
}
