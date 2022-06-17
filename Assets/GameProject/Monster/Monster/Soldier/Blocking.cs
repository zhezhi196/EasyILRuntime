using Module;
using Module.SkillAction;
using Project.Data;
using Sirenix.OdinInspector;

[SkillDescript("防爆士兵/格挡")]
public class Blocking : MonsterSkill, IConfrontationSkill
{
    [LabelText("格挡时间")]
    public float blockTime = 3;
    [LabelText("离玩家多少米停止格挡")]
    public float minBlockDistance = 2;
    public override bool isWanted
    {
        get
        {
            return Player.player.currentWeapon.aimMonster == monster && monster.simpleBtCtrl.currBehavior is Confrontation && isReadyRelease;
        }
    }
    public override void OnInit(ISkillObject owner)
    {
        base.OnInit(owner);
        PushAction(new MutiAction(MutiAction.MutiType.Or, new TimeAction(blockTime, owner),
            new PredicateAction(owner, () => monster.toPlayerDistance <= minBlockDistance)));
    }

    protected override void OnDispose()
    {
    }

    protected override void OnReleaseStart()
    {
        monster.AddStation(addStation);
        monster.GetAgentCtrl<AnimatorCtrl>().SetFloat("gedang", 1, 0.2f);
        monster.ExitConfrontation();
    }

    protected override void OnReleaseEnd(bool complete)
    {
        base.OnReleaseEnd(complete);
        monster.GetAgentCtrl<AnimatorCtrl>().SetFloat("gedang", 0, 0.2f);
    }

    protected override void OnActionUpdate(ISkillAction arg1, float percent)
    {
        base.OnActionUpdate(arg1, percent);
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

    public override float stopMoveDistance
    {
        get { return 0.5f; }
    }

    public int moveToward { get; }
    public Confrontation behavior { get; set; }
}