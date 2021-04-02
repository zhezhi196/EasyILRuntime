using System;
using System.Collections.Generic;
using System.Globalization;
using SDK;
using UnityEngine;

namespace Module
{
    public abstract class Iap
    {
        #region IapResult

        private static Dictionary<IapDataBase, Iap> IapDic = new Dictionary<IapDataBase, Iap>();

        public static Iap GetIapBySku(string sku)
        {
            foreach (KeyValuePair<IapDataBase,Iap> keyValuePair in IapDic)
            {
                if (keyValuePair.Key.sku == sku)
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
        
        protected Action<IapResult> onRewardCallback;
        protected IapResult result;
        public IapDataBase dbData { get; }
        public int getCount { get; set; }

        public virtual void OnTryGetReward(Action<IapResult> callback, IapResult result, bool skipConsume)
        {
            this.onRewardCallback = callback;
            this.result = result;
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