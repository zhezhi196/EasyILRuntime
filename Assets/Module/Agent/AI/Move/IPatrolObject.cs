using UnityEngine;

namespace Module
{
    public interface IPatrolObject : IMoveObject
    {
        Vector3 GetPatrolPoint(ref int index);
    }
}