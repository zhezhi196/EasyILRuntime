using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using UnityEngine.UI;
using DG.Tweening;
using SpringGUI;
using UnityEngine.Assertions;
using UnityEngine.Video;
using GameGift;

public class UIRaderData
{
    public GiftBranchType giftType;
    public float value { get; set; }

    public UIRaderData(float value, GiftBranchType type)
    {
        this.value = value;
        this.giftType = type;
    }
}

public class GiftUI : UIViewBase
{
    [System.Serializable]
    public class GiftUIConfig
    {
        public Toggle toggle;
        [HideInInspector]
        public Vector3 startScale = Vector3.one;
        public GiftBranchType branchType;
        public GameObject rightGroup;
        public GiteUIItem[] items;
    }
    public Text giftName;
    public Text giftDes;
    public UIBtnBase studyBtn;
    public Text giftCost;
    public GameObject giftPreview;
    public UIBtnBase backBtn;
    public RadarMap raderMap;
    [Sirenix.OdinInspector.BoxGroup("天赋UI配置")]
    public List<GiftUIConfig> giftUIConfigs = new List<GiftUIConfig>();
    public Transform leftGroup;
    public Transform rightGroup;
    private int selectBranch = 0;
    private GiteUIItem selectGiftItem;
    public GiftVideoComponent videoComponent = null; // 视频播放组件
    protected override void OnChildStart()
    {
        studyBtn.AddListener(OnStudy);
        backBtn.AddListener(BackBtn);
        leftGroup.localScale = Tools.GetScreenScale();
        rightGroup.localScale = Tools.GetScreenScale();
        
        //初始化视频组件
        videoComponent = giftPreview.GetComponent<GiftVideoComponent>();
        videoComponent.Init();
        //初始化天赋按钮
        for (int i = 0; i < giftUIConfigs.Count; i++)
        {
            int ii = i;
            giftUIConfigs[ii].startScale = giftUIConfigs[ii].toggle.transform.localScale;
            giftUIConfigs[ii].toggle.onValueChanged.AddListener((b) =>
            {
                SelectGiftBranch(ii, b);
                giftUIConfigs[ii].toggle.graphic.gameObject.OnActive(b);
            });
            giftUIConfigs[ii].items = giftUIConfigs[ii].rightGroup.GetComponentsInChildren<GiteUIItem>();
            giftUIConfigs[ii].rightGroup.GetComponent<CanvasGroup>().alpha = 0;
            Voter voter = new Voter(giftUIConfigs[ii].items.Length, () => {
                giftUIConfigs[ii].rightGroup.GetComponent<CanvasGroup>().DOFade(1, 1f).SetUpdate(true).SetId(winName);
            });
            List<Gift> gifts = BattleController.GetCtrl<GiftCtrl>().gift.FindAll(g => g.branchType == giftUIConfigs[ii].branchType);
            gifts.Sort((a, b) => a.dbData.index.CompareTo(b.dbData.index));
            if (gifts[0].station == GiftStation.Locked)
            {
                gifts[0].Unlock();
            }
            for (int j = 0; j < giftUIConfigs[ii].items.Length; j++)
            {
                int jj = j;
                giftUIConfigs[ii].items[jj].Init(gifts[jj], () =>
                {
                    voter.Add();
                });
                giftUIConfigs[ii].items[jj].toggle.onValueChanged.AddListener((b) =>
                {
                    if (b)
                    {
                        SelectGiftItem(giftUIConfigs[ii].items[jj]);
                    }
                });
            }
            GameDebug.LogError("GiftUI 初始化天赋按钮结束"+i);
        }
    }

    public override void Refresh(params object[] args)
    {
        if (model.direction == UIOpenDirection.Backward) return;
        giftUIConfigs[0].toggle.isOn = true;
        RefreshRadar();
        
        // Assert.IsTrue(selectGiftItem != null , "selectGiftItem == null");
        // videoComponent.Play(selectGiftItem.gift);
    }

    private void RefreshRadar()
    {
        List<UIRaderData> radarone = new List<UIRaderData>();
        for (int i = 0; i < giftUIConfigs.Count; i++)
        {
            float temp = 0;
            for (int j = 0; j < giftUIConfigs[i].items.Length; j++)
            {
                if (giftUIConfigs[i].items[j].gift.station == GiftStation.Owned)
                {
                    temp += 1f;
                }
            }
            temp = temp / giftUIConfigs[i].items.Length;
            UIRaderData data = new UIRaderData(Mathf.Clamp(temp, 0.1f, 1f), giftUIConfigs[i].branchType);
            radarone.Add(data);
        }
        raderMap.RadarBaseData.RemoveAllData();
        raderMap.Inject(radarone);
    }

    private void SelectGiftBranch(int index,bool b)
    {
        giftUIConfigs[index].toggle.transform.localScale =b? Vector3.one : giftUIConfigs[index].startScale;
        giftUIConfigs[index].rightGroup.OnActive(b);
        if (b)
        {
            selectBranch = index;
            //刷新天赋
            int itemIndex = -1;
            for (int i = 0; i < giftUIConfigs[index].items.Length; i++)
            {
                giftUIConfigs[index].items[i].Refesh();
                if (giftUIConfigs[index].items[i].gift.station == GiftStation.Owned)
                {
                    itemIndex = i;
                }
            }
            if (itemIndex == -1)
            {
                itemIndex = 0;
            }
            else if (itemIndex >= giftUIConfigs[index].items.Length - 1)
            {
                itemIndex = giftUIConfigs[index].items.Length - 1;
            }
            else {
                itemIndex += 1;
            }
            if (giftUIConfigs[index].items[itemIndex].gift.station == GiftStation.Locked)
            {
                giftUIConfigs[index].items[itemIndex].Unlock();
            }
            giftUIConfigs[index].items[itemIndex].toggle.isOn = true;
            SelectGiftItem(giftUIConfigs[index].items[itemIndex]);
        }
    }

    private void SelectGiftItem(GiteUIItem gift)
    {
        selectGiftItem = gift;
        studyBtn.OnActive(selectGiftItem.gift.station == GiftStation.Running);
        if (selectGiftItem.gift.station == GiftStation.Owned)
        { 
        
        }
        //刷新天赋详情
        giftCost.text = selectGiftItem.gift.dbData.cost.ToString();
        giftName.text = Language.GetContent(selectGiftItem.gift.dbData.title.ToString());
        giftDes.text = Language.GetContent(selectGiftItem.gift.dbData.describe.ToString());
        //刷新视频播放
        videoComponent.Play(selectGiftItem.gift);
    }

    //学习天赋
    private void OnStudy()
    {
        Cost cost = new Cost(MoneyInfo.ConvertType(selectGiftItem.gift.dbData.costType), selectGiftItem.gift.dbData.cost);
        if (MoneyInfo.Spend(0, cost))
        {
            selectGiftItem.OnStudy();
            AnalyticsEvent.SendEvent(AnalyticsType.GiftStudy, selectGiftItem.gift.dbData.ID.ToString(),false);
            SelectGiftBranch(selectBranch, true);
            RefreshRadar();
            //AudioPlay.PlayOneShot("shengji_cg");
            //AudioPlay.PlayOneShot("Gift").SetIgnorePause(true);
        }
    }

    private void BackBtn()
    {
        UIController.Instance.Back();
    }

    public void PlayAudio()
    {
        //AudioPlay.PlayOneShot(Config.globleConfig.commonButtonAudio).SetIgnorePause(true);
    }
}
