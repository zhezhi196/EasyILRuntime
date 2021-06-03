using System;
using DG.Tweening;
using UnityEngine;

namespace Module
{
    public class EffectBase : MonoBehaviour, IPoolObject, ITimeCtrl
    {
        public EffectPlay play;
        public string name { get; set; }
        public ObjectPool pool { get; set; }
        public virtual void StartFirst(){}
        public virtual void Restart(){}
        public virtual void Pause(){}
        public virtual void Stop(){}
        public virtual void Simulate(float time){}
        public virtual void OnGetObjectFromPool(){}
        public virtual void ReturnToPool() { }
        public event Action<float> onTimeScale;

        public float timeScale { get; set; } = 1;
        public float GetUnscaleDelatime(bool ignorePause)
        {
            if (ignorePause)
            {
                return timeScale * TimeHelper.unscaledDeltaTimeIgnorePause;
            }
            else
            {
                return timeScale * TimeHelper.unscaledDeltaTime;
            }
        }

        public float GetDelatime(bool ignorePause)
        {
            if (ignorePause)
            {
                return timeScale * TimeHelper.deltaTimeIgnorePause;
            }
            else
            {
                return timeScale * TimeHelper.deltaTime;
            }
        }

        public void SetTimescale(float timeScale)
        {
            this.timeScale = timeScale;
            if (onTimeScale != null)
            {
                onTimeScale.Invoke(timeScale);
            }
        }

        public Tweener SetTimescale(float timeScale, float time)
        {
            return DOTween.To(() => timeScale, value => SetTimescale(value), timeScale, time).SetUpdate(true);
        }

        private void OnDestroy()
        {
            if (play != null)
            {
                play.Reset();
                play.Stop();
            }
        }
    }
}