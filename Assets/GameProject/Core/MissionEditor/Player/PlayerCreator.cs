using System;
using System.Collections.Generic;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
public class PlayerCreator : SerializedMonoBehaviour, IEventReceiver, IEventSender
{
    private EventReceiver[] _receiver;
    private EventSender[] _sender;
    [LabelText("事件发送")] public List<EventSenderEditor> eventSender;
    [LabelText("事件接受")] public List<EventReciverEditor> eventRecivers;
    [LabelText("闪现点"), InlineButton("AddFlashPoint", "+闪现点"), ListDrawerSettings(HideAddButton = true,CustomRemoveIndexFunction = "OnRemoveFlashPoint")]
    public List<Transform> flashPoints;

    public int key
    {
        get { return 0; }
    }

    public EventReceiver[] receiver => _receiver;
    public EventSender[] sender => _sender;
    
    private void Awake()
    {
        _sender = CommonTools.CreatSender(eventSender, this);
        _receiver = CommonTools.CreatReceiver(eventRecivers, this);
    }

    #region Editor
    
#if UNITY_EDITOR

    private void OnRemoveFlashPoint(int index)
    {
        var ttt = flashPoints[index];
        flashPoints.RemoveAt(index);
        if (!ttt.gameObject.IsNullOrDestroyed())
        {
            DestroyImmediate(ttt.gameObject);
        }
    }
    private void AddFlashPoint()
    {
        Transform tar = transform.NewChild("flashPoints " + transform.childCount);
        flashPoints.Add(tar);
        UnityEditor.Selection.activeGameObject = tar.gameObject;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f);
            
        if (!flashPoints.IsNullOrEmpty())
        {
            for (int i = 0; i < flashPoints.Count; i++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawCube(flashPoints[i].position, Vector3.one * 0.4f);
            }
        }
    }

#endif

    #endregion

    public void EventCallback(int eventID, IEventCallback receiver)
    {
    }
    
    public void TrySendEvent(SendEventCondition predicate, params string[] arg)
    {
        if (!sender.IsNullOrEmpty())
        {
            for (int i = 0; i < sender.Length; i++)
            {
                if (sender[i].predicate == predicate)
                {
                    sender[i].TrySendEvent(arg);
                }
            }
        }
    }

    public void RunLogical(RunLogicalName logical, IEventCallback sender, RunLogicalFlag flag, string senderArg, params string[] args)
    {
        if (!CommonTools.CommonLogical(logical, args))
        {
            Player.player.RunLogical(logical, sender, flag, args);
        }
    }
    
    public bool TryPredicate(SendEventCondition predicate, string[] sendArg, string[] predicateArgs)
    {
        switch (predicate)
        {
            case SendEventCondition.Dead:
                return true;
            case SendEventCondition.AddStation:
            case SendEventCondition.RemoveStation:
                for (int i = 0; i < sendArg.Length; i++)
                {
                    sendArg[i] = sendArg[i].ToLower();
                }

                for (int i = 0; i < predicateArgs.Length; i++)
                {
                    predicateArgs[i] = predicateArgs[i].ToLower();
                }
                return sendArg.IsSame(predicateArgs);
        }

        return false;
    }
}