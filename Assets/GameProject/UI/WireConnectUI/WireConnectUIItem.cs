using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WireConnectUIItem : MonoBehaviour ,IPointerDownHandler
{
    [LabelText("位置")] 
    public Vector2Int pos;

    public WireConnectUI ui;

    [TabGroup("电池")]
    public GameObject BatteryImg;
    [TabGroup("出口")]
    public WireConnectUIExport export;
    
    [TabGroup("旋钮")]
    public List<GameObject> knobObj;

    [HideInInspector]
    public List<GameObject> knobHighlightList;
    
    [LabelText("通用高亮物体")]
    public GameObject highLight;
    
    
    //数据层的item
    [ReadOnly]
    public WireSlotItem slotItem;
    

    //当前选择的knobtype
    private WireKnobType _curKnobType = WireKnobType.None;

    private void OnEnable()
    {
        EventCenter.Register<Dictionary<WirePort,int>>(EventKey.WireRefreshHighLight , OnRefreshHighLight);
        EventCenter.Register(EventKey.WireRefresh , OnRefresh);
        InitKnobHighlight();
    }
    
    private void OnDisable()
    {
        EventCenter.UnRegister<Dictionary<WirePort,int>>(EventKey.WireRefreshHighLight, OnRefreshHighLight);
        EventCenter.UnRegister(EventKey.WireRefresh , OnRefresh);
    }

    private void InitKnobHighlight()
    {
        knobHighlightList = new List<GameObject>();
        for (int i = 0; i < knobObj.Count; i++)
        {
            if (knobObj[i].transform.childCount > 0)
            {
                knobHighlightList.Add(knobObj[i].transform.GetChild(0).gameObject);    
            }
            else
            {
                knobHighlightList.Add(null);
            }
        }

    }
    

    private void OnRefreshHighLight(Dictionary<WirePort,int> visit)
    {
        if (slotItem.slotType == WireSlotType.Wire)
        {
            var ports = slotItem.ports;
            var active = false;
            foreach (var wirePort in ports)
            {
                var t = visit[wirePort];
                if (t > 0)
                {
                    active = true;
                }
            }
            highLight.OnActive(active);
        }
        else if (slotItem.slotType == WireSlotType.Knob)
        {
            OnRefreshHighLightKnob(visit);
        }
        else if (slotItem.slotType == WireSlotType.Battery)
        {
            OnRefreshHighLightBattery(visit);
        }
        else if (slotItem.slotType == WireSlotType.Export)
        {
            OnRefreshHighLightExport(visit);
        }
    }

    /// <summary>
    /// 根据端口的position找到预制体上对应高光的名称中包含position的物体，显示
    /// </summary>
    /// <param name="visit"></param>
    private void OnRefreshHighLightKnob(Dictionary<WirePort,int> visit)
    {
        var highlightObj = knobHighlightList[(int) _curKnobType];
        highlightObj.OnActive(false);
        int count = 0;
        string t = "";
        foreach (var wirePort in slotItem.ports)
        {
            if (visit[wirePort] <= 0)
            {
                continue;
            }
            count++;
            if (_curKnobType == WireKnobType.BothTurn) //这个特殊处理
            {
                t += wirePort.position;
            }
            else
            {
                highlightObj.OnActive(true);
            }
        }
        if (_curKnobType == WireKnobType.BothTurn )
        {
            highlightObj.OnActive(count == 4);
            var singleObj = knobObj[(int) _curKnobType].transform.GetChild(1).gameObject;
            singleObj.OnActive(count > 0 && count != 4);

            if (t == "01" || t == "10")
            {
                singleObj.transform.rotation = Quaternion.Euler(0,0,-90);
            }
            else if (t == "12" || t == "21")
            {
                singleObj.transform.rotation = Quaternion.Euler(0,0,-180);
            }
            else if (t == "23" || t == "32")
            {
                singleObj.transform.rotation = Quaternion.Euler(0,0,-270);
            }
            else if (t == "03" || t == "30")
            {
                singleObj.transform.rotation = Quaternion.Euler(0,0,0);
            }
        }
    }

    private void OnRefreshHighLightBattery(Dictionary<WirePort,int> visit)
    {
        var batteryItem = (WireSideItem) slotItem;
        BatteryImg.transform.GetChild(0).gameObject.SetActive(batteryItem.batteryIsLinkToExport);
        BatteryImg.transform.GetChild(1).gameObject.SetActive(!batteryItem.batteryIsLinkToExport);
    }

    private void OnRefreshHighLightExport(Dictionary<WirePort,int> visit)
    {
        highLight.OnActive(visit[slotItem.ports[0]] > 0);
        
        var exportItem = (WireSideItem) slotItem;
        export.SetHighlightCount(exportItem.GetLinkedExportNum());
    }
    

    private void OnRefresh()
    {
        if (highLight)
        {
            highLight.SetActive(false);
        }
        
        switch (slotItem.slotType)
        {
            case WireSlotType.None:
                export.gameObject.OnActive(false);
                BatteryImg.gameObject.OnActive(false);
                return;
            case WireSlotType.Knob:
                var t = slotItem as WireKnobItem;
                ChangeKnobObj(t.knobType);
                knobObj[(int) t.knobType].transform.rotation = Quaternion.Euler(0, 0, -t.rotate * 90);
                break;
            case WireSlotType.Wire:
                break;
            case WireSlotType.Battery:
                export.gameObject.OnActive(false);
                BatteryImg.gameObject.OnActive(true);
                highLight.SetActive(true);
                break;
            case WireSlotType.Export:
                export.gameObject.OnActive(true);
                BatteryImg.gameObject.OnActive(false);
                break;
        }
    }
    

    public void ChangeTo(PointerEventData pointerData)
    {
        switch (slotItem.slotType)
        {
            case WireSlotType.Knob:
                OnClickKnob(pointerData);
                break;
        }
    }
    
    

    public void OnClickKnob(PointerEventData pointerData)
    {
        if (_curKnobType == WireKnobType.None)
        {
            return;
        }
        
        UICommpont.FreezeUI("WireConnectClickKnob");
        var item = (slotItem as WireKnobItem);
        //旋转按钮
        item.RotatePorts();
        ui.Core.CanOpen(); //刷新DFS访问节点
        ui.Core.RefreshSlotItemHighLight(); //刷新所有高亮

        var curRotate = knobObj[(int)item.knobType].transform.rotation.eulerAngles.z;
        knobObj[(int) item.knobType].transform.DORotate(new Vector3(0,0,curRotate - 90),0.5f).
            SetEase(Ease.InOutSine).SetUpdate(true).OnComplete(() =>
            {
                item.RotatePortsOver();
                if (ui.Core.CanOpen())
                {
                    GameDebug.Log("可以开了！");
                }
                ui.Core.RefreshSlotItemHighLight(); //刷新所有高亮
                UICommpont.UnFreezeUI("WireConnectClickKnob");
            });
    }

    
    private void ChangeKnobObj(WireKnobType knobType)
    {
        foreach (var o in knobObj)
        {
            o.transform.rotation = Quaternion.Euler(0,0,0);
            o.OnActive(false);
        }

        _curKnobType = knobType;
        knobObj[(int)knobType].OnActive(true);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeTo(eventData);
    }
}