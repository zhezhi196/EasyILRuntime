using System;
using Module;
using UnityEngine;

public enum DamageType
{
    /// <summary>
    /// 生命
    /// </summary>
    Hp,
    /// <summary>
    /// 硬直
    /// </summary>
    Lag
}

[Flags]
public enum DamageAttribute
{
    /// <summary>
    /// 暴击
    /// </summary>
    Critical = 1,
    /// <summary>
    /// 无敌
    /// </summary>
    Invincibility = 2,
}


public class Damage
{
    public DamageType type;
    public DamageAttribute attribute;
    public int damage;
    public IHurtObject target;
    public Agent owner;
    public Vector3 position;
    public bool isKilled;

    public void PlayDamageEffect()
    {
        EffectPlay.Play("Hit", position, Vector3.zero);
    }

    public static Damage RandomCritical(Damage damage, GameAttribute attribute)
    {
        if ((damage.attribute & DamageAttribute.Critical) == 0)
        {
            var cristical = attribute.critical.value;
            if (RandomHelper.RandomValue(cristical))
            {
                damage.attribute |= DamageAttribute.Critical;
                damage.damage = (int) (damage.damage * attribute.criticalDamage);
            }
        }

        return damage;
    }

    public static Damage CreatHpDamage(Agent agent, GameAttribute attribute, float plus)
    {
        var att = attribute.att.value * plus;
        Damage damage = new Damage(){damage = (int) att, owner = agent};
        damage.type = DamageType.Hp;
        damage = RandomCritical(damage, attribute);
        return damage;
    }

    public static Damage CreatLagDamage(int lag)
    {
        Damage damage = new Damage();
        damage.damage = lag;
        damage.type = DamageType.Lag;
        damage.attribute = 0;
        return damage;
    }
}