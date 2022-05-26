using System;
using System.Collections.Generic;
using System.Diagnostics;
using Module;

public abstract class BattleProcedure: IBattleEvent
{
    public ServiceType service;
    public abstract GameMode mode { get; }
    public List<IBattleEvent> battleCtrl { get; }
    public List<string> pauseList { get; }
    public Mission mission { get; }
    public bool isResult { get; set; }
    public RunTimeSequence finishAction { get; }
    public TaskNode currentNode { get; set; }
    public EnterNodeType enterType { get; set; }
    private Action callback;
    public float gameTime;
    public bool isStartFight;
    public bool isPause;

    public MissionGraph editorData
    {
        get { return mission.editorData; }
    }

    public BattleProcedure(Mission mission,Action callback)
    {
        this.mission = mission;
        battleCtrl = new List<IBattleEvent>();
        pauseList = new List<string>();
        //LoadScene = new List<SceneName>();
        finishAction = new RunTimeSequence();
        finishAction.autoDestroy = true;
        this.callback = callback;
    }
    
    [Conditional("LOG_ENABLE")]
    private void DebugLog(string msg)
    {
        BattleController.Instance.DebugLog("Node Message",msg);
    }
    
    public void RunMethod(string methodName, string log, params object[] args)
    {
        DebugLog(log);
        for (int i = 0; i < battleCtrl.Count; i++)
        {
            IBattleEvent temp = battleCtrl[i];
            temp.GetType().GetMethod(methodName).Invoke(temp, args);
        }
    }
    
    public void AppendCtrl(IBattleEvent systemCtrl)
    {
        battleCtrl.Add(systemCtrl);
    }

    public void BattlePrepare(EnterNodeType enterType)
    {
        this.enterType = enterType;
        RunMethod("BattlePrepare", "战斗准备: " + enterType, enterType);
    }

    public void StartBattle(EnterNodeType enterType)
    {
        AnalyticsEvent.SendEvent(AnalyticsType.GameStart, null);
        EventCenter.Dispatch(EventKey.EnterBattle);
        CommonPopup.onPopup = b =>
        {
            if (b)
            {
                BattleController.Instance.Pause("CommonPopup");
            }
            else
            {
                BattleController.Instance.Continue("CommonPopup");
            }
        };
        RunMethod("StartBattle", "战斗开始" + enterType, enterType);
        OnRestart(enterType);
    }

    public void OnRestart(EnterNodeType enterType)
    {
        isResult = false;
        if (enterType == EnterNodeType.Restart)
        {
            gameTime = 0;    
        }
        else
        {
            string timeStr = LocalSaveFile.GetString(LocalSave.savePath, "GameTime");
            gameTime = timeStr.IsNullOrEmpty() ? 0f : float.Parse(timeStr);
        }
        
        finishAction.Clear();
        finishAction.OnComplete(() => OnStartFight(currentNode));
        currentNode = editorData.GetFirstNode();
        RunMethod("OnRestart", "OnRestart: " + enterType, enterType);
        OnNodeEnter(currentNode, enterType);
    }

    public void OnNodeExit(NodeBase node)
    {
        RunMethod("OnNodeExit", "退出节点: " + node.name, node);
    }

    public void OnNodeEnter(NodeBase node, EnterNodeType enterType)
    {
        if (isResult) return;
        //editorData.LoadScene(currentNode, enterType, finishAction);
        AnalyticsEvent.SendEvent(AnalyticsType.StartNode, node.name);
        EventCenter.Dispatch(EventKey.OnNodeEnter, node);
        RunMethod("OnNodeEnter", "进入节点: " + node.name, node, enterType);
        SortAction();
        if (enterType == EnterNodeType.NextNode || enterType == EnterNodeType.SkipNode)
        {
            finishAction.OnComplete(() =>
            {
                BattleController.Instance.Save(0);
            });
        }
        finishAction.NextAction();

    }

    public void AddAction(RunTimeAction action)
    {
        if (action != null)
        {
            finishAction.Add(action);
        }
    }

    public void OnStartFight(NodeBase node)
    {
        RunMethod("OnStartFight", "开始战斗,开始节点" + node.name, node);
        callback?.Invoke();
        isStartFight = true;
    }

    public void OnPlayerDead()
    {
        RunMethod("OnPlayerDead", "玩家死亡: ");
    }

    public void OnTaskResult(bool result)
    {
        if (!isResult)
        {
            // AudioPlay.StopPlayBGM();
            AudioManager.StopMusic();
            isResult = true;
            if (result)
            {
                mission.Complete(gameTime);
            }

            RunMethod("OnTaskResult", "出现结局" + result, result);
            SortResultAction(mission, result);
            finishAction.OnComplete(() =>
            {
                BattleController.Instance.ExitBattle(result ? OutGameStation.Win : OutGameStation.Fail);
            });
            AnalyticsEvent.SendEvent(AnalyticsType.GameEnd, null);
            finishAction.NextAction();
        }
    }

    protected abstract void SortResultAction(Mission mission,bool result);

    public void OnContinue()
    {
        isPause = false;
        RunMethod("OnContinue", "OnContinue");
    }

    public void OnPause()
    {
        isPause = true;
        EventCenter.Dispatch(EventKey.OnPause);
        //AudioPlay.PauseByType(AudioPlayType.Audio);

        RunMethod("OnPause", "OnPause");
    }

    public void ExitBattle(OutGameStation station)
    {

        RunMethod("ExitBattle", "ExitBattle", station);
        battleCtrl.Clear();
    }

    public void Save()
    {
        for (int i = 0; i < mission.editorData.nodes.Count; i++)
        {
            if (mission.editorData.nodes[i] is TaskNode task)
            {
                LocalSave.Write(task);
            }
        }

        LocalSaveFile.SetString(LocalSave.savePath, new LocalSaveInfo() {@group = "MissionGroup", key = "BattleMission", value = mission.dbData.ID.ToString()});
        LocalSaveFile.SetString(LocalSave.savePath, new LocalSaveInfo() {@group = "MissionGroup", key = "GameTime", value = gameTime.ToString()});
        RunMethod("Save", "Save");
    }

    public void OnUpdate()
    {
        if (!isResult && !isPause)
        {
            gameTime += TimeHelper.deltaTime;
        }
        for (int i = 0; i < battleCtrl.Count; i++)
        {
            battleCtrl[i].OnUpdate();
        }
    }

    public void SkipNode(PredicateType predicate, EnterNodeType nodeType,params string[] args)
    {
        var node = currentNode.TryGetNextNode(predicate, args);
        if (node != null)
        {
            OnNodeExit(currentNode);
            if (node is TaskEnd)
            {
                OnTaskResult(true);
            }
            else if (node is TaskNode task)
            {
                task.RunningNode();
                currentNode = task;
                OnNodeEnter(currentNode, nodeType);
            }
            else
            {
                GameDebug.LogError($"node出错: {node}");
            }
        }
    }

    public void TryNextNode(PredicateType predicate, string[] args)
    {
        if (isResult) return;
        if (currentNode != null)
        {
            if (currentNode.IsFail(predicate, args))
            {
                OnTaskResult(false);
                return;
            }

            SkipNode(predicate, EnterNodeType.NextNode, args);
        }
    }
    
    public bool ContainService(ServiceType type)
    {
        return (service & type) != 0;
    }

    public void AddGlobalService(ServiceType type)
    {
        service = service | type;
    }

    public void RemoveGlobalService(ServiceType type)
    {
        service = service & ~type;
    }

    /// <summary>
    /// 获取按照流程应该对应的存储状态
    /// </summary>
    /// <param name="unloadId"></param>
    /// <param name="loadId"></param>
    /// <param name="initID"></param>
    /// <returns></returns>
    public virtual SaveStation GetSaveStation(int unloadId, int loadId, int initID)
    {
        return SaveStation.None;
    }

    protected abstract void SortAction();
}