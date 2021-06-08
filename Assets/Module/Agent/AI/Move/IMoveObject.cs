using System;
using UnityEngine;
using UnityEngine.AI;

namespace Module
{
    public interface IMoveObject : ITimeCtrl, ISwitchStation, IAgentObject
    {
        Vector3 moveTarget { get; }
        Transform transform { get; }
        NavMeshAgent navmesh { get; }
        float moveSpeed { get; }
        Matrix4x4 bornPoint { get; }
        void WalkTo(int type, Vector3 position, Action callback);
    }
}