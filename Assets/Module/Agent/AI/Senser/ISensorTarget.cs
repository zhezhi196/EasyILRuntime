using System;
using UnityEngine;

namespace Module
{
    public interface ISensorTarget : ITarget
    {
        bool isSenserable { get; }
        event Action<ISensorTarget> onDisable;
    }
}