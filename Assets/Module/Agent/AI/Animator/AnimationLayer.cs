using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Module
{
    public class AnimationLayer
    {
        private Tweener fadeIn;
        private Tweener fadeOut;
        private AnimationPlay _currentPlay;
        public AgentAnimatorCtrl ctrl { get; }
        public int layer { get; }

        public AnimationPlay currentPlay
        {
            get { return _currentPlay; }
        }

        public AnimationLayer(AgentAnimatorCtrl ctrl, int layer)
        {
            this.ctrl = ctrl;
            this.layer = layer;
        }

        public async void FadInLayer(float time)
        {
            Async.StopAsync(this);
            FadeInLayer();
            if (ctrl.owner.animator.updateMode == AnimatorUpdateMode.UnscaledTime)
            {
                await Async.WaitforSecondsRealTime(time, this);
            }
            else
            {
                await Async.WaitforSeconds(time, this);
            }
            FadeOutLayer();
        }

        private void FadeInLayer()
        {
            if (layer == 0) return;
            if (fadeIn != null)
            {
                fadeIn.Kill();
            }

            fadeIn = DOTween.To(() => ctrl.animator.GetLayerWeight(layer), lay => ctrl.animator.SetLayerWeight(layer, lay), 1f, 0.2f);
        }

        private void FadeOutLayer()
        {
            if (layer == 0) return;
            if (fadeOut != null)
            {
                fadeOut.Kill();
            }

            fadeOut = DOTween.To(() => ctrl.animator.GetLayerWeight(layer),
                lay => ctrl.animator.SetLayerWeight(layer, lay), 0f, 0.2f);
        }

        public void BreakPlay()
        {
            _currentPlay = null;
        }

        public void Reset()
        {
            ctrl.animator.SetLayerWeight(layer, 0);
            BreakPlay();
        }

        public AnimationPlay Play(AnimationInfo animationInfo, Action<AnimationPlayStation> callback)
        {
            FadeInLayer();
            _currentPlay = new AnimationPlay(ctrl, layer, animationInfo, station =>
            {
                if (station == AnimationPlayStation.Complete)
                { 
                    _currentPlay = null;
                    FadeOutLayer();
                }

                callback?.Invoke(station);
            });
            return _currentPlay;
        }

        public void OnUpdate()
        {
            if (_currentPlay != null)
            {
                _currentPlay.OnUpdate();
            }
        }
        
        public override string ToString()
        {
            return layer.ToString();
        }
    }
}