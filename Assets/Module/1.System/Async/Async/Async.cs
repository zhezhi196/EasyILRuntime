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
        protected string key;
        protected abstract bool isComplete { get; }
        public abstract void AsyncUpdate();
        protected abstract void OnClear();
        
        protected static List<Async> asyncList = new List<Async>();
        public static void Update()
        {
            for (int i = 0; i < asyncList.Count; i++)
            {
                asyncList[i].AsyncUpdate();
            }
        }
        
        public static void StopAsync(string key)
        {
            for (int i = 0; i < asyncList.Count; i++)
            {
                if (asyncList[i].key == key)
                {
                    asyncList.RemoveAt(i);
                }
            }
        }
        
        public static void Clear()
        {
            for (int i = 0; i < asyncList.Count; i++)
            {
                asyncList[i].OnClear();
            }
            asyncList.Clear();
        }
        
        public static AsyncTask WaitforSeconds(float time, string key = null)
        {
            return WaitForSecondsClass.GetClass(key,time).task;
        }

        public static AsyncTask WaitforSecondsRealTime(float time, string key = null)
        {
            return WaitForSecondsRealtimeClass.GetClass(key,time).task;
        }

        public static AsyncTask WaitUntil(Func<bool> predicate, string key = null)
        {
            return WaitUntilClass.GetClass(key,predicate).task;
        }

        public static AsyncTask WaitForEndOfFrame(string key = null)
        {
            return WaitForEndOfFrameClass.GetClass(key).task;
        }

        public static AsyncTask WaitForWWW(WWW www, string key = null)
        {
            return WaitForWWWClass.GetClass(key,www).task;
        }

        public static AsyncTask WaitForAsyncOption(AsyncOperation option, string key = null)
        {
            return WaitForAsyncOptionClass.GetClass(key,option).task;
        }

        #region 所有类

        private class WaitForSecondsClass : Async
        {
            public static Queue<WaitForSecondsClass> classPool = new Queue<WaitForSecondsClass>();

            public static WaitForSecondsClass GetClass(string key,float time)
            {
                if (classPool.Count > 0)
                {
                    WaitForSecondsClass result = classPool.Dequeue();
                    result.remainTime = time;
                    result.task = AsyncTask.GetTask();
                    asyncList.Add(result);
                    result.key = key;
                    return result;
                }
                else
                {
                    return new WaitForSecondsClass(key,time);
                }
            }

            public float remainTime;

            protected override bool isComplete
            {
                get { return remainTime <= 0; }
            }

            public WaitForSecondsClass(string key,float time)
            {
                remainTime = time;
                task = AsyncTask.GetTask();
                asyncList.Add(this);
                this.key = key;
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

            protected override void OnClear()
            {
                classPool.Clear();
            }
        }

        private class WaitForSecondsRealtimeClass : Async
        {
            public static Queue<WaitForSecondsRealtimeClass> classPool = new Queue<WaitForSecondsRealtimeClass>();

            public static WaitForSecondsRealtimeClass GetClass(string key,float time)
            {
                if (classPool.Count > 0)
                {
                    WaitForSecondsRealtimeClass result = classPool.Dequeue();
                    result.remainTime = time;
                    result.task = AsyncTask.GetTask();
                    result.key = key;
                    asyncList.Add(result);
                    return result;
                }
                else
                {
                    return new WaitForSecondsRealtimeClass(key,time);
                }
            }

            protected override bool isComplete
            {
                get { return remainTime <= 0; }
            }

            private float remainTime;

            public WaitForSecondsRealtimeClass(string key,float time)
            {
                remainTime = time;
                task = AsyncTask.GetTask();
                this.key = key;
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

            protected override void OnClear()
            {
                classPool.Clear();
            }
        }

        private class WaitUntilClass : Async
        {
            public static Queue<WaitUntilClass> classPool = new Queue<WaitUntilClass>();

            public static WaitUntilClass GetClass(string key,Func<bool> predicate)
            {
                if (classPool.Count > 0)
                {
                    WaitUntilClass result = classPool.Dequeue();
                    result.predicate = predicate;
                    result.task = AsyncTask.GetTask();
                    asyncList.Add(result);
                    result.key = key;
                    return result;
                }
                else
                {
                    return new WaitUntilClass(key,predicate);
                }
            }

            protected override bool isComplete
            {
                get { return predicate.Invoke(); }
            }

            private Func<bool> predicate;

            public WaitUntilClass(string key,Func<bool> predicate)
            {
                this.predicate = predicate;
                task = AsyncTask.GetTask();
                this.key = key;
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
            protected override void OnClear()
            {
                classPool.Clear();
            }
        }

        private class WaitForEndOfFrameClass : Async
        {
            public static Queue<WaitForEndOfFrameClass> classPool = new Queue<WaitForEndOfFrameClass>();

            public static WaitForEndOfFrameClass GetClass(string key)
            {
                if (classPool.Count > 0)
                {
                    WaitForEndOfFrameClass result = classPool.Dequeue();
                    result.m_isComplete = false;
                    result.task = AsyncTask.GetTask();
                    result.key = key;
                    asyncList.Add(result);
                    return result;
                }
                else
                {
                    return new WaitForEndOfFrameClass(key);
                }
            }

            private bool m_isComplete;

            protected override bool isComplete
            {
                get { return m_isComplete; }
            }

            public WaitForEndOfFrameClass(string key)
            {
                task = AsyncTask.GetTask();
                this.key = key;
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
            protected override void OnClear()
            {
                Debug.LogError(classPool.Count);
                classPool.Clear();
            }
        }

        private class WaitForWWWClass : Async
        {
            public static Queue<WaitForWWWClass> classPool = new Queue<WaitForWWWClass>();

            public static WaitForWWWClass GetClass(string key,WWW www)
            {
                if (classPool.Count > 0)
                {
                    WaitForWWWClass result = classPool.Dequeue();
                    result.www = www;
                    result.task = AsyncTask.GetTask();
                    result.key = key;
                    asyncList.Add(result);
                    return result;
                }
                else
                {
                    return new WaitForWWWClass(key,www);
                }
            }

            protected override bool isComplete
            {
                get { return www.isDone; }
            }

            private WWW www;

            public WaitForWWWClass(string key,WWW www)
            {
                this.www = www;
                task = AsyncTask.GetTask();
                this.key = key;
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
            protected override void OnClear()
            {
                classPool.Clear();
            }
        }


        private class WaitForAsyncOptionClass : Async
        {
            public static Queue<WaitForAsyncOptionClass> classPool = new Queue<WaitForAsyncOptionClass>();

            public static WaitForAsyncOptionClass GetClass(string key,AsyncOperation option)
            {
                if (classPool.Count > 0)
                {
                    WaitForAsyncOptionClass result = classPool.Dequeue();
                    result.option = option;
                    result.task = AsyncTask.GetTask();
                    result.key = key;
                    asyncList.Add(result);
                    return result;
                }
                else
                {
                    return new WaitForAsyncOptionClass(key,option);
                }
            }

            protected override bool isComplete
            {
                get { return option.isDone; }
            }

            private AsyncOperation option;

            public WaitForAsyncOptionClass(string key,AsyncOperation option)
            {
                this.option = option;
                task = AsyncTask.GetTask();
                this.key = key;
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
            protected override void OnClear()
            {
                classPool.Clear();
            }
        }

        #endregion



    }
}