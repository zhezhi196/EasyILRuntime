using System;
using System.Collections.Generic;
using System.Globalization;
using SDK;
using UnityEngine;

namespace Module
{
    public enum IapState
    {
        /// <summary>
        /// 正常生效
        /// </summary>
        Normal,
        /// <summary>
        /// 无效的
        /// </summary>
        Invalid,
        /// <summary>
        /// 跳过支付永远可用
        /// </summary>
        Skip
    }
    public abstract class Iap
    {
        #region IapResult

        private static Dictionary<IapDataBase, Iap> IapDic = new Dictionary<IapDataBase, Iap>();

        public static Iap GetIapBySku(string sku)
        {
            foreach (KeyValuePair<IapDataBase,Iap> keyValuePair in IapDic)
            {
                if (keyValuePair.Value.sku == sku)
                {
                    return keyValuePair.Value;
                }
            }

            return null;
        }

        public static Iap GetIap(IapDataBase sqldata)
        {
            Iap result = null;
            if (!IapDic.TryGetValue(sqldata, out result))
            {
                if (sqldata.consume == 0)
                {
                    result = new Currency(sqldata);
                }
                else if (sqldata.consume == 1)
                {
                    result = new AdsIap(sqldata, (AdsType) sqldata.adsType, E_InitializeAdType.Static);
                }
                else if (sqldata.consume == 2)
                {
                    result = new Subscribe(sqldata);
                }
                else if (sqldata.consume == 3)
                {
                    result = new FreeIap(sqldata);
                }

                IapDic.Add(sqldata, result);
            }

            return result;
        }

        protected Iap(IapDataBase data)
        {
            this.dbData = data;
        }

        #endregion

        public static Action<IapResult> onResultIapBeforeCall; 
        public static Action<IapResult> onResultIapAfterCall;

        
        protected IapResult result;
        public IapDataBase dbData { get; }
        public int getCount { get; set; }

        public string sku
        {
            get
            {
                switch (Channel.channel)
                {
                    case ChannelType.googlePlay:
                        return dbData.googlePlay;
                    case ChannelType.AppStore:
                        return dbData.appStore;
                    case ChannelType.AppStoreCN:
                        return dbData.appStoreCN;
                }
                GameDebug.LogError("Channel 配置错误");
                return null;
            }
        }

        public IapState iapState
        {
            get
            {
                return (IapState) dbData.switchStation;
            }
        }

        public virtual string OnTryGetReward(Action<IapResult> callback, IapResult result, bool skipConsume)
        {
            this.result = result;
            return null;
        }
        public abstract bool CanPay();

        public T GetData<T>() where T: IapDataBase
        {
            if (dbData != null)
            {
                return (T) dbData;
            }

            return default;
        }

        public override string ToString()
        {
            return dbData.ID.ToString();
        }
    }
}