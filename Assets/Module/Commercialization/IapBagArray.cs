using System;
using UnityEngine;

namespace Module
{
    /// <summary>
    /// 礼包,其中commodity是获得的物品.iap对象是消耗端的逻辑
    /// </summary>
    public abstract class IapBagArray : IapBagBase
    {
        protected Commodity[] _commodity;

        public override bool canBeShow
        {
            get
            {
                if (iapState == IapState.Normal)
                {
                    if (commodity.IsNullOrEmpty()) return false;
                    for (int i = 0; i < commodity.Length; i++)
                    {
                        if (!commodity[i].canBeShow) return false;
                    }
                }
                else if (iapState == IapState.Invalid)
                {
                    return false;
                }
                else if (iapState == IapState.Skip)
                {
                    return true;
                }
                
                return true;
            }
        }

        public Commodity[] commodity
        {
            get { return _commodity; }
        }

        public IapBagArray(IapDataBase sqlData, string count)
        {
            this._iap = Iap.GetIap(sqlData);
        }

        public IapBagArray(Iap iap, string count)
        {
            this._iap = iap;
        }

        public override float GetCommodityCount(int index = 0)
        {
            return (_commodity[index].count * product).ToLong();
        }

        public override Commodity GetCommodity(int index = 0)
        {
            return commodity[index];
        }
    }
}