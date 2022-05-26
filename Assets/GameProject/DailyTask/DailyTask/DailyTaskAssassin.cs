using Module;
using Project.Data;

public class DailyTaskAssassin: DailyTaskBase
{
    public DailyTaskAssassin(DailyTaskData dbData, int index) : base(dbData, index)
    {
    }

    public DailyTaskAssassin(DailyTaskData dbData, DailyTaskSaveData saveData) : base(dbData, saveData)
    {
    }

    public override bool IsAchieve(params object[] obj)
    {
        return obj[0].ToBool();
    }
}