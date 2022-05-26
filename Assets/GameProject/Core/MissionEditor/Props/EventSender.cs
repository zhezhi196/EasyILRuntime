using System;
using System.Collections.Generic;
using Module;
using Sirenix.OdinInspector;

[Serializable]
public class EventSenderEditor
{
    [HideLabel] public EventConditionEditor eventConditionEditor;

    [LabelText("发送事件ID"),ListDrawerSettings(Expanded = true)] public List<EventSenderArg> sendEventID;
}

[Serializable]
public class EventConditionEditor
{
    [HorizontalGroup, HideLabel] public SendEventCondition sendEventCondition;
    [HorizontalGroup, LabelText("条件参数")] public string[] args;
    
}

[Serializable]
public class EventSenderArg
{
    [HorizontalGroup, HideLabel] public int sendEventID;
    [HorizontalGroup, HideLabel] public string eventArgs;
}

[Serializable]
public class EventReciverEditor
{
    [HideLabel,HorizontalGroup]
    public int eventID;
    [LabelText("事件接受次数"),HorizontalGroup]
    public int count = 1;
    [LabelText("响应"),ListDrawerSettings(Expanded = true)]
    public List<ReceiveLogical> responseModels;

}

[Serializable]
public class ReceiveLogical
{
    [HideLabel,HorizontalGroup()]
    public bool save = true;
    [HideLabel, HorizontalGroup()] public RunLogicalName responseID;
    [LabelText("参数"), HorizontalGroup] public string[] args;
}