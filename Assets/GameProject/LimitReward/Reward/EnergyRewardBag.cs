using System;
using Module;
using Project.Data;

public class EnergyRewardBag : CommonLimitRewardBag
{
    public int minEnergy = 30;

    public EnergyRewardBag(IapSqlData sqlData) : base(sqlData)
    {
    }

    public EnergyRewardBag(Iap iap) : base(iap)
    {
    }

    public override void TryTrigger()
    {
        if (Player.player.energy <= minEnergy)
        {
            Trigger();
        }
    }
}