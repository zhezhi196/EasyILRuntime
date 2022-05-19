using System;
using UnityEngine;

namespace Module
{
    public class StateMachinePlay
    {
        public bool isPlaying;
        public event Action<AnimationEvent,int> onEvent;
        public event Action onStart;
        public event Action<bool> onEnd;
        public StateCell cell;
        private IStateMechineObject owner;
        public float playTime;
        public RuntimeAnimatorController controller;

        public StateMachinePlay(StateCell cell, IStateMechineObject owner)
        {
            this.cell = cell;
            this.owner = owner;
            this.controller = owner.animator.runtimeAnimatorController;
            owner.animationCallback.onAnimationCallback += OnEvent;
        }

        private void OnEvent(AnimationEvent obj, int index)
        {
            if (obj.animatorStateInfo.shortNameHash == cell.shortName && obj.isFiredByAnimator && isPlaying)
            {
                onEvent?.Invoke(obj, index);
            }
        }

        public void Play()
        {
            cell.Play();
            isPlaying = true;
        }

        public void StartPlay(AnimatorStateInfo info)
        {
            if (info.shortNameHash != cell.shortName)
            {
                GameDebug.LogError("无法找到匹配信息" + cell.firstStateName);
                return;
            }
            
            owner.LogFormat("状态机动画 开始播放{0}", cell.name);
            onStart?.Invoke();
        }

        public void EndPlay(bool complete)
        {
            if (isPlaying)
            {
                owner.animationCallback.onAnimationCallback -= OnEvent;
                onEnd?.Invoke(complete);
                isPlaying = false;
                cell.Stop(complete);
                owner.LogFormat("状态机动画 结束播放{0} 完成{1}", cell.name, complete);
            }
        }

        public void OnUpdate()
        {
            playTime += owner.GetDelatime(false);
        }


        public bool IsSame(Animator animator, int shotNameHash, int layerIndex)
        {
            return IsSame(animator, shotNameHash) && this.cell.layer == layerIndex;
        }
        public bool IsSame(Animator animator, int shotNameHash)
        {
            return this.isPlaying && this.controller == animator.runtimeAnimatorController &&
                   shotNameHash == this.cell.shortName;
        }

        public override string ToString()
        {
            return cell.ToString();
        }
    }
}