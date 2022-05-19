using BehaviorDesigner.Runtime.Tasks;

namespace Module
{
    public abstract class AgentAction : Action, IAgentAction
    {
        protected IBehaviorTreeObject owner;
        public string logName => owner.logName;

        public bool isLog
        {
            get
            {
                return owner.isLog;
            }
            set
            {
                owner.isLog = value;
            }
        }

        public override void OnAwake()
        {
            this.owner = Owner.gameObject.GetComponent<IBehaviorTreeObject>();
        }

        public object GetArgs(int index)
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
        
        public void LogFormat(string obj, params object[] args)
        {
            GetOwner<IAgentObject>().LogFormat(obj, args);
        }

    }
}