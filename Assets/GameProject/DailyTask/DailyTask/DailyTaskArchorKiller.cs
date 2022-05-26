using Project.Data;

public class DailyTaskArchorKiller : DailyTaskBase
{
    public DailyTaskArchorKiller(DailyTaskData dbData, int index) : base(dbData, index)
    {
    }

    public DailyTaskArchorKiller(DailyTaskData dbData, DailyTaskSaveData saveData) : base(dbData, saveData)
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
        TryAddCount(1, monster);
    }

    public override bool IsAchieve(params object[] obj)
    {
        //TODO 
        // Monster monster = obj[0] as Monster;
        // if (monster != null && monster is RemoteCorpse)
        // {
        //     return !monster.isAlive;
        // }

        return false;
    }
}