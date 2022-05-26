using System;
using Sirenix.OdinInspector;

[Serializable]
public struct BornStation
{
    [LabelText("出生状态"),LabelWidth(80)]
    public MonsterStation bornStation;

    [LabelText("出生行为树"), LabelWidth(80)] 
    public BehaviorType behavior;
    [LabelText("出生行为树参数"),LabelWidth(80)]
    public string[] Args;
}