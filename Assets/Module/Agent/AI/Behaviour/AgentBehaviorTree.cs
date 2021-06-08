using System;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Module
{
    [CreateAssetMenu(menuName = "行为树/通用行为树")]
    public class AgentBehaviorTree : ExternalBehaviorTree
    {
        public float[] args;
        public bool StartWhenEnabled;
        public bool pauseWhenDisabled;
        public bool restartWhenComplete;
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