using Module;

public class PlayerSkillCtrl : SkillCtrl, IMonsterHurtObject
{
    public PlayerSkillCtrl(ISkillObject owner) : base(owner)
    {
    }

    public MonsterDamage OnHurt(MonsterDamage damage)
    {
        for (int i = 0; i < allSkill.Count; i++)
        {
            if (allSkill[i] is IMonsterHurtObject instance)
            {
                damage = instance.OnHurt(damage);
            }

            if (damage.damage == 0) return damage;
        }

        return damage;
    }
}