using Module;

namespace SimBehavior
{
    public class FollowPlayer : SimpleBehavior
    {
        public Aides aides;
        public SkillCtrl skillCtrl;

        public FollowPlayer(Aides owner, params ISimpleBehavior[] behaviors) : base(behaviors)
        {
            this.aides = owner;
            skillCtrl = aides.GetAgentCtrl<SkillCtrl>();
        }

        public override TaskStatus OnUpdate()
        {
            if (skillCtrl.UpdateRelease(null))
            {
                return TaskStatus.Running;
            }
            if (aides.target == null)
            {
                if (aides.IsTooFar())
                {
                    aides.FollowPlayer();
                }
            }
            
            return TaskStatus.Running;
        }
    }
}