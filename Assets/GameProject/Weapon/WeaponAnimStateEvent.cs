using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器AnimtionState切换事件
/// </summary>
public class WeaponAnimStateEvent : StateMachineBehaviour
{
    public string stateName;
    public bool enterEvent = false;
    public bool exitEvent = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!string.IsNullOrEmpty(stateName))
        {
            animator.GetComponent<IWeaponAnimEvent>()?.OnAnimStateEnter(stateName);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!string.IsNullOrEmpty(stateName))
        {
            animator.GetComponent<IWeaponAnimEvent>()?.OnAnimStateExit(stateName);
        }
    }
}
