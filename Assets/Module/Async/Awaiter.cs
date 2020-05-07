using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Module;
using UnityEngine;

namespace Module
{
    public class Awaiter : IAwaiter
    {
        private bool m_isComplete;
        private Action callback;
        public bool IsCompleted
        {
            get
            {
                return m_isComplete;
            }
        }
        
        public void OnCompleted(Action continuation)
        {
            callback = continuation;
        }
        
        public void SetResult()
        {
            callback?.Invoke();
            m_isComplete = true;
        }
        public void GetResult()
        {
            m_isComplete = false;
        }
    }
}

