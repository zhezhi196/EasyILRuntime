using Module;
using Module.SkillAction;
using Project.Data;

[SkillDescript("防爆士兵/格挡")]
public class Blocking : MonsterSkill, IConfrontationSkill
{
    public float blockTime = 3;
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

    public override float stopMoveDistance
    {
        get { return 0.5f; }
    }

    public int moveToward { get; }
}