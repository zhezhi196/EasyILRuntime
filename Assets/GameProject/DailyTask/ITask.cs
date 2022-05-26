using System;
using Module;
using UnityEngine;

public interface ITask : IRedPoint, IRewardObject
{
    bool isComplete { get; }
    TaskStation station { get; set; }
    event Action<TaskStation> onStationChanged;
    bool IsAchieve(params object[] obj);
    void AddCompleteCount(int add);
    void Complete();
}