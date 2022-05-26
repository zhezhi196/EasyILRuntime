using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Module;

public class CollectionPlane : MonoBehaviour
{
    [Sirenix.OdinInspector.ReadOnly]
    public string itemPath = "UI/CollectionUI/CollectionItem.prefab";
   
    public Transform itemRoot;
    public List<CollectionItem> items;
    public Action<CollectionItem> onSelectItem;
    private CollectionItem selectItem = null;

    public void CreatItem(Collection c, Action callBack)
    {
        if (items == null || items.Count<=0)
        {
            items = new List<CollectionItem>();
        }
        AssetLoad.LoadGameObject<CollectionItem>(itemPath, itemRoot, (collectionItem, o) =>
        {
            int index = items.Count;
            collectionItem.sortIndex = index;
            items.Add(collectionItem);
            collectionItem.Init(c, callBack);
            collectionItem.button.AddListener(() =>
            {
                if (!collectionItem.isLock)
                {
                    OnSelectItem(collectionItem.sortIndex);
                }
            });
        });
    }

    public void Refesh()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].Refesh();
        }
        SortToggles<CollectionItem>((itm1, itm2) =>
        {
            if ((itm1.target.target.isGet && itm2.target.target.isGet) ||
                (!itm1.target.target.isGet && !itm2.target.target.isGet))
            {
                return itm1.target.target.collectionData.collectionIndex.CompareTo(itm2.target.target
                    .collectionData.collectionIndex);
            }
            else if (itm1.target.target.isGet && !itm2.target.target.isGet)
            {
                return -1;
            }
            else if (!itm1.target.target.isGet && itm2.target.target.isGet)
            {
                return 1;
            }

            return 0;
        }, true);
    }

    public void OnSelectItem(int index)
    {
        if (selectItem != items[index] && gameObject.activeSelf)
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].OnSelect(false);
            }
            
            selectItem = items[index];
            selectItem.OnSelect(true);
            onSelectItem?.Invoke(selectItem);
        }
    }

    public void ChangeState(bool b)
    {
        this.gameObject.OnActive(b);
        if (b)
        {
            Show();
        }
    }

    public void Show()
    {
        if (selectItem == null)
        {
            if (items[0].target.target.collectionStation != CollectionStation.UnGet)
            {
                OnSelectItem(0);
            }
            else
            {
                onSelectItem?.Invoke(null);
            }
        }
        else {
            onSelectItem?.Invoke(selectItem);
        }
    }

    public void Hide()
    {
        selectItem = null;
    }

    private void SortToggles<T>(Comparison<T> comparison, bool sortTransform) where T : CollectionItem
    {
        for (int i = 0; i < items.Count; i++)
        {
            Comparison<CollectionItem> newCom = (a, b) => comparison.Invoke(a as T, b as T);
            items.Sort(newCom);
        }

        if (sortTransform)
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].sortIndex = i;
                items[i].transform.SetSiblingIndex(i);
            }
        }
    }

    public Collection[] GetCollectionsArray()
    {
        Collection[] ret = new Collection[items.Count];
        for (int i = 0; i < items.Count; i++)
        {
            ret[i] = items[i].target;
        }
        return ret;
    }
}
