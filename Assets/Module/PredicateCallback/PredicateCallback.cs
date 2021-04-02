using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public abstract class PredicateCallback
    {
        private static List<PredicateCallback> predicate = new List<PredicateCallback>();
        
        public static void Update()
        {
            for (int i = 0; i < predicate.Count; i++)
            {
                predicate[i].OnUpdate();
                var tar = predicate[i];
                if (tar.PredicateJudge())
                {
                    tar.CallBack();
                    if (tar.autoRemove)
                    {
                        predicate.Remove(tar);
                    }
                }
            }
        }

        public static T GetCallback<T>(object id, Func<bool> fun, Action callback, bool autoRemove) where T: PredicateCallback,new()
        {
            for (int i = 0; i < predicate.Count; i++)
            {
                if (id == predicate[i].id)
                {
                    return predicate[i] as T;
                }
            }

            T tar = new T();
            tar.OnCreat(id, fun, callback, autoRemove);
            return tar;
        }
        
        public object id;
        protected Func<bool> predicateJudge;
        protected Action callback;
        public bool autoRemove;
        
        private PredicateCallback()
        {
        }
        
        protected virtual void OnCreat(object id, Func<bool> fun, Action callback, bool autoRemove)
        {
            this.id = id;
            this.predicateJudge = fun;
            this.callback = callback;
            this.autoRemove = autoRemove;
            predicate.Add(this);
        }
        protected virtual void OnUpdate()
        {
        }

        protected virtual bool PredicateJudge()
        {
            if (predicateJudge != null)
            {
                return predicateJudge.Invoke();
            }

            return true;
        }

        protected virtual void CallBack()
        {
            this.callback?.Invoke();
        }
    }
}