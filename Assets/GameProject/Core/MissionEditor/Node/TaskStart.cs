using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

[NodeWidth(100)][NodeTint(0.3f, 0.5f, 0)][DisallowMultipleNodes()]
public class TaskStart : NodeBase
{
    [Output,HideLabel]
    public int outPort;

    public override bool IsFail(PredicateType predicate, string[] args)
    {
        return false;
    }

    public override bool IsSuccess(PredicateType predicate, string[] args)
    {
        return false;
    }
}