using System;
using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;

public class AchievementManager : Singleton<AchievementManager>
{
    public event Action<AchievementBase,AchievementReward> onChangeReward;

    private List<AchievementBase> _achievementList = new List<AchievementBase>();

    public List<AchievementBase> Achievements => _achievementList;

    public int completeCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < _achievementList.Count; i++)
            {
                count += _achievementList[i].count;
            }

            return count;
        }
    }

    public AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        process.IsDone = false;
        List<AchievementData> dbData = DataMgr.Instance.GetSqlService<AchievementData>().tableList;
        for (int i = 0; i < dbData.Count; i++)
        {
            var tar = CreateAchievement(dbData[i]);
            if (tar != null)
            {
                _achievementList.Add(tar);
            }
        }

        for (int i = 0; i < _achievementList.Count; i++)
        {
            _achievementList[i].InitPreReward();
        }
        
        process.SetComplete();
        return process;
    }


    public AchievementReward FindReward(int id)
    {
        for (int i = 0; i < _achievementList.Count; i++)
        {
            for (int j = 0; j < _achievementList[i].allReward.Length; j++)
            {
                if (_achievementList[i].allReward[j].dbData.ID == id) return _achievementList[i].allReward[j];
            }
        }

        return null;
    }

    public void TryAchieve(Type type, int count, params object[] obj)
    {
        for (int i = 0; i < _achievementList.Count; i++)
        {
            if (_achievementList[i].GetType() == type)
            {
                _achievementList[i].TryAddCount(count, obj);
            }
        }
    }

    public AchievementBase CreateAchievement(AchievementData dbData)
    {
        for (int i = 0; i < _achievementList.Count; i++)
        {
            if (_achievementList[i].GetType().Name == dbData.field)
            {
                return null;
            }
        }

        Type type = Type.GetType("Ach." + dbData.field);
        if (type != null)
        {
            AchievementSaveData saveData = DataMgr.Instance.GetSqlService<AchievementSaveData>()
                .Where(st => st.type == dbData.field);
            return Activator.CreateInstance(type, saveData) as AchievementBase;
        }

        return null;
    }

    public void OnChangeReward(AchievementBase achieve , AchievementReward reward)
    {
        onChangeReward?.Invoke(achieve , reward);
    }


    /// <summary>
    /// GM调用，完成所有成就
    /// </summary>
    public void GMUnlockAchievement()
    {
        for (int i = 0; i < _achievementList.Count; i++)
        {
            _achievementList[i].currReward.Complete();
        }
    }

}