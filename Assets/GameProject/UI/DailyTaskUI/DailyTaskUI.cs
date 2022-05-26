using System;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using SDK;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Ach;

public class DailyTaskUI : UIViewBase
{
    public Transform Root;
    public ToggleTree panle;
    public UIBtnBase refreshDailyTask;
    public UIBtnBase backBtn;
    public Transform dailyTaskContent;
    public Transform achievementContent;

    [ReadOnly] public List<DailyTaskItem> taskItem = new List<DailyTaskItem>();
    [ReadOnly] public List<AchievementItem> achievementItems = new List<AchievementItem>();
    public CanvasGroup group;

    protected override void OnChildStart()
    {
        refreshDailyTask.AddListener(OnDailyTaskRefresh);
        backBtn.AddListener(OnBack);
        DailyTaskManager.Instance.OnRefreshDailyTaskEvent += RefreshDailyTask;
        AchievementManager.Instance.onChangeReward += OnChangeAchievement;
        Root.localScale = Tools.GetScreenScale();
    }


    public override void OnOpenStart()
    {
        //SDKMgr.GetInstance().MyAdSDK.EntryRewardVideoAdScene(string.Empty);
        string tag = model.args[0].ToString();
        if (tag == "DailyTask")
        {
            panle.NotifyToggleOn(0, false);
        }
        else if (tag == "Achievement")
        {
            panle.NotifyToggleOn(1, false);
        }
    }

    private void OnDestroy()
    {
        DailyTaskManager.Instance.OnRefreshDailyTaskEvent -= RefreshDailyTask;
        AchievementManager.Instance.onChangeReward -= OnChangeAchievement;
    }

    private void RefreshDailyTask(List<DailyTaskBase> obj)
    {
        for (int i = 0; i < taskItem.Count; i++)
        {
            ObjectPool.ReturnToPool(taskItem[i]);
        }

        taskItem.Clear();
        group.alpha = 0;
        var horLayout = dailyTaskContent.GetComponent<HorizontalLayoutGroup>();
        horLayout.enabled = false;
        Voter voter = new Voter(obj.Count, () =>
        {
            dailyTaskContent.Sort((a, b) => a.task.index.CompareTo(b.task.index), taskItem);
            horLayout.enabled = true;
            @group.DOFade(1, 0.2f).SetUpdate(true);
        });
        for (int i = 0; i < obj.Count; i++)
        {
            var tt = DailyTaskManager.Instance.DailyTasks[i];
            LoadPrefab<DailyTaskItem>("DailyTaskItem", dailyTaskContent, item =>
            {
                taskItem.Add(item);
                item.SetItem(tt, voter.Add);
            });
        }
    }

    public void RefreshAchievement()
    {
        for (int i = 0; i < achievementItems.Count; i++)
        {
            ObjectPool.ReturnToPool(achievementItems[i]);
        }

        achievementItems.Clear();
        group.alpha = 0;
        var achieves = AchievementManager.Instance.Achievements;

        Voter voter = new Voter(achieves.Count, () =>
        {
            achievementContent.Sort((a, b) =>
            {
                if (a.achievement.currReward.station != b.achievement.currReward.station)
                {
                    if (a.achievement.currReward.station == TaskStation.CompleteUnReward) return -1;
                    if (b.achievement.currReward.station == TaskStation.CompleteUnReward) return 1;

                    if (a.achievement.currReward.station == TaskStation.Rewarded) return 1;
                    if (b.achievement.currReward.station == TaskStation.Rewarded) return -1;

                    return a.achievement.currReward.dbData.ID.CompareTo(b.achievement.currReward.dbData.ID);
                }
                else
                {
                    return a.achievement.currReward.dbData.ID.CompareTo(b.achievement.currReward.dbData.ID);
                }
            }, achievementItems);
            @group.DOFade(1, 0.2f).SetUpdate(true);
        });

        for (int i = 0; i < achieves.Count; i++)
        {
            var temAcg = achieves[i];
            if (temAcg.stationCode == 0)
            {
                LoadPrefab<AchievementItem>("AchieveItem", achievementContent, item =>
                {
                    achievementItems.Add(item);
                    item.SetItem(temAcg, voter.Add);
                });
            }
            else
            {
                voter.Add();
            }
        }
    }

    private void OnBack()
    {
        OnExit();
    }

    private void OnDailyTaskRefresh()
    {
        ((RewardBag) Commercialize.GetRewardBag(DataMgr.CommonData(33003).ToInt())).GetReward(rt =>
        {
            if (rt.result == IapResultMessage.Success)
            {
                DailyTaskManager.Instance.RefreshNewTask(DailyTaskManager.defaultTaskCount, 0);
            }
        });
    }


    private void OnChangeAchievement(AchievementBase arg1, AchievementReward arg2)
    {
        RefreshAchievement();
    }


    public override void Refresh(params object[] args)
    {
        RefreshDailyTask(DailyTaskManager.Instance.DailyTasks);
        RefreshAchievement();
    }
}