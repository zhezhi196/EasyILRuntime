using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module.SkillAction
{
    public class StationAction : ISkillAction
    {
        public int layer { get; }
        public string name { get; }
        public ISkillObject owner { get; }
        public StateMachinePlay play;
        private List<(float, Action<float>)> frameCallback;
        public event Action<string,AnimationEvent,int> onEvent;

        public StationAction(string name, int layer, ISkillObject owner)
        {
            this.owner = owner;
            this.name = name;
            this.layer = layer;
        }

        public bool isEnd
        {
            get { return !play.isPlaying; }
        }

        public float percent
        {
            get { return play.playTime; }
        }

        public void OnStart()
        {
            play = owner.GetAgentCtrl<StateMachineCtrl>().Play(name);
            play.onEvent += (a, b) =>
            {
                onEvent?.Invoke(name, a, b);
            };
        }

        public void OnEnd(bool complete) 
        {
            if (!complete)
            {
                owner.GetAgentCtrl<StateMachineCtrl>().EndPlay(play, false);
            }
            frameCallback = null;
        }

        public void OnPause()
        {
        }

        public void OnContinue()
        {
        }

        public void Dispose()
        {
            play = null;
        }

        public void OnUpdate()
        {
            if (frameCallback != null)
            {
                for (int i = 0; i < frameCallback.Count; i++)
                {
                    var temp = frameCallback[i];
                    if (play.playTime >= temp.Item1)
                    {
                        temp.Item2?.Invoke(temp.Item1);
                        frameCallback.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void SetTimeCallback(float time, Action<float> callback)
        {
            if (frameCallback == null) frameCallback = new List<(float, Action<float>)>();
            frameCallback.Add((time, callback));
        }
    }
}