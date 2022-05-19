using System;
using UnityEngine;

namespace Module
{
    [Flags]
    public enum RewardFlag
    {
        NoAudio = 1,
        NoRecord = 2,
    }
    public interface IRewardObject : IconObject, ITextObject
    {
        /// <summary>
        /// 状态码 0:正常 1:不显示 2之后的需要自己定义了 
        /// </summary>
        int stationCode { get; }

        /// <summary>
        /// 获取相应的奖励
        /// </summary>
        /// <param name="rewardCount">获取奖励的数量</param>
        /// <returns>获取奖励的真实数量</returns>
        float GetReward(float rewardCount, RewardFlag flag);
    }
}