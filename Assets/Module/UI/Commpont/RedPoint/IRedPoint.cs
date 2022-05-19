using System;

namespace Module
{
    public interface IRedPoint: ISwitchEventObject
    {
        bool redPointIsOn { get; }
    }
}