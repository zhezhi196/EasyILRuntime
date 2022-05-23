/*
 * 脚本名称：UICommpont
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-03-06 20:09:49
 * 脚本作用：
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module
{
    public static class UICommpont
    {
        private static Transform m_UIRoot;
        private static UIMask m_Mask;
        private static Transform m_WinParent;
        private static Transform m_topParent;
        private static PopupWin m_popup;
        private static Canvas m_UICanvas;
        private static Canvas m_3DCanvas;

        private static CanvasScaler m_CanvasScaler;

        private static Transform _left;
        private static Transform _right;
        private static Transform _up;
        private static Transform _down;

        private static Transform _TopMenue;
        private static Transform _MenuBar;

        private static Camera _2DCamera;
        private static Transform _3DCreatPoint;

        public static Camera camera2D
        {
            get
            {
                if (_2DCamera == null)
                {
                    _2DCamera = GameObject.Find("UIRoot/Camera/2D").GetComponent<Camera>();
                }

                return _2DCamera;
            }
        }

        public static Transform creatPoint3D
        {
            get
            {
                if (_3DCreatPoint == null)
                {
                    return GameObject.Find("UIRoot/3DParent/prefab").transform;
                }

                return _3DCreatPoint;
            }
        }
        
        public static CanvasScaler UICanvasScaler
        {
            get
            {
                if (m_CanvasScaler == null)
                {
                    m_CanvasScaler = GameObject.Find("UIRoot/Canvas").GetComponent<CanvasScaler>();
                }

                return m_CanvasScaler;
            }
        }

        public static Canvas UICanvas
        {
            get
            {
                if (m_UICanvas == null)
                {
                    m_UICanvas = GameObject.Find("UIRoot/Canvas").GetComponent<Canvas>();
                }

                return m_UICanvas;
            }
        }

        public static Canvas UICanvas3D
        {
            get
            {
                if (m_3DCanvas == null)
                {
                    GameObject o = GameObject.Find("UIRoot/3DCanvas");
                    if (o != null)
                    {
                        m_3DCanvas = GameObject.Find("UIRoot/3DCanvas").GetComponent<Canvas>();
                    }
                }

                return m_3DCanvas;
            }
        }


        public static Transform UIRoot
        {
            get
            {
                if (m_UIRoot == null)
                {
                    m_UIRoot = GameObject.Find("UIRoot").transform;
                }

                return m_UIRoot;
            }
        }

        public static UIMask Mask
        {
            get
            {
                if (m_Mask == null)
                {
                    m_Mask = UIRoot.Find("Canvas/Mask").GetComponent<UIMask>();
                }

                return m_Mask;
            }
        }

        public static Transform TopMenu
        {
            get
            {
                if (_TopMenue == null)
                {
                    _TopMenue = UIRoot.Find("Canvas/TopWindow/TopMenu");
                }

                return _TopMenue;
            }
        }
        public static Transform MenuBar
        {
            get
            {
                if (_MenuBar == null)
                {
                    _MenuBar = UIRoot.Find("Canvas/TopWindow/MenuBar");
                }

                return _MenuBar;
            }
        }

        public static Transform winParent
        {
            get
            {
                if (m_WinParent == null)
                {
                    m_WinParent = UIRoot.Find("Canvas/Windows");
                }

                return m_WinParent;
            }
        }

        public static Transform topParent
        {
            get
            {
                if (m_topParent == null)
                {
                    m_topParent = UIRoot.Find("Canvas/TopWindow");
                }

                return m_topParent;
            }
        }

        /// <summary>
        /// 动画做的坐标
        /// </summary>
        public static Transform Left
        {
            get
            {
                if (_left == null)
                {
                    _left = UIRoot.Find("Canvas/PrefabUI/WinPos/Left");
                }

                return _left;
            }
        }

        /// <summary>
        /// 动画右的坐标
        /// </summary>
        public static Transform Right
        {
            get
            {
                if (_right == null)
                {
                    _right = UIRoot.Find("Canvas/PrefabUI/WinPos/Right");
                }

                return _right;
            }
        }

        /// <summary>
        /// 动画上的坐标
        /// </summary>
        public static Transform Up
        {
            get
            {
                if (_up == null)
                {
                    _up = UIRoot.Find("Canvas/PrefabUI/WinPos/Up");
                }

                return _up;
            }
        }

        /// <summary>
        /// 动画下的坐标
        /// </summary>
        public static Transform Down
        {
            get
            {
                if (_down == null)
                {
                    _down = UIRoot.Find("Canvas/PrefabUI/WinPos/Down");
                }

                return _down;
            }
        }
        
        
        private static List<object> m_freezeList = new List<object>();


        /// <summary>
        /// 窗口是否可点击
        /// </summary>
        public static bool isFreezed
        {
            get { return m_freezeList.Count != 0; }
        }
        /// <summary>
        /// 冻结窗口
        /// </summary>
        /// <param id="key"></param>
        public static void FreezeUI(object key, bool mask = false)
        {
            m_freezeList.Add(key);
            Mask.gameObject.OnActive(true);
            if (mask)
            {
                Mask.ShowMask();
            }
        }

        /// <summary>
        /// 解冻窗口
        /// </summary>
        /// <param buffId="key"></param>
        public static void UnFreezeUI(object key)
        {
            m_freezeList.Remove(key);

            if (m_freezeList.Count == 0)
            {
                Mask.gameObject.OnActive(false);
                Mask.HideMask();
            }
        }

        public static async void UnFreezeUI(string key, float time)
        {
            await Async.WaitforSeconds(time);
            UnFreezeUI(key);
        }

        public static List<object> GetFreezeList()
        {
            return m_freezeList;
        }

        public static void UnFreezeUIAll()
        {
            Mask.gameObject.OnActive(false);
            Mask.HideMask();
            m_freezeList.Clear();
        }

        public static void ClearList()
        {
            m_freezeList.Clear();
        }
    }
}