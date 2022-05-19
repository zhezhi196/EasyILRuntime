using UnityEngine;
using UnityEngine.AI;

namespace Module
{
    public interface IMoveObject : ISwitchEventObject, IAgentObject, ITarget
    {
        /// <summary>
        /// 旋转到移动 agent与目标方向的临界角度
        /// </summary>
        float rotateToMove { get; }

        /// <summary>
        /// 停止距离
        /// </summary>
        float stopMoveDistance { get; }

        /// <summary>
        /// 旋转速度
        /// </summary>
        float rotateSpeed { get; }
        /// <summary>
        /// 移动速度
        /// </summary>
        float moveSpeed { get; }
    }
}