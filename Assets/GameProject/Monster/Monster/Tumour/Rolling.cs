using Module;
using Module.SkillAction;
using Sirenix.OdinInspector;
using UnityEngine;

[SkillDescript("肉瘤/翻滚攻击")]
public class Rolling : ChargeSkill
{
    public override Transform checkAttackPoint
    {
        get
        {
            return monster.centerPoint;
        }
    }
}