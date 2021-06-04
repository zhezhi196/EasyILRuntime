/*
 * 脚本名称：UISequence
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-01-07 22:11:37
 * 脚本作用：
*/

using System;
using System.Collections.Generic;

namespace Module
{
    public class UISequence
    {
        #region 静态 

        private static Dictionary<string, UISequence> mDic = new Dictionary<string, UISequence>();

        public static UISequence GetSequence(string ID)
        {
            if (mDic.ContainsKey(ID))
            {
                return mDic[ID];
            }
            return null;
        }

        #endregion

        #region 字段属性

        private Queue<QueueAtrribute> winQueue = new Queue<QueueAtrribute>();
        private int type;
        public string ID { get; private set; }
        public UIObject currentUI { get; private set; }

        private event Action onComplete;

        #endregion

        #region 构造函数

        public UISequence(string ID)
        {
            this.ID = ID;
            if (mDic.ContainsKey(ID))
            {
                GameDebug.LogError(string.Format("已有{0}的UI队列", ID));
                return;
            }

            mDic.Add(ID, this);
        }

        public UISequence()
        {
        }

        #endregion

        #region 添加窗口到序列

        /// <summary>
        /// 添加窗口到序列
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        public void Add(string name, UITweenType tweenType, params object[] args)
        {
            QueueAtrribute queue = new QueueAtrribute(name, tweenType, args);
            winQueue.Enqueue(queue);
        }

        #endregion

        #region 窗口打开

        /// <summary>
        /// 挨个打开序列中的窗口
        /// </summary>
        /// <param name="type"></param>
        public void Open()
        {
            type = 0;
            if (winQueue.Count > 0)
            {
                QueueAtrribute win = winQueue.Dequeue();
                currentUI = UIController.Instance.Open(win.name, win.tweenType, win.args);
                currentUI.onSequenceComplete += NextWin;
            }
            else
            {
                currentUI.onSequenceComplete -= NextWin;
                onComplete?.Invoke();
                onComplete = null;
                if (ID != null)
                {
                    mDic.Remove(ID);
                }
            }
        }

        /// <summary>
        /// 挨个打开序列中的窗口,并且不关闭上一个窗口
        /// </summary>
        /// <param name="type"></param>
        public void Popup()
        {
            type = 1;
            if (winQueue.Count > 0)
            {
                QueueAtrribute win = winQueue.Dequeue();
                currentUI = UIController.Instance.Popup(win.name, win.tweenType, win.args);
                currentUI.onSequenceComplete += NextPopupWin;
            }
            else
            {
                currentUI.onSequenceComplete -= NextPopupWin;
                onComplete?.Invoke();
                onComplete = null;
                if (ID != null)
                {
                    mDic.Remove(ID);
                }
            }
        }

        #endregion

        #region 下一个窗口

        private void NextWin(UITweenType type)
        {
            currentUI.onSequenceComplete -= NextWin;
            Open();
        }

        private void NextPopupWin(UITweenType type)
        {
            currentUI.onSequenceComplete -= NextPopupWin;
            Popup();
        }

        public void Interrupt()
        {
            winQueue.Clear();
            if (ID != null)
            {
                mDic.Remove(ID);
            }

            if (currentUI != null)
            {
                if (type == 0)
                {
                    currentUI.onSequenceComplete -= NextWin;
                }
                else if (type == 1)
                {
                    currentUI.onSequenceComplete -= NextPopupWin;
                } 
            }
            onComplete?.Invoke();
        }

        #endregion

        public void OnComplete(Action callBack)
        {
            onComplete += callBack;
        }

        #region 序列的参数

        private struct QueueAtrribute
        {
            public string name;
            public float delay;
            public object[] args;
            public UITweenType tweenType;
            public QueueAtrribute(string name,UITweenType tweenType, object[] args)
            {
                this.name = name;
                this.delay = 0;
                this.tweenType = tweenType;
                this.args = args;
            }
        }

        #endregion
    }
}
