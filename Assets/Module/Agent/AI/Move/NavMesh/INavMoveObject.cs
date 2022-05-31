using System;
using UnityEngine;
using UnityEngine.AI;

namespace Module
{
    public interface INavMoveObject : IMoveObject
    {
        NavMeshAgent navmesh { get; }
        Vector3 faceDirection { get; }
        Vector3 terrainNormal { get; }
    }
}