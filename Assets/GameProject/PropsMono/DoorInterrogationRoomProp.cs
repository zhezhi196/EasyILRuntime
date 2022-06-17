using Module;
using UnityEngine;

/// <summary>
/// 审讯室门（提示专用）
/// </summary>
public class DoorInterrogationRoomProp : DoorRotate
{
    public override bool canInteractive => !IsAnimating && base.canInteractive;

    public override bool progressIsComplete
    {
        get
        {
            return creator.isGet || PropEntity.GetEntity(20012).isGet;
        }
    }
}