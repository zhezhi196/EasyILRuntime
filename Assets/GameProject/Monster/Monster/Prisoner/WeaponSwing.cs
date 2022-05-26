using Module;
using UnityEngine;

[SkillDescript("武器近战囚犯/挥击")]
public class WeaponSwing : MeleeAttack
{
    protected override string animation
    {
        get { return "Wprisoner@attack"; }
    }

    public override Transform damagePoint
    {
        get { return ((WeaponPrisoner) monster).damagePoint; }
    }
}