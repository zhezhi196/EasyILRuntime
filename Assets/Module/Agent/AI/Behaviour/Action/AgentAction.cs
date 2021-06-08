using BehaviorDesigner.Runtime.Tasks;

namespace Module
{
    public abstract class AgentAction : Action, IAgentAction
    {
        private IBehaviorObject owner;
        public override void OnAwake()
        {
            this.owner = Owner.gameObject.GetComponent<IBehaviorObject>();
        }

        public float GetArgs(int index)
        {
            var tar = ((AgentBehaviorTree) this.Owner.ExternalBehavior).args;
            if (tar != null)
            {
                if (index < tar.Length)
                {
                    return tar[index];
                }
            }
            return default;
        }

        public T GetOwnerCtrl<T>() where T : IAgentCtrl
        {
            return owner.GetAgentCtrl<T>();
        }

        public T GetOwner<T>()
        {
            return (T)this.owner;
        }
    }
}