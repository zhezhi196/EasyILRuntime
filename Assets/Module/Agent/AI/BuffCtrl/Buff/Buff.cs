using System;
using UnityEngine;

namespace Module
{
    [Serializable]
    public abstract class Buff: IRewardObject
    {
        protected IBuffObject _owner;
        private int _layCount;
        private bool _isActive;
        private BuffType _type;
        private bool _isPause;
        public float time;
        public abstract string name { get; }
        public int intervalIndex;
        public float totalIntervaltime;
        public BuffOption option;

        public IBuffObject owner
        {
            get { return _owner; }
        }
        public int stationCode { get; }

        /// <summary>
        /// buff层数
        /// </summary>
        public int layCount
        {
            get { return _layCount; }
        }

        /// <summary>
        /// buff是否激活
        /// </summary>
        public bool isActive
        {
            get { return _isActive && !_isPause; }
        }

        /// <summary>
        /// buff类型
        /// </summary>
        public BuffType type
        {
            get { return _type; }
        }

        #region Callback

        public virtual void OnInit(IBuffObject owner, BuffType type, object[] args)
        {
            this._owner = owner;
            this._type = type;
        }
        
        public virtual void OnAdd()
        {
            _isActive = true;
        }

        public virtual void OnRemove()
        {
            _layCount = 0;
            _isActive = false;

        }

        #endregion

        #region Method

        public bool OnUpdate(float detaTime)
        {
            if (isActive && !_isPause)
            {
                time += detaTime;
                if (time >= option.totalTime)
                {
                    Stop();
                    return false;
                }
                else
                {
                    if (!option.interval.IsNullOrEmpty())
                    {
                        var intew = option.interval[intervalIndex % option.interval.Length];
                        if (time >= totalIntervaltime + intew)
                        {
                            totalIntervaltime += intew;
                            OnInterval(intervalIndex);
                            intervalIndex++;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        protected virtual void OnInterval(int index)
        {
        }

        /// <summary>
        /// 重新开始
        /// </summary>
        /// <param name="option"></param>
        public virtual void Restart(BuffOption option)
        {
            this.option = option;
            totalIntervaltime = 0;
            intervalIndex = 0;
            time = 0;
        }

        private void OnComplteBuff()
        {
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public virtual void Pause()
        {
            _isPause = true;
        }
        /// <summary>
        /// buff继续
        /// </summary>
        public virtual void Continue()
        {
            _isPause = false;
        }
        /// <summary>
        /// buff停止
        /// </summary>
        public virtual void Stop()
        {
            _isActive = false;
        }

        #endregion

        public virtual void GetIcon(string type, Action<Sprite> callback)
        {
        }

        public virtual string GetText(string type)
        {
            return null;
        }

        public virtual float GetReward(float rewardCount, RewardFlag flag)
        {
            return rewardCount;
        }
    }
}