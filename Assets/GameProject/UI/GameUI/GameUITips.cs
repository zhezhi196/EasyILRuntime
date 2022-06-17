using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using DG.Tweening;
using UnityEngine.UI;
using System;

/// <summary>
/// GameUI各类提示管理
/// 能量不足,子弹不足
/// 获得物品
/// 交互文字提示
/// </summary>
public class GameUITips : MonoBehaviour
{
    public enum TextTipType
    { 
        Bullet,
        Energy,
        Dodge
    }
    [System.Serializable]
    public class TextTip
    {
        public TextTipType type;
        public Text text;
        public float showTime;
        public Clock clock;
        private Tweener tweener;
        public void Init()
        {
            clock = new Clock(showTime);
            clock.ignoreTimescale = true;
            clock.onRestart += OnRestart;
            clock.onComplete += OnComplete;
        }

        private void OnRestart()
        {
            if (!text.gameObject.activeSelf)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
                text.gameObject.OnActive(true);
            }
            if (tweener != null)
            {
                tweener.Kill();
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
            }
        }

        private void OnComplete()
        {
            tweener = text.DOFade(0, 0.5f).SetUpdate(true).OnComplete(() =>
            {
                text.gameObject.OnActive(false);
                tweener = null;
            });
        }
    }
    [Header("能量/子弹不足提示")]
    public TextTip[] textTips;
    [Header("拾取物品及飘字提示")]
    public Image propIcon;
    public Text propText;
    public GameObject propTip;
    public float porpTipTime = 1f;
    public List<PropInfo> propInfos = new List<PropInfo>();
    public class PropInfo
    {
        public Sprite sp;
        public float count;
        public string str;

        public PropInfo(Sprite sp,string s)
        {
            this.sp = sp;
            str = s;
        }
    }
    //[Header("飘字提示")]
    //public Text piaozi;
    //public GameObject piaoziObj;
    //public float pzTime = 1;

    private void Start()
    {
        for (int i = 0; i < textTips.Length; i++)
        {
            textTips[i].Init();
        }
        EventCenter.Register<TextTipType>(EventKey.ShowTextTip,ShowTextTip);
        EventCenter.Register<Sprite, float>(EventKey.DropProps, GetProp);
        EventCenter.Register<string>(EventKey.piao, PiaoZi);
    }

    private void OnEnable()
    {
        if (propInfos.Count>0 && !propTip.activeInHierarchy)
        {
            ShowPorpTip();
        }
    }

    private void PiaoZi(string obj)
    {
        //AudioPlay.PlayOneShot("shengji_cg");
        //piaozi.text = Language.GetContent(obj);
        //piaoziObj.OnActive(true);
        //await Async.WaitforSecondsRealTime(pzTime,gameObject);
        //piaoziObj.OnActive(false);
        propInfos.Add(new PropInfo(null, Language.GetContent(obj)));
        if (propInfos.Count == 1 && gameObject.activeInHierarchy)
        {
            ShowPorpTip();
        }
    }

    private void GetProp(Sprite arg1, float arg2)
    {
        propInfos.Add(new PropInfo(arg1, string.Format("+{0}", arg2.ToString())));
        if (propInfos.Count == 1 && gameObject.activeInHierarchy)
        {
            ShowPorpTip();
        }
    }

    private async void ShowPorpTip()
    {
        if (propInfos.Count <= 0)
        {
            return;
        }
        if (propIcon != null)
        {
            propIcon.sprite = propInfos[0].sp;
            propIcon.transform.parent.OnActive(propInfos[0].sp!=null);
        }
        propText.text = propInfos[0].str;
        propTip.OnActive(true);
        await Async.WaitforSecondsRealTime(porpTipTime,gameObject);
        if (propIcon != null)
        {
            propIcon.transform.parent.OnActive(false);
        }
        propTip.OnActive(false);
        propInfos.RemoveAt(0);
        if (propInfos.Count > 0 && gameObject.activeInHierarchy)
        {
            ShowPorpTip();
        }
    }

    private void OnDestroy()
    {
        EventCenter.UnRegister<TextTipType>(EventKey.ShowTextTip, ShowTextTip);
        EventCenter.UnRegister<Sprite, float>(EventKey.DropProps, GetProp);
        EventCenter.UnRegister<string>(EventKey.piao, PiaoZi);
        Async.StopAsync(gameObject);
    }

    public void ShowTextTip(TextTipType type)
    {
        TextTip t = textTips.Find(tt => tt.type == type);
        if (t != null)
        {
            t.clock.Restart();
        }
    }
}
