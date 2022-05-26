using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;

public enum TaskStation
{
    UnComplete,
    CompleteUnReward,
    Rewarded,
}


public abstract class DailyTaskBase : ITask
{
    private TaskStation _station;
    public DailyTaskData dbData { get; }
    public DailyTaskReward taskReward { get; }
    public int currentCount { get; set; }
    public int index { get; }
    public int stationCode { get; }

    public bool redPointIsOn
    {
        get { return station == TaskStation.CompleteUnReward; }
    }
    
    public event Action onSwitchStation;
    public event Action<TaskStation> onStationChanged;
    
    public TaskStation station
    {
        get { return _station; }
        set
        {
            TaskStation temp = _station;
            _station = value;
            onStationChanged?.Invoke(_station);
            DailyTaskManager.Instance.OnChangeTaskStation(this, _station);
            if (temp == TaskStation.UnComplete && value == TaskStation.CompleteUnReward)
            {
                onSwitchStation?.Invoke();
            }

            if (temp == TaskStation.CompleteUnReward && value == TaskStation.Rewarded)
            {
                onSwitchStation?.Invoke();
            }
        }
    }
    
    public DailyTaskBase(DailyTaskData dbData, int index)
    {
        this.dbData = dbData;
        this.index = index;
        currentCount = 0;
        _station = TaskStation.UnComplete;
        float[] difficulteRate = dbData.rate.Split(ConstKey.Spite0).ToFloatArray();
        int taskinfoIndex = RandomHelper.RandomWeight(difficulteRate);
        string[] rewardCount = dbData.rewardCount.Split(ConstKey.Spite0);
        int taskComplete = dbData.taskComplete.Split(ConstKey.Spite0)[taskinfoIndex].ToInt();
        taskReward = new DailyTaskReward(taskinfoIndex, dbData.rewardRate, dbData.reward, rewardCount[taskinfoIndex], taskComplete, dbData.getDes);
    }

    public DailyTaskBase(DailyTaskData dbData, DailyTaskSaveData saveData)
    {
        this.dbData = dbData;
        this._station = saveData.targetStation;
        this.taskReward = new DailyTaskReward(saveData);
        this.currentCount = saveData.targetCurrentCount;
        this.index = saveData.targetIndex;
    }

    public virtual void RegisterEvent()
    {
    }

    public virtual void UnRegisterEvent()
    {
    }

    public bool TryAddCount(int count, params object[] obj)
    {
        if (isComplete) return false;
        if (IsAchieve(obj))
        {
            AddCompleteCount(count);
            if (isComplete)
            {
                Complete();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 根据条件验证是否增加进度
    /// </summary>
    /// <param name="obj">条件参数</param>
    /// <returns>是否</returns>
    public abstract bool IsAchieve(params object[] obj);

    public virtual void AddCompleteCount(int add)
    {
        this.currentCount += add;
        DailyTaskManager.Instance.Save();
    }

    public virtual bool isComplete
    {
        get { return currentCount >= taskReward.completeCount; }
    }

    public virtual void Complete()
    {
        if (station == TaskStation.UnComplete)
        {
            this.station = TaskStation.CompleteUnReward;
        }
    }

    public void GetIcon(string type, Action<Sprite> callback)
    {
        SpriteLoader.LoadIcon(dbData.icon, callback);
    }

    public string GetText(string type)
    {
        if (type == TypeList.Title)
        {
            return Language.GetContent(dbData.taskName);
        }
        else if (type == TypeList.Des)
        {
            return string.Format(Language.GetContent(dbData.taskContent),taskReward.completeCount);
        }
        else if (type == TypeList.GetDes)
        {
            return string.Format(Language.GetContent(taskReward.getDes), taskReward.rewardContent.finalCount);
        }

        return null;
    }

    public float GetReward(float rewardCount, RewardFlag flag)
    {
        if (station == TaskStation.CompleteUnReward)
        {
            if (rewardCount > 1)
            {
                
                DailyTaskManager.Instance.doubleRewardBag.GetReward(rew =>
                {
                    if (rew.result == IapResultMessage.Success)
                    {
                        taskReward.GetReward(dbData.ID, rewardCount);
                        this.station = TaskStation.Rewarded;
                    }
                });
            }
            else
            {
                taskReward.GetReward(dbData.ID, rewardCount);
                this.station = TaskStation.Rewarded;
            }
        }

        return rewardCount;
    }


}

