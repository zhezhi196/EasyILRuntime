using Module;
using Module.SkillAction;
using Sirenix.OdinInspector;
using UnityEngine;

[SkillDescript("肉瘤/翻滚攻击")]
public class Rolling : ChargeSkill
{
    public Tumour tumour
    {
        get
        {
            return (Tumour)owner;
        }
    }
    public override Transform checkAttackPoint
    {
        get
        {
            return monster.centerPoint;
        }
    }

    protected override void OnReleaseStart()
    {
        base.OnReleaseStart();
        for (int i = 0; i < tumour.rollingEffet.Length; i++)
        {
            tumour.rollingEffet[i].gameObject.OnActive(true);
        }
    }

    protected override void OnReleaseEnd(bool complete)
    {
        base.OnReleaseEnd(complete);
        for (int i = 0; i < tumour.rollingEffet.Length; i++)
        {
            tumour.rollingEffet[i].gameObject.OnActive(false);
        }
    }
}