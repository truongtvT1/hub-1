using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Truongtv.Utilities;
using UnityEngine;

namespace Truongtv.PopUpController
{
    [RequireComponent(typeof(Canvas))]
    public class PopupController : MonoBehaviour
    {
        #region Properties

        //[SerializeField] private Image shadowBackground;
        [SerializeField] private Canvas lockSceneCanvas;
        private Stack<BasePopup> _stackPopup = new Stack<BasePopup>();
        private Canvas _canvasPopup;

        [SerializeField] private Toast toast;

        [SerializeField] private PopupNoInternet noInternet;
       // private Button _blackButton;
        #endregion
        private static PopupController _instance;
        public static PopupController Instance => _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }

            _instance = this;
        }
        #region Unity function

        private void Start()
        {
            _stackPopup = new Stack<BasePopup>();
            _canvasPopup = GetComponent<Canvas>();
            //_blackButton = shadowBackground.GetComponent<Button>();
            // _blackButton.onClick.AddListener(OutSideClick);
        }
        #if UNITY_ANDROID|| UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
                OutSideClick();
        }
        #endif
        #endregion

        #region Public function

        
        #endregion

        #region Private Function

        public int CalculatingSortingOrder(BasePopup basePopup)
        {
            var order = 0;
            if (_stackPopup.Count > 0)
            {
                order = (_stackPopup.Count) * 10;
            }
            if (_canvasPopup == null) _canvasPopup = GetComponent<Canvas>();
            _canvasPopup.sortingOrder = order;
            _stackPopup.Push(basePopup);
            return order + 10;
        }

        public void ReleaseStack()
        {
            var order = 0;
            if (_stackPopup.Count > 0)
            {
                _stackPopup.Pop();
            }

            if (_stackPopup.Count > 0)
            {
                var popup = _stackPopup.Peek();
                order = popup.GetSortingOrder() - 10;
            }

            _canvasPopup.sortingOrder = order;
            // if (order == 0)
            // {
            //     ShowShadowBackground(false);
            // }
        }

        private void OutSideClick()
        {
            if (_stackPopup.Count <= 0)
            {
                LockScene(false);
               // ShowShadowBackground(false);
                return;
            }
            var popup = _stackPopup.Peek();
            popup.Close();
            //custom here
        }
        public void LockScene(bool active)
        {
            if (lockSceneCanvas == null) return;
            lockSceneCanvas.enabled = active;
        }

        // private void ShowShadowBackground(bool active)
        // {
        //     if (shadowBackground == null) return;
        //     shadowBackground.enabled = active;
        // }

      

        #endregion

        [Button]
        public void ShowToast(string description)
        {
            toast.gameObject.SetActive(true);
            toast.Initialized(description);
            toast.Show(this);
        }

        public void ShowNoInternet()
        {
            noInternet.Init();
            noInternet.gameObject.SetActive(true);
            noInternet.Show(this);
        }
       
    }
}