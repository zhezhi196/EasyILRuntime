using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;

public class Mission
{
    public static List<Mission> missions = new List<Mission>();
    
    public static AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        var data = DataInit.Instance.GetSqlService<MissionData>().tableList;
        for (int i = 0; i < data.Count; i++)
        {
            var saveData = DataInit.Instance.GetSqlService<MissionSaveData>().Where(fd =>
            {
                return fd.missionID == data[i].ID;
            });
            Mission mis = new Mission(data[i],saveData);
            missions.Add(mis);
        }
        return process;
    }
    public MissionData dbData { get; }

    public GameAttribute playerAttribute
    {
        get
        {
            return new GameAttribute(0);
        }
    }

    public MonsterAttribute monsterAttribute
    {
        get
        {
            return new MonsterAttribute(0);
        }
    }

    public Mission(MissionData dbData, MissionSaveData saveData)
    {
        this.dbData = dbData;
    }

    public void LoadEditor(Action<MissionGraph> callback)
    {
        AssetLoad.PreloadAsset<MissionGraph>($"LevelEditor/{dbData.ID}.asset", ass =>
        {
            callback?.Invoke(ass.Result);
        });
    }
    
    public void Complete()
    {
        
    }
}