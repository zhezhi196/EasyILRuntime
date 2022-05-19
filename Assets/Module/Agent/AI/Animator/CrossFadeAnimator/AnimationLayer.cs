using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Module
{
    [Flags]
    public enum AnimationFlag
    {
        /// <summary>
        /// 进层无渐变
        /// </summary>
        NotFadeIn = 1,
        /// <summary>
        /// 动画结束不自动渐变出层
        /// </summary>
        NotAutoOut = 2,
        /// <summary>
        /// 无视是否有效
        /// </summary>
        IgnoreActive = 4,
        /// <summary>
        /// 出层无渐变
        /// </summary>
        NotFadeOut = 8,

        /// <summary>
        /// 无论如何强制出层
        /// </summary>
        ForceFadeOut = 16
    }
    
    /// <summary>
    /// 动画层 这里可以对分层状态机的层进行控制
    /// </summary>
    public class AnimationLayer
    {
        private List<string> fadeList = new List<string>();
        private bool _isPlaying;
        
        private Tweener fadeIn;
        private AnimationPlay _currentPlay;
        public AnimatorCtrl ctrl { get; }
        public int layer { get; }

        public bool isPlaying
        {
            get { return _isPlaying || layer == 0; }
        }

        public AnimationPlay currentPlay
        {
            get { return _currentPlay; }
        }

        public AnimationLayer(AnimatorCtrl ctrl, int layer)
        {
            this.ctrl = ctrl;
            this.layer = layer;
        }
        
        /// <summary>
        /// 播放层中的某一动画
        /// </summary>
        /// <param name="animationInfo">动画信息</param>
        /// <param name="translateTime">融合时间</param>
        /// <param name="callback">状态发生改变时的回调</param>
        /// <param name="flag">flag</param>
        /// <returns></returns>
        public AnimationPlay Play(AnimationInfo animationInfo, float translateTime, Action<AnimationPlayStation> callback, AnimationFlag flag)
        {
            BreakPlay(true);
            FadeInLayer(animationInfo.name, flag);
            
            _currentPlay = new AnimationPlay(ctrl, layer, animationInfo,translateTime, station =>
            {
                if (station.isComplete)
                {
                    if ((flag & AnimationFlag.NotAutoOut) == 0)
                    {
                        FadeOutLayer(true, flag, animationInfo.name);
                    }

                    callback?.Invoke(station.station);
                    _currentPlay = null;
                }
            });
            return _currentPlay;
        }
        
        /// <summary>
        /// 进层
        /// </summary>
        /// <param name="name"></param>
        /// <param name="flag"></param>
        private void FadeInLayer(string name, AnimationFlag flag)
        {
            if (layer == 0) return;
            if (!fadeList.Contains(name))
            {
                fadeList.Add(name);
            }
            if (fadeIn != null)
            {
                fadeIn.Kill();
            }
            _isPlaying = true;
            if ((flag & AnimationFlag.NotFadeIn) == 0)
            {
                fadeIn = DOTween.To(() => ctrl.animator.GetLayerWeight(layer), lay => ctrl.animator.SetLayerWeight(layer, lay), 1f, ctrl.owner.GetLayerFadeTime(0, name));
            }
            else
            {
                ctrl.animator.SetLayerWeight(layer, 1);
            }

            ctrl.owner.LogFormat("为了播放{0}动画,打开{1}动画层", name, layer);
        }

        /// <summary>
        /// 出层
        /// </summary>
        /// <param name="sendEvent"></param>
        /// <param name="flag"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool FadeOutLayer(bool sendEvent, AnimationFlag flag, string name = null)
        {
            if (name == null)
            {
                if (currentPlay != null)
                {
                    name = currentPlay.playInfo.name;
                }
                else
                {
                    return false;
                }
            }

            if (fadeList.Contains(name))
            {
                fadeList.Remove(name);
            }
            
            if (fadeIn != null)
            {
                fadeIn.Kill();
            }

            BreakPlay(sendEvent);
            if ((fadeList.Count == 0 || (flag & AnimationFlag.ForceFadeOut) != 0) && layer != 0)
            {
                if ((flag & AnimationFlag.NotFadeOut) == 0)
                {
                    fadeIn = DOTween.To(() => ctrl.animator.GetLayerWeight(layer),
                        lay => ctrl.animator.SetLayerWeight(layer, lay), 0f, ctrl.owner.GetLayerFadeTime(1, name));
                }
                else
                {
                    ctrl.animator.SetLayerWeight(layer, 0);
                }

                _isPlaying = false;
                ctrl.OnFadeOutLayer(layer);
                fadeList.Clear();
                ctrl.owner.LogFormat("关闭{0}动画层", layer);
                return true;
            }

            return false;
        }

        public bool BreakPlay(bool sendEvent, AnimationPlay play)
        {
            if (play != null)
            {
                var name = play.playInfo.name;
                if (play.Break(sendEvent))
                {
                    ctrl.owner.OnBreakAnamition(layer, name, sendEvent);
                    string defaultAni = ctrl.owner.GetLayerDefaultAnimation(layer);
                    if (!defaultAni.IsNullOrEmpty())
                    {
                        ctrl.owner.LogFormat("打断{0}层动画到,播放{1}", layer, defaultAni);
                        ctrl.animator.CrossFade(defaultAni, ctrl.owner.getBreakTranslation(layer, name), layer);
                    }
                    ctrl.owner.LogFormat("打断动画层级{0} 是否发送事件{1},当前正在播放{2}", layer, sendEvent,name);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 打断当前播放
        /// </summary>
        /// <param name="sendEvent"></param>
        /// <returns></returns>
        public bool BreakPlay(bool sendEvent)
        {
            if (BreakPlay(sendEvent, _currentPlay))
            {
                _currentPlay = null;
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            ctrl.animator.SetLayerWeight(layer, 0);
            _currentPlay = null;
        }
        
        /// <summary>
        /// 暂停当前播放的暂停
        /// </summary>
        public void PauseLayer()
        {
            if (currentPlay != null) currentPlay.Pause();
        }
        
        /// <summary>
        /// 继续当前播放的暂停
        /// </summary>
        public void ContinueLayer()
        {
            if (currentPlay != null) currentPlay.Continue();
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