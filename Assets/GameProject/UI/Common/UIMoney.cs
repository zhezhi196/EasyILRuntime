using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using UnityEngine.UI;

public class UIMoney : MonoBehaviour
{
    public UIBtnBase plusBtn;
    public Text moneyCount;
    public MoneyType moneyType;

    private void Awake()
    {
        moneyCount.text = MoneyInfo.GetMoneyEntity(moneyType).count.ToString();
        MoneyInfo.onMoneyChanged += OnMoneyChange;
        plusBtn.AddListener(OnClickPlus);
        transform.localScale = Tools.GetScreenScale();
    }

    private void OnDestroy()
    {
        MoneyInfo.onMoneyChanged -= OnMoneyChange;
    }

    private void OnMoneyChange(MoneyType type,int change)
    {
        if (moneyType == type)
        {
            moneyCount.text = MoneyInfo.GetMoneyEntity(moneyType).count.ToString();
        }
    }

    private void OnClickPlus()
    {
        Commercialize.OpenStore();
        GameDebug.Log("货币加号打开商店页");
    }
}
