using System.Collections.Generic;
using Projects.Scripts.Hub;
using Projects.Scripts.Menu;
using Projects.Scripts.Menu.Customize;
using ThirdParties.Truongtv;
using Truongtv.PopUpController;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Popup
{
    public class PopupCustomizeCharacter : BasePopup
    {
        [SerializeField] private Toggle skinToggle, hairToggle, suitToggle, gloveToggle, glassToggle, cloakToggle;
        [SerializeField] private SkinColorTab skinTab;
        [SerializeField] private SkinTab hair, suit, glove, glass, cloak;
        [SerializeField] private CharacterAnimationGraphic character;
        [SerializeField] private Button closeButton;
        [HideInInspector]public List<string> skinList;
        [HideInInspector]public Color color;
        private void Awake()
        {
            closeButton.onClick.AddListener(OnCloseButtonClick);
            skinToggle.onValueChanged.AddListener(OnSkinToggleChange);
            hairToggle.onValueChanged.AddListener(OnHairToggleChange);
            suitToggle.onValueChanged.AddListener(OnBodyToggleChange);
            gloveToggle.onValueChanged.AddListener(OnGloveToggleChange);
            glassToggle.onValueChanged.AddListener(OnGlassToggleChange);
            cloakToggle.onValueChanged.AddListener(OnCloakToggleChange);
        }

        public void Init()
        {
            skinList = GameDataManager.Instance.GetCurrentSkin();
            color = GameDataManager.Instance.GetSkinColor();
            character.UpdateSkin(skinList);
            character.SetSkinColor(color);
            skinToggle.isOn = true;
            skinToggle.onValueChanged.Invoke(true);
        }

        #region ButtonEvent

        private void OnSkinToggleChange(bool value)
        {
            skinTab.gameObject.SetActive(value);
            hair.gameObject.SetActive(!value);
            suit.gameObject.SetActive(!value);
            glove.gameObject.SetActive(!value);
            glass.gameObject.SetActive(!value);
            cloak.gameObject.SetActive(!value);
            if(value)
                skinTab.Init(this);
        }
        private void OnHairToggleChange(bool value)
        {
            skinTab.gameObject.SetActive(!value);
            hair.gameObject.SetActive(value);
            suit.gameObject.SetActive(!value);
            glove.gameObject.SetActive(!value);
            glass.gameObject.SetActive(!value);
            cloak.gameObject.SetActive(!value);
            hair.SetSkin(GameDataManager.Instance.skinData.hairSkins);
            if(value)
                hair.Init(this);
        }
        private void OnBodyToggleChange(bool value)
        {
            skinTab.gameObject.SetActive(!value);
            hair.gameObject.SetActive(!value);
            suit.gameObject.SetActive(value);
            glove.gameObject.SetActive(!value);
            glass.gameObject.SetActive(!value);
            cloak.gameObject.SetActive(!value);
            suit.SetSkin(GameDataManager.Instance.skinData.bodySkins);
            if(value)
                suit.Init(this);
        }
        private void OnGloveToggleChange(bool value)
        {
            skinTab.gameObject.SetActive(!value);
            hair.gameObject.SetActive(!value);
            suit.gameObject.SetActive(!value);
            glove.gameObject.SetActive(value);
            glass.gameObject.SetActive(!value);
            cloak.gameObject.SetActive(!value);
            glove.SetSkin(GameDataManager.Instance.skinData.gloveSkins);
            if(value)
                glove.Init(this);
        }
        private void OnGlassToggleChange(bool value)
        {
            skinTab.gameObject.SetActive(!value);
            hair.gameObject.SetActive(!value);
            suit.gameObject.SetActive(!value);
            glove.gameObject.SetActive(!value);
            glass.gameObject.SetActive(value);
            cloak.gameObject.SetActive(!value);
            glass.SetSkin(GameDataManager.Instance.skinData.glassSkins);
            if(value)
                glass.Init(this);
        }
        private void OnCloakToggleChange(bool value)
        {
            skinTab.gameObject.SetActive(!value);
            hair.gameObject.SetActive(!value);
            suit.gameObject.SetActive(!value);
            glove.gameObject.SetActive(!value);
            glass.gameObject.SetActive(!value);
            cloak.gameObject.SetActive(value);
            cloak.SetSkin(GameDataManager.Instance.skinData.cloakSkins);
            if(value)
                cloak.Init(this);
        }

        private void OnCloseButtonClick()
        {
            MenuController.Instance.UpdateCharacter();
            Close();
        }
        #endregion
    }
}
