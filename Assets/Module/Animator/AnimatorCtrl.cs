using System.Collections.Generic;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;

namespace Module
{
    public interface IAnimatorCtrl
    {
        AnimatorPlay Play(string name, int layer);
    }
    public class AnimatorCtrl<T> : IAnimatorCtrl where T : ICoroutine
    {
        #region LayerArray

        public List<AnimatorLayer> layer { get; }
        public List<AnimatorLayer> playLayer { get; } = new List<AnimatorLayer>();

        #endregion
        
        public AnimatorInfo[] info { get; }
        public T owner { get; }
        public Animator animator { get; }
        public List<IAnimatorCtrl> children { get; }

        public AnimatorLayer currPlayingLayer
        {
            get
            {
                if (playLayer.Count > 0)
                {
                    return playLayer.GetLast();
                }
                else
                {
                    return layer[0];
                }
            }
        }
        
        public AnimatorCtrl(T owner, Animator animator)
        {
            this.owner = owner;
            this.animator = animator;
            info = AnimatorInfo.CreatInfo(owner, animator);

            if (!Application.isEditor)
            {
                animator.logWarnings = false;
            }

            layer = new List<AnimatorLayer>();
            AnimatorLayer defaultLayer = new AnimatorLayer(0);
            layer.Add(defaultLayer);
        }

        public AnimatorPlay Play(string name, int layer)
        {
            if (animator == null) return null;
            
            for (int i = 0; i < this.info.Length; i++)
            {
                if (info[i].name == name)
                {
                    AnimatorLayer lastLayer = currPlayingLayer;
                    AnimatorLayer targetLayer = GetLayer(layer);
                    float fadeTime = info[i].duation * info[i].translate;
                    if (lastLayer != targetLayer)
                    {
                        targetLayer.FadeInLayer(animator, fadeTime);
                        playLayer.MoveLastOrAdd(targetLayer);
                    }

                    if (!children.IsNullOrEmpty())
                    {
                        for (int j = 0; j < children.Count; j++)
                        {
                            children[i].Play(name, layer);
                        }
                    }

                    return targetLayer.Play(info[i], res =>
                    {
                        if (res)
                        {
                            for (int j = playLayer.Count - 1; j >= 0; j--)
                            {
                                if (playLayer[j] == targetLayer)
                                {
                                    targetLayer.FadeOutLayer(animator, fadeTime);
                                    playLayer.RemoveAt(j);
                                }
                            }
                        }
                    });
                }
            }

            return null;
        }

        public Tweener SetFloat(string name,float value, float time)
        {
            return DOTween.To(() => animator.GetFloat(name), res => animator.SetFloat(name, res), value, time);
        }
        
        public Tweener SetInt(string name,int value, float time)
        {
            return DOTween.To(() => animator.GetInteger(name), res => animator.SetInteger(name, res), value, time);
        }
        public void SetInt(string name,int value)
        {
            animator.SetInteger(name, value);
        }

        public AnimatorLayer GetLayer(int layer)
        {
            for (int i = 0; i < this.layer.Count; i++)
            {
                if (this.layer[i].layer == layer)
                {
                    return this.layer[i];
                }
            }

            AnimatorLayer res = new AnimatorLayer(layer);
            this.layer.Add(res);
            return res;
        }
        
        public void RestartAllLayer(AnimatorPlay except)
        {
            for (int i = 0; i < layer.Count; i++)
            {
                if (layer[i].currPlay != null && layer[i].currPlay != except)
                {
                    layer[i].currPlay.Restart();
                }
            }
        }

        // public void CompleteAllLayer(AnimatorPlay except)
        // {
        //     for (int i = 0; i < layer.Count; i++)
        //     {
        //         if (layer[i].currPlay != null && layer[i].currPlay != except)
        //         {
        //             layer[i].currPlay.Complete();
        //         }
        //     }
        // }

        public void PauseAllLayer(AnimatorPlay except)
        {
            for (int i = 0; i < layer.Count; i++)
            {
                if (layer[i].currPlay != null && layer[i].currPlay != except)
                {
                    layer[i].currPlay.Pause();
                }
            }
        }

        public void ContinueAllLayer(AnimatorPlay except)
        {
            for (int i = 0; i < layer.Count; i++)
            {
                if (layer[i].currPlay != null && layer[i].currPlay != except)
                {
                    layer[i].currPlay.Continue();
                }
            }
        }

    }
}