using System;
using Module;
using Project.Data;
using UnityEngine;

public class StoreTimeReward : ShopItemChinese
{
    public DateTime deadLine
    {
        get { return LocalFileMgr.GetDateTime("StoreTimeReward"); }
        set { LocalFileMgr.SetDateTime("StoreTimeReward", value); }
    }
    
    public bool isGet
    {
        get { return LocalFileMgr.GetBool("StoreTimeRewardIsGet"); }
        set { LocalFileMgr.SetBool("StoreTimeRewardIsGet", value); }
    }
    
    public override void SetItem(RewardBag rewardBag)
    {
        base.SetItem(rewardBag);
        if (TimeHelper.now >= deadLine)
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
            var remainTim = deadLine - TimeHelper.now;
            remainTime.text = string.Format(Language.GetContent("1507"), Mathf.Clamp(remainTim.Hours, 0, 24), Mathf.Clamp(remainTim.Minutes, 0, 60), Mathf.Clamp(remainTim.Seconds, 0, 60));
            if (remainTim.TotalSeconds <= 0)
            {
                isGet = false;
                SetItem(this.reward);
            }
        }
    }
    
    
    protected override void DefaultListener()
    {
        reward.GetReward(res =>
        {
            if (res.result == IapResultMessage.Success)
            {
                isGet = true;
                deadLine = TimeHelper.now.AddSeconds(((AdsData) reward.iap.dbData).deadTune);
                RewardUI.OpenRewardUI(reward);
                SetItem(reward);
            }
        });
    }
}