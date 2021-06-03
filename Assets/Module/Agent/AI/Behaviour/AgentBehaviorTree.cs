using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Module
{
    [CreateAssetMenu(menuName = "行为树/通用行为树")]
    public class AgentBehaviorTree : ExternalBehaviorTree
    {
        public object[] args;
        public bool StartWhenEnabled;
        public bool pauseWhenDisabled;
        public bool restartWhenComplete;
        public bool resetValuesOnRestart;

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }
        public virtual void OnSuccess()
        {
        }

        public virtual void OnFail()
        {
        }
    }
}