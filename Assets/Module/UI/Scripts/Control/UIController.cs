/*
 * 脚本名称：UIController
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2017-12-21 22:30:15
 * 脚本作用：
*/

using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module
{
    public sealed class UIController : Singleton<UIController>
    {
        #region OpenCloseInfo

        private struct OpenInfo
        {
            public UIObject ui;
            public UITweenType tween;
            public OpenFlag flag;
            public bool isPopup;
            public object[] args;

            public OpenInfo(UIObject ui, UITweenType tween, OpenFlag flag, bool isPopup, object[] args)
            {
                this.ui = ui;
                this.tween = tween;
                this.flag = flag;
                this.isPopup = isPopup;
                this.args = args;
            }
        }
        
        private struct CloseInfo
        {
            public UIObject ui;
            public UITweenType tween;
            public CloseInfo(UIObject ui, UITweenType tween)
            {
                this.ui = ui;
                this.tween = tween;
            }
        }

        #endregion
        
        #region 字段，属性
        private bool isBusy;
        private List<UIObject> m_winPool = new List<UIObject>();
        private Queue<OpenInfo> readyOpenWindow = new Queue<OpenInfo>();
        private Queue<CloseInfo> readyCloseWin = new Queue<CloseInfo>();
        
        private Dictionary<string, UIObject> m_windows = new Dictionary<string, UIObject>();
        
        //-----------------------------------------------------------------------------------------------------
        public bool isLoading
        {
            get { return UICommpont.UiLoading.gameObject.activeInHierarchy; }
        }

        /// <summary>
        /// 窗口列表
        /// </summary>
        public List<UIObject> winList
        {
            get { return m_winPool; }
        }

        /// <summary>
        /// 目前打开的窗体，包括弹窗
        /// </summary>
        public UIObject currentUI
        {
            get
            {
                if (m_winPool.Count > 0)
                {
                    return m_winPool[m_winPool.Count - 1];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 当前已经打开窗口的数量
        /// </summary>
        public int WinCount
        {
            get { return m_winPool.Count; }
        }

        #endregion
        
        #region 对UI进行重排

        /// <summary>
        /// 对UI进行重排
        /// </summary>
        public void SortUI()
        {
            currentUI.gameObject.transform.SetAsLastSibling();
            UICommpont.Mask.transform.SetAsLastSibling();
        }

        #endregion

        #region 加载，卸载UI



        #endregion

        #region 获取窗体对象,打开，关闭

        /// <summary>
        /// 获取窗体对象
        /// </summary>
        /// <param id="win"></param>
        /// <returns></returns>
        public UIObject Get(string win)
        {
            UIObject uiObject = null;
            if (!m_windows.TryGetValue(win, out uiObject))
            {
                uiObject = new UIObject(win);
                m_windows.Add(win, uiObject);
                return uiObject;
            }
            else
            {
                return uiObject;
            }
        }



        public UIObject Open(string win, UITweenType tweenType, params object[] args)
        {
            return Open(win, tweenType, OpenFlag.Inorder, args);
        }
        
        public UIObject Open(string win, UITweenType tweenType,OpenFlag flag, params object[] args)
        {
            UIObject uiObject = Get(win);
            readyOpenWindow.Enqueue(new OpenInfo(uiObject, tweenType, flag, false, args));
            return uiObject;
        }
        
        public UIObject Popup(string win, UITweenType tweenType,OpenFlag flag, params object[] args)
        {
            UIObject uiObject = Get(win);
            readyOpenWindow.Enqueue(new OpenInfo(uiObject, tweenType, flag, true, args));
            return uiObject;
        }

        public UIObject Popup(string win, UITweenType tweenType, params object[] args)
        {
            return Popup(win, tweenType, OpenFlag.Inorder, args);
        }

        public UIObject Close(string win, UITweenType tweenType)
        {
            for (int i = 0; i < winList.Count; i++)
            {
                if (winList[i].winName == win)
                {
                    UIObject target = winList[i];
                    if (target == null)
                    {
                        GameDebug.LogError("当前页面为空,无法关闭当前界面");
                        return target;
                    }
                    
                    readyCloseWin.Enqueue(new CloseInfo(target,tweenType));
                    return target;
                }
            }

            return null;
        }
        #endregion

        #region 其他
        public void ClearStack(bool destroyThem,bool ContainCurrent = false)
        {
            if (ContainCurrent)
            {
                if (destroyThem)
                {
                    for (int i = 0; i < m_winPool.Count; i++)
                    {
                        m_winPool[i].Destroy();
                    }
                }
                m_winPool.Clear();
            }
            else
            {
                for (int i = m_winPool.Count - 2; i >= 0; i--)
                {
                    if (destroyThem) m_winPool[i].Destroy();
                    m_winPool.RemoveAt(i);
                }
            }

            if (destroyThem)
            {
                foreach (KeyValuePair<string, UIObject> keyValuePair in m_windows)
                {
                    if (currentUI == keyValuePair.Value)
                    {
                        if (ContainCurrent)
                        {
                            keyValuePair.Value.Destroy();
                        }
                    }
                    else
                    {
                        keyValuePair.Value.Destroy();
                    }
                }
            }

        }
        /// <summary>
        /// 3D坐标转UI坐标,坐标应为localpostion
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector2 Convert3DToUI(Camera camera, Vector3 pos)
        {
            Vector2 pos1;
            if (camera == null) return Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UICommpont.UICanvas.transform as RectTransform,
                camera.WorldToScreenPoint(pos), UICommpont.UICanvas.worldCamera, out pos1);

            return pos1;
        }
        /// <summary>
        /// UI坐标转换
        /// 计算一个ui元素在新的节点下的坐标
        /// </summary>
        /// <param name="newParent">新的根节点</param>
        /// <param name="pos">ui世界坐标</param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public Vector2 ConverUI2NewParent(RectTransform newParent,RectTransform ui,Camera camera)
        {
            RectTransform uiNodeRectTrans = ui.GetComponent<RectTransform>();
            Vector2 uiPosOffset = new Vector2(uiNodeRectTrans.sizeDelta.x * (0.5f - uiNodeRectTrans.pivot.x), uiNodeRectTrans.sizeDelta.y * (0.5f - uiNodeRectTrans.pivot.y));
            Vector2 newpos = Vector3.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(newParent, ui.transform.position, camera, out newpos);
            return newpos+ uiPosOffset;
        }

        public void SortStack(UIObject uiObject, OpenFlag flag)
        {
            switch (flag)
            {
                case OpenFlag.Insertion:
                {
                    int length = winList.Count;
                    for (int i = length - 1; i >= 0; i--)
                    {
                        if (winList[i] != uiObject)
                        {
                            winList.RemoveAt(i);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (winList.Count == 0)
                    {
                        winList.Add(uiObject);
                    }
                }
                    break;
                case OpenFlag.Inorder:
                    winList.Add(uiObject);
                    break;
            }
        }
        
        /// <summary>
        /// 点击物理返回键的逻辑
        /// </summary>
        public void Back()
        {
            if (currentUI != null && currentUI.isActive)
            {
                currentUI.viewBase.OnExit();
            }
        }

        public void Update()
        {
            if (readyCloseWin.Count > 0 && !isBusy) 
            {
                var closeQueue = readyCloseWin.Dequeue();
                isBusy = true;
                closeQueue.ui.Close(closeQueue.tween).OnComplete(() => isBusy = false);
                return;
            }
            
            if (readyOpenWindow.Count > 0)
            {
                var openQueue = readyOpenWindow.Peek();
                if (openQueue.ui.isInit && !isBusy)
                {
                    isBusy = true;
                    openQueue = readyOpenWindow.Dequeue();
                    if (currentUI == openQueue.ui)
                    {
                        isBusy = false;
                    }
                    
                    openQueue.ui.Open(openQueue.tween, openQueue.flag, openQueue.isPopup, openQueue.args).OnComplete(() => isBusy = false);
                }
            }
        }
        #endregion

        public bool canPhysiceback = true;

    }
}
