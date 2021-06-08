using System;
using Module.SkillLoop;

namespace Module
{
    public enum SkillStation
    {
        WaitRelease,
        OnRelease,
        OnDuation,
        OnCd
    }

    public abstract class SkillBase<T, K, L> : ISkill
        where T : ISkillLoop, new()
        where K : ISkillLoop, new()
        where L : ISkillLoop, new()
    {
        #region field

        private SkillStation _station;
        private bool _active;
        private Action onFinish;
        private K _duationLoop;
        private L _cdLoop;

        #endregion
        
        public abstract string name { get; }

        public SkillStation station
        {
            get { return _station; }
        }
        public ISkillObject owner { get; }

        public bool isActive
        {
            get { return _active && owner.IsSkillValid(name); }
        }

        public T release { get; }

        public K duation
        {
            get { return _duationLoop; }
            set
            {
                if (_duationLoop != null)
                {
                    _duationLoop.Dispose();
                    _duationLoop.onEnd -= OnDuationEnd;
                }
                _duationLoop = value;
                _duationLoop.onEnd += OnDuationEnd;
            }
        }

        public L cd
        {
            get { return _cdLoop; }
            set
            {
                if (_cdLoop != null)
                {
                    _cdLoop.Dispose();
                    _cdLoop.onEnd -= OnCdEnd;
                }
                
                _cdLoop = value; 
                _cdLoop.onEnd += OnCdEnd;
            }
        }

        #region OnEndLoop

        private void OnReleaseEnd()
        {
            if (duation == null)
            {
                OnDuationEnd();
            }
            else
            {
                EnterDuation();
            }
        }
        
        private void OnDuationEnd()
        {
            if (cd == null)
            {
                OnCdEnd();
            }
            else
            {
                EnterCd();
            }
            onFinish?.Invoke();
        }
        
        private void OnCdEnd()
        {
            _station = SkillStation.WaitRelease;
            GameDebug.LogFormat("{0} 开技能 {1} Cd 结束", owner, GetType().Name);
        }

        #endregion
        
        #region Enter

        private void EnterDuation()
        {
            _station = SkillStation.OnDuation;
            duation.Start();
            OnDuationStart();
            GameDebug.LogFormat("{0}开技能{1}开始持续", owner, GetType().Name);
        }

        private void EnterCd()
        {
            _station = SkillStation.OnCd;
            cd.Start();
            OnCdStart();
            GameDebug.LogFormat("{0}开技能{1}进入CD", owner, GetType().Name);
        }

        #endregion
        
        #region override

        protected virtual bool CanRelease()
        {
            return true;
        }
        protected virtual void OnReleaseStart()
        {
        }

        protected virtual void OnDuationStart()
        {
        }
        
        protected virtual void OnCdStart()
        {
        }

        protected virtual void OnBreak(bool withAnimation)
        {
        }

        #endregion

        #region Public

        public SkillBase(ISkillObject owner, float releaseTime)
        {
            this.owner = owner;
            release = new T();
            release.Init(this, releaseTime);
            release.onEnd += OnReleaseEnd;
        }

        public void SetActive(bool active, bool withAnimation = false)
        {
            _active = active;
            if (!active)
            {
                Break(withAnimation);
            }
        }

        public bool Release(Action callback)
        {
            if (isActive)
            {
                if (station == SkillStation.WaitRelease)
                {
                    if (CanRelease())
                    {
                        _station = SkillStation.OnRelease;
                        release.Start();
                        OnReleaseStart();
                        this.onFinish = () =>
                        {
                            GameDebug.LogFormat("{0}开技能{1}结束", owner, GetType().Name);
                            callback?.Invoke();
                        };
                        GameDebug.LogFormat("{0} 开始释放技能 {1}", owner, GetType().Name);
                        return true;
                    }
                }
                else
                {
                    GameDebug.LogFormat("{0}技能未就绪", this.GetType());
                }
            }
            else
            {
                GameDebug.LogFormat("{0}技能未激活", this.GetType());
            }

            return false;
        }

        public bool Break(bool withAnimation)
        {
            if (owner.CanBreakSkill(name))
            {
                switch (station)
                {
                    case SkillStation.OnRelease:
                        release.Break();
                        BreakPrivate(withAnimation);
                        return true;
                    case SkillStation.OnDuation:
                        duation.Break();
                        BreakPrivate(withAnimation);
                        return true;
                }
            }
            
            return false;
        }

        private void BreakPrivate(bool withAnimation)
        {
            GameDebug.LogFormat("{0} 技能 {1} 被打断", owner, GetType().Name);
            OnBreak(withAnimation);
            OnDuationEnd();
        }

        public virtual void Pause()
        {
            if (station != SkillStation.WaitRelease)
            {
                GameDebug.LogFormat("{0} 开技能 {1} 被暂停", owner, GetType().Name);
                if (station == SkillStation.OnRelease)
                {
                    release.Pause();
                }
                else if (station == SkillStation.OnDuation)
                {
                    duation.Pause();
                }
                else if (station == SkillStation.OnCd)
                {
                    cd.Pause();
                }
            }
        }

        public virtual void Continue()
        {
            if (station != SkillStation.WaitRelease)
            {
                GameDebug.LogFormat("{0} 开技能 {1} 继续", owner, GetType().Name);
                if (station == SkillStation.OnRelease)
                {
                    release.Continue();
                }
                else if (station == SkillStation.OnDuation)
                {
                    duation.Continue();
                }
                else if (station == SkillStation.OnCd)
                {
                    cd.Continue();
                }
            }

        }

        public void Dispose()
        {
            release.Dispose();
            release.onEnd -= OnReleaseEnd;
            
            if (duation != null)
            {
                duation.Dispose();
                duation.onEnd -= OnDuationEnd;
            }

            if (cd != null)
            {
                cd.Dispose();
                cd.onEnd -= OnCdEnd;
            }
            onFinish?.Invoke();
        }

        public override string ToString()
        {
            return GetType().Name + " Station: " + station;
        }

        #endregion
    }
}