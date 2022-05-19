using System;
using UnityEngine;

namespace Module
{
    public interface IPatrolObject : INavMoveObject
    {
        Vector3 GetPatrolPoint(ref int index);
    }
}