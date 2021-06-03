using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public enum IapResultMessage
    {
        Success,
        Fail,
    }
    public class IapResult
    {
        /// <summary>
        /// 结果
        /// </summary>
        public IapResultMessage result;
        
        /// <summary>
        /// 礼包，这里会有商品
        /// </summary>
        public IapReward reward;
        
        /// <summary>
        /// 是否跳过支付
        /// </summary>
        public bool skipConsume;
        
        /// <summary>
        /// 奖励数量
        /// </summary>
        public float[] realCounts;

        /// <summary>
        /// 自己生成的订购ID
        /// </summary>
        public string orderId;

        /// <summary>
        /// 平台的订单号
        /// </summary>
        public string plantOrder;
        /// <summary>
        /// 奖励种类数量
        /// </summary>
        public int rewardCount
        {
            get
            {
                if (reward is IapBag) return 1;
                if (reward is IapBagArray) return ((IapBagArray) reward).commodity.Length;
                return 0;
            }
        }

        public IapResult(IapReward reward)
        {
            this.reward = reward;
        }
        /// <summary>
        /// 获取商品
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Commodity GetCommodity(int index = 0)
        {
            IapBag temp = reward as IapBag;
            if (temp != null) return temp.commodity;
            IapBagArray temp2 = reward as IapBagArray;
            if (temp2 != null) return temp2.commodity[index];
            return null;
        }
        
        /// <summary>
        /// 获取某种奖励的数量
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetRewardCount(int index = 0)
        {
            return realCounts[index];
        }
    }
}