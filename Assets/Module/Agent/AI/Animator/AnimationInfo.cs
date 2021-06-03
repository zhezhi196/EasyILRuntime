using System;
using UnityEngine;

namespace Module
{
    public class AnimationInfo
    {
        private bool _haveParamater;
        private Animator _animator;
        private string _fullName;
        private float _translate = 0.1f;
        private string _speedParamater;
        private float _duationTime;
        private float _clipLength;
        private float _animationSpeed;
        private bool _isPause;
        public int layer { get; set; }
        public string name { get; }
        
        public int nameHash { get; }

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
                if (_haveParamater && value != 0)
                {
                    _animationSpeed = value;
                    _duationTime = _clipLength / value;
                    _animator.SetFloat(_speedParamater, isPause ? 0 : _animationSpeed);
                }
                else
                {
                    GameDebug.LogError($"{_fullName}无法控制速度,请在{_animator.gameObject.name}上加入float参数: {_fullName}");
                }
            }
        }

        public bool isPause
        {
            get { return _isPause; }
            set
            {
                _isPause = value;
                _animator.SetFloat(_speedParamater, isPause ? 0 : _animationSpeed);
            }
        }
        
        public AnimationInfo(Animator animator, AnimationClip clip)
        {
            this._animator = animator;
            this._fullName = clip.name;
            this.name = name.Split('@')[1];
            this._speedParamater = this.name;
            this._clipLength = clip.length;
            this._duationTime = _clipLength;
            _animationSpeed = 1;
            var allPar = _animator.parameters;
            for (int i = 0; i < allPar.Length; i++)
            {
                if (allPar[i].name == this.name)
                {
                    _haveParamater = true;
                    break;
                }
            }
        }

        public void Play(int layer)
        {
            this.layer = layer;
            _animator.CrossFade(_fullName, _translate, layer);
        }

        public override string ToString()
        {
            return _fullName;
        }
    }
}