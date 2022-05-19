using System;
using UnityEngine;

namespace Module
{
    [Flags]
    public enum IapRewardFlag
    {
        NoAudio = 1,
        NoLoading = 2,
        NoAnalysis = 4,
        Free = 8,
        NoUpdateDB = 16,
        NoPause = 32
    }

    public abstract class IapBag : IRewardBag
    {
        #region 属性字段

        protected RewardContent[] _content;
        private Iap _iap;
        private FloatField _product = new FloatField(1);

        public RewardContent[] content
        {
            get { return _content; }
        }

        public virtual int stationCode
        {
            get
            {
                for (int i = 0; i < content.Length; i++)
                {
                    if (content[i].stationCode != 0) return content[i].stationCode;
                }

                return iapState == IapState.Normal ? 0 : -1;
            }
        }

        public virtual IapState iapState
        {
            get { return iap.iapState; }
        }

        public virtual float product
        {
            get { return _product.value; }
            set { _product = new FloatField(value); }
        }

        public Iap iap
        {
            get { return _iap; }
        }

        #endregion

        #region 构造函数

        public IapBag(IapSqlData sqlData)
        {
            this._iap = Iap.GetIap(sqlData);
        }

        public IapBag(Iap iap)
        {
            this._iap = iap;
        }

        #endregion
        
        public override string ToString()
        {
            return iap.dbData.ID.ToString();
        }

        public virtual void GetIcon(string type, Action<Sprite> callback)
        {
            SpriteLoader.LoadIcon(type, callback);
        }
        
        /// <summary>
        /// 直接获取物品
        /// </summary>
        /// <param name="rewardCount"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public virtual float GetReward(float rewardCount, RewardFlag flag)
        {
            if (content != null)
            {
                for (int i = 0; i < content.Length; i++)
                {
                    content[i].GetReward(rewardCount, flag);
                }
            }

            return rewardCount;
        }
        
        public abstract int index { get; }
        
        public abstract void GetReward(Action<IapResult> callback, IapRewardFlag flag = 0);
        public abstract string GetText(string type);
    }
}