using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDK
{
    public class CommonAndroid : CommonBase
    {
        AndroidJavaClass jc;
        AndroidJavaObject jo;

        public override void InitCommon()
        {
            base.InitCommon();
            jc = new AndroidJavaClass("com.chenguan.common.CommonManager");
            jo = jc.GetStatic<AndroidJavaObject>("Instance");
          
        }

        public override void HideSplash()
        {
            base.HideSplash();
            jo.Call("HideSplash");
           
        }

        public override void OpenAPPMarket()
        {
            base.OpenAPPMarket();
            jo.Call("OpenAPPMarket");
        }

        public override void ShowPrivacyPolicy()
        {
            base.ShowPrivacyPolicy();
            jo.Call("ShowPrivacyPolicy");
        }

        public override bool IsSDKLog()
        {
            return jo.Call<bool>("IsSDKLog");
        }
    }
}
