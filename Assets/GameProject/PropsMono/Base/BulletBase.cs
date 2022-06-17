using System;
using Module;
using Sirenix.OdinInspector;

public class BulletBase: InteractiveToBag
{
    [LabelText("子弹空了显示")]
    public bool needCountShow;
    public override InterActiveStyle interactiveStyle
    {
        get { return isMaxCount ? InterActiveStyle.noPickUp : InterActiveStyle.Handle; }
    }

    protected override void OnInitLogical(RunLogicalName logical, RunLogicalFlag flag, string senderArg, string[] args)
    {
        base.OnInitLogical(logical, flag, senderArg, args);
        if (needCountShow)
        {
            EventCenter.Register<Weapon, int>(EventKey.WeaponBulletChange, OnBulletChange);
            var e = ((BulletEntity) entity);
            int cnt = e.GetAllBulletCount();
            if (cnt <= 0)
            {
                RunLogicalOnSelf(RunLogicalName.Show);
            }
            else
            {
                RunLogicalOnSelf(RunLogicalName.Hide);
            }
        }
    }
    
    

    private void OnBulletChange(Weapon weapon, int count)
    {
        if (station == PropsStation.Destroyed)
        {
            return;
        }
        else if (weapon.bulletType != ((BulletEntity) entity).bulletType)
        {
            return;
        }
        
        int total = count + ((BulletEntity)entity).bagCount;
        if (total <= 0)
        {
            RunLogicalOnSelf(RunLogicalName.Show);
        }
        else
        {
            RunLogicalOnSelf(RunLogicalName.Hide);
        }
    }
    

    protected override bool OnInteractive(bool fromMonster = false)
    {
        if (isMaxCount)
        {
            return false;
        }
        else
        {
            if (needCountShow)
            {
                entity.GetReward(count, creator.id, creator.matchInfo, 0);
                RunLogicalOnSelf(RunLogicalName.Hide);
                return true;
            }
            return base.OnInteractive();
        }
    }


    public bool isMaxCount
    {
        get
        {
            BulletEntity tempEntity = (BulletEntity) entity;
            return tempEntity.bagCount >= tempEntity.maxCount;
        }
    }
}