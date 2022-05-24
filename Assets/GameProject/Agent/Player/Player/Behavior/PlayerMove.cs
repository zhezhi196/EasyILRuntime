using Module;

namespace SimBehavior
{
    public class PlayerMove : SimpleBehavior
    {
        public SkillCtrl skillCtrl;
        public override void OnStart(ISimpleBehaviorObject owner, object[] args)
        {
            base.OnStart(owner, args);
            skillCtrl = owner.GetAgentCtrl<SkillCtrl>();
        }

        public override TaskStatus OnUpdate()
        {
            if (skillCtrl.UpdateRelease(null))
            {
                return TaskStatus.Running;
            }
            if (Player.player.target != null)
            {
                Player.player.MoveToPoint(Player.player.target.transform.position);
            }
            return TaskStatus.Running;
        }
    }
}