using System;
using UnityEngine;

namespace Module
{
    /// <summary>
    /// 商品的基类
    /// </summary>
    public abstract class Commodity
    {
        public IRewardObject reward;
        public long count;

        public virtual bool isActive
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

        public virtual void GetIcon(Action<Sprite> callback)
        {
            reward.GetIapIcon(callback);
        }

        public virtual void GetIapReward()
        {
            reward.GetIapReward(count);
        }
    }
}