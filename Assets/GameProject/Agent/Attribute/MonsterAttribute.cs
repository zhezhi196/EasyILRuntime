using System;
using Sirenix.OdinInspector;
[Serializable]
public struct MonsterAttribute : IMonsterAttribute<float>
{
    [LabelText("生命值")][ShowInInspector] public float hp { get; set; }
    [LabelText("攻击力")][ShowInInspector] public float att { get; set; }
    [LabelText("移动速度")][ShowInInspector] public float moveSpeed { get; set; }
    [LabelText("旋转速度")][ShowInInspector] public float rotateSpeed { get; set; }
    public MonsterAttribute(IMonsterAttribute<float> target)
    {
        this.hp = target.hp;
        this.att = target.att;
        this.moveSpeed = target.moveSpeed;
        this.rotateSpeed = target.rotateSpeed;
    }
    public MonsterAttribute(float value)
    {
        this.hp = value;
        this.att = value;
        this.moveSpeed = value;
        this.rotateSpeed = value;
    }
    public static MonsterAttribute operator +(MonsterAttribute left, MonsterAttribute right)
    {
        MonsterAttribute result = new MonsterAttribute();
        result.hp = left.hp + right.hp;
        result.att = left.att + right.att;
        result.moveSpeed = left.moveSpeed + right.moveSpeed;
        result.rotateSpeed = left.rotateSpeed + right.rotateSpeed;
        return result;
    }
    public static MonsterAttribute operator +(float left, MonsterAttribute right)
    {
        return new MonsterAttribute(left) + right;
    }
    public static MonsterAttribute operator +(MonsterAttribute left, float right)
    {
        return new MonsterAttribute(right) + left;
    }
    public static MonsterAttribute operator *(MonsterAttribute left, MonsterAttribute right)
    {
        MonsterAttribute result = new MonsterAttribute();
        result.hp = left.hp * right.hp;
        result.att = left.att * right.att;
        result.moveSpeed = left.moveSpeed * right.moveSpeed;
        result.rotateSpeed = left.rotateSpeed * right.rotateSpeed;
        return result;
    }
    public static MonsterAttribute operator *(float left, MonsterAttribute right)
    {
        return new MonsterAttribute(left) * right;
    }
    public static MonsterAttribute operator *(MonsterAttribute left, float right)
    {
        return new MonsterAttribute(right) * left;
    }

    public float GetMissionHp()
    {
        return hp;
    }
}