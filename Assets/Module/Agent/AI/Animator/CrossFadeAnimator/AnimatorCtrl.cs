using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Module
{
    public class AnimatorCtrl : IAgentCtrl
    {
        #region 字段 属性 事件

        private bool _isPause;
        private AnimationLayer[] _layer;
        private List<AnimationInfo> _animationInfo = new List<AnimationInfo>();
        public event Action<int> onFadeOutLayer;
                
        public AnimationLayer currentLayer
        {
            get
            {
                for (int i = layer.Length - 1; i >= 0; i--)
                {
                    if (layer[i].isPlaying) return layer[i];
                }

                return defalutLayer;
            }
        }

        public IAnimatorObject owner { get; }
        public Animator animator => owner.animator;
        public AnimationLayer[] layer => _layer;
        public bool isPause => _isPause;
        public AnimationLayer defalutLayer => layer[0];
        public List<AnimationInfo> animationInfo => _animationInfo;

        #endregion
        
        public AnimatorCtrl(IAnimatorObject owner)
        {
            this.owner = owner;
            animationInfo.Clear();
            ChangeController(owner);
            for (int i = 1; i < layer.Length; i++)
            {
                if (layer[i].layer > 0)
                {
                    layer[i].Reset();
                }
            }
            owner.onSwitchStation += OnOwnerSwitchStation;
            animator.logWarnings = false;
#if !UNITY_EDITOR
            animator.logWarnings = false;
#endif
        }

        public bool OnUpdate()
        {
            for (int i = 0; i < layer.Length; i++)
            {
                layer[i].OnUpdate();
            }

            return true;
        }

        private void OnOwnerSwitchStation()
        {
            if (isPause)
            {
                owner.animator.speed = 0;
            }
            else
            {
                owner.animator.speed = owner.animatorSpeed;
            }
        }
        /// <summary>
        /// 获取当前层级播放的动画名
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetPlayingName(int index)
        {
            var temp = layer[index];
            if (temp.currentPlay != null) return temp.currentPlay.playInfo.name;
            return null;
        }
        /// <summary>
        /// 犹豫融合树无法自动创建,需要手动创建动画信息
        /// </summary>
        /// <param name="fullName"></param>
        public void AddBlendTree(string fullName)
        {
            AddBlendTree(fullName, 0);
        }
        public void AddBlendTree(string fullName,float time)
        {
            AnimationInfo info = new AnimationInfo(animator, fullName, time, false);
            this.animationInfo.Add(info);
        }
        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="name">动画名</param>
        /// <param name="layer">层级</param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public AnimationPlay Play(string name, int layer, AnimationFlag flag)
        {
            AnimationInfo target = null;
            for (int i = 0; i < animationInfo.Count; i++)
            {
                if (animationInfo[i].name == name && animationInfo[i].isSameLayer(layer))
                {
                    target = animationInfo[i];
                    break;
                }
            }

            if (target != null)
            {
                for (int i = 0; i < this.layer.Length; i++)
                {
                    if (this.layer[i].layer == layer)
                    {
                        var result = this.layer[i].Play(target, owner.GetTranslateTime(name), null, flag);
                        return result;
                    }
                }
            }

            return null;
        }
        
        /// <summary>
        /// 渐变出层级
        /// </summary>
        /// <param name="layer"></param>
        public void OnFadeOutLayer(int layer)
        {
            onFadeOutLayer?.Invoke(layer);
        }

        private float slerpTemp;
        /// <summary>
        /// SetFoloat with tween
        /// </summary>
        /// <param name="name"></param>
        /// <param name="target"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public Tweener SetFloat(string name, float target, float speedOrtime, bool updateCall = false)
        {
            if (!updateCall)
            {
                return DOTween
                    .To(() => animator.GetFloat(name), (value) => { animator.SetFloat(name, value); }, target, speedOrtime)
                    .SetUpdate(true);
            }
            else
            {
                Mathf.MoveTowards(animator.GetFloat(name), target, owner.GetDelatime(false)*speedOrtime);
                return null;
            }

        }

        /// <summary>
        /// 打断当前正在播放的层级动画
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="SendEvent"></param>
        /// <param name="ignoreActive"></param>
        public void BreakAnimation(int layer, bool SendEvent, AnimationFlag flag = 0)
        {
            FadeOutLayer(layer, SendEvent, flag | AnimationFlag.ForceFadeOut);
        }
        
        /// <summary>
        /// 更改动画管理器
        /// </summary>
        /// <param name="tar"></param>
        public void ChangeController(IAnimatorObject tar)
        {
            if (_layer != null)
            {
                for (int i = 0; i < _layer.Length; i++)
                {
                    _layer[i].Reset();
                }
            }
            animationInfo.Clear();
            _layer = new AnimationLayer[owner.animator.layerCount];

            for (int i = 0; i < layer.Length; i++)
            {
                layer[i] = new AnimationLayer(this, i);
            }

            AnimationClip[] clips = owner.animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                var clip = clips[i];
                this.animationInfo.Add(new AnimationInfo(animator, clip.name, clip.length, clip.isLooping));
            }
        }

        /// <summary>
        /// 渐变出层
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="sendEvent"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool FadeOutLayer(int layer, bool sendEvent, AnimationFlag flag)
        {
            for (int i = 0; i < this.layer.Length; i++)
            {
                if (this.layer[i].layer == layer)
                {
                    return this.layer[i].FadeOutLayer(sendEvent, flag);
                }
            }

            return false;
        }

        /// <summary>
        /// 渐变到默认层
        /// </summary>
        /// <param name="sendEvent"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool FadeOutToDefaultLayer(bool sendEvent, AnimationFlag flag)
        {
            for (int i = 0; i < this.layer.Length; i++)
            {
                if (this.layer[i].layer != 0 && this.layer[i].isPlaying)
                {
                    return this.layer[i].FadeOutLayer(sendEvent, flag | AnimationFlag.ForceFadeOut);
                }
            }

            return false;
        }

        #region 继续暂停

        public void Pause()
        {
            owner.animator.speed = 0;
            for (int i = 0; i < this.layer.Length; i++)
            {
                if (this.layer[i].currentPlay != null)
                {
                    this.layer[i].currentPlay.Pause();
                }
            }

            _isPause = true;
        }
        
        public void Continue()
        {
            if (isPause)
            {
                owner.animator.speed = owner.animatorSpeed;
                for (int i = 0; i < this.layer.Length; i++)
                {
                    if (this.layer[i].currentPlay != null)
                    {
                        this.layer[i].currentPlay.Continue();
                    }
                }
                _isPause = false;
            }
        }

        public void PauseLayer(int layer)
        {
            for (int i = 0; i < this.layer.Length; i++)
            {
                if (this.layer[i].layer == layer)
                {
                    this.layer[i].PauseLayer();
                    break;
                }
            }
        }

        public void ContinueLayer(int layer)
        {
            for (int i = 0; i < this.layer.Length; i++)
            {
                if (this.layer[i].layer == layer)
                {
                    this.layer[i].ContinueLayer();
                    break;
                }
            }
        }

        #endregion

        public void OnAgentDead()
        {
        }

        /// <summary>
        /// 获取动画信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public AnimationInfo GetInfo(string name, int layer)
        {
            for (int i = 0; i < animationInfo.Count; i++)
            {
                if (animationInfo[i].name == name && animationInfo[i].isSameLayer(layer))
                {
                    return animationInfo[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 获取动画时长
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public float GetAnimationDuation(string name)
        {
            for (int i = 0; i < animationInfo.Count; i++)
            {
                if (animationInfo[i].name == name)
                {
                    return animationInfo[i].durationTime;
                }
            }

            return 0;
        }

        public void OnDestroy()
        {
            owner.onSwitchStation -= OnOwnerSwitchStation;
        }

        public T GetAgentCtrl<T>() where T : IAgentCtrl
        {
            return owner.GetAgentCtrl<T>();
        }

        public void EditorInit()
        {
            if (owner.transform != null)
            {
                Animator _animator = owner.transform.GetComponentInChildren<Animator>(true);
                _animator.applyRootMotion = false;
            }
        }

        public void OnDrawGizmos()
        {
        }
    }
}