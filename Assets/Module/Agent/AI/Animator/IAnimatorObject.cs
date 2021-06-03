using UnityEngine;

namespace Module
{
    public interface IAnimatorObject : ITimeCtrl, ISwitchStation, IAgentObject
    {
        Animator animator { get; }
        float animatorSpeed { get; }
    }
}