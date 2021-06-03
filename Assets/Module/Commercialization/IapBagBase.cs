using System;
using UnityEngine;

namespace Module
{
    [Flags]
    public enum IapRewardFlag
    {
        NoAudio = 1,
        NoLoading = 2,
        NoAnalysis = 4,
        Free = 8,
        NoUpdateDB = 16,
        NoPause = 32
    }



    public abstract class IapBagBase : IapReward
    {
        protected Iap _iap;
        
        public abstract void GetIcon(RewardIconType type, Action<Sprite> callback);

        public virtual IapState iapState
        {
            get { return iap.iapState; }
        }
        
        public virtual float product { get; set; } = 1;
        public virtual bool canBeShow { get; }

        public Iap iap
        {
            get { return _iap; }
        }

        public abstract void GetReward(Action<IapResult> callback, IapRewardFlag flag = (IapRewardFlag) 0);

        public abstract float GetPrice();

        public virtual string GetPriceWithCulture()
        {
            ICurrency currency = iap as ICurrency;
            
            if (currency != null)
            {
                return currency.price;
            }

            return null;
        }

        public abstract float GetCommodityCount(int index = 0);
        public abstract Commodity GetCommodity(int index = 0);
    }
}