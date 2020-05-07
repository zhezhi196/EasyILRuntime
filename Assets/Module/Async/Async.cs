using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Module
{
    public abstract class Async
    {
        protected AsyncTask task;
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
            return new WaitForSecondsClass(time).task;
        }

        public static AsyncTask WaitforSecondsRealTime(float time)
        {
            return new WaitForSecondsRealtimeClass(time).task;
        }

        public static AsyncTask  WaitUntil (Func<bool> predicate)
        {
            return new WaitUntilClass(predicate).task;
        }

        public static AsyncTask WaitForEndOfFrame()
        {
            return new WaitForEndOfFrameClass().task;
        }
        public static AsyncTask WaitForComplete(WWW www)
        {
            return new WaitForWWWClass(www).task;
        }

        public static AsyncTask WaitForUnityWebRequest(UnityWebRequest www)
        {
            return new WaitForUnityWebRequestClass(www).task;
        }
        
        private class WaitForSecondsClass : Async
        {
            public float remainTime;

            protected override bool isComplete
            {
                get { return remainTime <= 0; }
            }

            public WaitForSecondsClass(float time)
            {
                remainTime = time;
                task = new AsyncTask();
                asyncList.Add(this);
            }
        
            public override void AsyncUpdate()
            {
                remainTime -= Time.deltaTime;
                if (isComplete)
                {
                    task.SetResult();
                    asyncList.Remove(this);
                }
            }
        }
        
        private class WaitForSecondsRealtimeClass : Async
        {
            protected override bool isComplete
            {
                get { return remainTime <= 0; }
            }

            private float remainTime;

            public WaitForSecondsRealtimeClass(float time)
            {
                remainTime = time;
                task = new AsyncTask();
                asyncList.Add(this);
            }
            public override void AsyncUpdate()
            {
                remainTime -= Time.unscaledDeltaTime;
                if (isComplete)
                {
                    task.SetResult();
                    asyncList.Remove(this);
                }
            }
        }
        
        private class WaitUntilClass: Async
        {
            protected override bool isComplete
            {
                get { return predicate.Invoke(); }
            }

            private Func<bool> predicate;

            public WaitUntilClass(Func<bool> predicate)
            {
                this.predicate = predicate;
                task = new AsyncTask();
                asyncList.Add(this);
            }
            public override void AsyncUpdate()
            {
                if (isComplete)
                {
                    task.SetResult();
                    asyncList.Remove(this);
                }
            }
        }
        
        private class WaitForEndOfFrameClass: Async
        {
            private bool m_isComplete;

            protected override bool isComplete
            {
                get { return m_isComplete; }
            }

            public WaitForEndOfFrameClass()
            {
                task = new AsyncTask();
                asyncList.Add(this);
            }

            public override void AsyncUpdate()
            {
                if (isComplete)
                {
                    task.SetResult();
                    asyncList.Remove(this);
                }
                else
                {
                    m_isComplete = true;
                }
                
            }
        }
        
        private class WaitForWWWClass: Async
        {
            public WWW www;
            protected override bool isComplete
            {
                get { return www.isDone; }
            }

            public WaitForWWWClass(WWW www)
            {
                this.www = www;
                task = new AsyncTask();
                asyncList.Add(this);
            }

            public override void AsyncUpdate()
            {
                if (isComplete)
                {
                    task.SetResult();
                    asyncList.Remove(this);
                }
            }
        }
        
        private class WaitForUnityWebRequestClass: Async
        {
            public UnityWebRequest www;
            protected override bool isComplete
            {
                get { return www.isDone; }
            }

            public WaitForUnityWebRequestClass(UnityWebRequest www)
            {
                this.www = www;
                task = new AsyncTask();
                asyncList.Add(this);
            }

            public override void AsyncUpdate()
            {
                if (isComplete)
                {
                    task.SetResult();
                    asyncList.Remove(this);
                }
            }
        }
    }
  
}