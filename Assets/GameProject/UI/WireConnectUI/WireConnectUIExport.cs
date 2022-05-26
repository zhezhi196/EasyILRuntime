using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using UnityEngine;
using UnityEngine.UI;

public class WireConnectUIExport : MonoBehaviour
{
    public List<Transform> batteryList;
    public void SetBatteryCount(int count)
    {
        for (int i = 0; i < batteryList.Count; i++)
        {
            batteryList[i].gameObject.OnActive(false);
        }
        
        for (int i = 0; i < count; i++)
        {
            batteryList[i].gameObject.OnActive(true);
            batteryList[i].transform.GetChild(0).gameObject.OnActive(false);
            batteryList[i].transform.GetChild(1).gameObject.OnActive(false);
        }
    }

    private void OnEnable()
    {
        EventCenter.Register<int>(EventKey.WireBatteryChange ,SetBatteryCount);
    }

    private void OnDisable()
    {
        EventCenter.UnRegister<int>(EventKey.WireBatteryChange ,SetBatteryCount);
    }

    public void SetHighlightCount(int count)
    {
        for (int i = 0; i < batteryList.Count; i++)
        {
            batteryList[i].transform.GetChild(0).gameObject.OnActive(false);
        }
        
        for (int i = 0; i < count; i++)
        {
            batteryList[i].transform.GetChild(0).gameObject.OnActive(true);
        }
    }

    public void ChangeToGreen()
    {
        //黄光变绿光
        for (int i = 0; i < batteryList.Count; i++)
        {
            batteryList[i].transform.GetChild(0).gameObject.OnActive(false);
            batteryList[i].transform.GetChild(1).gameObject.OnActive(true);
        }
    }

    public void Wink()
    {
        var seq = DOTween.Sequence();
        seq.SetUpdate(true);
        
        for (int i = 0; i < batteryList.Count; i++)
        {
            var img = batteryList[i].transform.GetChild(1).GetComponent<Image>();
            seq.Insert(0,img.DOFade(0.2f, 0.3f));
            seq.Insert(0.3f,img.DOFade(1, 0.3f));
            seq.Insert(0.6f,img.DOFade(0.2f, 0.3f));
            seq.Insert(0.9f,img.DOFade(1, 0.3f));
        }
    }
    
}
