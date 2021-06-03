using System;

namespace Module
{
    public interface IRedPoint
    {
        bool redPointIsOn { get; }
        event Action<bool> onSwitchRedPointStation;
    }
}