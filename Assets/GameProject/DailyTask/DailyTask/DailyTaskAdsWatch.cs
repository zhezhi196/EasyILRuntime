using Module;
using Project.Data;

/// <summary>
/// 观看广告的每日任务
/// </summary>
public class DailyTaskAdsWatch: DailyTaskBase
{
    public DailyTaskAdsWatch(DailyTaskData dbData, int index) : base(dbData, index)
    {
    }

    public DailyTaskAdsWatch(DailyTaskData dbData, DailyTaskSaveData saveData) : base(dbData, saveData)
    {
    }

    public override void RegisterEvent()
    {
        RewardBag.OnGetRewardBag += OnGetReward;
    }

    public override void UnRegisterEvent()
    {
        RewardBag.OnGetRewardBag += OnGetReward;
    }

    private void OnGetReward(RewardBag obj)
    {
        if (obj.iap is AdsIap)
        {
            TryAddCount(1, obj.iap);
        }
    }

    public override bool IsAchieve(params object[] obj)
    {
        AdsIap iap=obj[0] as AdsIap;
        return iap != null;
    }
}