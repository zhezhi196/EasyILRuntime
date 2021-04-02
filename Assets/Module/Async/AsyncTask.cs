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
    //
    // [AsyncMethodBuilder(typeof(AsyncVoidMethodBuilder))]
    // public class AsyncTask<T>
    // {
    //     #region factory
    //
    //     public static Queue<AsyncTask<T>> taskFactory = new Queue<AsyncTask<T>>();
    //
    //     public static AsyncTask<T> GetTask()
    //     {
    //         if (taskFactory.Count > 0)
    //         {
    //             AsyncTask<T> task = taskFactory.Dequeue();
    //             task.GetAwaiter();
    //             return task;
    //         }
    //         else
    //         {
    //             return new AsyncTask<T>();
    //         }
    //     }
    //
    //     #endregion
    //     
    //     private readonly T result;
    //     public  Awaiter<T> awaiter;
    //     
    //     public Awaiter<T> GetAwaiter()
    //     {
    //         awaiter = Awaiter<T>.GetAwaiter();
    //         return awaiter;
    //     }
    //     
    //     public void SetResult()
    //     {
    //         taskFactory.Enqueue(this);
    //         awaiter.SetResult();
    //     }
    // }
}