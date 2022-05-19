using System;

namespace Module
{
    /// <summary>
    /// 主动技能
    /// </summary>
    public interface IActiveSkill : ISkill
    {
        /// <summary>
        /// 是否能成为想要释放的技能
        /// </summary>
        bool isWanted { get; }
        /// <summary>
        /// 当成为想要释放的技能却放不出来时,每帧调用的逻辑
        /// </summary>
        void UpdateTry();
    }
}