using Module;
using UnityEngine;

[SkillDescript("空手近战囚犯/挥击")]
public class Swing : MeleeAttack
{
    protected override string animation
    {
        get { return "prisoner@zhuaji"; }
    }

    public override Transform damagePoint
    {
        get { return ((Prisoner) monster).damagePoint; }
    }
}