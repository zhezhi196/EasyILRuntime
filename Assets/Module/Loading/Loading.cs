using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class Loading : IProcess
    {
        private static Dictionary<Type, Loading> loadingStyle = new Dictionary<Type, Loading>();

        public static T Load<T>() where T : Loading, new()
        {
            return Load<T>(null);
        }
        
        public static T Load<T>(Func<bool> predicate) where T : Loading, new()
        {
            Loading result = default;
            if (!loadingStyle.TryGetValue(typeof(T), out result))
            {
                result = new T();
                loadingStyle.Add(typeof(T), result);
            }
            
            if (result.station != 0) return result as T;
            result.OpenLoading();
            if (predicate != null)
            {
                WaitClose<T>(predicate);
            }
            return result as T;
        }

        private static async void WaitClose<T>(Func<bool> predicate)
        {
            await Async.WaitUntil(predicate);
            Close<T>();
        }

        public static void Close<T>()
        {
            loadingStyle[typeof(T)].Reset();
        }

        public bool MoveNext()
        {
            return !isComplete;
        }

        private bool _isComplete;
        public object Current { get; }
        public int station { get; set; }

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

        public Action onComplete;

        public virtual void OpenLoading()
        {
        }

        public virtual void Reset()
        {
        }
    }
}