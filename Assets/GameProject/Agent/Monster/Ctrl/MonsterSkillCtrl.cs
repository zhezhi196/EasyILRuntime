using Module;

public class MonsterSkillCtrl : SkillCtrl, IHurtObject
{
    public MonsterSkillCtrl(ISkillObject owner) : base(owner)
    {
    }

    public Damage OnHurt(Damage damage)
    {
        for (int i = 0; i < allSkill.Count; i++)
        {
            var skil = allSkill[i];
            if (skil is IHurtObject instance)
            {
                damage = instance.OnHurt(damage);
            }

            if (damage.damage == 0) return damage;
        }

        return damage;
    }
}