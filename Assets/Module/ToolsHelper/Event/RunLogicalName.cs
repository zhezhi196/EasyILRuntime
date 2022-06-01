using Sirenix.OdinInspector;

public enum RunLogicalName
{
    [LabelText("什么都不做")] None,
    
    [LabelText("重置到出生")] Reset,
    [LabelText("显示")] Show,
    [LabelText("隐藏")] Hide,
    [LabelText("卸载")] Destroy,
    
    [LabelText("打开")] On,
    [LabelText("关闭")] Off,
    [LabelText("上锁")] Lock,
    [LabelText("移除上锁")] RemoveLock,
    [LabelText("旋转匹配")] RotateMatch,
    
    [LabelText("切换行为树(仅怪)")] SwitchBt,
    [LabelText("死亡")] Dead,
    [LabelText("传送(仅玩家)")] Flash,
    
    [LabelText("UI")] UI,
    [LabelText("背景音乐")] Bgm,
    [LabelText("加载场景")] LoadScene,
    [LabelText("卸载场景")] UnloadScene,
    TimeLine,
    [LabelText("强制销毁")] ForceDestroy,
    [LabelText("非激活")] UnActive,
    [LabelText("飘字")] flyFont,
    [LabelText("激活")] RemoveUnActive,
    
    [LabelText("教学")]  TeachEvent,
    [LabelText("停止攻击")]   StopAttack,
    [LabelText("切换雾效")] SwitchFog,
    
    [LabelText("强制上锁")]ForceLock,
    [LabelText("解除强制上锁")]RemoveForceLock,

}

public enum SendEventCondition
{
    [LabelText("无条件执行")] None,
    [LabelText("交互")] Interactive,
    [LabelText("添加状态")] AddStation,
    [LabelText("移除状态")] RemoveStation,
    [LabelText("死亡")] Dead,
    [LabelText("玩家出生")]
    PlayerBorn,
}

// public class EventContent
// {
//     public const string flyFont = "flyFont";
//     public const string None = "None";
//     public const string Reset = "Reset";
//     public const string Show = "Show";
//     public const string Hide = "Hide";
//     public const string Destroy = "Destroy";
//     public const string On = "On";
//     public const string Off = "Off";
//     public const string Lock = "Lock";
//     public const string UnActive = "UnActive";
//     public const string ForceDestroy = "ForceDestroy";
//     public const string RemoveLock = "RemoveLock";
//     public const string RotateMatch = "RotateMatch";
//     
//     public const string Flash = "Flash";
//     public const string SwitchBt = "SwitchBt";
//
//     
//     public const string Interactive = "Interactive";
//     public const string AddStation = "AddStation";
//     public const string RemoveStation = "RemoveStation";
//     public const string Dead = "Dead";
//     
//     public const string Bgm = "bgm";
//     public const string LoadScene = "LoadScene";
//     public const string UnloadScene = "UnloadScene";
//     public const string TimeLine = "TimeLine";
//
//     public const string RemoveUnActive = "RemoveUnActive";
//     public const string TeachEvent = "TeachEvent";
// }