using System;
using System.Collections;
using DG.Tweening;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[Flags]
public enum PropsStation
{
    [LabelText("锁定")] Locked = 1,
    [LabelText("关闭")] Off = 2,
    [LabelText("隐藏")] Hide = 4,
    [LabelText("非激活")] UnActive = 8,
    [LabelText("已经销毁")]Destroyed = 16,
}

public abstract class PropsBase : MonoBehaviour, IStationObject<PropsStation>,  IPoolObject,InteractiveObject,IProgressObject,IMatch,IBilingObject
{
    [ReadOnly,LabelText("当前已经交互次数")]
    public int interactiveCount;
    [LabelText("提示点"),SerializeField]
    protected Transform[] tipsPoint;

    public static event Action<bool> OnUnInteractiveProp;
    private static float unInteractiveTime;
    public static bool isCountTime = true;
    
    /// <summary>
    /// 是否正在播放动画
    /// </summary>
    [LabelText("是否正在播放动画"),ReadOnly]
    public bool IsAnimating = false;

    public static void UpdateTime()
    {
        if (isCountTime)
        {
            unInteractiveTime += TimeHelper.deltaTime;
            if (unInteractiveTime >= 180)
            {
                OnUnInteractiveProp?.Invoke(true);
                isCountTime = false;
            }
        }
    }
        
    public static void InteractiveKeyObject()
    {
        unInteractiveTime = 0;
        OnUnInteractiveProp?.Invoke(false);
        isCountTime = true;
    }
    public virtual int ActiveLayer
    {
        get { return 15; }
    }

    public Transform progressPoint;
    [SerializeField]
    private PropsStation _station;
    [ReadOnly]
    public bool isInit;

    public bool isActive
    {
        get { return isInit && !ContainStation(PropsStation.UnActive) && hasEnoughInteractiveCount; }
    }

    public virtual Transform progressTipsPoint
    {
        get
        {
            if (!tipsPoint.IsNullOrEmpty())
            {
                return tipsPoint[0];
            }

            if (!lookPoints.IsNullOrEmpty())
            {
                return lookPoints[0].transform;
            }
            
            return transform;
        }
    }

    [ReadOnly]
    public PropsCreator creator;
    public PropEntity entity;

    [LabelText("交互点"),ReadOnly]
    public LookPoint[] lookPoints;

    public virtual bool mapIsGet
    {
        get { return true; }
    }

    public PropsStation station => _station;
    public Match[] matchObject { get; set; }
    private int localInitCount
    {
        get
        {
            string kk = creator.id + "LocalInit";
            if (!LocalFileMgr.ContainKey(kk)) return 0;
            return LocalFileMgr.GetInt(kk);
        }

        set { LocalFileMgr.SetInt(creator.id + "LocalInit", value); }
    }

    public ObjectPool pool { get; set; }

    public virtual int count
    {
        get { return creator.count; }
    }
    
    public virtual bool showBiling => isInit;

    public event Action onSwitchStation;

    public virtual void Init(PropsInitLogical logical, PropEntity entity, string customData)
    {
        if (creator == null || creator.initLocalCount == -1 || localInitCount < creator.initLocalCount)
        {
            if (creator != null && creator.initLocalCount > 0) localInitCount++;
            this.entity = entity;
            creator.InitEvent();
            creator.RunLogical(logical.runLogical, creator, RunLogicalFlag.None, null, logical.args);
            this._station = logical.station;
            if (logical.runLogical != RunLogicalName.Destroy)
            {
                if (!creator.matchInfo.IsNullOrEmpty())
                {
                    matchObject = new Match[creator.matchInfo.Length];
                    for (int i = 0; i < matchObject.Length; i++)
                    {
                        matchObject[i] = Match.GetMatch(creator.matchInfo[i], "Battle");
                        matchObject[i].TryAddToStore(this);
                    }
                }

                lookPoints = transform.GetComponentsInChildren<LookPoint>(true);
                if (!lookPoints.IsNullOrEmpty())
                {
                    for (int i = 0; i < lookPoints.Length; i++)
                    {
                        lookPoints[i].Init(this);
                    }
                }
            }
            isInit = true;
        }
    }

    public virtual bool canInteractive
    {
        get
        {
            return isActive && hasEnoughInteractiveCount;
        }
    }

    public virtual InterActiveStyle interactiveStyle
    {
        get
        {
            if (!canInteractive) return InterActiveStyle.None;
            if (entity.dbData.putBag == 0) 
                return InterActiveStyle.Watch;
            return InterActiveStyle.Handle;
        }
    }

    public virtual bool isButtonActive
    {
        get { return canInteractive; }
    }

    public virtual string tips
    {
        get
        {
            if (!creator.uiShowTips.IsNullOrEmpty() && isActive)
            {
                return Language.GetContent(creator.uiShowTips);
            }

            return null;
        }
    }

    public virtual string interactiveTips
    {
        get
        {
            if (!creator.interactiveTips.IsNullOrEmpty() && isActive)
            {
                return Language.GetContent(creator.interactiveTips);
            }

            return null;
        }
    }

    public bool hasEnoughInteractiveCount
    {
        get { return creator.remainInteractiveCount == -1 || interactiveCount < creator.remainInteractiveCount; }
    }

    public virtual string writeData { get; }
    
    [LabelText("是否找父物体")]
    public bool findParent;
    [ShowIf("findParent"),LabelText("父物体id")]
    public int parentId;

    [LabelText("跟随点")]
    public Transform followPoint;
    
    public bool SwitchStation(PropsStation station)
    {
        if (this.station != station)
        {
            this._station = station;
            onSwitchStation?.Invoke();
            return true;
        }

        return false;
    }

    public bool ContainStation(PropsStation station)
    {
        return (this._station & station) == station;
    }

    public bool AddStation(PropsStation station)
    {
        if (ContainStation(station)) return false;
        
        BattleController.Instance.TryNextNode(PredicateType.AddStation, creator.id.ToString(), station.ToString());

        this._station = this.station | station;
        OnAddStation(station);
        creator.TrySendEvent(SendEventCondition.AddStation, station.ToString());
        onSwitchStation?.Invoke();
        return true;
    }

    public bool RemoveStation(PropsStation station)
    {
        if (!ContainStation(station)) return false;

        BattleController.Instance.TryNextNode(PredicateType.RemoveStation, creator.id.ToString(), station.ToString());

        this._station = this.station & ~station;
        OnRemoveStation(station);
        creator.TrySendEvent(SendEventCondition.RemoveStation, station.ToString());
        onSwitchStation?.Invoke();
        return true;
    }

    protected virtual void OnAddStation(PropsStation station)
    {
    }

    protected virtual void OnRemoveStation(PropsStation station)
    {
    }

    public bool Interactive(bool fromMonster = false)
    {
        if (BattleController.Instance.ctrlProcedure.isResult) return false;
        if (hasEnoughInteractiveCount)
        {
            if ((entity == null || entity.OnInteractive()) && OnInteractive(fromMonster))
            {
                creator.isGet = true;
                AnalyticsEvent.SendEvent(AnalyticsType.InterProps, creator.id.ToString());
                creator.TrySendEvent(SendEventCondition.Interactive);
                BattleController.Instance.TryNextNode(PredicateType.Intaractive, creator.id.ToString());
                if (interactiveCount < creator.remainInteractiveCount)
                {
                    interactiveCount++;
                }

                return true;
            }
        }

        return false;
    }

    protected void DestroyWhileUnActive()
    {
        if (!isActive)
        {
            RunLogicalOnSelf(RunLogicalName.Destroy);
        }
    }

    protected virtual bool OnInteractive(bool fromMonster = false)
    {
        return true;
    }

    public virtual void RunLogical(RunLogicalName logical, IEventCallback sender, RunLogicalFlag flag, string senderArg,params string[] args)
    {
        switch (logical)
        {
            case RunLogicalName.On:
                if (!ContainStation(PropsStation.Locked)) //锁着的物体不能直接打开
                {
                    RemoveStation(PropsStation.Off);
                }
                break;
            case RunLogicalName.Off:
                AddStation(PropsStation.Off);
                break;
            case RunLogicalName.Lock:
                AddStation(PropsStation.Locked);
                AddStation(PropsStation.Off);
                break;
            case RunLogicalName.RemoveLock:
                RemoveStation(PropsStation.Locked);
                break;
            case RunLogicalName.Hide:
                AddStation(PropsStation.Hide);
                break;
            case RunLogicalName.Show:
                RemoveStation(PropsStation.Hide);
                break;
            case RunLogicalName.UnActive:
                AddStation(PropsStation.UnActive);
                break;
            case RunLogicalName.RemoveUnActive:
                RemoveStation(PropsStation.UnActive);
                break;
            case RunLogicalName.Destroy:
            case RunLogicalName.ForceDestroy:    
                AddStation(PropsStation.Destroyed);
                break;
        }

        OnPerfomance(logical,sender,flag,senderArg,args);
    }

    protected virtual IEnumerator WaitUntilAnimOverPerformance<T>(IEventCallback sender , Action callback) where T : PropsBase
    {
        if (sender == this.creator)
        {
            callback?.Invoke();
            yield break;
        }
        
        var originType = ((PropsCreator) sender).props;
        if (!(originType is T))
        {
            yield break;
        }
        var prop = originType as T;
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(()=>!prop.IsAnimating);
        callback?.Invoke();
    }
    

    protected void OnPerfomance(RunLogicalName logical, IEventCallback sender, RunLogicalFlag flag, string senderArg,params string[] args)
    {
        if ((flag & RunLogicalFlag.WithPerformance) != 0) //如果需要表现的话
        {
            OnRunPerformance(logical, flag, sender, senderArg, args);
        }
        else
        {
            OnInitLogical(logical, flag, senderArg, args);
        }
    }
    
    
    protected virtual void OnInitLogical(RunLogicalName logical,RunLogicalFlag flag,string senderArg, string[] args)
    {
        //初始化的时候设置父物体跟随
        if (findParent)
        {
            var find = PropsCreator.editorList.Find(v=>v.id == parentId);
            if (find != null)
            {
                transform.SetParent(find.props.followPoint,false);
                transform.localPosition = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// 事件的场景表现逻辑
    /// </summary>
    protected virtual void OnRunPerformance(RunLogicalName logical, RunLogicalFlag flag, IEventCallback sender,string senderArg, string[] args)
    {
        switch (logical)
        {
            case RunLogicalName.Destroy:
                DestroyThis(false);
                break;
            case RunLogicalName.ForceDestroy:
                DestroyThis(true);
                break;
            case RunLogicalName.Hide:                
                gameObject.OnActive(false);
                break;
            case RunLogicalName.Show:                
                gameObject.OnActive(true);
                break;
        }
    }

    protected virtual void OnDestroy()
    {
        if (!matchObject.IsNullOrEmpty())
        {
            for (int i = 0; i < matchObject.Length; i++)
            {
                matchObject[i].RemoveMatch(this);
            }
        }
        
        AudioPlay.StopAudio(st => st.ID == gameObject);
        DOTween.Kill(gameObject);
    }

    public void ReturnToPool()
    {
    }

    public void OnGetObjectFromPool()
    {
    }

    public virtual void DestroyThis(bool force)
    {
        this.isInit = false;
        
        if(force)
        {
            AssetLoad.Destroy(gameObject);
            return;
        }
        
        if (!creator.unloadUnDestroy)
        {
            gameObject.OnActive(false);
        }
    }

    public Vector3 progressPos
    {
        get { return progressPoint == null ? transform.position : progressPoint.position; }
    }

    public void OnMatchSuccess(IMatch[] target)
    {
        
    }

    public virtual void MatchSuccess()
    {
    }

    public bool CanMatch(IMatch target)
    {
        if (matchObject.IsNullOrEmpty()) return false;
        for (int i = 0; i < matchObject.Length; i++)
        {
            if (matchObject[i].CanMatch(target))
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// 自身运行部分逻辑（不影响其他prop）
    /// </summary>
    public void RunLogicalOnSelf(RunLogicalName logical, params string[] args)
    {
        creator.RunLogical(logical, creator, RunLogicalFlag.WithPerformance | RunLogicalFlag.Save, null, args);
    }
    
    
    public virtual bool isProgressShow => isInit;
#if UNITY_EDITOR
    [SerializeField,LabelText("传送点(Editor用)"),FoldoutGroup("Editor传送测试")]
    protected Transform flashPoint;


    [Button("传送至玩家"),FoldoutGroup("Editor传送测试")]
    private void FlashToPlayer()
    {
        transform.position = Player.player.chasePoint;
    }
    [Button("玩家到位置"),FoldoutGroup("Editor传送测试")]
    private void PlayerToProps()
    {
        Player.player.characterController.enabled = false;
        Player.player.transform.position = flashPoint != null ? flashPoint.transform.position : transform.position;
        Player.player.characterController.enabled = true;
    }
#endif
}