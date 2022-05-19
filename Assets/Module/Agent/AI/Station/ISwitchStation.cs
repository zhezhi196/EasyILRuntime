using System;

namespace Module
{
    public interface ISwitchStation<T> : ISwitchEventObject
    {
        bool SwitchStation(T station);
    }
}