﻿using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    public class RedPoint : MonoBehaviour, IRedPoint
    {
        private IRedPoint[] temTar;
        private bool isCache;
        
        [ReadOnly]
        public bool isInit;
        public bool isActive = true;

        public IRedPoint[] target;
        public Func<bool> predicate;

        public bool redPointIsOn
        {
            get
            {
                if (!isActive) return false;
                if (predicate != null && predicate.Invoke()) return true;
                if (!target.IsNullOrEmpty())
                {
                    for (int i = 0; i < target.Length; i++)
                    {
                        if (target[i].redPointIsOn) return true;
                    }
                }

                return false;
            }
        }

        public event Action<bool> onSwitchRedPointStation;

        private void Awake()
        {
            isInit = true;
        }

        private void OnDestroy()
        {
            if (!target.IsNullOrEmpty())
            {
                for (int i = 0; i < target.Length; i++)
                {
                    target[i].onSwitchRedPointStation -= OnSwitch;
                }
            }

            target = null;
            isActive = false;
        }

        public void Refresh()
        {
            bool temp = redPointIsOn;
            gameObject.OnActive(temp);

            onSwitchRedPointStation?.Invoke(temp);
        }

        private void OnEnable()
        {
            if (isCache)
            {
                SetTarget(temTar);
                isCache = false;
                temTar = null;
            }
        }

        public void SetPredicate(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public void SetTarget(params IRedPoint[] tar)
        {
            if (!isInit)
            {
                isCache = true;
                temTar = tar;
                return;
            }
            
            if (tar != null)
            {
                if (!target.IsNullOrEmpty())
                {
                    for (int i = 0; i < target.Length; i++)
                    {
                        target[i].onSwitchRedPointStation -= OnSwitch;
                    }
                }

                this.target = tar;

                for (int i = 0; i < target.Length; i++)
                {
                    target[i].onSwitchRedPointStation += OnSwitch;
                }
                
                Refresh();
            }
        }

        private void OnSwitch(bool obj)
        {
            if (isActive)
            {
                Refresh();
            }
        }
    }
}