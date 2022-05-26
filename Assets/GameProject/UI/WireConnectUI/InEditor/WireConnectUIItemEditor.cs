using System;
using System.Collections;
using System.Collections.Generic;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WireConnectUIItemEditor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [LabelText("位置")] public Vector2Int pos;

    public WireConnectUIEditor editorUI;

    [TabGroup("电池")]
    public GameObject BatteryImg;
    [TabGroup("出口")]
    public WireConnectUIExportEditor export;
    
    [TabGroup("旋钮")]
    public List<GameObject> knobObj;
    
    public GameObject highLight;
    
    
    //数据层的item
    [ReadOnly]
    public WireSlotItem slotItem; 


    private float pressTime = 0;
    private bool isPressed = false;
    private float waitTime = 0.6f;


    private void OnEnable()
    {
        EventCenter.Register<Dictionary<WirePort,int>>(EventKey.WireRefreshHighLight , OnRefreshHighLight);
        EventCenter.Register(EventKey.WireRefresh , OnRefresh);
    }
    
    private void OnDisable()
    {
        EventCenter.UnRegister<Dictionary<WirePort,int>>(EventKey.WireRefreshHighLight, OnRefreshHighLight);
        EventCenter.UnRegister(EventKey.WireRefresh , OnRefresh);
    }

    private void OnRefreshHighLight(Dictionary<WirePort,int> visit)
    {
        if (highLight)
        {
            if (slotItem.slotType == WireSlotType.Knob || slotItem.slotType == WireSlotType.Wire)
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
        }
    }

    private void OnRefresh()
    {
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
                break;
            case WireSlotType.Export:
                export.gameObject.OnActive(true);
                BatteryImg.gameObject.OnActive(false);
                break;
        }
    }
    



    void Update()
    {
        if (isPressed)
        {
            pressTime += Time.deltaTime;
        }
    }

    public void ChangeTo(bool longPress , PointerEventData pointerData)
    {
        switch (slotItem.slotType)
        {
            case WireSlotType.None:
                OnClickNone(longPress);
                return;
            case WireSlotType.Knob:
                OnClickKnob(pointerData);
                break;
            // case WireSlotType.Wire:
            //     OnClickWire();
            //     break;
            case WireSlotType.Battery:
                OnClickBattery();
                break;
            case WireSlotType.Export:
                OnClickExport();
                break;
        }
    }
    
    public virtual void OnClickWire()
    {

    }


    public void OnClickNone(bool longPress)
    {
        if (longPress)
        {
            ChangeToExport();
        }
        else
        {
            ChangeToBattery();
        }
    }

    public void OnClickKnob(PointerEventData pointerData)
    {
        var item = (slotItem as WireKnobItem);
        if (pointerData.pointerId == -1) //左键 旋转按钮
        {
            item.RotatePorts();
            item.RotatePortsOver();
            knobObj[(int) item.knobType].transform.rotation = Quaternion.Euler(0, 0, -item.rotate * 90);
        }
        else if (pointerData.pointerId == -2) //右键 更换按钮样式
        {
            item.SetKnobType((WireKnobType)((int)item.knobType + 1));
            ChangeKnobObj(item.knobType);
        }
        
        if (editorUI.curCore.CanOpen())
        {
            GameDebug.Log("可以开了！");
        }

        editorUI.curCore.RefreshSlotItemHighLight(); //刷新所有高亮
    }

 
    public void OnClickBattery()
    {
        editorUI.curCore.OnSubBattery(pos.x ,pos.y);
        editorUI.curCore.RefreshSlotItemHighLight();
        BatteryImg.OnActive(false);
    }
    public void ChangeToBattery()
    {
        editorUI.curCore.OnAddBattery(pos.x ,pos.y);
        editorUI.curCore.RefreshSlotItemHighLight();
        BatteryImg.OnActive(true);
    }
    
    
    public void OnClickExport()
    {
        if (editorUI.curCore.exportCount == 0)
        {
            return;
        }
        export.gameObject.OnActive(false);
        
        editorUI.curCore.OnSubExport(pos.x ,pos.y);
        editorUI.curCore.RefreshSlotItemHighLight();
    }
    public void ChangeToExport()
    {
        if (editorUI.curCore.exportCount > 0)
        {
            return;
        }
        
        export.gameObject.OnActive(true);
        editorUI.curCore.OnAddExport(pos.x ,pos.y);
        editorUI.curCore.RefreshSlotItemHighLight();
    }
    
    
    public void ChangeKnobObj(WireKnobType knobType)
    {
        foreach (var o in knobObj)
        {
            o.transform.rotation = Quaternion.Euler(0,0,0);
            o.OnActive(false);
        }
        
        knobObj[(int)knobType].OnActive(true);
    }

    

    public void OnPointerDown(PointerEventData eventData)
    {
        pressTime = 0;
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ChangeTo(pressTime >= waitTime , eventData);
        pressTime = 0;
    }
}