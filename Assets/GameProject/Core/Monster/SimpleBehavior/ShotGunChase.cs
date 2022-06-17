using Module;

public class ShotGunChase : SimpleBehavior
{
    public GunMonster monster
    {
        get { return owner as GunMonster; }
    }

    public SkillCtrl skillCtrl;

    public override void OnStart(ISimpleBehaviorObject owner, object[] args)
    {
        base.OnStart(owner, args);
        skillCtrl = owner.GetAgentCtrl<SkillCtrl>();
    }

    public override TaskStatus OnUpdate()
    {
        if (monster.target != null)
        {
            monster.LookAtPoint(monster.target.transform.position);
            skillCtrl.UpdateRelease(null);
        }
        return TaskStatus.Running;
    }
}