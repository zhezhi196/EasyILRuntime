using Module;
using UnityEngine;
[SkillDescript("肉瘤/吐刺")]
public class TuCi : MeleeAttack
{
    public Tumour tumour
    {
        get
        {
            return owner as Tumour;
        }
    }

    public override float animationSpeed
    {
        get
        {
            return 0.7f;
        }
    }

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
            return ((Tumour) monster).tuciAttackPoint;
        }
    }

    protected override void OnReleaseStart()
    {
        base.OnReleaseStart();
        tumour.tuciXuewu.gameObject.OnActive(true);
    }

    protected override void OnReleaseEnd(bool complete)
    {
        base.OnReleaseEnd(complete);
        tumour.tuciXuewu.gameObject.OnActive(false);
    }
}