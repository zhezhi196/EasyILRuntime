using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Module
{
    public abstract class Async
    {
        protected AsyncTask task;
        protected bool isBusy;
        protected abstract bool isComplete { get; }
        public abstract void AsyncUpdate();
        protected static List<Async> asyncList = new List<Async>();

        public static void Update()
        {
            for (int i = 0; i < asyncList.Count; i++)
            {
                asyncList[i].AsyncUpdate();
            }
        }

        public static AsyncTask WaitforSeconds(float time)
        {
            return WaitForSecondsClass.GetClass(time).task;
        }

        public static AsyncTask WaitforSecondsRealTime(float time)
        {
            return WaitForSecondsRealtimeClass.GetClass(time).task;
        }

        public static AsyncTask WaitUntil(Func<bool> predicate)
        {
            return WaitUntilClass.GetClass(predicate).task;
        }

        public static AsyncTask WaitForEndOfFrame()
        {
            return WaitForEndOfFrameClass.GetClass().task;
        }

        public static AsyncTask WaitForWWW(WWW www)
        {
            return WaitForWWWClass.GetClass(www).task;
        }

        public static AsyncTask WaitForUnityWebRequest(UnityWebRequest request)
        {
            return WaitForUnityWebRequestClass.GetClass(request).task;
        }

        public static AsyncTask WaitForAsyncOption(AsyncOperation option)
        {
            return WaitForAsyncOptionClass.GetClass(option).task;
        }

        #region 所有类

        private class WaitForSecondsClass : Async
        {
            public static Queue<WaitForSecondsClass> classPool = new Queue<WaitForSecondsClass>();

            public static WaitForSecondsClass GetClass(float time)
            {
                if (classPool.Count > 0)
                {
                    
                    WaitForSecondsClass result = classPool.Dequeue();
                    result.remainTime = time;
                    result.task = AsyncTask.GetTask();
                    asyncList.Add(result);
                    return result;
                }
                else
                {
                    return new WaitForSecondsClass(time);
                }
            }

            public float remainTime;

            protected override bool isComplete
            {
                get { return remainTime <= 0; }
            }

            public WaitForSecondsClass(float time)
            {
                remainTime = time;
                task = AsyncTask.GetTask();
                asyncList.Add(this);
            }

            public override void AsyncUpdate()
            {
                remainTime -= Time.deltaTime;
                if (isComplete)
                {
                    task.SetResult();
                    asyncList.Remove(this);
                    classPool.Enqueue(this);
                }
            }
        }

        private class WaitForSecondsRealtimeClass : Async
        {
            public static Queue<WaitForSecondsRealtimeClass> classPool = new Queue<WaitForSecondsRealtimeClass>();

            public static WaitForSecondsRealtimeClass GetClass(float time)
            {
                if (classPool.Count > 0)
                {
                    WaitForSecondsRealtimeClass result = classPool.Dequeue();
                    result.remainTime = time;
                    result.task = AsyncTask.GetTask();
                    asyncList.Add(result);
                    return result;
                }
                else
                {
                    return new WaitForSecondsRealtimeClass(time);
                }
            }

            protected override bool isComplete
            {
                get { return remainTime <= 0; }
            }

            private float remainTime;

            public WaitForSecondsRealtimeClass(float time)
            {
                remainTime = time;
                task = AsyncTask.GetTask();
                asyncList.Add(this);
            }

            public override void AsyncUpdate()
            {
                remainTime -= Time.unscaledDeltaTime;
                if (isComplete)
                {
                    task.SetResult();
                    asyncList.Remove(this);
                    classPool.Enqueue(this);
                }
            }
        }

        private class WaitUntilClass : Async
        {
            public static Queue<WaitUntilClass> classPool = new Queue<WaitUntilClass>();

            public static WaitUntilClass GetClass(Func<bool> predicate)
            {
                if (classPool.Count > 0)
                {
                    WaitUntilClass result = classPool.Dequeue();
                    result.predicate = predicate;
                    result.task = AsyncTask.GetTask();
                    asyncList.Add(result);
                    return result;
                }
                else
                {
                    return new WaitUntilClass(predicate);
                }
            }

            protected override bool isComplete
            {
                get { return predicate.Invoke(); }
            }

            private Func<bool> predicate;

            public WaitUntilClass(Func<bool> predicate)
            {
                this.predicate = predicate;
                task = AsyncTask.GetTask();
                asyncList.Add(this);
            }

            public override void AsyncUpdate()
            {
                if (isComplete)
                {
                    task.SetResult();
                    asyncList.Remove(this);
                    classPool.Enqueue(this);
                }
            }
        }

        private class WaitForEndOfFrameClass : Async
        {
            public static Queue<WaitForEndOfFrameClass> classPool = new Queue<WaitForEndOfFrameClass>();

            public static WaitForEndOfFrameClass GetClass()
            {
                if (classPool.Count > 0)
                {
                    WaitForEndOfFrameClass result = classPool.Dequeue();
                    result.m_isComplete = false;
                    result.task = AsyncTask.GetTask();
                    asyncList.Add(result);
                    return result;
                }
                else
                {
                    return new WaitForEndOfFrameClass();
                }
            }

            private bool m_isComplete;

            protected override bool isComplete
            {
                get { return m_isComplete; }
            }

            public WaitForEndOfFrameClass()
            {
                task = AsyncTask.GetTask();
                asyncList.Add(this);
            }

            public override void AsyncUpdate()
            {
                if (isComplete)
                {
                    task.SetResult();
                    asyncList.Remove(this);
                    classPool.Enqueue(this);
                }
                else
                {
                    m_isComplete = true;
                }
            }
        }

        private class WaitForWWWClass : Async
        {
            public static Queue<WaitForWWWClass> classPool = new Queue<WaitForWWWClass>();

            public static WaitForWWWClass GetClass(WWW www)
            {
                if (classPool.Count > 0)
                {
                    WaitForWWWClass result = classPool.Dequeue();
                    result.www = www;
                    result.task = AsyncTask.GetTask();
                    asyncList.Add(result);
                    return result;
                }
                else
                {
                    return new WaitForWWWClass(www);
                }
            }

            protected override bool isComplete
            {
                get { return www.isDone; }
            }

            private WWW www;

            public WaitForWWWClass(WWW www)
            {
                this.www = www;
                task = AsyncTask.GetTask();
                asyncList.Add(this);
            }

            public override void AsyncUpdate()
            {
                if (isComplete)
                {
                    task.SetResult();
                    asyncList.Remove(this);
                    classPool.Enqueue(this);
                }
            }
        }

        private class WaitForUnityWebRequestClass : Async
        {
            public static Queue<WaitForUnityWebRequestClass> classPool = new Queue<WaitForUnityWebRequestClass>();

            public static WaitForUnityWebRequestClass GetClass(UnityWebRequest www)
            {
                if (classPool.Count > 0)
                {
                    WaitForUnityWebRequestClass result = classPool.Dequeue();
                    result.www = www;
                    result.task = AsyncTask.GetTask();
                    asyncList.Add(result);
                    return result;
                }
                else
                {
                    return new WaitForUnityWebRequestClass(www);
                }
            }

            protected override bool isComplete
            {
                get { return www.isDone; }
            }

            private UnityWebRequest www;

            public WaitForUnityWebRequestClass(UnityWebRequest www)
            {
                this.www = www;
                task = AsyncTask.GetTask();
                asyncList.Add(this);
            }

            public override void AsyncUpdate()
            {
                if (isComplete)
                {
                    task.SetResult();
                    asyncList.Remove(this);
                    classPool.Enqueue(this);
                }
            }
        }

        private class WaitForAsyncOptionClass : Async
        {
            public static Queue<WaitForAsyncOptionClass> classPool = new Queue<WaitForAsyncOptionClass>();

            public static WaitForAsyncOptionClass GetClass(AsyncOperation option)
            {
                if (classPool.Count > 0)
                {
                    WaitForAsyncOptionClass result = classPool.Dequeue();
                    result.option = option;
                    result.task = AsyncTask.GetTask();
                    asyncList.Add(result);
                    return result;
                }
                else
                {
                    return new WaitForAsyncOptionClass(option);
                }
            }

            protected override bool isComplete
            {
                get { return option.isDone; }
            }

            private AsyncOperation option;

            public WaitForAsyncOptionClass(AsyncOperation option)
            {
                this.option = option;
                task = AsyncTask.GetTask();
                asyncList.Add(this);
            }

            public override void AsyncUpdate()
            {
                if (isComplete)
                {
                    task.SetResult();
                    asyncList.Remove(this);
                    classPool.Enqueue(this);
                }
            }
        }

        #endregion
    }
}