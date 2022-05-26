using Project.Data;
using Module;

namespace GameGift
{
    /// <summary>
    /// 远近兼备
    /// 枪械攻击后，提升近战伤害
    /// </summary>
    public class GunAttack : Gift   
    {
        float arg = 0;
        public Clock buffClock;
        public GunAttack(GiftData data, GiftSaveData saveData) : base(data, saveData)
        {
            arg = data.giftArg1;
        }

        public override void ActivateGife()
        {
            buffClock = new Clock(arg);
            buffClock.autoKill = false;
            //注册攻击事件
            EventCenter.Register<bool, MonsterPart>(EventKey.HitMonster, OnHurtMonster);
        }

        public override GameAttribute GetGrowAtt(Weapon weapon, MonsterPart part)
        {
            if (buffClock.isActive && weapon.weaponType == WeaponType.MeleeWeapon)
            {
                return growUpAttribute;
            }
            return base.GetGrowAtt(weapon, part);
        }

        private void OnHurtMonster(bool death, MonsterPart part)
        {
            //不是近战武器击中怪物,激活buff
            if (Player.player.currentWeapon.weaponType != WeaponType.MeleeWeapon)
            {
                buffClock.Restart();
            }
        }

        public override void ExitBattle()
        {
            buffClock?.Stop();
            //注销攻击事件
            EventCenter.UnRegister<bool, MonsterPart>(EventKey.HitMonster, OnHurtMonster);
        }
    }
}