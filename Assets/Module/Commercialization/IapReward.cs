using System;
using UnityEngine;

namespace Module
{
    public interface IapReward
    {
        bool isActive { get; }
        Iap iap { get; }
        void GetReward(Action<IapResult> callback, IapRewardFlag flag = 0);
        void GetIcon(Action<Sprite> callback);
        float GetPrice();
        string GetPriceWithCulture();
        
    }
}