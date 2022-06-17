using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WireConnectUIExport : MonoBehaviour
{
    [FormerlySerializedAs("batteryList")] public List<Transform> exportList;
    public void SetBatteryCount(int count)
    {
        for (int i = 0; i < exportList.Count; i++)
        {
            exportList[i].gameObject.OnActive(false);
        }
        
        for (int i = 0; i < count; i++)
        {
            exportList[i].gameObject.OnActive(true);
            exportList[i].transform.GetChild(0).gameObject.OnActive(false);
            exportList[i].transform.GetChild(1).gameObject.OnActive(false);
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
        for (int i = 0; i < exportList.Count; i++)
        {
            exportList[i].transform.GetChild(0).gameObject.OnActive(true);
            exportList[i].transform.GetChild(1).gameObject.OnActive(false);
        }
        
        for (int i = 0; i < count; i++)
        {
            exportList[i].transform.GetChild(0).gameObject.OnActive(false);
            exportList[i].transform.GetChild(1).gameObject.OnActive(true);
        }
    }

    public void ChangeToGreen()
    {
        //黄光变绿光
        for (int i = 0; i < exportList.Count; i++)
        {
            exportList[i].transform.GetChild(0).gameObject.OnActive(false);
            exportList[i].transform.GetChild(1).gameObject.OnActive(true);
        }
    }

    public void Wink()
    {
        var seq = DOTween.Sequence();
        seq.SetUpdate(true);
        
        for (int i = 0; i < exportList.Count; i++)
        {
            var img = exportList[i].transform.GetChild(1).GetComponent<CanvasGroup>();
            seq.Insert(0,img.DOFade(0.2f, 0.3f));
            seq.Insert(0.3f,img.DOFade(1, 0.3f));
            seq.Insert(0.6f,img.DOFade(0.2f, 0.3f));
            seq.Insert(0.9f,img.DOFade(1, 0.3f));
        }
    }
    
}
