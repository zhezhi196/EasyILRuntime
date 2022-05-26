using Module;
using Sirenix.OdinInspector;
using UnityEngine;
/// <summary>
/// 需要撬锁的门 (玩小游戏解锁)
/// </summary>
public class DoorLockPickProp : DoorRotate
{
    public override InterActiveStyle interactiveStyle => InterActiveStyle.Handle;

    public override bool canInteractive => !IsAnimating && ContainStation(PropsStation.Locked) && isActive;
    
    [LabelText("开锁小游戏配置id")]
    public int unlockId = 0;
    
    protected override bool OnInteractive(bool fromMonster = false)
    {
        // if (!base.OnInteractive(fromMonster))
        // {
        //     return false;
        // }
        
        if (ContainStation(PropsStation.Locked))
        {
            if (fromMonster)
            {
                return false;
            }
            UIController.Instance.Open("OpenLockUI", UITweenType.None, this, unlockId);
            return false;
        }
        
        return true;
    }


    protected override void OnRunPerformance(RunLogicalName logical, RunLogicalFlag flag, IEventCallback sender, string senderArg,
        string[] args)
    {
        if (logical == RunLogicalName.RemoveLock)
        {
            RemoveStation(PropsStation.Locked);
            RunLogicalOnSelf(RunLogicalName.On);
            SetObstacle(false);
        }
        base.OnRunPerformance(logical, flag, sender, senderArg, args);
    }

    /// <summary>
    /// 小游戏解锁成功
    /// </summary>
    public void UnlockSuccess()
    {
        RunLogicalOnSelf(RunLogicalName.RemoveLock);//解锁
        creator.isGet = true;
    }
}