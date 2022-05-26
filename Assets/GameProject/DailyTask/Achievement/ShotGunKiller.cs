namespace Ach
{
    public class ShotGunKiller: AchievementBase
    {
        public ShotGunKiller(AchievementSaveData saveData) : base(saveData)
        {
        }

        private void OnMonsterDead(Monster arg1, MonsterPart arg2,DeadType deadType,Damage damage)
        {
            if (!arg1.isAlive && deadType == DeadType.Shot && damage.weapon == WeaponType.ShotGun)
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