using Module;
using Project.Data;

public class DailyTaskBulletConsume: DailyTaskBase
{
    public DailyTaskBulletConsume(DailyTaskData dbData, int index) : base(dbData, index)
    {
    }

    public DailyTaskBulletConsume(DailyTaskData dbData, DailyTaskSaveData saveData) : base(dbData, saveData)
    {
    }
    public override void RegisterEvent()
    {
        EventCenter.Register<Weapon>(EventKey.OnWeaponFire, OnBulletConsume);
    }

    protected virtual void OnBulletConsume(Weapon weapon)
    {
        if (weapon.bulletType == BulletType.Pistol)
        {
            TryAddCount(1);
        }
    }

    public override void UnRegisterEvent()
    {
        EventCenter.UnRegister<Weapon>(EventKey.OnWeaponFire, OnBulletConsume);
    }

    public override bool IsAchieve(params object[] obj)
    {
        return true;
    }
}