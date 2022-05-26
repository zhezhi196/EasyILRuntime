using System;
using Module;
using Project.Data;
using UnityEngine;

public enum BulletCreatType
{
    Single,
    Group
}
public enum BulletType
{
    Pistol = 1,
    ShotGun,
    Arrow,
    Mp5,
    Birck,
}

[RequireComponent(typeof(Weapon))]
public class BulletEntity : PropEntity
{
    public static event Action<BulletEntity,int> onBulletCountChanged;

    public static int OwnBullet(BulletType bulletType, BulletCreatType type, int count)
    {
        return BattleController.GetCtrl<BulletCtrl>().GetBullet(bulletType).OwnBullet(type, count);
    }
    public int bagCount => BattleController.GetCtrl<BagPackCtrl>().GetBagItemNum(this);

    private Weapon _weapon;

    public Weapon weapon
    {
        get
        {
            if (_weapon == null)
            {
                var weaponManager = Player.player.weaponManager;
                for (int i = 0; i < weaponManager.weaponList.Count; i++)
                {
                    if (weaponManager.weaponList[i].bulletType == this.bulletType)
                    {
                        _weapon = weaponManager.weaponList[i];
                        break;
                    }
                }
            }
            return _weapon;
        }
    }

    public override int stationCode
    {
        get
        {
            if (bagCount >= maxCount) return 2;
            return 0;
        }
    }

    public BulletType bulletType { get; }
    public BulletData bulletData { get; }
    public int maxCount => weapon.weaponAttribute.bulletBag.value.ToInt();
    
    public BulletEntity(PropData dbData) : base(dbData)
    {
        bulletData = DataMgr.Instance.GetSqlService<BulletData>().Where(bullet => bullet.propId == dbData.ID);
        bulletType = (BulletType) bulletData.type;
    }

    public int Get(int needCount)
    {
        var temp = BattleController.GetCtrl<BagPackCtrl>().ConsumeItem(this, needCount);
        onBulletCountChanged?.Invoke(this, -temp);
        BattleController.Instance.Save(0);
        return temp;
    }
    
    public int OwnBullet(BulletCreatType type, int count)
    {
        int oldCount = this.bagCount;
        int currCount = Mathf.Clamp(oldCount + (type == BulletCreatType.Group ? (bulletData.createCount * count) : count), 0, maxCount);
        return (int)GetReward(currCount - oldCount, RewardFlag.NoAudio);
    }

    public override float GetReward(float rewardCount,int editorid, string[] match, RewardFlag flag)
    {
        var current = 0;
        BagPackCtrl bagCtrl = BattleController.GetCtrl<BagPackCtrl>();
        if (bagCtrl != null)
        {
            if (!bagCtrl.bagList.IsNullOrEmpty())
            {
                var tarEntity = bagCtrl.bagList.Find(fd => fd.entity == this);
                if (tarEntity != null)
                {
                    current = tarEntity.count;
                }
            }
        }

        var tarCount = current + rewardCount;
        var realAddCount = Mathf.Clamp(tarCount, current, maxCount);
        float resu = base.GetReward(realAddCount - current, editorid, match, flag);
        if (resu >= 0.9f)
        {
            if (BattleController.Instance.ctrlProcedure.isStartFight)
            {
                BattleController.Instance.Save(0);
            }
            onBulletCountChanged?.Invoke(this, (int)resu);
        }
        else
        {
            if (dbData.ID != DataMgr.CommonData(33015).ToInt())
            {
                CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("704"), null,
                    new PupupOption() {title = Language.GetContent("702")});
            }
            else
            {
                CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("723"), null,
                    new PupupOption() {title = Language.GetContent("702")});
            }

        }
        return resu;
    }

    public override string GetText(string type)
    {
        if (type == TypeList.connotGet)
        {
            return Language.GetContent("1912");
        }
        else
        {
            return base.GetText(type);
        }
        
    }
    
    public void GMSetBulletCount(int count)
    {
#if LOG_ENABLE
        BagPackCtrl bagCtrl = BattleController.GetCtrl<BagPackCtrl>();
        if (bagCtrl != null)
        {
            bagCtrl.PutToBag(this, count, 0,null, PutBagFlag.NotAudio);
        }
        
        if (BattleController.Instance.ctrlProcedure.isStartFight)
        {
            BattleController.Instance.Save(0);
        }
        onBulletCountChanged?.Invoke(this, count);
#endif
       
    }
}