using System;
using System.Collections.Generic;
using LitJson;
using Module;
using UnityEngine;

[Serializable]
public class WirePort
{
    public int position; //0 1 2 3  上 右 下 左 (顺时针)  电线和旋钮 0-2,1-3 能相互连接
    public List<WirePort> connectPorts; //此端口连接的其他端口
    public bool enabled; //是否开启，未开启就表示这个地方是断路，不通电，但是有这个口在
    public WirePortType portType; //端口类型
    public WireSlotItem slotItem;
    
    public bool isRotating = false; //旋转的时候假设这个点是不在的


    public WirePort(WireSlotItem slotItem, int position, WirePortType type, bool enabled)
    {
        this.position = position;
        this.enabled = enabled;
        portType = type;
        connectPorts = new List<WirePort>();
        this.slotItem = slotItem;

        slotItem.AddPortVisit(this); //为遍历算法做数据缓存
    }

    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
    }

    public void Disconnect()
    {
        for (int i = 0; i < connectPorts.Count; i++)
        {
            //把和和自己不同类型的接口都断开，knob的电线不断开
            if (connectPorts[i].portType != portType)
            {
                var wirePort = connectPorts[i];
                connectPorts.RemoveAt(i); //从列表中删除
                i--;
                wirePort.connectPorts.Remove(this); //调用电线的断开连接
            }
        }
    }

    //双方互联，去重
    public void Connect(WirePort port)
    {
        if (port == this)
        {
            return; //不和自己联
        }

        if (connectPorts.Contains(port))
        {
            return;
        }

        connectPorts.Add(port);

        port.Connect(this);
    }
}

public class WireSave
{
    public int wireType;
    public List<List<WireSaveSlotItem>> items;
}

public class WireSaveSlotItem
{
    public int type;
    public List<int> pos;
    public bool enabled;
    public int lineType;
    public int knobType;
    public int rotate;
}


public class WireConnectCore
{
    public const string SavePath = "Assets/Bundles/TinyGame/WireConnect/{0}.json";
    public List<List<WireSlotItem>> itemList;
    public Dictionary<WirePort, int> dfsVisit;
    public int size;
    public int batteryCount = 0;
    public int exportCount = 0;

    public int findExportCount = 0;
    /// <summary>
    /// Editor调用
    /// </summary>
    /// <param name="size"></param>
    public WireConnectCore(int size)
    {
        this.size = size;
        if (size == 3)
        {
            CreateItemList(5, 9);
        }
        else if (size == 4)
        {
            CreateItemList(7, 11);
        }
    }
    
    /// <summary>
    /// 游戏内调用
    /// </summary>
    public WireConnectCore()
    {
    }
    
    private void CreateItemList(int row, int column)
    {
        itemList = new List<List<WireSlotItem>>(7);
        dfsVisit = new Dictionary<WirePort, int>();
        
        for (int i = 0; i < row; i++)
        {
            List<WireSlotItem> tempItem = new List<WireSlotItem>(column);
            for (int j = 0; j < column; j++)
            {
                if (j == 0 || j == column - 1) //添加slotItem
                {
                    tempItem.Add(i % 2 == 0
                        ? new WireSideItem(this, WireSlotType.None, i, j, true, j == 0)
                        : new WireSlotItem(this, WireSlotType.None, i, j, false));
                }
                else if (j % 2 == 0) //偶数列
                {
                    if (i % 2 == 0) //偶数行
                    {
                        tempItem.Add(new WireKnobItem(this, WireSlotType.Knob, i, j, true, WireKnobType.None));
                    }
                    else //奇数行
                    {
                        tempItem.Add(new WireLineItem(this, WireSlotType.Wire, i, j, true, WireLineType.Vertical));
                    }
                }
                else //奇数列
                {
                    if (i % 2 == 0) //偶数行
                    {
                        tempItem.Add(new WireLineItem(this, WireSlotType.Wire, i, j, true, WireLineType.Horizontal));
                    }
                    else
                    {
                        tempItem.Add(new WireSlotItem(this, WireSlotType.None, i, j, false));
                    }
                }
            }

            itemList.Add(tempItem);
        }

        // RefreshAllWireLink(); //创建完后全部刷新连接关系
    }


    /// <summary>
    /// 刷新所有电器的连接
    /// </summary>
    public void RefreshAllWireLink()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            for (int j = 0; j < itemList[i].Count; j++)
            {
                itemList[i][j].WireDisconnect();
                RefreshWireLink(itemList[i][j]);
            }
        }
    }

    /// <summary>
    /// 刷新指定电线的连接
    /// </summary>
    /// <param name="item"></param>
    public void RefreshWireLink(WireSlotItem item)
    {
        if (!item.enabled)
        {
            return;
        }
        
        for (int i = 0; i < item.ports.Count; i++)
        {
            if (item.ports[i].isRotating)
            {
                continue;
            }
            int findPortPos = GetAdjacentSlotItem(item, item.ports[i], out WireSlotItem t);
            if (t == null)
            {
                continue;
            }

            WireLink(item.ports[i], t, findPortPos);
        }
    }

    /// <summary>
    /// 得到相邻的SlotItem
    /// </summary>
    /// <param name="fromItem">当前的SlotItem</param>
    /// <param name="fromPort">当前的WirePort</param>
    /// <param name="ret">根据port得到的相邻SlotItem</param>
    /// <returns>返回需要找到的Port pos</returns>
    private int GetAdjacentSlotItem(WireSlotItem fromItem, WirePort fromPort, out WireSlotItem ret)
    {
        //0-2 1-3 2-0 3-1
        switch (fromPort.position)
        {
            case 0:
                if (fromItem.position.x <= 0)
                {
                    break;
                }

                ret = itemList[fromItem.position.x - 1][fromItem.position.y]; //取上
                return 2;
            case 1:
                if (fromItem.position.y + 1 >= itemList[fromItem.position.x].Count)
                {
                    break;
                }

                ret = itemList[fromItem.position.x][fromItem.position.y + 1]; //取右
                return 3;
            case 2:
                if (fromItem.position.x + 1 >= itemList.Count)
                {
                    break;
                }

                ret = itemList[fromItem.position.x + 1][fromItem.position.y]; //取下
                return 0;
            case 3:
                if (fromItem.position.y <= 0)
                {
                    break;
                }

                ret = itemList[fromItem.position.x][fromItem.position.y - 1]; //取左
                return 1;
            default:
                ret = null;
                return -1;
        }

        ret = null;
        return -1;
    }


    /// <summary>
    /// 电线连接
    /// </summary>
    /// <param name="port">此port</param>
    /// <param name="to">需要连接的slot</param>
    /// <param name="findPortPos">需要连接的slot的port位置</param>
    private void WireLink(WirePort port, WireSlotItem to, int findPortPos)
    {
        if (!to.enabled) //这个位置没有启动，返回
        {
            return;
        }
        

        WirePort find = to.ports.Find(v => v.position == findPortPos);
        if (find != null && find.enabled && !find.isRotating) //这个端口也得打开才能连接上
        {
            find.Connect(port);
        }
    }


    //---------------------------------------------------
    public void OnAddBattery(int x, int y)
    {
        if (itemList[x][y].slotType == WireSlotType.None)
        {
            itemList[x][y].SetSlotType(WireSlotType.Battery);

            (itemList[x][y] as WireSideItem)?.SetEnabled(true);
            batteryCount++;
            EventCenter.Dispatch<int>(EventKey.WireBatteryChange, batteryCount);
            CanOpen();
        }
    }

    public void OnSubBattery(int x, int y)
    {
        itemList[x][y].SetSlotType(WireSlotType.None);
        (itemList[x][y] as WireSideItem)?.SetEnabled(false);
        batteryCount--;
        EventCenter.Dispatch<int>(EventKey.WireBatteryChange, batteryCount);
        CanOpen();
    }

    public void OnAddExport(int x, int y)
    {
        if (itemList[x][y].slotType == WireSlotType.None)
        {
            itemList[x][y].SetSlotType(WireSlotType.Export);
            (itemList[x][y] as WireSideItem)?.SetEnabled(true);
            exportCount++;
            EventCenter.Dispatch<int>(EventKey.WireBatteryChange, batteryCount);
            CanOpen();
        }
    }

    public void OnSubExport(int x, int y)
    {
        itemList[x][y].SetSlotType(WireSlotType.None);
        (itemList[x][y] as WireSideItem)?.SetEnabled(false);
        exportCount--;
        CanOpen();
    }


    /// <summary>
    /// 运行检测程序，检查当前电路是否通畅
    /// </summary>
    /// <returns></returns>
    public bool CanOpen()
    {
        ResetVisit(); //重置Port访问缓存

        List<WireSideItem> allBattery = new List<WireSideItem>();

        for (int i = 0; i < itemList.Count; i++)
        {
            for (int j = 0; j < itemList[i].Count; j++)
            {
                if (itemList[i][j].slotType == WireSlotType.Battery)
                {
                    allBattery.Add((WireSideItem) itemList[i][j]);
                }
            }
        }

        if (allBattery.Count <= 0 || exportCount == 0)
        {
            return false;
        }

        findExportCount = 0;
        for (int i = 0; i < allBattery.Count; i++)
        {
            if (DFS(allBattery[i].ports[0], i + 1))
            {
                allBattery[i].batteryIsLinkToExport = true;
                findExportCount++;
            }
            else
            {
                allBattery[i].batteryIsLinkToExport = false;
            }
        }

        bool canOpen = findExportCount == batteryCount;

        EventCenter.Dispatch(EventKey.WireConnectCanOpen, canOpen);

        return canOpen;
    }

    private bool DFS(WirePort port, int batteryId)
    {
        if (!port.enabled) //不激活的port不访问
        {
            return false;
        }

        if (dfsVisit[port] == batteryId) //某一块电池的电流通过的port不访问
        {
            return false;
        }

        dfsVisit[port] = batteryId;
        if (port.slotItem.slotType == WireSlotType.Export)
        {
            return true;
        }

        bool find = false;
        for (int i = 0; i < port.connectPorts.Count; i++)
        {
            if (DFS(port.connectPorts[i], batteryId))
            {
                find = true;
            }
        }

        return find;
    }

    public void RefreshSlotItemHighLight()
    {
        EventCenter.Dispatch(EventKey.WireRefreshHighLight, dfsVisit);
    }

    private void ResetVisit()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            for (int j = 0; j < itemList[i].Count; j++)
            {
                if (!itemList[i][j].enabled)
                {
                    continue;
                }

                for (int k = 0; k < itemList[i][j].ports.Count; k++)
                {
                    //这里是真的恶心，字典数据遍历的时候不能更改…暂时用三个循环解决，循环总数是一样的，就是比较恶心
                    dfsVisit[itemList[i][j].ports[k]] = 0;
                }
            }
        }
    }


    /// <summary>
    /// 从json读取数据并填充数据结构
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isEditor"></param>
    /// <returns></returns>
    public WireMode LoadData(string id , bool isEditor = true)
    {
        //读取棋盘data，赋值到core里
        string json = FileUtility.SafeReadAllText(string.Format(SavePath, id));
        if (json.IsNullOrEmpty())
        {
            GameDebug.Log($"没有到文件！{id}");
            return WireMode.x3;
        }

        batteryCount = 0;
        exportCount = 0;
        WireSave save = JsonMapper.ToObject<WireSave>(json);
        WireMode ret = WireMode.x3;
        if (!isEditor)
        {
            ret = (WireMode) save.wireType;
            if (ret == WireMode.x3)
            {
                CreateItemList(5, 9);
                size = 3;
            }
            else
            {
                CreateItemList(7, 11);
                size = 4;
            }
        }
        
        for (int i = 0; i < save.items.Count; i++)
        {
            for (int j = 0; j < save.items[i].Count; j++)
            {
                WireSlotType type = (WireSlotType)save.items[i][j].type;
                itemList[i][j].SetSlotType(type);
                if (type == WireSlotType.Knob)
                {
                    var t = (itemList[i][j] as WireKnobItem);
                    t.SetKnobType((WireKnobType)save.items[i][j].knobType);
                    t.SetRotate(save.items[i][j].rotate);
                }
                else if (type == WireSlotType.Wire)
                {
                    var t = (itemList[i][j] as WireLineItem);
                    t.lineType = (WireLineType)save.items[i][j].lineType;
                }
                else if (type == WireSlotType.Battery)
                {
                    batteryCount++;
                }
                else if (type == WireSlotType.Export)
                {
                    exportCount++;
                }
                itemList[i][j].SetEnabled(save.items[i][j].enabled);
            }
        }

        return ret;
    }

    public void RefreshAll()
    {
        RefreshAllWireLink();
        CanOpen();
        EventCenter.Dispatch(EventKey.WireRefresh);
        EventCenter.Dispatch(EventKey.WireBatteryChange ,batteryCount);
        RefreshSlotItemHighLight();
    }

    public void SaveData(string id)
    {
        WireSave save = new WireSave();
        save.wireType = size == 3 ? (int) WireMode.x3 : (int) WireMode.x4;
        save.items = new List<List<WireSaveSlotItem>>();

        for (int i = 0; i < itemList.Count; i++)
        {
            List<WireSaveSlotItem> temp = new List<WireSaveSlotItem>();
            for (int j = 0; j < itemList[i].Count; j++)
            {
                WireSaveSlotItem item = new WireSaveSlotItem();
                item.enabled = itemList[i][j].enabled;
                item.pos = new List<int>() {itemList[i][j].position.x, itemList[i][j].position.y};
                item.type = (int) itemList[i][j].slotType;
                if (itemList[i][j].slotType == WireSlotType.Knob)
                {
                    var t = (itemList[i][j] as WireKnobItem);
                    item.knobType = (int) t.knobType;
                    item.rotate = t.rotate;
                }
                else if (itemList[i][j].slotType == WireSlotType.Wire)
                {
                    var t = (itemList[i][j] as WireLineItem);
                    item.lineType = (int) t.lineType;
                }
                temp.Add(item);
            }
            save.items.Add(temp);
        }

        string json = JsonMapper.ToJson(save);
        FileUtility.SafeWriteAllText(string.Format(SavePath, id), json);
    }

    //---------------------------------------------------
}