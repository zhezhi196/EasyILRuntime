using UnityEngine;


namespace SDK
{

    public enum E_PayType
    {
        /// <summary>
        /// 消耗
        /// </summary>
        inapp = 0,
        /// <summary>
        /// 订阅
        /// </summary>
        subs
    }


    public class PayUACommunication : MonoBehaviour
    {
        /// <summary>
        /// 购买回调成功/失败
        /// </summary>
        ///                              0              |1  |2   |3    |4         |5
        /// <param name="state">true(成功)或false(失败)|sku|time|token|Signature|orderID</param>
        public void PayCallBack(string state)
        {
            SDKMgr.GetInstance().Log("PayUACommunication  PayCallBack --- state = " + state);
            SDKMgr.GetInstance().MyPaySDK.OnBuyCallBack(state);
        }

        /// <summary>
        /// 掉单恢复
        /// </summary>
        /// <param name="sku">sku</param>
        public void RecoveryOrder(string sku)
        {
            SDKMgr.GetInstance().Log("PayUACommunication  RecoveryOrder --- sku = " + sku);
            SDKMgr.GetInstance().MyPaySDK.OnRecoveryOrder(sku);
        }

        /// <summary>
        /// 查询商品详情
        /// </summary>
        /// <param name="state">price|sku|description</param>
        public void QuerySkuDetails(string state)
        {
            SDKMgr.GetInstance().MyPaySDK.QueryGoodsDetails(state);
        }
    }

    
}