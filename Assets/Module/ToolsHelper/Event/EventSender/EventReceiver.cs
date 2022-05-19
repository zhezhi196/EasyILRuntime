using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    [Serializable,HideReferenceObjectPicker]
    public class EventReceiverArgs
    {
        public bool save;
        [HorizontalGroup,HideLabel]
        public string logical;
        [HorizontalGroup,LabelText("逻辑参数")]
        public string[] args;
    }
    
    [Serializable,HideReferenceObjectPicker]
    public class EventReceiver
    {
        private IEventReceiver receiver;
        [LabelText("事件ID")]
        public int eventKey;
        [LabelText("接受次数")]
        public int receiveCount;
        [LabelText("执行逻辑")]
        public List<EventReceiverArgs> logical;

        public int currReceiveCount;
        public bool isActive;

        public void InitReceiver(IEventReceiver eventReceiver)
        {
            this.receiver = eventReceiver;
            ResetValue();
        }

        private void OnGetEvent(string eventArgs, IEventCallback sender)
        {
            if (receiveCount != -1)
            {
                currReceiveCount++;
            }

            for (int i = 0; i < logical.Count; i++)
            {
                EventReceiverArgs log = logical[i];
                GameDebug.LogFormat("{0}接受到{1}事件,执行{2}逻辑", receiver.key, eventKey, log.logical);
                if (!log.save)
                {
                    receiver.RunLogical(log.logical, sender, RunLogicalFlag.WithPerformance, eventArgs, log.args);
                }
                else
                {
                    receiver.RunLogical(log.logical, sender, RunLogicalFlag.WithPerformance | RunLogicalFlag.Save, eventArgs, log.args);
                }
                sender?.EventCallback(eventKey, receiver);
            }

            if (receiveCount != -1 && currReceiveCount >= receiveCount)
            {
                Dispose();
            }
        }

        public void ResetValue()
        {
            currReceiveCount = 0;
            EventTools.ReceiveEvent(eventKey, OnGetEvent);
        }

        public void Dispose()
        {
            EventTools.UnReceiveEvent(eventKey, OnGetEvent);
        }

    }
}