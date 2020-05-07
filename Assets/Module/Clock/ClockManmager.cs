///*
// * 脚本名称：ClockManmager
// * 项目名称：FrameWork
// * 脚本作者：黄哲智
// * 创建时间：2018-05-21 12:02:30
// * 脚本作用：
//*/
//
//
//using DG.Tweening;
//using DG.Tweening.Core;
//using DG.Tweening.Plugins.Options;
//using System;
//using System.Collections.Generic;
//using Module;
//using UnityEngine;
//
//
//public class ClockManmager : Singleton<ClockManmager>
//{
//    #region 字段
//
//    private TweenerCore<float, float, FloatOptions> m_timeScaleAnimator;
//
//    #endregion
//
//    #region 属性
//
//    public List<Clock> clockList { get; private set; } = new List<Clock>();
//    public float fixUnscaleDelatime { get; protected set; }
//    public float timeScaleOrder { get; set; }
//    public int currentOrder = int.MaxValue;
//
//    #endregion
//
//    public AsyncLoadProcess Init(AsyncLoadProcess process)
//    {
//        process.IsDone = false;
//        fixUnscaleDelatime = Time.fixedUnscaledDeltaTime;
//        GamePlay.Instance.OnGameUpdate += Update;
//        process.SetComplete();
//        return process;
//    }
//
//
//    private void Update()
//    {
//        for (int i = 0; i < clockList.Count; i++)
//        {
//            clockList[i].Update();
//        }
//    }
//
//    public TweenerCore<float, float, FloatOptions> SetTimeScale(float timeScale, float time, Action callback = null)
//    {
//        return SetTimeScale(timeScale, time, 0, callback);
//    }
//
//
//    public TweenerCore<float, float, FloatOptions> SetTimeScale(float timeScale, float time, int order,
//        Action callBack = null)
//    {
//        if (order < currentOrder)
//        {
//            currentOrder = order;
//            if (m_timeScaleAnimator != null) m_timeScaleAnimator.Kill();
//            m_timeScaleAnimator = DOTween.To((() => Time.timeScale), value =>
//                {
//                    Time.timeScale = value;
//                    Time.fixedDeltaTime = Time.timeScale * 0.02f;
//                }, timeScale, time).SetUpdate(true)
//                .OnComplete(() =>
//                {
//                    currentOrder = int.MaxValue;
//                    callBack?.Invoke();
//                });
//        }
//
//        return m_timeScaleAnimator;
//    }
//
//
//    public Clock GetClock(string key)
//    {
//        if (string.IsNullOrEmpty(key))
//        {
//            return null;
//        }
//
//        for (int i = 0; i < clockList.Count; i++)
//        {
//            if (clockList[i].key == key)
//            {
//                return clockList[i];
//            }
//        }
//
//        return null;
//    }
//
//    public void RemoveClock(string key)
//    {
//        if (string.IsNullOrEmpty(key))
//        {
//            return;
//        }
//
//        for (int i = 0; i < clockList.Count; i++)
//        {
//            if (clockList[i].key == key)
//            {
//                clockList.Remove(clockList[i]);
//            }
//        }
//    }
//
//    public void ClearClock()
//    {
//        clockList.Clear();
//    }
//}