using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 退出换弹动画
/// todo--暂时只做打断音效,待优化
/// </summary>
public class WeaponReloadAnimStateExit : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Weapon>()?.StopAnimAudio();
    }
}
