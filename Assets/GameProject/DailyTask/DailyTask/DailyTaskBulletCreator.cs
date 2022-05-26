using Module;
using Project.Data;

public class DailyTaskBulletCreator: DailyTaskBase
{
    public DailyTaskBulletCreator(DailyTaskData dbData, int index) : base(dbData, index)
    {
    }

    public DailyTaskBulletCreator(DailyTaskData dbData, DailyTaskSaveData saveData) : base(dbData, saveData)
    {
    }

    public override bool IsAchieve(params object[] obj)
    {
        return true;
    }
        
                
    public override void RegisterEvent()
    {
        EventCenter.Register<int>(EventKey.BulletCreat, OnBulletCreat);
    }
    
    public override void UnRegisterEvent()
    {
        EventCenter.UnRegister<int>(EventKey.BulletCreat, OnBulletCreat);
    }

    private void OnBulletCreat(int obj)
    {
        if (obj > 0)
        {
            TryAddCount(1);
        }
    }


}