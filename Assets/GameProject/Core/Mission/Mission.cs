using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Mission : IRedPoint, IRewardObject
{
    public const string Title = "Title";
    public const string Des = "Des";
    public const string unlockDes = "unlockDes";
    
    public static List<Mission> missionList = new List<Mission>();
    public static AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        var allData = DataMgr.Instance.GetSqlService<MissionData>().tableList;
        for (int i = 0; i < allData.Count; i++)
        {
            Mission mission = new Mission(allData[i]);
            missionList.Add(mission);
        }

        Mission firstMission = FirstMission();
        if (firstMission.station == CommonStation.Locked)
        {
            firstMission.RunningMission();
        }
        return process;
    }

    private static Mission FirstMission()
    {
        for (int i = 0; i < missionList.Count; i++)
        {
            if (missionList[i].dbData.unlockMission == 0)
            {
                return missionList[i];
            }
        }
        return null;
    }
    
    
    public int stationCode => 0;
    private RewardBag _unlockIap;

    public float crossTime
    {
        get
        {
            return DataMgr.Instance.GetSqlService<MissionSaveData>().Where(fd => fd.missionID == dbData.ID).crossTime;
        }
    }
    private CommonStation _station;

    public CommonStation station
    {
        get { return _station; }
        set
        {
            _station = value;
            onSwitchStation?.Invoke();
        }
    }

    public MissionGraph editorData;

    public RewardBag unlockIap
    {
        get
        {
            if (_unlockIap == null) _unlockIap = (RewardBag) Commercialize.GetRewardBag(dbData.unlockIap);
            return _unlockIap;
        }
    }

    public float time
    {
        get
        {
            var saveData = DataMgr.Instance.GetSqlService<MissionSaveData>().Where(td => td.missionID == dbData.ID);
            if (saveData != null)
            {
                return saveData.crossTime;
            }

            return 0;
        }
    }

    public string des
    {
        get { return Language.GetContent(dbData.des); }
    }

    public bool isComplete
    {
        get { return station == CommonStation.Complete || station == CommonStation.NewComplete; }
    }

    public bool redPointIsOn
    {
        get { return station == CommonStation.Unlocked; }
    }

    public MissionData dbData { get; }

    public GameMode gameMode
    {
        get { return (GameMode) dbData.mode; }
    }

    public GameDifficulte difficulte { get; }

    public event Action onSwitchStation;

    public Mission(MissionData dbData)
    {
        this.dbData = dbData;
        difficulte = (GameDifficulte) dbData.difficulte;
        MissionSaveData saveData = DataMgr.Instance.GetSqlService<MissionSaveData>().Where(sd => sd.missionID == dbData.ID);
        if (saveData != null)
        {
            this.station = (CommonStation) saveData.station;
        }
        else
        {
            this.station = CommonStation.Locked;
        }

    }

    public void EnterMission(Action callback)
    {
    }

    public void Unlocked(bool trigger = true)
    {
        if (this.station == CommonStation.Locked)
        {
            this.station = CommonStation.Unlocked;
            var service = DataMgr.Instance.GetSqlService<MissionSaveData>();
            var data = service.Where(sd => { return sd.missionID == dbData.ID; });
            if (data == null)
            {
                data = new MissionSaveData() {missionID = dbData.ID, station = 1};
                service.Insert(data);
            }
            else
            {
                service.Update(new MissionSaveData() {ID = data.ID, missionID = dbData.ID, station = 1});
            }

            if (dbData.difficulte != 0  && trigger)
            {
                GlobleTrigger.AddTrigger("MissionComplete", this);
            }
        }
    }

    public void RunningMission()
    {
        this.station = CommonStation.Running;
        var service = DataMgr.Instance.GetSqlService<MissionSaveData>();
        var data = service.Where(sd => sd.missionID == dbData.ID);
        if (data != null)
        {
            service.Update(sd => sd.missionID == dbData.ID, "station", 2);
        }
        else
        {
            service.Insert(new MissionSaveData() {missionID = dbData.ID, station = 2});
        }
    }

    public void Complete(float time)
    {
        if (this.station == CommonStation.Running)
        {
            this.station = CommonStation.NewComplete;
            var service = DataMgr.Instance.GetSqlService<MissionSaveData>();
            var data = service.Where(sd => sd.missionID == dbData.ID);
            if (data == null)
            {
                data = new MissionSaveData() {missionID = dbData.ID, station = 3, crossTime = time};
                service.Insert(data);
            }
            else
            {
                if (data.crossTime > time || data.crossTime == 0)
                {
                    service.UpdateAll(sd => sd.missionID == dbData.ID, new[] {"station", "crossTime"},
                        new object[] {3, time});
                }
            }

            for (int i = 0; i < missionList.Count; i++)
            {
                if (missionList[i].dbData.unlockMission == dbData.ID)
                {
                    missionList[i].Unlocked();
                }
            }

            EventCenter.Dispatch(EventKey.CompleteMission, this);
        }
        else if (isComplete)
        {
            this.station = CommonStation.Complete;
            var service = DataMgr.Instance.GetSqlService<MissionSaveData>();
            var data = service.Where(sd => sd.missionID == dbData.ID);
            if (data == null)
            {
                data = new MissionSaveData() {missionID = dbData.ID, station = 4, crossTime = time};
                service.Insert(data);
            }
            else
            {
                if (data.crossTime > time || data.crossTime == 0)
                {
                    service.UpdateAll(sd => sd.missionID == dbData.ID, new[] {"station", "crossTime"},
                        new object[] {4, time});
                }
            }
        }
    }

    public void GetIcon(string type, Action<Sprite> callback)
    {
        SpriteLoader.LoadIcon(dbData.icon, callback);
    }

    public string GetText(string type)
    {
        if (type == Title)
        {
            return Language.GetContent(dbData.title);
        }
        else if (type == Des)
        {
            return Language.GetContent(dbData.des);
        }
        else if (type == unlockDes)
        {
            return Language.GetContent(dbData.unlockDes);
        }

        return string.Empty;
    }

    public void LoadEditor(EnterNodeType type, Action<MissionGraph> callback)
    {
        AssetLoad.PreloadAsset<MissionGraph>($"LevelEditor/{dbData.ID}.asset", handle =>
        {
            editorData = handle.Result;
            //初始化节点状态
            editorData.InitNodeStation(type);
            
            //加载节点摆放数据
            editorData.LoadEditor(type);
            callback?.Invoke(editorData);
            AssetLoad.Release(handle);
        });
    }

    public NodeStation GetNodeStation(int id)
    {
        if (editorData != null)
        {
            for (int i = 0; i < editorData.nodes.Count; i++)
            {
                if (editorData.nodes[i] is TaskNode node)
                {
                    if (node.id == id)
                    {
                        return node.station;
                    }
                }
            }
        }

        return NodeStation.Locked;
    }


    public float GetReward(float rewardCount, RewardFlag flag)
    {
        RunningMission();
        return 1;
    }
}