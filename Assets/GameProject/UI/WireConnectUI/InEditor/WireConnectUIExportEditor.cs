using System;
using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;

public class WireConnectUIExportEditor : MonoBehaviour
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
}
