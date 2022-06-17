using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Module;

public class BlackMaskChange : MonoBehaviour
{
    public static BlackMaskChange _instance;
    public static BlackMaskChange Instance
    {
        get {
            return _instance;
        }
    }
    public Image maskImage;
    public Text text;
    public string endText ="0";
    string[] deathLabel = new string[] {"2304", "2305", "2306", "2307", "2308", "2330" };
    public AnimationCurve resurrectionCurve;//复活动画曲线
    private Action onComplete;//完成回调
    public void OnComplete(Action action)
    {
        onComplete = action;
    }

    private void Awake()
    {
        _instance = this;
        gameObject.OnActive(false);
    }

    private void OnDisable()
    {
    }

    //渐现
    public void Open(float time = 0.25f, bool showBanner = false)
    {
        text.text = "";
        text.gameObject.OnActive(true);
        maskImage.color = new Color(0, 0, 0, 0);
        this.gameObject.OnActive(true);
        maskImage.DOColor(new Color(0, 0, 0, 1), time).SetUpdate(true).OnComplete(() =>
            {
                onComplete?.Invoke();
                onComplete = null;
            });
    }
    //渐隐
    public void Close(float time = 0.25f)
    {
        tweener1?.Kill();
        tweener2?.Kill();
        text.text = "";
        if (time > 0)
        {
            Color endColor = new Color(maskImage.color.r, maskImage.color.g, maskImage.color.b, 0);
            //maskImage.color = new Color(0, 0, 0, 1);
            maskImage.DOColor(endColor, time).SetUpdate(true).OnComplete(() =>
            {
                onComplete?.Invoke();
                onComplete = null;
                this.gameObject.OnActive(false);
            });
        }
        else {
            maskImage.color = new Color(0, 0, 0, 0);
            this.gameObject.OnActive(false);
        }
    }
    //复活
    //public void Resurrection(float time = 3f)
    //{
    //    text.text = "";
    //    maskImage.color = new Color(0, 0, 0, 1);
    //    this.gameObject.OnActive(true);
    //    maskImage.DOColor(new Color(0, 0, 0, 0), time).SetUpdate(true).OnComplete
    //    (() =>
    //    {
    //        onComplete?.Invoke();
    //        onComplete = null;
    //        this.gameObject.OnActive(false);
    //    }).SetEase(resurrectionCurve);
    //}
    //纯黑
    public void Black()
    {
        text.text = "";
        this.gameObject.OnActive(true);
        maskImage.color = new Color(0, 0, 0, 1);
    }

    public void White()
    {
        text.text = "";
        this.gameObject.OnActive(true);
        maskImage.color = new Color(1, 1, 1, 1);
    }

    public void ShowText()
    {
        if (deathLabel.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, deathLabel.Length);//随机文本
            text.text = "";
            text.gameObject.OnActive(true);
            tweener1 = text.DOColor(new Color(1, 1, 1, 1), 0.5f).SetUpdate(true);
            tweener2 = text.DOText(Language.GetContent(deathLabel[index]), 1.5f).SetUpdate(true);//随机显示文本的显示
            GamePlay.Instance.StartCoroutine(HideText());
        }
    }

    public void ShowText(string t)
    {
        text.text = "";
        tweener1 = text.DOColor(new Color(1, 1, 1, 1), 0.5f);
        tweener2 = text.DOText(t, 1.5f);
        GamePlay.Instance.StartCoroutine(HideText());
    }

    private Tweener tweener1;
    private Tweener tweener2;
    public void ShowEndText()
    {
        text.text = Language.GetContent(endText);
        text.DOColor(new Color(1, 1, 1, 1), 0.5f);
        GamePlay.Instance.StartCoroutine(HideText());
    }

    IEnumerator HideText()
    {
        yield return new WaitForSeconds(3.5f);
        text.DOColor(new Color(1, 1, 1, 0), 0.5f).OnComplete(() => 
        {
            text.text = ""; 
            text.gameObject.OnActive(false);
        });
    }
}

