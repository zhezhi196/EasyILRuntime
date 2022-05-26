using System;
using Module;
using Project.Data;

public class LifeRewardBag : CommonLimitRewardBag
{
    public LifeRewardBag(IapSqlData sqlData) : base(sqlData)
    {
    }

    public LifeRewardBag(Iap iap) : base(iap)
    {
    }

    public override void TryTrigger()
    {
        PropEntity entity=PropEntity.GetEntity(20032);
        if (BattleController.GetCtrl<BagPackCtrl>().GetBagItemNum(entity) != 0)
        {
            return;
        }
        Trigger();
    }
}