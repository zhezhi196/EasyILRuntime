using System;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class DoorLocked : Door
{
    [LabelText("是否需要钥匙")]
    public bool needKey = false;
    [LabelText("解锁后是否可以继续开关"), ShowIf("needKey")]
    public bool unlockedCanInteractive = false;
    
    
    
    //门交互点逻辑：通电门小游戏、闸门、单向门背面是放大镜；任何方式打不开的门都是锁头；能用钥匙打开的、开锁小游戏都是手
    public override InterActiveStyle interactiveStyle
    {
        get
        {
            if (singleDirectionOpen) //处理单向门逻辑
            {
                if (ContainStation(PropsStation.Locked) || ContainStation(PropsStation.Off))
                {
                    return lookPoints[0].isLookAtPlayer ? InterActiveStyle.Handle : InterActiveStyle.Watch;
                }
                else
                {
                    return InterActiveStyle.Handle; //已经打开的就是手
                }
            }
            else
            {
                if (needKey)
                {
                    return InterActiveStyle.Handle;
                }
                else
                {
                    if (ContainStation(PropsStation.Locked))
                    {
                        return InterActiveStyle.Lockdead;
                    }
                    else
                    {
                        return InterActiveStyle.Handle;
                    }
                }
            }
        }
    }

    public override bool canInteractive
    {
        get
        {
            if (haveChain)
            {
                return false;
            }
            if (needKey)
            {
                if (ContainStation(PropsStation.Locked)) //锁着的时候可以交互
                {
                    return !IsAnimating && isActive;
                }
                else if(unlockedCanInteractive) //解锁了如果可以交互，则交互
                {
                    return !IsAnimating && isActive;
                }
                else
                {
                    return false; //反之不能交互了
                }
            }
            else
            {
                return !IsAnimating && isActive; //门任何时候都可以交互了！5.19继森如是说
            }
        }
    }
    
    protected override bool OnInteractive(bool fromMonster = false)
    {
        if (!base.OnInteractive(fromMonster))
        {
            return false;
        }
        
        if (needKey)
        {
            if (ContainStation(PropsStation.Locked))
            {
                if (fromMonster)
                {
                    return false;
                }
                UIController.Instance.Open("BagUI", UITweenType.None, this);
                return false;
            }
            else if(unlockedCanInteractive)
            {
                if(ContainStation(PropsStation.Off))
                {
                    RunLogicalOnSelf(RunLogicalName.On);
                }
                else
                {
                    RunLogicalOnSelf(RunLogicalName.Off);
                }
            } 
        }
        else
        {
            if (ContainStation(PropsStation.Locked)) //不需要钥匙，但是上锁的门，这种门交互无效
            {
                return false;
            }
            else if(ContainStation(PropsStation.Off))
            {
                RunLogicalOnSelf(RunLogicalName.On);
            }
            else
            {
                RunLogicalOnSelf(RunLogicalName.Off);
            }
        }
        
        return true;
    }
    
    
    public override void MatchSuccess()
    {
        RunLogicalOnSelf(RunLogicalName.RemoveLock);
        creator.isGet = true;
    }
    
    protected override void OnInitLogical(RunLogicalName logical, RunLogicalFlag flag, string senderArg, string[] args)
    {
        if (logical == RunLogicalName.RemoveLock)
        {
            Open(false);
        }
        base.OnInitLogical(logical, flag, senderArg, args);
    }
    
    
    protected override void OnRunPerformance(RunLogicalName logical, RunLogicalFlag flag, IEventCallback sender, string senderArg, string[] args)
    {
        if (logical == RunLogicalName.RemoveLock)
        {
            RemoveStation(PropsStation.Off);
            RemoveStation(PropsStation.Locked);
            Open(true);
            SetObstacle(false);
            haveChain = false;
        }

        base.OnRunPerformance(logical, flag, sender, senderArg, args);
    }
   
}