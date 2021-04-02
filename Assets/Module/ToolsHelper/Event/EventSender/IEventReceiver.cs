using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public interface IEventReceiver<T> : IEventCallback where T : Enum
    {
        int key { get; }
        EventReceiver<T>[] receiver { get; }
        void RunLogical(T logical, IEventCallback sender, bool withPerformance, bool record, params string[] args);
    }
}

