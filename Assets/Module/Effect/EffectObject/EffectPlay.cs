using System;
using System.Collections;
using UnityEngine;

namespace Module
{
    public class EffectPlay
    {
        private Action onStart;
        private Action onComplete;
        public bool ignoreTimeScale { get; private set; } = false;
        public EffectBase effect { get; private set; }
        public int remainLoops { get; private set; } = 1;
        public bool isActive
        {
            get { return effect != null; }
        }

        public void Start(EffectBase effect)
        {
            this.effect = effect;
            effect.StartCoroutine(StartPlay(effect));
        }

        private IEnumerator StartPlay(EffectBase effect)
        {
            onStart?.Invoke();
            yield return new WaitForEndOfFrame();
            while (remainLoops > 0)
            {
                effect.Restart();
                EffectAsync async = new EffectAsync(this);
                yield return async;
                async.Reset();
                remainLoops--;
            }
            
            onComplete?.Invoke();
            effect.pool.ReturnObject(effect);
            onStart = null;
            onComplete = null;
        }

        public EffectPlay OnStart(Action action)
        {
            onStart += action;
            return this;
        }

        public EffectPlay OnComplete(Action action)
        {
            onComplete += action;
            return this;
        }

        public EffectPlay SetUpdate(bool ignoreTimeScale)
        {
            this.ignoreTimeScale = ignoreTimeScale;
            return this;
        }

        public EffectPlay SetLoops(int loop)
        {
            remainLoops = loop;
            return this;
        }
    }

    public class EffectAsync : IEnumerator
    {
        private EffectPlay play;
        private float time;
        public object Current { get; }
        public EffectAsync(EffectPlay play)
        {
            this.play = play;
        }
        public bool MoveNext()
        {
            time += play.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            play.effect.Simulate(time);
            return !play.effect.isComplete;
        }

        public void Reset()
        {
            time = 0;
        }


    }
}