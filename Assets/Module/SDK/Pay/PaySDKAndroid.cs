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
            jc = new AndroidJavaClass("com.chenguan.play.GooglePlayManager");
            jo = jc.GetStatic<AndroidJavaObject>("Instance");
            jo.Call("InitIAP");
        }

        public override void Buy(string sku, string type, Action<string> succeed, Action<string> failed)
        {
            base.Buy(sku, type, succeed, failed);
            jo.Call("Purchase", sku,type);
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

        public override void ConsumeOrder(string sku)
        {
            base.ConsumeOrder(sku);
            jo.Call("ConsumeOrder",sku);
        }
    }
}
