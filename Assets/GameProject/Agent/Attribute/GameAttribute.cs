using System;
using Sirenix.OdinInspector;
using Module;
[Serializable]
public struct GameAttribute : IAttribute<FloatField>
{
    [LabelText("生命值")][ShowInInspector] public FloatField hp { get; set; }
    [LabelText("攻击力")][ShowInInspector] public FloatField att { get; set; }
    [LabelText("移动速度")][ShowInInspector] public FloatField moveSpeed { get; set; }
    [LabelText("最大怒气值")][ShowInInspector] public FloatField anger { get; set; }
    [LabelText("暴击率")][ShowInInspector] public FloatField critical { get; set; }
    [LabelText("暴击伤害")][ShowInInspector] public FloatField criticalDamage { get; set; }
    [LabelText("旋转速度")][ShowInInspector] public FloatField rotateSpeed { get; set; }
    [LabelText("怒气圈半径")][ShowInInspector] public FloatField angerRadius { get; set; }
    [LabelText("怒气增长速度")][ShowInInspector] public FloatField angerUpSpeed { get; set; }
    [LabelText("怒气下降速度")][ShowInInspector] public FloatField angerDownSpeed { get; set; }
    [LabelText("狂暴时间")][ShowInInspector] public FloatField angryTime { get; set; }
    public GameAttribute(IAttribute<float> target)
    {
        this.hp = new FloatField(target.hp,FieldAesType.Xor);
        this.att = new FloatField(target.att,FieldAesType.Xor);
        this.moveSpeed = new FloatField(target.moveSpeed,FieldAesType.Xor);
        this.anger = new FloatField(target.anger,FieldAesType.Xor);
        this.critical = new FloatField(target.critical,FieldAesType.Xor);
        this.criticalDamage = new FloatField(target.criticalDamage,FieldAesType.Xor);
        this.rotateSpeed = new FloatField(target.rotateSpeed,FieldAesType.Xor);
        this.angerRadius = new FloatField(target.angerRadius,FieldAesType.Xor);
        this.angerUpSpeed = new FloatField(target.angerUpSpeed,FieldAesType.Xor);
        this.angerDownSpeed = new FloatField(target.angerDownSpeed,FieldAesType.Xor);
        this.angryTime = new FloatField(target.angryTime,FieldAesType.Xor);
    }
    public GameAttribute(float value)
    {
        this.hp = new FloatField(value,FieldAesType.Xor);
        this.att = new FloatField(value,FieldAesType.Xor);
        this.moveSpeed = new FloatField(value,FieldAesType.Xor);
        this.anger = new FloatField(value,FieldAesType.Xor);
        this.critical = new FloatField(value,FieldAesType.Xor);
        this.criticalDamage = new FloatField(value,FieldAesType.Xor);
        this.rotateSpeed = new FloatField(value,FieldAesType.Xor);
        this.angerRadius = new FloatField(value,FieldAesType.Xor);
        this.angerUpSpeed = new FloatField(value,FieldAesType.Xor);
        this.angerDownSpeed = new FloatField(value,FieldAesType.Xor);
        this.angryTime = new FloatField(value,FieldAesType.Xor);
    }
    public static GameAttribute operator +(GameAttribute left, GameAttribute right)
    {
        GameAttribute result = new GameAttribute();
        result.hp = left.hp + right.hp;
        result.att = left.att + right.att;
        result.moveSpeed = left.moveSpeed + right.moveSpeed;
        result.anger = left.anger + right.anger;
        result.critical = left.critical + right.critical;
        result.criticalDamage = left.criticalDamage + right.criticalDamage;
        result.rotateSpeed = left.rotateSpeed + right.rotateSpeed;
        result.angerRadius = left.angerRadius + right.angerRadius;
        result.angerUpSpeed = left.angerUpSpeed + right.angerUpSpeed;
        result.angerDownSpeed = left.angerDownSpeed + right.angerDownSpeed;
        result.angryTime = left.angryTime + right.angryTime;
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
        result.att = left.att * right.att;
        result.moveSpeed = left.moveSpeed * right.moveSpeed;
        result.anger = left.anger * right.anger;
        result.critical = left.critical * right.critical;
        result.criticalDamage = left.criticalDamage * right.criticalDamage;
        result.rotateSpeed = left.rotateSpeed * right.rotateSpeed;
        result.angerRadius = left.angerRadius * right.angerRadius;
        result.angerUpSpeed = left.angerUpSpeed * right.angerUpSpeed;
        result.angerDownSpeed = left.angerDownSpeed * right.angerDownSpeed;
        result.angryTime = left.angryTime * right.angryTime;
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