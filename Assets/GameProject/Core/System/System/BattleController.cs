/*
/*
/*
/*
 * 脚本名称：BattleController
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-05-11 06:25:10
 * 脚本作用：
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Module;
using UnityEngine;

public enum GameStation
{
    UI,
    Battle,
}

public enum OutGameStation
{
    Win, //通关推出
    Fail, //失败退出 
    Break, //手动退出
}

[Flags]
public enum SaveFlag
{
    Loading = 1,
}

public class BattleController : Singleton<BattleController>
{
    #region 字段
    private BattleProcedure m_ctrlProcedure;
    private Transform m_heroTransform;
    private Transform m_timelineTransform;
    private Transform _root;

    public int missionId
    {
        get
        {
            if (ctrlProcedure == null) return 0;
            return ctrlProcedure.mission.dbData.ID;
        }
    }

    #endregion
    
    #region 属性
    
    public BattleProcedure ctrlProcedure => m_ctrlProcedure;
    
    public List<string> pauseList
    {
        get
        {
            if (ctrlProcedure == null) return null;
            return ctrlProcedure.pauseList;
        }
    }

    public GameStation gameStation
    {
        get
        {
            if (ctrlProcedure == null) return GameStation.UI;
            return GameStation.Battle;
        }
    }
    
    /// <summary>
    /// 英雄root
    /// </summary>
    public Transform heroRoot
    {
        get
        {
            if (m_heroTransform == null)
            {
                m_heroTransform = GameObject.Find("BattleController/HeroRoot").transform;
            }
    
            return m_heroTransform;
        }
    }
    
    public Transform timelineRoot
    {
        get {
            if (m_timelineTransform == null)
            {
                m_timelineTransform = GameObject.Find("BattleController/TimelineRoot").transform;
            }
    
            return m_timelineTransform;
        }
    }

    public Transform root
    {
        get
        {
            if (_root == null)
            {
                _root = GameObject.Find("BattleController").transform;
            }

            return _root;
        }
    }
    
    #endregion
    
    #region Public接口

    public static TaskNode GetNode(int id)
    {
        if (Instance.ctrlProcedure == null) return null;
        for (int i = 0; i < Instance.ctrlProcedure.editorData.nodes.Count; i++)
        {
            if (Instance.ctrlProcedure.editorData.nodes[i] is TaskNode task && task.id == id)
            {
                return task;
            }
        }

        return null;
    }

    public static TaskNode currNode
    {
        get
        {
            if (Instance.ctrlProcedure == null) return null;
            return Instance.ctrlProcedure.currentNode;
        }
    }
    public static T GetCtrl<T>() where T: BattleSystem
    {
        if (Instance.ctrlProcedure != null)
        {
            for (int i = 0; i < Instance.ctrlProcedure.battleCtrl.Count; i++)
            {
                if (Instance.ctrlProcedure.battleCtrl[i] is T result)
                {
                    return result;
                }
            }
        }

        return null;
    }

    public void EnterBattle(Mission mission, EnterNodeType type)
    {
        if (gameStation == GameStation.UI)
        {
            GameDebug.LogFormat("进入关卡: {0}", mission.dbData.ID);
            Loading.Open(UILoading.uiLoading, "EnterBattle");
            if (mission.gameMode == GameMode.Main)
            {
                TimeHelper.ResetTimeScale();
                m_ctrlProcedure = new MainProcedure(mission, null);
            } else if (mission.gameMode == GameMode.Black)
            {
                TimeHelper.ResetTimeScale();
                m_ctrlProcedure = new BlackProcedure(mission, null);
            }

            AudioManager.StopMusic();
            // AudioPlay.StopPlayBGM();
            TimeHelper.ChangeBattleScene();
            UIController.Instance.ClearStack(true, true);
            EventCenter.Register<string>(ConstKey.UIOpenStart, OnOpenUI);
            EventCenter.Register<string>(ConstKey.UICloseStart, OnCloseUI);
            m_ctrlProcedure.BattlePrepare(type);
            GameScene.Load("Battle", () =>
            {
                mission.LoadEditor(type, editor =>
                {
                    m_ctrlProcedure.StartBattle(type);
                });
            });
        }
    }

    private void OnOpenUI(string name)
    {
        if (name != "GameUI" && name != "BlackGameUI")
        {
            Pause(name);
        }
    }    
    private void OnCloseUI(string name)
    {
        if (name != "GameUI")
        {
            Continue(name);
        }
    }

    /// <summary>
    /// 退出当前战斗场景
    /// </summary>
    /// <param name="station"></param>
    public bool ExitBattle(OutGameStation station)
    {
        if (gameStation == GameStation.Battle)
        {
            //AudioPlay.StopAudio();
            DebugLog("退出游戏", "原因" + station);
            EventCenter.Register<string>(ConstKey.UIOpenComplete, OnOpenUI);
            EventCenter.Register<string>(ConstKey.UICloseStart, OnCloseUI);
            Loading.Open(UILoading.uiLoading,"ExitBattle");
            TimeHelper.ChangeBattleScene();
            UIController.Instance.ClearStack(true, true);
            // AudioPlay.defaultListener.enabled = true;
            AudioManager.listener.enabled = true;
            Group.ChangeBattleScene();
            Match.ClearAll();
            ctrlProcedure.ExitBattle(station);
            GameScene.Load("UIScene", () =>
            {
                m_ctrlProcedure = null;
                TimeHelper.ResetTimeScale();
                UIController.Instance.Open("MainUI", UITweenType.Fade); 
                Loading.Close(UILoading.uiLoading,"ExitBattle");
                if (station == OutGameStation.Fail || station == OutGameStation.Win)
                {
                    LocalSave.DeleteFile();
                }
            });
            
            return true;
        }

        return false;
    }

    
    #region 继续，暂停,退出战斗
    
    public void Continue(string key)
    {
        if (ctrlProcedure == null) return;
        if (TimeHelper.isPause && pauseList.Contains(key))
        {
            pauseList.Remove(key);
            if (pauseList.Count == 0)
            {
                GameDebug.Log("继续游戏");
                TimeHelper.Continue();
                EventCenter.Dispatch(EventKey.GamePause, false);
                ctrlProcedure.OnContinue();
                AudioListener.pause = false;
                GameDebug.LogFormat("TimeHelper Pause: {0}", string.Join(",", pauseList));
            }
        }
    }
    
    public void Pause(string key)
    {
        if (ctrlProcedure == null) return;
        if (!pauseList.Contains(key))
        {
            pauseList.Add(key);
            if (!TimeHelper.isPause && pauseList.Count == 1)
            {
                GameDebug.Log("暂停");
                //AudioListener.pause = true;
                TimeHelper.Pause();
                EventCenter.Dispatch(EventKey.GamePause, true);
                ctrlProcedure.OnPause();
                GameDebug.LogFormat("TimeHelper Pause: {0}", string.Join(",", pauseList));
            }
        }
    }
    
    #endregion
    
    #endregion


    public int watchAdsCount
    {
        get
        {
            var serveic=DataMgr.Instance.GetSqlService<AdsSaveData>();
            if (serveic.tableList.IsNullOrEmpty()) return 0;
            return DataMgr.Instance.GetSqlService<AdsSaveData>().tableList[0].Count;
        }
    }
    
    public async void Save(SaveFlag flag = SaveFlag.Loading)
    {
        //todo 场景存档 物品存档 节点存档 掉落存档 怪物死亡存档 地图区域存档 背包存档 黄点进度存档
        if (ctrlProcedure != null)
        {
            if (ctrlProcedure.mode == GameMode.Black)
                return;
            ctrlProcedure.Save();
        }

        if ((flag & SaveFlag.Loading) != 0)
        {
            Loading.Open(UILoading.uiLoading, "Save", Language.GetContent("2317"), Language.GetContent("2318"));
            await Async.WaitforSecondsRealTime(2);
            Loading.Close(UILoading.uiLoading, "Save");
        }
    }
    
    public void NextFinishAction(string log)
    {
        if(ctrlProcedure!=null)
        {
            DebugLog("FinishAction", log);
            ctrlProcedure.finishAction.NextAction();
        }
    }
    
    [Conditional("LOG_ENABLE")]
    public void DebugLog(string title, string msg)
    {
        GameDebug.LogFormat("{1}==>{0}", msg,  title);
    }

    public void TryNextNode(PredicateType predicate, params string[] args)
    {
        if (ctrlProcedure != null)
        {
            ctrlProcedure.TryNextNode(predicate, args);
        }
    }

    public void SkipCurrNode()
    {
        if (ctrlProcedure != null)
        {
            ctrlProcedure.SkipNode(PredicateType.AlwayTrue,EnterNodeType.SkipNode);
        }
    }

    public void OnUpdate()
    {
        if (ctrlProcedure != null)
        {
            ctrlProcedure.OnUpdate();
        }

        if (mouseCount > 0)
        {
            mouseTime += TimeHelper.deltaTime;
            if (mouseTime >= 1)
            {
                mouseCount = 0;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (GetCtrl<PlayerCtrl>() != null && Player.player != null)
            {
                mouseCount++;
                mouseTime = 0;
                if (mouseCount >= 3)
                {
                    Ray ray = Player.player.evCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject.layer == 11)
                        {
                            Monster monster = hit.collider.transform.GetComponentInParent<Monster>();
                            monster.isLog = !monster.isLog;
                            mouseCount = 0;
                        }
                    }

                }
            }
        }
    }

    private int mouseCount;
    private float mouseTime;


    public Mission GetCurMission()
    {
        return ctrlProcedure.mission;
    }
}
