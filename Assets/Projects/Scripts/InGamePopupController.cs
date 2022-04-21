using System;
using Projects.Scripts.Popup;
using Truongtv.PopUpController;
using UnityEngine;

namespace MiniGame
{
    [RequireComponent(typeof(PopupController))]
    public class InGamePopupController : MonoBehaviour
    {
        public static InGamePopupController Instance;
        
        [SerializeField] private PopupSetting popupSetting;
        private PopupController _controller;
        
        private void Awake()
        {
            if(Instance!=null)
                Destroy(gameObject);
            Instance = this;
            _controller = GetComponent<PopupController>();
        }
        
        
        public void ShowPopupSetting(Action replay, Action home, Action close)
        {
            popupSetting.gameObject.SetActive(true);
            popupSetting.Init(replay,home,close);
            popupSetting.Show(_controller);
        }
        
        
        
    }
}