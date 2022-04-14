using Projects.Scripts.Popup;
using Truongtv.PopUpController;
using UnityEngine;

namespace Projects.Scripts.Menu
{
    [RequireComponent(typeof(PopupController))]
    public class PopupMenuController : MonoBehaviour
    {
        public static PopupMenuController Instance;
        
        [SerializeField] private PopupSetting popupSetting;
        [SerializeField] private PopupLeaderBoard popupLeaderBoard;
        [SerializeField] private PopupCustomizeCharacter popupCustomizeCharacter;
        private PopupController _controller;

        private void Awake()
        {
            if(Instance!=null)
                Destroy(gameObject);
            Instance = this;
            _controller = GetComponent<PopupController>();
        }

        public void ShowPopupSetting()
        {
            popupSetting.gameObject.SetActive(true);
            popupSetting.Init();
            popupSetting.Show(_controller);
        }

        public void ShowPopupLeaderBoard()
        {
            popupLeaderBoard.gameObject.SetActive(true);
            popupLeaderBoard.Init();
            popupLeaderBoard.Show(_controller);
        }
        public void ShowPopupCustomizeCharacter()
        {
            popupCustomizeCharacter.gameObject.SetActive(true);
            popupCustomizeCharacter.Init();
            popupCustomizeCharacter.Show(_controller);
        }
    }
}
