using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using UnityEngine.UI;
using Project.Data;
using System;

public class BulletUIItem : UIBtnBase, IPoolObject
{
    public ObjectPool pool { get; set; }
    public Image icon;
    public Text title;
    public Text off;
    public Text costCount;
    public Image miniIcon;
    public Text rewardCount;
    public RewardBag reward;
    public GameObject adsButton;
    public GameObject BuyButton;
    public void ReturnToPool()
    {
        ObjectPool.ReturnToPool(this);
    }

    public void OnGetObjectFromPool()
    {
    }

    protected override void DefaultListener()
    {
        if (reward.stationCode == 0)
        {
            reward.GetReward(res =>
            {
                if (res.result == IapResultMessage.Success)
                {
                    EventCenter.Dispatch<int>(EventKey.BulletCreat, reward.content[0].finalCount.value.ToInt());
                    RewardUI.OpenRewardUI(reward);
                    AudioManager.PlayUI("zidan_cg");
                    // AudioPlay.Play("zidan_cg").SetIgnorePause(true);
                }
            });
        }
        else if (reward.stationCode == 2)
        {
            CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("704"), null);
        }
    }

    public void SetItem(RewardBag reward)
    {
        title.text = reward.GetText(TypeList.Title);
        reward.GetIcon(TypeList.High, sp => icon.sprite = sp);

        if (reward.iap is Currency currency)
        {
            adsButton.OnActive(false);
            BuyButton.OnActive(true);
            costCount.text = currency.price;
            IapData iapData = (IapData)currency.dbData;
            if (Math.Abs(iapData.sale) > 0.001)
            {
                off.transform.parent.gameObject.OnActive(true);
                off.text = (1 - iapData.sale).ToString("P0") + "\nOFF";
            }
            else
            {
                off.transform.parent.gameObject.OnActive(false);
            }
        }
        else if (reward.iap is AdsIap ads)
        {
            off.transform.parent.gameObject.OnActive(false);
            adsButton.OnActive(true);
            BuyButton.OnActive(false);
        }
        reward.GetIcon(TypeList.MiniIcon, sp =>
        {
            miniIcon.sprite = sp;
        });
        rewardCount.text = reward.GetText(TypeList.rewardCount);
        this.reward = reward;
    }
}
