using System;
using Module;
using UnityEngine;
using UnityEngine.UI;

public class BagCell : MonoBehaviour
{
    public Toggle toggle;
    public BagItem cellEntity;
    public Image icon;
    public Text count;
    public Action<BagCell> onSelectCell;
    public GameObject select;
    public bool isEmpty
    {
        get { return cellEntity == null; }
    }

    public void Init()
    {
        toggle.onValueChanged.AddListener(Select);
    }

    public void Select(bool active)
    {
        if (active)
        {
            UI3DShow.Instance.OnShow("BagUI",cellEntity.entity);
            onSelectCell?.Invoke(this);
        }
        select.OnActive(active);
    }

    public void SetInfo(BagItem item,Action callback)
    {
        toggle.interactable = true;
        this.cellEntity = item;
        cellEntity.entity.GetIcon(TypeList.Normal, sp =>
        {
            icon.sprite = sp;
            callback?.Invoke();
        });
        icon.gameObject.OnActive(true);
        RefreshCount();
    }

    public void ClearInfo()
    {
        toggle.interactable = false;
        toggle.isOn = false;
        this.cellEntity = null;
        icon.sprite = null;
        icon.gameObject.OnActive(false);
        count.transform.parent.gameObject.OnActive(false);
    }

    public void RefreshCount()
    {
        if ((cellEntity != null && (cellEntity.count == 1 || cellEntity.count == 0)) ||
            cellEntity.entity.dbData.bagShowCount == 0)
        {
            count.transform.parent.gameObject.OnActive(false);
        }
        else
        {
            count.transform.parent.gameObject.OnActive(true);
            count.text = cellEntity.count.ToString();
        }
    }
}
