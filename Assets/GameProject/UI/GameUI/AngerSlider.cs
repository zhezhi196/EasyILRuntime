using System;
using UnityEngine;
using UnityEngine.UI;

public class AngerSlider: MonoBehaviour
{
    public Slider angerSlider;
    private void Start()
    {
        Player.player.onChangedAtt += OnPlayerChangedAtt;
        Player.player.onChangeAnger += OnChangeAnger;


        angerSlider.value = 0;
        SetAnger();
    }

    private void OnDestroy()
    {
        Player.player.onChangedAtt -= OnPlayerChangedAtt;
        Player.player.onChangeAnger -= OnChangeAnger;
    }
    
    private void OnPlayerChangedAtt()
    {
        SetAnger();
    }
    
    private void OnChangeAnger(float obj)
    {
        angerSlider.value = Mathf.Clamp(obj, angerSlider.minValue, angerSlider.maxValue);
    }

    private void SetAnger()
    {
        angerSlider.minValue = 0;
        angerSlider.maxValue = Player.player.finalAtt.anger;
    }
}