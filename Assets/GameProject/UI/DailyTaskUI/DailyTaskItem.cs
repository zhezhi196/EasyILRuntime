using System;
using Module;
using UnityEngine;
using UnityEngine.UI;

public class DailyTaskItem : UIBtnBase
{
    public Image taskIcon;
    public Text taskName;
    public Text taskDes;
    public Image taskRewardIcon;
    public Text taskRewardCount;
    public Text taskProcess;
    public DailyTaskBase task;
    public Color completeColor;
    public Sprite normalSprite;
    public Sprite completeSprite;
    public Slider slider;
    public GameObject kelingqu;
    public Color unRewardColor;
    public Color unCompleteColor;
    public Image fillImage;
    public UIBtnBase GetBtn;
    public Text GetBtnText;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        if (Application.isPlaying)
        {
            DailyTaskManager.Instance.OnDailyTaskStationEvent += OnDailyTaskStation;
        }
    }



    protected override void OnDisable()
    {
        base.OnDisable();
        if (Application.isPlaying)
        {
            DailyTaskManager.Instance.OnDailyTaskStationEvent -= OnDailyTaskStation;
        }
    }
    
    private void OnDailyTaskStation(DailyTaskBase arg1, TaskStation arg2)
    {
        if (arg1 == task)
        {
            StationChanged();
        }
    }

    private void StationChanged()
    {
        interactable = task.station == TaskStation.CompleteUnReward;
        if (task.station == TaskStation.Rewarded)
        {
            slider.fillRect.gameObject.OnActive(false);
        }
        else if (task.station == TaskStation.CompleteUnReward)
        {
            slider.fillRect.gameObject.OnActive(true);
            fillImage.color = unRewardColor;
        }
        else if (task.station == TaskStation.UnComplete)
        {
            slider.fillRect.gameObject.OnActive(true);
            fillImage.color = unCompleteColor;
        }
        
        slider.maxValue = task.taskReward.completeCount;
        slider.value = task.currentCount;
        slider.targetGraphic.SetAlpha(task.station == TaskStation.Rewarded ? 0.5f : 0.8784314f);
        kelingqu.gameObject.OnActive(task.station == TaskStation.CompleteUnReward);
        slider.gameObject.OnActive(task.station == TaskStation.UnComplete);
        GetBtn.gameObject.OnActive(task.station != TaskStation.UnComplete);
        if (task.station == TaskStation.UnComplete)
        {
            taskProcess.text = task.currentCount + "/" + task.taskReward.completeCount;
            taskProcess.color = Color.white;
            //GetComponent<Image>().sprite = completeSprite;
        }
        else if (task.station == TaskStation.CompleteUnReward)
        {
            taskProcess.text = Language.GetContent("1467");
            taskProcess.color = Color.white;
            GetBtnText.text = Language.GetContent("1467");
            GetBtn.interactable = true;
            //GetComponent<Image>().sprite = normalSprite;
        }
        else if (task.station == TaskStation.Rewarded)
        {
            taskProcess.text = Language.GetContent("1470");
            taskProcess.color = completeColor;
            GetBtnText.text = Language.GetContent("1470");
            GetBtn.interactable = false;
            //GetComponent<Image>().sprite = completeSprite;
        }
    }

    protected override void DefaultListener()
    {
        SpriteLoader.LoadIcon("GameUI_guanggao", sp =>
        {
            CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("1303"), null, new PupupOption(() =>
            {
                task.GetReward(1, 0);
            }, Language.GetContent("1326")), new PupupOption(() =>
            {
                task.GetReward(2, 0);
            }, Language.GetContent("1327"), sp));
        });
    }

    public void SetItem(DailyTaskBase item, Action callback)
    {
        this.task = item;
        taskName.text = item.GetText(TypeList.Title);
        taskDes.text = item.GetText(TypeList.Des);

        taskRewardCount.text = item.taskReward.rewardContent.finalCount.value.ToString();
        item.taskReward.rewardContent.GetIcon(TypeList.High, sp => taskRewardIcon.sprite = sp);
        item.GetIcon(null, sp =>
        {
            taskIcon.sprite = sp;
            callback?.Invoke();
        });
        StationChanged();
    }
}