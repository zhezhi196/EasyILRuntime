using DG.Tweening;
using UnityEngine;

namespace Module
{
    public class AnimatorInfo
    {
        public static AnimatorInfo[] CreatInfo(ICoroutine owner,Animator animator)
        {
            AnimatorInfo[] temp = new AnimatorInfo[animator.runtimeAnimatorController.animationClips.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                AnimationClip clip = animator.runtimeAnimatorController.animationClips[i];
                float length = 0;
                temp[i] = new AnimatorInfo(owner,animator, clip.name, clip.length);
            }

            return temp;
        }

        private float _localTimescale;
        private float _orignalSpeed;
        private float _duation;
        private string _speedCtrl;
        private bool isFreeze;

        public float translate { get; set; } = 0.1f;

        public Animator animator { get; }
        public string name { get; }
        public string fullName { get; }
        public float clipDuation { get; }
        public bool speedCtrl { get; }

        public ICoroutine owner { get; }

        public AnimatorLayer layer;

        public float AnimatorSpeed
        {
            get { return _localTimescale * _orignalSpeed * (isFreeze ? 0 : 1); }
        }

        public float localTimescale
        {
            get { return _localTimescale;}
            set
            {
                if (speedCtrl)
                {
                    _localTimescale = value;
                    _duation = clipDuation / value * _localTimescale;
                    animator.SetFloat(_speedCtrl, AnimatorSpeed);
                }
            }
        }

        public float duation
        {
            get { return _duation; }
            set
            {
                _duation = value;
                orignalSpeed = clipDuation / value;
            }
        }

        public float orignalSpeed
        {
            get { return _orignalSpeed; }
            set
            {
                if (speedCtrl)
                {
                    _orignalSpeed = value;
                    _duation = clipDuation / value * _localTimescale;
                    animator.SetFloat(_speedCtrl, AnimatorSpeed);
                }
            }
        }

        public AnimatorInfo(ICoroutine owner,Animator animator, string name, float clipDuation)
        {
            this.owner = owner;
            this.animator = animator;
            this.fullName = name;
            this.name = name.Split('@')[1];
            _speedCtrl = this.name;
            var t = animator.parameters;
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i].name == this.name)
                {
                    speedCtrl = true;
                    break;
                }
            }
            
            this.clipDuation = clipDuation;
            this._duation = clipDuation;
            _orignalSpeed = 1;
            localTimescale = owner.timeScale;
        }

        public void Play(AnimatorLayer layer)
        {
            this.layer = layer;
            animator.CrossFade(fullName, translate, layer.layer);
        }

        public void SetPause(bool freeze)
        {
            if (speedCtrl)
            {
                isFreeze = freeze;
                animator.SetFloat(_speedCtrl, AnimatorSpeed);
            }
        }

        public override string ToString()
        {
            return fullName;
        }
    }
}