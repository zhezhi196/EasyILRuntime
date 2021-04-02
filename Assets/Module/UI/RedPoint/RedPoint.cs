using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class RedPoint : MonoBehaviour
    {
        private IRedPoint[] target;
        public bool isActive { get; set; } = true;
        public bool isOn
        {
            get
            {
                if (!isActive) return false;
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
        
        private void OnDestroy()
        {
            if (!target.IsNullOrEmpty())
            {
                for (int i = 0; i < target.Length; i++)
                {
                    target[i].onSwitchStation -= OnSwitch;
                }
            }
        }

        public void SetTarget(params IRedPoint[] tar)
        {
            if (tar != null)
            {
                if (!target.IsNullOrEmpty())
                {
                    for (int i = 0; i < target.Length; i++)
                    {
                        target[i].onSwitchStation -= OnSwitch;
                    }
                }
                this.target = tar;
                
                if (!target.IsNullOrEmpty())
                {
                    for (int i = 0; i < target.Length; i++)
                    {
                        target[i].onSwitchStation += OnSwitch;
                    }
                }
            }
        }

        private void OnSwitch(bool obj)
        {
            gameObject.OnActive(isOn);
        }
    }
}