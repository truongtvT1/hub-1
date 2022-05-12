using Projects.Scripts.Data;
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
        [SerializeField] private TextMeshProUGUI  winText, loseText,nameText;
        [SerializeField] private GameObject mostPlayObj,lastPlayObj,onObj,offObj;
        [SerializeField] private Button button;
        private MiniGameInfo _info;
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
            GameDataManager.Instance.UpdateLastPlayed(_info.gameId);
            GameDataManager.Instance.UpdateMiniGameCountPlayed(_info.gameId);
            SceneManager.LoadScene(_info.GetScene);
        }
    }
}