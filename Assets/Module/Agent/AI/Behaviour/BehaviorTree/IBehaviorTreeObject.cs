using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Module
{
    public interface IBehaviorTreeObject : IAgentObject, ITarget
    {
        BehaviorTree behaviourTree { get; }
        AgentBehaviorTree GetBehaviorTree(string name);
    }
}