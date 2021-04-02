using System;
using System.Collections.Generic;
using UnityEngine;
             
namespace Module
{
    public abstract class Async
    {
        protected AsyncTask task;                
        protected abstract bool isComplete { get; }
        protected object key ;
        public abstract void AsyncUpdate();             
        protected static List<Async> asyncList = new List<Async>();

        static Async()
        {
            for (int i = 0; i < asyncList.Count; i++)
            {
                asyncList[i].Clear();
            }

            asyncList.Clear();
        }

        protected abstract void Clear();


        public static void Update()
        {
            if(asyncList.IsNullOrEmpty()) return;
            for (int i = 0; i < asyncList.Count; i++)
            {
                asyncList[i].AsyncUpdate();
            }
        }

        public static AsyncTask WaitforSeconds(float time, object key = null)
        {
            return WaitForSecondsClass.GetClass(key,time).task;
        }

        public static AsyncTask WaitforSecondsRealTime(float time, object key = null)
        {
            return WaitForSecondsRealtimeClass.GetClass(key,time).task;
        }

        public static AsyncTask WaitUntil(Func<bool> predicate, object key = null)
        {
            return WaitUntilClass.GetClass(key,predicate).task;
        }

        public static AsyncTask WaitForEndOfFrame(object key = null)
        {
            return WaitForEndOfFrameClass.GetClass(key).task;
        }

        public static AsyncTask WaitForWWW(WWW www, object key = null)
        {
            return WaitForWWWClass.GetClass(key,www).task;
        }

        public static AsyncTask WaitForAsyncOption(AsyncOperation option, object key = null)
        {
            return WaitForAsyncOptionClass.GetClass(key,option).task;
        }

        public static AsyncTask WaitProcess(IProcess request, object key = null)
        {
            return AssetProcessClass.GetClass(key, request).task;
        }
        public static void StopAsync(object key)
        {
            for (int i = 0; i < asyncList.Count; i++)
            {
                if (asyncList[i].key == key)
                {
                    asyncList.RemoveAt(i);
                }
            }
        }

        #region 所有类

        private class WaitForSecondsClass : Async
        {
            public static Queue<WaitForSecondsClass> classPool = new Queue<WaitForSecondsClass>();

            public static WaitForSecondsClass GetClass(object key,float time)
            {
                WaitForSecondsClass result = null;
                if (classPool.Count > 0)
                {
                    result = classPool.Dequeue();
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

            public WaitForSecondsClass(object key,float time)
            {
                remainTime = time;
                task = AsyncTask.GetTask();
                asyncList.Add(this);
                this.key = key;
            }

            public override void AsyncUpdate()
            {
                remainTime -= TimeHelper.deltaTimeIgnorePause;
                if (isComplete)
                {
                    task.SetResult();
                    asyncList.Remove(this);
                    classPool.Enqueue(this);
                }
            }

            protected override void Clear()
            {
                classPool.Clear();
            }
        }

        private class WaitForSecondsRealtimeClass : Async
        {
            public static Queue<WaitForSecondsRealtimeClass> classPool = new Queue<WaitForSecondsRealtimeClass>();

            public static WaitForSecondsRealtimeClass GetClass(object key,float time)
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

            public WaitForSecondsRealtimeClass(object key,float time)
            {
                remainTime = time;
                task = AsyncTask.GetTask();
                this.key = key;
                asyncList.Add(this);
            }

            public override void AsyncUpdate()
            {
                remainTime -= TimeHelper.unscaledDeltaTimeIgnorePause;
                if (isComplete)
                {
                    task.SetResult();
                    asyncList.Remove(this);
                    classPool.Enqueue(this);
                }
            }
            protected override void Clear()
            {
                classPool.Clear();
            }
        }

        private class WaitUntilClass : Async
        {
            public static Queue<WaitUntilClass> classPool = new Queue<WaitUntilClass>();

            public static WaitUntilClass GetClass(object key,Func<bool> predicate)
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

            public WaitUntilClass(object key,Func<bool> predicate)
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
            protected override void Clear()
            {
                classPool.Clear();
            }
        }

        private class WaitForEndOfFrameClass : Async
        {
            public static Queue<WaitForEndOfFrameClass> classPool = new Queue<WaitForEndOfFrameClass>();

            public static WaitForEndOfFrameClass GetClass(object key)
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

            public WaitForEndOfFrameClass(object key)
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
            protected override void Clear()
            {
                Debug.LogError(classPool.Count);
                classPool.Clear();
            }
        }

        private class WaitForWWWClass : Async
        {
            public static Queue<WaitForWWWClass> classPool = new Queue<WaitForWWWClass>();

            public static WaitForWWWClass GetClass(object key,WWW www)
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

            public WaitForWWWClass(object key,WWW www)
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
            protected override void Clear()
            {
                classPool.Clear();
            }
        }
        
        private class AssetProcessClass: Async
        {
            public static Queue<AssetProcessClass> classPool = new Queue<AssetProcessClass>();
            protected override bool isComplete
            {
                get { return _request.isComplete; }
            }

            private IProcess _request;
            public AssetProcessClass(object key,IProcess request)
            {
                _request = request;
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

            protected override void Clear()
            {
                classPool.Clear();
            }

            public static AssetProcessClass GetClass(object keys, IProcess request)
            {
                if (classPool.Count > 0)
                {
                    AssetProcessClass result = classPool.Dequeue();
                    result._request = request;
                    result.task = AsyncTask.GetTask();
                    result.key = keys;
                    asyncList.Add(result);
                    return result;
                }
                else
                {
                    return new AssetProcessClass(keys, request);
                }
            }
        }

        private class WaitForAsyncOptionClass : Async
        {
            public static Queue<WaitForAsyncOptionClass> classPool = new Queue<WaitForAsyncOptionClass>();

            public static WaitForAsyncOptionClass GetClass(object key,AsyncOperation option)
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

            public WaitForAsyncOptionClass(object key,AsyncOperation option)
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
            protected override void Clear()
            {
                classPool.Clear();
            }
        }

        #endregion


    }
}