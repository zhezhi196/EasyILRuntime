using System;
using UnityEngine;

namespace SDK
{
    public class PaySDKAndroid : PaySDKBase
    {
        AndroidJavaClass jc;
        AndroidJavaObject jo;


        public override void InitPaySDK()
        {
            base.InitPaySDK();
            jc = new AndroidJavaClass("com.chenguan.pay.IAPManager");
            jo = jc.GetStatic<AndroidJavaObject>("Instance");
            jo.Call("InitIAP");
        }

        public override void Buy(string sku, string key, string type, Action<string> succeed, Action<string> failed)
        {
            SDKMgr.GetInstance().Log("PaySDKAndroid  ---  Buy  sku = " + sku);
            base.Buy(sku, key, type, succeed, failed);
            jo.Call("Purchase", sku, type, key);
        }

        public override void QuerySkuDetails(string type, Action<string> goodsDetails)
        {
            base.QuerySkuDetails(type, goodsDetails);
            jo.Call("QuerySkuDetails", type);
        }
       

        public override bool IsSubscribe()
        {
            return jo.Call<bool>("IsSubscribe");
        }
       
        public override void QueryPurchases(string itemType,Action<string> recoveryOrder)
        {
            base.QueryPurchases(itemType,recoveryOrder);
            jo.Call("QueryPurchases", itemType);
        }

   
        public override void ConsumeOrder(string sku, Action<string> consumer)
        {
            base.ConsumeOrder(sku, consumer);
            jo.Call("ConsumeOrder", sku);
        }
    }
}
