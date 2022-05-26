using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;

public abstract class AchievementBase : IRedPoint, IRewardObject
{

    /// <summary>
    /// 这个成就拥有的所有奖励（阶段奖励）
    /// </summary>
    public AchievementReward[] allReward { get; }

    public event Action OnChangeCount;
    
    /// <summary>
    /// 当前的奖励
    /// </summary>
    private AchievementReward _currReward;
    

    public AchievementReward currReward
    {
        get { return _currReward; }
        set
        {
            _currReward = value;
            if (currReward != null)
            {
                currReward.OnCurrent();
                AchievementManager.Instance.OnChangeReward(this,currReward);
            }
        }
    }

    public int count
    {
        get
        {
            int total = 0;
            for (int i = 0; i < allReward.Length; i++)
            {
                if (allReward[i].isComplete)
                {
                    total++;
                }
            }

            return total;
        }
    }

    public bool isComplete
    {
        get
        {
            for (int i = 0; i < allReward.Length; i++)
            {
                if (allReward[i].station != TaskStation.Rewarded) return false;
            }
            
            return true;
        }
    }



    public AchievementBase(AchievementSaveData saveData)
    {
        List<AchievementData> typeData = DataMgr.Instance.GetSqlService<AchievementData>().WhereList(fd => fd.field == this.GetType().Name);
        if (saveData != null)
        {
            this.currentCount = saveData.currentCount;
        }
        allReward = new AchievementReward[typeData.Count];
        for (int i = 0; i < allReward.Length; i++)
        {
            allReward[i] = new AchievementReward(typeData[i], this,saveData,OnRewardChanged);
        }

        if (saveData != null)
        {
            for (int i = 0; i < allReward.Length; i++)
            {
                if (allReward[i].dbData.ID == saveData.currId)
                {
                    currReward = allReward[i];
                    break;
                }
            }
        }
        else
        {
            currReward = allReward.Find(fd => fd.dbData.lastid == 0);
        }

        if (!isComplete)
        {
            Register();
        }
    }

    private void OnRewardChanged(AchievementReward arg1, TaskStation arg2)
    {
        if (arg2 == TaskStation.Rewarded)
        {
            if (allReward.Contains(sd => sd.lastReward == arg1))
            {
                NextReward();
            }
        }
        SaveToLocal();
        RefreshRedPoint();
    }

    public bool TryAddCount(int count, params object[] obj)
    {
        if (IsAchieve(obj))
        {
            AddCompleteCount(count);
            if (currReward.isComplete)
            {
                currReward.Complete();
                if (isComplete)
                {
                    UnRegister();
                }
                return true;
            }
        }

        return false;
    }
    
    public bool TrySetCount(int count, params object[] obj)
    {
        if (IsAchieve(obj))
        {
            currentCount = count;
            SaveToLocal();
            if (currReward.isComplete)
            {
                currReward.Complete();
                if (isComplete)
                {
                    UnRegister();
                }
                return true;
            }
        }

        return false;
    }

    public abstract void Register();

    public abstract void UnRegister();


    public bool redPointIsOn
    {
        get { return currReward.station == TaskStation.CompleteUnReward; }
    }

    public int currentCount { get; set; }

    public abstract bool IsAchieve(params object[] obj);

    public void AddCompleteCount(int add)
    {
        currentCount += add;
        SaveToLocal();
    }
    
    public void InitPreReward()
    {
        for (int i = 0; i < allReward.Length; i++)
        {
            allReward[i].InitPreReward();
        }
    }

    public void SaveToLocal()
    {
        SqlService<AchievementSaveData> service = DataMgr.Instance.GetSqlService<AchievementSaveData>();
        if (service.tableList.Contains(st => st.type == GetType().Name))
        {
            service.UpdateAll(st => st.type == GetType().Name, new[] {"currentCount", "currId","complete"}, new object[] {currentCount, currReward.dbData.ID,isComplete?1:0});
        }
        else
        {
            service.Insert(new AchievementSaveData(){type = GetType().Name, currentCount = currentCount, currId = currReward.dbData.ID,complete = isComplete?1:0});
        }
    }

    public void GetIcon(string type, Action<Sprite> callback)
    {
        SpriteLoader.LoadIcon(currReward.dbData.icon, callback);
    }

    public string GetText(string type)
    {
        switch (type)
        {
            case TypeList.Title:
                return Language.GetContent(currReward.dbData.title);
            case TypeList.Des:
                return String.Format(Language.GetContent(currReward.dbData.des), Mathf.Clamp(currentCount,0,currReward.dbData.targetCount),currReward.dbData.targetCount);
            case TypeList.GetDes:
                //return currReward.reward[0].GetText(type);
                return string.Format(Language.GetContent(currReward.dbData.getDes), currReward.reward[0].finalCount.value);
        }

        return null;
    }

    public int stationCode
    {
        get { return currReward != null ? 0 : 1; }
    }

    /// <summary>
    /// 获得当前奖励几倍的奖励
    /// </summary>
    /// <param name="rewardCount"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public float GetReward(float rewardCount, RewardFlag flag)
    {
        return currReward.GetReward(rewardCount, flag);
    }

    public void NextReward()
    {
        if (stationCode == 0)
        {
            for (int i = 0; i < allReward.Length; i++)
            {
                if (allReward[i].lastReward == currReward)
                {
                    currReward = allReward[i];
                    break;
                }
            }
        }
        else
        {
            currReward = allReward[0];
        }

        SaveToLocal();
    }

    public void RefreshRedPoint()
    {
        onSwitchStation?.Invoke();
    }

    public event Action onSwitchStation;
}