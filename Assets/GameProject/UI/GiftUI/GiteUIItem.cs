using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using System;
using UnityEngine.UI;
using GameGift;
public class GiteUIItem : MonoBehaviour
{
    public Image icon;
    public Image selectIcon;
    [HideInInspector]
    public Toggle toggle;
    public Gift gift;
    //public GameObject ownEffect;
    public GameObject selectEffect;
    public GameObject lineObj;
    public GameObject lockIcon;
    public GameObject studyEffect;
    public float studyEffectTime = 1.3f;
    public GameObject unlockEffect;
    public float unlockEffectTime = 1f;
    public void Init(Gift g,Action callBack)
    {
        this.gift = g;
        toggle = this.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener((b) => OnToggleChange(b));
        SpriteLoader.LoadIcon(gift.dbData.icon, (s) =>
        {
            icon.sprite = s;
            callBack?.Invoke();
        });
        Refesh();
    }

    public void Refesh()
    {
        //ownEffect.OnActive(gift.station == GiftStation.Owned);
        lockIcon.OnActive(gift.station == GiftStation.Locked);
        icon.SetAlpha(gift.station == GiftStation.Owned ? 1f : 0.35f);
        if (gift.station == GiftStation.Owned)
        {
            icon.color = new Color(1, 0.796f, 0.318f, 1f);
            selectIcon.color = new Color(1, 0.796f, 0.318f, 1f);
        }
        else
        {
            icon.color = new Color(1, 1, 1, 1f);
            selectIcon.color = new Color(1, 1, 1, 1f);
        }
        
        if (lineObj != null)
            lineObj.OnActive(gift.station == GiftStation.Owned);
    }

    private void OnToggleChange(bool b)
    {
        transform.localScale = b ? Vector3.one * 1.2f : Vector3.one;
        selectEffect.OnActive(b);
    }

    private void OnDisable()
    {
        if (waitUnlock)
        {
            //gift.Unlock();
            waitUnlock = false;
            unlockEffect.OnActive(false);
            lockIcon.OnActive(false);
        }
        studyEffect.OnActive(false);
        Async.StopAsync(gameObject);
    }

    public void OnStudy()
    {
        BattleController.GetCtrl<GiftCtrl>().StudyGift(gift);
        EventCenter.Dispatch<Gift>(EventKey.OnStudyGift, gift);
        if (lineObj != null)
            lineObj.OnActive(true);
        //ownEffect.OnActive(gift.station == GiftStation.Owned);
        lockIcon.OnActive(false);
        icon.SetAlpha(1);
        StudyEffect();
    }

    private async void StudyEffect()
    {
        studyEffect.OnActive(true);
        await Async.WaitforSecondsRealTime(studyEffectTime,gameObject);
        studyEffect.OnActive(false);
        if (gift.station == GiftStation.Owned)
        {
            icon.color = new Color(1, 0.796f, 0.318f, 1f);
        }
        else
        {
            icon.color = new Color(1, 1, 1, 1f);
        }
    }

    public void Unlock()
    {
        //解锁特效
        gift.Unlock();
        UnlockEffect();
    }

    private bool waitUnlock = false;
    private async void UnlockEffect()
    {
        waitUnlock = true;
        await Async.WaitforSecondsRealTime(1f, gameObject);
        unlockEffect.OnActive(true);
        await Async.WaitforSecondsRealTime(unlockEffectTime, gameObject);
        unlockEffect.OnActive(false);
        waitUnlock = false;
        lockIcon.OnActive(false);
    }
}
