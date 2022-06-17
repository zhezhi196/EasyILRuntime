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
    public AnimationAction jumpAnimation;
    public override void OnInit(ISkillObject owner)
    {
        base.OnInit(owner);
        jumpAnimation = new AnimationAction(animationName, 0, owner);
        PushAction(jumpAnimation);
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
        jumpAnimation.play.SetDuationTime(jumpTime + 0.1f);
        lineTrack = new LineTrack(monster.transform.position, tar, jumpTime);
        AnimationCurve xlineCurve = AnimationCurve.Linear(0, 0, 1, 0);
        lineTrack.offset = new XYZOffset(xlineCurve, xlineCurve, xlineCurve);

        if (monster.navmesh)
        {
            monster.navmesh.enabled = false;
        }
        monster.AddStation(addStation);
    }

    protected override void OnReleaseEnd(bool complete)
    {
        base.OnReleaseEnd(complete);
        if (monster.navmesh)
        {
            monster.navmesh.enabled = true;
        }

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
