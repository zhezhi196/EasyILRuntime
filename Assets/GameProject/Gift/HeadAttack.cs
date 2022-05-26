using Project.Data;
using Module;

namespace GameGift
{
    /// <summary>
    /// 致残射击
    /// 枪械爆头市，虚弱怪物3秒，怪物攻击，移速降低
    /// </summary>
    public class HeadAttack : Gift
    {
        float arg;
        float arg1;//伤害
        float arg2;//移速
        public HeadAttack(GiftData data, GiftSaveData saveData) : base(data, saveData)
        {
            arg = data.giftArg1;
            arg1 = data.giftArg2;
            arg2 = data.giftArg3;
        }
        public override void ActivateGife()
        {
            //注册攻击事件
            EventCenter.Register<bool, MonsterPart>(EventKey.HitMonster, OnHurtMonster);
        }
        private void OnHurtMonster(bool death, MonsterPart part)
        {
            //不是近战武器击中怪物头部
            if (part.partType == MonsterPartType.Head && Player.player.currentWeapon.weaponType != WeaponType.MeleeWeapon)
            {
                part.monster.Week(arg, arg1, arg2);
            }
        }
        public override void ExitBattle()
        {
            //注销攻击事件
            EventCenter.UnRegister<bool, MonsterPart>(EventKey.HitMonster, OnHurtMonster);
        }
    }
}