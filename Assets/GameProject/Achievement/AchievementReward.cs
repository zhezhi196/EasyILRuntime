using System;
using Module;
using Project.Data;

public class AchievementReward
{
    private TaskStation _station;
    public AchievementData dbData { get; }
    public RewardContent[] reward { get; }
    
    public AchievementBase achievement { get; }

    public AchievementReward lastReward;
    private Action<AchievementReward, TaskStation> _stationChangedl;

    public static event Action<AchievementReward,TaskStation> onStationChanged;

    public TaskStation station
    {
        get { return _station; }
        set
        {
            this._station = value;
            _stationChangedl?.Invoke(this, this._station);
            onStationChanged?.Invoke(this, this.station);
        }
    }

    public bool isComplete
    {
        get { return achievement.currentCount >= dbData.targetCount; }
    }

    public AchievementReward(AchievementData dbData, AchievementBase achievement,AchievementSaveData saveData,Action<AchievementReward,TaskStation> callback)
    {
        this.dbData = dbData;
        this.achievement = achievement;
        string[] rewardId = dbData.reward.Split(ConstKey.Spite0);
        string[] rewardCount = dbData.rewardCount.Split(ConstKey.Spite0);
        reward = new RewardContent[rewardId.Length];
        for (int i = 0; i < reward.Length; i++)
        {
            reward[i] = Commercialize.GetRewardContent(rewardId[i].ToInt(), rewardCount[i].ToLong());
        }

        if (saveData == null || saveData.complete == 0)
        {
            if (isComplete)
            {
                if (saveData.currId <= dbData.ID)
                {
                    station = TaskStation.CompleteUnReward;
                }
                else
                {
                    station = TaskStation.Rewarded;
                }
            }
            else
            {
                station = TaskStation.UnComplete;
            }
        }
        else
        {
            station = TaskStation.Rewarded;
        }
        this._stationChangedl = callback;
    }


    /// <summary>
    /// 获得几倍的奖励
    /// </summary>
    /// <param name="rewardCount"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public float GetReward(float rewardCount, RewardFlag flag)
    {
        for (int i = 0; i < reward.Length; i++)
        {
            reward[i].GetReward(rewardCount, flag);
        }

        this.station = TaskStation.Rewarded;
        AnalyticsEvent.SendEvent(AnalyticsType.AchievementGet, dbData.ID.ToString(),false);
        return rewardCount;
    }

    public void Complete()
    {
        if (this.station == TaskStation.UnComplete)
        {
            this.station = TaskStation.CompleteUnReward;
        }
    }

    public void InitPreReward()
    {
        if (dbData.lastid != 0)
        {
            lastReward = AchievementManager.Instance.FindReward(dbData.lastid);
        }
    }

    public void OnCurrent()
    {
        if (isComplete && station != TaskStation.Rewarded)
        {
            Complete();
        }
    }
}