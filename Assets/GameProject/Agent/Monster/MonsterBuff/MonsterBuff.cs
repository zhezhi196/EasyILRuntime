using Module;

public abstract class MonsterBuff : Buff, IHurtObject
{
    public override string name { get; }

    public virtual Damage OnHurt(Damage damage)
    {
        return damage;
    }
}