using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDK
{

    public class AnalyticsSDKAndroid : AnalyticsSDKBase
    {
        AndroidJavaClass jc;
        AndroidJavaObject jo;

        public override void InitAnalytics()
        {
            base.InitAnalytics();
            jc = new AndroidJavaClass("com.chenguan.analytics.AnalyticsManager");
            jo = jc.GetStatic<AndroidJavaObject>("Instance");
            jo.Call("InitAnalytics");
        }

        public override void OnEvent(string eventName, E_AnalyticsType e_AnalyticsType)
        {
            base.OnEvent(eventName, e_AnalyticsType);
            jo.Call("OnEvent",eventName,e_AnalyticsType);
        }


        public override void OnPurchaseEvent(string eventId, float price, string currencyType, int amount, string itemType, string itemId, string cartType, string receipt, string store, string signature,E_AnalyticsType e_AnalyticsType)
        {
            base.OnPurchaseEvent(eventId, price, currencyType, amount, itemType, itemId, cartType, receipt, store, signature,e_AnalyticsType);
            jo.Call("OnPurchaseEvent", eventId, price, currencyType, amount, itemType, itemId, cartType, receipt, store, signature, e_AnalyticsType);
        }

        public override void OnLevelEvent(int status, string chapter, string model, string stageLevel,E_AnalyticsType e_AnalyticsType)
        {
            base.OnLevelEvent(status,chapter, model, stageLevel,e_AnalyticsType);
            jo.Call("OnLevelEvent",status, chapter, model, stageLevel, e_AnalyticsType);
        }
    }
}
