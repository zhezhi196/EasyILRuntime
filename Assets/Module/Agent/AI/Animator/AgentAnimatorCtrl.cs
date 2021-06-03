using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Module
{
    public class AgentAnimatorCtrl : IAgentCtrl
    {
        #region 构造

        public AgentAnimatorCtrl(IAnimatorObject owner)
        {
            this.owner = owner;
            layer = new AnimationLayer[owner.animator.layerCount];

            for (int i = 0; i < layer.Length; i++)
            {
                layer[i] = new AnimationLayer(this, i);
            }

            AnimationClip[] clips = owner.animator.runtimeAnimatorController.animationClips;
            this.animationInfo = new AnimationInfo[clips.Length];
            for (int i = 0; i < clips.Length; i++)
            {
                this.animationInfo[i] = new AnimationInfo(animator, clips[i]);
            }

        }

        #endregion

        #region 字段

        private List<object> _pauseList = new List<object>();
        private AnimationLayer _playingLayer;

        #endregion

        #region 属性

        public IAnimatorObject owner { get; }

        public Animator animator
        {
            get { return owner.animator; }
        }

        public AnimationLayer[] layer { get; }

        public bool isPause
        {
            get { return _pauseList.Count > 0; }
        }

        public AnimationLayer defalutLayer
        {
            get { return layer[0]; }
        }

        public AnimationLayer playingLayer
        {
            get
            {
                if (_playingLayer == null) return defalutLayer;
                return _playingLayer;
            }
        }

        public AnimationInfo[] animationInfo { get; }

        #endregion

        public void OnCreat()
        {
            owner.onSwitchStation += OnOwnerSwitchStation;
        }

        public void OnBorn()
        {
            for (int i = 1; i < layer.Length; i++)
            {
                layer[i].Reset();
            }
        }

        public void OnUpdate()
        {
            if (isPause) return;
            for (int i = 0; i < layer.Length; i++)
            {
                layer[i].OnUpdate();
            }
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

        public AnimationPlay Play(string name, int layer)
        {
            AnimationInfo target = null;
            for (int i = 0; i < animationInfo.Length; i++)
            {
                if (animationInfo[i].name == name)
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
                        _playingLayer = this.layer[i];
                        return this.layer[i].Play(target, station =>
                        {
                            if (station == AnimationPlayStation.Complete)
                            {
                                _playingLayer = null;
                            }
                        });
                    }
                }
            }

            return null;
        }

        public void Pause(object key)
        {
            _pauseList.Add(key);
            owner.animator.speed = 0;
            if (playingLayer.currentPlay != null)
            {
                playingLayer.currentPlay.Pause();
            }
        }

        public void Continue(object key)
        {
            _pauseList.Remove(key);
            if (!isPause)
            {
                owner.animator.speed = owner.animatorSpeed;
                if (playingLayer.currentPlay != null)
                {
                    playingLayer.currentPlay.Continue();
                }
            }
        }

        public void OnAgentDead()
        {
        }

        public float GetAnimationDuation(string name)
        {
            for (int i = 0; i < animationInfo.Length; i++)
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
    }
}