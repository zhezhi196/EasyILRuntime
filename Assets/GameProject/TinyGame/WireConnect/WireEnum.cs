using Sirenix.OdinInspector;

public enum WireMode
{
    [LabelText("3x3")]x3,
    [LabelText("4x4")]x4,
}

public enum WireSlotType
{
    [LabelText("无")] None,
    [LabelText("电池")] Battery,
    [LabelText("出口")] Export,
    [LabelText("导线")] Wire,
    [LabelText("旋钮")] Knob
}

public enum WireKnobType
{
    [LabelText("无")] None = 0,
    [LabelText("四面连通")] All = 1,
    [LabelText("双拐弯")] BothTurn = 2,
    [LabelText("T型")] TModel = 3,
    [LabelText("单拐弯")] Turn = 4,
    [LabelText("直线")] Line = 5,
}

public enum WireLineType
{
    [LabelText("竖线")] Vertical = 0,
    [LabelText("横线")] Horizontal = 1,
}

public enum WirePortType
{
    [LabelText("旋钮电线")] InKnob = 0,
    [LabelText("普通电线")] Wire = 1,
    [LabelText("电池电线")] Battery = 2,
}