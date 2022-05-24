using System;
using Module;
using UnityEngine;

public class PropEntity: IRewardObject
{
    public void GetIcon(string type, Action<Sprite> callback)
    {
    }

    public string GetText(string type)
    {
        return string.Empty;
    }

    public int stationCode { get; }
    public virtual float GetReward(float rewardCount, RewardFlag flag)
    {
        return rewardCount;
    }
}