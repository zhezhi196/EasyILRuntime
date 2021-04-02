using System;

namespace Module
{
    public interface IStationSwitch<T>
    {
        T station { get; set; }
        event Action<T> onSwitchStation;
        void SwitchStation(T station);
    }
}