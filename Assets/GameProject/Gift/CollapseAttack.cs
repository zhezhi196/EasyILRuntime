using Project.Data;

namespace GameGift
{
    /// <summary>
    /// 乘胜追击
    /// 提升对倒地敌人的枪械伤害
    /// </summary>
    public class CollapseAttack : Gift
    {
        public CollapseAttack(GiftData data, GiftSaveData saveData) : base(data, saveData)
        {
        }

        public override GameAttribute GetGrowAtt(Weapon weapon, MonsterPart part)
        {
            if (part != null && part.monster.station == MonsterStation.Paralysis)
            {
                return growUpAttribute;
            }
            return base.GetGrowAtt(weapon, part);
        }
    }
}