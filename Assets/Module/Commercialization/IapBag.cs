using System;
using UnityEngine;

namespace Module
{
    [Flags]
    public enum IapRewardFlag
    {
        NoAudio = 1,
        NoLoading = 2,
        NoAnalysis = 4
    }
    /// <summary>
    /// 礼包,其中commodity是获得的物品.iap对象是消耗端的逻辑
    /// </summary>
    public abstract class IapBag : IapReward
    {
        protected Commodity _commodity;

        public virtual bool isActive
        {
            get
            {
                if (commodity == null) return false;
                return commodity.isActive;
            }
        }

        public Iap iap { get; }

        public Commodity commodity
        {
            get { return _commodity; }
        }

        public IapBag(IapDataBase sqlData, long count)
        {
            this.iap = Iap.GetIap(sqlData);
        }

        public IapBag(Iap iap, long count)
        {
            this.iap = iap;
        }

        public virtual void GetIcon(Action<Sprite> callback)
        {
        }

        public virtual float GetPrice()
        {
            return 0;
        }

        public string GetPriceWithCulture()
        {
            ICurrency currency = iap as ICurrency;
            
            if (currency != null)
            {
                return currency.price;
            }

            return null;
        }

        public abstract void GetReward(Action<IapResult> callback, IapRewardFlag flag = 0);
    }
}