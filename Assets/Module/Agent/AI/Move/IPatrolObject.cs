using System;
using UnityEngine;

namespace Module
{
    public interface IPatrolObject : IMoveObject
    {
        Vector3 GetPatrolPoint(ref int index);
        void PatrolTo(Vector3 position,Action callback);
    }
}