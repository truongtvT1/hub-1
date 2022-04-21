using System.Collections.Generic;
using Projects.Scripts.Hub;
using Projects.Scripts.Popup;
using Projects.Scripts.Scriptable;
using ThirdParties.Truongtv;
using TMPro;
using Truongtv.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Menu.Customize
{
    [RequireComponent(typeof(ToggleGroup))]
    public class SkinTab : MonoBehaviour
    {
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private SkinItemGroup prefab;
        [SerializeField] private CharacterAnimationGraphic customCharacter;
        [SerializeField] private Button tryButton, buyByTicketButton, buyByAdButton, selectButton;
        [SerializeField] private GameObject selected;
        [SerializeField] private TextMeshProUGUI priceTicketText;
        private ToggleGroup _group;
        private List<SkinInfo> _skins;
        private List<SkinItem> _itemList;
        private SkinItem _selected;
        private PopupCustomizeCharacter _controller;
        private bool _init;
        private void Awake()
        {
            _group = GetComponent<ToggleGroup>();
            tryButton.onClick.AddListener(OnTryButtonClick);
            buyByTicketButton.onClick.AddListener(OnBuyButtonClick);
            buyByAdButton.onClick.AddListener(OnBuyByAdButtonClick);
            selectButton.onClick.AddListener(OnSelectButtonClick);
        }

        public void SetSkin(List<SkinInfo> skinList)
        {
            _skins = skinList;
        }
        public void Init(PopupCustomizeCharacter customizeCharacter)
        {
            if(_init) return;
            _controller = customizeCharacter;
            scroll.content.RemoveAllChild();
            _itemList = new List<SkinItem>();
            var round = _skins.Count % 3 == 0 ? _skins.Count / 3 : _skins.Count / 3 + 1;
            var rect = scroll.content.sizeDelta;
            rect.y = round * 240;
            scroll.content.sizeDelta = rect;
            for (var i = 0; i < round; i++)
            {
                var group = Instantiate(prefab, scroll.content);
                group.Init(new Vector2(20-i*20,-240*i),scroll);
                for (var j = 0; j <  3;j++)
                {
                    if (i * 3 + j < _skins.Count)
                    {
                        group.items[j].GetComponent<SkinItem>().Init(_skins[i * 3 + j],_group,OnSkinSelected);
                        _itemList.Add(group.items[j].GetComponent<SkinItem>());
                    }
                    else
                    {
                        group.items[j].GetComponent<SkinItem>().Hide();
                    }
                }
                scroll.onValueChanged.AddListener(group.UpdateLayoutPosition);
            }
            _init = true;
            
        }
        private void OnSkinSelected(SkinItem item)
        {
            _selected = item;
            var skinName = item.item.skinName;
            _controller.skinList = GameDataManager.Instance.UpdateSkinForList(_controller.skinList, skinName);
            _controller.UpdateCharacter();
            if (GameDataManager.Instance.IsSkinUnlock(_selected.item.skinName)||GameDataManager.Instance.GetSkinInGame().Contains(_selected.item.skinName))
            {
                if (GameDataManager.Instance.GetSkinInGame().Contains(_selected.item.skinName))
                {
                    selected.SetActive(true);
                    selectButton.gameObject.SetActive(false);
                }
                else
                {
                    selected.SetActive(false);
                    selectButton.gameObject.SetActive(true);
                }
                tryButton.gameObject.SetActive(false);
                buyByAdButton.gameObject.SetActive(false);
                buyByTicketButton.gameObject.SetActive(false);
            }
            else
            {
                var price = GameDataManager.Instance.GetSkinPriceByRank(_selected.item.rank);
                priceTicketText.text = $"{price}";
                selected.SetActive(false);
                selectButton.gameObject.SetActive(false);
                if (_selected.item.rank == SkinRank.C || _selected.item.rank == SkinRank.B)
                {
                    tryButton.gameObject.SetActive(false);
                    buyByAdButton.gameObject.SetActive(true);
                    buyByTicketButton.gameObject.SetActive(true);
                }
                else if (_selected.item.rank == SkinRank.A || _selected.item.rank == SkinRank.S)
                {
                    tryButton.gameObject.SetActive(true);
                    buyByAdButton.gameObject.SetActive(false);
                    buyByTicketButton.gameObject.SetActive(true);
                }
            }
        }

        private void OnTryButtonClick()
        {
            GameServiceManager.Instance.ShowRewardedAd("customize_try_skin", () =>
            {
                GameDataManager.Instance.TrySkin(_selected.item.skinName);
                OnSelectButtonClick();
            });
        }

        private void OnBuyButtonClick()
        {
            var price = GameDataManager.Instance.GetSkinPriceByRank(_selected.item.rank);
            if (GameDataManager.Instance.GetTotalTicket() >=price)
            {
                MenuController.Instance.UseTicket(price);
                GameDataManager.Instance.UnlockSkin(_selected.item.skinName);
                GameDataManager.Instance.UpdateCurrentSkin(_selected.item.skinName);
                OnSelectButtonClick();
            }
            else
            {
                // show not enough ticket
            }
        }

        private void OnBuyByAdButtonClick()
        {
            GameServiceManager.Instance.ShowRewardedAd("customize_unlock_skin", () =>
            {
                GameDataManager.Instance.UnlockSkin(_selected.item.skinName);
                GameDataManager.Instance.UpdateCurrentSkin(_selected.item.skinName);
                OnSelectButtonClick();
            });
        }

        private void OnSelectButtonClick()
        {
            MenuController.Instance.UpdateCharacter();
            foreach (var item in _itemList)
            {
                item.SetSelected();
            }
            _controller.SetChangeClothes();
        }
    }
}
