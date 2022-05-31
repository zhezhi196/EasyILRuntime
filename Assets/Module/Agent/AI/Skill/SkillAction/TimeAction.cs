using System;
using System.Collections.Generic;

namespace Module.SkillAction
{
    public class TimeAction : ISkillAction
    {
        public bool isActive;
        public float targetTime;
        public float currentTime;
        private List<(float, Action<float>)> frameCallback;

        public float remainTime
        {
            get
            {
                return targetTime - currentTime;
            }
        }
        public ISkillObject owner { get; }

        public bool isEnd
        {
            get { return currentTime >= targetTime; }
        }

        public float percent
        {
            get { return currentTime / targetTime; }
        }

        public TimeAction(float time, ISkillObject owner)
        {
            this.targetTime = time;
            this.owner = owner;
        }

        public void OnStart()
        {
            currentTime = 0;
            isActive = true;
        }

        public void OnEnd(bool complete)
        {
            isActive = false;
            frameCallback = null;
        }

        public void OnPause()
        {
            isActive = false;
        }

        public void OnContinue()
        {
            isActive = true;
        }

        public void Dispose()
        {
            frameCallback = null;
            isActive = false;
            currentTime = 0;
        }

        public void OnUpdate()
        {
            if (isActive)
            {
                currentTime += owner.GetDelatime(false);
            }
            if (frameCallback != null)
            {
                for (int i = 0; i < frameCallback.Count; i++)
                {
                    var temp = frameCallback[i];
                    if (percent >= temp.Item1)
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