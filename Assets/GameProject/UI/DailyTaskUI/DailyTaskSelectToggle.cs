using Module;
using UnityEngine;

public class DailyTaskSelectToggle: ToggleTree
{
    public GameObject selectImage;
    public GameObject dailyContent;
    public RedPoint redPoint;
    protected override void OnToggleChanged(ToggleTreeStation @from, ToggleTreeStation to)
    {
        selectImage.OnActive(to == ToggleTreeStation.On);
        interactable = to != ToggleTreeStation.On;
        dailyContent.OnActive(to == ToggleTreeStation.On);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        redPoint.SetTarget(DailyTaskManager.Instance.DailyTasks.ToArray());
    }
}