/*
* 脚本名称：UIBtnBase
* 项目名称：ugui
* 脚本作者：黄哲智
* 创建时间：2018-01-06 16:51:15
* 脚本作用：
*/

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Module
{
    public class UIBtnBase : Button,IPoolObject
    {
        public static string defaultAudio = "tongyongButton";
        private float m_pointTime;
        public Action onClick;
        private Action onPointDown;
        private Action onPointUp;
        private Action<float> onPointing;

        public UIBtnBase[] rejectBtn;
        public bool mute;
        public string playAudio;
        public int clickCount;

        public UIViewBase window { get; private set; }
        public bool hasInit { get; private set; }

        public bool isDetective { get; set; }
        private bool isPointed;
        public bool isAnalytics;

        #region unity 自带事件

        protected override void Awake()
        {
            base.Awake();
            if (!hasInit) Init();
        }

        public void Click()
        {
            onClick?.Invoke();
        }

        public void Init()
        {
            window = transform.GetComponentInParent<UIViewBase>();
            onClick += DefaultListener;
            base.onClick.AddListener(() =>
            {
                bool temp = false;
                for (int i = 0; i < rejectBtn.Length; i++)
                {
                    if (rejectBtn[i].isPointed)
                    {
                        temp = true;
                        break;
                    }
                }

                if (!temp)
                {
                    onClick?.Invoke();
                    clickCount++;
                    GlobleAction.onButtonClick?.Invoke(this);
                    if (!mute)
                    {
                        if (playAudio.IsNullOrEmpty()) playAudio = defaultAudio;
                        AudioPlay.PlayOneShot(playAudio);
                    }
                }
            });

            if (window != null)
            {
                window.OnWinInitComplete += OnWinComplete;
            }

            OnChildAwake();
        }

        public void OnClickAction()
        {
            if (this.gameObject.activeInHierarchy&&interactable)
            {
                Click();
            }
        }

        public void OnWinComplete()
        {
            OnChildStart();
        }

        void Update()
        {
            if (isPointed || isDetective)
            {
                m_pointTime += TimeHelper.unscaledDeltaTime;
                onPointing?.Invoke(m_pointTime);
            }
            OnChildUpdate();
        }

        void OnDestroy()
        {
            onClick = null;
            onPointDown = null;
            onPointUp = null;
            onPointing = null;
            OnChildDestroy();
        }

        protected override void OnEnable()
        {
            if (interactable)
            {
                DoStateTransition(SelectionState.Normal, false);
            }
            else
            {
                DoStateTransition(SelectionState.Disabled, false);
            }
        }

        protected override void OnDisable()
        {
            isPointed = false;
            m_pointTime = 0;
            onPointUp?.Invoke();
        }

        protected virtual void OnChildDestroy()
        {
        }

        protected virtual void OnChildAwake()
        {
        }

        protected virtual void OnChildStart()
        {
        }

        protected virtual void DefaultListener()
        {
        }

        protected virtual void OnChildUpdate()
        {
        }
        #endregion

        #region AddListener

        public void AddListener(Action callBack)
        {
            onClick += () => { callBack(); };
        }

        public void AddListener<T>(Action<T> callBack, T arg)
        {
            onClick += () => { callBack(arg); };
        }

        public void AddListener<T, K>(Action<T, K> callBack, T arg1, K arg2)
        {
            onClick += () => { callBack(arg1, arg2); };
        }

        public void AddListener<T, K, L>(Action<T, K, L> callBack, T arg1, K arg2, L arg3)
        {
            onClick += () => { callBack(arg1, arg2, arg3); };
        }

        public void AddListener<T, K, L, M>(Action<T, K, L, M> callBack, T arg1, K arg2, L arg3, M arg4)
        {
            onClick += () => { callBack(arg1, arg2, arg3, arg4); };
        }

        #endregion

        #region AddPointDown

        public void AddPointDown(Action callBack)
        {
            onPointDown += () => { callBack(); };
        }

        public void AddPointDown<T>(Action<T> callBack, T arg)
        {
            onPointDown += () => { callBack(arg); };
        }

        public void AddPointDown<T, K>(Action<T, K> callBack, T arg1, K arg2)
        {
            onPointDown += () => { callBack(arg1, arg2); };
        }

        public void AddPointDown<T, K, L>(Action<T, K, L> callBack, T arg1, K arg2, L arg3)
        {
            onPointDown += () => { callBack(arg1, arg2, arg3); };
        }

        public void AddPointDown<T, K, L, M>(Action<T, K, L, M> callBack, T arg1, K arg2, L arg3, M arg4)
        {
            onPointDown += () => { callBack(arg1, arg2, arg3, arg4); };
        }

        #endregion

        #region AddPointUp

        public void AddPointUp(Action callBack)
        {
            onPointUp += () => { callBack(); };
        }

        public void AddPointUp<T>(Action<T> callBack, T arg)
        {
            onPointUp += () => { callBack(arg); };
        }

        public void AddPointUp<T, K>(Action<T, K> callBack, T arg1, K arg2)
        {
            onPointUp += () => { callBack(arg1, arg2); };
        }

        public void AddPointUp<T, K, L>(Action<T, K, L> callBack, T arg1, K arg2, L arg3)
        {
            onPointUp += () => { callBack(arg1, arg2, arg3); };
        }

        public void AddPointUp<T, K, L, M>(Action<T, K, L, M> callBack, T arg1, K arg2, L arg3, M arg4)
        {
            onPointUp += () => { callBack(arg1, arg2, arg3, arg4); };
        }

        #endregion

        #region AddPointing

        public void AddPointing(Action<float> callBack)
        {
            onPointing += (pointTime) => { callBack(this.m_pointTime); };
        }

        public void AddPointing<T>(Action<float, T> callBack, T arg)
        {
            onPointing += (pointTime) => { callBack(this.m_pointTime, arg); };
        }

        public void AddPointing<T, K>(Action<float, T, K> callBack, T arg1, K arg2)
        {
            onPointing += (pointTime) => { callBack(this.m_pointTime, arg1, arg2); };
        }

        public void AddPointing<T, K, L>(Action<float, T, K, L> callBack, T arg1, K arg2, L arg3)
        {
            onPointing += (pointTime) => { callBack(this.m_pointTime, arg1, arg2, arg3); };
        }

        public void AddPointing<T, K, L, M>(Action<float, T, K, L, M> callBack, T arg1, K arg2, L arg3, M arg4)
        {
            onPointing += (pointTime) => { callBack(this.m_pointTime, arg1, arg2, arg3, arg4); };
        }

        #endregion

        #region 对点击抬起的重写

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (base.IsPressed() || isDetective)
            {
                isPointed = true;
                m_pointTime = 0;
                onPointDown?.Invoke();
            }
        }

        //        public override void OnPointerEnter(PointerEventData eventData)
        //        {
        //            base.OnPointerEnter(eventData);
        //
        //            if (base.IsPressed()||isDetective)
        //            {
        //                m_pointTime = 0;
        //                onPointDown?.Invoke();
        //            }
        //        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (!base.IsPressed() || isDetective)
            {
                isPointed = false;
                m_pointTime = 0;

                onPointUp?.Invoke();
            }
        }
        //
        //        public override void OnPointerExit(PointerEventData eventData)
        //        {
        //            base.OnPointerExit(eventData);
        //
        //            if (!base.IsPressed()||isDetective)
        //            {
        //                m_pointTime = 0;
        //                onPointUp?.Invoke();
        //            }
        //        }

        #endregion

        public ObjectPool pool { get; set; }
        public virtual void ReturnToPool()
        {
            pool.ReturnObject(this);
            onClick = null;
        }

        public virtual void OnGetObjectFromPool()
        {
        }
    }
}
