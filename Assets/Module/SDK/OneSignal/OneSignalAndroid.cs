using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDK
{
    public class OneSignalAndroid : OneSignalBase
    {
        AndroidJavaClass jc;
        AndroidJavaObject jo;
        public override void InitOneSignal()
        {
            jc = new AndroidJavaClass("com.chenguan.oneSignal.OneSignalManager");
            jo = jc.GetStatic<AndroidJavaObject>("Instance");
            jo.Call("InitOneSignalSDK");
        }

        public override void PostNotification(string language, string message, string title, long delay)
        {
            jo.Call("PostNotification",language, message, title,delay);
        }

        public override void CancelNotification()
        {
            jo.Call("CancelNotification");
        }
    }
}
