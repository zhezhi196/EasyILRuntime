using Module;
using Module.SkillAction;
using Sirenix.OdinInspector;
using UnityEngine;
[SkillDescript("将军/镰刀拉近")]
public class Closer : RemoteAttack
{

    General general
    {
        get { return monster as General; }
    }
    [LabelText("目标偏移")]
    public Vector3 targetOffset;
    [LabelText("伤害范围")]
    public Bounds damageBounds;
    
    [ReadOnly]
    public bool isHurtPlayer;
    [ReadOnly]
    public bool isPreRelease = true;
    protected override string[] animation { get; } = {"liandaoxuanfeng"};

    protected override void OnReleaseStart()
    {
        base.OnReleaseStart();
        isPreRelease = true;
        general.flyWeapon.isFire = true;
        isHurtPlayer = false;
    }

    protected override void OnReleaseEnd(bool complete)
    {
        base.OnReleaseEnd(complete);
        general.weapon.gameObject.OnActive(true);
    }

    protected override void OnActionUpdate(ISkillAction arg1, float percent)
    {
        base.OnActionUpdate(arg1, percent);
        if (isPreRelease)
        {
            general.flyWeapon.UpdateLine(general.handPoint.position, general.weapon.transform.position);
        }
        else
        {
            general.flyWeapon.UpdateLine(general.handPoint.position, general.flyWeapon.transform.position);
            if (!isHurtPlayer)
            {
                PlayerDamage damage = GetDamage();
                if (monster.HurtPlayer(damage, damageBounds, general.flyWeapon.transform))
                {
                    Player.player.StartDragMove(general.flyWeapon.transform,monster);
                    general.flyWeapon.Back(true);
                    isHurtPlayer = true;
                }
            }

        }
    }
    

    public override void OnReleaseGo(AnimationEvent @event, int index)
    {
        isPreRelease = false;
        general.weapon.gameObject.OnActive(false);
        general.flyWeapon.transform.position = general.handPoint.transform.position;
        Vector3 dir = (Player.player.CenterPostion + targetOffset - general.handPoint.position).normalized;
        general.flyWeapon.Move(general.handPoint, maxDistance * dir + general.handPoint.position);
    }
}