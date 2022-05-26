using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public enum PredicateType
{
    [LabelText("判定条件满足")]
    AlwayTrue,
    [LabelText("判定条件不满足")]
    AlwayFalse,
    [LabelText("交互")]
    Intaractive,
    [LabelText("杀死怪物")]
    KillMonster,
    [LabelText("增加状态")]
    AddStation,
    [LabelText("删除状态")]
    RemoveStation
}
[Serializable]
public class TaskPredicataItem
{
    [VerticalGroup("TaskPredicataItem")][HideLabel]
    public PredicateType predicate;
    [HorizontalGroup("TaskPredicataItem/arg")][ListDrawerSettings(Expanded = true)]
    public List<string> arg;
}
