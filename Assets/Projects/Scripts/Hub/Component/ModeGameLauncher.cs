using DG.Tweening;
using Projects.Scripts.Data;
using Projects.Scripts.Menu;
using ThirdParties.Truongtv;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Projects.Scripts.Hub.Component
{
    public class ModeGameLauncher : MonoBehaviour
    {
        [SerializeField] private Image gameBg,titleBg;
        [SerializeField] private TextMeshProUGUI  winText, loseText,nameText, unlockText;
        [SerializeField] private GameObject mostPlayObj,lastPlayObj,onObj,offObj,lockedObj;
        [SerializeField] private Button button;
        private MiniGameInfo _info;
        private bool canUnlock;
        public void Init(Color color, MiniGameInfo info=null)
        {
            _info = info;
            if (_info == null)
            {
                onObj.SetActive(false);
                offObj.SetActive(true);
                button.interactable = false;
                return;
            }

            if (!GameDataManager.Instance.IsModeUnlock(_info))
            {
                lockedObj.SetActive(true);
                unlockText.text = $"UNLOCK WITH {_info.ticketToUnlock} TICKETS";
                canUnlock = GameDataManager.Instance.GetTotalTicket() >= _info.ticketToUnlock;
            }
            titleBg.color = color;
            onObj.SetActive(true);
            offObj.SetActive(false);
            gameBg.sprite = _info.bg;
            gameBg.SetNativeSize();
            lastPlayObj.SetActive(_info.recentPlay);
            mostPlayObj.SetActive(_info.mostPlay);
            winText.text = $"{_info.win}";
            loseText.text = $"{_info.lose}";
            nameText.text = _info.name;
            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnPlayButtonClick);

        }
        private void OnPlayButtonClick()
        {
            if (lockedObj.activeSelf)
            {
                if (canUnlock)
                {
                    GameDataManager.Instance.UnlockMode(_info);
                    MenuController.Instance.UseTicket(_info.ticketToUnlock);
                    lockedObj.SetActive(false);
                }
                return;
            }
            GameDataManager.Instance.UpdateLastPlayed(_info.gameId);
            GameDataManager.Instance.UpdateMiniGameCountPlayed(_info.gameId);
            Loading.Instance.LoadMiniGame(_info);
        }
    }
}