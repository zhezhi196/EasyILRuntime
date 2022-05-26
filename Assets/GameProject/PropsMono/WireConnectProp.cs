using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 电箱
/// </summary>
public class WireConnectProp : OnlyInteractiveOpenUI
{
    [LabelText("配置文件名称")]
    public int configId;
    
    public override bool canInteractive
    {
        get
        {
            return ContainStation(PropsStation.Locked) && base.canInteractive;
        }
    }
    
    
    
    /// <summary>
    /// 小游戏解锁成功
    /// </summary>
    public void UnlockSuccess()
    {
        RunLogicalOnSelf(RunLogicalName.RemoveLock); //自身加锁
        creator.isGet = true;
    }
}
