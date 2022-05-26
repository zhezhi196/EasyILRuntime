using Sirenix.OdinInspector;
using Module;
public struct GameAttribute : IAttribute<FloatField>
{
    [LabelText("生命值")][ShowInInspector] public FloatField hp { get; set; }
    [LabelText("武器攻击力")][ShowInInspector] public FloatField gunAtt { get; set; }
    [LabelText("移动速度")][ShowInInspector] public FloatField moveSpeed { get; set; }
    [LabelText("武器爆头攻击力系数")][ShowInInspector] public FloatField gunHeadAttK { get; set; }
    [LabelText("近战武器攻击力")][ShowInInspector] public FloatField meleeAtt { get; set; }
    [LabelText("硬直")][ShowInInspector] public FloatField paralysis { get; set; }
    [LabelText("体力")][ShowInInspector] public FloatField strength { get; set; }
    [LabelText("能量")][ShowInInspector] public FloatField energy { get; set; }
    [LabelText("暴击概率")][ShowInInspector] public FloatField violentAttP { get; set; }
    [LabelText("暴击伤害系数")][ShowInInspector] public FloatField violentAttK { get; set; }
    [LabelText("下蹲移动系数")][ShowInInspector] public FloatField crouchMovek { get; set; }
    [LabelText("瞄准移动系数")][ShowInInspector] public FloatField aimMoveK { get; set; }
    [LabelText("奔跑移动系数")][ShowInInspector] public FloatField runMoveK { get; set; }
    [LabelText("硬直暴击率")][ShowInInspector] public FloatField hardAttP { get; set; }
    [LabelText("硬直暴击系数")][ShowInInspector] public FloatField hardAttK { get; set; }
    [LabelText("射击间隔")][ShowInInspector] public FloatField shotInterval { get; set; }
    [LabelText("弹夹容量")][ShowInInspector][AttributeName("203")] public FloatField bulletCount { get; set; }
    [LabelText("子弹上限")][ShowInInspector] public FloatField bulletBag { get; set; }
    [LabelText("精准度")][ShowInInspector] public FloatField accurate { get; set; }
    [LabelText("后坐力")][ShowInInspector][AttributeName("204")] public FloatField recoilForce { get; set; }
    [LabelText("攻击范围")][ShowInInspector][AttributeName("205")] public FloatField attRange { get; set; }
    [LabelText("暗杀伤害")][ShowInInspector] public FloatField assDamage  { get; set; }
    public GameAttribute(IAttribute<float> target)
    {
        this.hp = new FloatField(target.hp,FieldAesType.Xor);
        this.gunAtt = new FloatField(target.gunAtt,FieldAesType.Xor);
        this.moveSpeed = new FloatField(target.moveSpeed,FieldAesType.Xor);
        this.gunHeadAttK = new FloatField(target.gunHeadAttK,FieldAesType.Xor);
        this.meleeAtt = new FloatField(target.meleeAtt,FieldAesType.Xor);
        this.paralysis = new FloatField(target.paralysis,FieldAesType.Xor);
        this.strength = new FloatField(target.strength,FieldAesType.Xor);
        this.energy = new FloatField(target.energy,FieldAesType.Xor);
        this.violentAttP = new FloatField(target.violentAttP,FieldAesType.Xor);
        this.violentAttK = new FloatField(target.violentAttK,FieldAesType.Xor);
        this.crouchMovek = new FloatField(target.crouchMovek,FieldAesType.Xor);
        this.aimMoveK = new FloatField(target.aimMoveK,FieldAesType.Xor);
        this.runMoveK = new FloatField(target.runMoveK,FieldAesType.Xor);
        this.hardAttP = new FloatField(target.hardAttP,FieldAesType.Xor);
        this.hardAttK = new FloatField(target.hardAttK,FieldAesType.Xor);
        this.shotInterval = new FloatField(target.shotInterval,FieldAesType.Xor);
        this.bulletCount = new FloatField(target.bulletCount,FieldAesType.Xor);
        this.bulletBag = new FloatField(target.bulletBag,FieldAesType.Xor);
        this.accurate = new FloatField(target.accurate,FieldAesType.Xor);
        this.recoilForce = new FloatField(target.recoilForce,FieldAesType.Xor);
        this.attRange = new FloatField(target.attRange,FieldAesType.Xor);
        this.assDamage  = new FloatField(target.assDamage ,FieldAesType.Xor);
    }
    public GameAttribute(float value)
    {
        this.hp = new FloatField(value,FieldAesType.Xor);
        this.gunAtt = new FloatField(value,FieldAesType.Xor);
        this.moveSpeed = new FloatField(value,FieldAesType.Xor);
        this.gunHeadAttK = new FloatField(value,FieldAesType.Xor);
        this.meleeAtt = new FloatField(value,FieldAesType.Xor);
        this.paralysis = new FloatField(value,FieldAesType.Xor);
        this.strength = new FloatField(value,FieldAesType.Xor);
        this.energy = new FloatField(value,FieldAesType.Xor);
        this.violentAttP = new FloatField(value,FieldAesType.Xor);
        this.violentAttK = new FloatField(value,FieldAesType.Xor);
        this.crouchMovek = new FloatField(value,FieldAesType.Xor);
        this.aimMoveK = new FloatField(value,FieldAesType.Xor);
        this.runMoveK = new FloatField(value,FieldAesType.Xor);
        this.hardAttP = new FloatField(value,FieldAesType.Xor);
        this.hardAttK = new FloatField(value,FieldAesType.Xor);
        this.shotInterval = new FloatField(value,FieldAesType.Xor);
        this.bulletCount = new FloatField(value,FieldAesType.Xor);
        this.bulletBag = new FloatField(value,FieldAesType.Xor);
        this.accurate = new FloatField(value,FieldAesType.Xor);
        this.recoilForce = new FloatField(value,FieldAesType.Xor);
        this.attRange = new FloatField(value,FieldAesType.Xor);
        this.assDamage  = new FloatField(value,FieldAesType.Xor);
    }
    public static GameAttribute operator +(GameAttribute left, GameAttribute right)
    {
        GameAttribute result = new GameAttribute();
        result.hp = left.hp + right.hp;
        result.gunAtt = left.gunAtt + right.gunAtt;
        result.moveSpeed = left.moveSpeed + right.moveSpeed;
        result.gunHeadAttK = left.gunHeadAttK + right.gunHeadAttK;
        result.meleeAtt = left.meleeAtt + right.meleeAtt;
        result.paralysis = left.paralysis + right.paralysis;
        result.strength = left.strength + right.strength;
        result.energy = left.energy + right.energy;
        result.violentAttP = left.violentAttP + right.violentAttP;
        result.violentAttK = left.violentAttK + right.violentAttK;
        result.crouchMovek = left.crouchMovek + right.crouchMovek;
        result.aimMoveK = left.aimMoveK + right.aimMoveK;
        result.runMoveK = left.runMoveK + right.runMoveK;
        result.hardAttP = left.hardAttP + right.hardAttP;
        result.hardAttK = left.hardAttK + right.hardAttK;
        result.shotInterval = left.shotInterval + right.shotInterval;
        result.bulletCount = left.bulletCount + right.bulletCount;
        result.bulletBag = left.bulletBag + right.bulletBag;
        result.accurate = left.accurate + right.accurate;
        result.recoilForce = left.recoilForce + right.recoilForce;
        result.attRange = left.attRange + right.attRange;
        result.assDamage  = left.assDamage  + right.assDamage ;
        return result;
    }
    public static GameAttribute operator +(float left, GameAttribute right)
    {
        return new GameAttribute(left) + right;
    }
    public static GameAttribute operator +(GameAttribute left, float right)
    {
        return new GameAttribute(right) + left;
    }
    public static GameAttribute operator *(GameAttribute left, GameAttribute right)
    {
        GameAttribute result = new GameAttribute();
        result.hp = left.hp * right.hp;
        result.gunAtt = left.gunAtt * right.gunAtt;
        result.moveSpeed = left.moveSpeed * right.moveSpeed;
        result.gunHeadAttK = left.gunHeadAttK * right.gunHeadAttK;
        result.meleeAtt = left.meleeAtt * right.meleeAtt;
        result.paralysis = left.paralysis * right.paralysis;
        result.strength = left.strength * right.strength;
        result.energy = left.energy * right.energy;
        result.violentAttP = left.violentAttP * right.violentAttP;
        result.violentAttK = left.violentAttK * right.violentAttK;
        result.crouchMovek = left.crouchMovek * right.crouchMovek;
        result.aimMoveK = left.aimMoveK * right.aimMoveK;
        result.runMoveK = left.runMoveK * right.runMoveK;
        result.hardAttP = left.hardAttP * right.hardAttP;
        result.hardAttK = left.hardAttK * right.hardAttK;
        result.shotInterval = left.shotInterval * right.shotInterval;
        result.bulletCount = left.bulletCount * right.bulletCount;
        result.bulletBag = left.bulletBag * right.bulletBag;
        result.accurate = left.accurate * right.accurate;
        result.recoilForce = left.recoilForce * right.recoilForce;
        result.attRange = left.attRange * right.attRange;
        result.assDamage  = left.assDamage  * right.assDamage ;
        return result;
    }
    public static GameAttribute operator *(float left, GameAttribute right)
    {
        return new GameAttribute(left) * right;
    }
    public static GameAttribute operator *(GameAttribute left, float right)
    {
        return new GameAttribute(right) * left;
    }
}