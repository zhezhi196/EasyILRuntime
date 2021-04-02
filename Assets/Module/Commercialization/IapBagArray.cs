using System;
using UnityEngine;

namespace Module
{
    /// <summary>
    /// 礼包,其中commodity是获得的物品.iap对象是消耗端的逻辑
    /// </summary>
    public abstract class IapBagArray : IapReward
    {
        public virtual bool isActive
        {
            get
            {
                if (commodity.IsNullOrEmpty()) return false;
                for (int i = 0; i < commodity.Length; i++)
                {
                    if (!commodity[i].isActive) return false;
                }

                return true;
            }
        }

        public Iap iap { get; }

        protected Commodity[] _commodity;

        public Commodity[] commodity
        {
            get { return _commodity; }
        }

        public IapBagArray(IapDataBase sqlData, string count)
        {
            this.iap = Iap.GetIap(sqlData);
        }

        public IapBagArray(Iap iap, string count)
        {
            this.iap = iap;
        }

        public abstract void GetReward(Action<IapResult> callback, IapRewardFlag flag = 0);
        public virtual void GetIcon(Action<Sprite> callback)
        {
        }

        public float GetPrice()
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
    }
}