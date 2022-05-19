using System;
using UnityEngine;

namespace Module
{
    public class AnimationInfo
    {
        private bool _haveParamater;
        private Animator _animator;
        public string fullName;
        private string _speedParamater;
        private float _duationTime;
        private float _clipLength;
        private float _animationSpeed;
        private bool _isPause;
        public int shotNameHash;
        public int layer { get; set; } = -1;
        public string name { get; }
        public bool isLoop { get; }

        public float durationTime
        {
            get { return _duationTime; }
            set
            {
                _duationTime = value;
                animationSpeed = _clipLength / value;
            }
        }

        public float animationSpeed
        {
            get { return _animationSpeed; }
            set
            {
                if (_haveParamater && Mathf.Abs(value) > 0.0001f)
                {
                    _animationSpeed = value;
                    _duationTime = _clipLength / value;
                    _animator.SetFloat(_speedParamater, isPause ? 0 : _animationSpeed);
                }
                else
                {
                    GameDebug.LogWarn($"{fullName}无法控制速度,请在{_animator.gameObject.name}上加入float参数: {fullName}");
                }
            }
        }

        public bool isPause
        {
            get { return _isPause; }
            set
            {
                _isPause = value;
                _animator.SetFloat(_speedParamater, _isPause ? 0 : animationSpeed);
            }
        }

        public AnimationInfo(Animator animator, string fullName, float length, bool isLoop)
        {
            this._animator = animator;
            this.fullName = fullName;
            this.name = fullName;
            this._speedParamater = this.name;
            this._clipLength = length;
            this._duationTime = _clipLength;
            var allPar = _animator.parameters;
            for (int i = 0; i < allPar.Length; i++)
            {
                if (allPar[i].name == this.fullName)
                {
                    _haveParamater = true;
                    break;
                }
            }

            animationSpeed = 1;
            this.isLoop = isLoop;
            shotNameHash = Animator.StringToHash(fullName);
        }

        public virtual void Play(int layer, float translateTime,float offset)
        {
            this.layer = layer;
            _animator.CrossFade(fullName, translateTime, layer, offset, 0);
        }

        public override string ToString()
        {
            return fullName;
        }

        public bool isSameLayer(int layer)
        {
            return this.layer == -1 || this.layer == layer;
        }

        public virtual bool isCompletePlay(float percent,Animator animator)
        {
            bool isComplete;
            if (Math.Abs(durationTime) > 0.001f)
            {
                isComplete = percent >= 0.6f;
            }
            else
            {
                isComplete = !animator.IsInTransition(layer);
            }
            var temp = animator.GetCurrentAnimatorStateInfo(layer);
            return (percent >= 1 || temp.shortNameHash != shotNameHash) && isComplete;
        }
    }
}