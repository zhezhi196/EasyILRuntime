using System;
using Module;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemChinese : UIBtnBase
{
    [Serializable]
    public struct RewardInfo
    {
        public Image rewardIcon;
        public Text rewardCount;
        public GameObject rewardObj;
    }
    public Text titile;
    public Text off;
    public Image icon;
    public RewardInfo[] rewardInfo;

    public GameObject adsBuy;
    public Text remainTime;

    public RewardBag reward;

    public virtual float rewardBeishu
    {
        get { return 1; }
    }

    public virtual void SetItem(RewardBag rewardBag)
    {
        this.reward = rewardBag;
        this.titile.text = rewardBag.GetText(TypeList.Title);
        rewardBag.GetIcon(TypeList.High, sp => { this.icon.sprite = sp; });
        for (int i = 0; i < rewardInfo.Length; i++)
        {
            if (i < rewardBag.content.Length)
            {
                int iund = i;
                rewardInfo[iund].rewardIcon.gameObject.OnActive(true);
                rewardInfo[iund].rewardObj.OnActive(true);
                rewardBag.content[iund].GetIcon(TypeList.High, sp =>
                {
                    rewardInfo[iund].rewardIcon.sprite = sp;
                });
                rewardInfo[iund].rewardCount.text = ConstKey.Cheng + (rewardBag.content[iund].dbCount*rewardBag.product);
            }
            else
            {
                rewardInfo[i].rewardIcon.gameObject.OnActive(false);
                rewardInfo[i].rewardObj.OnActive(false);
            }
        }

        adsBuy.gameObject.OnActive(true);
        remainTime.transform.parent.gameObject.OnActive(false);
    }

    protected override void DefaultListener()
    {

    }
}