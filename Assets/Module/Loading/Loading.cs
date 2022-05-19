using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class Loading : IProcess
    {
        public static event Action<string,object[]> openCallback;
        public static event Action<string> closeCallback;

        public static void Open(string style, string key, params object[] args)
        {
            UICommpont.FreezeUI(style);
            openCallback?.Invoke(style, args);
        }

        public static void Close(string style,string key)
        {
            UICommpont.UnFreezeUI(style);
            closeCallback?.Invoke(style);
        }

        private bool _isComplete;

        public object Current
        {
            get { return this; }
        }

        public Func<bool> listener { get; set; }

        public bool isComplete
        {
            get
            {
                if (listener == null)
                {
                    return _isComplete;
                }
                else
                {
                    return _isComplete || listener();
                }
            }
        }
        
        public void SetListener(Func<bool> listen)
        {
            this.listener = listen;
        }

        public virtual void Reset()
        {
        }

        public bool MoveNext()
        {
            return !isComplete;
        }
    }
}