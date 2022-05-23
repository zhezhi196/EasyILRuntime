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
    public enum ButtonTriggerType
    {
        Disable,
        Point,
        Enter,
    }
    public class UIBtnBase : Button,IPoolObject,IButtonConfig
    {
        private bool isPointed;
        private float m_pointTime;
        public Action click;
        private Action<ButtonTriggerType> onPointDown;
        private Action<ButtonTriggerType> onPointUp;
        private Action<float> onPointing;
        public UIBtnBase[] rejectBtn;
        public int clickCount;
        [SerializeField]
        public ButtonConfig _config;

        public ButtonConfig config
        {
            get { return _config; }
        }

        public UIViewBase window { get; private set; }
        public bool hasInit { get; private set; }

        public bool isDetective { get; set; }
        public ObjectPool pool { get; set; }


        #region unity 自带事件

        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    if (!hasInit) Init();
                }
                else
                {
                    if (config.logical == NoChannelLogical.Hide)
                    {
                        gameObject.OnActive(false);
                    }
                    else if (config.logical == NoChannelLogical.Uninteractive)
                    {
                        interactable = false;
                    }
                }
            }
        }

        public void Click()
        {
            //onClick?.Invoke();
            base.onClick.Invoke();
        }

        public void BtnDown()
        {
            onPointDown?.Invoke(ButtonTriggerType.Point);
        }

        public void BtnUp()
        {
            onPointUp?.Invoke(ButtonTriggerType.Point);
        }

        public void Init()
        {
            if (base.transition == Transition.None)
            {
                Debug.LogWarning("按钮过渡为none", gameObject);
            }
            window = transform.GetComponentInParent<UIViewBase>();
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
                    DefaultListener();
                    click?.Invoke();
                    clickCount++;
                    if ((config.flag & UIButtonFlag.Analystics) != 0)
                    {
                        Analytics.SendEvent(Config.globleConfig.ButtonAnalyticsType, gameObject.name, 0);
                    }
                    if ((config.flag & UIButtonFlag.NoAudio) == 0)
                    {
                        if (config.audio.IsNullOrEmpty()) config.audio = Config.globleConfig.commonButtonAudio;
                        if (!config.audio.IsNullOrEmpty())
                        {
                            AudioManager.PlayUI(config.audio);
                            // AudioPlay.PlayOneShot(config.audio).SetIgnorePause(true);
                        }
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
                m_pointTime += TimeHelper.unscaledDeltaTimeIgnorePause;
                onPointing?.Invoke(m_pointTime);
            }
            OnChildUpdate();
        }

        void OnDestroy()
        {
            click = null;
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

        public virtual void OnActive(bool active)
        {
            gameObject.OnActive(active && Channel.HasChannel(config.channel));
        }

        public virtual void OnActive(bool active, Func<bool> fun)
        {
            gameObject.OnActive(active && Channel.HasChannel(config.channel) && (fun == null || fun.Invoke()));
        }
        
        public void AddListener(Action callBack)
        {
            if (Application.isPlaying)
            {
                click += callBack;
            }
        }

        public void AddListener<T>(Action<T> callBack, T arg)
        {
            if (Application.isPlaying)
            {
                click += () => { callBack(arg); };
            }
        }

        public void AddListener<T, K>(Action<T, K> callBack, T arg1, K arg2)
        {
            if (Application.isPlaying)
            {
                click += () => { callBack(arg1, arg2); };
            }
        }

        public void AddListener<T, K, L>(Action<T, K, L> callBack, T arg1, K arg2, L arg3)
        {
            if (Application.isPlaying)
            {
                click += () => { callBack(arg1, arg2, arg3); };
            }
        }

        public void AddListener<T, K, L, M>(Action<T, K, L, M> callBack, T arg1, K arg2, L arg3, M arg4)
        {
            if (Application.isPlaying)
            {
                click += () => { callBack(arg1, arg2, arg3, arg4); };
            }
        }

        public void RemoveListener(Action callback)
        {
            click -= callback;
        }

        #endregion

        #region AddPointDown

        public void AddPointDown(Action<ButtonTriggerType> callBack)
        {
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    onPointDown += callBack;
                }
                else
                {
                    gameObject.OnActive(false);
                }
            }
        }

        public void AddPointDown<T>(Action<ButtonTriggerType,T> callBack, T arg)
        {
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    onPointDown += t => { callBack(t,arg); };

                }
                else
                {
                    gameObject.OnActive(false);
                }
            }
        }

        public void AddPointDown<T, K>(Action<ButtonTriggerType,T, K> callBack, T arg1, K arg2)
        {
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    onPointDown += t => { callBack(t,arg1, arg2); };
                }
                else
                {
                    gameObject.OnActive(false);
                }
            }
        }

        public void AddPointDown<T, K, L>(Action<ButtonTriggerType,T, K, L> callBack, T arg1, K arg2, L arg3)
        {
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    onPointDown += t => { callBack(t,arg1, arg2, arg3); };
                }
                else
                {
                    gameObject.OnActive(false);
                }
            }
        }

        public void AddPointDown<T, K, L, M>(Action<ButtonTriggerType,T, K, L, M> callBack, T arg1, K arg2, L arg3, M arg4)
        {
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    onPointDown += t => { callBack(t,arg1, arg2, arg3, arg4); };
                }
                else
                {
                    gameObject.OnActive(false);
                }
            }

        }

        #endregion

        #region AddPointUp

        public void AddPointUp(Action<ButtonTriggerType> callBack)
        {
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    onPointUp += callBack;
                }
                else
                {
                    gameObject.OnActive(false);
                }
            }
        }

        public void AddPointUp<T>(Action<ButtonTriggerType, T> callBack, T arg)
        {
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    onPointUp += t => { callBack(t, arg); };
                }
                else
                {
                    gameObject.OnActive(false);
                }
            }
        }

        public void AddPointUp<T, K>(Action<ButtonTriggerType,T, K> callBack, T arg1, K arg2)
        {
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    onPointUp += t => { callBack(t,arg1, arg2); };
                }
                else
                {
                    gameObject.OnActive(false);
                }
            }
        }

        public void AddPointUp<T, K, L>(Action<ButtonTriggerType,T, K, L> callBack, T arg1, K arg2, L arg3)
        {
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    onPointUp += t => { callBack(t,arg1, arg2, arg3); };
                }
                else
                {
                    gameObject.OnActive(false);
                }
            }
        }

        public void AddPointUp<T, K, L, M>(Action<ButtonTriggerType,T, K, L, M> callBack, T arg1, K arg2, L arg3, M arg4)
        {
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    onPointUp += t => { callBack(t,arg1, arg2, arg3, arg4); };
                }
                else
                {
                    gameObject.OnActive(false);
                }
            }
        }

        #endregion

        #region AddPointing

        public void AddPointing(Action<float> callBack)
        {
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    onPointing += (pointTime) => { callBack(this.m_pointTime); };
                }
                else
                {
                    gameObject.OnActive(false);
                }
            }
        }

        public void AddPointing<T>(Action<float, T> callBack, T arg)
        {
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    onPointing += (pointTime) => { callBack(this.m_pointTime, arg); };
                }
                else
                {
                    gameObject.OnActive(false);
                }
            }
        }

        public void AddPointing<T, K>(Action<float, T, K> callBack, T arg1, K arg2)
        {
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    onPointing += (pointTime) => { callBack(this.m_pointTime, arg1, arg2); };

                }
                else
                {
                    gameObject.OnActive(false);
                }
            }
        }

        public void AddPointing<T, K, L>(Action<float, T, K, L> callBack, T arg1, K arg2, L arg3)
        {
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    onPointing += (pointTime) => { callBack(this.m_pointTime, arg1, arg2, arg3); };
                }
                else
                {
                    gameObject.OnActive(false);
                }
            }
        }

        public void AddPointing<T, K, L, M>(Action<float, T, K, L, M> callBack, T arg1, K arg2, L arg3, M arg4)
        {
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    onPointing += (pointTime) => { callBack(this.m_pointTime, arg1, arg2, arg3, arg4); };
                }
                else
                {
                    gameObject.OnActive(false);
                }
            }
        }

        #endregion

        #region 对点击抬起的重写
        
        protected override void OnDisable()
        {
            base.OnDisable();
            isPointed = false;
            m_pointTime = 0;
            onPointUp?.Invoke(ButtonTriggerType.Disable);
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (base.IsPressed() || isDetective)
            {
                isPointed = true;
                m_pointTime = 0;
                onPointDown?.Invoke(ButtonTriggerType.Point);
            }
        }

        // public override void OnPointerEnter(PointerEventData eventData)
        // {
        //     base.OnPointerEnter(eventData);
        //     if (base.IsPressed()||isDetective)
        //     {
        //         m_pointTime = 0;
        //         onPointDown?.Invoke(ButtonTriggerType.Enter);
        //     }
        // }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (!base.IsPressed() || isDetective)
            {
                isPointed = false;
                m_pointTime = 0;

                onPointUp?.Invoke(ButtonTriggerType.Point);
            }
        }
        
        // public override void OnPointerExit(PointerEventData eventData)
        // {
        //     base.OnPointerExit(eventData);
        //     if (!base.IsPressed()||isDetective)
        //     {
        //         m_pointTime = 0;
        //         onPointUp?.Invoke(ButtonTriggerType.Enter);
        //     }
        // }

        #endregion

        public virtual void ReturnToPool()
        {
            click = null;
        }

        public virtual void OnGetObjectFromPool()
        {
        }
    }
}
