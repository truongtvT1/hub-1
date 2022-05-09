using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Projects.Scripts.Hub.Component;
using Projects.Scripts.Menu.Customize;
using Projects.Scripts.Scriptable;
using UnityEngine;

public class ChestItem : MonoBehaviour
{
    [SerializeField] private SkinItem item;
        
    public void Init(SkinInfo skin,Transform chestTransform,float delay)
    {
        item.gameObject.SetActive(true);
        item.transform.position = chestTransform.position;
        item.transform.localScale = Vector3.zero;
        item.Init(skin,null,null,false);
        var sequence = DOTween.Sequence();
        sequence.Append(item.transform.DOScale(1, 0.5f));
        sequence.Join(item.transform.DOLocalMove(Vector3.zero, 0.5f));
        sequence.SetDelay(delay);
        sequence.Play();
    }
}
