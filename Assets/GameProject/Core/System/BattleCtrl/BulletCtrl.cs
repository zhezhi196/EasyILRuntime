using System;
using System.Collections.Generic;
using Module;

public class BulletCtrl : BattleSystem
{
    public List<BulletEntity> bulletList = new List<BulletEntity>();
    public RunTimeAction initBullet;
    public event Action<BulletEntity> onBulletCountChanged; 

    public override void OnRestart(EnterNodeType enterType)
    {
        initBullet = new RunTimeAction(() =>
        {
            bulletList.Clear();
            for (int i = 0; i < PropEntity.entityList.Count; i++)
            {
                if (PropEntity.entityList[i] is BulletEntity bulletEntity)
                {
                    bulletList.Add(bulletEntity);
                }
            }

            BattleController.Instance.NextFinishAction("initBullet");
            initBullet = null;
        });
    }

    public BulletEntity GetBullet(BulletType type)
    {
        for (int i = 0; i < bulletList.Count; i++)
        {
            if (bulletList[i].bulletType == type)
            {
                return bulletList[i];
            }
        }

        return null;
    }
}