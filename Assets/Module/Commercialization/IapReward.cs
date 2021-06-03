using System;
using UnityEngine;

namespace Module
{
    public interface IapReward : IconObject
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        IapState iapState { get; }
        /// <summary>
        /// 乘积
        /// </summary>
        float product { get; set; }
        /// <summary>
        /// 是否能显示
        /// </summary>
        bool canBeShow { get; }
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
        Commodity GetCommodity(int index = 0);
    }
}