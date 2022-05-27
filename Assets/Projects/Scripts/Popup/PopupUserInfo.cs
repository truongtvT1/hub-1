using System;
using Projects.Scripts.Data;
using Projects.Scripts.Hub;
using Projects.Scripts.Menu;
using ThirdParties.Truongtv;
using TMPro;
using Truongtv.PopUpController;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Popup
{
    public class PopupUserInfo : BasePopup
    {
        [SerializeField] private Button closeButton, editNameButton;
        [SerializeField] private TextMeshProUGUI textWinRate, textBattleMatch, textHighestTrophies, textBattleVictory, textUserID;
        [SerializeField] private TMP_InputField textInput;
        [SerializeField] private CharacterAnimationGraphic avatar;
        private UserRanking userRanking;

        private void Awake()
        {
            closeButton.onClick.AddListener(Close);
            editNameButton.onClick.AddListener(() =>
            {
                textInput.readOnly = false;
                textInput.Select();
            });
            textInput.onDeselect.AddListener(value =>
            {
                textInput.readOnly = true;
                GameDataManager.Instance.SetUserName(value);
                MenuController.Instance.UpdatePlayerName();
            });
            textInput.onSubmit.AddListener(value =>
            {
                textInput.readOnly = true;
                GameDataManager.Instance.SetUserName(value);
                MenuController.Instance.UpdatePlayerName();
            });
            textInput.onEndEdit.AddListener(value =>
            {
                textInput.readOnly = true;
                GameDataManager.Instance.SetUserName(value);
                MenuController.Instance.UpdatePlayerName();
            });
        }

        public void Init()
        {
            userRanking = GameDataManager.Instance.GetUserRanking();
            avatar.SetSkin(GameDataManager.Instance.GetCurrentSkin());
            avatar.SetSkinColor(GameDataManager.Instance.GetCurrentColor());
            if (userRanking.win + userRanking.lose == 0)
            {
                textWinRate.text = "0";
            }
            else
            {
                textWinRate.text = ((float) userRanking.win * 100 / (userRanking.win + userRanking.lose)).ToString(@"F") + "%";
            }
            textBattleMatch.text = userRanking.win + userRanking.lose + "";
            textBattleVictory.text = userRanking.win + "";
            textHighestTrophies.text = GameDataManager.Instance.GetTotalTrophy() + "";
            textUserID.text = "ID: " + userRanking.id;
            textInput.text = userRanking.userName;
        }
    }
}