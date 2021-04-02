using System;

namespace Module
{
    public interface IStationAdd<T> where T : Enum
    {
        T station { get; }
        event Action<T> onAddStation; 
        event Action<T> onRemoveStation;
        bool ContainStation(T station);
        void AddStation(T station);
        void RemoveStation(T station);
    }
}