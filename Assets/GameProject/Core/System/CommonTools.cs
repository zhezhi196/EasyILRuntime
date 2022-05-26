using System.Collections.Generic;
using Module;

public static class CommonTools
{
    public static bool CommonLogical(RunLogicalName logical, params string[] args)
    {
        if (logical == RunLogicalName.LoadScene && !args.IsNullOrEmpty())
        {
            BattleController.GetCtrl<SceneCtrl>().LoadScenes(args, null);
            return true;
        }
        else if (logical == RunLogicalName.UnloadScene && !args.IsNullOrEmpty())
        {
            BattleController.GetCtrl<SceneCtrl>().UnLoadScenes(args, null);
            return true;
        }
        else if (logical == RunLogicalName.Bgm && args.Length > 0 && !args[0].IsNullOrEmpty())
        {
            AudioManager.PlayMusic(args[0]);
            // AudioPlay.PlayBackGroundMusic(args[0]);
            return true;
        }
        else if (logical == RunLogicalName.flyFont && args.Length == 1)
        {
            EventCenter.Dispatch<string>(EventKey.piao, args[0]);
            return true;
        }
        else if (logical == RunLogicalName.TimeLine && args.Length == 1)
        {
            string id = args[0];
            Player.player.PlayStoryTimeline(id, () => { });
            return true;
        }
        else if (logical == RunLogicalName.TeachEvent && args.Length == 1)
        {
            EventCenter.Dispatch<string>(EventKey.TeachEvent, args[0]);
            return true;
        }
        else if (logical == RunLogicalName.SwitchFog && args.Length >= 1)
        {
            float time = args.Length == 1 ? 2 : args[1].ToFloat();
            BattleController.GetCtrl<SceneCtrl>().SwitchFog(BattleController.Instance.ctrlProcedure.currentNode, args[0].ToInt(), time);
        }

        return false;
    }
    public static EventSender[] CreatSender(List<EventSenderEditor> model, IEventSender sender)
    {
        if (model == null) return null;
        List<EventSender> temp = new List<EventSender>();
        for (int i = 0; i < model.Count; i++)
        {
            EventSender item = new EventSender();
            item.predicate = model[i].eventConditionEditor.sendEventCondition;
            item.predicateArgs = model[i].eventConditionEditor.args;
            item.eventKey = new EventID[model[i].sendEventID.Count];
            for (int j = 0; j < item.eventKey.Length; j++)
            {
                item.eventKey[j].eventKey = model[i].sendEventID[j].sendEventID;
                item.eventKey[j].eventArgs = model[i].sendEventID[j].eventArgs;
            }

            item.InitSender(sender);
            temp.Add(item);
        }

        return temp.ToArray();
    }

    public static EventReceiver[] CreatReceiver(List<EventReciverEditor> model, IEventReceiver receiver)
    {
        if (model == null) return null;
        List<EventReceiver> temp = new List<EventReceiver>();
        for (int i = 0; i < model.Count; i++)
        {
            EventReceiver responce = new EventReceiver();
            responce.eventKey = model[i].eventID;
            responce.receiveCount = model[i].count;
            responce.logical = new List<EventReceiverArgs>();
            for (int j = 0; j < model[i].responseModels.Count; j++)
            {
                responce.logical.Add(new EventReceiverArgs());
                responce.logical[j].save = model[i].responseModels[j].save;
                responce.logical[j].logical = model[i].responseModels[j].responseID;
                responce.logical[j].args = model[i].responseModels[j].args;
            }
            responce.InitReceiver(receiver);
            temp.Add(responce);
        }

        return temp.ToArray();
    }
}