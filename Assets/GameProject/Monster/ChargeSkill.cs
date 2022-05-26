using Module;
using Module.SkillAction;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 冲锋技能
/// </summary>
public abstract class ChargeSkill : MonsterSkill
{
    [LabelText("最小释放距离")]
    public float minWantedDistance;
    [LabelText("最大释放距离")]
    public float maxWantedDistance;
    [LabelText("冲刺速度")]
    public float chargeSpeed;
    [LabelText("最大冲刺距离")]
    public float maxChargeDistance;
    [LabelText("伤害范围检测")]
    public Bounds damageBounds;
    [LabelText("冲锋到终点后的动画")]
    public string OnTriggerAnimation;
    [LabelText("冲锋检测层")]
    public LayerMask stopLayer;
    public abstract Transform checkAttackPoint { get; }
    [ReadOnly]
    public Vector3 stopPoint;
    [ReadOnly]
    public bool isHurtPlayer;
    [ReadOnly]
    public bool isCompleteCharge;
    [ReadOnly]
    public bool hasObstacle;
    public override bool isReadyRelease
    {
        get { return isWanted && base.isReadyRelease; }
    }

    public override bool isWanted
    {
        get
        {
            return isActive && station == SkillStation.Ready && monster.toPlayerDistance <= maxWantedDistance && monster.toPlayerDistance >= minWantedDistance;
        }
    }

    public override void OnInit(ISkillObject owner)
    {
        base.OnInit(owner);
        PushAction(new PredicateAction(owner, () => isCompleteCharge));
    }

    protected override void OnReleaseStart()
    {
        base.OnReleaseStart();
        isHurtPlayer = false;
        Ray ray = new Ray(monster.eye.transform.position, Player.player.transform.position - monster.transform.position);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, maxChargeDistance, stopLayer))
        {
            stopPoint = new Vector3(hitInfo.point.x, monster.transform.position.y, hitInfo.point.z);
            hasObstacle = true;
        }
        else
        {
            stopPoint = ray.GetPoint(maxChargeDistance);
            hasObstacle = false;
        }
    }

    protected override void OnActionUpdate(ISkillAction arg1, float percent)
    {
        base.OnActionUpdate(arg1, percent);
        monster.MoveTo(MoveStyle.Run, stopPoint, ((status, b) =>
        {
            if (b)
            {
                if (hasObstacle)
                {
                    TriggerObject();
                }
                else
                {
                    isCompleteCharge = true;
                }
            }
        }));
        
        if (!isHurtPlayer && monster.HurtPlayer(GetDamage(), damageBounds, checkAttackPoint))
        {
            TriggerObject();
            isHurtPlayer = true;
        }
    }
    
    public void TriggerObject()
    {
        if (!OnTriggerAnimation.IsNullOrEmpty())
        {
            monster.AddStation(MonsterStation.Paralysis);
            monster.GetAgentCtrl<AnimatorCtrl>().Play(OnTriggerAnimation, 0, 0).onStationChange += st =>
            {
                if (st.isComplete)
                {
                    monster.RemoveStation(MonsterStation.Paralysis);
                }
            };
        }

        isCompleteCharge = true;
    }

    protected override void OnDispose()
    {
    }

    public override float stopMoveDistance => damageBounds.extents.z;
}