using System;
using UnityEngine;

namespace Module
{
    public interface IapReward : IconObject, IShowObject
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        IapState iapState { get; }
        /// <summary>
        /// 乘积
        /// </summary>
        FloatField product { get; set; }
        
        int rewardCount { get; }

        Iap iap { get; }

        /// <summary>
        /// 获得礼包内的所有奖励
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="flag"></param>
        void GetReward(Action<IapResult> callback, IapRewardFlag flag = 0);

        /// <summary>
        /// 获得当前礼包的价格
        /// </summary>
        /// <returns></returns>
        float GetPrice();
        /// <summary>
        /// 从sdk读取价格
        /// </summary>
        /// <returns></returns>
        string GetPriceWithCulture();
        /// <summary>
        /// 获取商品数量
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        float GetCommodityCount(int index = 0);

        /// <summary>
        /// 根据index获取商品
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        RewardContent GetCommodity(int index = 0);
    }
}