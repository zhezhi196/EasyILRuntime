using Module;
using Sirenix.OdinInspector;
using UnityEngine;
/// <summary>
/// 监狱门的灯
/// </summary>
public class DoorLockLightProp : OnlyReceiveEventProp
{
    [LabelText("打开灯色")]
    public Color OpenLightColor;
    [LabelText("关闭灯色")]
    public Color CloseLightColor;
    
    [LabelText("灯光")]
    public SpriteRenderer lightQuad;
    [LabelText("绿灯罩")]
    public GameObject lightMaskGreen;
    [LabelText("红灯罩")]
    public GameObject lightMaskRed;

    protected override void OnInitLogical(RunLogicalName logical, RunLogicalFlag flag, string senderArg, string[] args)
    {
        base.OnInitLogical(logical, flag, senderArg, args);
        if (logical == RunLogicalName.On)
        {
            Open(true);
        }
        else if (logical == RunLogicalName.Off)
        {
            Open(false);
        }
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
            Open(false);
        }
    }

    public void Open(bool isOpen)
    {
        lightMaskGreen.OnActive(isOpen);
        lightMaskRed.OnActive(!isOpen);

        lightQuad.color = isOpen ? OpenLightColor : CloseLightColor;

    }
}