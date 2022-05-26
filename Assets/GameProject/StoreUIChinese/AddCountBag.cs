using Module;

public class AddCountBag : ShopItemChinese
{
    public int beishu
    {
        get
        {
            if (TimeHelper.IsNewDay(reward.iap.dbData.ID.ToString()))
            {
                beishu = 0;
                return 0;
            }
            else
            {
                return LocalFileMgr.GetInt("AddCountBag" + reward.iap.dbData.ID);
            }
        }
        set
        {
            if (value == 5) value = 0;
            LocalFileMgr.SetInt("AddCountBag" + reward.iap.dbData.ID, value);
        }
    }

    public override void SetItem(RewardBag rewardBag)
    {
        this.reward = rewardBag;
        rewardBag.product = (beishu + 1);
        base.SetItem(rewardBag);
        off.transform.parent.gameObject.OnActive(beishu > 0);
        remainTime.transform.parent.gameObject.OnActive(false);
        off.text = ConstKey.Cheng + reward.product;
        adsBuy.OnActive(true);
    }
    
    protected override void DefaultListener()
    {
        reward.GetReward(res =>
        {
            if (res.result == IapResultMessage.Success)
            {
                beishu++;
                RewardUI.OpenRewardUI(reward);
                SetItem(reward);
            }
        });
    }
}