using System;
using System.Collections.Generic;
using Module;
using UnityEngine;

public class GameReward : IRewardBag
{
    private FloatField _product = new FloatField(1);
    public List<Cost> cost { get; }
    public IGameRewardData dbData { get; }

    public override string ToString()
    {
        return dbData.ID.ToString();
    }

    public void GetIcon(string type, Action<Sprite> callback)
    {
        if (type == TypeList.Cost)
        {
            MoneyInfo.GetMoneyEntity(cost[0].type).GetIcon(TypeList.Normal, callback);
        }
        else if (type == TypeList.Normal)
        {
            SpriteLoader.LoadIcon(dbData.Icon, callback);
        }
        else
        {
            content[0].GetIcon(type, callback);
        }
    }

    public int stationCode
    {
        get
        {
            if (!content.IsNullOrEmpty())
            {
                for (int i = 0; i < content.Length; i++)
                {
                    if (content[i].stationCode != 0) return content[i].stationCode;
                }
            }
            return 0;
        }
    }

    public float product
    {
        get { return _product.value; }
        set { _product = new FloatField(value); }
    }

    public RewardContent[] content { get; }

    public GameReward(IGameRewardData data)
    {
        this.dbData = data;
        string[] rewardSpite = data.rewardId.Split(ConstKey.Spite0);
        string[] rewardCount = data.rewardCount.Split(ConstKey.Spite0);
        content = new RewardContent[rewardSpite.Length];
        for (int i = 0; i < content.Length; i++)
        {
            content[i] = new RewardContent(Commercialize.GetReward(rewardSpite[i].ToInt()), rewardCount[i].ToInt());
        }

        if (data.alloyCost != 0 || data.partsCost != 0 || data.memoryCost != 0)
        {
            cost = new List<Cost>();
            if (data.alloyCost != 0)
            {
                cost.Add(new Cost(MoneyType.Alloy, data.alloyCost));
            }

            if (data.partsCost != 0)
            {
                cost.Add(new Cost(MoneyType.Parts, data.partsCost));
            }
            
            if (data.memoryCost != 0)
            {
                cost.Add(new Cost(MoneyType.Memory, data.memoryCost));
            }
        }
        
    }

    public float GetReward(float rewardCount, RewardFlag flag)
    {
        MoneyInfo lack = null;
        if (MoneyInfo.Enough(cost.ToArray(), out lack).value && MoneyInfo.Spend(0, cost.ToArray()).value)
        {
            float result = 0;
            if (content.Length == 0)
            {
                result = 0;
            }
            else if (content.Length == 1)
            {
                result = content[0].GetReward(rewardCount, flag);
            }
            else
            {
                for (int i = 0; i < content.Length; i++)
                {
                    content[i].GetReward(rewardCount, flag);
                }

                result = rewardCount;
            }
            return result;
        }
        else
        {
            MoneyInfo.ShowLackMoney(lack.moneyType);
            return 0;
        }
    }

    
    public string GetText(string type)
    {
        switch (type)
        {
            case TypeList.Title:
                if (!dbData.title.IsNullOrEmpty())
                {
                    return Language.GetContent(dbData.title);
                }
                else
                {
                    if (content.Length == 1)
                    {
                        return content[0].GetText(type);
                    }
                    else
                    {
                        GameDebug.LogError($"IapData{dbData.ID}奖励数量{content.Length},无法获取图标");
                    }
                }
                break;
            case TypeList.rewardCount:
                return ConstKey.Cheng + content[0].finalCount;
            case TypeList.GetDes:
                if (content.Length == 1)
                {
                    return content[0].GetText(type);
                }
                else
                {
                    GameDebug.LogError($"IapData{dbData.ID}奖励数量{content.Length},无法获取图标");
                    return null;
                }
        }
        return null;
    }
}