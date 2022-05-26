using Module;
using UnityEngine;
[SkillDescript("普通狗/撕咬")]
public class SiYao : MeleeAttack
{

    protected override string animation => "Siyao";


    public override Transform damagePoint
    {
        get
        {
            return ((Dog) monster).damagePoint;
        }
    }
}