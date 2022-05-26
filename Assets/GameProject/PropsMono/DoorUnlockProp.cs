using Module;
using UnityEngine;

/// <summary>
/// 未上锁的门
/// </summary>
public class DoorUnlockProp : DoorRotate
{
    public override bool canInteractive => !IsAnimating && base.canInteractive;
    
}