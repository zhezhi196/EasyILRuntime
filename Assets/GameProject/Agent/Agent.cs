using System;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Agent : MonoBehaviour, IAgentObject
{
    protected List<IAgentCtrl> ctrlList = new List<IAgentCtrl>();

    [SerializeField, TabGroup("其他")] private bool _isLog;
    [SerializeField, TabGroup("其他")] protected InitStation _initStation;
    [SerializeField, TabGroup("状态")] protected FloatField _hp;
    [TabGroup("信息"), LabelText("阵营")] public Camp camp;
    [TabGroup("信息"),LabelText("是否可以成为目标")] public bool canBeTarget;
    [TabGroup("信息"),SerializeField] private bool _isVisiable;


    [TabGroup("信息"), LabelText("战斗状态")] public FightState fightState;
    [TabGroup("信息"), LabelText("移动样式")] public MoveStyle moveStyle;

    public virtual AgentType type => AgentType.Human;
    public float hp => _hp;
    public abstract string logName { get; }

    public Vector3 terrainNormal => transform.up;
    public bool isLog
    {
        get
        {
            return _isLog;
        }
        set
        {
            _isLog = value;
            ILogObject[] log = transform.GetComponentsInChildren<ILogObject>(true);
            for (int i = 0; i < log.Length; i++)
            {
                if (log[i] != this)
                {
                    log[i].isLog = value;
                }
            }
        }
    }
    [Button]
    public void SetLog()
    {
        isLog = !isLog;
    }
    
    public InitStation initStation => _initStation;
    public bool isAlive => initStation == InitStation.Normal;

    public float timeScale => TimeHelper.timeScale;

    public void LogFormat(string obj, params object[] args)
    {
        if (isLog)
        {
            GameDebug.LogFormat(obj, args);
        }
    }
    



    public float GetUnscaleDelatime(bool ignorePause)
    {
        return ignorePause ? TimeHelper.unscaledDeltaTimeIgnorePause : TimeHelper.unscaledDeltaTime;
    }

    public bool IsEnemy(Agent target)
    {
        return canBeTarget && target.camp != camp;
    }

    public float GetDelatime(bool ignorePause)
    {
        return ignorePause ? TimeHelper.deltaTimeIgnorePause : TimeHelper.deltaTime;
    }

    public void SetTimescale(float timeScale)
    {
        TimeHelper.timeScale = timeScale;
    }

    public Tweener SetTimescale(float timeScale, float time)
    {
        return TimeHelper.SetTimeScale(timeScale, time);
    }
    public virtual T GetAgentCtrl<T>() where T: IAgentCtrl
    {
        for (int i = 0; i < ctrlList.Count; i++)
        {
            if (ctrlList[i] is T t)
            {
                return t;
            }
        }
        return default;
    }

    protected virtual void AddCtrl()
    {
        
    }
    protected virtual void Update()
    {
        for (int i = 0; i < ctrlList.Count; i++)
        {
            ctrlList[i].OnUpdate();
        }
        
    }
    [Button]
    private void EditorInit()
    {
        AddCtrl();
        for (int i = 0; i < ctrlList.Count; i++)
        {
            ctrlList[i].EditorInit();
        }
    }

    public event Action onSwitchStation;

    public void OnSwitchStation()
    {
        onSwitchStation?.Invoke();
    }

    private void OnBecameInvisible()
    {
        _isVisiable = false;
    }

    private void OnBecameVisible()
    {
        _isVisiable = true;
    }

    public virtual bool isVisiable
    {
        get { return _isVisiable; }
    }

    private void OnDrawGizmos()
    {
        if (isLog)
        {
            for (int i = 0; i < ctrlList.Count; i++)
            {
                if (ctrlList[i] is NavMoveCtrl navMove)
                {
                    navMove.OnDrawGizmos();
                }
            }
        }
    }
}