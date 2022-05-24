using System;
using Module;
using UnityEngine;

public class Talent : IRewardObject
{
    public static GameAttribute talentAttribute;

    public static AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        talentAttribute = new GameAttribute(0);
        return process;
    }

    public GameAttribute attribute;
    public void GetIcon(string type, Action<Sprite> callback)
    {
    }

    public string GetText(string type)
    {
        return type;
    }

    public int stationCode { get; }

    public float GetReward(float rewardCount, RewardFlag flag)
    {
        return rewardCount;
    }
}