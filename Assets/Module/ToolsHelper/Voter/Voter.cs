using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class Voter : ISetID<object, Voter>
    {
        public static Dictionary<object, List<Voter>> voterDic = new Dictionary<object, List<Voter>>();
        public object ID { get; set; }
        public int count { get; set; }
        
        public bool isActive { get; set; } = true;

        public Voter(int count)
        {
            this.maxCount = count;
        }

        public Voter(int count, Action callback)
        {
            this.maxCount = count;
            OnComplete(callback);
        }
        public int maxCount { get; }

        public event Action onComplete;

        public Voter Add()
        {
            if (isActive)
            {
                count++;
                if (count == maxCount)
                {
                    onComplete?.Invoke();
                }
                else if (count>maxCount)
                {
                    GameDebug.LogError("voter 越界");
                }
            }

            return this;
        }

        public Voter OnComplete(Action callback)
        {
            if (maxCount == 0)
            {
                callback?.Invoke();
                return this;
            }
            onComplete = callback;
            return this;
        }

        public Voter SetID(object ID)
        {
            List<Voter> voter = null;
            if (!voterDic.TryGetValue(ID, out voter))
            {
                voter = new List<Voter>();
                voterDic.Add(ID,voter);
            }
            
            voter.Add(this);
            return this;
        }
    }
    public class Voter<T> : Voter
    {
        public List<T> coutList { get; set; }
        public Voter(int count) : base(count)
        {
            coutList = new List<T>();
        }
        public void Add(T tag)
        {
            if (isActive)
            {
                if (!coutList.Contains(tag))
                {
                    Add();
                    coutList.Add(tag);
                }
            }
        }
    }
}
