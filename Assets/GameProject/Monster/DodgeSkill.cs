using Module;
using Module.SkillAction;

public class DodgeSkill : MonsterSkill,IConfrontationSkill
{
    private int _moveToward;
    public override bool isWanted
    {
        get
        {
            return Player.player.currentWeapon.aimMonster == monster && monster.simpleBtCtrl.currBehavior is Confrontation && isReadyRelease;
        }
    }

    public override float stopMoveDistance => 0;
    public override void OnInit(ISkillObject owner)
    {
        base.OnInit(owner);
        owner.GetAgentCtrl<AnimatorCtrl>().AddBlendTree("Dodge", 1);
        PushAction(new AnimationAction("Dodge", 0, owner));
    }

    protected override void OnReleaseStart()
    {
        monster.AddStation(addStation);
        _moveToward = RandomHelper.RandomSymbol();
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