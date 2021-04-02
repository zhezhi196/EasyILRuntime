using System;
using UnityEngine;

namespace Module
{
    public interface IRewardObject
    {
        void GetIapIcon(Action<Sprite> callback);
        void GetIapReward(long count);
    }
}