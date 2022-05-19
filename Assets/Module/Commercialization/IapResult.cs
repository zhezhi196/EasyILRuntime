using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Module
{
    public enum IapResultMessage
    {
        Success,
        Fail,
    }

    public class IapResult : IconObject, ITextObject
    {
        /// <summary>
        /// 结果
        /// </summary>
        public IapResultMessage result;
        
        /// <summary>
        /// 礼包，这里会有商品
        /// </summary>
        public IapBag reward;
        
        /// <summary>
        /// 是否跳过支付
        /// </summary>
        public bool skipConsume;

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
            get { return reward.content.Length; }
        }

        public IapResult(IapBag reward)
        {
            this.reward = reward;
        }

        public void GetIcon(string type, Action<Sprite> callback)
        {
            reward.GetIcon(type, callback);
        }

        public string GetText(string type)
        {
            return reward.GetText(type);
        }
    }
}