using System;

namespace Module
{
    public interface ISwitchEventObject
    {
        event Action onSwitchStation;
    }
}