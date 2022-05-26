using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;

/// <summary>
/// 每日任务管理器
/// </summary>
public class DailyTaskManager : Singleton<DailyTaskManager>
{
    /// <summary>
    /// 每日任务列表
    /// </summary>
    private List<DailyTaskBase> _dailyTaskList = new List<DailyTaskBase>();

    public List<DailyTaskBase> DailyTasks => _dailyTaskList;

    /// <summary>
    /// 刷新任务事件
    /// </summary>
    public event Action<List<DailyTaskBase>> OnRefreshDailyTaskEvent;
    
    /// <summary>
    /// 任务状态改变事件
    /// </summary>
    public event Action<DailyTaskBase, TaskStation> OnDailyTaskStationEvent;
    
    /// <summary>
    /// 默认每日任务的数量
    /// </summary>
    public const int defaultTaskCount = 3;
    
    
    private IRewardBag _refreshReward;
    private IRewardBag _doubleReward;
    
    
    public AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        process.IsDone = false;
        SqlService<DailyTaskSaveData> service = DataMgr.Instance.GetSqlService<DailyTaskSaveData>();

        if (!TimeHelper.IsNewDay("DailyTask") && service.tableList.Count > 0)
        {
            List<DailyTaskSaveData> saveData = service.tableList;
            for (int i = 0; i < saveData.Count; i++)
            {
                DailyTaskBase taskBase = CreateTask(saveData[i]);
                _dailyTaskList.Add(taskBase);
                taskBase.RegisterEvent();
            }
        }
        else
        {
            //TODO 临时注释掉
             RefreshNewTask(); 
        }

        //注册跨天刷新逻辑
        TimeHelper.onNewDay += RefreshNewTask;

        process.SetComplete();
        return process;
    }
    
    public IapBag doubleRewardBag
    {
        get
        {
            if (_doubleReward == null)
            {
                _doubleReward = Commercialize.GetRewardBag(DataMgr.CommonData(33002).ToInt());
            }

            return (RewardBag)_doubleReward;
        }
    }
    
    public IapBag refreshDailyTaskBag
    {
        get
        {
            if (_refreshReward == null)
            {
                _refreshReward = Commercialize.GetRewardBag(DataMgr.CommonData(33003).ToInt());
            }

            return (IapBag)_refreshReward;
        }
    }
    
    private DailyTaskBase CreateTask(DailyTaskSaveData saveData)
    {
        DailyTaskData data = DataMgr.Instance.GetSqlService<DailyTaskData>().WhereID(saveData.targetID);
        if (data == null)
        {
            GameDebug.LogError("DailyTask connot be null" + saveData.ID);
        }

        var t = GetDailyTaskType(data.field);
        return Activator.CreateInstance(t, data, saveData) as DailyTaskBase;
    }
    
    private DailyTaskBase CreateTask(DailyTaskData data, int index)
    {
        var t = GetDailyTaskType(data.field);
        return Activator.CreateInstance(t, data, index) as DailyTaskBase;
    }

    private Type GetDailyTaskType(string fieldType)
    {
        Type t = Type.GetType("DailyTask" + fieldType);
        if (t == null)
        {
            GameDebug.LogError("DailyTask is Null" + fieldType);
            return null;
        }
        return t;
    }
    
    
    /// <summary>
    /// 刷新任务，外界调用
    /// </summary>
    public void RefreshNewTask()
    {
        _refreshNewTask(defaultTaskCount, refreshDailyTaskBag.iapState == IapState.Invalid ? 2 : 1);
    }

    /// <summary>
    /// 刷新任务，外界调用
    /// </summary>
    public void RefreshNewTask(int count, int type)
    {
        _refreshNewTask(count, type);
    }
    
    /// <summary>
    /// 刷新任务
    /// </summary>
    /// <param name="count"></param>
    /// <param name="type">0: 主动刷新全部随机 1: 自动刷新必有广告 2: 没有广告</param>
    private void _refreshNewTask(int count, int type)
    {
        List<DailyTaskData> allData = DataMgr.Instance.GetSqlService<DailyTaskData>().tableList;
        List<DailyTaskBase> result = new List<DailyTaskBase>();
        if (!_dailyTaskList.IsNullOrEmpty())
        {
            for (int i = 0; i < _dailyTaskList.Count; i++)
            {
                _dailyTaskList[i].UnRegisterEvent();
            }
        }

        if (type == 0)
        {
            List<DailyTaskData> target = allData.Random(defaultTaskCount);

            for (int i = 0; i < target.Count; i++)
            {
                result.Add(CreateTask(target[i], i));
            }
        }
        else if (type == 1)
        {
            List<DailyTaskData> needData = new List<DailyTaskData>();
            List<float> weight = new List<float>();

            for (int i = 0; i < allData.Count; i++)
            {
                if (allData[i].autoMustShow == 1)
                {
                    needData.Add(allData[i]);
                    weight.Add(0);
                }
                else
                {
                    weight.Add(1);
                }
            }

            int index = 0;

            for (int i = 0; i < needData.Count; i++)
            {
                result.Add(CreateTask(needData[i], index));
                index++;
            }

            int resultCount = count - result.Count;
            if (resultCount > 0)
            {
                List<int> target = RandomHelper.RandomSeveralWeight(weight, resultCount);
                for (int i = 0; i < target.Count; i++)
                {
                    result.Add(CreateTask(allData[target[i]], index));
                    index++;
                }
            }
        }
        else if (type == 2)
        {
            List<DailyTaskData> target = allData.Random(defaultTaskCount, ts => ts.autoMustShow != 1);

            for (int i = 0; i < target.Count; i++)
            {
                result.Add(CreateTask(target[i], i));
            }
        }

        _dailyTaskList = result;
        if (!_dailyTaskList.IsNullOrEmpty())
        {
            for (int i = 0; i < _dailyTaskList.Count; i++)
            {
                _dailyTaskList[i].RegisterEvent();
            }
        }

        OnRefreshDailyTaskEvent?.Invoke(_dailyTaskList);
        Save();
    }

    /// <summary>
    /// 任务状态改变
    /// </summary>
    public void OnChangeTaskStation(DailyTaskBase taskBase , TaskStation curStation)
    {
        OnDailyTaskStationEvent?.Invoke(taskBase , curStation);
        Save();
    }
    
    public void Save()
    {
        SqlService<DailyTaskSaveData> service = DataMgr.Instance.GetSqlService<DailyTaskSaveData>();
        var saveData = DailyTaskSaveData.GetSaveData(_dailyTaskList.ToArray());

        if (service.tableList.Count > 0)
        {
            for (int i = 0; i < _dailyTaskList.Count; i++)
            {
                saveData[i].ID = service.tableList[i].ID;
                service.Update(saveData[i]);
            }
        }
        else
        {
            service.InsertAll(saveData);
        }
    }
}