using GameBuff;
using Module;
using Module.SkillAction;

public abstract class BuffSkill : PlayerSkillInstance
{
    public abstract string attackAnimation { get; }
    public PlayerStation addStation = PlayerStation.Attack;
    public ISkillAction attackAction;
    
    public override void OnInit(ISkillObject owner)
    {
        base.OnInit(owner);
        attackAction = new StationAction(attackAnimation, 0, owner);
        PushAction(attackAction);
        cd = new TimeAction(skillModle.dbData.cd, owner);
    }

    protected override void OnReleaseStart()
    {
        base.OnReleaseStart();
        Player.player.AddStation(addStation);
    }
    protected override void OnReleaseEnd(bool complete)
    {
        base.OnReleaseEnd(complete);
        Player.player.RemoveStation(addStation);
    }
}