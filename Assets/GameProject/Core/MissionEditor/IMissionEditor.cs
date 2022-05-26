using Module;

public enum SaveStation
{
    None = 0,
    Unload = 1,
    Load = 2,
    Init = 3,
    UnActive = 4,
}

/// <summary>
/// 关卡中的实体编辑器接口，继承这个接口表示是场景中的某个实体（怪物，道具）的Creator
/// </summary>
public interface IMissionEditor: ILocalSave,IProgressOption,IEventReceiver, IEventSender,IMapInfo
{
    /// <summary>
    /// 当场景加载过来,关卡编辑预制加载上来的时候,会调用一下这个
    /// </summary>
    /// <param name="type"></param>
    void OnEnterBattle(EnterNodeType type);
}