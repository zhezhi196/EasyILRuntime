using System;
using System.Collections.Generic;
using Module.SkillAction;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    public abstract class Skill : SerializedScriptableObject, ISkill
    {
        #region field
        private bool _isActive;
        private int _indexAction;
        private SkillStation _station;
        private Action<bool> _realeseEndAction;
        private ISkillObject _owner;
        protected List<ISkillAction> actions = new List<ISkillAction>();
        
        public event Action<bool, SkillStation> onStationChanged;

        #endregion

        #region 属性
        public string logName => owner.logName;

        public virtual string name
        {
            get { return this.GetType().Name; }
        }
        public bool isLog
        {
            get
            {
                return owner.isLog;
            }
            set
            {
                owner.isLog = value;
            }
        }


        public virtual bool isReadyRelease
        {
            get { return isActive && station == SkillStation.Ready; }
        }

        public float cdTotalTime
        {
            get
            {
                if (cd is TimeAction time)
                {
                    return time.timeClock.targetTime;
                }

                return 0;
            }
        }

        public float cdTime
        {
            get
            {
                if (cd is TimeAction time)
                {
                    return time.timeClock.remainTime;
                }

                return 0;
            }
        }

        public SkillStation station
        {
            get { return _station; }
            set
            {
                if (_station != value)
                {
                    _station = value;
                    onStationChanged?.Invoke(isActive, value);
                }
            }
        }

        public ISkillAction runningAction
        {
            get
            {
                if (station != SkillStation.OnRelease) return null;
                return actions[_indexAction];
            }
        }

        public ISkillObject owner
        {
            get { return _owner; }
        }

        public ISkillAction cd { get; set; }

        public virtual bool isActive
        {
            get { return _isActive; }
            set
            {
                if (!value) Break(BreakSkillFlag.WithAnimation | BreakSkillFlag.BreakAction);
                if (isActive != value)
                {
                    onStationChanged?.Invoke(value, station);
                }

                _isActive = value;
            }
        }

        #endregion

        #region Private method

        private void EndAction(ISkillAction obj, bool complete)
        {
            if (runningAction != null)
            {
                if (!complete || actions.Last() == obj)
                {
                    _realeseEndAction?.Invoke(complete);
                    OnReleaseEnd(complete);
                    _indexAction = 0;
                    if (cd != null && cdTotalTime > 0)
                    {
                        EnterCd();
                    }
                    else
                    {
                        CdEnd(null);
                    }
                }
                else
                {
                    OnActionEnd(runningAction);
                    _indexAction++;
                    runningAction.OnStart();
                    OnActionEnter(runningAction);
                }
            }
        }

        private void EnterCd()
        {
            station = SkillStation.CD;
            cd.OnStart();
            OnCdStart();
            LogFormat("{0}开技能{1}进入CD", owner, GetType().Name);
        }

        public void EnterReady()
        {
            CdEnd(cd);
        }
        private void CdEnd(ISkillAction cd)
        {
            station = SkillStation.Ready;
            OnCdEnd();
            LogFormat("{0} 开技能 {1} Cd 结束", owner, GetType().Name);
        }

        #endregion
        
        #region override
        
        /// <summary>
        /// 当准备释放的时候调用
        /// </summary>
        protected virtual void OnRelesePrepare()
        {
        }
        
        /// <summary>
        ///  当开始释放的时候调用
        /// </summary>
        protected virtual void OnReleaseStart()
        {
        }
        
        /// <summary>
        /// 当进入一个循环逻辑调用
        /// </summary>
        /// <param name="skillAction"></param>
        protected virtual void OnActionEnter(ISkillAction skillAction)
        {
        }

        /// <summary>
        ///  当一个循环逻辑结束调用
        /// </summary>
        /// <param name="skillAction"></param>
        protected virtual void OnActionEnd(ISkillAction skillAction)
        {
        }

        /// <summary>
        /// 当结束释放调用
        /// </summary>
        protected virtual void OnReleaseEnd(bool complete)
        {
        }

        protected virtual void OnCdStart()
        {
        }

        protected virtual void OnCdEnd()
        {
        }
        
        protected virtual void OnActionUpdate(ISkillAction arg1,float percent)
        {
        }
        
        protected virtual void OnCdUpdate(ISkillAction arg1,float percent)
        {
        }

        protected virtual bool OnBreak(BreakSkillFlag flag)
        {
            return true;
        }
        public abstract void OnInit(ISkillObject owner);

        protected abstract void OnDispose();

        
        public virtual void Pause()
        {
            runningAction?.OnPause();
            if (cd != null && station == SkillStation.CD)
            {
                cd.OnPause();
            }
        }

        public virtual void Continue()
        {
            runningAction?.OnContinue();
            if (cd != null && station == SkillStation.CD)
            {
                cd.OnContinue();
            }
        }

        #endregion

        #region Public

        public void OnLoadToOwner(ISkillObject owner)
        {
            this._owner = owner;
            OnInit(owner);
        }

        public void PushAction(ISkillAction action)
        {
            actions.Add(action);
        }

        public void Release(Action<bool> callback)
        {
            if (actions.Count == 0)
            {
                PushAction(new EmptyAction());
            }

            station = SkillStation.OnRelease;
            _indexAction = 0;
            OnRelesePrepare();
            actions[0].OnStart();
            OnReleaseStart();
            this._realeseEndAction = result =>
            {
                LogFormat("{0}技能{1}结束", owner, GetType().Name);
                callback?.Invoke(result);
                this._realeseEndAction = null;
            };
            LogFormat("{0} 开始释放技能 {1}", owner, GetType().Name);
        }

        public bool Break(BreakSkillFlag flag)
        {
            var temp = runningAction;
            if (temp != null)
            {
                if (OnBreak(flag))
                {
                    if ((flag & BreakSkillFlag.BreakAction) != 0)
                    {
                        temp.OnEnd(false);
                    }
                    EndAction(temp, false);
                    LogFormat("打断技能 {0}", name);
                    return true;
                }
            }
            
            return false;
        }
        
        public void OnUnload()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].Dispose();
            }

            OnDispose();
        }

        public void OnUpdate()
        {
            var temp = runningAction;
            if (temp != null)
            {
                if (temp.isEnd)
                {
                    temp.OnEnd(true);
                    EndAction(temp, true);
                }
                else
                {
                    temp.OnUpdate();
                    OnActionUpdate(temp, temp.percent);
                }
            }
            else
            {
                if (cd != null && station == SkillStation.CD)
                {
                    if (cd.isEnd)
                    {
                        EnterReady();
                    }
                    else
                    {
                        cd.OnUpdate();
                        OnCdUpdate(cd, cd.percent);
                    }
                }
            }
        }

        public T GetAction<T>(int index) where T : ISkillAction
        {
            if (actions[index] is T result)
            {
                return result;
            }

            return default;
        }

        public virtual string GetText(string type1)
        {
            return string.Empty;
        }

        public virtual void GetIcon(string type, Action<Sprite> callback)
        {
        }

        public void LogFormat(string obj, params object[] args)
        {
            owner.LogFormat(obj, args);
        }

        public override string ToString()
        {
            return GetType().Name;
        }
        
        #endregion

    }
}