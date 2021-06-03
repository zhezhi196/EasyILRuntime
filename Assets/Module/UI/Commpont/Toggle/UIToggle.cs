using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Module
{
    public class UIToggle : Toggle, IPoolObject, IButtonConfig
    {
        [SerializeField]
        private ButtonConfig _config;

        public ButtonConfig config
        {
            get { return _config; }
        }

        private int _clickCount;
        private Action<bool> onClick;
        public GameObject onGo;
        public GameObject offGo;
        private bool _ison;
        public int index
        {
            get { return (group as UIToggleGroup).GetToggleIndex(this); }
        }

        public int clickCount
        {
            get { return _clickCount; }
        }

        public ObjectPool pool { get; set; }
        
        public void OnActive(bool active)
        {
            gameObject.OnActive(active && Channel.HasChannel(config.channel));
        }

        public void OnActive(bool active, Func<bool> fun)
        {
            gameObject.OnActive(active && Channel.HasChannel(config.channel) && (fun == null || fun.Invoke()));
        }

        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
            {
                if (Channel.HasChannel(config.channel))
                {
                    _ison = this.isOn;
                    onValueChanged.AddListener(OnToggleChanged);
                    OnAwake();
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

            if (base.transition == Transition.None)
            {
                Debug.LogError("按钮过渡为none", gameObject);
            }
        }
        

        protected override void Start()
        {
            base.Start();
            if (Application.isPlaying)
            {
                OnStart();
            }
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnToggle(bool arg0)
        {
            
        }

        protected virtual void OnPoint(PointerEventData eventData)
        {
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (Application.isPlaying)
            {
                if ((config.flag & UIButtonFlag.NoAudio) == 0)
                {
                    if (config.audio.IsNullOrEmpty()) config.audio = ButtonConfig.defaultAudio;
                    AudioPlay.PlayOneShot(config.audio).SetIgnorePause(true);
                }
                
                OnPoint(eventData);
            }
        }

        public void OnToggleChanged(bool arg0)
        {
            if(_ison== arg0) return;
            _ison = arg0;
            if (Application.isPlaying)
            {
                onClick?.Invoke(arg0);
                _clickCount++;
                GlobleAction.onButtonClick?.Invoke(this);

                (group as UIToggleGroup).OnToggleChanged(this, arg0);
                if (onGo != null)
                {
                    onGo.OnActive(arg0);
                }

                if (offGo != null)
                {
                    offGo.OnActive(!arg0);
                }
                
                OnToggle(arg0);
            }
        }
        

        public void AddListener(Action<bool> callback)
        {
            if (Application.isPlaying)
            {
                onClick += callback;
            }
        }
        
        public void AddListener<T>(Action<bool,T> callback,T arg)
        {
            if (Application.isPlaying)
            {
                onClick += s => callback?.Invoke(s, arg);
            }
        }

        public void AddListener<T, K>(Action<bool, T, K> callback, T arg, K arg1)
        {
            if (Application.isPlaying)
            {
                onClick += s => callback?.Invoke(s, arg, arg1);
            }
        }
        
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