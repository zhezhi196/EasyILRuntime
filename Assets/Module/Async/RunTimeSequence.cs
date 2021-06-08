using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class RunTimeSequence : IProcess
    {
        public static int totalCount;

        #region 字段,属性

        public int ID;
        private bool m_isComplete;
        private int totalChild;
        private IRunTime m_run;
        private Dictionary<string, object> args;
        private Queue<IRunTime> m_queue=new Queue<IRunTime>();
        public bool autoDestroy = true;
        public event Action onComplete;
        public event Action onStart;
        public event Action<IRunTime> onNextAction;
    
        public IRunTime run
        {
            get { return m_run; }
        }

        public Func<bool> listener { get; set; }

        public bool isComplete
        {
            get
            {
                bool temp = m_queue.Count == 0 && m_isComplete;
                if (listener == null)
                {
                    return temp;
                }
                else
                {
                    return temp || listener();
                }
            }
        }

        public void SetListener(Func<bool> listen)
        {
            this.listener = listen;
        }

        public int count
        {
            get { return m_queue.Count; }
        }
    

        #endregion

        public RunTimeSequence()
        {
            totalCount++;
            ID = totalCount;
        }
        public void Add(IRunTime runtime)
        {
            m_queue.Enqueue(runtime);
            totalChild++;
            runtime.index = totalChild;
        }

        public void AddSequence(RunTimeSequence sequence)
        {
            for (int i = 0; i < sequence.count; i++)
            {
                Add(sequence.m_queue.Dequeue());
            }
        }

        public void AddRange(List<IRunTime> action)
        {
            if (action == null) return;
            for (int i = 0; i < action.Count; i++)
            {
                Add(action[i]);
            }
        }
        
        public void NextAction()
        {
            if (m_queue.Count == totalChild&& !m_isComplete)
            {
                onStart?.Invoke();
            }

            if (m_run != null)
            {
                m_run.Complete();
            }

            if (!m_isComplete)
            {
                if (m_queue.Count > 0)
                {
                    m_run = m_queue.Dequeue();
                    m_run.BeginAction();
                    onNextAction?.Invoke(m_run);
                }
                else if (!m_isComplete)
                {
                    onComplete?.Invoke();
                    m_isComplete = true;
                    if (autoDestroy)
                    {
                        Clear();
                    }
                }
            }
        }

        public void Clear()
        {
            m_queue.Clear();
            m_run = null;
            onComplete = null;
            onStart = null;
            onNextAction = null;
            
            totalChild = 0;
            m_isComplete = false;
        }

        public void OnComplete(Action callback)
        {
            onComplete += callback;
        }

        public void OnStart(Action callback)
        {
            onStart += callback;
        }

        public void OnNextAction(Action<IRunTime> callBack)
        {
            onNextAction += callBack;
        }

        public bool MoveNext()
        {
            return !isComplete;
        }

        public void Reset()
        {
            Clear();
        }

        public object Current
        {
            get { return m_run; }
        }
    }

}