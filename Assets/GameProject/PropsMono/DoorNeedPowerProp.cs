using Module;
using UnityEngine;

/// <summary>
/// 需要电箱解锁的门
/// </summary>
public class DoorNeedPowerProp : DoorRotate
{
    public override bool canInteractive => ContainStation(PropsStation.Locked);

    public override InterActiveStyle interactiveStyle => InterActiveStyle.Watch;
    
    protected override bool OnInteractive(bool fromMonster = false)
    {
        return false;
    }
}