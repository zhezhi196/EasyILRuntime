using Project.Data;

public class DailyTaskMummyKiller: DailyTaskBase
{
    public DailyTaskMummyKiller(DailyTaskData dbData, int index) : base(dbData, index)
    {
    }

    public DailyTaskMummyKiller(DailyTaskData dbData, DailyTaskSaveData saveData) : base(dbData, saveData)
    {
    }

    public override bool IsAchieve(params object[] obj)
    {
        //TODO 
         Monster monster = obj[0] as Monster;
         if (monster != null && monster is Dog)
         {
             return !monster.isAlive;
         }

        return false;
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
}