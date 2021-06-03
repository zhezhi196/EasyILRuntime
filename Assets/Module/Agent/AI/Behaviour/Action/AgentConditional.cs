using BehaviorDesigner.Runtime.Tasks;

namespace Module
{
    public class AgentConditional : Conditional, IAgentAction
    {
        public T GetArgs<T>(int index)
        {
            return (T) ((AgentBehaviorTree) this.Owner.ExternalBehavior).args[index];
        }
    }
}