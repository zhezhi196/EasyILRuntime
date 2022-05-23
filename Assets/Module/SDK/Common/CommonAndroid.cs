using System;
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
            AndroidJavaClass jcc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject joo = jcc.GetStatic<AndroidJavaObject>("currentActivity");
            joo.Call("HideSplash");
           
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

        public override string GetKeyStoreMD5()
        {
            string sign = jo.Call<string>("GetSign");
            SDKMgr.GetInstance().Log("CommonAndroid  GetKeyStoreMD5 = " + sign);
            return sign;
        }

        public override string GetUDID()
        {
            return jo.Call<string>("GetUdID");
        }

        public override void SetBrightness(int brightness)
        {
            jo.Call("SetBrightness", brightness);
        }

        public override void OpenInput(string text, Action<string> onValueChange, Action<bool> onKeybordShowChange)
        {
            base.OpenInput(text, onValueChange, onKeybordShowChange);
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call("OpenInput", text);
        }

        public override void CopyData(string state)
        {
            jo.Call("CopyData",state);

        }
    }
}
