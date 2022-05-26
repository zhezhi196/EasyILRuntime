using Module;
using UnityEngine;
[SkillDescript("将军/挥击")]
public class GeneralAttack : MeleeAttack
{
    protected override string animation => "gongji";

    public override Transform damagePoint
    {
        get
        {
            return ((General) monster).attackPoint;
        }
    }
}