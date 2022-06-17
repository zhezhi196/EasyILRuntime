using Module;
using Module.SkillAction;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class MeleeAttack : MonsterSkill
{
    public override float stopMoveDistance => maxDistance;
    public virtual float animationSpeed { get; } = 1;
    

    protected abstract string animation { get; }
    protected bool isHurtPlayer;

    [LabelText("最小判定距离"), GUIColor(0.51f, 1, 1)]
    public float minDistance;

    [LabelText("最大判定距离"), GUIColor(1f, 0.68f, 0.51f)]
    public float maxDistance;
    [LabelText("半径伤害")]
    public bool isRadiusDamage;
    [HideIf("isRadiusDamage"),LabelText("伤害范围")]
    public Bounds damageBounds;
    [ShowIf("isRadiusDamage"),LabelText("伤害半径")]
    public float damageRadius;
    public abstract Transform damagePoint { get; }

    public override bool isWanted
    {
        get { return isActive && station == SkillStation.Ready; }
    }

    public override bool isReadyRelease
    {
        get
        {
            return base.isReadyRelease && monster.toPlayerDistance <= maxDistance && monster.toPlayerDistance >= minDistance;
        }
    }
    

    public override void OnInit(ISkillObject owner)
    {
        base.OnInit(owner);
        PushAction(new AnimationAction(animation, 0, owner) {speed = animationSpeed});
        monster.onAnimationCallback += OnAnimationCallback;
    }

    protected override void OnReleaseStart()
    {
        base.OnReleaseStart();
        LookAtPlayer();
        isHurtPlayer = false;
    }

    public virtual void LookAtPlayer()
    {
        monster.LookAtPoint(Player.player.chasePoint);
    }

    protected override void OnReleaseEnd(bool complete)
    {
        base.OnReleaseEnd(complete);
        if (complete)
        {
            if (monster.afterStyle == AfterAttackStyle.Duizhi)
            {
                monster.GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(new Confrontation());
            }
            else if (monster.afterStyle == AfterAttackStyle.Roar)
            {
                monster.Roar(null);
            }
        }
    }

    protected override void OnDispose()
    {
        monster.onAnimationCallback -= OnAnimationCallback;
    }

    protected virtual void OnAnimationCallback(AnimationEvent @event, int index)
    {
        if (@event.animatorClipInfo.clip.name == animation && monster.skillCtrl.currActive == this)
        {
            PlayerDamage damage = GetDamage(index);

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

    public override void UpdateTry()
    {
        if (monster.target != null)
        {
            monster.MoveTo(MoveStyle.Walk, Player.player.chasePoint, ((status, b) =>
            {
                if (b && monster.ContainStation(MonsterStation.IsSeePlayHide))
                {
                    Player.player.OnHurt(GetDamage());
                }
            } ));
        }
    }
}