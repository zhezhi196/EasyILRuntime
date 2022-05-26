using System;
using Module;
using Project.Data;

public class WeaponUpdateRewardBag : CommonLimitRewardBag
{
    private float stopTime = 15;
    public bool isUpdate;
    public DateTime LocalToday
    {
        get { return LocalFileMgr.GetDateTime("Limit" + iap.dbData.ID); }
        set { LocalFileMgr.SetDateTime("Limit" + iap.dbData.ID, TimeHelper.today); }
    }
    public WeaponUpdateRewardBag(IapSqlData sqlData) : base(sqlData)
    {
        EventCenter.Register<int,int>(EventKey.OnWeaponUpgrade, UpdateWeapon);
    }

    private void UpdateWeapon(int arg1, int arg2)
    {
        isUpdate = true;
    }

    public WeaponUpdateRewardBag(Iap iap) : base(iap)
    {
    }

    public override void OnExitBattle()
    {
        EventCenter.Register<int,int>(EventKey.OnWeaponUpgrade, UpdateWeapon);
    }

    public override void TryTrigger()
    {
        if (UIController.Instance.currentUI != null && UIController.Instance.currentUI.winName == "WeaponUpgradeUI")
        {
            if (UIController.Instance.currentUI.viewBase != null &&
                UIController.Instance.currentUI.viewBase.model.time >= stopTime && !isUpdate)
            {
                if (TimeHelper.today > LocalToday)
                {
                    Trigger();
                    LocalToday = TimeHelper.today;
                }
            }
        }
        else
        {
            isUpdate = false;
        }
    }
}