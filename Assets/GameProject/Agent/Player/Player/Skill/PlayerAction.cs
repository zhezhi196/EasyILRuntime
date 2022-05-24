using Module;

public abstract class PlayerAction : Skill, IActiveSkill
{
    public abstract AgentType agentType { get; }
    
    public bool isWanted
    {
        get
        {
            var morphology = ((agentType & Player.player.currLevel.agentType) != 0);
            return morphology && isActive && station == SkillStation.Ready;
        }
    }

    public virtual void UpdateTry()
    {
    }

    public override bool isReadyRelease
    {
        get { return Player.player.toTargetAngle <= 5 && base.isReadyRelease; }
    }
    
        
    
    public virtual MonsterDamage OnHurt(MonsterDamage damage)
    {
        return damage;
    }
}