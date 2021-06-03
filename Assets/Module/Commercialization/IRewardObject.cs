using System;
using UnityEngine;

namespace Module
{
    public interface IRewardObject : IconObject
    {
        string rewardDes { get; }
        void GetReward(float count);
        
    }
}