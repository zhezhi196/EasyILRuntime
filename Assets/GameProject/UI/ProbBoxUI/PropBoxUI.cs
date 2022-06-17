using System;
using Module;
using UnityEngine;
using UnityEngine.UI;

public class PropBoxUI: UIViewBase
{
    [Serializable]
    public struct MoneyGameo
    {
        public MoneyType monty;
        public GameObject go;
    }
    public UIBtnBase danbei;
    public UIBtnBase duobei;
    public Image rewardIcon;
    public MoneyGameo[] go;
    public Text rewardCount;
    public RewardBag bag;
    public RewardContent content;
    private SupplyBoxProp supplyBox;
    protected override void OnChildStart()
    {
        danbei.AddListener(OnDanbei);
        duobei.AddListener(OnDuobei);
    }

    public override void Refresh(params object[] args)
    {
        base.Refresh(args);
        SupplyBoxProp box = args[0] as SupplyBoxProp;
        for (int i = 0; i < go.Length; i++)
        {
            if (go[i].monty == box.type)
            {
                go[i].go.OnActive(true);
            }
            else
            {
                go[i].go.OnActive(false);
            }
        }
        MoneyInfo info=MoneyInfo.GetMoneyEntity(box.type);
        supplyBox = box;
        info.GetIcon(TypeList.High, sp => { rewardIcon.sprite = sp; });
        rewardCount.text = ConstKey.Cheng + box.creator.count;
        content = Commercialize.GetRewardContent(info.dbData.ID, box.creator.count);
    }

    private void OnDuobei()
    {
        bag = (RewardBag) Commercialize.GetRewardBag(DataMgr.CommonData(33005).ToInt());
        bag.GetReward(res =>
        {
            if (res.result == IapResultMessage.Success)
            {
                Get(5);
            }
        });
    }

    private void OnDanbei()
    {
        Get(1);
    }

    public void Get(float beishu)
    {
        content.GetReward(beishu, 0);
        UISequence seq = new UISequence();
        UIController.Instance.Close("SupplyBox", UITweenType.None);
        seq.Add("RewardUI", UITweenType.None, OpenFlag.Inorder, content);
        seq.Open();
        seq.OnComplete(() =>
        {
            UIController.Instance.Open("GameUI", UITweenType.None, OpenFlag.Insertion);
            if (supplyBox != null)
            {
                supplyBox.OnOpen();
            }
        });
    }
}