using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Module
{
    public class AgentBehaviorCtrl : IAgentCtrl
    {
        private List<object> pauseList = new List<object>();
        public IBehaviorObject owner { get; }

        public bool isPause
        {
            get { return pauseList.Count > 0 || owner.isPauseBehavior; }
        }

        public AgentBehaviorCtrl(IBehaviorObject owner)
        {
            this.owner = owner;
        }

        public AgentBehaviorTree currBT
        {
            get { return owner.behaviourTree.ExternalBehavior as AgentBehaviorTree; }
        }

        public AgentBehaviorTree SwitchBehavior(string behavior, params object[] args)
        {
            AgentBehaviorTree behavoior = owner.GetBehaviorTree(behavior);
            return SwitchBehavior(behavoior);
        }
        
        public AgentBehaviorTree SwitchBehavior(AgentBehaviorTree behavoior, params object[] args)
        {
            AgentBehaviorTree result = AssetLoad.Instantiate(behavoior);
            result.args = args;
            if (currBT != null) currBT.OnExit();
            owner.behaviourTree.ExternalBehavior = result;
            owner.behaviourTree.StartWhenEnabled = result.StartWhenEnabled;
            owner.behaviourTree.PauseWhenDisabled = result.pauseWhenDisabled;
            owner.behaviourTree.RestartWhenComplete = result.restartWhenComplete;
            owner.behaviourTree.ResetValuesOnRestart = result.resetValuesOnRestart;
            result.OnEnter();
            return result;
        }

        private void OnBehaviorEnd(Behavior behavior)
        {
            if (behavior.ExecutionStatus == TaskStatus.Success)
            {
                currBT.OnSuccess();
            }
            else if (behavior.ExecutionStatus == TaskStatus.Failure)
            {
                currBT.OnFail();
            }
        }
        
        private void OnOwnerSwithStation()
        {
            if (isPause)
            {
                owner.behaviourTree.DisableBehavior(true);
            }
            else
            {
                owner.behaviourTree.EnableBehavior();
            }
        }

        public void OnCreat()
        {
            this.owner.behaviourTree.OnBehaviorEnd += OnBehaviorEnd;
            this.owner.onSwitchStation += OnOwnerSwithStation;
        }

        public void OnBorn()
        {
            this.owner.behaviourTree.enabled = true;
            SwitchBehavior(owner.bornBehavior);
        }

        public void OnUpdate()
        {
        }

        public void Pause(object key)
        {
            if (!pauseList.Contains(key))
            {
                pauseList.Add(key);
            }
            
            owner.behaviourTree.DisableBehavior(true);
        }

        public void Continue(object key)
        {
            if (pauseList.Contains(key))
            {
                pauseList.Remove(key);
            }

            if (pauseList.Count == 0 && !owner.isPauseBehavior)
            {
                owner.behaviourTree.EnableBehavior();
            }
        }

        public void OnAgentDead()
        {
            owner.behaviourTree.enabled = false;
        }

        public void OnDestroy()
        {
            this.owner.onSwitchStation -= OnOwnerSwithStation;
            this.owner.behaviourTree.OnBehaviorEnd -= OnBehaviorEnd;
        }


        public T GetAgentCtrl<T>() where T : IAgentCtrl
        {
            return owner.GetAgentCtrl<T>();
        }
    }
}