using System;
using System.Collections.Generic;

namespace Module.SkillAction
{
    public class AnimationAction : ISkillAction
    {
        public int layer { get; }
        public string name { get; }
        public ISkillObject owner { get; }
        public AnimationPlay play;
        private List<(float, Action<float>)> frameCallback;

        public float speed = 1;

        public AnimationAction(string name,int layer, ISkillObject owner)
        {
            this.owner = owner;
            this.name = name;
            this.layer = layer;
        }

        public bool isEnd
        {
            get { return play.isComplete; }
        }

        public float percent
        {
            get { return play.percent; }
        }

        public void OnStart()
        {
            play = owner.GetAgentCtrl<AnimatorCtrl>().Play(name, layer, 0).SetDuationSpeed(speed);
        }

        public void OnEnd(bool complete)
        {
            if (!complete)
            {
                owner.GetAgentCtrl<AnimatorCtrl>().BreakAnimation(layer, true);
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
        }

        public void OnUpdate()
        {
            if (frameCallback != null)
            {
                for (int i = 0; i < frameCallback.Count; i++)
                {
                    var temp = frameCallback[i];
                    if (play.percent >= temp.Item1)
                    {
                        temp.Item2?.Invoke(temp.Item1);
                        frameCallback.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void SetTimeCallback(float percent, Action<float> callback)
        {
            if (frameCallback == null) frameCallback = new List<(float, Action<float>)>();
            frameCallback.Add((percent, callback));
        }
    }
}