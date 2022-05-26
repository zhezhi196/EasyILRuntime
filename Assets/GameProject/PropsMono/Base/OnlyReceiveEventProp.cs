
/// <summary>
/// 只接受事件的prop，不能交互
/// </summary>
public class OnlyReceiveEventProp : PropsBase
{
    /// <summary>
    /// 不能交互
    /// </summary>
    public override bool canInteractive => false;
    
}