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
        public string GetUIPrefabPath()
        {
            return string.Join("/", ConstKey.GetFolder(AssetLoad.AssetFolderType.UI), winName);
        }
        
        private string m_winName;

        private UIViewBase _mViewBase;
        private (bool, float) setDelay;
        private (bool, float) setInterval;
        private Tweener uiTweener;

        public bool isInit;
        public event Action<UITweenType> OnOpenComplete;
        public event Action<UITweenType> OnCloseComplete;

        public event Action<UITweenType> onSequenceComplete;
        public event Action<UIViewBase> OnLoadView;

        public string path { get; }

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

        public UIObject(string name)
        {
            m_winName = name;
            path = $"{GetUIPrefabPath()}/{name}.prefab";
        }

        #endregion

        public void SetTweenDelay(float delay)
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
            Refresh(lastUI, viewBase, viewBase.model.args);
        }

        #region 窗体开关

        #region 开

        public void Open(UITweenType tweenType, OpenFlag flag, bool isPopup, object[] args,Action callback)
        {
            RunTimeSequence procudle = new RunTimeSequence();
            UIObject lastUI = UIController.Instance.currentUI;
            List<UIObject> readyClose = null;
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
                if (lastUI != null)
                    readyClose.Add(lastUI);
            }

            if (lastUI == this)
            {
                direction = UIOpenDirection.Stay;
                _mViewBase.model.RefreshModel(false, tweenType, flag, isPopup, args, direction);
                Refresh(null, viewBase, args);
                procudle.NextAction();
                return;
            }

            UIController.Instance.SortStack(this, flag);
            //打开遮罩与关闭遮罩
            UICommpont.FreezeUI(m_winName);
            procudle.OnComplete(() =>
            {
                callback?.Invoke();
                UICommpont.UnFreezeUI(m_winName); 
            });

            Load(winName, isPopup, () =>
            {
                _mViewBase.model.RefreshModel(true, tweenType, flag, isPopup, args, direction);
                viewBase.OnOpenStart();
                EventCenter.Dispatch(ConstKey.UIOpenStart, winName);
                if (!isPopup && !readyClose.IsNullOrEmpty())
                {
                    procudle.Add(new RunTimeAction(() =>
                    {
                        Voter voter = new Voter(readyClose.Count * 2, procudle.NextAction);
                        for (int i = 0; i < readyClose.Count; i++)
                        {
                            if (readyClose[i].viewBase != null)
                            {
                                //关闭上个界面的部件
                                readyClose[i].viewBase.ExitAnimator(() => voter.Add());
                                //关闭上个界面
                                readyClose[i].uiTweener = readyClose[i].JustClose(UIModel.Invert(tweenType), complete =>
                                {
                                    //这里,并不是彻底关闭UI才开始打开下个界面的,而是开始播动画的时候,就开始下一段逻辑了
                                    if (!complete) voter.Add();
                                });
                            }
                            else
                            {
                                voter.Add(2);
                            }
                        }
                    }));
                }

                Voter openVoter = new Voter(2, procudle.NextAction);
                //打开下个界面
                uiTweener = JustOpen(tweenType, complete =>
                {
                    if (complete) openVoter.Add();
                });
                //打开下个界面部件
                UIController.Instance.currentUI.viewBase.EnterAnimator(() => openVoter.Add());
                Refresh(lastUI, UIController.Instance.currentUI.viewBase, args);
                procudle.NextAction();
            });
        }

        public Tweener JustOpen(UITweenType tweenType,Action<bool> callback)
        {
            if (uiTweener.IsActive())
            {
                DOTween.Kill(uiTweener);
            }
            _mViewBase.gameObject.OnActive(true);
            UIController.Instance.SortUI();
            Tweener tween = TweenUI(tweenType, OpenOrClose.Open);
            if (tween != null)
            {
                tween.onComplete += (() =>
                {
                    viewBase.OnOpenComplete();
                    EventCenter.Dispatch(ConstKey.UIOpenComplete, winName);
                    OnOpenComplete?.Invoke(tweenType);
                    OnOpenComplete = null;                    
					callback?.Invoke(true);
                });
                
                callback?.Invoke(false);
            }

            return tween;
        }

        #endregion

        #region 关

        public void Close(UITweenType tweenType, Action callback)
        {
            if (UIController.Instance.currentUI == null)
            {
                GameDebug.LogError("当前页面为空,无法关闭当前界面");
                return;
            }

            UICommpont.FreezeUI(m_winName);
            RunTimeSequence sq = new RunTimeSequence();
            sq.OnComplete(() =>
            {
                callback?.Invoke();
                UICommpont.UnFreezeUI(m_winName);
            });
            sq.Add(new RunTimeAction(() =>
            {
                Voter closeVoter = new Voter(2, sq.NextAction);
                viewBase.ExitAnimator(() => closeVoter.Add());
                JustClose(tweenType, complete =>
                {
                    if (complete) closeVoter.Add();
                });
                UIObject lastUi = UIController.Instance.currentUI;
                UIController.Instance.winList.RemoveBack(this);
                UIController.Instance.currentUI.RefreshSelf(lastUi);
            }));
            sq.NextAction();
        }

        public Tweener JustClose(UITweenType tweenType, Action<bool> callback)
        {
            if (uiTweener.IsActive())
            {
                DOTween.Kill(uiTweener);
            }
            EventCenter.Dispatch(ConstKey.UICloseStart, winName);
            viewBase.OnCloseStart();
            EventCenter.Dispatch(ConstKey.CloseUI, winName);

            Tweener tween = TweenUI(tweenType, OpenOrClose.Close);
            if (tween != null)
            {
                tween.onComplete += () =>
                {
                    UIController.Instance.SortUI();
                    viewBase.OnCloseComplete();
                    OnCloseComplete?.Invoke(tweenType);
                    onSequenceComplete?.Invoke(tweenType);
                    OnCloseComplete = null;
                    onSequenceComplete = null;
                    UnLoad();
                    callback?.Invoke(true);
                };
                callback?.Invoke(false);
            }

            return tween;
        }

        #endregion

        #endregion

        #region 加载卸载

        private void Load(string name, bool isPopup, Action callback)
        {
            if (_mViewBase == null)
            {
                AssetLoad.LoadGameObject<UIViewBase>(path, isPopup ? UICommpont.topParent : UICommpont.winParent,
                    (ass, arg) =>
                    {
                        _mViewBase = ass;
                        _mViewBase.Init(name);
                        _mViewBase.uiConfig = this;
                        _mViewBase.transform.localPosition = Vector3.zero;
                        _mViewBase.gameObject.OnActive(false);
                        OnLoadView?.Invoke(_mViewBase);
                        callback?.Invoke();
                        isInit = true;
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
            if (viewBase != null && viewBase.gameObject != null && !viewBase.destroyViewOnClose)
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
            AssetLoad.Destroy(_mViewBase.gameObject);
            DOTween.Kill(this);
            _mViewBase = null;
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
            if (viewBase == null)
            {
                GameDebug.LogError("View尚未加载");
                return null;
            }
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

            return canvas.DOFade(((int) state - 1), setInterval.Item1 ? setInterval.Item2 : viewBase.tweenInterval)
                .SetEase(_mViewBase.tweenCurve)
                .SetDelay(setDelay.Item1 ? setDelay.Item2 : viewBase.delay).SetUpdate(true).SetId(this);
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

            return viewBase.transform
                .DOLocalMove(target, setInterval.Item1 ? setInterval.Item2 : viewBase.tweenInterval)
                .SetEase(_mViewBase.tweenCurve)
                .SetDelay(setDelay.Item1 ? setDelay.Item2 : viewBase.delay).SetUpdate(true).SetId(this);
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

            return viewBase.transform
                .DOLocalMove(target, setInterval.Item1 ? setInterval.Item2 : viewBase.tweenInterval)
                .SetEase(_mViewBase.tweenCurve)
                .SetDelay(setDelay.Item1 ? setDelay.Item2 : viewBase.delay).SetUpdate(true).SetId(this);
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

            return viewBase.transform
                .DOLocalMove(target, setInterval.Item1 ? setInterval.Item2 : viewBase.tweenInterval)
                .SetEase(_mViewBase.tweenCurve)
                .SetDelay(setDelay.Item1 ? setDelay.Item2 : viewBase.delay).SetUpdate(true).SetId(this);
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

            return viewBase.transform
                .DOLocalMove(target, setInterval.Item1 ? setInterval.Item2 : viewBase.tweenInterval)
                .SetEase(_mViewBase.tweenCurve)
                .SetDelay(setDelay.Item1 ? setDelay.Item2 : viewBase.delay).SetUpdate(true).SetId(this);
        }

        private Tweener DoScale(OpenOrClose state)
        {
            float StartValue = ((int) state) % 2;
            viewBase.transform.localScale = new Vector3(StartValue, StartValue, StartValue);
            float scaleEnd = ((int) state - 1);
            return viewBase.transform.DOScale(scaleEnd, setInterval.Item1 ? setInterval.Item2 : viewBase.tweenInterval)
                .SetEase(_mViewBase.tweenCurve)
                .SetDelay(setDelay.Item1 ? setDelay.Item2 : viewBase.delay).SetUpdate(true).SetId(this);
        }

        private Tweener DoNone(OpenOrClose state)
        {
            return viewBase.transform.DOScale(1, 0).SetDelay(setDelay.Item1 ? setDelay.Item2 : viewBase.delay).SetUpdate(true).SetId(this);
        }

        #endregion

        public override string ToString()
        {
            return winName;
        }

        public void ReloadUI(Action callback)
        {
            UIModel model = viewBase.model;
            AssetLoad.DestroyImmediate(viewBase.gameObject);
            Load(winName, model.isPopup, () =>
            {
                viewBase.OnOpenStart();
                JustOpen(UITweenType.None, complete =>
                {
                    if (complete) callback?.Invoke();
                });
                Refresh(model.lastUI, UIController.Instance.currentUI.viewBase, model.args);
            });
        }
    }
}