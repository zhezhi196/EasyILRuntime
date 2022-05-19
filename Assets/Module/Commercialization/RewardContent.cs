using System;
using UnityEngine;

namespace Module
{
    /// <summary>
    /// 商品的基类
    /// </summary>
    public class RewardContent : IRewardObject
    {
        public IRewardObject reward { get; }
        public FloatField dbCount { get; }
        public FloatField finalCount { get; set; }
        
        public RewardContent(IRewardObject reward, float count)
        {
            this.reward = reward;
            this.dbCount = new FloatField(count);
            this.finalCount = new FloatField(count);
        }

        public void GetIcon(string type, Action<Sprite> callback)
        {
            if (reward != null)
            {
                reward.GetIcon(type, callback);
            }
        }


        public int stationCode
        {
            get { return reward == null ? 0 : reward.stationCode; }
        }

        public float GetReward(float rewardCount, RewardFlag flag)
        {
            if (reward != null)
            {
                float value = this.dbCount.value * rewardCount;
                finalCount = new FloatField(value);
                return reward.GetReward(value, flag);
            }

            return 0;
        }

        public string GetText(string type)
        {
            return string.Format(reward.GetText(type), finalCount);
        }
    }
}