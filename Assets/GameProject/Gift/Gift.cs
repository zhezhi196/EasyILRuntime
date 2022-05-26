using Project.Data;
public enum GiftBranchType
{
    Assassination = 1,//潜行
    Fight,//战斗
    Survival,//生存
    Technology//科技
}
public enum GiftType
{ 
    Attribute,//增加属性的天赋
    Buff,//增加buff的天赋
    Skill,//天赋技能
}
public enum GiftStation
{
    Locked,
    Running,
    Owned
}

public enum GiftName
{
    //QuickTransfer = 15001,//快速转移
    //FistPounce =15002,//(铁拳)突袭
    //EnergAbsorb = 15003,//能量汲取
    //Shuttle = 15004,//碎颅打击(原穿梭者)
    //SniperGift = 15005,//狙击手
    //AttackConverted = 15006,//攻击转换
    //SoulAbsoty = 15007,//灵魂汲取
    //Breakthrough = 15008,//突围
    //FistCharge = 15009,//铁拳充能,蓄力
    //DeathBefall = 15010,//死神降临
    //RocketBoxing = 15016,//火箭拳击
    //QuickRecover = 15017,//快速回复
    //LightningPulse = 15018,//闪电脉冲
    //TimeControl = 15019,//时间控制
    //Stealth = 15020,//隐身
    //新天赋
    FastMove = 15001,//快速突袭
    HealthAbsorb = 15003,//生命汲取
    Sneak = 15005,//潜行

    SubCost = 15011,//熟练强化
    GunAttack = 15012,//远近兼备,枪械后，近战伤害加成
    HeadAttack = 15013,//致残攻击，爆头加成
    CollapseAttack = 15014,//乘胜追击，攻击倒地敌人伤害加成
    DoublePistol = 15015,//羽翼射手，手枪双持

    MaxRecover = 15025,

    PerfactDodge = 15031,//完美闪避
    ChargeAttack = 15032,//序列攻击
    DodgeAttack = 15033,//闪避反击，闪避后枪械攻击加成
    DodgeSuppress = 15034,//闪避压制，闪避后近战超重击
    FastDodge = 15035,//灵巧身法，更容易完美闪避
}
namespace GameGift
{
    /// <summary>
    /// GiftType.Attribute 改变基础属性，直接累加计算
    /// GiftType.Buff buff类型天赋，属性增长为百分比增长
    ///  GiftType.Skill 技能类天赋，单独处理
    /// </summary>
    public class Gift
    {
        public GiftData dbData { get; }
        public GiftStation station { get; set; }
        public GiftBranchType branchType
        {
            get { return (GiftBranchType)dbData.branchType; }
        }

        public GiftType giftType
        {
            get { return (GiftType)dbData.giftType; }
        }
        public GameAttribute baseAttribute { get; }
        public GameAttribute growUpAttribute { get; }

        public Gift(GiftData data, GiftSaveData saveData)
        {
            this.dbData = data;
            if (saveData != null)
            {
                this.station = (GiftStation)saveData.station;
            }

            var att = AttributeHelper.GetAttributeByType(dbData);
            baseAttribute = dbData.giftType == 0 ? att[0] : default;
            growUpAttribute = dbData.giftType != 0 ? att[1] : default;
        }

        public virtual void ActivateGife()
        {

        }

        public void Unlock()
        {
            station = GiftStation.Running;
        }

        public virtual GameAttribute GetGrowAtt(Weapon weapon, MonsterPart part)
        {
            return new GameAttribute(0);
        }

        public virtual void ExitBattle()
        {
        }
    }
}