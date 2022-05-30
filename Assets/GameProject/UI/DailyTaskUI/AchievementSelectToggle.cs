﻿using Module;
using UnityEngine;

public class AchievementSelectToggle: ToggleTree
{
    public GameObject selectImage;
    public GameObject achievementContent;
    public RedPoint redPoint;

    protected override void OnToggleChanged(ToggleTreeStation @from, ToggleTreeStation to)
    {
        selectImage.OnActive(to == ToggleTreeStation.On);
        interactable = to != ToggleTreeStation.On;
        achievementContent.OnActive(to == ToggleTreeStation.On);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        redPoint.SetTarget(AchievementManager.Instance.Achievements.ToArray());
    }
}