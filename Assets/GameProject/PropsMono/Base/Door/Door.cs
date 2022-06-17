using System;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public abstract class Door : PropsBase
{
    [LabelText("播放开门关门动画的物体")] public GameObject animDoorGo;
    [LabelText("开门的时间")] public float openDoorTime = 3;
    [LabelText("关门的时间")] public float closeDoorTime = 3;
    
    public NavMeshObstacle navObstacle;
    
    // private OcclusionPortal _occlusion;
    // public OcclusionPortal occlusion
    // {
    //     get
    //     {
    //         if (_occlusion == null)
    //         {
    //             GameObject gg = GameObject.Find(creator.sceneObject[0]);
    //             if (gg == null)
    //             {
    //                 gg = GameObject.Find($"Occ/{creator.id}");
    //             }
    //
    //             if (gg == null)
    //             {
    //                 GameDebug.LogError($"{creator.id}遮挡剔除不能为空");
    //             }
    //
    //             _occlusion = gg.GetComponent<OcclusionPortal>();
    //         }
    //
    //         return _occlusion;
    //     }
    // }
    
    public override string tips
    {
        get
        {
            if (forceLock)
            {
                return null;
            }
            else if (singleDirectionOpen)
            {
                if (interactiveStyle == InterActiveStyle.Watch)
                {
                    if (!creator.uiShowTips.IsNullOrEmpty() && isActive)
                    {
                        return Language.GetContent(creator.uiShowTips);
                    }
                }
            }
            else
            {
                return base.tips;
            }
            return null;
        }
    }


    // public override bool mapIsGet
    // {
    //     get { return !ContainStation(PropsStation.Off) && !ContainStation(PropsStation.Locked); }
    // }

    public override int ActiveLayer
    {
        get { return LayerMask.NameToLayer("Door"); }
    }

    protected virtual float timeScale
    {
        get { return TimeHelper.timeScale; }
    }

    public AnimationCurve curve;
    // [LabelText("开门计数")] public int receiveCodeOpenCount = 1;
    protected Tweener openTween;

    protected override bool OnInteractive(bool fromMonster = false)
    {
        // if (singleDirectionOpen && interactiveStyle == InterActiveStyle.Lockdead && !fromMonster)
        // {
        //     // RunLogicalOnSelf(RunLogicalName.flyFont,"808");
        //     return false;
        // }

        //怪这种门打不开！单项门
        if (singleDirectionOpen)
        {
            if (fromMonster || interactiveStyle == InterActiveStyle.Watch)
            {
                return false;
            }
        }
        else
        {
            if (fromMonster || interactiveStyle == InterActiveStyle.Lockdead)
            {
                return false;
            }
        }

        

        return true;
    }

    public override InterActiveStyle interactiveStyle => InterActiveStyle.Handle;
    public override bool canInteractive => !IsAnimating && base.canInteractive; //只要不上锁就可以交互

    [LabelText("是否是单向门")]
    public bool singleDirectionOpen;

    [LabelText("铁链拴着(拴着无交互点)")]
    public bool haveChain;
    
    [ReadOnly,LabelText("强制上锁")]
    public bool forceLock;

    // public override string writeData => receiveCodeOpenCount.ToString();

    // public override void Init(PropsInitLogical logical, PropEntity entity, string customData)
    // {
    //     // if (customData != null)
    //     // {
    //     //     receiveCodeOpenCount = customData.ToInt();
    //     // }
    //
    //     base.Init(logical, entity, customData);
    // }

    protected virtual void Update()
    {
        if (openTween != null && openTween.IsActive())
        {
            openTween.timeScale = timeScale;
        }
    }

    protected override void OnInitLogical(RunLogicalName logical, RunLogicalFlag flag, string senderArg, string[] args)
    {
        base.OnInitLogical(logical, flag, senderArg, args);
        switch (logical)
        {
            case RunLogicalName.On:
                Open(false);
                break;
            case RunLogicalName.Off:
            case RunLogicalName.Lock:
            case RunLogicalName.ForceLock:
                Close(false);
                break;
        }

        if (singleDirectionOpen) //单向门的寻路特殊处理
        {
            if(logical == RunLogicalName.On)
            {
                SetObstacle(false);
            }
            else
            {
                SetObstacle(true);
            }
        }
        else //剩下的其他门的逻辑
        {
            if (logical == RunLogicalName.Lock)
            {
                SetObstacle(true);
            }
            else
            {
                SetObstacle(false);
            }
        }

        //如果一个门没有上锁状态，那么铁链一定解锁了
        if (haveChain)
        {
            if (!ContainStation(PropsStation.Locked))
            {
                haveChain = false;
            }
        }
       
    }

    protected override void OnRunPerformance(RunLogicalName logical, RunLogicalFlag flag, IEventCallback sender,
        string senderArg, string[] args)
    {
        base.OnRunPerformance(logical, flag, sender, senderArg, args);
        
        switch (logical)
        {
            case RunLogicalName.On:
                if (!ContainStation(PropsStation.Locked))
                {
                    Open(true);
                }

                if (singleDirectionOpen)
                {
                    SetObstacle(false);
                }
                break;
            case RunLogicalName.Off:
                Close(true);
                if (singleDirectionOpen)
                {
                    SetObstacle(true);
                }
                break;
            case RunLogicalName.Lock:
            case RunLogicalName.ForceLock:
                Close(true);
                AddStation(PropsStation.Off);
                AddStation(PropsStation.Locked);
                SetObstacle(true);
                break;
        }
    }

    [Button]
    private void Open()
    {
        Open(true);
    }

    [Button]
    private void Close()
    {
        Close(true);
    }

    public AudioSourceProxy moveAudio;

    public virtual void Open(bool withAnimator)
    {
        EventCenter.Dispatch(EventKey.OnOpenCloseRoomPortal , creator.id , true);
    }

    protected virtual void Close(bool withAnimator)
    {
        EventCenter.Dispatch(EventKey.OnOpenCloseRoomPortal , creator.id , false);
    }
    
    
    public override void MatchSuccess()
    {
        RunLogicalOnSelf(RunLogicalName.RemoveLock);
        creator.isGet = true;
    }

    public void SetObstacle(bool enable)
    {
        if (navObstacle != null)
        {
            navObstacle.enabled = enable;    
        }
    }
    
    
    
    [Button]
    public void CopyOccData()
    {
        GameObject go = new GameObject(creator.id.ToString());
        go.transform.position = navObstacle.transform.position;
        go.transform.rotation = navObstacle.transform.rotation;
        OcclusionPortal portal = go.AddOrGetComponent<OcclusionPortal>();
    }
}