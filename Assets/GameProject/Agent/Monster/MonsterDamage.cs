using System;

/// <summary>
/// 能被怪伤害的对象
/// </summary>
public interface IMonsterHurtObject
{
    MonsterDamage OnHurt(MonsterDamage damage);
}

public struct MonsterDamage
{
    public int damage;
    
    public static MonsterDamage zero = new MonsterDamage() {damage = 0};
    public static MonsterDamage maxValue = new MonsterDamage() {damage = Int32.MaxValue};
 }