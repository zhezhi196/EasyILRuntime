using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDK
{
    public class PaySDKBase
    {
        /// <summary>
        /// 支付成功回调
        /// </summary>
        public Dictionary<string, Action<string>> PaySucceedCallBack = new Dictionary<string, Action<string>>();

        /// <summary>
        /// 支付失败回调
        /// </summary>
        public Dictionary<string,Action<string>> PayFailedCallBack = new Dictionary<string, Action<string>>();
        /// <summary>
        /// 查询是否有掉单恢复回调
        /// </summary>
        public Action<string> RecoveryOrderCallBack = null;
        /// <summary>
        /// 查询商品详情回调
        /// </summary>
        public Action<string> QueryGoodsDetailsCallBack = null;
        /// <summary>
        /// 消耗商品回调
        /// </summary>
        public Action<string> ConsumeOrderCallBack = null;

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void InitPaySDK()
        {

        }

        /// <summary>
        /// 购买
        /// </summary>
        /// <param name="sku">商品ID</param>
        /// <param name="type">商品类型(消耗(inApp),订阅(subs))</param>
        /// <param name="succeed"></param>
        /// <param name="failed"></param>
        public virtual void Buy(string sku,string key, string type, Action<string> succeed, Action<string> failed)
        {
            if (succeed != null && failed != null)
            {
                PaySucceedCallBack.Add(key,succeed);
                PayFailedCallBack.Add(key, failed);
            }
        }

        /// <summary>
        /// 查询商品详情
        /// </summary>
        /// <param name="type">商品类型(消耗(inApp),订阅(subs))</param>
        public virtual void QuerySkuDetails(string type,Action<string> goodsDetails)
        {
            if (goodsDetails != null)
            {
                QueryGoodsDetailsCallBack += goodsDetails;
            }
        }

        /// <summary>
        /// 当前是否是订阅
        /// </summary> RecoveryPlayerDropPay
        /// <returns></returns>
        public virtual bool IsSubscribe()
        {
            return false;
        }

        /// <summary>
        /// 查询已经购买且未消耗的商品
        /// </summary>
        /// <param name="itemType">商品类型(消耗(inApp),订阅(subs))</param>
        public virtual void QueryPurchases(string itemType, Action<string> recoveryOrder)
        {
            if (recoveryOrder != null)
            {
                RecoveryOrderCallBack = recoveryOrder;
                //RecoveryOrderCallBack?.Invoke("ds|endless.nightmare.weird.hospital.horror.scary.free.android.part.499");
            }
        }

        /// <summary>
        /// 消耗商品
        /// </summary>
        /// <param name="sku"></param>
        public virtual void ConsumeOrder(string sku,Action<string> consumer)
        {
            if (consumer != null)
            {
                ConsumeOrderCallBack = consumer;
                //ConsumeOrderCallBack?.Invoke("");
            }
        }

        /// <summary>
        /// 购买回调
        /// </summary>
        ///                             0              |1  |2   |3    |4         |5      |6
        /// <param name="state">true(成功)或false(失败)|sku|time|token|Signature|orderID|KEY</param>
        public void OnBuyCallBack(string state)
        {
            SDKMgr.GetInstance().Log("PaySDKBase  OnBuyCallBack --- " + state);
            string[] tempStr = state.Split('|');
            bool result = Convert.ToBoolean(tempStr[0]);

            if (PaySucceedCallBack != null && result)
            {
                PaySucceedCallBack[tempStr[6]].Invoke(state);
                PaySucceedCallBack.Remove(tempStr[6]);
            }
            else if (PayFailedCallBack != null && !result)
            {
                PayFailedCallBack[tempStr[6]].Invoke(state);
                PayFailedCallBack.Remove(tempStr[6]);
            }
        }

        /// <summary>
        /// 查询商品是否有掉单回调
        /// </summary>
        /// <param name="state">sku</param>
        public void OnRecoveryOrder(string state)
        {
           
            if (RecoveryOrderCallBack != null)
            {
                RecoveryOrderCallBack?.Invoke(state);
                
            }
        }

        /// <summary>
        /// 消耗商品回调
        /// </summary>
        /// <param name="state">true(成功)或false(失败)|sku</param>
        public void OnConsumeOrder(string state)
        {
            if (ConsumeOrderCallBack != null)
            {
                ConsumeOrderCallBack?.Invoke(state);
            }

        }

        /// <summary>
        /// 查询商品详细回调
        /// </summary>
        /// <param name="state">price|sku|description</param>
        public void QueryGoodsDetails(string state)
        {
            if (QueryGoodsDetailsCallBack != null)
            {
                QueryGoodsDetailsCallBack?.Invoke(state);
                
            }
        }
    }

}
