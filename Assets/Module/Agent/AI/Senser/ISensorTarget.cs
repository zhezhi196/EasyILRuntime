using System;
using UnityEngine;

namespace Module
{
    public interface ISensorTarget
    {
        Transform transform { get; }

        bool isSenserable { get; }

        event Action<ISensorTarget> onDisable;
    }
}