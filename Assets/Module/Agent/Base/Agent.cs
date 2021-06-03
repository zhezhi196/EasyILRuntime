using System;
using UnityEngine;

namespace Module
{
    public abstract class Agent<T> : MonoBehaviour, IStationObject<T> where T : Enum
    {
        public abstract T station { get; }

        public abstract event Action onSwitchStation;
        public abstract event Action<T> onAddStation;
        public abstract event Action<T> onRemoveStation;
        public abstract bool ContainStation(T station);
        public abstract void AddStation(T station);
        public abstract void RemoveStation(T station);
    }
}