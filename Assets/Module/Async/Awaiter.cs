using System;
using UnityEngine;

namespace Module
{
    public class Awaiter : IAwaiter
    {
        public static Awaiter GetAwaiter()
        {
            return new Awaiter();
        }

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
            callback = null;
        }
    }
    
    public class Awaiter<T> : IAwaiter
    {
        public static  Awaiter<T> GetAwaiter()
        {
            return new  Awaiter<T>();
        }

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
            callback = null;
        }
    }
}

