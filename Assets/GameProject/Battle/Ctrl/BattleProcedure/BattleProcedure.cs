using System.Collections.Generic;
using System.Diagnostics;
using Module;

public abstract class BattleProcedure : IBattleProcedure
{
    public Mission mission;
    public abstract PlayMode mode { get; }
    public bool isResult;
    public RunTimeSequence finishAction;
    public TaskNode currNode;
    public List<BattleSystem> battleCtrl = new List<BattleSystem>();

    public BattleProcedure(Mission mission)
    {
        this.mission = mission;
        this.finishAction = new RunTimeSequence();
    }
    
    public void BattlePrepare()
    {
        RunMethod("BattlePrepare", "战斗准备");
    }

    public void StartBattle(Mission mission, MissionGraph editorGraph)
    {
        editorGraph.LoadEditorData();
        RunMethod("StartBattle", "开始战斗", mission, editorGraph);
        RestartBattle(editorGraph);
    }

    public void RestartBattle(MissionGraph editorGraph)
    {
        isResult = false;
        finishAction.Clear();
        finishAction.OnComplete(Fight);
        RunMethod("RestartBattle", "战斗准备", editorGraph);
        currNode = editorGraph.FirstNode();
        OnNodeEnter(currNode);
    }

    public void OnNodeEnter(TaskNode node)
    {
        if (!isResult)
        {
            RunMethod("OnNodeEnter", "进入节点: " + node.name, node);
            SortAction();
            finishAction.NextAction();
        }
    }

    public void OnNodeExit(TaskNode node)
    {
        RunMethod("OnNodeExit", "退出节点: " + node.name, node);
    }

    public void Fight()
    {
        Loading.Close(UILoading.uiLoading, "EnterBattle");
        RunMethod("Fight", "战斗开始");
    }

    public void OnTaskResult(bool result)
    {
        if (!isResult)
        {
            isResult = true;
            if (result)
            {
                mission.Complete();
                RunMethod("OnTaskResult", "出现结局" + result, result);
                SortResultAction(result);
                finishAction.OnComplete(() =>
                {
                    BattleController.Instance.ExitBattle(result ? OutGameStation.Win : OutGameStation.Fail);
                });
                finishAction.NextAction();
            }
        }
    }

    public void ExitBattle(OutGameStation outGame)
    {
        RunMethod("ExitBattle", "ExitBattle", outGame);
        battleCtrl.Clear();
    }

    public void OnUpdate()
    {
        for (int i = 0; i < battleCtrl.Count; i++)
        {
            battleCtrl[i].OnUpdate();
        }
    }

    protected abstract void SortAction();
    protected abstract void SortResultAction(bool result);

    public void AddAction(RunTimeAction action)
    {
        if (action != null)
        {
            finishAction.Add(action);
        }
    }
    
    public void RunMethod(string methodName, string log, params object[] args)
    {
        DebugLog(log);
        for (int i = 0; i < battleCtrl.Count; i++)
        {
            BattleSystem temp = battleCtrl[i];
            temp.GetType().GetMethod(methodName).Invoke(temp, args);
        }
    }
    
    [Conditional("LOG_ENABLE")]
    private void DebugLog(string msg)
    {
        BattleController.Instance.DebugLog("Procedure", msg);
    }

    public T GetCtrl<T>()
    {
        for (int i = 0; i < battleCtrl.Count; i++)
        {
            if (battleCtrl[i] is T resu)
            {
                return resu;
            }
        }

        return default;
    }


    public void TryNextNode(PredicateType predicate, string[] args)
    {
        if (isResult) return;
        if (currNode != null)
        {
            //if (currNode.IsFail(predicate, args))
            if (false)
            {
                OnTaskResult(false);
                return;
            }

            SkipNode(predicate, args);
        }
    }
    
    public void SkipNode(PredicateType predicate, params string[] args)
    {
        var node = currNode.TryGetNextNode(predicate, args);
        if (node != null)
        {
            OnNodeExit(currNode);
            if (node is TaskEnd)
            {
                OnTaskResult(true);
            }
            else if (node is TaskNode task)
            {
                task.RunningNode();
                currNode = task;
                OnNodeEnter(currNode);
            }
            else
            {
                GameDebug.LogError($"node出错: {node}");
            }
        }
    }

}