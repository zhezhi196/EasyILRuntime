using System;
using System.Collections;

namespace Module
{
    public interface IProcess : IEnumerator
    {
        Func<bool> monitor { get; set; }
        bool isComplete { get; }
        void SetMonitor(Func<bool> monitor);
    }
}