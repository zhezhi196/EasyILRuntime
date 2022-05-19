using UnityEngine;

namespace Module
{
    public interface IAnimaotr : IAgentObject, ITarget
    {
        AnimationCallback animationCallback { get; }
        bool canReceiveEvent { get; }
        Animator animator { get; }
    }
}