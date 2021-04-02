using UnityEngine.AI;

namespace Module
{
    public interface INavmeshMoveAgent: IMoveAgent
    {
        NavMeshAgent navmeshAgent { get; }
    }
}