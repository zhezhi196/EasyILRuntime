/// <summary>
/// 怪物对玩家造成伤害
/// </summary>
public class PlayerDamage
{
    public IMonster monster;
    /// <summary>
    /// 造成的伤害
    /// </summary>
    public float damage;
    /// <summary>
    /// 攻击类型
    /// </summary>
    public int attackType = 0;
    /// <summary>
    /// 是否播放处决玩家动画
    /// </summary>
    public bool excuteAnim = false;
    /// <summary>
    /// 是否挣脱动画
    /// </summary>
    public bool outAnim = false;

    public PlayerDamage(IMonster m)
    {
        monster = m;
    }
}
