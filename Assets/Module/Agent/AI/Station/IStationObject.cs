using System;

namespace Module
{
    public interface IStationObject<T> : ISwitchStation<T> where T : Enum
    {
        T station { get; }
        bool ContainStation(T station);
        bool AddStation(T station);
        bool RemoveStation(T station);
    }
}