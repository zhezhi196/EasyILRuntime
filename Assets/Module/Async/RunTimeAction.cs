using System;
using System.Collections;
using UnityEngine;

namespace Module
{
    
    public interface IRunTime
    {
        bool isComplete { get; }

        void BeginAction();
        void Complete();
    }

    public class RunTimeAction : IRunTime
    {
        #region 字段属性事件

        private bool m_isComplete;
        private Action action;
        private Action onStart;
        private Action onComplete;



        public bool isComplete
        {
            get { return m_isComplete; }
        }
    
        #endregion
    
        public RunTimeAction(Action action)
        {
            this.action = action;
        }

        public void BeginAction()
        {
            onStart?.Invoke();
            //Type t = action.Target.GetType();
            //GameDebug.Log(action.Target.GetType().DeclaringType + "开始执行" + action.Method.Name);
            action?.Invoke();
        }

        public void Complete()
        {
            m_isComplete = true;
            //GameDebug.Log(action.Target.GetType().DeclaringType.ToString() + action.Method + "执行 结束");
            onComplete?.Invoke();
        }

        public void OnStart(Action<IRunTime> action)
        {
            onStart += () => action(this);
        }

        public void OnComplete(Action<IRunTime> action)
        {
            onComplete += () => action(this);
        }
    }
    public class RunTimeAction<T> : IRunTime
    {
        #region 字段属性事件

        private bool m_isComplete;
        private Action action;
        private Action onStart;
        private Action onComplete;
        

        public bool isComplete
        {
            get { return m_isComplete; }
        }
        public T arg { get; }
        #endregion

        public RunTimeAction(T args, Action action)
        {
            this.action = action;
            this.arg = args;
        }

        public void BeginAction()
        {
            onStart?.Invoke();
            action?.Invoke();
        }

        public void Complete()
        {
            m_isComplete = true;
            onComplete?.Invoke();
        }

        public void OnStart(Action<IRunTime> action)
        {
            onStart += () => action(this);
        }

        public void OnComplete(Action<IRunTime> action)
        {
            onComplete += () => action(this);
        }
    }

}