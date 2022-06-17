using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;

/// <summary>
/// 补给箱
/// </summary>
public class SupplyBoxProp : PropsBase
{
    public GameObject closedObj;
    public GameObject openedObj;

    public override InterActiveStyle interactiveStyle => InterActiveStyle.Handle;
    public override bool canInteractive => base.canInteractive && ContainStation(PropsStation.Off);
    public MoneyType type;
    public override bool showBiling => canInteractive;
    
    public override void Init(PropsInitLogical logical, PropEntity entity, string data)
    {
        Group group = Group.GetGroup(creator.@group, creator);
        group.Random();
        if (group != null && group.randomObject == creator)
        {
            base.Init(logical, entity, data);
        }
        else
        {
            RunLogicalOnSelf(RunLogicalName.ForceDestroy);
        }
    }
    
    protected override bool OnInteractive(bool fromMonster = false)
    {
        UIController.Instance.Open("SupplyBox", UITweenType.None, this); 

        // AudioPlay.PlayOneShot("baozang_open").SetIgnorePause(true);
        // AnalyticsEvent.SendEvent(AnalyticsType.GetPropBox, entity.dbData.ID.ToString(),false);
        return true;
    }
    
    
    protected override void OnInitLogical(RunLogicalName logical, RunLogicalFlag flag, string senderArg, string[] args)
    {
        base.OnInitLogical(logical, flag, senderArg, args);

        if (logical == RunLogicalName.On)
        {
            Open();
        }
    }

    protected override void OnRunPerformance(RunLogicalName logical, RunLogicalFlag flag, IEventCallback sender, string senderArg,
        string[] args)
    {
        base.OnRunPerformance(logical, flag, sender, senderArg, args);
        
        if (logical == RunLogicalName.On)
        {
            Open();
        }
    }
    
    public void Open()
    {
        openedObj.OnActive(true);
        closedObj.OnActive(false);
    }

    public void OnOpen()
    {
        RunLogicalOnSelf(RunLogicalName.On);
    }
}
