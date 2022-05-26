using Module;
using Module.SkillAction;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class JumpSkill : MonsterSkill
{
    [LabelText("跳跃时间")]
    public float jumpTime = 0.8f;
    [LabelText("跳跃曲线")]
    public AnimationCurve jumpCurve;
    [LabelText("最小释放距离")]
    public float minWantedDistance;
    [LabelText("最大释放距离")]
    public float maxWantedDistance;
    [LabelText("伤害半径")]
    public float damageRadius;
    [LabelText("跳跃动画")]
    public string animationName;
    
    private LineTrack lineTrack;

    public abstract Vector3 jumpTarget { get; }
    public abstract Vector3 damagePoint { get; }
    
    public override void OnInit(ISkillObject owner)
    {
        base.OnInit(owner);
        PushAction(new AnimationAction(animationName, 0, owner));
    }
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

    protected override void OnReleaseStart()
    {
        Vector3 tar = jumpTarget;
        lineTrack = new LineTrack(monster.transform.position, tar, jumpTime);
        AnimationCurve xlineCurve = AnimationCurve.Linear(0, 0, 1, 0);
        lineTrack.offset = new XYZOffset(xlineCurve, xlineCurve, xlineCurve);
        lineTrack.onComplete += JumpComplete;
        if (monster.navmesh)
        {
            monster.navmesh.enabled = false;
        }
        monster.AddStation(addStation);
    }

    public virtual void JumpComplete()
    {
        if (monster.navmesh)
        {
            monster.navmesh.enabled = true;
        }
        monster.HurtPlayer(GetDamage(), damageRadius, damagePoint);
        monster.RemoveStation(addStation);
    }
    
    protected override void OnActionUpdate(ISkillAction arg1, float percent)
    {
        monster.transform.position = lineTrack.UpdatePosition(monster.GetDelatime(false));
    }


    protected override void OnDispose()
    {
    }

    public override float stopMoveDistance { get; }
}
