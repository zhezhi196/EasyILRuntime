using DG.Tweening;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
/// <summary>
/// 密码箱
/// </summary>
public class PasswordBoxProp : OnlyInteractiveOpenUI
{
    public override bool canInteractive => ContainStation(PropsStation.Locked) && base.canInteractive;

    public Transform door;
    
    [LabelText("密码")]
    public string password;
    public void InputSuccess()
    {
        RunLogicalOnSelf(RunLogicalName.RemoveLock);
        creator.isGet = true;
    }

    protected override void OnInitLogical(RunLogicalName logical, RunLogicalFlag flag, string senderArg, string[] args)
    {
        base.OnInitLogical(logical, flag, senderArg, args);
        if (logical == RunLogicalName.On || logical == RunLogicalName.RemoveLock)
        {
            RemoveStation(PropsStation.Locked);
            door.transform.localRotation = Quaternion.Euler(0, -100, 0);
        }
    }

    protected override void OnRunPerformance(RunLogicalName logical, RunLogicalFlag flag, IEventCallback sender, string senderArg,
        string[] args)
    {
        base.OnRunPerformance(logical, flag, sender, senderArg, args);
        if (logical == RunLogicalName.RemoveLock)
        {
            Open();
        }
    }

    public void Open()
    {
        door.DOLocalRotate(new Vector3(0, -100, 0), 1);
    }
}