using System;
using System.Collections.Generic;

namespace Module.SkillAction
{
    public class TimeAction : ISkillAction
    {
        public Clock timeClock;
        private List<(float, Action<float>)> frameCallback;
        public ISkillObject owner { get; }

        public bool isEnd
        {
            get { return !timeClock.isActive; }
        }

        public float percent
        {
            get { return timeClock.percent; }
        }

        public TimeAction(float time, ISkillObject owner)
        {
            timeClock = new Clock(time);
            this.owner = owner;
            timeClock.owner = owner;
            timeClock.autoKill = false;
        }

        public void OnStart()
        {
            timeClock.Restart();
        }

        public void OnEnd(bool complete)
        {
            if (!complete)
            {
                timeClock.Stop();
            }
            frameCallback = null;
        }

        public void OnPause()
        {
            timeClock.Pause();
        }

        public void OnContinue()
        {
            timeClock.StartTick();
        }

        public void Dispose()
        {
            frameCallback = null;
            timeClock.autoKill = true;
            timeClock.Stop();
        }

        public void OnUpdate()
        {
            if (frameCallback != null)
            {
                for (int i = 0; i < frameCallback.Count; i++)
                {
                    var temp = frameCallback[i];
                    if (timeClock.percent >= temp.Item1)
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