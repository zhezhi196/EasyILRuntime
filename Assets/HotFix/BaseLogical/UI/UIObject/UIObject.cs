using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using Module;
using Sirenix.Utilities;
using UnityEngine;

namespace HotFix
{
    #region UIEnum

    public enum UITweenType
    {
        Ignore,
        None,
        Fade,
        Left,
        Right,
        Up,
        Down,
        Scale
    }

    public enum OpenFlag
    {
        /// <summary>
        /// ABCD---打开B---ABCDB
        /// </summary>
        Inorder,

        /// <summary>
        /// ABCD---打开B---AB
        /// </summary>
        Insertion,
    }

    #endregion

    public  class UIObject
    {
        private enum OpenOrClose
        {
            Close = 1,
            Open
        }

        #region Static

        public static Dictionary<UIType, UIObject> UIObjects = new Dictionary<UIType, UIObject>();
        private static List<UIObject> uiQueue = new List<UIObject>();

        
        public static UIObject currentUI
        {
            get { return uiQueue.IsNullOrEmpty() ? null : uiQueue.GetLast(); }
        }

        public static UIObject Open(UIType uiType, UITweenType tweenType, params object[] args)
        {
            if (currentUI != null && currentUI.modul != null && currentUI.modul.isPopup)
            {
                currentUI.Close();
            }

            return UIObjects[uiType].OpenPanle(tweenType, OpenFlag.Insertion, false, args);
        }

        public static UIObject Popup(UIType uiType, UITweenType tweenType, params object[] args)
        {
            return UIObjects[uiType].OpenPanle(tweenType, OpenFlag.Insertion, true, args);
        }

        public static void Back()
        {
            if (currentUI != null && currentUI.isActive && !Module.UIComponent.isFreezed && uiQueue.Count > 1)
            {
                currentUI.ctrl.Back(uiQueue[uiQueue.Count - 2]);
            }
        }

        #endregion

        #region field

        private GameObject m_prefab;
        private ICtrlUi m_ctrl;
        private UIModul m_modul;
        private UIView m_view;
        
        #endregion

        #region 属性

        public ICtrlUi ctrl
        {
            get { return m_ctrl; }
        }

        public UIModul modul
        {
            get { return m_modul; }
        }

        public UIView view
        {
            get { return m_view; }
        }

        public UIType uiType { get; }
        public bool isActive
        {
            get { return m_ctrl != null; }
        }
        public bool isInit
        {
            get { return m_prefab != null; }
        }

        #endregion

        public UIObject(UIType type)
        {
            uiType = type;
        }

        public void Init(Voter voter)
        {
            if (uiType.preLoad)
            {
                ResourceManager.PreLoad(uiType.prefabPath, obj =>
                {
                    m_prefab = (GameObject) obj;

                    if (uiType.preLoad)
                    {
                        voter.Add();
                    }
                });
            }
            else
            {
                voter.Add();
            }
        }

        public UIObject OpenPanle(UITweenType tweenType, OpenFlag flag, bool isPopup, params object[] args)
        {
            if (currentUI == this)
            {
                m_ctrl.Refresh();
                return this;
            }

            Module.UIComponent.Freeze(uiType.name);

            if (!isActive)
            {
                m_ctrl = (ICtrlUi) Activator.CreateInstance(uiType.ctrlType);
            }


            m_modul = m_ctrl.RefreshModul(tweenType, isPopup, currentUI, args);

            LoadUIView(() =>
            {
                try
                {
                    m_ctrl.OnOpenStart();
                }
                catch (Exception e)
                {
                    GameDebug.LogError($"{uiType.name}: OnOpenStart error: {e}");
                }

                Voter voter = new Voter(2);
                voter.OnComplete(() => Module.UIComponent.UnFreeze(uiType.name));
                
                if (currentUI != null)
                {                    
                    currentUI.modul.OnDisable();
                    if (!isPopup)
                    {
                        UIObject closeObj = currentUI;
                        currentUI.ctrl.OnCloseStart();
                        //关闭当前界面
                        currentUI.ctrl.ExitPartAnimator(tweenType, () =>
                        {
                            currentUI.JustClose(currentUI.modul.tweenType).OnComplete(() =>
                            {
                                try
                                {
                                    currentUI.ctrl.OnCloseComplete();
                                }
                                catch (Exception e)
                                {
                                    GameDebug.LogError($"{currentUI.uiType.name}: OnCloseComplete error: {e}");
                                }
                                closeObj.view.gameObject.SetActive(false);
                                voter.Add();
                            });
                        });
                    }
                    else
                    {
                        voter.Add();
                    }

                }
                else
                {
                    voter.Add();
                }

                SortQueue(flag);

                //打开目标界面
                JustOpen(tweenType).OnComplete(() =>
                {
                    try
                    {
                        currentUI.ctrl.OnOpenComplete();
                    }
                    catch (Exception e)
                    {
                        GameDebug.LogError($"{currentUI.uiType.name}: OnOpenComplete error: {e}");
                    }
                    
                    currentUI.ctrl.EnterPartAnimator(tweenType, () =>
                    {
                        voter.Add();
                    });
                });
                
                try
                {
                    currentUI.ctrl.Refresh();
                    EventCenter.Dispatch(EventKey.OnShowUI, uiType);
                }
                catch (Exception e)
                {
                    GameDebug.LogError($"{uiType.name}: Refresh error: {e}");
                }
            }, isPopup);

            return this;
        }
        
        public UIObject Close()
        {
            if (!modul.isPopup)
            {
                GameDebug.LogError($"{uiType.name} 非弹窗,不能直接关闭");
                return this;
            }

            //UIObject target = uiQueue[uiQueue.Count - 2];

            Module.UIComponent.Freeze(uiType.name);
            ctrl.OnCloseStart();
            ctrl.ExitPartAnimator(modul.tweenType, () =>
            {
                JustClose(modul.tweenType).OnComplete(() =>
                {
                    Module.UIComponent.UnFreeze(uiType.name);
                    ctrl.OnCloseComplete();
                    view.gameObject.SetActive(false);
                });
            });
            
            uiQueue.RemoveAt(uiQueue.Count - 1);
            currentUI.ctrl.Refresh();
            return currentUI;
        }

        private void LoadUIView(Action action, bool isPopup)
        {
            if (!isInit)
            {
                ResourceManager.PreLoad(uiType.prefabPath, go =>
                {
                    m_prefab = (GameObject)go;
                    m_view = m_ctrl.CreatView(m_prefab, isPopup);
                    action();
                });
            }
            else
            {
                m_view = m_ctrl.CreatView(m_prefab, isPopup);
                action();
            }
        }
        
        private Tweener JustClose(UITweenType tweenType)
        {
            return TweenUI(tweenType, OpenOrClose.Close);
        }

        private Tweener JustOpen(UITweenType tweenType)
        {
            m_view.gameObject.SetActive(true);
            return TweenUI(tweenType, OpenOrClose.Open);
        }

        private void SortQueue(OpenFlag flag)
        {
            if(m_view==null) return;
            
                m_view.transform.SetAsLastSibling();
            switch (flag)
            {
                case OpenFlag.Insertion:
                    int readToRemoveIndex = uiQueue.Count;
                    for (int i = uiQueue.Count - 1; i >= 0; i--)
                    {
                        if (uiQueue[i] == this)
                        {
                            readToRemoveIndex = i;
                        }
                    }

                    for (int i = uiQueue.Count - 1; i >= readToRemoveIndex; i--)
                    {
                        uiQueue.RemoveAt(i);
                    }

                    uiQueue.Add(this);
                    break;
                case OpenFlag.Inorder:
                    uiQueue.Add(this);
                    break;
            }
        }

        #region 窗体动画

        /// <summary>
        /// 窗体动画
        /// </summary>
        /// <param buffId="state"></param>
        /// <returns></returns>
        private Tweener TweenUI(UITweenType uiTweenType, OpenOrClose state)
        {
            switch (uiTweenType)
            {
                case UITweenType.None:
                    return DoFade(state);
                case UITweenType.Fade:
                    return DoFade(state);
                case UITweenType.Left:
                    return DoFade(state);
                case UITweenType.Right:
                    return DoFade(state);
                case UITweenType.Up:
                    return DoFade(state);
                case UITweenType.Down:
                    return DoFade(state);
                case UITweenType.Scale:
                    return DoFade(state);
            }

            return null;
        }

        private Tweener DoFade(OpenOrClose state)
        {
            CanvasGroup canvas = m_view.gameObject.AddOrGetComponent<CanvasGroup>();
            canvas.alpha = ((int) state) % 2;
            //return null;
            return canvas.DOFade(((int) state - 1), m_view.target.tweenInterval).SetEase(m_view.target.tweenCurve)
                .SetDelay(m_view.target.delay).SetUpdate(true);
        }

        private Tweener DoLeft(OpenOrClose state)
        {
            return null;
        }

        private Tweener DoRight(OpenOrClose state)
        {
            return null;
        }

        private Tweener DoUp(OpenOrClose state)
        {
            return null;
        }

        private Tweener DoDown(OpenOrClose state)
        {
            return null;
        }

        private Tweener DoScale(OpenOrClose state)
        {
            return null;
        }

        private Tweener DoNone(OpenOrClose state)
        {
            return null;
        }

        #endregion
    }
}