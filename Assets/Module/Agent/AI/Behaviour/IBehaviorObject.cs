using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Module
{
    public interface IBehaviorObject : ITimeCtrl, ISwitchStation, IAgentObject
    {
        GameObject gameObject { get; }
        bool isPauseBehavior { get; }
        BehaviorTree behaviourTree { get; }
        AgentBehaviorTree bornBehavior { get; }
        AgentBehaviorTree GetBehaviorTree(string name);

    }
}