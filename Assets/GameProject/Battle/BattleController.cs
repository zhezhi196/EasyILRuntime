using Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : Singleton<BattleController>
{
    public static T GetCtrl<T>()
    {
        if (Instance.procedure != null)
        {
            return Instance.procedure.GetCtrl<T>();
        }

        return default;
    }
    
    public List<string> pauseList = new List<string>();
    public BattleProcedure procedure;
    private Transform _root;

    public Transform root
    {
        get
        {
            if (_root == null) _root = GameObject.Find("BattleController").transform;
            return _root;
        }
    }

    public void EnterBattle(Mission mission, PlayMode playMode)
    {
        DebugLog("开始进入关卡:", mission.ToString());
        Loading.Open(UILoading.uiLoading, "EnterBattle");
        TimeHelper.ChangeBattleScene();
        AudioPlay.defaultListener.enabled = false;
        if (playMode == PlayMode.Day) procedure = new DayProcedure(mission);
        procedure.BattlePrepare();
        GameScene.Load("Battle", () =>
        {
            mission.LoadEditor(editor =>
            {
                procedure.StartBattle(mission, editor);
            });
        });
    }

    public void Pause(string key)
    {
    }

    public void Continue(string key)
    {
    }

    public void TryNextNode(PredicateType predicate, params string[] args)
    {
        if (procedure != null)
        {
            procedure.TryNextNode(predicate, args);
        }
    }
    
    public void NextFinishAction(string log)
    {
        if(procedure!=null)
        {
            DebugLog("FinishAction", log);
            procedure.finishAction.NextAction();
        }
    }

    public void DebugLog(string title, string msg)
    {
        GameDebug.LogFormat("{0}==>{1}", title, msg);
    }

    public void ExitBattle(OutGameStation reason)
    {
        
    }
}