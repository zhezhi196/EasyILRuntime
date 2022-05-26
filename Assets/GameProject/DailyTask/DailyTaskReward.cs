using System;
using Module;
using UnityEngine;

public struct DailyTaskReward
{
    public int difficulte;
    public int completeCount;
    public RewardContent rewardContent;
    public string rewardId;
    public string getDes;


    /// <param name="difficulte"></param>
    /// <param name="rate">rewardRate</param>
    /// <param name="reward">reward</param>
    /// <param name="count">rewardCount的下滑杠的一部分</param>
    /// <param name="taskComplete"></param>
    public DailyTaskReward(int difficulte, string rate, string reward, string count, int taskComplete, string getDes)
    {
        string[] rewardRate = rate.Split(ConstKey.Spite0);
        string[] rewardID = reward.Split(ConstKey.Spite0);
        string[] des = getDes.Split(ConstKey.Spite0);
        string[] rewardCount = count.Split(ConstKey.Spite1);
        int index = RandomHelper.RandomWeight(rewardRate.ToFloatArray());

        this.difficulte = difficulte;
        this.rewardId = rewardID[index];
        this.rewardContent = Commercialize.GetRewardContent(rewardId.ToInt(), rewardCount[index].ToLong());
        this.completeCount = taskComplete;
        this.getDes = des[index];
    }

    public DailyTaskReward(DailyTaskSaveData saveData)
    {
        this.difficulte = saveData.targetDifficulte;
        this.completeCount = saveData.targetComleteCount;
        this.rewardId = saveData.targetRewardID;
        this.rewardContent = Commercialize.GetRewardContent(rewardId.ToInt(), saveData.targetRewardCount);
        this.getDes = saveData.getDes;
    }

    public void GetReward(int dbId,float count)
    {
        rewardContent.GetReward(count, 0);
        RewardUI.OpenRewardUI(rewardContent);
        AnalyticsEvent.SendEvent(AnalyticsType.DailyTaskGet, dbId.ToString(),false);
    }
}