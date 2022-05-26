using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;

public class GameService: IRewardObject
{
    protected static ServiceType globalService;

    private static List<GameService> serviceList = new List<GameService>();

    public static ServiceType currentService
    {
        get
        {
            if (BattleController.Instance.ctrlProcedure == null) return globalService;
            return globalService | BattleController.Instance.ctrlProcedure.service;
        }
    }

    public static bool ContainService(ServiceType type)
    {
        return (currentService & type) != 0;
    }

    public static void AddGlobalService(ServiceType type)
    {
        globalService = globalService | type;
    }

    public static void RemoveGlobalService(ServiceType type)
    {
        globalService = globalService & ~type;
    }

    public static AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        var serviceData = DataMgr.Instance.GetSqlService<ServiceData>();
        for (int i = 0; i < serviceData.tableList.Count; i++)
        {
            if (serviceData.tableList[i].global == 1)
            {
                serviceList.Add(GetService(serviceData.tableList[i]));
            }
        }

        return process;
    }

    public static GameService GetService(ServiceData data)
    {
        ServiceSaveData saveData = DataMgr.Instance.GetSqlService<ServiceSaveData>().Where(ta => ta.targetId == data.ID);
        if (!data.field.IsNullOrEmpty())
        {
             return (GameService) Activator.CreateInstance(Type.GetType(data.field), data, saveData);
        }
        else
        {
            return new GameService(data, saveData);
        }
    }

    public static T GetService<T>(int ID) where T : GameService
    {
        return (T) GetService(ID);
    }
    
    public static GameService GetService(int ID)
    {
        ServiceData data = DataMgr.Instance.GetSqlService<ServiceData>().WhereID(ID);
        return GetService(data);
    }

    public ServiceData dbData { get; }

    public GameService(ServiceData data, ServiceSaveData saveData)
    {
        this.dbData = data;
    }
    
    public void GetIcon(string type, Action<Sprite> callback)
    {
        SpriteLoader.LoadIcon(dbData.icon, callback);
    }

    public string GetText(string type)
    {
        switch (type)
        {
            case TypeList.Des:
                return Language.GetContent(dbData.des);
            case TypeList.GetDes:
                return Language.GetContent(dbData.getdes);
            case TypeList.Title:
                return Language.GetContent(dbData.title);
        }

        return null;
    }

    public int stationCode { get; }

    public virtual float GetReward(float rewardCount, RewardFlag flag)
    {
        return rewardCount;
    }    
}