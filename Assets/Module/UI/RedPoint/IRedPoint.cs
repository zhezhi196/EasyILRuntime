using System;

namespace Module
{
    public interface IRedPoint
    {
        bool redPointIsOn { get; }
        void OnSwitchRedPointStation(bool isOn,bool sendEvent);
        event Action<bool> onSwitchStation;
    }
}