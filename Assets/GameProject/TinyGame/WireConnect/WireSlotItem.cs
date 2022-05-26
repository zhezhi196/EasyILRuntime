using System;
using System.Collections.Generic;
using UnityEngine;

//棋盘上的元素基类
[Serializable]
public class WireSlotItem
{
    public WireSlotType slotType; //元素类型
    public Vector2Int position; //棋盘上的位置
    public bool enabled; //这个元素是否开启，不开启，UI不显示
    public List<WirePort> ports;    //插槽上的所有电线接口
    protected WireConnectCore _core;

    public WireSlotItem(WireConnectCore core , WireSlotType type, int x, int y, bool enable)
    {
        _core = core;
        slotType = type;
        position = new Vector2Int(x, y);
        enabled = enable;
    }

    public virtual void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
    }

    public void WireDisconnect()
    {
        if (ports != null)
        {
            for (int i = 0; i < ports.Count; i++)
            {
                ports[i].Disconnect();
            }
        }
    }

    public void SetSlotType(WireSlotType type)
    {
        slotType = type;
    }

    public void AddPortVisit(WirePort addPort)
    {
        _core.dfsVisit[addPort] = 0;
    }
}

//电池/出口元素
[Serializable]
public class WireSideItem : WireSlotItem
{
    public bool batteryIsLinkToExport = false;
    public WireSideItem(WireConnectCore core , WireSlotType type, int x, int y, bool enable, bool left) : base(core, type, x, y, enable)
    {
        ports = new List<WirePort>(1) {new WirePort(this, left ? 1 : 3, WirePortType.Battery, enable)};
    }
    
    public override void SetEnabled(bool enabled)
    {
        base.SetEnabled(enabled);

        if (!enabled)
        {
            WireDisconnect();
        }
        else
        {
            _core.RefreshWireLink(this); //连接其他电器
        }
    }
    
    public int GetLinkedExportNum()
    {
        return _core.findExportCount;
    }
}


//电线元素
[Serializable]
public class WireLineItem : WireSlotItem
{
    public WireLineType lineType;
    
    public WireLineItem(WireConnectCore core , WireSlotType type, int x, int y, bool enable, WireLineType lType) : base(core, type, x, y, enable)
    {
        lineType = lType;
        ports = new List<WirePort>(2)
        {
            new WirePort(this, lineType == WireLineType.Vertical ? 0 : 1, WirePortType.Wire, enable),
            new WirePort(this, lineType == WireLineType.Vertical ? 2 : 3, WirePortType.Wire, enable)
        };
        //竖着和横着分别使用不同的位置，竖着02 横着13
        
        if (enable)
        {
            ResetPorts();
        }
    }

    private void ResetPorts()
    {
        ports[0].Connect(ports[1]);
    }

    public override void SetEnabled(bool enabled)
    {
        //TIP: 2022/5/10 李佳鑫要求暂时线都是open的
        base.SetEnabled(true);

        // if (!enabled)
        // {
        //     WireDisconnect();
        // }
        // else
        // {
        //     ports[0].Connect(ports[1]); //内部相连，电线之后两个端口，不能禁用
        // }
        ports[0].Connect(ports[1]); //内部相连，电线之后两个端口，不能禁用
        _core.RefreshWireLink(this); //连接其他电器
    }
}

/// <summary>
/// 旋钮元素
/// </summary>
[Serializable]
public class WireKnobItem : WireSlotItem
{
    public WireKnobType knobType;
    public int rotate;
    public WireKnobItem(WireConnectCore core , WireSlotType type, int x, int y, bool enable, WireKnobType kType) : base(core , type, x, y, enable)
    {
        knobType = kType;
        rotate = 0;
        ports = new List<WirePort>(4);
        for (int i = 0; i < 4; i++)
        {
            ports.Add(new WirePort(this, i, WirePortType.InKnob, enable));
        }
        
        if (enable)
        {
            ResetPorts();
        }
    }

    /// <summary>
    /// 更换旋钮类型
    /// </summary>
    public void SetKnobType(WireKnobType type)
    {
        //类型保护
        if ((int) type > 5)
        {
            type = WireKnobType.None;
        }

        knobType = type;
        WireDisconnect(); //更换类型，全部端口断开
        ResetPorts();    //重置接口
        _core.RefreshWireLink(this); //然后重新连接
        rotate = 0;
    }

    public void SetRotate(int set)
    {
        if (set > 3 || set < 0)
        {
            return;
        }

        rotate = set;
        Rotate(rotate);
        RotatePortsOver();
    }

    /// <summary>
    /// 旋转端口
    /// </summary>
    public void RotatePorts()
    {
        rotate += 1;
        if (rotate > 3)
        {
            rotate = 0;
        }
        Rotate(1);
    }

    private void Rotate(int rot)
    {
        //旋转的时候内部的port是不会断开连接的，但是每个port的position编号会变化，顺时针+1
        //旋转的时候先断开
        WireDisconnect();    
        for (int i = 0; i < ports.Count; i++)
        {
            ports[i].isRotating = true;
        }
        for (int i = 0; i < ports.Count; i++)
        {
            ports[i].position = (ports[i].position + rot) % 4; //角也跟着转
        }
    }

    public void RotatePortsOver()
    {
        for (int i = 0; i < ports.Count; i++)
        {
            ports[i].isRotating = false;
        }
        _core.RefreshWireLink(this); //然后重新连接
    }
    
    public void ResetPorts()
    {
        for (int i = 0; i < 4; i++)
        {
            ports[i].connectPorts = new List<WirePort>();//重置连接关系
            ports[i].SetEnabled(false);
            ports[i].position = i;
        }

        switch (knobType)
        {
            case WireKnobType.None: //空白格子，每个方向都没有接口
                for (int i = 0; i < 4; i++)
                {
                    ports[i].SetEnabled(false);
                }
                break;
            case WireKnobType.All: //四个方向都有接口
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        ports[i].Connect(ports[j]);
                    }
                    ports[i].SetEnabled(true);
                }

                break;
            case WireKnobType.Line: //默认上下两个方向有接口
                for (int i = 0; i < 4; i++)
                {
                    if (ports[i].position == 0 || ports[i].position == 2)
                    {
                        ports[i].SetEnabled(true);
                        ports[i].Connect(ports[2]);
                    }
                }

                break;
            case WireKnobType.BothTurn:
                for (int i = 0; i < 4; i++)
                {
                    if (ports[i].position == 0 )
                    {
                        ports[i].Connect(ports[3]);
                    }
                    else if(ports[i].position == 1)
                    {
                        ports[i].Connect(ports[2]);
                    }
                    ports[i].SetEnabled(true);
                }
                break;
            case WireKnobType.Turn:
                for (int i = 0; i < 4; i++)
                {
                    if (ports[i].position == 0 || ports[i].position == 3)
                    {
                        ports[i].Connect(ports[3]);
                        ports[i].SetEnabled(true);
                    }
                }
                break;
            case WireKnobType.TModel:
                for (int i = 0; i < 4; i++)
                {
                    if (ports[i].position == 0)
                    {
                        ports[i].Connect(ports[3]);
                        ports[i].Connect(ports[1]);
                        ports[i].SetEnabled(true);
                    }
                    else if (ports[i].position == 1 || ports[i].position == 3)
                    {
                        ports[i].Connect(ports[3]);
                        ports[i].SetEnabled(true);
                    }
                }
                break;
        }
        
    }
    
}