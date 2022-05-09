using System;
using System.Collections;
using System.Collections.Generic;
using Projects.Scripts.Scriptable;
using Spine.Unity;
using Truongtv.PopUpController;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.Popup
{
    public class PopupOpenChest : BasePopup
    {
        [SerializeField] private List<ChestItem> chestItemList;
        [SerializeField] private Transform chestTransform;
        [SerializeField] private Button closeButton;
        [SerializeField] private SkeletonGraphic graphic;
        [SerializeField, SpineAnimation] private string idle, open, idle2;
        private Action _close;
        private Action _start;
        private void Awake()
        {
            closeButton.onClick.AddListener(Close);
            closeAction = _close;
            
        }

        private void OnOpen(List<SkinInfo> skinList)
        {
            var entry = graphic.AnimationState.SetAnimation(0, idle, false);
            entry.Complete += trackEntry =>
            {
                var entry2 = graphic.AnimationState.SetAnimation(0, open, false);
                entry2.Complete += entry3 =>
                {
                    graphic.AnimationState.SetAnimation(0, idle2, true);
                    for (var i = 0; i < skinList.Count; i++)
                    {
                        chestItemList[i].gameObject.SetActive(true);
                        chestItemList[i].Init(skinList[i],chestTransform,i*0.25f);
                    }

                    for (var i = skinList.Count; i < chestItemList.Count; i++)
                    {
                        chestItemList[i].gameObject.SetActive(false);
                    }

                    StartCoroutine(ShowClose((skinList.Count + 2) * 0.25f));
                };
            };
        }
        public void Init(List<SkinInfo> skinList,Action closePopup = null)
        {
            _close = closePopup;
            closeButton.gameObject.SetActive(false);
            openCompleteAction = () =>
            {
                OnOpen(skinList);
            };
            
            
        }

        private IEnumerator ShowClose(float delay)
        {
            yield return new WaitForSeconds(delay);
            closeButton.gameObject.SetActive(true);
        }
    }
}
