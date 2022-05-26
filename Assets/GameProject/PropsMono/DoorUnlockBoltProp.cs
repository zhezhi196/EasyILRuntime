using System;
using DG.Tweening;
using Module;
using UnityEngine;

/// <summary>
/// 未上锁的门,带单侧插销的
/// </summary>
public class DoorUnlockBoltProp : DoorRotate
{
    public GameObject bolt;
    public float boltMoveDistance = 0.087f;
    
    private bool boltLock = true;
    public override string writeData => boltLock.ToString();


    public override void Init(PropsInitLogical logical, PropEntity entity, string customData)
    {
        if (!string.IsNullOrEmpty(customData))
        {
            boltLock = bool.Parse(customData);    
        }
        
        base.Init(logical, entity, customData);
    }

    protected override bool OnInteractive(bool fromMonster = false)
    {
        if (fromMonster)
        {
            if (boltLock)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        return base.OnInteractive(fromMonster);
    }

    protected override void OnRunPerformance(RunLogicalName logical, RunLogicalFlag flag, IEventCallback sender, string senderArg, string[] args)
    {
        if (logical == RunLogicalName.On)
        {
            if (boltLock)
            {
                boltLock = false;
                UnlockBolt(() =>
                {
                    OpenPrivate();
                });
            }
            else
            {
                OpenPrivate();
            }
        }
        else if (logical == RunLogicalName.Off)
        {
            Close(true);
            if (singleDirectionOpen)
            {
                SetObstacle(true);
            }
        }
        else if (logical == RunLogicalName.RemoveLock)
        {
            AddStation(PropsStation.Off);
            RemoveStation(PropsStation.Locked);
            SetObstacle(false);
            haveChain = false;
        }
        else
        {
            base.OnRunPerformance(logical, flag, sender, senderArg, args);
        }

    }

    private void OpenPrivate()
    {
        IsAnimating = false;
        Open(true);
        SetObstacle(false);
    }

    protected override void OnInitLogical(RunLogicalName logical, RunLogicalFlag flag, string senderArg, string[] args)
    {
        base.OnInitLogical(logical, flag, senderArg, args);
        if (boltLock)
        {
            for (int i = 0; i < lookPoints.Length; i++)
            {
                lookPoints[i].forwardShow = true;
            }
            bolt.transform.localPosition = Vector3.zero;
            SetObstacle(true);
        }
        else
        {
            for (int i = 0; i < lookPoints.Length; i++)
            {
                lookPoints[i].forwardShow = false;
            }
            bolt.transform.localPosition = new Vector3(boltMoveDistance,0,0);
            SetObstacle(false);
        }
    }

    private void UnlockBolt(Action callBack)
    {
        if (bolt == null)
        {
            callBack?.Invoke();
            return;
        }
        
        
        for (int i = 0; i < lookPoints.Length; i++)
        {
            lookPoints[i].forwardShow = false;
        }

        IsAnimating = true;
        bolt.transform.DOLocalMoveX(boltMoveDistance,1).OnComplete(() =>
        {
            callBack?.Invoke();
        });
    }
}