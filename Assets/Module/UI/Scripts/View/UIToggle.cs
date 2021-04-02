using System;
using UnityEngine;
using UnityEngine.UI;

namespace Module
{
    public class UIToggle : Toggle, IPoolObject
    {
        private int _clickCount;
        public string playAudio;
        public UIButtonFlag flag;
        private Action<bool> onClick;
        public GameObject onGo;
        public GameObject offGo;

        public int index
        {
            get { return (group as UIToggleGroup).GetToggleIndex(this); }
        }

        public int clickCount
        {
            get { return _clickCount; }
        }

        public ObjectPool pool { get; set; }

        protected override void Awake()
        {
            base.Awake();
            onValueChanged.AddListener(OnToggleChanged);
        }

        public virtual void OnToggleChanged(bool arg0)
        {
            onClick?.Invoke(arg0);
            _clickCount++;
            GlobleAction.onToggleClick?.Invoke(this);
            if ((flag & UIButtonFlag.Mute) == 0)
            {
                if (playAudio.IsNullOrEmpty()) playAudio = ButtonConfig.defaultAudio;
                AudioPlay.PlayOneShot(playAudio);
            }

            (group as UIToggleGroup).OnToggleChanged(this, arg0);
            if (onGo != null)
            {
                onGo.OnActive(arg0);
            }

            if (offGo != null)
            {
                offGo.OnActive(!arg0);
            }
        }
        

        public void AddListener(Action<bool> callback)
        {
            onClick += s => callback?.Invoke(s);
        }
        
        public void AddListener<T>(Action<bool,T> callback,T arg)
        {
            onClick += s => callback?.Invoke(s, arg);
        }

        public void AddListener<T, K>(Action<bool, T, K> callback, T arg, K arg1)
        {
            onClick += s => callback?.Invoke(s, arg, arg1);
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