using UnityEngine;

namespace Module
{
    public enum InitStation
    {
        /// <summary>
        /// 无效
        /// </summary>
        UnActive,
        /// <summary>
        /// 生成
        /// </summary>
        OnlyCreat,
        /// <summary>
        /// 出生后正常
        /// </summary>
        Normal,

        /// <summary>
        /// 死之前
        /// </summary>
        PreDead,

        /// <summary>
        /// 死亡
        /// </summary>
        Dead
    }

    public interface IAgentObject : ILogObject, ITimeCtrl, ISwitchEventObject
    {
        T GetAgentCtrl<T>() where T : IAgentCtrl;
    }
}