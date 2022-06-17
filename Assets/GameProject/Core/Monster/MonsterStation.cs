using System;

[Flags]
public enum MonsterStation
{
    //发现玩家的惊动
    Alarm = 1,
    /// <summary>
    /// 正在攻击动作
    /// </summary>
    Attack = 1 << 2,
    /// <summary>
    /// 等待
    /// </summary>
    Wait = 1 << 3,
    /// <summary>
    /// 眩晕,被控制
    /// </summary>
    Paralysis = 1 << 4,
    /// <summary>
    /// 正在播放timeline
    /// </summary>
    TimeLine = 1 << 5,
    /// <summary>
    /// 正在怒吼
    /// </summary>
    Roar = 1 << 6,
    /// <summary>
    /// 查看
    /// </summary>
    CheckLook = 1 << 7,
    /// <summary>
    /// 被攻击硬直
    /// </summary>
    HitParalysis = 1 << 8,

    /// <summary>
    /// 逃跑怪进入逃跑的的状态
    /// </summary>
    Escape = 1 << 9,

    /// <summary>
    /// 巡逻
    /// </summary>
    Paraller = 1 << 10,

    ResetToBorn = 1 << 11,

    /// <summary>
    /// 醒来
    /// </summary>
    WakeUp = 1 << 12,

    /// <summary>
    /// 是否看到玩家躲藏
    /// </summary>
    IsSeePlayHide = 1 << 13,
    /// <summary>
    /// 闪避,这个没用了,下次给删了
    /// </summary>
    Blocking = 1 << 14,
    Jump = 1 << 15,
    Sleep = 1 << 16
}