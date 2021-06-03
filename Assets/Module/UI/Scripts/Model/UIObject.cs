/*
 * 脚本名称：UIModel
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2017-12-21 22:36:04
 * 脚本作用：
*/

using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class UIObject
    {
        private enum OpenOrClose
        {
            Close = 1,
            Open
        }

        #region 字段属性

        private string m_winName;

        private UIViewBase _mViewBase;
        private (bool,float) setDelay;
        private (bool,float) setInterval;
        private ObjectPool _viewPool;

        private ObjectPool viewPool
        {
            get
            {
                if (_viewPool == null || !_viewPool.isComplete)
                {
                    _viewPool = ObjectPool.GetPool(path, null);
                }

                return _viewPool;
            }
        }

        public string path;
        public event Action<UITweenType> OnOpenComplete;
        public event Action<UITweenType> OnCloseComplete;
        public event Action<UIViewBase> OnLoadView; 

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool isVisiable
        {
            get
            {
                if (isActive)
                {
                    return viewBase.gameObject.activeInHierarchy;
                }

                return false;
            }

            set
            {
                if (isActive)
                {
                    viewBase.gameObject.OnActive(value);
                }
            }
        }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool isActive
        {
            get { return _mViewBase != null; }
        }

        public bool isInit
        {
            get { return viewPool.isComplete; }
        }

        /// <summary>
        /// 窗口名
        /// </summary>
        public string winName
        {
            get { return m_winName; }
        }

        /// <summary>
        /// 游戏物体
        /// </summary>
        public GameObject gameObject
        {
            get { return _mViewBase.gameObject; }
        }

        /// <summary>
        /// mono实例物体
        /// </summary>
        public UIViewBase viewBase
        {
            get { return _mViewBase; }
        }

        #endregion

        #region 初始化

        private string GetUIPrefabPath(string name)
        {
            return string.Join("/", "UI", name) + ".prefab";
        }

        public UIObject(string name)
        {
            m_winName = name;
            path = GetUIPrefabPath(name);
        }

        #endregion

        public void SetDelay(float delay)
        {
            setDelay = (true, delay);
        }

        public void SetTweenInterval(float interval)
        {
            setInterval = (true, interval);
        }

        private void Refresh(UIObject lastUI, UIViewBase view, object[] args)
        {
            if (lastUI != null)
            {
                view.model.lastUI = lastUI;
            }
            
            setDelay = (false, 0);
            setInterval = (false, 0);
            view.Refresh(args);
            EventCenter.Dispatch(ConstKey.OpenUI, winName);
        }

        private void RefreshSelf(UIObject lastUI)
        {
            Refresh(lastUI,viewBase,viewBase.model.args);
        }

        #region 窗体开关

        #region 开

        public RunTimeSequence Open(UITweenType tweenType, OpenFlag flag, bool isPopup, object[] args)
        {
            RunTimeSequence procudle = new RunTimeSequence();
            UIObject lastUI = UIController.Instance.currentUI;
            List<UIObject> readyClose= null;
            if (lastUI != null) readyClose = new List<UIObject>();
            UIOpenDirection direction = UIOpenDirection.Forward;
            if (flag == OpenFlag.Insertion)
            {
                for (int i = UIController.Instance.winList.Count - 1; i >= 0; i--)
                {
                    if (UIController.Instance.winList[i] != this)
                    {
                        readyClose.Add(UIController.Instance.winList[i]);
                    }
                    else
                    {
                        direction = UIOpenDirection.Backward;
                        break;
                    }
                }
            }
            else
            {
                if(lastUI!=null)
                readyClose.Add(lastUI);
            }
            
            if (lastUI == this)
            {
                direction = UIOpenDirection.Stay;
                _mViewBase.model.RefreshModel(false, tweenType, flag, isPopup, args,direction);
                Refresh(null, viewBase, args);
                procudle.NextAction();
                return procudle;
            }

            UIController.Instance.SortStack(this, flag);
            //打开遮罩与关闭遮罩
            UICommpont.FreezeUI(m_winName);
            procudle.OnComplete(() =>
            {
                UICommpont.UnFreezeUI(m_winName);
            });

            Load(winName, isPopup, () =>
            {
                _mViewBase.model.RefreshModel(true, tweenType, flag, isPopup, args, direction);

                try
                {
                    viewBase.OnOpenStart();
                }
                catch (Exception e)
                {
                    GameDebug.LogError(winName + "的OnOpenStart函数有错误:" + e);
                }

                if (!isPopup && !readyClose.IsNullOrEmpty())
                {
                    //关闭上个界面的部件
                    procudle.Add(new RunTimeAction(() =>
                    {
                        Voter voter = new Voter(readyClose.Count, () => procudle.NextAction());
                        for (int i = 0; i < readyClose.Count; i++)
                        {
                            if (readyClose[i].viewBase != null)
                            {
                                readyClose[i].viewBase.ExitAnimator(() => voter.Add());
                            }
                            else
                            {
                                voter.Add();
                            }
                        }
                    }));

                    //关闭上个界面
                    procudle.Add(new RunTimeAction(() =>
                    {
                        //在这里，把部件的动画和界面主动画放一起了

                        for (int i = 0; i < readyClose.Count; i++)
                        {
                            //if (ob[i].viewBase != null)
                            //{
                            readyClose[i].JustClose(UIModel.Invert(tweenType));

                            //}
                            //else
                            //{

                            //}
                        }

                        procudle.NextAction();
                    }));
                }


                procudle.Add(new RunTimeAction(() =>
                {
                    UIController.Instance.currentUI.viewBase.EnterAnimator(() => procudle.NextAction());
                }));

            //打开下个界面
            procudle.Add(new RunTimeAction(() =>
            {
                JustOpen(tweenType).onComplete += () => procudle.NextAction();
                Refresh(lastUI, UIController.Instance.currentUI.viewBase, args);
            }));

            procudle.NextAction();
            });

            return procudle;
        }

        public Tweener JustOpen(UITweenType tweenType)
        {
            _mViewBase.gameObject.OnActive(true);
            try
            {
                viewBase.OnShowStart();
            }
            catch (Exception e)
            {
                GameDebug.LogError(winName + "的OnShowStart函数有错误:" + e);
            }
            UIController.Instance.SortUI();
            Tweener tween = TweenUI(tweenType, OpenOrClose.Open);
            tween.onComplete += (() =>
            {
                viewBase.OnOpenComplete();
                EventCenter.Dispatch(ConstKey.UIOpenComplete, winName);
                OnOpenComplete?.Invoke(tweenType);
                OnOpenComplete = null;
                tween = null;
            });

            return tween;
        }

        #endregion
        
        #region 关

        public RunTimeSequence Close(UITweenType tweenType)
        {
            if (UIController.Instance.currentUI == null)
            {
                GameDebug.LogError("当前页面为空,无法关闭当前界面");
                return null;
            }

            UICommpont.FreezeUI(m_winName);
            RunTimeSequence sq = new RunTimeSequence();
            sq.Add(new RunTimeAction(() => { viewBase.ExitAnimator(()=>sq.NextAction()); }));
            sq.Add(new RunTimeAction(() =>
            {
                JustClose(tweenType).onComplete += () =>
                {
                    sq.NextAction();
                };
                UIObject lastUi = UIController.Instance.currentUI;
                UIController.Instance.winList.RemoveBack(this);
                UIController.Instance.currentUI.RefreshSelf(lastUi);
            }));

            sq.NextAction();
            return sq;
        }

        public Tweener JustClose(UITweenType tweenType)
        {
            try
            {
                EventCenter.Dispatch(ConstKey.UICloseStart, winName);
                viewBase?.OnCloseStart();
                EventCenter.Dispatch(ConstKey.CloseUI, winName);
            }
            catch (Exception e)
            {
                GameDebug.LogError(winName + "的OnCloseStart函数有错误:" + e);
            }

            Tweener tween = TweenUI(tweenType, OpenOrClose.Close);
            if (tween == null)
            {
                UIController.Instance.SortUI();
            }
            else
            {
                tween.OnComplete(() =>
                {
                    UIController.Instance.SortUI();
                    viewBase?.OnCloseComplete();
                    OnCloseComplete?.Invoke(tweenType);
                    OnCloseComplete = null;
                    UnLoad();
                    UICommpont.UnFreezeUI(m_winName);
                });
            }

            return tween;
        }

        #endregion

        #endregion

        #region 加载卸载

        private void Load(string name, bool isPopup,Action callback)
        {
            if (_mViewBase == null)
            {
                viewPool.GetObject<UIViewBase>(isPopup ? UICommpont.topParent : UICommpont.winParent, ass =>
                {
                    _mViewBase = ass;
                    _mViewBase.Init(name);
                    _mViewBase.uiConfig = this;
                    _mViewBase.transform.localPosition = Vector3.zero;
                    OnLoadView?.Invoke(_mViewBase);
                    callback?.Invoke();
                });
            }
            else
            {
                Transform parent = isPopup ? UICommpont.topParent : UICommpont.winParent;
                _mViewBase.transform.SetParent(parent);
                callback?.Invoke();
            }
        }

        private void UnLoad()
        {
            if (!viewBase.destroyViewOnClose)
            {
                CanvasGroup canvas = viewBase.gameObject.GetComponent<CanvasGroup>();

                if (canvas != null)
                {
                    canvas.alpha = 1;
                }
                viewBase.transform.localScale = Vector3.one;
                viewBase.transform.localPosition = Vector3.zero;
                _mViewBase?.gameObject.OnActive(false);
            }
            else
            {
                Destroy();
            }
        }

        public void Destroy()
        {
            if (_mViewBase == null) return;
            _mViewBase = null;
            viewPool.DisposeFromMemory();
            _viewPool = null;
        }

        #endregion

        #region 窗体动画

        /// <summary>
        /// 窗体动画
        /// </summary>
        /// <param buffId="state"></param>
        /// <returns></returns>
        private Tweener TweenUI(UITweenType uiTweenType, OpenOrClose state)
        {
            if (viewBase == null) return null;
            switch (uiTweenType)
            {
                case UITweenType.None:
                    return DoNone(state);
                case UITweenType.Fade:
                    return DoFade(state);
                case UITweenType.Left:
                    return DoLeft(state);
                case UITweenType.Right:
                    return DoRight(state);
                case UITweenType.Up:
                    return DoUp(state);
                case UITweenType.Down:
                    return DoDown(state);
                case UITweenType.Scale:
                    return DoScale(state);
            }

            return null;
        }

        private Tweener DoFade(OpenOrClose state)
        {
            CanvasGroup canvas = viewBase.gameObject.AddOrGetComponent<CanvasGroup>();
            canvas.alpha = ((int) state) % 2;
            //Debug.Log(viewBase.gameObject.name);

            return canvas.DOFade(((int) state - 1), setInterval.Item1? setInterval.Item2: viewBase.tweenInterval).SetEase(_mViewBase.tweenCurve)
                .SetDelay(setDelay.Item1? setDelay.Item2: viewBase.delay).SetUpdate(true);
        }

        private Tweener DoLeft(OpenOrClose state)
        {
            Vector2 target = Vector2.zero;
            switch (state)
            {
                case OpenOrClose.Open:
                    viewBase.transform.localPosition = UICommpont.Left.localPosition;
                    target = Vector2.zero;
                    break;
                case OpenOrClose.Close:
                    viewBase.transform.localPosition = Vector3.zero;
                    target = UICommpont.Left.localPosition;
                    break;
            }

            return viewBase.transform.DOLocalMove(target, setInterval.Item1? setInterval.Item2: viewBase.tweenInterval).SetEase(_mViewBase.tweenCurve)
                .SetDelay(setDelay.Item1? setDelay.Item2: viewBase.delay).SetUpdate(true);
            //UIPlay tween = new UIPlay(t, TweenerType.Left);
            //return tween;
        }


        private Tweener DoRight(OpenOrClose state)
        {
            Vector2 target = Vector2.zero;
            switch (state)
            {
                case OpenOrClose.Open:
                    viewBase.transform.localPosition = UICommpont.Right.localPosition;
                    target = Vector2.zero;
                    break;
                case OpenOrClose.Close:
                    viewBase.transform.localPosition = Vector3.zero;
                    target = UICommpont.Right.localPosition;
                    break;
            }

            return viewBase.transform.DOLocalMove(target, setInterval.Item1? setInterval.Item2: viewBase.tweenInterval).SetEase(_mViewBase.tweenCurve)
                .SetDelay(setDelay.Item1? setDelay.Item2: viewBase.delay).SetUpdate(true);
        }

        private Tweener DoUp(OpenOrClose state)
        {
            Vector2 target = Vector2.zero;
            switch (state)
            {
                case OpenOrClose.Open:
                    viewBase.transform.localPosition = UICommpont.Up.localPosition;
                    target = Vector2.zero;
                    break;
                case OpenOrClose.Close:
                    viewBase.transform.localPosition = Vector3.zero;
                    target = UICommpont.Up.localPosition;
                    break;
            }

            return viewBase.transform.DOLocalMove(target, setInterval.Item1? setInterval.Item2: viewBase.tweenInterval).SetEase(_mViewBase.tweenCurve)
                .SetDelay(setDelay.Item1? setDelay.Item2: viewBase.delay).SetUpdate(true);
        }

        private Tweener DoDown(OpenOrClose state)
        {
            Vector2 target = Vector2.zero;
            switch (state)
            {
                case OpenOrClose.Open:
                    viewBase.transform.localPosition = UICommpont.Down.localPosition;
                    target = Vector2.zero;
                    break;
                case OpenOrClose.Close:
                    viewBase.transform.localPosition = Vector3.zero;
                    target = UICommpont.Down.localPosition;
                    break;
            }

            return viewBase.transform.DOLocalMove(target, setInterval.Item1? setInterval.Item2: viewBase.tweenInterval).SetEase(_mViewBase.tweenCurve)
                .SetDelay(setDelay.Item1? setDelay.Item2: viewBase.delay).SetUpdate(true);
        }

        private Tweener DoScale(OpenOrClose state)
        {
            float StartValue = ((int) state) % 2;
            viewBase.transform.localScale = new Vector3(StartValue, StartValue, StartValue);
            float scaleEnd = ((int) state - 1);
            return viewBase.transform.DOScale(scaleEnd, setInterval.Item1? setInterval.Item2: viewBase.tweenInterval).SetEase(_mViewBase.tweenCurve)
                .SetDelay(setDelay.Item1? setDelay.Item2: viewBase.delay).SetUpdate(true);
        }

        private Tweener DoNone(OpenOrClose state)
        {
            CanvasGroup canvas = viewBase.gameObject.AddOrGetComponent<CanvasGroup>();
            canvas.alpha = ((int) state) % 2;
            return canvas.DOFade(((int) state - 1), TimeHelper.deltaTime).SetDelay(setDelay.Item1? setDelay.Item2: viewBase.delay).SetUpdate(true);
        }

        #endregion
        
        public override string ToString()
        {
            return winName;
        }
    }
}