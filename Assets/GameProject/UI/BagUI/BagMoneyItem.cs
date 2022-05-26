using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using UnityEngine;
using UnityEngine.UI;

public class BagMoneyItem : UIBtnBase
{
    public MoneyType type;
    public Text numberText;
    public Transform lackTrans;
    private Tweener lackTweener;
    public AnimationCurve lackCurve;
    public float lackAnimationTime = 1;
    public bool isOpenLack;

    public Transform plus;
    
    protected override void Awake()
    {
        base.Awake();
        if (Application.isPlaying)
        {
            MoneyInfo.onMoneyChanged += OnMoneyChanged;

            if (Application.isPlaying && !Channel.HasChannel(config.channel))
            {
                plus.gameObject.OnActive(false);
            }
        }
    }
    
    protected override void DefaultListener()
    {
        Commercialize.OpenStore();
        // UIController.Instance.Open("StoreUI", UITweenType.None, type);
    }
    
    private void OnMoneyChanged(MoneyType arg1, int arg2)
    {
        if (type == arg1)
        {
            numberText.text = arg2.ToString();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        numberText.text = MoneyInfo.GetMoneyEntity(type).count.ToString();
    }
    
        
    public void OpenLackMoneyTips()
    {
        if (!Channel.HasChannel(config.channel)) return;
        if (lackTrans.gameObject.activeInHierarchy) return;
        if (lackTweener != null)
        {
            lackTweener.Rewind();
            lackTweener.Kill();
        }

        lackTrans.gameObject.OnActive(true);
        lackTweener = lackTrans.GetChild(0).DOBlendableLocalMoveBy(new Vector3(0, 30, 0), lackAnimationTime)
            .SetUpdate(true)
            .SetEase(lackCurve)
            .SetLoops(-1, LoopType.Yoyo);
        isOpenLack = true;
    }
    
    public void CloseLackMoneyTips()
    {
        if(!lackTrans.gameObject.activeInHierarchy) return;
        if (lackTweener != null)
        {
            lackTweener.Rewind();
            lackTweener.Kill();
        }

        lackTrans.gameObject.OnActive(false);
        isOpenLack = false;
    }
}
