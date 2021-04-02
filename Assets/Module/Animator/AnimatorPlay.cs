using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Module
{

    public class AnimatorPlay: IEnumerator
    {
        #region AnimatorArgs

        private struct AnimatorArgs<T>
        {
            public T value;
            public Func<bool> listener;
            public bool changed;

            public AnimatorArgs(T value)
            {
                this.value = value;
                this.changed = false;
                listener = null;
            }

            public void Change(T arg, Func<bool> listner)
            {
                this.value = arg;
                changed = true;
                this.listener = listner;
            }

            public void Reset()
            {
                if (changed)
                {
                    changed = false;
                }
            }
        }

        #endregion

        #region Set

        private List<AnimatorArgs<(float, Action<float>)>> frameCallback =
            new List<AnimatorArgs<(float, Action<float>)>>();
        public bool isPause;
        private AnimatorArgs<float> speed;
        private AnimatorArgs<float> duation;
        private AnimatorArgs<bool> freeze;
        private AnimatorArgs<bool> ignoreTimescale;
        private AnimatorArgs<int> loop;
        private AnimatorArgs<bool> stop;
        private List<AnimatorPlay> children;

        public int loopIndex;

        #endregion

        private Coroutine coroutine;

        public ICoroutine owner { get; }
        public AnimatorInfo info { get; }
        public object Current { get; }

        public bool isPlaying
        {
            get { return coroutine != null; }
        }

        public bool isComplete;

        public float playTime;

        public float percent
        {
            get { return playTime / info.duation; }
        }

        private Action onStop;
        private Action tempCompleteAction;
        public AnimatorPlay(ICoroutine owner, AnimatorInfo info)
        {
            this.owner = owner;
            this.info = info;
        }

        public void StartCoroutine(Action callback)
        {
            tempCompleteAction = callback;
            coroutine = this.owner.StartCoroutine(WaitComplete(callback));
        }

        private IEnumerator WaitComplete(Action callback)
        {
            yield return this;
            if (loop.changed)
            {
                if ((loop.value != -1 && loop.value < loopIndex) || loop.value == -1)
                {
                    Restart();
                    loopIndex++;
                    yield break;
                }
                else
                {
                    loop.changed = false;
                }
            }
            isComplete = true;
            callback?.Invoke();
            this.onStop?.Invoke();
            Reset();
        }
        

        public AnimatorPlay OnStop(Action callback)
        {
            this.onStop = callback;
            return this;
        }

        public AnimatorPlay SetFrameCallback(float percent, Action<float> callback)
        {
            frameCallback.Add(new AnimatorArgs<(float, Action<float>)>((percent, callback)) {changed = true});
            return this;
        }

        public AnimatorPlay SetSpeed(float speed, Func<bool> listener = null)
        {
            if (!info.speedCtrl) GameDebug.LogError($"{info.fullName}不能控制动画速度");
            this.speed.Change(speed, listener);
            return this;
        }

        public AnimatorPlay SetFreeze(bool active, Func<bool> listener = null)
        {
            if (!info.speedCtrl) GameDebug.LogError($"{info.fullName}不能控制动画速度");
            this.freeze.Change(active, listener);
            return this;
        }

        public AnimatorPlay SetDuationTime(float time, Func<bool> listener = null)
        {
            if (!info.speedCtrl) GameDebug.LogError($"{info.fullName}不能控制动画速度");
            this.duation.Change(time, listener);
            return this;
        }

        public AnimatorPlay SetUpdate(bool ignoreTimescale, Func<bool> listener = null)
        {
            this.ignoreTimescale.Change(ignoreTimescale, listener);
            return this;
        }

        public AnimatorPlay SetLoop(int loop, Func<bool> listener = null)
        {
            this.loop.Change(loop, listener);
            return this;
        }


        public bool MoveNext()
        {
            if (isPause) return true;
            if (stop.changed && (stop.listener == null || (stop.listener != null && stop.listener.Invoke())))
            {
                Stop();
                return false;
            }
            
            if (duation.changed &&
                (duation.listener == null || (duation.listener != null && duation.listener.Invoke())))
            {
                info.duation = duation.value;
                info.localTimescale = owner.timeScale;
                duation.changed = false;
            }

            if (speed.changed&&
                (speed.listener == null || (speed.listener != null && speed.listener.Invoke())))
            {
                info.orignalSpeed = speed.value;
                speed.changed = false;
            }

            if (freeze.changed&&
                (freeze.listener == null || (freeze.listener != null && freeze.listener.Invoke())))
            {
                info.animator.speed = freeze.value ? 0 : 1;
                speed.changed = false;
            }

            if (ignoreTimescale.changed&&
                (ignoreTimescale.listener == null || (ignoreTimescale.listener != null && ignoreTimescale.listener.Invoke())))
            {
                info.animator.updateMode = ignoreTimescale.value ? AnimatorUpdateMode.UnscaledTime : AnimatorUpdateMode.Normal;
                ignoreTimescale.changed = false;
            }

            if (!freeze.value)
            {
                playTime += ignoreTimescale.value ? owner.GetUnscaleDelatime(false) : owner.GetDelatime(false);
            }

            if (!frameCallback.IsNullOrEmpty())
            {
                for (int i = 0; i < frameCallback.Count; i++)
                {
                    var temp = frameCallback[i];
                    if (percent >= temp.value.Item1 && temp.changed)
                    {
                        temp.value.Item2?.Invoke(playTime);
                        temp.changed = false;
                        frameCallback[i] = temp;
                    }
                }
            }

            bool childResult = true;
            if (!children.IsNullOrEmpty())
            {
                for (int i = 0; i < children.Count; i++)
                {
                    childResult = childResult && children[i].isComplete;
                }
            }

            if ((childResult && percent >= 1) || (percent > 0.5f && info.animator.IsInTransition(info.layer.layer))) return false;

            return true;
        }


        public void Reset()
        {
            isPause = false;
            onStop = null;
            coroutine = null;
        }

        public void AddChildren(AnimatorPlay play)
        {
            if (children == null) children = new List<AnimatorPlay>();
            children.Add(play);
        }

        public override string ToString()
        {
            return info.name;
        }

        public AnimatorPlay SetStop(Func<bool> listener = null)
        {
            if (listener == null)
            {
                Stop();
            }
            else
            {
                stop.Change(true, listener);
            }
            return this;
        }

        private float stopTranslate;

        private AnimatorPlay Stop()
        {
            if (coroutine != null)
            {
                this.onStop?.Invoke();
                owner.StopCoroutine(coroutine);
                info.layer.currPlay = null;
                Reset();
            }

            return this;
        }


        public AnimatorPlay Restart()
        {
            info.animator.CrossFade(info.fullName, info.translate, info.layer.layer, 0);
            playTime = 0;
            StartCoroutine(tempCompleteAction);
            if (!frameCallback.IsNullOrEmpty())
            {
                for (int i = 0; i < frameCallback.Count; i++)
                {
                    var temp = frameCallback[i];
                    temp.changed = true;
                    frameCallback[i] = temp;
                }
            }
            return this;
        }

        // public AnimatorPlay Complete()
        // {
        //     info.animator.Update(info.duation);
        //     playTime = info.duation;
        //     return this;
        // }

        public AnimatorPlay Pause()
        {
            if (!info.speedCtrl)
            {
                GameDebug.LogError($"{info.fullName}不能控制动画速度");
            }
            else
            {
                isPause = true;
                info.SetPause(true);
            }

            return this;
        }

        public AnimatorPlay Continue()
        {
            if (!info.speedCtrl)
            {
                GameDebug.LogError($"{info.fullName}不能控制动画速度");
            }
            else
            {
                isPause = false;
                info.SetPause(false);
            }

            return this;
        }
    }
}