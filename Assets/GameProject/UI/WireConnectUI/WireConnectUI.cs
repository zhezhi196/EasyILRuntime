using System.Collections;
using System.Collections.Generic;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;


public class WireConnectUI : UIViewBase
{
    public Transform Root;
    public GameObject obj3x3;
    public GameObject obj4x4;
    public UIBtnBase backBtn;
    public UIBtnBase successBackBtn;

    public WireMode wireMode;
    public WireConnectUIItem[] items3x3;
    public WireConnectUIItem[] items4x4;
    
    /// <summary>
    /// 数据层
    /// </summary>
    public WireConnectCore Core;

    /// <summary>
    /// 当前正在触发的Prop
    /// </summary>
    private WireConnectProp _prop;
    
    protected override void OnChildStart()
    {
        base.OnChildStart();
        Core = new WireConnectCore();
        Root.localScale = Tools.GetScreenScale();
        backBtn.AddListener(OnClickBack);
        successBackBtn.AddListener(OnClickBack);
    }

    public override void OnOpenStart()
    {
        base.OnOpenStart();
        EventCenter.Register<Dictionary<WirePort,int>>(EventKey.WireRefreshHighLight , OnRefreshHighLight);
        EventCenter.Register(EventKey.WireRefresh , OnRefresh);
        EventCenter.Register<bool>(EventKey.WireConnectCanOpen ,OnRefreshOpen);
    }
    

    public override void OnExit(params object[] args)
    {
        EventCenter.UnRegister<Dictionary<WirePort,int>>(EventKey.WireRefreshHighLight, OnRefreshHighLight);
        EventCenter.UnRegister(EventKey.WireRefresh , OnRefresh);
        EventCenter.UnRegister<bool>(EventKey.WireConnectCanOpen, OnRefreshOpen);

        base.OnExit(args);
    }
    
    public override void Refresh(params object[] args)
    {
        base.Refresh(args);
        
        if (args.Length == 0)
            return;
        _prop = (WireConnectProp) args[0];
        
        WireMode mode = Core.LoadData(_prop.configId.ToString() , false); //数据层加载数据
        //表现层加载数据
        Init(mode);
        //数据层刷新数据
        Core.RefreshAll();
    }
    
    private void Init(WireMode mode)
    {
        wireMode = mode;
        obj3x3.SetActive(mode == WireMode.x3);
        obj4x4.SetActive(mode == WireMode.x4);
        
        if (mode == WireMode.x3)
        {
            items3x3 = obj3x3.GetComponentsInChildren<WireConnectUIItem>();
            for (int i = 0; i < items3x3.Length; i++)
            {
                items3x3[i].ui = this;
                items3x3[i].slotItem = Core.itemList[items3x3[i].pos.x][items3x3[i].pos.y];
            }
        }
        else
        {
            items4x4 = obj4x4.GetComponentsInChildren<WireConnectUIItem>();
            for (int i = 0; i < items4x4.Length; i++)
            {
                items4x4[i].ui = this;
                items4x4[i].slotItem = Core.itemList[items4x4[i].pos.x][items4x4[i].pos.y];
            }
        }
    }
    
    private void OnRefresh()
    {
    }

    private void OnRefreshHighLight(Dictionary<WirePort, int> visit)
    {
        
    }
    
    private void OnRefreshOpen(bool canOpen)
    {
        if (canOpen)
        {
            GameDebug.Log("WireConnectUI可以开了！");
            OnConnectSuccess();
        }
    }

    private async void OnConnectSuccess()
    {
        _prop.UnlockSuccess();
        UICommpont.FreezeUI("WireConnectSuccess");
        //解谜成功时，全部变为绿光闪烁2下，然后保持绿光
        // await Async.WaitforSecondsRealTime(0.5f);
        WireConnectUIItem[] items = null;
        if (wireMode == WireMode.x3)
        {
            items = items3x3;
        }
        else
        {
            items = items4x4;
        }

        WireConnectUIExport export = null;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].slotItem.slotType == WireSlotType.Export)
            {
                export = items[i].export;
            }
        }

        export.ChangeToGreen();
        //TODO 发出机械运作音效
        
        //闪烁1s
        export.Wink();
        await Async.WaitforSecondsRealTime(1);
        
        //TODO 然后屏幕中心出现成功解谜提示文字
        successBackBtn.OnActive(true);
        UICommpont.UnFreezeUI("WireConnectSuccess");
        

    }

    private void OnClickBack()
    {
        successBackBtn.OnActive(false);
        UIController.Instance.Back();
    }
    
}
