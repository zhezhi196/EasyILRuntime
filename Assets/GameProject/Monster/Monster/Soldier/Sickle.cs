using Module;
using UnityEngine;
[SkillDescript("防爆士兵/镰刀攻击")]
public class Sickle : MeleeAttack
{
    protected override string animation
    {
        get
        {
            return "soldier@attack";
        }
    }

    public override Transform damagePoint
    {
        get
        {
            return ((Soldier) monster).damagePoint;
        }
    }
}