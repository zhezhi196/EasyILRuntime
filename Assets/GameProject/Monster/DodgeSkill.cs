using Module;
using Module.SkillAction;
using Sirenix.OdinInspector;

public class DodgeSkill : MonsterSkill,IConfrontationSkill
{
    [LabelText("闪避距离")] public float dodgeDistance = 20;
    [LabelText("闪避时间"),ReadOnly]
    public float dodgeTime = 1;
    public float dodgeSpeed
    {
        get
        {
            return dodgeDistance/dodgeTime;
        }
    }
    private int _moveToward;

    public override bool isWanted
    {
        get
        {
            return monster.simpleBtCtrl.currBehavior is Confrontation &&
                   Player.player.currentWeapon.aimMonster == monster && isReadyRelease;
        }
    }

    public Confrontation behavior { get; set; }

    public override float stopMoveDistance => 0;

    public override void OnInit(ISkillObject owner)
    {
        base.OnInit(owner);
        owner.GetAgentCtrl<AnimatorCtrl>().AddBlendTree("Dodge", dodgeTime);
        PushAction(new AnimationAction("Dodge", 0, owner));
    }

    protected override void OnReleaseStart()
    {
        monster.AddStation(addStation);
        _moveToward = Confrontation.GetMoveDir(behavior.isBack, 10, monster.navmesh);
    }

    protected override void OnDispose()
    {
        
    }

    public int moveToward
    {
        get
        {
            return _moveToward;
        }
    }
}