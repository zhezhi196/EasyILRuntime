namespace Ach
{
    public class AssKiller: AchievementBase
    {
        public AssKiller(AchievementSaveData saveData) : base(saveData)
        {
        }

        private void OnMonsterDead(Monster arg1, MonsterPart arg2, DeadType arg3, Damage damage)
        {
            if (!arg1.isAlive && (arg3 == DeadType.Ass1 || arg3 == DeadType.Ass2))
            {
                TryAddCount(1);
            }
        }

        public override void Register()
        {
            Monster.OnMonsterDead += OnMonsterDead;
        }

        public override void UnRegister()
        {
            Monster.OnMonsterDead -= OnMonsterDead;
        }

        public override bool IsAchieve(params object[] obj)
        {
            return true;
        }
    }
}