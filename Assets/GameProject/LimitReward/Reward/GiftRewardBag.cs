using System;
using Module;
using Project.Data;

public class GiftRewardBag : CommonLimitRewardBag
{
    private float stopTime = 15;

    public DateTime LocalToday
    {
        get { return LocalFileMgr.GetDateTime("Limit" + iap.dbData.ID); }
        set { LocalFileMgr.SetDateTime("Limit" + iap.dbData.ID, TimeHelper.today); }
    }

    public GiftRewardBag(IapSqlData sqlData) : base(sqlData)
    {
    }

    public GiftRewardBag(Iap iap) : base(iap)
    {
    }

    public override void TryTrigger()
    {
        if (UIController.Instance.currentUI != null &&UIController.Instance.currentUI.winName == "GiftUI")
        {
            if (UIController.Instance.currentUI.viewBase != null && UIController.Instance.currentUI.viewBase.model.time >= stopTime)
            {
                if (TimeHelper.today > LocalToday)
                {
                    Trigger();
                    LocalToday = TimeHelper.today;
                }
            }
        }
    }
}