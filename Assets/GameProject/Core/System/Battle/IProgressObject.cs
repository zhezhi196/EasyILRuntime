using Sirenix.OdinInspector;
using UnityEngine;

public interface IProgressObject
{
    Vector3 progressPos { get; }
}

public enum CompleteProgressPredicate
{
    [LabelText("交互")]
    Interactive,
    [LabelText("放入背包")]
    PutToBag,
    [LabelText("查看")]
    Watch,
    [LabelText("杀死怪物")]
    KillMonster,
    [LabelText("添加状态")]
    AddStation,
    [LabelText("删除状态")]
    RemoveStation,
}