using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    [Flags]
    public enum RunLogicalFlag
    {
        [LabelText("动画")] WithPerformance = 1,
        [LabelText("不存档")] Save = 2
    }
    public interface IEventReceiver : IEventCallback
    {
        int key { get; }
        EventReceiver[] receiver { get; }
        void RunLogical(string logical, IEventCallback sender, RunLogicalFlag flag,string senderArg, params string[] args);
    }
}

