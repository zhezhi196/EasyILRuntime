using System;
using UnityEngine;
using UnityEngine.Animations;

namespace Module
{
    public class StationMachineCallback : StateMachineBehaviour
    {
        private StateMachineCtrl ctrl;
        private AnimatorStateInfo stateInfo;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
            AnimatorControllerPlayable controller)
        {
            this.stateInfo = stateInfo;
            if (ctrl == null)
            {
                IStateMechineObject owner = animator.GetComponentInParent<IStateMechineObject>();
                ctrl = owner.GetAgentCtrl<StateMachineCtrl>();
            }

            ctrl?.OnStateEnter(stateInfo, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
            AnimatorControllerPlayable controller)
        {
            ctrl?.OnStateExit(stateInfo, layerIndex);
        }
    }
}