using System;
using UnityEngine;

namespace Module
{
    public interface ISensorTarget : ISwitchStation
    {
        Transform transform { get; }

        bool isSenserable { get; }

        event Action<ISensorTarget> onDisable;
    }
}