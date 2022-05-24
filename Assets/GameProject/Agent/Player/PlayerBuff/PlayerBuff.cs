using Module;

public abstract class PlayerBuff : Buff, IMonsterHurtObject
{
    public virtual MonsterDamage OnHurt(MonsterDamage damage)
    {
        return damage;
    }
}