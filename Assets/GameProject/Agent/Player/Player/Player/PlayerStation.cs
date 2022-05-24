using System;

[Flags]
public enum PlayerStation
{
    /// <summary>
    /// 变身
    /// </summary>
    Transform = 1 << 0,
    /// <summary>
    /// 攻击
    /// </summary>
    Attack = 1 << 1,
    /// <summary>
    /// 怒吼
    /// </summary>
    Roar = 1 << 2,
    /// <summary>
    /// 跳跃
    /// </summary>
    Jump = 1 << 3,
    /// <summary>
    /// 狂暴
    /// </summary>
    Rampage = 1 << 4,
    /// <summary>
    /// 受伤
    /// </summary>
    BeHurt = 1 << 5,
    /// <summary>
    /// 霸气
    /// </summary>
    Despot = 1 << 6
}
[Flags]
public enum SonStation
{
    Follow = 1,
    Attack = 2,
}

public enum BaseStation
{
    
}
