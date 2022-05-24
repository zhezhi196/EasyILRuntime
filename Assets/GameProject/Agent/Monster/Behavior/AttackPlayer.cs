using Module;

namespace MonsterBehavior
{
    public class AttackPlayer : SimpleBehavior
    {
        public SkillCtrl skillCtrl;
        public override void OnStart(ISimpleBehaviorObject owner, object[] args)
        {
            base.OnStart(owner, args);
            skillCtrl = owner.GetAgentCtrl<SkillCtrl>();
        }

        public override TaskStatus OnUpdate()
        {
            skillCtrl.UpdateRelease(null);
            return TaskStatus.Running;
        }
    }
}