using Module;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

[NodeWidth(250)]
[NodeTint(0.3f, 0.1f, 0)]
public class TaskEnd : NodeBase
{
    [Input,HideLabel] public int inPort;
    [HideLabel]
    public TaskPredicate taskPredicate;

    public override bool IsFail(PredicateType predicate, string[] args)
    {
        return false;
    }

    public override bool IsSuccess(PredicateType predicate, string[] args)
    {
        if (predicate == PredicateType.AlwayTrue) return true;
        if (predicate == PredicateType.AlwayFalse) return false;
        for (int i = 0; i < taskPredicate.enterPredicate.Count; i++)
        {
            if (taskPredicate.enterPredicate[i].predicate == predicate && taskPredicate.enterPredicate[i].arg.IsSame(args))
            {
                return true;
            }
        }

        return false;
    }
}