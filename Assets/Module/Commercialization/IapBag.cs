using System;
using UnityEngine;

namespace Module
{
    /// <summary>
    /// 礼包,其中commodity是获得的物品.iap对象是消耗端的逻辑
    /// </summary>
    public abstract class IapBag : IapBagBase
    {
        protected Commodity _commodity;

        public override bool canBeShow
        {
            get
            {
                if (iapState == IapState.Normal)
                {
                    if (commodity == null) return false;
                    return commodity.canBeShow;
                }
                else if (iapState == IapState.Invalid)
                {
                    return false;
                }
                else if (iapState == IapState.Skip)
                {
                    return true;
                }

                return false;
            }
        }

        public Commodity commodity
        {
            get { return _commodity; }
        }

        public IapBag(IapDataBase sqlData, long count)
        {
            this._iap = Iap.GetIap(sqlData);
        }

        public IapBag(Iap iap, long count)
        {
            this._iap = iap;
        }

        /// <summary>
        /// 获得的物品奖励数量,不计算像广告双倍的那些,如若要得到包括广告后的那些,请到iapresult里面获取
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override float GetCommodityCount(int index = 0)
        {
            return (_commodity.count * product).ToFloat();
        }
        
        /// <summary>
        /// 获取商品
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override Commodity GetCommodity(int index = 0)
        {
            return commodity;
        }
    }
}