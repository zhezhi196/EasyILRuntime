using System;
using System.Collections;

namespace Module
{
    public interface IProcess : IEnumerator
    {
        Func<bool> listener { get; set; }
        bool isComplete { get; }
        void SetListener(Func<bool> monitor);
    }
}