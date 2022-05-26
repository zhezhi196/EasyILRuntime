using System;
using System.Collections;
using DG.Tweening;
using Module;
using UnityEngine;

/// <summary>
/// 闸门
/// </summary>
public class GateProp : DoorTranslation
{
    
    public override InterActiveStyle interactiveStyle => InterActiveStyle.Watch;
    
    private Coroutine _co;
    

    public override void RunLogical(RunLogicalName logical, IEventCallback sender, RunLogicalFlag flag, string senderArg, params string[] args)
    {
        switch (logical)
        {
            case RunLogicalName.On:
                RemoveStation(PropsStation.Off);
                SetObstacle(false);
                break;
            case RunLogicalName.Off:
                AddStation(PropsStation.Off);
                SetObstacle(true);
                break;
            case RunLogicalName.Destroy:
                AddStation(PropsStation.Destroyed);
                break;
        }

        if (_co != null)
        {
            StopCoroutine(_co);
            _co = null;
        }

        _co = StartCoroutine(WaitUntilAnimOverPerformance<SwitchGateProp>(sender, () =>
        {
            OnPerfomance(logical, sender, flag, senderArg);    
        }));

    }

    protected override bool OnInteractive(bool fromMonster = false)
    {
        return false;
    }
}