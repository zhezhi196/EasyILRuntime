using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Module
{
    [AsyncMethodBuilder(typeof(AsyncVoidMethodBuilder))]
    public class AsyncTask
    {
        #region factory

        public static Queue<AsyncTask> taskFactory = new Queue<AsyncTask>();

        public static AsyncTask GetTask()
        {
            if (taskFactory.Count > 0)
            {
                AsyncTask task = taskFactory.Dequeue();
                task.GetAwaiter();
                return task;
            }
            else
            {
                return new AsyncTask();
            }
        }

        #endregion
        public  Awaiter awaiter;
        
        public Awaiter GetAwaiter()
        {
            awaiter = Awaiter.GetAwaiter();
            return awaiter;
        }
        
        public void SetResult()
        {
            taskFactory.Enqueue(this);
            awaiter.SetResult();
        }
    }
}