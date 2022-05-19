using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public enum AnimationPlayStation
    {
        /// <summary>
        /// 正在播放
        /// </summary>
        Playing,
        /// <summary>
        /// 暂停
        /// </summary>
        Pause,
        /// <summary>
        /// 完成
        /// </summary>
        Complete,
        /// <summary>
        /// 暂停
        /// </summary>
        Break
    }

    public class AnimationPlay : IProcess
    {
        #region 字段

        private float _playTime;
        private float _playTotalTime;
        private int _loop = 1;
        private int _currentLoop;
        private AnimationPlayStation _station;
        private bool completeIgnoreTranslate;

        private AgentArgs<bool> _pause;
        private AgentArgs<bool> _continue;
        private AgentArgs<bool> _complete;
        private List<AgentArgs<float>> _frameCallback;

        #endregion

        #region 属性

        public int currentLoop
        {
            get { return _currentLoop; }
        }

        public AnimatorCtrl ctrl { get; }
        /// <summary>
        /// 当前播放的动画信息
        /// </summary>
        public AnimationInfo playInfo { get; }
        /// <summary>
        /// 当前播放的状态
        /// </summary>
        public AnimationPlayStation station
        {
            get { return _station; }
        }
        /// <summary>
        /// 当前播放的百分比
        /// </summary>
        public float percent
        {
            get
            {
                if (Mathf.Abs(playInfo.durationTime) < 0.001)
                {
                    return 0;
                }
                return _playTime / playInfo.durationTime;
            }
        }

        public event Action<AnimationPlay> onStationChange;

        #endregion

        public AnimationPlay(AnimatorCtrl ctrl, int layer, AnimationInfo info, float translate,Action<AnimationPlay> onStationChange)
        {
            this.ctrl = ctrl;
            this.playInfo = info;
            this.onStationChange = onStationChange;
            _currentLoop = 1;
            ctrl.owner.LogFormat("播放动画{0}", info.name);
            info.Play(layer, translate,0);
            _station = AnimationPlayStation.Playing;

            onStationChange?.Invoke(this);
        }

        public AnimationPlay SetCompleteTranslate(bool ignoreTranslate)
        {
            this.completeIgnoreTranslate = ignoreTranslate;
            return this;
        }

        public void Pause()
        {
            if (station == AnimationPlayStation.Playing)
            {
                playInfo.isPause = true;
                _station = AnimationPlayStation.Pause;
                onStationChange?.Invoke(this);
                _pause.value = true;
                _continue.value = false;
            }
        }

        public AnimationPlay SetPause(Func<bool> listener)
        {
            _pause = new AgentArgs<bool>(false, listener, Pause);
            return this;
        }

        public void Continue()
        {
            if (station == AnimationPlayStation.Pause)
            {
                playInfo.isPause = false;
                _station = AnimationPlayStation.Playing;
                onStationChange?.Invoke(this);
                _pause.value = false;
                _continue.value = true;
            }
        }

        public AnimationPlay SetContinue(Func<bool> listener)
        {
            _continue = new AgentArgs<bool>(false, listener, Continue);
            return this;
        }
        
        public void Complete()
        {
            if (station != AnimationPlayStation.Complete)
            {
                _playTime = 0;
                _station = AnimationPlayStation.Complete;
                onStationChange?.Invoke(this);
                _complete.value = true;
            }
        }

        public AnimationPlay SetComplete(Func<bool> listener)
        {
            _complete = new AgentArgs<bool>(false, listener, Complete);
            return this;
        }

        public AnimationPlay SetFrameCallback(float time, Action callback)
        {
            if (_frameCallback == null) _frameCallback = new List<AgentArgs<float>>();
            AgentArgs<float> temp = default;
            temp = new AgentArgs<float>(time, () => _playTime >= time, () =>
            {
                callback?.Invoke();
                _frameCallback.Remove(temp);
            });
            _frameCallback.Add(temp);
            return this;
        }

        public AnimationPlay SetLoop(int loop)
        {
            this._loop = loop;
            return this;
        }

        public AnimationPlay SetDuationTime(float time)
        {
            playInfo.durationTime = time;
            return this;
        }
        
        public AnimationPlay SetDuationSpeed(float speed)
        {
            playInfo.animationSpeed = speed;
            return this;
        }

        public bool OnUpdate()
        {
            if (station == AnimationPlayStation.Playing)
            {
                if (listener != null && listener.Invoke())
                {
                    Complete();
                    return false;
                }
                
                float delatime = ctrl.owner.animator.updateMode != AnimatorUpdateMode.UnscaledTime
                    ? ctrl.owner.GetDelatime(false)
                    : ctrl.owner.GetUnscaleDelatime(false);
                _playTime += delatime;
                _playTotalTime += delatime;

                if (currentLoop < _loop || _loop == -1 || playInfo.isLoop)
                {
                    if (isCompletePlay)
                    {
                        if (_loop != -1)
                        {
                            _currentLoop++;
                        }
                            
                        _playTime = 0;
                        if (!playInfo.isLoop)
                        {
                            playInfo.Play(playInfo.layer, 0, 0);
                        }
                    }
                }
                else
                {
                    if (isCompletePlay)
                    {
                        Complete();
                        return false;
                    }
                }
            }

            _pause.OnUpdate();
            _continue.OnUpdate();
            _complete.OnUpdate();
            if (_frameCallback != null)
            {
                for (int i = 0; i < _frameCallback.Count; i++)
                {
                    _frameCallback[i].OnUpdate();
                }
            }

            return true;
        }

        private bool isCompletePlay
        {
            get { return playInfo.isCompletePlay(percent, ctrl.owner.animator); }
        }

        public bool Break(bool sendEvent)
        {
            if (_station == AnimationPlayStation.Playing)
            {
                _station = AnimationPlayStation.Break;
                if (sendEvent)
                {
                    onStationChange?.Invoke(this);
                }
                Reset();

                return true;
            }

            return false;
        }

        public bool MoveNext()
        {
            return !isComplete;
        }

        public void Reset()
        {
            _playTime = 0;
            _playTotalTime = 0;
        }

        public object Current
        {
            get { return playInfo; }
        }

        public Func<bool> listener { get; set; }

        public bool isComplete
        {
            get { return station == AnimationPlayStation.Complete || station == AnimationPlayStation.Break; }
        }

        public void SetListener(Func<bool> listen)
        {
            this.listener = listen;
        }
    }
}