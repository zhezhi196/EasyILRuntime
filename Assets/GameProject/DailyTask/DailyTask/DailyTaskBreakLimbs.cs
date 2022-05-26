using Project.Data;

public class DailyTaskBreakLimbs : DailyTaskBase
{
    public override bool IsAchieve(params object[] obj)
    {
        MonsterPart part = obj[0] as MonsterPart;
        return (part.partType == MonsterPartType.Leg || part.partType == MonsterPartType.Arm) &&!part.monster.isAlive;
    }

    public DailyTaskBreakLimbs(DailyTaskData dbData, int index) : base(dbData, index)
    {
    }

    public DailyTaskBreakLimbs(DailyTaskData dbData, DailyTaskSaveData saveData) : base(dbData, saveData)
    {
    }

    public override void RegisterEvent()
    {
        Monster.OnMonsterDead += OnMonsterDead;
    }

    public override void UnRegisterEvent()
    {
        Monster.OnMonsterDead -= OnMonsterDead;
    }

    private void OnMonsterDead(Monster monster, MonsterPart part,DeadType deadType,Damage damage)
    {
        TryAddCount(1, part);
    }
}