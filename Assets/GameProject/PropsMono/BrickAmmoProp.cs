using UnityEngine;

/// <summary>
/// 砖块（二代水杯，投掷物）
/// </summary>
public class BrickAmmoProp : BulletBase
{
    protected override bool OnInteractive(bool fromMonster = false)
    {
        entity.GetReward(count, creator.id, creator.matchInfo, 0);
        if (base.OnInteractive(fromMonster))
        {
            RunLogicalOnSelf(RunLogicalName.Destroy);
            return true;
        }
        return true;
    }
}