using Project.Data;

public class DailyTaskBulletConsume1 : DailyTaskBulletConsume
{
    public DailyTaskBulletConsume1(DailyTaskData dbData, int index) : base(dbData, index)
    {
    }

    public DailyTaskBulletConsume1(DailyTaskData dbData, DailyTaskSaveData saveData) : base(dbData, saveData)
    {
    }
        
    protected override void OnBulletConsume(Weapon weapon)
    {
        if (weapon.bulletType == BulletType.ShotGun)
        {
            TryAddCount(1);
        }
    }
}