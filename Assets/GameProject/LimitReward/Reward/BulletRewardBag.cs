using System;
using Module;
using Project.Data;

public class BulletRewardBag : CommonLimitRewardBag
{
    public BulletRewardBag(IapSqlData sqlData) : base(sqlData)
    {
    }

    public BulletRewardBag(Iap iap) : base(iap)
    {
    }

    public override void TryTrigger()
    {
        PropEntity entity=PropEntity.GetEntity(20012);
        if (entity.stationCode != 0)
        {
            return;
        }
        var bag = BattleController.GetCtrl<BagPackCtrl>().bagList;
        for (int i = 0; i < bag.Count; i++)
        {
            if (bag[i].entity is BulletEntity)
            {
                if (bag[i].count > 0)
                {
                    return;
                }
            }
        }
        var ownWeapon = Player.player.weaponManager.ownWeapon;
        for (int i = 0; i < ownWeapon.Count; i++)
        {
            if (ownWeapon[i].bulletCount > 0 || ownWeapon[i].bullet?.bagCount > 0)
            {
                return;
            }
        }
        Trigger();

    }
}