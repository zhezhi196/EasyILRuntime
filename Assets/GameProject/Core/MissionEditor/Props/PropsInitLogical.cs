using System;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class PropsInitLogical
{
    [LabelText("初始化逻辑"),OnValueChanged("GetPropsStation")]
    public RunLogicalName runLogical;
    
    [LabelText("初始化状态"),ReadOnly]
    public PropsStation station;
    
    [LabelText("初始化参数")]
    public string[] args;

    public static PropsInitLogical GetInit(PropSave saveData)
    {
        PropsInitLogical result = new PropsInitLogical();
        result.runLogical = saveData.lastRunlogicalName.ToEnum<RunLogicalName>();
        result.station = (PropsStation) saveData.saveStation;
        result.args = saveData.runlogicalArgs;
        return result;
    }

    public void GetPropsStation()
    {
        if (runLogical == RunLogicalName.On||runLogical == RunLogicalName.None)
        {
            station = 0;
        }
        else if (runLogical == RunLogicalName.Off)
        {
            station = PropsStation.Off;
        }
        else if (runLogical == RunLogicalName.Lock)
        {
            station = PropsStation.Off | PropsStation.Locked;
        }
        else if (runLogical == RunLogicalName.Hide)
        {
            station = PropsStation.Hide;
        }
        else if (runLogical == RunLogicalName.UnActive)
        {
            station = PropsStation.UnActive;
        }
    }
}
