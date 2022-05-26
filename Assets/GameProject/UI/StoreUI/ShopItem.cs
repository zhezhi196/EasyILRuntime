using System;
using Module;
using Project.Data;
using ProjectUI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : UIBtnBase, IPoolObject
{
    public ObjectPool pool { get; set; }
    public Image icon;
    public Text title;
    public Text off;
    public Text costCount;
    public RewardBag reward;
    public GameObject adsButton;
    public GameObject BuyButton;
    public Image[] rewardIcon;
    public Text[] rewardCount;
    public void ReturnToPool()
    {
        //ObjectPool.ReturnToPool(this);
    }

    public void OnGetObjectFromPool()
    {
    }

    protected override void DefaultListener()
    {
        reward.GetReward(res =>
        {
            if (res.result == IapResultMessage.Success)
            {
                RewardUI.OpenRewardUI(reward);
            }
        });
    }

    public virtual void SetItem(RewardBag reward)
    {
        title.text = reward.GetText(TypeList.Title);
        reward.GetIcon(TypeList.High, sp => icon.sprite = sp);
        for (int i = 0; i < rewardIcon.Length; i++)
        {
            rewardIcon[i].gameObject.OnActive(false);
        }
        for (int i = 0; i < reward.content.Length; i++)
        {
            rewardIcon[i].gameObject.OnActive(true);
            int index = i;
            reward.content[index].GetIcon(TypeList.High, sp => rewardIcon[index].sprite = sp);
            rewardCount[index].text = $"X {reward.content[i].finalCount*reward.product}";
        }
        if (reward.iap is Currency currency)
        {
            adsButton.OnActive(false);
            BuyButton.OnActive(true);
            costCount.text = currency.price;
            IapData iapData = (IapData) currency.dbData;
            if (Math.Abs(iapData.sale) > 0.001)
            {
                off.transform.parent.gameObject.OnActive(true);
                off.text = (1-iapData.sale).ToString("P0") + "\n"+Language.GetContent("1756");
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

        this.reward = reward;
    }

}