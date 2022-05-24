using UnityEngine;

/// <summary>
/// 玩家基地
/// </summary>
public interface IBasement : IProtection
{
    float toPlayerDistance { get; }
}