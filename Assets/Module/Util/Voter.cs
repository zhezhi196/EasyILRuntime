using System;
using System.Collections;
using UnityEngine;

namespace Module
{
    public class Voter
    {
        public int count { get; set; }
        public int maxCount { get; }
        public event Action onComplete;
        public bool isComplete { get; set; }

        public Voter(int count)
        {
            maxCount = count;
        }

        public void Add()
        {
            count++;

            if (count >= maxCount)
            {
                onComplete?.Invoke();
            }
        }

        public void OnComplete(Action callback)
        {
            onComplete = callback;
        }
    }


}
