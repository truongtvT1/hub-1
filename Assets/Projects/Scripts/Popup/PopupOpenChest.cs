using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        [SerializeField] private SkeletonGraphic graphic;
        [SerializeField, SpineAnimation] private string idle, open, idle2;
        [SerializeField] private GameObject spark;

        private void OnOpen(List<SkinInfo> skinList)
        {
            var entry = graphic.AnimationState.SetAnimation(0, idle, false);
            entry.Complete += trackEntry =>
            {
                var entry2 = graphic.AnimationState.SetAnimation(0, open, false);
                entry2.Complete += entry3 =>
                {
                    graphic.AnimationState.SetAnimation(0, idle2, true).Complete+= entry1 =>
                    {
                        backGroundButton.onClick.AddListener(Close);
                    };
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
            closeAction = closePopup;
            spark.SetActive(false);
            graphic.AnimationState.SetAnimation(0, idle, true);
            foreach (var item in chestItemList)
            {
                item.gameObject.SetActive(false);
            }
            openCompleteAction = () =>
            {
                OnOpen(skinList);
            };
        }

        private void OnDisable()
        {
            closeAction = null;
        }

        private IEnumerator ShowClose(float delay)
        {
            yield return new WaitForSeconds(delay);
            spark.SetActive(true);
        }
    }
}
