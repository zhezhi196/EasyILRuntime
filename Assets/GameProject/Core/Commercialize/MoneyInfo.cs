using System;
using Module;
using Project.Data;

[Flags]
public enum MoneySpendFlag
{
    NotToShop = 1,
}
public abstract class MoneyInfo : PropEntity
{
    public static Action<MoneyType, int> onMoneyChanged;

    public static MoneyType ConvertType(int type)
    {
        if (type == 1) return MoneyType.Parts;
        if (type == 2) return MoneyType.Alloy;
        if (type == 3) return MoneyType.Memory;
        return MoneyType.Parts;
    }

    public static void OwnMoney(MoneyType type, int count)
    {
        MoneyInfo tar = GetMoneyEntity(type);
        tar.GetReward(count, RewardFlag.NoAudio);
    }

    public static BoolField Spend(MoneySpendFlag flag, params Cost[] cost)
    {
        if (cost.IsNullOrEmpty()) return new BoolField(false);
        for (int i = 0; i < cost.Length; i++)
        {
            MoneyInfo tar = GetMoneyEntity(cost[i].type);
            if (!tar.HasEnoughMoney(cost[i].cost).value && (flag & MoneySpendFlag.NotToShop) == 0)
            {
                ShowLackMoney(tar.moneyType);
                return new BoolField(false);
            }
        }

        bool temp = true;
        for (int i = 0; i < cost.Length; i++)
        {
            MoneyInfo tar = GetMoneyEntity(cost[i].type);
            temp = temp && tar.SpendMoney(cost[i].cost).value;
        }

        return new BoolField(temp);
    }

    public static void ShowLackMoney(MoneyType moneyType)
    {
        string content = null;
        if (moneyType == MoneyType.Alloy)
        {
            content = Language.GetContent("1711");
        }
        else if (moneyType == MoneyType.Parts)
        {
            content = Language.GetContent("1710");
        }
        else if (moneyType == MoneyType.Memory)
        {
            content = Language.GetContent("1712");
        }

        CommonPopup.Popup(Language.GetContent("701"), content, null,
            new PupupOption(() =>
                {
                    Commercialize.OpenStore();
                },
                Language.GetContent("702")));
    }

    public static MoneyInfo GetMoneyEntity(MoneyType type)
    {
        for (int i = 0; i < entityList.Count; i++)
        {
            if (entityList[i] is MoneyInfo info)
            {
                if (info.moneyType == type)
                {
                    return info;
                }
            }
        }

        return null;
    }

    public static BoolField Enough(Cost[] cost,out MoneyInfo lack)
    {
        for (int i = 0; i < cost.Length; i++)
        {
            MoneyInfo info = GetMoneyEntity(cost[i].type);
            if (!info.HasEnoughMoney(cost[i].cost))
            {
                lack = info;
                return new BoolField(false);
            }
        }

        lack = null;
        return new BoolField(true);
    }

    #region MoneyInfo

    protected IntField _count;

    public abstract MoneyType moneyType { get; }
    public virtual int maxGetCount { get; set; }
    public int count
    {
        get { return _count; }
    }

    protected string saveData
    {
        get { return EncryptionHelper.AesEncrypt(this.count.ToString(), PlayerInfo.pid); }
    }

    public MoneyInfo(PropData dbData) : base(dbData)
    {
    }

    public override float GetReward(float rewardCount,int editorId, string[] match, RewardFlag flag)
    {
        base.GetReward(rewardCount, editorId, match, RewardFlag.NoAudio);
        _count = new IntField((_count + rewardCount).ToInt(), FieldAesType.Aes);
        onMoneyChanged?.Invoke(moneyType, (int) rewardCount);
        BattleController.Instance.Save(0);
        return rewardCount;
    }

    public virtual BoolField SpendMoney(IntField spendCount)
    {
        if (HasEnoughMoney(spendCount))
        {
            _count = new IntField((_count - spendCount), FieldAesType.Aes);
            onMoneyChanged?.Invoke(moneyType, spendCount);
            BattleController.Instance.Save(0);
            return new BoolField(true);
        }

        return new BoolField(false);
    }

    public BoolField HasEnoughMoney(IntField spendCount)
    {
        return new BoolField(spendCount >= 0 && this.count >= spendCount);
    }


    public void GMSetCount(int cnt)
    {
#if LOG_ENABLE
        _count = new IntField(cnt, FieldAesType.Aes);
#endif
    }

    

    #endregion
}