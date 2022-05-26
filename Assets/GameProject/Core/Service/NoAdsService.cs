using Module;
using Project.Data;

public class NoAdsService : GameService
{
    protected NoAdsService(ServiceData data, ServiceSaveData saveData) : base(data, saveData)
    {
        if (saveData != null)
        {
            if (saveData.deadLine.ToDateTime() <= TimeHelper.now)
            {
                DataMgr.Instance.GetSqlService<ServiceSaveData>().Delete(sd => sd.targetId == dbData.ID);
            }
            else
            {
                GetReward(1, 0);
            }
        }
    }
    
    public override float GetReward(float rewardCount, RewardFlag flag)
    {
        AddGlobalService(ServiceType.NoAds);
        return 1;
    }

    public static bool CanIgnoreAds(Iap iap)
    {
        return iap is AdsIap && ContainService(ServiceType.NoAds);
    }

}