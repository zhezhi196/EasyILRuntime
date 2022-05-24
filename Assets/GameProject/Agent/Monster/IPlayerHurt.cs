using Module;

/// <summary>
/// 能被玩家伤害的对象
/// </summary>
public interface IPlayerHurt : IPlayerTarget
{
    bool canBeHurt { get; }
}