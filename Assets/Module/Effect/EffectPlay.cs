using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public class EffectPlay : IProcess, ISetID<object, EffectPlay>
    {
        #region Static

        private const string effectPath = "Effect/Prefabs/{0}.prefab";
        private static Dictionary<object, List<EffectPlay>> effPlayList = new Dictionary<object, List<EffectPlay>>();
        private Coroutine playCoroutine;

        public static string GetPath(string path)
        {
            return string.Format(effectPath, path);
        }

        public static EffectPlay Play(string name, Transform parent, Action<EffectBase> callback = null)
        {
            return Play(name, parent, parent != null ? parent.position : Vector3.zero,
                parent != null ? parent.eulerAngles : Vector3.zero, 1, callback);
        }

        public static EffectPlay Play(string name, Transform parent, float scale, Action<EffectBase> callback = null)
        {
            return Play(name, parent, parent != null ? parent.position : Vector3.zero,
                parent != null ? parent.eulerAngles : Vector3.zero, scale, callback);
        }

        public static EffectPlay Play(string name, Vector3 pos, Vector3 dir, Action<EffectBase> callback = null)
        {
            return Play(name, null, pos, dir, 1, callback);
        }

        public static EffectPlay Play(string name, Vector3 pos, Vector3 dir, float scale,
            Action<EffectBase> callback = null)
        {
            return Play(name, null, pos, dir, scale, callback);
        }

        public static EffectPlay Play(string name, Transform parent, Vector3 pos, Vector3 dir, float scale, Action<EffectBase> callback)
        {
            EffectPlay play = new EffectPlay();
            if (parent != null && !parent.gameObject.activeInHierarchy) return play;
            play.pool = AssetLoad.LoadGameObject<EffectBase>(string.Format(effectPath, name), parent, (effect, args) =>
            {
                if (effect.gameObject.activeInHierarchy)
                {
                    if (effect == null)
                    {
                        Debug.LogError("effect==null : " + name);
                        return;
                    }

                    effect.play = play;
                    play.onLoadFinish?.Invoke(effect);

                    play.playCoroutine = effect.StartCoroutine(play.StartPlay(effect));
                    effect.transform.SetParent(parent);
                    effect.transform.position = pos;
                    effect.transform.eulerAngles = dir;

                    if (scale != 1)
                    {
                        effect.transform.localScale = Vector3.one * scale;
                    }

                    callback?.Invoke(effect);
                }
            });

            return play;
        }

        #endregion

        private bool m_isComplete;
        public object ID { get; set; }
        public Func<bool> monitor { get; set; }
        public bool ignoreTimeScale { get; private set; } = false;
        public EffectBase effect { get; private set; }
        public float runTime;

        public bool isActive
        {
            get { return effect != null; }
        }

        public ObjectPool pool;

        public bool ignorePause;

        public bool isComplete
        {
            get
            {
                if (monitor == null)
                {
                    return m_isComplete;
                }
                else
                {
                    return m_isComplete || monitor.Invoke();
                }
            }
        }

        public event Action onStart;
        public event Action<EffectBase> onLoadFinish;
        public event Action onComplete;
        public event Action<float> onUpdate;

        public IEnumerator StartPlay(EffectBase effect)
        {
            this.effect = effect;
            m_isComplete = false;
            effect.StartFirst();
            onStart?.Invoke();
            if (isComplete) yield break;
            effect.Restart();
            yield return this;
            Reset();
            Stop();
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

        public EffectPlay OnLoadFinish(Action<EffectBase> action)
        {
            onLoadFinish += action;
            return this;
        }

        public EffectPlay SetUpdate(bool ignoreTimeScale)
        {
            this.ignoreTimeScale = ignoreTimeScale;
            return this;
        }

        public EffectPlay SetIgnorePause(bool ignorePause)
        {
            this.ignorePause = ignorePause;
            return this;
        }

        public EffectPlay SetID(object id)
        {
            this.ID = id;
            List<EffectPlay> p = null;
            if (!effPlayList.TryGetValue(id, out p))
            {
                p = new List<EffectPlay>();
                effPlayList.Add(id, p);
            }

            p.Add(this);
            return this;
        }

        public void SetMonitor(Func<bool> monitor)
        {
            this.monitor = monitor;
        }

        public bool MoveNext()
        {
            if (!isActive) return true;
            runTime += ignoreTimeScale ? effect.GetUnscaleDelatime(ignorePause) : effect.GetDelatime(ignorePause);
            effect.Simulate(runTime);
            onUpdate?.Invoke(runTime);
            return !isComplete;
        }

        public void Reset()
        {
            m_isComplete = false;
            runTime = 0;
        }

        public object Current
        {
            get { return effect; }
        }

        public void Stop()
        {
            if (m_isComplete) return;
            m_isComplete = true;
            onComplete?.Invoke();
            effect.ReturnToPool();
            onStart = null;
            onComplete = null;
            if (playCoroutine != null)
            {
                effect.StopCoroutine(playCoroutine);
            }

            if (ID != null) effPlayList.Remove(ID);
        }

        public void SetComplete(bool flag)
        {
            m_isComplete = flag;
        }
    }
}