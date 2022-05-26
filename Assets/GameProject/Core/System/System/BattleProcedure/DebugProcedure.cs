using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using System;

public class DebugProcedure : BattleProcedure
{
    public DebugProcedure(Mission mission, Action callback) : base(mission, callback)
    {
        AppendCtrl(new SceneCtrl());
        AppendCtrl(new MonsterCtrl());
        AppendCtrl(new BagPackCtrl());
        //AppendCtrl(new DropCtrl());
        AppendCtrl(new BulletCtrl());
        AppendCtrl(new PlayerCtrl());
        //AppendCtrl(new GiftCtrl());
        //AppendCtrl(new PropsCtrl());
        //AppendCtrl(new ProgressCtrl());
        //AppendCtrl(new TimelineCtrl());
        //AppendCtrl(new MapCtrl());
        AppendCtrl(new LimitRewardCtrl());
    }

    public override GameMode mode
    {
        get { return GameMode.Main; }
    }

    protected override void SortAction()
    {
        AddAction(BattleController.GetCtrl<SceneCtrl>().loadScene);
        AddAction(BattleController.GetCtrl<SceneCtrl>().unloadScene);
        AddAction(BattleController.GetCtrl<PlayerCtrl>().loadPlayer);
        AddAction(BattleController.GetCtrl<BulletCtrl>().initBullet);
        AddAction(BattleController.GetCtrl<PlayerCtrl>().bronPlayer);
        //AddAction(BattleController.GetCtrl<GiftCtrl>().initGift);
        AddAction(BattleController.GetCtrl<MonsterCtrl>().loadMonster);
        //AddAction(BattleController.GetCtrl<PropsCtrl>().loadPros);
        //AddAction(BattleController.GetCtrl<ProgressCtrl>().sortProgress);
        AddAction(BattleController.GetCtrl<LimitRewardCtrl>().loadLimit);
        AddAction(BattleController.GetCtrl<SceneCtrl>().LoadComplete);
        //AddAction(BattleController.GetCtrl<PlayerCtrl>().bronAnim);
        AddAction(BattleController.GetCtrl<PlayerCtrl>().openGameUi);
    }

    protected override void SortResultAction(Mission mission, bool result)
    {
        AddAction(BattleController.GetCtrl<PlayerCtrl>().endAction);
    }

    public override SaveStation GetSaveStation(int unloadId, int loadId, int initID)
    {
        SaveStation result = SaveStation.None;
        var nods = editorData.startNode.nextNode;
        GetNode(nods, unloadId, loadId, initID, ref result);
        return result;
    }

    private void GetNode(List<NodeBase> nodes, int unloadId, int loadId, int initID, ref SaveStation result)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] is TaskNode node)
            {
                if (node.station == NodeStation.Complete || node.station == NodeStation.Running)
                {
                    if (result == SaveStation.None && node.id == loadId)
                    {
                        result = SaveStation.Load;
                    }

                    if (result == SaveStation.Load && node.id == initID)
                    {
                        result = SaveStation.Init;
                    }

                    if (node.id == unloadId)
                    {
                        result = SaveStation.Unload;
                    }

                    if (node.station == NodeStation.Complete && result != SaveStation.Unload)
                    {
                        GetNode(node.nextNode, unloadId, loadId, initID, ref result);
                    }
                    break;
                }
            }
        }
    }
}
