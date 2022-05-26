using Module;
using UnityEngine;
[SkillDescript("肉瘤/吐刺")]
public class TuCi : MeleeAttack
{
    protected override string animation
    {
        get
        {
            return "tumour@tuci";
        }
    }

    public override Transform damagePoint
    {
        get
        {
            return ((Tumour) monster).fogFirePoint;
        }
    }
}