using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

public abstract class NodeBase : Node
{
    [HideInInspector]
    public List<NodeBase> nextNode;

    public abstract bool IsFail(PredicateType predicate, string[] args);

    public abstract bool IsSuccess(PredicateType predicate, string[] args);
}
