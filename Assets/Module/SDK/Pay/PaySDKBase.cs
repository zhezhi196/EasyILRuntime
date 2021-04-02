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
        public Action<string> PaySucceedCallBack = null;
        /// <summary>
        /// 支付失败回调
        /// </summary>
        public Action<string> PayFailedCallBack = null;
        /// <summary>
        /// 掉单恢复回调
        /// </summary>
        public Action<string> RecoveryOrderCallBack = null;
        /// <summary>
        /// 查询商品详情回调
        /// </summary>
        public Action<string> QueryGoodsDetailsCallBack = null;

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
        public virtual void Buy(string sku,string type,Action<string> succeed,Action<string> failed)
        {
            if (succeed !=null && failed != null)
            {
                PaySucceedCallBack = succeed;
                PayFailedCallBack = failed;
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
                QueryGoodsDetailsCallBack = goodsDetails;
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
            }
        }

        /// <summary>
        /// 消耗商品
        /// </summary>
        /// <param name="sku"></param>
        public virtual void ConsumeOrder(string sku)
        {

        }

        /// <summary>
        /// 购买回调
        /// </summary>
        ///                             0              |1  |2   |3    |4         |5
        /// <param name="state">true(成功)或false(失败)|sku|time|token|Signature|orderID</param>
        public void OnBuyCallBack(string state)
        {
            SDKMgr.GetInstance().Log("PaySDKBase  OnBuyCallBack --- " + state);
            string[] tempStr = state.Split('|');
            bool result = Convert.ToBoolean(tempStr[0]);

            if (PaySucceedCallBack != null && result)
            {
                PaySucceedCallBack(state);
                PaySucceedCallBack = null;
            }
            else if (PayFailedCallBack != null && !result)
            {
                PayFailedCallBack(state);
                PayFailedCallBack = null;
            }
        }

        /// <summary>
        /// 掉单恢复回调
        /// </summary>
        /// <param name="state">OrderId|sku</param>
        public void OnRecoveryOrder(string state)
        {
           
            if (RecoveryOrderCallBack != null)
            {
                RecoveryOrderCallBack(state);
                RecoveryOrderCallBack = null;
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
                QueryGoodsDetailsCallBack(state);
                QueryGoodsDetailsCallBack = null;
            }
        }
    }

}
