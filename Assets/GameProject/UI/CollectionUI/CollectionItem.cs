using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Module;
using Sirenix.OdinInspector;

public class CollectionItem : MonoBehaviour
{
    public GameObject tipObj;
    public Image icon;
    public GameObject lockIcon;
    public GameObject select;
    public UIBtnBase button;
    
    [ReadOnly]
    public bool isLock = true;
    [ReadOnly]
    public Collection target;
    [ReadOnly]
    public int sortIndex = 0;

    /// <summary>
    /// 初始化方法
    /// </summary>
    /// <param name="collection">收集物数据</param>
    /// <param name="group">toggle的组</param>
    public void Init(Collection collection, Action spriteLoadedCallback)
    {
        target = collection;
        target.target.GetIcon(CollectionStation.Get, (s)=>
        {
            icon.sprite = s;
            spriteLoadedCallback?.Invoke();
        });
    }

    public void Refesh()
    {
        isLock = target.target.collectionStation == CollectionStation.UnGet;
        
        lockIcon.OnActive(isLock);
        icon.gameObject.OnActive(!isLock);
        tipObj.OnActive(target.target.collectionStation == CollectionStation.NewGet);
        // toggle.interactable = !isLock;
    }

    public void OnSelect(bool show)
    {
        if (select.gameObject.activeSelf == show)
        {
            return;
        }
        
        //TODO 可以设计成带有动画效果的，在这里处理
        // select.GetComponent<Image>().DOFade(show ? 1 : 0, 0.2f);
        // select.transform.GetChild(0).GetComponent<Image>().DOFade(show ? 1 : 0, 0.2f);
        select.gameObject.OnActive(show);
    }
    
}
