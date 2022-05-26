using Module;

public class DailySupply : ShopItemChinese
{
    public bool isGet
    {
        get { return LocalFileMgr.GetBool("DailySupply"); }
        set { LocalFileMgr.SetBool("DailySupply", value); }
    }
    public override void SetItem(RewardBag rewardBag)
    {
        base.SetItem(rewardBag);
        if (TimeHelper.IsNewDay(rewardBag.iap.dbData.ID.ToString()))
        {
            isGet = false;
        }

        bool get = isGet;
        interactable = !get;
        remainTime.transform.parent.gameObject.OnActive(get);
        adsBuy.OnActive(!get);
        //off.transform.parent.gameObject.OnActive(false);
    }

    protected override void OnChildUpdate()
    {
        if (remainTime.gameObject.activeInHierarchy)
        {
            remainTime.text = string.Format(Language.GetContent("1507"), TimeHelper.remainTomorrow.Hours,
                TimeHelper.remainTomorrow.Minutes, TimeHelper.remainTomorrow.Seconds);
        }
    }

    protected override void DefaultListener()
    {
        reward.GetReward(res =>
        {
            if (res.result == IapResultMessage.Success)
            {
                isGet = true;
                RewardUI.OpenRewardUI(reward);
                SetItem(reward);
            }
        });
    }
}