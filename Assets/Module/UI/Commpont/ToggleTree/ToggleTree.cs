using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    public class ToggleTree : MonoBehaviour, IPoolObject
    {
        public ButtonConfig config;
        
        private List<ToggleTree> _children;
        [SerializeField] 
        private ToggleTree _parent;
        [ShowInInspector]
        public int layer
        {
            get
            {
                if (parent == null) return 0;
                return parent.layer + 1;
            }
        }
        
        [ShowInInspector]
        public int index
        {
            get
            {
                if (parent == null) return 0;
                for (int i = 0; i < parent._children.Count; i++)
                {
                    if (parent._children[i] == this)
                    {
                        return i;
                    }
                }

                return 0;
            }
        }
        [SerializeField,HideInInspector]
        private ToggleTreeStation _station;
        private ToggleTree m_parent;
        private ToggleTreeStation _lastActiveStation;
        private Action<ToggleTreeStation, ToggleTreeStation> onToggleChanged;
        public ToggleTree parent
        {
            get { return m_parent; }
            set
            {
                if (m_parent != null)
                {
                    m_parent.RemoveChild(this);
                }

                if (value != null)
                {
                    m_parent = value;
                    m_parent.AddChild(this);
                }
            }
        }
        
        public ObjectPool pool { get; set; }

        
        [ShowInInspector]
        public ToggleTreeStation station
        {
            get { return _station; }
            set
            {
                if (_station != value)
                {
                    var temp = _station;
                    _station = value;
                    if (value == ToggleTreeStation.On)
                    {
                        if (_children != null)
                        {
                            for (int i = 0; i < _children.Count; i++)
                            {
                                _children[i].station = _children[i]._lastActiveStation;
                            }
                        }

                        if (_parent != null && _parent._children != null)
                        {
                            for (int i = 0; i < _parent._children.Count; i++)
                            {
                                if (_parent._children[i].station == ToggleTreeStation.On &&
                                    _parent._children[i] != this)
                                {
                                    _parent._children[i].station = ToggleTreeStation.Off;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_children != null)
                        {
                            for (int i = 0; i < _children.Count; i++)
                            {
                                _children[i]._lastActiveStation = _children[i].station;
                                _children[i].station = ToggleTreeStation.Unactive;
                            }
                        }
                    }


                    OnToggleChanged(temp, _station);
                    onToggleChanged?.Invoke(temp, _station);
                }
            }
        }

        private void Awake()
        {
            if (_parent != null)
            {
                parent = _parent;
            }

            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }

        private void AddChild(ToggleTree child)
        {
            if (_children == null) _children = new List<ToggleTree>();
            if (!_children.Contains(child))
            {
                _children.Add(child);
            }
        }

        private void RemoveChild(ToggleTree child)
        {
            if (_children == null) _children = new List<ToggleTree>();

            if (_children.Contains(child))
            {
                _children.Remove(child);
            }
        }

        public void ReturnToPool()
        {
            pool.ReturnObject(this);
            onToggleChanged = null;
        }

        public void OnGetObjectFromPool()
        {
            station = _station;
        }

        public virtual void OnToggleChanged(ToggleTreeStation from, ToggleTreeStation to)
        {
        }

        public void NotifyToggleOn(int index)
        {
            NotifyToggleOn(_children[index]);
        }

        public void NotifyToggleOn(ToggleTree toggle)
        {
            toggle.station = ToggleTreeStation.On;
        }

        public virtual void AddListener(Action<ToggleTreeStation, ToggleTreeStation> callback)
        {
            onToggleChanged = callback;
        }
        public virtual void AddListener<T>(Action<ToggleTreeStation, ToggleTreeStation,T> callback,T arg)
        {
            onToggleChanged = (a, b) => callback?.Invoke(a, b, arg);
        }
        public virtual void AddListener<T,K>(Action<ToggleTreeStation, ToggleTreeStation,T,K> callback,T arg,K arg1)
        {
            onToggleChanged = (a, b) => callback?.Invoke(a, b, arg, arg1);
        }
    }
}