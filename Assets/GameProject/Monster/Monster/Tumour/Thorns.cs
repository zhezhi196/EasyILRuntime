using Module;
using UnityEngine;

[SkillDescript("肉瘤/海胆刺")]
public class Thorns : MeleeAttack
{
    public override bool isReadyRelease
    {
        get { return isWanted && base.isReadyRelease; }
    }

    public override bool isWanted
    {
        get
        {
            return isActive && station == SkillStation.Ready && monster.toPlayerDistance <= maxDistance && monster.toPlayerDistance >= minDistance;
        }
    }

    protected override string animation => "tumour@haidanci";
    public override Transform damagePoint
    {
        get
        {
            return ((Tumour) monster).centerPoint;
        }
    }
}