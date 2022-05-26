using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Module;
using System.Reflection;
using System;

/// <summary>
/// 武器升级武器属性slider组件
/// </summary>
public class UpgradeSlider : MonoBehaviour
{
    public Image currentSlider;
    public float sliderLength = 100;
    public Image nextSlider;
    public Text attName;
    public Text maxLabel;
    public Text currentLabel;
    public Image upImage;

    private string upText = "<size=35><color=#FFD179>({0})</color></size>";
    private GameAttribute currentAtt;
    private float currentData = 0;
    private GameAttribute nextPart;
    private float growData = 0;
    private GameAttribute maxAtt;
    private float maxData;
    public GameObject upEffect;
    public float effectTime = 1.5f;
    private int effectCount = 0;
    //private Clock effectClock;
    private Weapon lastWeapon;
    //private void Start()
    //{
    //    effectClock = new Clock(effectTime);
    //    effectClock.ignorePause = true;
    //    effectClock.onComplete += ClockComplete;
    //}
    public void Refesh(Weapon w,WeaponUpFiled filed)
    {
        if (lastWeapon != w)
        {
            Async.StopAsync(gameObject);
            lastWeapon = w;
            upEffect.OnActive(false);
            effectCount = 0;
        }
        //基础属性
        GameAttribute baseAtt = w.baseAttribute;
        PropertyInfo[] baseProperty = baseAtt.GetType().GetProperties();
        float baseData = baseProperty[filed.index].GetValue(baseAtt).ToFloat();
        //满属性计算
        maxAtt = w.MaxAttribute;
        PropertyInfo[] maxProperty = maxAtt.GetType().GetProperties();
        maxData = maxProperty[filed.index].GetValue(maxAtt).ToFloat();
        //查找升级的属性名
        attName.text = WeaponManager.GetAttName(w, filed.filed);
        //当前属性,和下一级属性
        if (w.level >= WeaponManager.weaponAllPartDataDic[w.weaponID].Count)
        {
            currentData = maxData;
            growData = 0;
        }
        else {
            currentAtt = w.weaponAttribute;
            PropertyInfo[] curretnProperty = currentAtt.GetType().GetProperties();
            currentData = curretnProperty[filed.index].GetValue(currentAtt).ToFloat();
            nextPart = WeaponManager.weaponAllPartDataDic[w.weaponID][w.level].attribute;
            PropertyInfo[] nextProperty = nextPart.GetType().GetProperties();
            growData = nextProperty[filed.index].GetValue(nextPart).ToFloat();
        }
        maxLabel.text = maxData.ToString();

        if (growData != 0)
        {
            currentLabel.text = currentData+string.Format(upText, (growData>0)?("+"+ growData.ToString()): growData.ToString());
            nextSlider.gameObject.OnActive(true);
            upImage.gameObject.OnActive(true);
        }
        else {
            currentLabel.text = currentData.ToString();
            nextSlider.gameObject.OnActive(false);
            upImage.gameObject.OnActive(false);
        }
        if (maxData >= baseData)
        {
            currentSlider.rectTransform.sizeDelta = new Vector2(sliderLength * (currentData / maxData), currentSlider.rectTransform.sizeDelta.y);
            nextSlider.rectTransform.sizeDelta = new Vector2(sliderLength * (growData / maxData) + 24f, nextSlider.rectTransform.sizeDelta.y);
            nextSlider.rectTransform.anchoredPosition = new Vector2(-12f, nextSlider.rectTransform.anchoredPosition.y);
        }
        else {
            currentSlider.rectTransform.sizeDelta = new Vector2(sliderLength * ((currentData-maxData) /(baseData- maxData)), currentSlider.rectTransform.sizeDelta.y);
            nextSlider.rectTransform.sizeDelta = new Vector2(sliderLength * (Mathf.Abs(growData)/ (baseData - maxData)) + 24f, nextSlider.rectTransform.sizeDelta.y);
            nextSlider.rectTransform.anchoredPosition = new Vector2(-(nextSlider.rectTransform.sizeDelta.x - 12f), nextSlider.rectTransform.anchoredPosition.y);
        }
    }

    private void OnDisable()
    {
        upEffect.OnActive(false);
        //effectClock?.Stop();
        effectCount = 0;
        Async.StopAsync(gameObject);
    }

    public void Upgrad()
    {
        if (upImage.gameObject.activeSelf)//如果是可以升级的,打开升级特效
        {
            if (upEffect.activeInHierarchy)
            {
                effectCount += 1;
            }
            else {
                upEffect.OnActive(true);
                EffectClock();
            }
        }
    }

    private async void EffectClock()
    {
        await Async.WaitforSecondsRealTime(effectTime,gameObject);
        upEffect.OnActive(false);
        if (effectCount > 0)
        {
            effectCount -= 1;
            upEffect.OnActive(true);
            EffectClock();
        }
    }
}
