using DG.Tweening;
using Module;
using UnityEngine;
/// <summary>
/// 闸门开关
/// </summary>
public class SwitchGateProp : OnlyInteractive
{
    public GameObject redLight;
    public GameObject greenLight;
    
    public override bool canInteractive => base.canInteractive && !IsAnimating;
    public override InterActiveStyle interactiveStyle => InterActiveStyle.Handle;

    public Transform handle;
    protected override bool OnInteractive(bool fromMonster = false)
    {
        if (fromMonster)
        {
            return false;
        }
        //可以重复打开和关闭
        if (ContainStation(PropsStation.Off))
        {
            RunLogicalOnSelf(RunLogicalName.On);
        }
        else
        {
            RunLogicalOnSelf(RunLogicalName.Off);
        }
        return true;
    }

    protected override void OnRunPerformance(RunLogicalName logical, RunLogicalFlag flag, IEventCallback sender, string senderArg,
        string[] args)
    {
        base.OnRunPerformance(logical, flag, sender, senderArg, args);

        if (logical == RunLogicalName.On)
        {
            Open(true);
        }
        else if (logical == RunLogicalName.Off)
        {
            Close(true);
        }
    }

    protected override void OnInitLogical(RunLogicalName logical, RunLogicalFlag flag, string senderArg, string[] args)
    {
        base.OnInitLogical(logical, flag, senderArg, args);
        if (logical == RunLogicalName.On)
        {
            Open(false);
        }
        else if (logical == RunLogicalName.Off)
        {
            Close(false);
        }
    }

    private void Open(bool anim = true)
    {
        if (handle == null)
        {
            return;
        }
        if (anim)
        {
            IsAnimating = true;
            handle.transform.localRotation = Quaternion.Euler(-75,0,0);
            handle.DOLocalRotateQuaternion(Quaternion.Euler(50,0,0), 1).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                greenLight.OnActive(true);
                redLight.OnActive(false);
                IsAnimating = false;    
            });
        }
        else
        {
            greenLight.OnActive(true);
            redLight.OnActive(false);
            handle.transform.localRotation = Quaternion.Euler(50,0,0);
        }
    }

    private void Close(bool anim = true)
    {
        if (handle == null)
        {
            return;
        }
        if (anim)
        {
            IsAnimating = true;
            handle.transform.localRotation = Quaternion.Euler(50,0,0);
            handle.DOLocalRotateQuaternion(Quaternion.Euler(-75, 0, 0), 1).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                greenLight.OnActive(false);
                redLight.OnActive(true);
                IsAnimating = false;
            });
        }
        else
        {
            greenLight.OnActive(false);
            redLight.OnActive(true);
            handle.transform.localRotation = Quaternion.Euler(-75,0,0);
        }
    }
    

}