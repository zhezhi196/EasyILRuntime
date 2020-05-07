using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    
    public class RunTimeSequence
    {
        public static Dictionary<string, RunTimeSequence> allRunTime = new Dictionary<string, RunTimeSequence>();

        #region 字段,属性

        private string m_ID;
        private IRunTime m_run;
        private Dictionary<string, object> args;
        private Queue<IRunTime> m_queue=new Queue<IRunTime>();
        private Action m_onComplete;
        private Action m_onStart;
        private Action<IRunTime> onNextAction;

        public string ID
        {
            get { return m_ID; }
        }
    
        public IRunTime run
        {
            get { return m_run; }
        }

        public bool isDone
        {
            get { return m_queue.Count == 0; }
        }

        public int count
        {
            get { return m_queue.Count; }
        }
    

        #endregion

        public void Add(IRunTime runtime)
        {
            m_queue.Enqueue(runtime);
        }
    
        public void BeginAction()
        {
            m_onStart?.Invoke();
            if (m_queue.Count == 0)
            {
                m_onComplete?.Invoke();
                return;
            }
        
            m_run= m_queue.Dequeue();
            m_run.BeginAction();
        }
        
        public void NextAction()
        {
            if (m_run != null)
            {
                m_run.Complete();
                onNextAction?.Invoke(m_run);
            }

            if (m_queue.Count == 0)
            {
                m_onComplete?.Invoke();
                return;
            }

            m_run = m_queue.Dequeue();
            m_run.BeginAction();
        }

        public void Clear()
        {
            m_queue.Clear();
        }

        public void OnComplete(Action callback)
        {
            m_onComplete = callback;
        }

        public void OnStart(Action callback)
        {
            m_onStart = callback;
        }

        public void OnNextAction(Action<IRunTime> callBack)
        {
            onNextAction = callBack;
        }
    }

}