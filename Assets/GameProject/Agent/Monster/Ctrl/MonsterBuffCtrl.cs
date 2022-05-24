using Module;

public class MonsterBuffCtrl : BuffCtrl, IHurtObject
{
    public MonsterBuffCtrl(IBuffObject owner) : base(owner)
    {
    }

    public Damage OnHurt(Damage damage)
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i] is IHurtObject hurt)
            {
                damage = hurt.OnHurt(damage);
            }

            if (damage.damage == 0) return damage;
        }

        return damage;
    }
}