using System;
using System.Collections;
using System.Collections.Generic;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;



public class WireConnectUIEditor : MonoBehaviour
{
    [LabelText("矩阵大小"),OnValueChanged("OnSelectWireMode")]
    public WireMode mode;
    
    [LabelText("Id")]
    public string wireGameId;

    private WireConnectCore _core3x3;
    private WireConnectCore _core4x4;

    public GameObject obj3x3;
    public GameObject obj4x4;

    public WireConnectCore curCore;

    public Image CanOpen;


    public InputField input;
    public Button loadBtn;
    public Button saveBtn;
    
    public void OnSave()
    {
        if (!string.IsNullOrEmpty(input.text) && curCore !=null)
        {
            curCore.SaveData(input.text);
        }
        else
        {
            GameDebug.Log("需要正确填写id");
        }
    }

    public void OnLoad()
    {
        if (!string.IsNullOrEmpty(input.text) && curCore !=null)
        {
            curCore.LoadData(input.text);
            curCore.RefreshAll();
        }
        else
        {
            GameDebug.Log("需要正确填写id");
        }
    }

    private void Start()
    {
        loadBtn.onClick.AddListener(OnLoad);
        saveBtn.onClick.AddListener(OnSave);
        
        _core3x3 = new WireConnectCore(3);
        _core4x4 = new WireConnectCore(4);
        curCore = mode == WireMode.x3 ? _core3x3 : _core4x4;

        obj3x3.SetActive(mode == WireMode.x3);
        obj4x4.SetActive(mode == WireMode.x4);
        Init();
    }

    private void Init()
    {
        var items3x3 = obj3x3.GetComponentsInChildren<WireConnectUIItemEditor>();
        for (int i = 0; i < items3x3.Length; i++)
        {
            items3x3[i].editorUI = this;
            items3x3[i].slotItem = _core3x3.itemList[items3x3[i].pos.x][items3x3[i].pos.y];
        }
        
        var items4x4 = obj4x4.GetComponentsInChildren<WireConnectUIItemEditor>();
        for (int i = 0; i < items4x4.Length; i++)
        {
            items4x4[i].editorUI = this;
            items4x4[i].slotItem = _core4x4.itemList[items4x4[i].pos.x][items4x4[i].pos.y];
        }
    }
    
    public void OnSelectWireMode()
    {
        switch (mode)
        {
            case WireMode.x3:
                Regenerate3x3();
                break;
            case WireMode.x4:
                Regenerate4x4();
                break;
        }
    }

    public void Regenerate3x3()
    {
        GameDebug.Log("选择了3x3的矩阵，重新生成棋盘");
        curCore = _core3x3;
        obj3x3.OnActive(true);
        obj4x4.OnActive(false);
    }
    
    public void Regenerate4x4()
    {
        GameDebug.Log("选择了4x4的矩阵，重新生成棋盘");
        curCore = _core4x4;
        obj3x3.OnActive(false);
        obj4x4.OnActive(true);
    }


    private void OnEnable()
    {
        EventCenter.Register<bool>(EventKey.WireConnectCanOpen ,OnRefreshOpen);
    }
    
    private void OnDisable()
    {
        EventCenter.UnRegister<bool>(EventKey.WireConnectCanOpen, OnRefreshOpen);
    }
    
    private void OnRefreshOpen(bool canOpen)
    {
        CanOpen.color = canOpen ? Color.green : Color.red;
    }
}
