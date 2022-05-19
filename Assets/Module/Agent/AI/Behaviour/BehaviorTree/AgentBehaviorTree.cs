using System;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Module
{
    [CreateAssetMenu(menuName = "HZZ/通用行为树")]
    public class AgentBehaviorTree : ExternalBehaviorTree
    {
        public object[] args;
        public bool StartWhenEnabled = true;
        public bool pauseWhenDisabled = true;
        public bool restartWhenComplete = true;
        public bool resetValuesOnRestart;

        public event Action<bool> onFinish; 

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }
        public virtual void OnSuccess()
        {
            onFinish?.Invoke(true);
            onFinish = null;
        }

        public virtual void OnFail()
        {
            onFinish?.Invoke(false);
            onFinish = null;
        }
    }
}