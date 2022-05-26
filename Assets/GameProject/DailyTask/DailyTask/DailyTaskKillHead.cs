using Project.Data;

public class DailyTaskKillHead : DailyTaskBase
{
    public DailyTaskKillHead(DailyTaskData dbData, int index) : base(dbData, index)
    {
    }

    public DailyTaskKillHead(DailyTaskData dbData, DailyTaskSaveData saveData) : base(dbData, saveData)
    {
    }

    public override bool IsAchieve(params object[] obj)
    {
        HeadPart part = obj[0] as HeadPart;
        return part != null && !part.monster.isAlive;
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
        if (deadType == DeadType.Shot)
        {
            TryAddCount(1, part);
        }
    }
}