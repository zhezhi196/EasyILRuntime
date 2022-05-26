using Module;

namespace Ach
{
    public class MP5Full: AchievementBase
    {
        public MP5Full(AchievementSaveData saveData) : base(saveData)
        {
        }

        private void OnWeaponUpgrade(int arg1, int arg2)
        {
            if (arg1 == 23005)
            {
                var t = WeaponManager.weaponAllPartDataDic[arg1];
                var weapon = WeaponManager.weaponAllEntitys[arg1];
                if (weapon.level >= t.Count)
                {
                    TryAddCount(1);
                }
            }
        }

        public override void Register()
        {
            EventCenter.Register<int, int>(EventKey.OnWeaponUpgrade, OnWeaponUpgrade);

        }

        public override void UnRegister()
        {
            EventCenter.UnRegister<int, int>(EventKey.OnWeaponUpgrade, OnWeaponUpgrade);
        }

        public override bool IsAchieve(params object[] obj)
        {
            return true;
        }
    }
}