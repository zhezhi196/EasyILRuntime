using System;

namespace Module
{
    public interface IRunTime
    {
        bool isComplete { get; }
        int index { get; set; }

        void BeginAction();
        void Complete();
    }

    public class RunTimeAction : IRunTime
    {
        #region 字段属性事件

        private bool m_isComplete;
        private Action action;

        public bool isComplete
        {
            get { return m_isComplete; }
        }

        public int index { get; set; }

        #endregion
    
        public RunTimeAction(Action action)
        {
            this.action = action;
        }

        public void BeginAction()
        {
            action?.Invoke();
        }

        public void Complete()
        {
            m_isComplete = true;
        }
    }
    
    public class RunTimeAction<T> : IRunTime
    {
        #region 字段属性事件

        private bool m_isComplete;
        private Action action;

        public bool isComplete
        {
            get { return m_isComplete; }
        }

        public int index { get; set; }
        public T arg { get; }
        #endregion

        public RunTimeAction(T args, Action action)
        {
            this.action = action;
            this.arg = args;
        }

        public void BeginAction()
        {
            action?.Invoke();
        }

        public void Complete()
        {
            m_isComplete = true;
        }
    }
}