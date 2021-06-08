using BehaviorDesigner.Runtime.Tasks;

namespace Module
{
    public class AgentConditional : Conditional, IAgentAction
    {
        public float GetArgs(int index)
        {
            return ((AgentBehaviorTree) this.Owner.ExternalBehavior).args[index];
        }
    }
}