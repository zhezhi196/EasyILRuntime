public enum CommonStation
{
    /// <summary>
    /// 锁定状态,无法选择
    /// </summary>
    Locked = 0,
    /// <summary>
    /// 解锁状态,可以选择,但是要进游戏需要看广告或者支付
    /// </summary>
    Unlocked = 1,
    /// <summary>
    /// 正在进行状态,可以进行游戏
    /// </summary>
    Running = 2,
    /// <summary>
    /// 刚刚完成阶段
    /// </summary>
    NewComplete = 3,
    /// <summary>
    /// 完成后第二次进入
    /// </summary>
    Complete = 4,
}