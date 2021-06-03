using System;
using UnityEngine;

namespace Module
{
    public enum AnimationPlayStation
    {
        Playing,
        Pause,
        Complete
    }
    
    public class AnimationPlay
    {
        #region 字段

        private float _playTime;
        
        private AgentArgs<bool> _pause;
        private AgentArgs<bool> _continue;
        private AgentArgs<bool> _complete;
        private AgentArgs<float> _frameCallback;
        private int _loop;

        #endregion

        #region 属性

        public int currentLoop { get; set; }
        public AgentAnimatorCtrl agentAnimator { get; }
        public AnimationInfo playInfo { get; }
        public AnimationPlayStation station { get; set; }
        
        public float percent
        {
            get { return _playTime / playInfo.durationTime; }
        }

        public Action<AnimationPlayStation> onStationChange;

        #endregion

        public AnimationPlay(AgentAnimatorCtrl ctrl, int layer, AnimationInfo info, Action<AnimationPlayStation> onStationChange)
        {
            this.agentAnimator = ctrl;
            this.playInfo = info;
            this.onStationChange = onStationChange;
            info.Play(layer);
            station = AnimationPlayStation.Playing;
            onStationChange?.Invoke(station);
        }

        public void Pause()
        {
            station = AnimationPlayStation.Pause;
            onStationChange?.Invoke(station);
            _pause.value = true;
            _continue.value = false;
        }

        public AnimationPlay SetPause(Func<bool> listener)
        {
            _pause = new AgentArgs<bool>(false, listener, Pause);
            return this;
        }

        public void Continue()
        {
            station = AnimationPlayStation.Playing;
            onStationChange?.Invoke(station);
            _pause.value = false;
            _continue.value = true;
        }

        public AnimationPlay SetContinue(Func<bool> listener)
        {
            _continue = new AgentArgs<bool>(false, listener, Continue);
            return this;
        }
        
        public void Complete()
        {
            _playTime = 0;
            station = AnimationPlayStation.Complete;
            onStationChange?.Invoke(station);
            _complete.value = true;
        }

        public AnimationPlay SetComplete(Func<bool> listener)
        {
            _complete = new AgentArgs<bool>(false, listener, Complete);
            return this;
        }

        public AnimationPlay SetFrameCallback(float percent, Action callback)
        {
            _frameCallback = new AgentArgs<float>(percent, () => this.percent >= percent, callback);
            return this;
        }

        public AnimationPlay SetLoop(int loop)
        {
            this._loop = loop;
            return this;
        }

        public void OnUpdate()
        {
            if (station == AnimationPlayStation.Playing)
            {
                _playTime += agentAnimator.owner.animator.updateMode != AnimatorUpdateMode.UnscaledTime
                    ? agentAnimator.owner.GetDelatime(false)
                    : agentAnimator.owner.GetUnscaleDelatime(false);

                if (currentLoop < _loop || _loop == -1)
                {
                    if (percent >= 1)
                    {
                        if (_loop != -1)
                        {
                            currentLoop++;
                        }
                        _playTime = 0;
                        playInfo.Play(playInfo.layer);
                    }
                }
                else
                {
                    if (percent >= 1 || (percent > 0.6f && agentAnimator.animator.IsInTransition(playInfo.layer)))
                    {
                        Complete();
                    }
                }
            }

            _pause.OnUpdate();
            _continue.OnUpdate();
            _complete.OnUpdate();
            _frameCallback.OnUpdate();
        }
    }
}