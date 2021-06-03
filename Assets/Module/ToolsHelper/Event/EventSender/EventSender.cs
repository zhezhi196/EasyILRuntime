using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    public struct EventID
    {
        public uint eventKey;
        public string eventArgs;
        public override string ToString()
        {
            return eventKey.ToString();
        }
    }
    [Serializable, HideReferenceObjectPicker]
    public class EventSender<T> where T : Enum
    {
        private IEventSender<T> sender;
        [HorizontalGroup("条件"), HideLabel] public T predicate;
        [HorizontalGroup("条件"), LabelText("参数"),HideReferenceObjectPicker] public string[] predicateArgs;
        [HorizontalGroup("事件"), LabelText("事件ID"),HideReferenceObjectPicker] public EventID[] eventKey;

        public void InitSender(IEventSender<T> sender)
        {
            this.sender = sender;
        }

        public void TrySendEvent(string[] arg)
        {
            if (eventKey.IsNullOrEmpty() || sender == null) return;
            if (sender.TryPredicate(predicate, arg, predicateArgs))
            {
                Send();
            }
        }

        private void Send()
        {
            for (int i = 0; i < eventKey.Length; i++)
            {
                EventCenter.Dispatch<uint, string, IEventCallback>(ConstKey.EventKey, eventKey[i].eventKey, eventKey[i].eventArgs, sender);
                GameDebug.LogFormat("发送事件ID: {0}" , eventKey[i]);
            }
        }
    }
}