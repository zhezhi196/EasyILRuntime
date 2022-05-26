using Module;
using Module.SkillAction;
using UnityEngine;

[SkillDescript("将军/镰刀旋风")]
public class Whirlwind : MeleeAttack
{
    General general
    {
        get { return monster as General; }
    }
    //美术动画名字命错了
    protected override string animation
    {
        get { return "liandaolajin"; }
    }

    public override Transform damagePoint
    {
        get { return general.weapon.transform; }
    }

    protected override void OnReleaseStart()
    {
        base.OnReleaseStart();
        general.flyWeapon.isFire = true;
    }

    protected override void OnReleaseEnd(bool complete)
    {
        base.OnReleaseEnd(complete);
        general.flyWeapon.isFire = false;
    }

    protected override void OnActionUpdate(ISkillAction arg1, float percent)
    {
        general.flyWeapon.UpdateLine(general.handPoint.position, general.weapon.transform.position);
        if (!isHurtPlayer)
        {
            PlayerDamage damage = GetDamage();

            if (!isRadiusDamage)
            {
                if (monster.HurtPlayer(damage, damageBounds, damagePoint))
                {
                    isHurtPlayer = true;
                }
            }
            else
            {
                if (monster.HurtPlayer(damage, damageRadius, damagePoint.position))
                {
                    isHurtPlayer = true;
                }
            }
        }
    }
}