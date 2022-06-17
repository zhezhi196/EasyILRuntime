
using Module;
using Module.SkillAction;
using UnityEngine;

[SkillDescript("将军/坦克冲锋")]
public class Tank : ChargeSkill
{
    public override Transform checkAttackPoint
    {
        get
        {
            return monster.centerPoint;
        }
    }
}