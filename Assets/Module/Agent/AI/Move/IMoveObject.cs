using UnityEngine;
using UnityEngine.AI;

namespace Module
{
    public interface IMoveObject : ITimeCtrl, ISwitchStation, IAgentObject
    {
        Transform transform { get; }
        NavMeshAgent navmesh { get; }
        float moveSpeed { get; }
        Matrix4x4 bornPoint { get; }
    }
}