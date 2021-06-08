using System;

namespace Module
{
    public interface IStationObject<T> : ISwitchStation where T : Enum
    {
        T station { get; }
        event Action<T> onAddStation;
        event Action<T> onRemoveStation;
        bool ContainStation(T station);
        bool AddStation(T station);
        bool RemoveStation(T station);
    }
}