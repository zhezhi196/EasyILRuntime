using Module;
using Module.SkillAction;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class RemoteAttack : MonsterSkill
{
    [LabelText("最小想要释放的距离"), GUIColor(0.51f, 1, 1)]
    public float minDistance;

    [LabelText("最大想要释放的距离"), GUIColor(1f, 0.68f, 0.51f)]
    public float maxDistance;
    [LabelText("能够释放的距离")]
    public float minReleaseDistance;

    [LabelText("投掷物速度")] public float moveSpeed = 5;
    protected abstract string[] animation { get; }
    [LabelText("释放技能时看着玩家")]
    public bool isAim;
    public override float stopMoveDistance => minDistance;


    public override bool isWanted
    {
        get
        {
            return isActive && station == SkillStation.Ready && monster.toPlayerDistance <= maxDistance &&
                   monster.toPlayerDistance >= minDistance;
        }
    }
    public override bool isReadyRelease
    {
        get
        {
            return base.isReadyRelease && monster.toPlayerDistance <= minReleaseDistance;
        }
    }
    
    public override void OnInit(ISkillObject owner)
    {
        base.OnInit(owner);
        for (int i = 0; i < animation.Length; i++)
        {
            var acction = new AnimationAction(animation[i], 0, owner);
            PushAction(acction);
        }

        monster.onAnimationCallback += OnAnimationCallback;
    }

    protected override bool OnBreak(BreakSkillFlag flag)
    {
        monster.RemoveStation(MonsterStation.Attack);
        return true;
    }


    protected override void OnReleaseStart()
    {
        base.OnReleaseStart();
        NavMoveCtrl nav = monster.GetAgentCtrl<NavMoveCtrl>();
        if (nav != null)
        {
            monster.MoveTo(monster.transform.position, null);
        }
    }
    
    protected override void OnActionUpdate(ISkillAction arg1, float percent)
    {
        if (isAim)
        {
            Vector3 temp = Player.player.chasePoint - monster.transform.position;
            monster.transform.forward = new Vector3(temp.x, 0, temp.z);
        }
    }

    protected override void OnDispose()
    {
        monster.onAnimationCallback -= OnAnimationCallback;
    }

    public override void UpdateTry()
    {
        bool isSeePlayer = monster.isSeePlayer;
        if (monster.toPlayerDistance >= minReleaseDistance || !isSeePlayer)
        {
            base.UpdateTry();
        }
        else
        {
            monster.MoveTo(monster.transform.position, null);
        }
    }

    protected virtual void OnAnimationCallback(AnimationEvent @event, int index)
    {
        if (animation.Contains(@event.animatorClipInfo.clip.name) && monster.skillCtrl.currActive == this)
        {
            OnReleaseGo(@event, index);
        }
    }

    public abstract void OnReleaseGo(AnimationEvent @event, int index);
}