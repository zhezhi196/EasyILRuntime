using System;
using UnityEngine;

namespace Module
{

    
    /// <summary>
    /// 商品的基类
    /// </summary>
    public abstract class Commodity : IconObject
    {
        public IRewardObject reward { get; }
        public long count { get; }
        public string GetRewardDes
        {
            get
            {
                if (reward != null)
                {
                    return reward.rewardDes;
                }

                return null;
            }
        }

        public virtual bool canBeShow
        {
            get { return true; }
        }

        public Commodity(IRewardObject reward, long count)
        {
            this.reward = reward;
            this.count = count;
        }
        public Commodity(long count)
        {
            this.count = count;
        }

        public virtual void GetIcon(RewardIconType type, Action<Sprite> callback)
        {
            if (reward != null)
            {
                reward.GetIcon(type, callback);
            }
        }

        public virtual float GetNormalReward(float mutiple = 1)
        {
            if (reward != null)
            {
                float count2 = count * mutiple;
                reward.GetReward(count2);
                return count2;
            }

            return 0;
        }


    }
}