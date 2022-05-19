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

        private static Dictionary<IapSqlData, Iap> IapDic = new Dictionary<IapSqlData, Iap>();

        public static Iap GetIapBySku(string sku)
        {
            foreach (KeyValuePair<IapSqlData,Iap> keyValuePair in IapDic)
            {
                if (keyValuePair.Value is Currency currency)
                {
                    if (currency.sku == sku)
                    {
                        return keyValuePair.Value;
                    }
                }
            }

            return null;
        }

        public static Iap GetIap(IapSqlData sqldata)
        {
            Iap result = null;
            if (!IapDic.TryGetValue(sqldata, out result))
            {
                if (sqldata is ICurrencyData currData)
                {
                    result = new Currency(currData);
                }
                else if (sqldata is IAdsData ads)
                {
                    result = new AdsIap(ads, (AdsType) ads.adsType, E_InitializeAdType.Static);
                }
                else if (sqldata is ISubscribeData sub)
                {
                    result = new Subscribe(sub);
                }
                else
                {
                    result = new FreeIap(sqldata);
                }

                IapDic.Add(sqldata, result);
            }

            return result;
        }

        protected Iap(IapSqlData data)
        {
            this.dbData = data;
        }

        #endregion

        public static Action<IapResult> onResultIapBeforeCall; 
        public static Action<IapResult> onResultIapAfterCall;

        private int _getCount;
        protected IapResult result;
        public IapSqlData dbData { get; }

        /// <summary>
        /// 本次打开游戏获取本礼包的数量
        /// </summary>
        public int getCount
        {
            get { return _getCount; }
            set
            {
                var temp = _getCount;
                _getCount = value;
                totalGetCount += (_getCount - temp);
                totayGetCount += (_getCount - temp);
                
            }
        }

        /// <summary>
        /// 总共获得此礼包的数量
        /// </summary>
        public int totalGetCount
        {
            get { return LocalFileMgr.GetInt($"Iap{dbData.ID}totalCount"); }
            private set { LocalFileMgr.SetInt($"Iap{dbData.ID}totalCount", value); }
        }

        /// <summary>
        /// 今日获得此礼包的数量
        /// </summary>
        public int totayGetCount
        {
            get
            {
                if (TimeHelper.IsNewDay($"Iap{dbData.ID}todayCount"))
                {
                    totayGetCount = 0;
                    return 0;
                }
                else
                {
                    return LocalFileMgr.GetInt($"Iap{dbData.ID}todayCount");
                }
            }
            private set { LocalFileMgr.SetInt($"Iap{dbData.ID}todayCount", value); }
        }



        public IapState iapState
        {
            get { return (IapState) dbData.switchStation; }
        }

        public virtual string OnTryGetReward(Action<IapResult> callback, IapResult result, bool skipConsume)
        {
            this.result = result;
            return null;
        }
        public abstract bool CanPay();

        public T GetData<T>() where T: IapSqlData
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