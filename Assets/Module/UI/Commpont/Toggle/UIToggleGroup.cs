using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Module
{
    public class UIToggleGroup : ToggleGroup, IPoolObject
    {
        private UIToggle _currToggle;
        public ObjectPool pool { get; set; }

        public List<Toggle> toggles
        {
            get { return m_Toggles; }
        }

        public UIToggle currentToggle
        {
            get
            {
                if (_currToggle == null && startSetDefault)
                {
                    _currToggle = m_Toggles[0] as UIToggle;
                }
                return _currToggle;
            }
        }

        public bool startSetDefault = true;

        protected override void Start()
        {
            if (startSetDefault)
            {
                base.Start();
            }
        }

        public int currentIndex
        {
            get
            {
                return currentToggle.index;
            }
        }

        private Action<UIToggle, bool> onToggleChange;

        public virtual void OnToggleChanged(UIToggle uiToggle, bool arg0)
        {
            _currToggle = uiToggle;
            onToggleChange?.Invoke(uiToggle, arg0);
        }


        public void SortToggles<T>(Comparison<T> comparison, bool sortTransform) where T : Toggle
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                Comparison<Toggle> newCom = (a, b) => comparison.Invoke(a as T, b as T);
                toggles.Sort(newCom);
            }

            if (sortTransform)
            {
                for (int i = 0; i < toggles.Count; i++)
                {
                    toggles[i].transform.SetSiblingIndex(i);
                }
            }
        }

        public int GetToggleIndex(Toggle toggle)
        {
            for (int i = 0; i < m_Toggles.Count; i++)
            {
                if (m_Toggles[i] == toggle)
                {
                    return i;
                }
            }

            return -1;
        }

        public void AddListener(Action<UIToggle, bool> callback)
        {
            onToggleChange += callback;
        }

        public void AddListener<T>(Action<UIToggle, bool, T> callback, T arg)
        {
            onToggleChange += (a, b) => callback?.Invoke(a, b, arg);
        }

        public void AddListener<T, K>(Action<UIToggle, bool, T, K> callback, T arg, K arg1)
        {
            onToggleChange += (a, b) => callback?.Invoke(a, b, arg, arg1);
        }

        public void NotifyToggleOn(Toggle toggle, bool sendCallback = true, UIButtonFlag flag = 0)
        {
            toggle.isOn = true;
            UIToggle tar=toggle as UIToggle;
            var bakFlg = tar.config.flag;
            tar.config.flag = flag;
            base.NotifyToggleOn(toggle, sendCallback);
            tar.config.flag = bakFlg;
            
        }

        public void NotifyToggleOn(int index, bool sendCallback = true, UIButtonFlag flag = 0)
        {
            if (index >= m_Toggles.Count) return;
            for (int i = 0; i < toggles.Count; i++)
            {
                UIToggle togg = toggles[i] as UIToggle;
                if (togg != null)
                {
                    if (index == togg.index)
                    {
                        if (togg.onGo != null) togg.onGo.OnActive(true);
                        if (togg.offGo != null) togg.offGo.OnActive(false);
                    }
                    else
                    {
                        if (togg.onGo != null) togg.onGo.OnActive(false);
                        if (togg.offGo != null) togg.offGo.OnActive(true);
                    }
                }
            }
            NotifyToggleOn(m_Toggles[index], sendCallback, flag);
        }

        public virtual void ReturnToPool()
        {
            pool.ReturnObject(this);
            for (int i = 0; i < m_Toggles.Count; i++)
            {
                m_Toggles[i].group = null;
            }

            onToggleChange = null;
        }

        public async void CreatToggle<T>(string path, Transform parent, int count, Action<List<T>> callback,
            bool byOrder = false) where T : UIToggle
        {
            List<T> res = new List<T>();

            Voter v = new Voter(count, () => callback?.Invoke(res));
            for (int i = 0; i < count; i++)
            {
                bool isComplete = false;
                AssetLoad.LoadGameObject<T>(path, parent, (a, b) =>
                {
                    a.group = this;
                    isComplete = true;
                    res.Add(a);
                    v.Add();
                });
                
                if (byOrder)
                {
                    await Async.WaitUntil(() => isComplete);
                }
            }
        }
        
        public virtual void OnGetObjectFromPool()
        {
        }
    }
}