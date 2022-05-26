using Sirenix.OdinInspector;

public enum BehaviorType
{
    None,
    Idle,
    [LabelText("巡逻")]
    Parallel,
    [LabelText("追捕")]
    Chase,
}