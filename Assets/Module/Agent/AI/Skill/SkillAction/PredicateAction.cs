using System;
using System.Collections.Generic;

namespace Module.SkillAction
{
    public class PredicateAction: ISkillAction
    {
        private Func<bool> predicate;
        private bool isStart;
        private bool isPause;
        private float currTime;
        private List<(float, Action<float>)> frameCallback;


        public PredicateAction(ISkillObject owner,Func<bool> predicate)
        {
            this.owner = owner;
            this.predicate = predicate;
        }

        public ISkillObject owner { get; }


        public bool isEnd
        {
            get { return predicate.Invoke(); }
        }

        public float percent
        {
            get { return currTime; }
        }

        public void OnStart()
        {
            isStart = true;
            currTime = 0;
        }

        public void OnEnd(bool complete)
        {
            isStart = false;
            frameCallback = null;
            currTime = 0;
        }

        public void OnPause()
        {
            isPause = true;
        }

        public void OnContinue()
        {
            isPause = false;
        }

        public void Dispose()
        {
            isStart = false;
            isPause = false;
            predicate = null;
            frameCallback = null;
        }

        public void OnUpdate()
        {
            if (isStart && !isPause)
            {
                currTime += owner.GetDelatime(false);
                if (frameCallback != null)
                {
                    for (int i = 0; i < frameCallback.Count; i++)
                    {
                        var temp = frameCallback[i];
                        if (currTime >= temp.Item1)
                        {
                            temp.Item2?.Invoke(temp.Item1);
                            frameCallback.RemoveAt(i);
                            break;
                        }
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