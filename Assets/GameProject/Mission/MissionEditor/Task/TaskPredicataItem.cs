using System.Collections.Generic;
using Sirenix.OdinInspector;

public class TaskPredicataItem
{
    [VerticalGroup("TaskPredicataItem")] [HideLabel]
    public PredicateType predicate;

    [HorizontalGroup("TaskPredicataItem/arg")] [ListDrawerSettings(Expanded = true)]
    public List<string> arg;
}