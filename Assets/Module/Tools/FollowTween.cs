using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public static class TweenManager
    {
        public static void Update()
        {
            FollowTween.Update();
        }
    }

    public class FollowTween : ISetID<object, FollowTween>
    {
        private static List<FollowTween> tweenList = new List<FollowTween>();

        public static void Update()
        {
            for (int i = 0; i < tweenList.Count; i++)
            {
                if (!tweenList[i].isComplete)
                {
                    tweenList[i].OnUpdate();
                }
                else
                {
                    tweenList.RemoveAt(i);
                }
            }
        }

        public static void Kill(object id)
        {
            for (int i = tweenList.Count - 1; i >= 0; i--)
            {
                if (tweenList[i].ID == id)
                {
                    tweenList[i].Kill();
                    tweenList.RemoveAt(i);
                }
            }
        }

        public static void Pause(object id)
        {
            for (int i = tweenList.Count - 1; i >= 0; i--)
            {
                if (tweenList[i].ID == id)
                {
                    tweenList[i].isPause = true;
                }
            }
        }

        public static void Resume(object id)
        {
            for (int i = tweenList.Count - 1; i >= 0; i--)
            {
                if (tweenList[i].ID == id)
                {
                    tweenList[i].isPause = false;
                }
            }
        }
        public event Action onComplete;
        public event Action onStart;
        public object ID { get; set; }
        public bool isPause;
        public event Action<float> onUpdate;
        public float speed;
        public Transform boss;
        public Transform target;
        public bool isComplete;
        public bool ignoreTimeScale = true;
        private bool isReadyComplete;
        private Vector3 offset;
        private float delayTime;
        private AnimationCurve curve;
        private float currTime;
        public FollowTween(Transform boss, Transform target, float speed, Vector3 offset)
        {
            this.offset = offset;
            this.boss = boss;
            this.target = target;
            this.speed = speed;
            tweenList.Add(this);
        }
        public FollowTween(Transform boss, Transform target,float speed)
        {
            this.boss = boss;
            this.target = target;
            this.speed = speed;
            tweenList.Add(this);
        }
        public FollowTween OnComplete(Action callback)
        {
            onComplete += callback;
            return this;
        }

        public FollowTween OnStart(Action callback)
        {
            onStart += callback;
            return this;
        }

        public FollowTween SetUpdate(bool update)
        {
            this.ignoreTimeScale = update;
            return this;
        }

        public FollowTween SetEasa(AnimationCurve curve)
        {
            this.curve = curve;
            return this;
        }

        public void Complete()
        {
            onComplete?.Invoke();
            onStart = null;
            onComplete = null;
            isComplete = true;
            tweenList.Remove(this);
        }

        public FollowTween SetSpeed(float value)
        {
            speed = value;
            return this;
        }

        public FollowTween SetID(object id)
        {
            this.ID = id;
            return this;
        }

        public void Kill(bool complete = false)
        {
            if (complete)
            {
                boss.position = target.position+offset;
            }
            
            onStart = null;
            onComplete = null;
            isComplete = true;
        }

        private void OnUpdate()
        {
            if(isPause) return;
            
            if (boss == null)
            {
                Complete();
                return;
            }

            if (isReadyComplete)
            {
                boss.position = target.position + offset;
                Complete();
                return;
            }

            Vector3 speed = target.position + offset - boss.position;
            float delatime = (ignoreTimeScale ? TimeHelper.unscaledDeltaTime : TimeHelper.deltaTime);
            currTime += delatime;
            if (curve != null && curve.length >= 2)
            {
                this.speed = curve.Evaluate(currTime);
            }
            delayTime -= delatime;
            if (delayTime <= 0)
            {
                delayTime = 0;
            }
            if (delayTime <= 0)
            {
                boss.position += delatime * this.speed * speed.normalized;
                onUpdate?.Invoke(currTime);
                if (Vector3.Magnitude(speed) <= 4 * delatime * this.speed * delatime * this.speed)
                {
                    isReadyComplete = true;
                }
            }
        }

        public FollowTween SetDelay(float delayFly)
        {
            this.delayTime = delayFly;
            return this;
        }


        public void Dispose()
        {
            if (tweenList.Contains(this))
            {
                tweenList.Remove(this);
            }
        }

        public static void KillAll()
        {
            for (int i = 0; i < tweenList.Count; i++)
            {
                tweenList[i].Kill();
            }
            tweenList.Clear();
        }
    }
}