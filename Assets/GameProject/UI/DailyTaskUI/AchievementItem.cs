using System;
using Module;
using UnityEngine;
using UnityEngine.UI;

public class AchievementItem : UIBtnBase
{
    public GameObject completeEffect;
    public Image achIcon;
    public Text achTitle;
    public Text achDes;
    public Image rewardIcon;
    public Image skinIcon;

    public Image rewardIconFinal
    {
        get
        {
            if (achievement.currReward.reward[0].reward is SkinEntity)
            {
                rewardIcon.transform.parent.OnActive(false);
                skinIcon.transform.parent.OnActive(true);
                return skinIcon;
            }
            else
            {
                rewardIcon.transform.parent.OnActive(true);
                skinIcon.transform.parent.OnActive(false);
                return rewardIcon;
            }
        }
    }
    public void SetRewardIconActive(bool active)
    {
        rewardIcon.transform.parent.gameObject.OnActive(active);
        skinIcon.transform.parent.gameObject.OnActive(active);
    }
    public Text rewardCount;
    public AchievementBase achievement;
    public Sprite normal;
    public Sprite canReward;
    public Sprite rewarded;
    public Image statusImage;
    public Text complete;
    public GameObject ChengJiu_An;
    public GameObject ChengJiu_Zhong;
    public GameObject ChengJiu_Liang;
    public void SetItem(AchievementBase achievement, Action callback)
    {
        //return;
        this.achievement = achievement;
        achievement.GetIcon(null, sp =>
        {
            achIcon.sprite = sp;
            callback?.Invoke();
        });
        achTitle.text = achievement.GetText(TypeList.Title);
        achDes.text = achievement.GetText(TypeList.Des);
        if (!achievement.isComplete)
        {
            SetRewardIconActive(true);
            complete.gameObject.OnActive(false);
            achievement.currReward.reward[0].GetIcon(TypeList.High, sp =>
            {
                rewardIconFinal.sprite = sp;
                //rewardIcon.SetNativeSize();
            });
            if (achievement.currReward.reward[0].finalCount.value > 1)
            {
                rewardCount.gameObject.OnActive(true);
                rewardCount.text = achievement.currReward.reward[0].finalCount.value.ToString();
            }
            else
            {
                rewardCount.gameObject.OnActive(false);
            }
        }
        else
        {
            SetRewardIconActive(false);
            complete.gameObject.OnActive(true);
        }


        achievement.OnChangeCount += OnChangeCount;
        OnChangeReward(achievement.currReward);
    }

    protected override void DefaultListener()
    {
        //TODO 
         if (achievement.currReward.station == TaskStation.CompleteUnReward)
         {
             RewardUI.OpenRewardUI(achievement.currReward.reward[0]);
             achievement.GetReward(1, 0);
         }
    }

    private void OnChangeCount()
    {
        achDes.text = achievement.GetText(TypeList.Des);
    }

    private void OnChangeReward(AchievementReward ach)
    {
        interactable = ach.station == TaskStation.CompleteUnReward;
        completeEffect.gameObject.OnActive(ach.station == TaskStation.CompleteUnReward);
        ChengJiu_Zhong.gameObject.OnActive(ach.station == TaskStation.UnComplete);
        ChengJiu_Liang.gameObject.OnActive(ach.station == TaskStation.CompleteUnReward);
        ChengJiu_An.gameObject.OnActive(ach.station == TaskStation.Rewarded);
        /*if (ach.station == TaskStation.UnComplete)
        {
            statusImage.color = Color.white;
            statusImage.sprite = normal;
        }
        else if (ach.station == TaskStation.CompleteUnReward)
        {
            statusImage.color = new Color(1, 1, 1, 0.34f);
            statusImage.sprite = canReward;
        }
        else if (ach.station == TaskStation.Rewarded)
        {
            statusImage.color = Color.white;
            statusImage.sprite = rewarded;
        }*/
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        achievement.OnChangeCount -= OnChangeCount;
    }
}