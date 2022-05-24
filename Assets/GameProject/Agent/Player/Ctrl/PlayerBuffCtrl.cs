using Module;

public class PlayerBuffCtrl : BuffCtrl, IMonsterHurtObject
{
    public PlayerBuffCtrl(IBuffObject owner) : base(owner)
    {
    }

    public MonsterDamage OnHurt(MonsterDamage damage)
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i] is PlayerBuff pb)
            {
                damage = pb.OnHurt(damage);
            }

            if (damage.damage == 0) return damage;
        }

        return damage;
    }
}