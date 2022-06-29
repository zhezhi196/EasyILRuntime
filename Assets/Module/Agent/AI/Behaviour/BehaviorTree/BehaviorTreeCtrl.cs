using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Module
{
    /// <summary>
    /// 注意: 如果初始行为树是巡逻之类的带有移动的,这个初始化要在移动控制器之后
    /// </summary>
    public class BehaviorTreeCtrl : IAgentCtrl
    {
        protected bool _isPause;
        protected string lastBehavior;
        protected object[] lastBehaviorArgs;
        public string logName => owner.logName;
        public bool isPause => _isPause;
        public IBehaviorTreeObject owner { get; }
        public bool isLog => owner.isLog;

        public AgentBehaviorTree currBT => owner.behaviourTree.ExternalBehavior as AgentBehaviorTree;
        
        public BehaviorTreeCtrl(IBehaviorTreeObject owner)
        {
            this.owner = owner;
            this.owner.behaviourTree.OnBehaviorEnd += OnBehaviorEnd;
            this.owner.onSwitchStation += OnOwnerSwithStation;
        }
        
        /// <summary>
        /// 切换行为树
        /// </summary>
        /// <param name="behavior"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public AgentBehaviorTree SwitchBehavior(BehaviorOption option)
        {
            return SwitchBehavior(option.behaviorName, option.behaviorArg);
        }
        
        /// <summary>
        /// 切换行为树
        /// </summary>
        /// <param name="behavior"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public AgentBehaviorTree SwitchBehavior(string behavior, params object[] args)
        {
            AgentBehaviorTree behavoior = owner.GetBehaviorTree(behavior);
            return SwitchBehavior(behavoior,args);
        }
        
        /// <summary>
        /// 切换行为树
        /// </summary>
        /// <param name="behavoior"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public AgentBehaviorTree SwitchBehavior(AgentBehaviorTree behavoior, params object[] args)
        {
            if (behavoior == null)
            {
                GameDebug.LogError("行为树不能为空");
                return null;
            }
            if (currBT != null)
            {
                lastBehavior = currBT.name;
                lastBehaviorArgs = currBT.args;
            }

            owner.behaviourTree.enabled = false;
            owner.behaviourTree.DisableBehavior(false);
            AgentBehaviorTree result = Object.Instantiate(behavoior);
            result.name = result.name.Substring(0, result.name.Length - 7); result.args = args;
            if (currBT != null) currBT.OnExit();
            owner.behaviourTree.StartWhenEnabled = result.StartWhenEnabled;
            owner.behaviourTree.PauseWhenDisabled = result.pauseWhenDisabled;
            owner.behaviourTree.RestartWhenComplete = result.restartWhenComplete;
            owner.behaviourTree.ResetValuesOnRestart = result.resetValuesOnRestart;
            owner.behaviourTree.ExternalBehavior = result;
            result.OnEnter();
            owner.LogFormat("{0}切换行为树{1}", owner.transform.name, behavoior.name);
            owner.behaviourTree.enabled = true;
            owner.behaviourTree.EnableBehavior();
            return result;
        }

        /// <summary>
        /// 切换到上一个行为树
        /// </summary>
        /// <returns></returns>
        public AgentBehaviorTree SwitchLastBehavior()
        {
            if (lastBehavior != null)
            {
               return SwitchBehavior(lastBehavior,lastBehaviorArgs);
            }

            return null;
        }

        /// <summary>
        /// 当行为树结束时
        /// </summary>
        /// <param name="behavior"></param>
        private void OnBehaviorEnd(Behavior behavior)
        {
            if (currBT != null)
            {
                owner.LogFormat("行为树{0}结束: {1}", currBT.name, behavior.ExecutionStatus);
            }

            if (behavior.ExecutionStatus == BehaviorDesigner.Runtime.Tasks.TaskStatus.Success)
            {
                currBT.OnSuccess();
            }
            else if (behavior.ExecutionStatus == BehaviorDesigner.Runtime.Tasks.TaskStatus.Failure)
            {
                currBT.OnFail();
            }
        }
        
        private void OnOwnerSwithStation()
        {
        }

        public bool OnUpdate()
        {
            return true;
        }

        public void Pause()
        {
            owner.behaviourTree.DisableBehavior(true);
            _isPause = true;
        }

        public void Continue()
        {
            owner.behaviourTree.EnableBehavior();
            _isPause = true;
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

        public void EditorInit()
        {
            BehaviorTree _beahviorTree = owner.transform.gameObject.AddOrGetComponent<BehaviorTree>();
            _beahviorTree.enabled = false;
        }

        public void OnDrawGizmos()
        {
            
        }
    }
}