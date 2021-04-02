using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    [Serializable,HideReferenceObjectPicker]
    public class EventReceiverArgs<T> where T : Enum
    {
        [HorizontalGroup,HideLabel]
        public bool record = true;
        [HorizontalGroup,HideLabel]
        public T logical;
        [HorizontalGroup,LabelText("逻辑参数")]
        public string[] args;
    }
    
    [Serializable,HideReferenceObjectPicker]
    public class EventReceiver<T> where T : Enum
    {
        private IEventReceiver<T> receiver;
        [LabelText("事件ID")]
        public uint eventKey;
        [LabelText("接受次数")]
        public int receiveCount;
        [LabelText("执行逻辑")]
        public EventReceiverArgs<T>[] logical;

        public void InitReceiver(IEventReceiver<T> eventReceiver)
        {
            this.receiver = eventReceiver;
            EventCenter.Register<uint, string, IEventCallback>(ConstKey.EventKey, OnGetEvent);
        }

        private void OnGetEvent(uint eventKey, string eventArgs, IEventCallback sender)
        {
            if (eventKey == this.eventKey)
            {
                if (receiveCount != -1)
                {
                    receiveCount--;
                }

                for (int i = 0; i < logical.Length; i++)
                {
                    EventReceiverArgs<T> log = logical[i];
                    GameDebug.LogFormat("{0}接受到{1}事件,执行{2}逻辑", receiver.key, eventKey, log.logical);
                    receiver.RunLogical(log.logical, sender, true, log.record, log.args);
                    sender?.EventCallback(eventKey, receiver);
                }

                if (receiveCount != -1 && receiveCount <= 0)
                {
                    Dispose();
                }
            }
        }

        public void Dispose()
        {
            EventCenter.UnRegister<uint, string, IEventCallback>(ConstKey.EventKey, OnGetEvent);
        }
    }
}